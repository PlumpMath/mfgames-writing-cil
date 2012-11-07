// Copyright 2012 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-writing/license

using System;
using System.IO;
using System.Xml;

namespace MfGames.Writing
{
	/// <summary>
	/// Defines a gathering process for DocBook files which intergrates all of
	/// the XInclude operations along with copying files and media into the
	/// same directory as the output.
	/// </summary>
	public class DocbookGatherProcess
	{
		/// <summary>
		/// Gets or sets the input filename which is in DocBook 5 format.
		/// </summary>
		public string InputFilename { get; set; }

		/// <summary>
		/// Gets or sets the output filename, which will be a DocBook format.
		/// </summary>
		public string OutputFilename { get; set; }

		/// <summary>
		/// Runs this process and performs the appropriate actions.
		/// </summary>
		public void Run()
		{
			// Open up the input file.
			var inputFile = new FileInfo(InputFilename);

			if (!inputFile.Exists)
			{
				throw new FileNotFoundException(
					"Cannot find input file.",
					InputFilename);
			}

			using (FileStream inputStream = inputFile.Open(
				FileMode.Open,
				FileAccess.Read,
				FileShare.Read))
			using (XmlReader reader = XmlReader.Create(inputStream))
			{
				// Create the output file.
				var outputFile = new FileInfo(OutputFilename);

				using (FileStream outputStream = outputFile.Open(
					FileMode.Create,
					FileAccess.Write,
					FileShare.Write))
				using (XmlWriter writer = XmlWriter.Create(outputStream))
				{
					// Process the input file and generate the output.
					Process(
						reader,
						writer);
				}
			}
		}

		/// <summary>
		/// Processes the specified reader and generate the output.
		/// </summary>
		/// <param name="reader">The reader.</param>
		/// <param name="writer">The writer.</param>
		private void Process(
			XmlReader reader,
			XmlWriter writer)
		{
			// Loop through the reader input.
			while (reader.Read())
			{
				// Figure out what to do based on the type.
				switch (reader.NodeType)
				{
					case XmlNodeType.Element:
						writer.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
						writer.WriteAttributes(reader, true);

						if (reader.IsEmptyElement)
						{
							writer.WriteEndElement();
						}

						break;

					case XmlNodeType.Text:
						writer.WriteString(reader.Value);
						break;

					case XmlNodeType.Whitespace:
					case XmlNodeType.SignificantWhitespace:
						writer.WriteWhitespace(reader.Value);
						break;

					case XmlNodeType.CDATA:
						writer.WriteCData(reader.Value);
						break;

					case XmlNodeType.EntityReference:
						writer.WriteEntityRef(reader.Name);
						break;

					case XmlNodeType.XmlDeclaration:
					case XmlNodeType.ProcessingInstruction:
						writer.WriteProcessingInstruction(reader.Name, reader.Value);
						break;

					case XmlNodeType.DocumentType:
						writer.WriteDocType(reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value);
						break;

					case XmlNodeType.Comment:
						writer.WriteComment(reader.Value);
						break;

					case XmlNodeType.EndElement:
						writer.WriteFullEndElement();
						break;
				}
			}
		}
	}
}
