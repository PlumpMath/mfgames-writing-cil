// <copyright file="ConvertToDocBookProcess.cs" company="Moonfire Games">
//     Copyright (c) Moonfire Games. Some Rights Reserved.
// </copyright>
// MIT Licensed (http://opensource.org/licenses/MIT)
namespace MfGames.Writing.Markdown
{
    using System;
    using System.IO;
    using System.Linq;
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
        private readonly string paragraphElement;

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
        /// Gets or sets a flag whether the title element should be
        /// outside of the info tag.
        /// </summary>
        public bool TitleOutsideInfo { get; set; }

        /// <summary>
        /// Gets or sets the "xml:id" attribute set at the top-level of the resulting
        /// document.
        /// </summary>
        public string XmlId { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Converts the given Markdown stream into an appropriate DocBook 5 XML.
        /// </summary>
        /// <param name="markdown">
        /// </param>
        /// <param name="xml">
        /// </param>
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
                            this.rootElement, 
                            XmlNamespaces.DocBook5);
                        xml.WriteAttributeString(
                            "version", 
                            "5.0");
                        break;

                    case MarkupElementType.EndDocument:
                        xml.WriteEndElement();
                        xml.WriteEndDocument();
                        break;

                    case MarkupElementType.BeginMetadata:
                    case MarkupElementType.EndMetadata:
                    case MarkupElementType.BeginContent:
                    case MarkupElementType.EndContent:
                        break;

                    case MarkupElementType.BeginCodeSpan:
                        this.WriteForeignPhrase(markdown, xml);
                        break;

                    case MarkupElementType.BeginBold:
                        this.WriteBold(markdown, xml);
                        break;

                    case MarkupElementType.BeginItalic:
                        this.WriteItalic(markdown, xml);
                        break;

                    case MarkupElementType.EndCodeSpan:
                    case MarkupElementType.EndBold:
                    case MarkupElementType.EndItalic:
                        xml.WriteEndElement();
                        break;

                    case MarkupElementType.YamlMetadata:
                        this.WriteYamlMetadata(markdown, xml);
                        break;

                    case MarkupElementType.BeginParagraph:
                        xml.WriteStartElement(this.paragraphElement);
                        break;

                    case MarkupElementType.Text:
                        xml.WriteString(markdown.Text);
                        break;

                    case MarkupElementType.EndParagraph:
                        xml.WriteEndElement();
                        break;

                    case MarkupElementType.HorizontalRule:
                        this.WriteBreak(markdown, xml);
                        break;

