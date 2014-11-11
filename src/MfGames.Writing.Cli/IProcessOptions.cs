// <copyright file="IProcessOptions.cs" company="Moonfire Games">
//     Copyright (c) Moonfire Games. Some Rights Reserved.
// </copyright>
// MIT Licensed (http://opensource.org/licenses/MIT)
namespace MfGames.Writing.Cli
{
    /// <summary>
    /// Defines an option that is directly associated with selecting and configuring
    /// a process.
    /// </summary>
    public interface IProcessOptions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Retrieves an IProcess represented by the options.
        /// </summary>
        /// <returns>
        /// A process object associated with the options.
        /// </returns>
        ProcessBase GetProcess();

        #endregion
    }
}