using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Subspace.TextTemplating.ScriptBuilding
{
    /// <summary>
    ///     Provides a means of building a C# script that is suitable for execution.
    /// </summary>
    internal sealed class CSharpScriptBuilder : ScriptBuilder
    {
        /// <summary>
        ///     Gets the script language.
        /// </summary>
        protected override ScriptLanguage ScriptLanguage
        {
            get
            {
                return ScriptLanguage.CSharp;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CSharpScriptBuilder"/> class.
        /// </summary>
        /// <param name="namespaceName">The namespace name.</param>
        /// <param name="className">The class name.</param>
        internal CSharpScriptBuilder(string namespaceName, string className)
            : base(namespaceName, className)
        {
        }

        /// <summary>
        ///     Returns the specified text as a remark.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The remark.</returns>
        public override string GetAsRemark(string text)
        {
            return "// " + text ?? string.Empty;
        }

        /// <summary>
        ///     Appends the script that appends the specified parameters.
        /// </summary>
        /// <param name="scriptParameters">The script parameters.</param>
        public override void AppendParametersScript(List<ScriptParameter> scriptParameters)
        {
            StringBuilder parameterDefinitions = new StringBuilder();
            StringBuilder parameterAssignments = new StringBuilder();

            foreach (ScriptParameter scriptParameter in scriptParameters)
            {
                AppendPrivatePropertyDeclaration(scriptParameter.Name, scriptParameter.TypeName);

                if (parameterDefinitions.Length > 0)
                {
                    parameterDefinitions.Append(", ");
                }

                parameterDefinitions.AppendFormat(
                    CultureInfo.InvariantCulture,
                    "{0} {1}",
                    scriptParameter.TypeName,
                    scriptParameter.Name);

                parameterAssignments.AppendFormat(
                    CultureInfo.InvariantCulture,
                    "this.{0} = {0};",
                    scriptParameter.Name);
            }

            Script.Append(
                string.Format(
                    CultureInfo.InvariantCulture,
                    @"
                    public void {0}({1})
                    {{
                        {2}
                    }}                    
                    ",
                    Constants.ParameterInitializationMethodName,
                    parameterDefinitions.ToString(),
                    parameterAssignments.ToString()));

            Script.AppendLine();
        }

        /// <summary>
        ///     Appends the output write script that writes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        public override void AppendOutputWriteScript(string text)
        {
            Script.AppendFormat(
                CultureInfo.InvariantCulture,
                "Output.Write({0});",
                text);

            Script.AppendLine();
        }

        /// <summary>
        ///     Appends the output write script that writes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        public override void AppendOutputWriteScript(string text, int lineNumber, string sourceFilePath)
        {
            AppendScriptFragment(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Output.Write(@\"{0}\");",
                    text.Replace("\"", "\"\"")),
                lineNumber,
                sourceFilePath);

            Script.AppendLine();
        }

        /// <summary>
        ///     Appends the output write script that writes an empty line.
        /// </summary>
        public override void AppendOutputWriteLineScript()
        {
            Script.AppendLine("Output.WriteLine();");
            Script.AppendLine();
        }

        /// <summary>
        ///     Appends the output write script that writes the specified text as a line.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        public override void AppendOutputWriteLineScript(string text, int lineNumber, string sourceFilePath)
        {
            AppendScriptFragment(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Output.Write({0});",
                    text),
                lineNumber,
                sourceFilePath);

            Script.AppendLine();
        }

        /// <summary>
        ///     Appends the output write script that writes the specified literal text as a line.
        /// </summary>
        /// <param name="text">The text.</param>
        public override void AppendOutputWriteLineScriptLiteral(string text)
        {
            Script.AppendLine(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Output.Write(@\"{0}\");",
                    text.Replace("\"", "\"\"")));
        }

        /// <summary>
        ///     Appends the private property declaration.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="typeName">The name of the type.</param>
        public override void AppendPrivatePropertyDeclaration(string propertyName, string typeName)
        {
            Script.AppendFormat("private {0} {1};", typeName, propertyName);
        }

        /// <summary>
        ///     Appends the initialization method script.
        /// </summary>
        /// <param name="contextTypeName">The name of the context type.</param>
        public override void AppendInitializationMethodScript(string contextTypeName)
        {
            StringBuilder format = new StringBuilder();

            format.AppendLine("public void {0}(ref {1} output, {2} context, {3} transformer)");
            format.AppendLine("{{");
            format.AppendLine("   {4} = context;");
            format.AppendLine("   {5} = output;");
            format.AppendLine("   {6} = transformer;");
            format.AppendLine("}}");

            Script.AppendFormat(
                CultureInfo.InvariantCulture,
                format.ToString(),
                Constants.InitializationMethodName,
                typeof(TextTemplateOutputWriter).Name,
                contextTypeName,
                typeof(TextTemplateTransformer).Name,
                Constants.ContextPropertyName,
                Constants.OutputPropertyName,
                Constants.TransformerPropertyName);
        }

        /// <summary>
        ///     Appends the specified namespace reference.
        /// </summary>
        /// <param name="namespaceReference">The namespace reference.</param>
        protected override void AppendNamespaceReference(NamespaceReference namespaceReference)
        {
            Script.AppendFormat(CultureInfo.InvariantCulture, "using {0};", namespaceReference.Namespace);
            Script.AppendLine();
        }

        /// <summary>
        ///     Appends the namespace declaration.
        /// </summary>
        /// <param name="namespaceName">The name of the namespace.</param>
        protected override void AppendNamespaceDeclaration(string namespaceName)
        {
            Script.AppendFormat(CultureInfo.InvariantCulture, "namespace {0} {{", namespaceName);
            Script.AppendLine();

            ScriptNestingStack.Push(null);
        }

        /// <summary>
        ///     Appends the class definition.
        /// </summary>
        /// <param name="className">The class name.</param>
        protected override void AppendClassDefinition(string className)
        {
            Script.AppendFormat(CultureInfo.InvariantCulture, "public sealed class {0} {{", className);
            Script.AppendLine();

            ScriptNestingStack.Push(null);
        }

        /// <summary>
        ///     Appends the main method.
        /// </summary>
        /// <param name="mainMethodName">The name of the main method.</param>
        protected override void AppendMainMethod(string mainMethodName)
        {
            Script.AppendFormat(CultureInfo.InvariantCulture, "public void {0}() {{", mainMethodName);

            Script.AppendLine();
            Script.AppendLine(InnerScriptBuilder.ToString());
            Script.AppendLine("}");
        }

        /// <summary>
        ///     Appends a nesting terminator.
        /// </summary>
        /// <param name="tag">The nesting tag.</param>
        protected override void AppendNestingTerminator(string tag)
        {
            Script.AppendLine();
            Script.Append("}");
        }

        /// <summary>
        ///     Appends a source file reference.
        /// </summary>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <param name="lineNumber">The line number.</param>
        protected override void AppendSourceFileReference(string sourceFilePath, int lineNumber)
        {
            Script.AppendLine();
            Script.AppendFormat(CultureInfo.InvariantCulture, "#line {0} \"{1}\"", lineNumber, sourceFilePath);
            Script.AppendLine();
        }
    }
}
