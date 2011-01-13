/*
This code is released under the Creative Commons Attribute 3.0 Unported license.
You are free to share and reuse this code as long as you keep a reference to the author.
See http://creativecommons.org/licenses/by/3.0/
*/

using System;

namespace Subspace.TextTemplating
{
    /// <summary>
    ///     Internal constants.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        ///     The version of the C# compiler to use for inline scripting.
        /// </summary>
        internal static readonly string InlineScriptingCompilerVersion = "v4.0";

        /// <summary>
        ///     The file extension to use when writing generated inline script files.
        /// </summary>
        internal static readonly string InlineScriptingScriptFileExtension = ".cs";

        /// <summary>
        ///     The namespace to execute inline scripts in.
        /// </summary>
        internal static readonly string NamespaceName = "Subspace.TextTemplating.InlineScripting";

        /// <summary>
        ///     The name of the class to hold the document scripts.
        /// </summary>
        internal static readonly string ClassName = "__documentScripts";

        /// <summary>
        ///     The name of the document script initialization method.
        /// </summary>
        internal static readonly string InitializationMethodName = "__initialize";

        /// <summary>
        ///     The name of the script parameter initialization method.
        /// </summary>
        internal static readonly string ParameterInitializationMethodName = "__initializeParameters";

        /// <summary>
        ///     The start marker of inline scripts.
        /// </summary>
        internal static readonly string ScriptStartMarker = "<#";

        /// <summary>
        ///     The end marker of inline scripts.
        /// </summary>
        internal static readonly string ScriptEndMarker = "#>";

        /// <summary>
        ///     The marker that is placed directly after the start marker, to mark a class body script, such as a method definition.
        /// </summary>
        internal static readonly string ScriptClassBodyMarker = "+";

        /// <summary>
        ///     The marker that is placed directly after the start marker, to mark an automatic output expression.
        /// </summary>
        internal static readonly string ScriptAutoWriteMarker = "=";

        /// <summary>
        ///     The marker that is placed directly after the start marker, to mark a template directive.
        /// </summary>
        internal static readonly string TemplateMarker = "@ template";

        /// <summary>
        ///     The marker that is placed directly after the start marker, to mark a namespace import directive.
        /// </summary>
        internal static readonly string NamespaceImportMarker = "@ import";

        /// <summary>
        ///     The marker that is placed directly after the start marker, to mark a script include directive.
        /// </summary>
        internal static readonly string ScriptIncludeMarker = "@ include";

        /// <summary>
        ///     The marker that is placed directly after the start marker, to mark a script parameter directive.
        /// </summary>
        internal static readonly string ScriptParameterMarker = "@ parameter";
    }
}
