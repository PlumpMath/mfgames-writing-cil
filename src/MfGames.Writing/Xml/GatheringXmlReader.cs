// Copyright 2012 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-writing/license

using System;
using System.IO;
using System.Xml;
using MfGames.Xml;

namespace MfGames.Writing.Xml
{
	/// <summary>
	/// Implements a gathering XML reader that parses a DocBook 5 file into a
	/// single version while processing all of the XInclude elements. At the
	/// same time, it merges the various assets used by the XML file into the
	/// source directory.
	/// </summary>
	public class GatheringXmlReader: XIncludeReader
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="GatheringXmlReader"/> class.
		/// </summary>
		/// <param name="underlyingReader">The underlying reader.</param>
		public GatheringXmlReader(XmlReader underlyingReader)
			: base(underlyingReader)
		{
		}

		#endregion

		/// <summary>
		/// Gets or sets the output directory for storing files.
		/// </summary>
		public DirectoryInfo OutputDirectory { get; set; }

		/// <summary>
		/// Gets the included XML reader based on the current node.
		/// </summary>
		/// <returns></returns>
		protected override XmlReader GetIncludedXmlReader()
		{
			// Grab the @href element.
			string href = GetAttribute("href");

			if (href == null)
			{
				throw new ApplicationException("Cannot include href from the XInclude tag.");
			}

			// Create the resulting XML reader
			XmlReader reader = Create(href);

			return reader;
		}
	}
}
