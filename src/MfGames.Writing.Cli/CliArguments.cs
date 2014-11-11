// <copyright file="CliArguments.cs" company="Moonfire Games">
//     Copyright (c) Moonfire Games. Some Rights Reserved.
// </copyright>
// MIT Licensed (http://opensource.org/licenses/MIT)
namespace MfGames.Writing.Cli
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the command-line arguments.
    /// </summary>
    internal class CliArguments
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CliArguments"/> class.
        /// </summary>
        public CliArguments()
        {
            this.RemainingArguments = new List<string>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the format for the operation.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Gets or sets the operation to perform on the specific format.
        /// </summary>
        public string Operation { get; set; }

        /// <summary>
        /// Gets the remaining arguments.
        /// </summary>
        public List<string> RemainingArguments { get; private set; }

        #endregion
    }
}