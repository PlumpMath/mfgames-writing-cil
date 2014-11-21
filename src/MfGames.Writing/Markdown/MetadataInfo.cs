// <copyright file="MetadataInfo.cs" company="Moonfire Games">
//     Copyright (c) Moonfire Games. Some Rights Reserved.
// </copyright>
// MIT Licensed (http://opensource.org/licenses/MIT)
namespace MfGames.Writing.Markdown
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Encapsulates the parsing and generation logic for common header elements
    /// in Markup files, such as copyright, author, and titles.
    /// </summary>
    public class MetadataInfo
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataInfo"/> class.
        /// </summary>
        public MetadataInfo()
        {
            this.CopyrightYears = new List<int>();
            this.Schemes = new Dictionary<string, List<string>>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// </summary>
        public string CopyrightHolder { get; set; }

        /// <summary>
        /// </summary>
        public List<int> CopyrightYears { get; private set; }

        /// <summary>
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// </summary>
        public Dictionary<string, List<string>> Schemes { get; private set; }

        /// <summary>
        /// </summary>
        public string Title { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Adds a key/value pair to the metadata, replacing any parsed element
        /// with the new value.
        /// </summary>
        /// <param name="key">
        /// </param>
        /// <param name="value">
        /// </param>
        public void Add(
            string key, 
            string value)
        {
            // Establish our contracts.
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            // We use case-insensitive key searching.
            string searchKey = key.ToLower();

            // Figure out what to do based on the key. In some cases, we only get a single
            // key while others we do some additional parsing.
            value = value.Trim();

            switch (searchKey)
            {
                case "title":
                    this.Title = value.Trim();
                    break;

                case "author":
                    this.ParseAuthor(value);
                    break;

                case "copyright":
                    this.ParseCopyright(value);
                    break;

                case "date":
                    this.ParseDate(value);
                    break;

                default:
                    this.ParseSchemes(key, value);
                    break;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="objects">
        /// </param>
        /// <exception cref="Exception">
        /// </exception>
        public void AddYamlObjects(object objects)
        {
            // If this is a dictionary, then process it as such.
            var dictionary = objects as Dictionary<object, object>;

            if (dictionary != null)
            {
                // Go through the keys of of this object.
                IEnumerable<string> keys = dictionary.Keys.OfType<string>();

                foreach (string key in keys)
                {
                    // Get the string value for this key/value pair.
                    var value = dictionary[key] as string;

                    if (value == null)
                    {
                        throw new Exception(
                            "Cannot read metadata element: "
                                + dictionary[key].GetType() + ".");
                    }

                    // Add the value to the metadata.
                    this.Add(key, value);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="normalized">
        /// </param>
        private void ParseAuthor(string normalized)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="value">
        /// </param>
        private void ParseCopyright(string value)
        {
            // Copyright consists of zero or more years followed by
            // a copyright name. In theory, there could be commas with the
            // name, so we stop parsing after the first non-integer name.
            List<string> values =
                value.Split(',').Select(t => t.Trim()).ToList();
            int index;

            this.CopyrightYears = new List<int>();

            for (index = 0; index < values.Count; index++)
            {
                // Try to parse it as a year.
                int year;

                if (int.TryParse(values[index], out year))
                {
                    this.CopyrightYears.Add(year);
                    continue;
                }

                // It isn't parsable as a year, so break out.
                break;
            }

            // Parse everything else as the name.
            var builder = new StringBuilder();

            for (; index < values.Count; index++)
            {
                builder.AppendFormat("{0}, ", values[index]);
            }

            // Clean up the holder and save it, but after we remove the trailing
            // comma and spaces.
            string holder = builder.ToString().TrimEnd().TrimEnd(',');

            this.CopyrightHolder = holder;
        }

        /// <summary>
        /// </summary>
        /// <param name="value">
        /// </param>
        private void ParseDate(string value)
        {
            DateTime parsedDate;

            if (DateTime.TryParse(value, out parsedDate))
            {
                this.Date = parsedDate;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="key">
        /// </param>
        /// <param name="value">
        /// </param>
        private void ParseSchemes(
            string key, 
            string value)
        {
            // If we have a semicolon, then split on that. Otherwise,
            // check to see if we have comma-separated items.
            List<string> terms;

            if (value.Contains(';'))
            {
                terms = value
                    .Split(';')
                    .Select(t => t.Trim())
                    .ToList();
            }
            else
            {
                terms = value
                    .Split(',')
                    .Select(t => t.Trim())
                    .ToList();
            }

            // Save the value in the key set.
            this.Schemes[key] = terms;
        }

        #endregion
    }
}