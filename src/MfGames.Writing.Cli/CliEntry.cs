// <copyright file="CliEntry.cs" company="Moonfire Games">
//     Copyright (c) Moonfire Games. Some Rights Reserved.
// </copyright>
// MIT Licensed (http://opensource.org/licenses/MIT)
namespace MfGames.Writing.Cli
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Defines the entry point for the command-line interface for MfGames Writing.
    /// </summary>
    internal class CliEntry
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
            // Parse the arguments and places it into the separate objects.
            CliArguments arguments = ParseArguments(args);

            // If we aren't doing "docbook" and "gather", we don't allow it.
            if (arguments.Format != "docbook")
            {
                throw new ApplicationException(
                    "The only format supported is 'docbook'.");
            }

            if (arguments.Operation != "gather")
            {
                throw new ApplicationException(
                    "The only operation supported is 'gather'.");
            }

            // Create the process arguments and execute it.
            var process = new DocbookGatherProcess
                {
                    InputFile = new FileInfo(arguments.RemainingArguments[0]), 
                    OutputFile = new FileInfo(arguments.RemainingArguments[1])
                };

            process.Run();
        }

        /// <summary>
        /// Parses the command line arguments and places it into an appropriate object.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// </returns>
        private static CliArguments ParseArguments(IEnumerable<string> args)
        {
            // Create a new options argument.
            var arguments = new CliArguments();

            // Go through the arguments and populate the various fields.
            int ordinal = 0;

            foreach (string arg in args)
            {
                // If we are a command-line option, then populate those fields.
                if (arg.StartsWith("--"))
                {
                    switch (arg.Substring(2))
                    {
                        case "root-directory":
                        case "search-directory":
                            break;
                    }

                    // We don't do any other processing of arguments.
                    continue;
                }

                // For everything else, just place it in the ordinals.
                switch (ordinal++)
                {
                    case 0:
                        arguments.Format = arg;
                        break;

                    case 1:
                        arguments.Operation = arg;
                        break;

                    default:
                        arguments.RemainingArguments.Add(arg);
                        break;
                }
            }

            // Return the resulting arguments object.
            return arguments;
        }

        #endregion
    }
}