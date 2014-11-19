using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MfGames.Writing.Tests
{
    using System.IO;
    using System.Xml;

    using MfGames.Writing.Markdown;

    using NUnit.Framework;

    /// <summary>
    /// Tests various functionality of converting Markdown into DocBook.
    /// </summary>
    public class ConvertMarkdownToDocBookTests
    {
        [Test]
        public void SimpleMarkdownConversion()
        {
            // Set up the input and output.
            var input = new[]
                {
                    "---", "title: Article Title", "---",
                    "One two three.",
                };
            var expected = new[]
                {
                    "<article version=\"5\" xmlns=\"http://docbook.org/ns/docbook\">",
                    "<title>Article Title</title>", "<para>One two three.</para>",
                    "</article>",
                };

            // Execute the process to convert it.
            string inputBuffer = string.Join(Environment.NewLine, input);
            StringReader stringReader = new StringReader(inputBuffer);
            StringWriter stringWriter = new StringWriter();
            var process = new ConvertToDocBookProcess() {
                Input = stringReader,
                Output = stringWriter,
                OutputSettings = new XmlWriterSettings()
                    {
                        OmitXmlDeclaration = true,
                        Indent = true,
                        IndentChars = "",
                    },
            };

            process.Run();

            // Compare the results.
            string actual = stringWriter.ToString();
            CompareText(
                expected,
                actual);
        }

        private void CompareText(
            string[] expected,
            string actualBuffer)
        {
            // Split the actual buffer into lines.
            List<string> actual = new List<string>();

            using (var reader = new StringReader(actualBuffer))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                actual.Add(line);
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
                    Console.WriteLine("Line {0:N0} did not match:");
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
                Console.WriteLine();
                Assert.Fail(
                    "Output lines did not match expected. See console log for details.");
            }
        }
    }
}
