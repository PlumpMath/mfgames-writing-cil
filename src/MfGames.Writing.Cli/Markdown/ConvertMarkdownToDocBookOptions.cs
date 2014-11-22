// <copyright file="ConvertMarkdownToDocBookOptions.cs" company="Moonfire Games">
//     Copyright (c) Moonfire Games. Some Rights Reserved.
// </copyright>
// MIT Licensed (http://opensource.org/licenses/MIT)
namespace MfGames.Writing.Cli.Markdown
{
    using System.Collections.Generic;
    using System.IO;

    using CommandLine;

    using MfGames.Writing.Markdown;

    /// <summary>
    /// Contains and manages the command-line request for converting Markdown to
    /// DocBook. This only creates single document files, such as a chapter or
    /// article.
    /// </summary>
    internal class ConvertMarkdownToDocBookOptions : IProcessOptions
    {
        #region Public Properties

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
        /// Gets or sets the remaining arguments which includes the input and output file.
        /// </summary>
        [ValueList(typeof(List<string>), MaximumElements = 2)]
        public IList<string> RemainingArguments { get; set; }

        /// <summary>
        /// Gets or sets the top-level DocBook element for the parsed file. This defaults
        /// to "article".
        /// </summary>
        public string RootElement { get; set; }

        /// <summary>
        /// Gets or sets the "xml:id" attribute set at the top-level of the resulting
        /// document.
        /// </summary>
        public string XmlId { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Retrieves an IProcess represented by the options.
        /// </summary>
        /// <returns>
        /// A process object associated with the options.
        /// </returns>
        public ProcessBase GetProcess()
        {
            var process = new ConvertToDocBookProcess
                {
                    ParseAttributions = this.ParseAttributions, 
                    ParseBackticks = this.ParseBackticks, 
                    ParseEpigraphs = this.ParseEpigraphs, 
                    ParseLanguages = this.ParseLanguages, 
                    ParseNotes = this.ParseNotes, 
                    ParseQuotes = this.ParseQuotes, 
                    RootElement = this.RootElement, 
                    XmlId = this.XmlId, 
                    Input =
                        new StreamReader(
                            File.OpenRead(this.RemainingArguments[0])), 
                    Output =
                        new StreamWriter(
                            File.OpenWrite(this.RemainingArguments[1])), 
                };

            return process;
        }

        #endregion
    }
}