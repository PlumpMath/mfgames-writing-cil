// <copyright file="ConvertMarkdownToDocBookTests.cs" company="Moonfire Games">
//     Copyright (c) Moonfire Games. Some Rights Reserved.
// </copyright>
// MIT Licensed (http://opensource.org/licenses/MIT)
namespace MfGames.Writing.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;

    using MfGames.Writing.Markdown;
    using MfGames.Xml;

    using NUnit.Framework;

    /// <summary>
    /// Tests various functionality of converting Markdown into DocBook.
    /// </summary>
    public class ConvertMarkdownToDocBookTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        [Test]
        public void SimpleMarkdownConversion()
        {
            // Set up the input and output.
            var input = new[]
                {
                    "---", 
                    "title: Article Title", 
                    "---", 
                    "One two three.", 
                };
            var expected = new[]
                {
                    string.Format(
                        "<article version=\"5.0\" xmlns=\"{0}\">", 
                        XmlNamespaces.DocBook5), 
                    "<title>Article Title</title>", 
                    "<para>One two three.</para>", 
                    "</article>", 
                };

            // Execute the process to convert it.
            string inputBuffer = string.Join(Environment.NewLine, input);
            var stringReader = new StringReader(inputBuffer);
            var stringWriter = new StringWriter();
            var process = new ConvertToDocBookProcess
                {
                    Input = stringReader, 
                    Output = stringWriter, 
                    OutputSettings = new XmlWriterSettings
                        {
                            OmitXmlDeclaration = true, 
                            Indent = true, 
                            IndentChars = string.Empty, 
                        }, 
                    TitleOutsideInfo = true, 
                };

            process.Run();

            // Compare the results.
            string actual = stringWriter.ToString();
            this.CompareText(
                expected, 
                actual);
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="expected">
        /// </param>
        /// <param name="actualBuffer">
        /// </param>
        private void CompareText(
            string[] expected, 
            string actualBuffer)
        {
            // Split the actual buffer into lines.
            var actual = new List<string>();

            using (var reader = new StringReader(actualBuffer))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    actual.Add(line);
                }
            }

            // Compare the individual lines.
            bool mismatch = false;
            int max = Math.Max(
                expected.Length, 
                actual.Count);

            for (int i = 0; i < max; i++)
            {
                // Pull out each line.
                string expectedLine = expected.Length <= i ? null : expected[i];
                string actualLine = actual.Count <= i ? null : actual[i];

                // If the line doesn't match, then report it.
                if (expectedLine != actualLine)
                {
                    Console.WriteLine("Line {0:N0} did not match:", i);
                    Console.WriteLine(
                        "  Expected: {0}", 
                        expectedLine);
                    Console.WriteLine(
                        "  Actual:   {0}", 
                        actualLine);
                    mismatch = true;
                }
            }

            // If we have a mismatch, then blow up.
            if (mismatch)
            {
                // We didn't have the lines we expected, so write them all out.
                Console.WriteLine();

                Console.WriteLine("Expected");
                Console.WriteLine("========");
                foreach (string line in expected)
                {
                    Console.WriteLine(line);
                }

                Console.WriteLine();
                Console.WriteLine("Actual");
                Console.WriteLine("========");
                foreach (string line in actual)
                {
                    Console.WriteLine(line);
                }

                // Finish up with a message.
                Console.WriteLine();
                Assert.Fail(
                    "Output lines did not match expected. See console log for details.");
            }
        }

        #endregion
    }
}