// <copyright file="ProcessBase.cs" company="Moonfire Games">
//     Copyright (c) Moonfire Games. Some Rights Reserved.
// </copyright>
// MIT Licensed (http://opensource.org/licenses/MIT)
namespace MfGames.Writing
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Defines the common process for individual operations in the library.
    /// </summary>
    public abstract class ProcessBase
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessBase"/> class.
        /// </summary>
        public ProcessBase()
        {
            this.SearchDirectories = new List<DirectoryInfo>();
        }

        #endregion

        #region Public Properties

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

        #region Public Methods and Operators

        /// <summary>
        /// Runs this process and performs the appropriate actions.
        /// </summary>
        public abstract void Run();

        #endregion
    }
}