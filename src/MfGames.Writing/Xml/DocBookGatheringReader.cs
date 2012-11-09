// Copyright 2012 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-writing/license

using System;
using System.Collections.Generic;
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
		/// When overridden in a derived class, gets the number of attributes on the current node.
		/// </summary>
		/// <returns>
		/// The number of attributes on the current node.
		///   </returns>
		public override int AttributeCount
		{
			get
			{
				if (overriddenAttributes != null)
				{
					return overriddenAttributes.Count;
				}

				return base.AttributeCount;
			}
		}

		/// <summary>
		/// Gets or sets the output directory for copying media files.
		/// </summary>
		public DirectoryInfo OutputDirectory { get; set; }

		/// <summary>
		/// When overridden in a derived class, gets the text value of the current node.
		/// </summary>
		/// <returns>
		/// The value returned depends on the <see cref="P:System.Xml.XmlReader.NodeType"/> of the node. The following table lists node types that have a value to return. All other node types return String.Empty.
		/// Node type
		/// Value
		/// Attribute
		/// The value of the attribute.
		/// CDATA
		/// The content of the CDATA section.
		/// Comment
		/// The content of the comment.
		/// DocumentType
		/// The internal subset.
		/// ProcessingInstruction
		/// The entire content, excluding the target.
		/// SignificantWhitespace
		/// The white space between markup in a mixed content model.
		/// Text
		/// The content of the text node.
		/// Whitespace
		/// The white space between markup.
		/// XmlDeclaration
		/// The content of the declaration.
		///   </returns>
		public override string Value
		{
			get
			{
				// If we are reading an attribute and we have an override, we
				// need to use that value instead.
				if (NodeType == XmlNodeType.Attribute
					&& overriddenAttributes != null)
				{
					// Pull out that element.
					Tuple<string, string> item = overriddenAttributes[currentAttribute];
					return item.Item2;
				}

				return base.Value;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// When overridden in a derived class, gets the value of the attribute with the specified index.
		/// </summary>
		/// <param name="i">The index of the attribute. The index is zero-based. (The first attribute has index 0.)</param>
		/// <returns>
		/// The value of the specified attribute. This method does not move the reader.
		/// </returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="i"/> is out of range. It must be non-negative and less than the size of the attribute collection.
		///   </exception>
		public override string GetAttribute(int i)
		{
			if (overriddenAttributes != null)
			{
				return overriddenAttributes[i].Item2;
			}

			return base.GetAttribute(i);
		}

		/// <summary>
		/// When overridden in a derived class, gets the value of the attribute with the specified <see cref="P:System.Xml.XmlReader.Name"/>.
		/// </summary>
		/// <param name="name">The qualified name of the attribute.</param>
		/// <returns>
		/// The value of the specified attribute. If the attribute is not found or the value is String.Empty, null is returned.
		/// </returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name"/> is null.
		///   </exception>
		public override string GetAttribute(string name)
		{
			if (overriddenAttributes != null)
			{
				foreach (Tuple<string, string> attribute in overriddenAttributes)
				{
					if (attribute.Item1 == name)
					{
						return attribute.Item2;
					}
				}

				return null;
			}

			return base.GetAttribute(name);
		}

		/// <summary>
		/// When overridden in a derived class, gets the value of the attribute with the specified <see cref="P:System.Xml.XmlReader.LocalName"/> and <see cref="P:System.Xml.XmlReader.NamespaceURI"/>.
		/// </summary>
		/// <param name="name">The local name of the attribute.</param>
		/// <param name="namespaceURI">The namespace URI of the attribute.</param>
		/// <returns>
		/// The value of the specified attribute. If the attribute is not found or the value is String.Empty, null is returned. This method does not move the reader.
		/// </returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name"/> is null.
		///   </exception>
		public override string GetAttribute(
			string name,
			string namespaceURI)
		{
			return GetAttribute(name);
		}

		/// <summary>
		/// When overridden in a derived class, moves to the first attribute.
		/// </summary>
		/// <returns>
		/// true if an attribute exists (the reader moves to the first attribute); otherwise, false (the position of the reader does not change).
		/// </returns>
		public override bool MoveToFirstAttribute()
		{
			currentAttribute = 0;
			return base.MoveToFirstAttribute();
		}

		/// <summary>
		/// When overridden in a derived class, moves to the next attribute.
		/// </summary>
		/// <returns>
		/// true if there is a next attribute; false if there are no more attributes.
		/// </returns>
		public override bool MoveToNextAttribute()
		{
			currentAttribute++;
			return base.MoveToNextAttribute();
		}

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
			// Reset the overriden attributes so we use the underyling version.
			overriddenAttributes = null;

			// Attempt to read from the underlying reader.
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
			// Copy the file into the appropriate location.
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
			// Create the overriden attributes list so we use that instead.
			overriddenAttributes = new List<Tuple<string, string>>();

			// Loop through all the attributes, grabbing their name and values.
			MoveToFirstAttribute();

			do
			{
				// Grab the next attribute, unless it is the fileref.
				var entry = new Tuple<string, string>(
					Name,
					Name == "fileref"
						? file.FullName
						: base.Value);

				// Add the new attribute to the list.
				overriddenAttributes.Add(entry);
			}
			while (MoveToNextAttribute());

			// Move back to the beginning of the element.
			MoveToElement();
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

		#region Fields

		private int currentAttribute;
		private List<Tuple<string, string>> overriddenAttributes;

		#endregion
	}
}
