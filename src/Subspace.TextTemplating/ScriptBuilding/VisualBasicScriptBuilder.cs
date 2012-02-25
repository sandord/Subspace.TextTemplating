// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Subspace" file="VisualBasicScriptBuilder.cs">
//   Copyright (c) Subspace. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Subspace.TextTemplating.ScriptBuilding
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     Provides a means of building a Visual Basic script that is suitable for execution.
    /// </summary>
    internal sealed class VisualBasicScriptBuilder : ScriptBuilder
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="VisualBasicScriptBuilder"/> class.
        /// </summary>
        /// <param name="namespaceName">The namespace name.</param>
        /// <param name="className">The class name.</param>
        internal VisualBasicScriptBuilder(string namespaceName, string className)
            : base(namespaceName, className)
        {
        }

        /// <summary>
        ///     Gets the script language.
        /// </summary>
        protected override ScriptLanguage ScriptLanguage
        {
            get
            {
                return ScriptLanguage.VisualBasic;
            }
        }

        /// <summary>
        ///     Returns the specified text as a remark.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The remark.</returns>
        public override string GetAsRemark(string text)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Appends the script that appends the specified properties.
        /// </summary>
        /// <param name="properties">The properties.</param>
        public override void AppendPropertiesScript(List<ScriptProperty> properties)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Appends the output write script that writes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        public override void AppendOutputWriteScript(string text)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Appends the output write script that writes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        public override void AppendOutputWriteScript(string text, int lineNumber, string sourceFilePath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Appends the output write script that writes an empty line.
        /// </summary>
        public override void AppendOutputWriteLineScript()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Appends the output write script that writes the specified text as a line.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        public override void AppendOutputWriteLineScript(string text, int lineNumber, string sourceFilePath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Appends the output write script that writes the specified literal text as a line.
        /// </summary>
        /// <param name="text">The text.</param>
        public override void AppendOutputWriteLineScriptLiteral(string text)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Appends the private property declaration.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="typeName">The name of the type.</param>
        public override void AppendPrivatePropertyDeclaration(string propertyName, string typeName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Appends the initialization method script.
        /// </summary>
        /// <param name="contextTypeName">The name of the context type.</param>
        public override void AppendInitializationMethodScript(string contextTypeName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Appends the specified namespace reference.
        /// </summary>
        /// <param name="namespaceReference">The namespace reference.</param>
        protected override void AppendNamespaceReference(NamespaceReference namespaceReference)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Appends the namespace declaration.
        /// </summary>
        /// <param name="namespaceName">The name of the namespace.</param>
        protected override void AppendNamespaceDeclaration(string namespaceName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Appends the class definition.
        /// </summary>
        /// <param name="className">The class name.</param>
        protected override void AppendClassDefinition(string className)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Appends the main method.
        /// </summary>
        /// <param name="mainMethodName">The name of the main method.</param>
        protected override void AppendMainMethod(string mainMethodName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Appends a nesting terminator.
        /// </summary>
        /// <param name="tag">The nesting tag.</param>
        protected override void AppendNestingTerminator(string tag)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Appends a source file reference.
        /// </summary>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <param name="lineNumber">The line number.</param>
        protected override void AppendSourceFileReference(string sourceFilePath, int lineNumber)
        {
            throw new NotImplementedException();
        }
    }
}
