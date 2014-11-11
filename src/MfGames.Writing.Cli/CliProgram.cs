// <copyright file="CliProgram.cs" company="Moonfire Games">
//     Copyright (c) Moonfire Games. Some Rights Reserved.
// </copyright>
// MIT Licensed (http://opensource.org/licenses/MIT)
namespace MfGames.Writing.Cli
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using CommandLine;

    /// <summary>
    /// Defines the entry point for the command-line interface for MfGames Writing.
    /// </summary>
    internal class CliProgram
    {
        #region Methods

        /// <summary>
        /// Starting point for the application.
        /// </summary>
        /// <param name="args">
        /// The command-line arguments to the application. 
        /// </param>
        private static void Main(string[] args)
        {
            // Parse the arguments into an options object.
            var options = new CliOptions();
            string invokedVerb = null;
            object invokedOptions = null;
            var parser = new Parser(
                delegate(ParserSettings settings)
                {
                    settings.CaseSensitive = true;
                    settings.IgnoreUnknownArguments = false;
                });
            bool successful = parser.ParseArguments(
                args,
                options,
                (verb,
                    subOptions) =>
                {
                    // if parsing succeeds the verb name and correct instance
                    // will be passed to onVerbCommand delegate (string,object)
                    invokedVerb = verb;
                    invokedOptions = subOptions;
                });

            if (!successful)
            {
                Console.WriteLine("Cannot parse arguments.");
                Environment.Exit(1);
            }

            // If this is a process options, then configure it.
            var processOptions = invokedOptions as IProcessOptions;

            if (processOptions != null)
            {
                ProcessBase process = processOptions.GetProcess();
                process.Run();
                return;
            }

            // If we got this far, we have an invalid state.
            throw new Exception("Cannot identify resulting options.");
        }

        #endregion
    }
}