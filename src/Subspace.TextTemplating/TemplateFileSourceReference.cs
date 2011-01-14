using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subspace.TextTemplating
{
    /// <summary>
    ///     A reference to a line in a template file source.
    /// </summary>
    public struct TemplateFileSourceReference
    {
        /// <summary>
        ///     Represents an empty template file source reference.
        /// </summary>
        public static readonly TemplateFileSourceReference Empty = new TemplateFileSourceReference();

        /// <summary>
        ///     Gets or sets the path.
        /// </summary>
        public string Path
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the line.
        /// </summary>
        public int Line
        {
            get;
            set;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="System.Object"/> is equal to this
        ///     instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            TemplateFileSourceReference instance = (TemplateFileSourceReference)obj;
            
            return Path == instance.Path && Line == instance.Line;
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return Path.GetHashCode() ^ Line.GetHashCode();
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="left">The left hand value.</param>
        /// <param name="right">The right hand value.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(TemplateFileSourceReference left, TemplateFileSourceReference right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="left">The left hand value.</param>
        /// <param name="right">The right hand value.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(TemplateFileSourceReference left, TemplateFileSourceReference right)
        {
            return !left.Equals(right);
        }
    }
}
