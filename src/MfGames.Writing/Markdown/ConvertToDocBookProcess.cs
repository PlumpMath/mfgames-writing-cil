// <copyright file="ConvertToDocBookProcess.cs" company="Moonfire Games">
//     Copyright (c) Moonfire Games. Some Rights Reserved.
// </copyright>
// MIT Licensed (http://opensource.org/licenses/MIT)
namespace MfGames.Writing.Markdown
{
    using System.IO;
    using System.Xml;

    using MfGames.Writing.DocBook;
    using MfGames.Xml;

    /// <summary>
    /// Defines a process for converting Markdown into DocBook 5 XML files.
    /// </summary>
    public class ConvertToDocBookProcess : ProcessBase
    {
        #region Fields

        /// <summary>
        /// </summary>
        private string rootElement;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertToDocBookProcess"/> class.
        /// </summary>
        public ConvertToDocBookProcess()
        {
            this.rootElement = "article";
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the input file which is in DocBook 5 format.
        /// </summary>
        public FileInfo InputFile { get; set; }

        /// <summary>
        /// Gets or sets the output directory for the media files. References
        /// in the resulting XML will be relative from the OutputFile to
        /// this directory.
        /// </summary>
        public DirectoryInfo OutputDirectory { get; set; }

        /// <summary>
        /// Gets or sets the output file, which will be a DocBook format.
        /// </summary>
        public FileInfo OutputFile { get; set; }

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
            // Verify data.
            if (this.OutputDirectory == null)
            {
                this.OutputDirectory = this.OutputFile.Directory;
            }

            // Verify that the input file exists since if we can't, it is
            // meaningless to continue.
            if (!this.InputFile.Exists)
            {
                throw new FileNotFoundException(
                    "Cannot find input file.", 
                    this.InputFile.FullName);
            }

            // We use the identity XML writer to copy out the resulting XML
            // stream but use an intelligent loader to do all the work. This
            // way, we can reuse much of the IO functionality without
            // cluttering our code.
            using (XmlReader xmlReader = this.CreateXmlReader())
            using (XmlIdentityWriter xmlWriter = this.CreateXmlWriter())
            {
                // Using the identity XML writer will go through the entire
                // reader XML and write it out. This will cause the gathering
                // reader to load in the various XInclude elements and also
                // copy the media files to the appropriate place.
                xmlWriter.Load(xmlReader);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the XML reader for loading the XML file.
        /// </summary>
        /// <returns>
        /// </returns>
        private XmlReader CreateXmlReader()
        {
            // Get an appropriate stream to the input file.
            FileStream stream = this.InputFile.Open(
                FileMode.Open, 
                FileAccess.Read, 
                FileShare.Read);
            XmlReader xmlReader = XmlReader.Create(
                stream, 
                new XmlReaderSettings(), 
                this.InputFile.FullName);

            // Set up the gathering XML reader.
            var gatheringReader = new GatheringReader(xmlReader)
                {
                    OutputFile = this.OutputFile, 
                    OutputDirectory = this.OutputDirectory
                };
            return gatheringReader;
        }

        /// <summary>
        /// Creates the XML writer to the output file.
        /// </summary>
        /// <returns>
        /// </returns>
        private XmlIdentityWriter CreateXmlWriter()
        {
            // Create an appropriate output stream for the file.
            FileStream stream = this.OutputFile.Open(
                FileMode.Create, 
                FileAccess.Write);
            var writingSettings = new XmlWriterSettings
                {
                    OmitXmlDeclaration = true
                };
            XmlWriter xmlWriter = XmlWriter.Create(
                stream, 
                writingSettings);
            var identityWriter = new XmlIdentityWriter(xmlWriter);

            return identityWriter;
        }

        #endregion
    }
}