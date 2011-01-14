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
        ///     Returns the composed script.
        /// </summary>
        /// <returns>The script.</returns>
        internal override string ComposeScript()
        {
            StringBuilder output = new StringBuilder();
            int nestingLevel = 0;

            foreach (NamespaceReference namespaceReference in NamespaceReferences)
            {
                output.AppendFormat(
                    CultureInfo.InvariantCulture,
                    "using {0};",
                    namespaceReference.Namespace);

                output.AppendLine();
            }

            if (!string.IsNullOrEmpty(NamespaceName))
            {
                output.AppendFormat(
                    CultureInfo.InvariantCulture,
                    "namespace {0} {{",
                    NamespaceName);

                output.AppendLine();
                nestingLevel++;
            }

            if (!string.IsNullOrEmpty(NamespaceName))
            {
                output.AppendFormat(
                    CultureInfo.InvariantCulture,
                    "public sealed class {0} {{",
                    ClassName);

                output.AppendLine();
                nestingLevel++;
            }

            if (HasMainMethodScript)
            {
                output.AppendFormat(
                    CultureInfo.InvariantCulture,
                    "public void {0}() {{",
                    MainMethodName);

                output.AppendLine();
                output.AppendLine(MainMethodScript.ComposeScript());
                output.AppendLine("}");
            }

            output.AppendLine(Script.ToString());
            output.AppendLine();

            while (nestingLevel-- > 0)
            {
                output.Append("}");
            }

            return output.ToString();
        }

        /// <summary>
        ///     Writes a source file reference.
        /// </summary>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <param name="lineNumber">The line number.</param>
        protected override void WriteSourceFileReference(string sourceFilePath, int lineNumber)
        {
            Script.AppendLine();
            Script.AppendFormat(CultureInfo.InvariantCulture, "#line {0} \"{1}\"", lineNumber, sourceFilePath);
            Script.AppendLine();
        }
    }
}
