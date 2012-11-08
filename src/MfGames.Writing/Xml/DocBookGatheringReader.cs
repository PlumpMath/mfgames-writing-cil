// Copyright 2012 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-writing/license

using System;
using System.IO;
using System.Xml;
using MfGames.Extensions.System;
using MfGames.Extensions.System.IO;
using MfGames.Xml;

namespace MfGames.Writing.Xml
{
	/// <summary>
	/// Implements a reader that also gathers the media files reference by the
	/// DocBook file.
	/// </summary>
	public class DocBookGatheringReader: XIncludeReader
	{
		#region Properties

		/// <summary>
		/// Gets or sets the output directory for copying media files.
		/// </summary>
		public DirectoryInfo OutputDirectory { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// When overridden in a derived class, reads the next node from the stream.
		/// </summary>
		/// <returns>
		/// true if the next node was read successfully; false if there are no more nodes to read.
		/// </returns>
		/// <exception cref="T:System.Xml.XmlException">
		/// An error occurred while parsing the XML.
		///   </exception>
		public override bool Read()
		{
			// Attemp to read from the underlying reader.
			bool read = base.Read();

			if (!read)
			{
				return false;
			}

			// Check to see if we are looking at a media node.
			if (NodeType == XmlNodeType.Element
				&& LocalName == "imagedata")
			{
				// Pull out the file reference.
				string fileReference = GetAttribute("fileref");

				if (fileReference == null)
				{
					throw new ApplicationException("Cannot find a @fileref");
				}

				// Use the URI to figure out the path.
				Uri baseUri = NormalizedBaseUri;
				var uri = new Uri(
					baseUri,
					fileReference);

				// Copy the file into the output directory.
				CopyFile(uri);
			}

			// Return that we read it successfully.
			return true;
		}

		/// <summary>
		/// Copies the given URI into the output directory.
		/// </summary>
		/// <param name="uri">The URI.</param>
		private void CopyFile(Uri uri)
		{
			// If this is not a file, ignore it.
			if (!uri.IsFile)
			{
				throw new ApplicationException("Cannot handle non-file URI: " + uri);
			}

			// If the file doesn't exist, skip it.
			var sourceFile = new FileInfo(uri.AbsolutePath);

			if (!sourceFile.Exists)
			{
				return;
			}

			// Figure out what the output file will be.
			var outputFile = new FileInfo(
				Path.Combine(
					OutputDirectory.FullName,
					sourceFile.Name));

			// If the output file doesn't exist, just copy it.
			if (!outputFile.Exists)
			{
				// The file can be copied directly.
				CopyFile(
					sourceFile,
					outputFile);
				return;
			}

			// Check to see if the two hashes match.
			string sourceHash = sourceFile.GetSha256().ToHexString();
			string outputHash = outputFile.GetSha256().ToHexString();

			if (sourceHash == outputHash)
			{
				UpdateFileAttribute(outputFile);
				return;
			}

			// The output file exists, so try to match it, otherwise create a
			// new version using the SHA256 hash as part of the name.
			string basename = outputFile.GetBasename();

			for (int testLength = 4;
				testLength <= 32;
				testLength += 4)
			{
				// Reconstruct a new output filename based on the hash.
				string sourceHashSubset = sourceHash.Substring(
					0,
					testLength);
				string newFilename = Path.Combine(
					outputFile.Directory.FullName,
					basename + "_" + sourceHashSubset + outputFile.Extension);

				// Check to see if the file exists.
				outputFile = new FileInfo(newFilename);

				if (!outputFile.Exists)
				{
					CopyFile(
						sourceFile,
						outputFile);
					return;
				}

				// See if the file is identical to the source.
				outputHash = outputFile.GetSha256().ToHexString();

				if (sourceHash == outputHash)
				{
					// File is identical to the source.
					UpdateFileAttribute(outputFile);
					return;
				}
			}

			// If we got this far, we can't figure out the filename.
			throw new ApplicationException(
				"Cannot figure out how to copy in " + sourceFile.FullName);
		}

		/// <summary>
		/// Copies the file from the source to the output and upates the
		/// attribute.
		/// </summary>
		/// <param name="sourceFile">The source file.</param>
		/// <param name="outputFile">The output file.</param>
		private void CopyFile(
			FileInfo sourceFile,
			FileInfo outputFile)
		{
			// TODO Update the attribute.
			sourceFile.CopyTo(outputFile.FullName);

			// Update the attributes.
			UpdateFileAttribute(outputFile);
		}

		/// <summary>
		/// Updates the file attributes so the resulting file will point to the
		/// correct location.
		/// </summary>
		/// <param name="file">The file.</param>
		private void UpdateFileAttribute(FileInfo file)
		{
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DocBookGatheringReader"/> class.
		/// </summary>
		/// <param name="underlyingReader">The underlying reader.</param>
		public DocBookGatheringReader(XmlReader underlyingReader)
			: base(underlyingReader)
		{
		}

		#endregion
	}
}
