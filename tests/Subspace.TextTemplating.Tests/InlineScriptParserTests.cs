/*
This code is released under the Creative Commons Attribute 3.0 Unported license.
You are free to share and reuse this code as long as you keep a reference to the author.
See http://creativecommons.org/licenses/by/3.0/
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Subspace.TextTemplating;

using Subspace.TextTemplating.Tests.TestObjects;

namespace Subspace.TextTemplating.Tests
{
    [TestFixture]
    public class InlineScriptParserTests
    {
        [Test]
        public void Transforming_Template1_ResultsInExpectedOutput()
        {
            // Arrange.
            var inlineScriptParser = new InlineScriptParser();
            var expected = File.ReadAllText(@"Templates\Template1.ExpectedOutput.txt");

            // Act.
            string output = inlineScriptParser.TransformFile(@"Templates\Template1.txt");

            // Assert.
            Assert.AreEqual(expected, output);
        }

        [Test]
        public void Transforming_Template2WithImport_ResultsInExpectedOutput()
        {
            // Arrange.
            var context = new TestContext1
            {
                Property1 = DateTime.Now.ToString()
            };

            var inlineScriptParser = new InlineScriptParser(context);

            var expected = string.Format(
                File.ReadAllText(@"Templates\Template2.ExpectedOutput.txt"),
                context.Property1);

            // Act.

            string output = inlineScriptParser.TransformFile(@"Templates\Template2.txt");

            // Assert.
            Assert.AreEqual(expected, output);
        }

        [Test]
        public void Transforming_Template3WithInclude_ResultsInExpectedOutput()
        {
            // Arrange.
            var inlineScriptParser = new InlineScriptParser();

            var expected = File.ReadAllText(@"Templates\Template3.ExpectedOutput.txt");

            // Act.

            string output = inlineScriptParser.TransformFile(@"Templates\Template3.txt");

            // Assert.
            Assert.AreEqual(expected, output);
        }
    }
}
