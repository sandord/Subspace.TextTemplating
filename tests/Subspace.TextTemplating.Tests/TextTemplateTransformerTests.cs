using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Subspace.TextTemplating;

using Subspace.TextTemplating.Tests.TestObjects;
using Subspace.TextTemplating.Tests.Templates;

namespace Subspace.TextTemplating.Tests
{
    [TestFixture]
    public class TextTemplateTransformerTests
    {
        [Test]
        public void Transforming_Template1_ResultsInExpectedOutput()
        {
            // Arrange.
            var transformer = new TextTemplateTransformer();
            var expected = new Template1().TransformText();

            // Act.
            string output = transformer.TransformFile(@"Templates\Template1.tt");

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

            var transformer = new TextTemplateTransformer(context);
            var expected = new Template2() { Context = context }.TransformText();

            // Act.

            string output = transformer.TransformFile(@"Templates\Template2.tt");

            // Assert.
            Assert.AreEqual(expected, output);
        }

        [Test]
        public void Transforming_Template3WithInclude_ResultsInExpectedOutput()
        {
            // Arrange.
            var transformer = new TextTemplateTransformer();
            var expected = new Template3().TransformText();

            // Act.

            string output = transformer.TransformFile(@"Templates\Template3.tt");

            // Assert.
            Assert.AreEqual(expected, output);
        }
    }
}
