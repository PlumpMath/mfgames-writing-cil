// <copyright file="GatherDocBookOptions.cs" company="Moonfire Games">
//     Copyright (c) Moonfire Games. Some Rights Reserved.
// </copyright>
// MIT Licensed (http://opensource.org/licenses/MIT)
namespace MfGames.Writing.Cli.DocBook
{
    using System.Collections.Generic;
    using System.IO;

    using CommandLine;

    using MfGames.Writing.DocBook;

    /// <summary>
    /// Defines the option for the `docbook-gather` operation.
    /// </summary>
    internal class GatherDocBookOptions : IProcessOptions
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the remaining arguments which includes the input and output file.
        /// </summary>
        [ValueList(typeof(List<string>), MaximumElements = 2)]
        public IList<string> RemainingArguments { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Retrieves an IProcess represented by the options.
        /// </summary>
        /// <returns>
        /// A process object associated with the options.
        /// </returns>
        public ProcessBase GetProcess()
        {
            var process = new GatherDocBookProcess
            {
                InputFile = new FileInfo(this.RemainingArguments[0]), 
                OutputFile = new FileInfo(this.RemainingArguments[1])
            };

            return process;
        }

        #endregion
    }
}