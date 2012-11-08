// Copyright 2012 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-writing/license

using System.IO;
using System.Xml;
using MfGames.Writing.Xml;
using MfGames.Xml;

namespace MfGames.Writing
{
	/// <summary>
	/// Defines a gathering process for DocBook files which intergrates all of
	/// the XInclude operations along with copying files and media into the
	/// same directory as the output.
	/// </summary>
	public class DocbookGatherProcess: ProcessBase
	{
		/// <summary>
		/// Gets or sets the input file which is in DocBook 5 format.
		/// </summary>
		public FileInfo InputFile { get; set; }

		/// <summary>
		/// Gets or sets the output file, which will be a DocBook format.
		/// </summary>
		public FileInfo OutputFile { get; set; }

		/// <summary>
		/// Gets or sets the output directory for the media files. References
		/// in the resulting XML will be relative from the OutputFile to
		/// this directory.
		/// </summary>
		public DirectoryInfo OutputDirectory { get; set; }

		/// <summary>
		/// Runs this process and performs the appropriate actions.
		/// </summary>
		public override void Run()
		{
			// Verify that the input file exists since if we can't, it is
			// meaningless to continue.
			if (!InputFile.Exists)
			{
				throw new FileNotFoundException(
					"Cannot find input file.",
					InputFile.FullName);
			}

			// We use the identity XML writer to copy out the resulting XML
			// stream but use an intelligent loader to do all the work. This
			// way, we can reuse much of the IO functionality without
			// cluttering our code.
			using (GatheringXmlReader xmlReader = CreateXmlReader())
			using (XmlIdentityWriter xmlWriter = CreateXmlWriter())
			{
				// Using the identity XML writer will go through the entire
				// reader XML and write it out. This will cause the gathering
				// reader to load in the various XInclude elements and also
				// copy the media files to the appropriate place.
				xmlWriter.Load(xmlReader);
			}
		}

		/// <summary>
		/// Creates the XML reader for loading the XML file.
		/// </summary>
		/// <returns></returns>
		private GatheringXmlReader CreateXmlReader()
		{
			// Get an appropriate stream to the input file.
			FileStream stream = InputFile.Open(
				FileMode.Open,
				FileAccess.Read,
				FileShare.Read);
			XmlReader xmlReader = XmlReader.Create(stream);

			// Set up the gathering XML reader.
			var gatheringXmlReader = new GatheringXmlReader(xmlReader)
			{
				OutputDirectory = OutputDirectory
			};

			return gatheringXmlReader;
		}

		/// <summary>
		/// Creates the XML writer to the output file.
		/// </summary>
		/// <returns></returns>
		private XmlIdentityWriter CreateXmlWriter()
		{
			// Create an appropriate output stream for the file.
			FileStream stream = OutputFile.Open(
				FileMode.Create,
				FileAccess.Write);
			XmlWriter xmlWriter = XmlWriter.Create(
				stream,
				new XmlWriterSettings
				{
					OmitXmlDeclaration = true
				});

			// Create an identity writer which handles outputing the input.
			var identityWriter = new XmlIdentityWriter(xmlWriter);

			return identityWriter;
		}
	}
}
