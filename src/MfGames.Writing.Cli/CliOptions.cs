// <copyright file="CliOptions.cs" company="Moonfire Games">
//     Copyright (c) Moonfire Games. Some Rights Reserved.
// </copyright>
// MIT Licensed (http://opensource.org/licenses/MIT)
namespace MfGames.Writing.Cli
{
    using CommandLine;

    using MfGames.Writing.Cli.DocBook;
    using MfGames.Writing.Cli.Markdown;

    /// <summary>
    /// Defines the command-line arguments.
    /// </summary>
    internal class CliOptions
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the options for the sub-command form transforming
        /// files from one format to another.
        /// </summary>
        [VerbOption("docbook-gather", 
            HelpText =
                "Gathers all dependencies for a DocBook file into a single one."
            )]
        public GatherDocBookOptions GatherDocBookOptions { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Gets or sets the options for the sub-command form transforming
        /// files from one format to another.
        /// </summary>
        /// <returns>
        /// </returns>
        [VerbOption("convert-markdown-docbook", 
            HelpText = "Converts a Markdown file into a DocBook one.")]
        public ConvertMarkdownToDocBookOptions ConvertMarkdownToDocBook
        {
            get;
            set;
        }

        #endregion
    }
}