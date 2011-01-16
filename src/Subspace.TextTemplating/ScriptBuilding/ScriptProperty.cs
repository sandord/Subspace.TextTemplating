using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subspace.TextTemplating.ScriptBuilding
{
    /// <summary>
    ///     Describes a property for which a value can be passed into a script.
    /// </summary>
    internal class ScriptProperty
    {
        /// <summary>
        ///     Gets the name.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the name of the type.
        /// </summary>
        public string TypeName
        {
            get;
            private set;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ScriptProperty"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="typeName">The name of the type.</param>
        public ScriptProperty(string name, string typeName)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            else if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(InternalExceptionStrings.ArgumentException_EmptyOrWhitespaceString, "name");
            }
            else if (typeName == null)
            {
                throw new ArgumentNullException("typeName");
            }
            else if (string.IsNullOrWhiteSpace(typeName))
            {
                throw new ArgumentException(InternalExceptionStrings.ArgumentException_EmptyOrWhitespaceString, "typeName");
            }

            this.Name = name;
            this.TypeName = typeName;
        }
    }
}
