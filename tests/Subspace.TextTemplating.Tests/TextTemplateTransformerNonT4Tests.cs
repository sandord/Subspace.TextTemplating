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
    public class TextTemplateTransformerNonT4Tests
    {
        [Test]
        public void Transforming_Template4WithParameter_ResultsInExpectedOutput()
        {
            // Arrange.
            var transformer = new TextTemplateTransformer();
            var parameterValue = "test-value";
            var expected = "<html>\r\n<head>\r\n    <title>\r\n    </title>\r\n</head>\r\n<body>\r\n    Parameter 1 : " + parameterValue + "\r\n</body>\r\n</html>\r\n";

            // Act.
            string output = transformer.TransformFile(@"Templates\NonT4Features\Template1.tt", parameterValue);

            // Assert.
            Assert.AreEqual(expected, output);
        }

        [Test]
        public void Transforming_Template4WithProgrammaticTransform_ResultsInExpectedOutput()
        {
            // Arrange.
            var transformer = new TextTemplateTransformer();
            var expected = "<html>\r\n<head>\r\n    <title>\r\n    </title>\r\n</head>\r\n<body>\r\n    Parameter 1 : passed-through-test-value\r\n</body>\r\n</html>\r\n";

            // Act.
            string output = transformer.TransformFile(@"Templates\NonT4Features\Template2.tt");

            // Assert.
            Assert.AreEqual(expected, output);
        }
    }
}
