// Copyright 2012 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-writing/license

using System.Collections.Generic;
using System.IO;

namespace MfGames.Writing
{
	/// <summary>
	/// Defines the common process for individual operations in the library.
	/// </summary>
	public abstract class ProcessBase
	{
		#region Properties

		/// <summary>
		/// Gets or sets the root directory. When searching for using the 
		/// SearchDirectories, the relatively root from the parent will be used
		/// to determine paths.
		/// </summary>
		public DirectoryInfo RootDirectory { get; set; }

		/// <summary>
		/// Gets the search directories which are used to find relative files.
		/// </summary>
		public List<DirectoryInfo> SearchDirectories { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Runs this process and performs the appropriate actions.
		/// </summary>
		public abstract void Run();

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessBase"/> class.
		/// </summary>
		public ProcessBase()
		{
			SearchDirectories = new List<DirectoryInfo>();
		}

		#endregion
	}
}
