// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Subspace" file="TextTemplateTransformerNonT4Tests.cs">
//   Copyright (c) Subspace. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Subspace.TextTemplating.Tests
{
    using NUnit.Framework;

    using Subspace.TextTemplating;

    [TestFixture]
    public class TextTemplateTransformerNonT4Tests
    {
        [Test]
        public void Transforming_Template4WithProperty_ResultsInExpectedOutput()
        {
            // Arrange.
            var transformer = new TextTemplateTransformer();
            var propertyValue = "test-value";
            var expected = "<html>\r\n<head>\r\n    <title>\r\n    </title>\r\n</head>\r\n<body>\r\n    Property 1 : " + propertyValue + "\r\n</body>\r\n</html>\r\n";

            // Act.
            string output = transformer.TransformFile(@"Templates\NonT4Features\Template1.stt", propertyValue);

            // Assert.
            Assert.AreEqual(expected, output);
        }

        [Test]
        public void Transforming_Template4WithProgrammaticTransform_ResultsInExpectedOutput()
        {
            // Arrange.
            var transformer = new TextTemplateTransformer();
            var expected = "<html>\r\n<head>\r\n    <title>\r\n    </title>\r\n</head>\r\n<body>\r\n    Property 1 : passed-through-test-value\r\n</body>\r\n</html>\r\n";

            // Act.
            string output = transformer.TransformFile(@"Templates\NonT4Features\Template2.stt");

            // Assert.
            Assert.AreEqual(expected, output);
        }
    }
}
