// <copyright file="ConvertToDocBookProcess.cs" company="Moonfire Games">
//     Copyright (c) Moonfire Games. Some Rights Reserved.
// </copyright>
// MIT Licensed (http://opensource.org/licenses/MIT)
namespace MfGames.Writing.Markdown
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;

    using MfGames.Text.Markup;
    using MfGames.Text.Markup.Markdown;
    using MfGames.Xml;

    using YamlDotNet.Serialization;

    /// <summary>
    /// Defines a process for converting Markdown into DocBook 5 XML files.
    /// </summary>
    public class ConvertToDocBookProcess : ProcessBase
    {
        #region Fields

        /// <summary>
        /// </summary>
        private string rootElement;

        private string paragraphElement;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertToDocBookProcess"/> class.
        /// </summary>
        public ConvertToDocBookProcess()
        {
            this.rootElement = "article";
            this.paragraphElement = "para";
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the input.
        /// </summary>
        /// <value>
        /// The input.
        /// </value>
        public TextReader Input { get; set; }

        /// <summary>
        /// Gets or sets the output.
        /// </summary>
        /// <value>
        /// The output.
        /// </value>
        public TextWriter Output { get; set; }
        /// <summary>
        /// Gets or sets the output settings.
        /// </summary>
        /// <value>
        /// The output settings.
        /// </value>
        public XmlWriterSettings OutputSettings { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether attributions should be parsed out of
        /// blockquotes. An attribution is separated by the final "---" in the final
        /// paragraph of a blockquote.
        /// </summary>
        public bool ParseAttributions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether backtics ('`') should be converted
        /// into DocBook foreignphrase elements.
        /// </summary>
        public bool ParseBackticks { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to parse commented regions in the
        /// files. A comment is defined as any line starts with an octothorpe ('#').
        /// </summary>
        public bool ParseComments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a blockquote immediatelly following a
        /// heading is converted into a formal epigraph instead of remaining a blockquote.
        /// </summary>
        public bool ParseEpigraphs { get; set; }

        /// <summary>
        /// Gets or sets whether quotes and foreignphrases are parsed to identify
        /// a language code. If they are, they are put into the xml:lang attribute of
        /// the resulting element. Language codes are a ISO 639 code followed by a colon.
        /// For example "en: English" or "fra-que: French".
        /// </summary>
        public bool ParseLanguages { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether notes and special paragraphs are
        /// converted into the appropriate docbook elements. These are identified as having
        /// "Important:", "Note:", "Tip:", and "Warning:" in the beginning of the paragraph.
        /// The case of the text is not important. Sequential paragraphs of the same type are
        /// combined into a single one unless the exclaimation instead of a colon is used
        /// (for example, "Note! ").
        /// </summary>
        public bool ParseNotes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether flag to determine if quotes should be
        /// parsed either into ASCII, Unicode, or DocBook elements.
        /// </summary>
        public bool ParseQuotes { get; set; }

        /// <summary>
        /// Gets or sets the top-level DocBook element for the parsed file. This defaults
        /// to "article".
        /// </summary>
        public string RootElement
        {
            get
            {
                return this.rootElement;
            }

            set
            {
                this.rootElement = value ?? "article";
            }
        }

        /// <summary>
        /// Gets or sets the "xml:id" attribute set at the top-level of the resulting
        /// document.
        /// </summary>
        public string XmlId { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Runs this process and performs the appropriate actions.
        /// </summary>
        public override void Run()
        {
            // Verify that the input file exists since if we can't, it is
            // meaningless to continue.
            if (this.Input == null)
                throw new Exception("Input was not properly set to a value.");

            // Open up a handle to the Markdown file that we are processing. This uses an
            // event-based reader to allow us to write the output file easily.
            using (var markdownReader = new MarkdownReader(Input))
            {
                // We also need an XML writer for the resulting file.
                using (XmlWriter xmlWriter = this.CreateXmlWriter())
                {
                    // Convert the Markdown into XML.
                    this.ConvertMarkdown(
                        markdownReader,
                        xmlWriter);
                }
            }
        }

        /// <summary>
        /// Converts the given Markdown stream into an appropriate DocBook 5 XML.
        /// </summary>
        /// <param name="markdown"></param>
        /// <param name="xml"></param>
        public void ConvertMarkdown(
            MarkdownReader markdown,
            XmlWriter xml)

        {
            // Ensure our contracts.
            if (markdown == null)
            {
                throw new ArgumentNullException("markdown");
            }

            if (xml == null)
            {
                throw new ArgumentNullException("xml");
            }

            // Loop through the Markdown and process each one.
            while (markdown.Read())
            {
                switch (markdown.ElementType)
                {
                    case MarkupElementType.BeginDocument:
                        xml.WriteStartDocument(true);
                        xml.WriteStartElement(
                            rootElement,
                            XmlConstants.DocBookNamespace5);
                        xml.WriteAttributeString("version", "5");
                        break;

                    case MarkupElementType.EndDocument:
                        xml.WriteEndElement();
                        xml.WriteEndDocument();
                        break;

                    case MarkupElementType.BeginMetadata:
                    case MarkupElementType.EndMetadata:
                        break;

                    case MarkupElementType.YamlMetadata:
                        var deserializer = new Deserializer();
                        string text = markdown.Text;
                        text = text.TrimEnd()
                            .TrimEnd('-')
                            .TrimEnd();
                        var stringReader = new StringReader(text);

                        object metadata = deserializer.Deserialize(stringReader);
                        WriteInfoElement(
                            xml,
                            metadata);
                        break;

                    case MarkupElementType.BeginParagraph:
                        xml.WriteStartElement(paragraphElement);
                        break;

                    case MarkupElementType.Text:
                        xml.WriteString(markdown.Text);
                        break;

                        case MarkupElementType.EndParagraph:
                        xml.WriteEndElement();
                        break;

                    default:
                        Console.WriteLine("Cannot process: " + markdown.ElementType);
                        break;
                }
            }
        }

        private void WriteInfoElement(XmlWriter xml,
            object metadata)
        {
            // If we have a null or blank, then skip it.
            if (metadata == null) return;

            // If this is a dictionary, then process it.
            var dictionary = metadata as Dictionary<object, object>;

            if (dictionary != null)
            {
                foreach (var entry in dictionary)
                {
                    if (Convert.ToString(entry.Key) == "title")
                    {
                        xml.WriteElementString(
                            "title",
                            Convert.ToString(entry.Value));
                    }
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the XML writer to the output file.
        /// </summary>
        /// <returns>
        /// </returns>
        private XmlWriter CreateXmlWriter()
        {
            // Create an appropriate output stream for the file.
            var writingSettings = OutputSettings;
            
            if (writingSettings == null)
                writingSettings = new XmlWriterSettings
                 {
                     OmitXmlDeclaration = true
                 };
            XmlWriter xmlWriter = XmlWriter.Create(
                Output, 
                writingSettings);
            return xmlWriter;
        }

        #endregion
    }
}