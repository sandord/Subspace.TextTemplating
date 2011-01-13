/*
This code is released under the Creative Commons Attribute 3.0 Unported license.
You are free to share and reuse this code as long as you keep a reference to the author.
See http://creativecommons.org/licenses/by/3.0/
*/

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

        /// <summary>
        ///     Initializes a new instance of the specified <see cref="NamespaceReference"/> class.
        /// </summary>
        /// <param name="type">The type of which to use the namespace to initialize the instance.</param>
        public NamespaceReference(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            this.Namespace = type.Namespace;
        }
    }
}
