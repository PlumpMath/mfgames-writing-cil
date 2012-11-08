// Copyright 2012 Moonfire Games
// Released under the MIT license
// http://mfgames.com/mfgames-writing/license

using System.Collections.Generic;

namespace MfGames.Writing.Cli
{
	/// <summary>
	/// Defines the command-line arguments.
	/// </summary>
	internal class CliArguments
	{
		#region Properties

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

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="CliArguments"/> class.
		/// </summary>
		public CliArguments()
		{
			RemainingArguments = new List<string>();
		}

		#endregion
	}
}
