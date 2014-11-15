// <copyright file="ConvertToDocBookProcess.cs" company="Moonfire Games">
//     Copyright (c) Moonfire Games. Some Rights Reserved.
// </copyright>
// MIT Licensed (http://opensource.org/licenses/MIT)
namespace MfGames.Writing.DocBook
{
    using System.IO;
    using System.Xml;

    using MfGames.Xml;

    /// <summary>
    /// Defines a gathering process for DocBook files which intergrates all of
    /// the XInclude operations along with copying files and media into the
    /// same directory as the output.
    /// </summary>
    public class GatherDocBookProcess : ProcessBase
    {
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