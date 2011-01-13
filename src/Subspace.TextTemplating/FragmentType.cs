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
    ///     An enumeration used to specify a fragment type.
    /// </summary>
    internal enum FragmentType
    {
        /// <summary>
        ///     A markup fragment.
        /// </summary>
        Markup,

        /// <summary>
        ///     A script fragment.
        /// </summary>
        Script,

        /// <summary>
        ///     A script expression of which the result is automatically written to the output.
        /// </summary>
        AutoWriteScript,

        /// <summary>
        ///     A script, such as a method definition, that will end up in the class body.
        /// </summary>
        ClassBody,

        /// <summary>
        ///     A template directive.
        /// </summary>
        Template,

        /// <summary>
        ///     An include directive.
        /// </summary>
        Include,

        /// <summary>
        ///     A namespace import directive.
        /// </summary>
        NamespaceImport,

        /// <summary>
        ///     A script parameter directive.
        /// </summary>
        ScriptParameter
    }
}
