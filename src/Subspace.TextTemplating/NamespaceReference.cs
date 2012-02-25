// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Subspace" file="NamespaceReference.cs">
//   Copyright (c) Subspace. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Subspace.TextTemplating
{
    /// <summary>
    ///     A template namespace reference.
    /// </summary>
    internal sealed class NamespaceReference
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NamespaceReference"/> class.
        /// </summary>
        /// <param name="namespace">The namespace.</param>
        public NamespaceReference(string @namespace)
        {
            this.Namespace = @namespace;
        }

        /// <summary>
        ///     Gets the namespace.
        /// </summary>
        public string Namespace
        {
            get;
            private set;
        }
    }
}