                    default:
                        Console.WriteLine(
                            "Cannot process: " + markdown.ElementType);
                        break;
                }
            }
        }

        /// <summary>
        /// Runs this process and performs the appropriate actions.
        /// </summary>
        public override void Run()
        {
            // Verify that the input file exists since if we can't, it is
            // meaningless to continue.
            if (this.Input == null)
            {
                throw new Exception("Input was not properly set to a value.");
            }

            // Open up a handle to the Markdown file that we are processing. This uses an
            // event-based reader to allow us to write the output file easily.
            using (var markdownReader = new MarkdownReader(this.Input))
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

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="xml">
        /// </param>
        /// <param name="metadata">
        /// </param>
        private static void WriteMetadataCopyright(
            XmlWriter xml, 
            MetadataInfo metadata)
        {
            // If we don't have a year or holder, then skip it.
            bool hasHolder = string.IsNullOrWhiteSpace(metadata.CopyrightHolder);
            bool hasYears = metadata.CopyrightYears.Count == 0;

            if (hasYears || hasHolder)
            {
                return;
            }

            // Write out the copyright element.
            xml.WriteStartElement("copyright");

            // Write out the years first.
            foreach (int year in metadata.CopyrightYears)
            {
                xml.WriteElementString("year", year.ToString());
            }

            // Write out the holder next.
            if (!hasHolder)
            {
                xml.WriteElementString("holder", metadata.CopyrightHolder);
            }

            // Finish off the copyright.
            xml.WriteEndElement();
        }

        /// <summary>
        /// </summary>
        /// <param name="xml">
        /// </param>
        /// <param name="metadata">
        /// </param>
        private static void WriteMetadataDate(
            XmlWriter xml, 
            MetadataInfo metadata)
        {
            if (metadata.Date.HasValue)
            {
                xml.WriteElementString(
                    "date", 
                    metadata.Date.Value.ToString("yyyy-MM-dd"));
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="xml">
        /// </param>
        /// <param name="metadata">
        /// </param>
        private static void WriteMetadataSchemes(
            XmlWriter xml, 
            MetadataInfo metadata)
        {
            // If we don't have at least one scheme, skip it.
            if (metadata.Schemes.Count == 0)
            {
                return;
            }

            // Write out each scheme in order.
            IOrderedEnumerable<string> keys =
                metadata.Schemes.Keys.OrderBy(s => s);

            foreach (string key in keys)
            {
                // Write out the beginning of the subject set.
                xml.WriteStartElement("subjectset");
                xml.WriteAttributeString("scheme", key);
                xml.WriteStartElement("subject");

                // Write out all the terms.
                IOrderedEnumerable<string> terms =
                    metadata.Schemes[key].OrderBy(s => s);

                foreach (string term in terms)
                {
                    xml.WriteElementString("subjectterm", term);
                }

                // Finish up the subjectset.
                xml.WriteEndElement();
                xml.WriteEndElement();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="xml">
        /// </param>
        /// <param name="metadata">
        /// </param>
        private static void WriteMetadataSummary(
            XmlWriter xml, 
            MetadataInfo metadata)
        {
            if (!string.IsNullOrWhiteSpace(metadata.Summary))
            {
                xml.WriteStartElement("abstract");
                xml.WriteElementString("para", metadata.Summary);
                xml.WriteEndElement();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="xml">
        /// </param>
        /// <param name="metadata">
        /// </param>
        private static void WriteMetadataTitle(
            XmlWriter xml, 
            MetadataInfo metadata)
        {
            xml.WriteElementString("title", metadata.Title);
        }

        /// <summary>
        /// Creates the XML writer to the output file.
        /// </summary>
        /// <returns>
        /// </returns>
        private XmlWriter CreateXmlWriter()
        {
            // Create an appropriate output stream for the file.
            XmlWriterSettings writingSettings = this.OutputSettings;

            if (writingSettings == null)
            {
                writingSettings = new XmlWriterSettings
                    {
                        OmitXmlDeclaration = true
                    };
            }

            XmlWriter xmlWriter = XmlWriter.Create(
                this.Output, 
                writingSettings);
            return xmlWriter;
        }

        /// <summary>
        /// </summary>
        /// <param name="markdown">
        /// </param>
        /// <param name="xml">
        /// </param>
        private void WriteBold(
            MarkdownReader markdown, 
            XmlWriter xml)
        {
            xml.WriteStartElement("emphasis");
            xml.WriteAttributeString("role", "strong");
        }

        /// <summary>
        /// </summary>
        /// <param name="markdown">
        /// </param>
        /// <param name="xml">
        /// </param>
        private void WriteBreak(
            MarkdownReader markdown, 
            XmlWriter xml)
        {
            xml.WriteStartElement("bridgehead");
            xml.WriteAttributeString("renderas", "other");
            xml.WriteAttributeString("otherrenderas", "break");
            xml.WriteEndElement();
        }

        /// <summary>
        /// </summary>
        /// <param name="markdown">
        /// </param>
        /// <param name="xml">
        /// </param>
        private void WriteForeignPhrase(
            MarkdownReader markdown, 
            XmlWriter xml)
        {
            xml.WriteStartElement("foreignphrase");
        }

        /// <summary>
        /// </summary>
        /// <param name="markdown">
        /// </param>
        /// <param name="xml">
        /// </param>
        private void WriteItalic(
            MarkdownReader markdown, 
            XmlWriter xml)
        {
            xml.WriteStartElement("emphasis");
        }

        /// <summary>
        /// Writes out the elements of the metadata to the XML writer.
        /// </summary>
        /// <param name="xml">
        /// </param>
        /// <param name="metadata">
        /// </param>
        private void WriteMetadata(
            XmlWriter xml, 
            MetadataInfo metadata)
        {
            // See if we need to write the title tag outside the info
            // block.
            if (metadata.HasTitle && this.TitleOutsideInfo)
            {
                WriteMetadataTitle(xml, metadata);
            }

            // We only write an info element if we have at least one
            // element that needs to be written out.
            bool titleInInfo = metadata.HasTitle && !this.TitleOutsideInfo;
            bool needsInfo = metadata.HasNonTitleMetadata;

            if (titleInInfo || needsInfo)
            {
                // Write out the info tag first.
                xml.WriteStartElement("info");

                // Write out the info, if we have it.
                if (titleInInfo)
                {
                    WriteMetadataTitle(xml, metadata);
                }

                WriteMetadataCopyright(xml, metadata);
                WriteMetadataDate(xml, metadata);
                WriteMetadataSummary(xml, metadata);
                WriteMetadataSchemes(xml, metadata);

                // Finish up the info tag.
                xml.WriteEndElement();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="markdown">
        /// </param>
        /// <param name="xml">
        /// </param>
        private void WriteYamlMetadata(
            MarkdownReader markdown, 
            XmlWriter xml)
        {
            // Deserialize the YAML text.
            var deserializer = new Deserializer();
            string text = markdown.Text;
            text = text.TrimEnd()
                .TrimEnd('-')
                .TrimEnd();
            var stringReader = new StringReader(text);

            object objects = deserializer.Deserialize(stringReader);

            // Create the metadata from the object graph.
            var metadata = new MetadataInfo();

            metadata.AddYamlObjects(objects);

            // Use the resulting metadata to write out the elements.
            this.WriteMetadata(xml, metadata);
        }

        #endregion
    }
}