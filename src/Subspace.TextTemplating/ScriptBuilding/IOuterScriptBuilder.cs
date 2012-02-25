// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Subspace" file="IOuterScriptBuilder.cs">
//   Copyright (c) Subspace. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Subspace.TextTemplating.ScriptBuilding
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     Defines the public members of a class that provides means of building an outer script,
    ///     which is a script that manages an inner script, which in turn represents a text
    ///     template.
    /// </summary>
    internal interface IOuterScriptBuilder
    {
        /// <summary>
        ///     Gets the inner script builder.
        /// </summary>
        IInnerScriptBuilder InnerScriptBuilder
        {
            get;
        }

        /// <summary>
        ///     Gets the name of the main method.
        /// </summary>
        string MainMethodName
        {
            get;
        }

        /// <summary>
        ///     Gets the namespace references to include in the script output.
        /// </summary>
        List<NamespaceReference> NamespaceReferences
        {
            get;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to include source file references in the
        ///     generated code.
        /// </summary>
        bool IncludeSourceFileReferences
        {
            get;
            set;
        }

        /// <summary>
        ///     Returns the source file path that matches the specified GUID.
        /// </summary>
        /// <param name="guid">The GUID of the source file path.</param>
        /// <returns>The source file path.</returns>
        /// <remarks>
        ///     This method is intended to resolve GUIDs that are found in the
        ///     <see cref="System.CodeDom.Compiler.CompilerError.FileName"/> property.
        /// </remarks>
        string GetSourceFilePath(Guid guid);

        /// <summary>
        ///     Returns the specified text as a remark.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The remark.</returns>
        string GetAsRemark(string text);

        /// <summary>
        ///     Appends an empty line.
        /// </summary>
        void AppendLine();

        /// <summary>
        ///     Appends the specified script fragment.
        /// </summary>
        /// <param name="fragment">The fragment.</param>
        /// <param name="lineNumber">The line number of the first line of the fragment.</param>
        /// <param name="sourceFilePath">The path of the source file.</param>
        /// <returns>The number of lines that were extracted from the fragment.</returns>
        /// <exception cref="ArgumentNullException">The specified <paramref name="fragment"/> is <c>null</c>.</exception>
        int AppendScriptFragment(string fragment, int lineNumber, string sourceFilePath);

        /// <summary>
        ///     Appends the private property declaration.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="typeName">The name of the type.</param>
        void AppendPrivatePropertyDeclaration(string propertyName, string typeName);

        /// <summary>
        ///     Appends the initialization method script.
        /// </summary>
        /// <param name="contextTypeName">The name of the context type.</param>
        void AppendInitializationMethodScript(string contextTypeName);

        /// <summary>
        ///     Appends the script that appends the specified properties.
        /// </summary>
        /// <param name="properties">The properties.</param>
        void AppendPropertiesScript(List<ScriptProperty> properties);
    }
}
