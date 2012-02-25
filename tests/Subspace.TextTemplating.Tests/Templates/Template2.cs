// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Subspace" file="Template2.cs">
//   Copyright (c) Subspace. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Subspace.TextTemplating.Tests.Templates
{
    using Subspace.TextTemplating.Tests.TestObjects;

    public partial class Template2 : Template2Base
    {
        // This property is implemented only to allow the template to be transformed by the T4
        // engine so that we can compare its output with the output of our own engine.
        public TestContext1 Context
        {
            get;
            set;
        }
    }
}
