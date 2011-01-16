using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subspace.TextTemplating
{
    /// <summary>
    ///     A template namespace reference.
    /// </summary>
    internal sealed class NamespaceReference
    {
        /// <summary>
        ///     Gets the namespace.
        /// </summary>
        public string Namespace
        {
            get;
            private set;
        }

        /// <summary>
        ///     Initializes a new instance of the specified <see cref="NamespaceReference"/> class.
        /// </summary>
        /// <param name="namespace">The namespace.</param>
        public NamespaceReference(string @namespace)
        {
            this.Namespace = @namespace;
        }
    }
}
