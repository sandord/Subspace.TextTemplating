using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Subspace.TextTemplating.Tests.TestObjects;

namespace Subspace.TextTemplating.Tests.Templates
{
    public partial class Template2 : Template2Base
    {
        public TestContext1 Context
        {
            get;
            set;
        }
    }
}
