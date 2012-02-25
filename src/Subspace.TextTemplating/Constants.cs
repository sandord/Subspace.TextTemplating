// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Subspace" file="Constants.cs">
//   Copyright (c) Subspace. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Subspace.TextTemplating
{
    /// <summary>
    ///     Internal constants.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        ///     The namespace to execute inline scripts in.
        /// </summary>
        internal const string NamespaceName = "Subspace.TextTemplating.InlineScripting";

        /// <summary>
        ///     The name of the class to hold the document scripts.
        /// </summary>
        internal const string ClassName = "__documentScripts";

        /// <summary>
        ///     The name of the document script initialization method.
        /// </summary>
        internal const string InitializationMethodName = "__initialize";

        /// <summary>
        ///     The name of the script property initialization method.
        /// </summary>
        internal const string PropertyInitializationMethodName = "__initializeProperties";

        /// <summary>
        ///     The start marker of inline scripts.
        /// </summary>
        internal const string ScriptStartMarker = "<#";

        /// <summary>
        ///     The end marker of inline scripts.
        /// </summary>
        internal const string ScriptEndMarker = "#>";

        /// <summary>
        ///     The marker that is placed directly after the start marker, to mark a class body script, such as a method definition.
        /// </summary>
        internal const string ScriptClassBodyMarker = "+";

        /// <summary>
        ///     The marker that is placed directly after the start marker, to mark an automatic output expression.
        /// </summary>
        internal const string ScriptAutoWriteMarker = "=";

        /// <summary>
        ///     The marker that is placed directly after the start marker, to mark a directive.
        /// </summary>
        internal const string DirectiveMarker = "@ ";

        /// <summary>
        ///     The marker that is placed directly after the directive marker, to mark a template directive.
        /// </summary>
        internal const string TemplateMarker = "template";

        /// <summary>
        ///     The marker that is placed directly after the directive marker, to mark a namespace import directive.
        /// </summary>
        internal const string NamespaceImportMarker = "import";

        /// <summary>
        ///     The marker that is placed directly after the directive marker, to mark a script include directive.
        /// </summary>
        internal const string ScriptIncludeMarker = "include";

        /// <summary>
        ///     The marker that is placed directly after the directive marker, to mark a script property directive.
        /// </summary>
        internal const string ScriptPropertyMarker = "property";

        /// <summary>
        ///     The name of the main method name.
        /// </summary>
        internal const string MainMethodName = "__main";

        /// <summary>
        ///     The name of the Context property.
        /// </summary>
        internal const string ContextPropertyName = "Context";

        /// <summary>
        ///     The name of the Output property.
        /// </summary>
        internal const string OutputPropertyName = "Output";

        /// <summary>
        ///     The name of the Transformer property.
        /// </summary>
        internal const string TransformerPropertyName = "Transformer";

        /// <summary>
        ///     The name of the language attribute.
        /// </summary>
        internal const string LanguageAttributeName = "language";

        /// <summary>
        ///     The name of the file attribute.
        /// </summary>
        internal const string FileAttributeName = "file";

        /// <summary>
        ///     The name of the namespace attribute.
        /// </summary>
        internal const string NamespaceAttributeName = "namespace";

        /// <summary>
        ///     The name of the name attribute.
        /// </summary>
        internal const string NameAttributeName = "name";

        /// <summary>
        ///     The name of the type attribute.
        /// </summary>
        internal const string TypeAttributeName = "type";

        /// <summary>
        ///     The language identifier for C#.
        /// </summary>
        internal const string CSharpLanguageIdentifier = "C#";

        /// <summary>
        ///     The language identifier for Visual Basic.
        /// </summary>
        internal const string VisualBasicLanguageIdentifier = "VB";
    }
}
