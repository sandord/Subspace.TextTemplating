using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Subspace.TextTemplating
{
    /// <summary>
    ///     Provides a means of building an inline template script that is suitable for direct
    ///     execution.
    /// </summary>
    internal sealed class InlineScript
    {
        private StringBuilder script;

        private static readonly object sourceFilePathsLock = new object();
        private static IDictionary<Guid, string> sourceFilePaths = new Dictionary<Guid, string>();

        private InlineScript _mainMethodScript;

        /// <summary>
        ///     The name of the main method.
        /// </summary>
        internal static readonly string MainMethodName = "__main";

        /// <summary>
        ///     Gets or sets the name of the namespace the script will be contained in.
        /// </summary>
        internal string NamespaceName
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the name of the class the script will be contained in.
        /// </summary>
        internal string ClassName
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets the namespace references to include in the script output.
        /// </summary>
        internal List<NamespaceReference> NamespaceReferences
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the main method script.
        /// </summary>
        internal InlineScript MainMethodScript
        {
            get
            {
                if (_mainMethodScript == null)
                {
                    _mainMethodScript = new InlineScript();
                }

                return _mainMethodScript;
            }
        }

        /// <summary>
        ///     Gets or sets value indicating whether to include source file references in the
        ///     generated code.
        /// </summary>
        internal bool IncludeSourceFileReferences
        {
            get;
            set;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="InlineScript"/> class.
        /// </summary>
        internal InlineScript()
        {
            script = new StringBuilder();
            NamespaceReferences = new List<NamespaceReference>();
        }

        /// <summary>
        ///     Appends an empty line.
        /// </summary>
        internal void AppendLine()
        {
            script.AppendLine();
        }

        /// <summary>
        ///     Appends the specified line.
        /// </summary>
        /// <param name="line">The line to append.</param>
        internal void AppendLine(string line)
        {
            script.AppendLine(line);
        }

        /// <summary>
        ///     Appends the specified text.
        /// </summary>
        /// <param name="text">The text to append.</param>
        internal void Append(string text)
        {
            script.Append(text);
        }

        /// <summary>
        ///     Appends the specified format.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An array of objects to format.</param>
        internal void AppendFormat(string format, params object[] args)
        {
            script.AppendFormat(CultureInfo.InvariantCulture, format, args);
        }

        /// <summary>
        ///     Appends the specified fragment.
        /// </summary>
        /// <param name="fragment">The fragment.</param>
        /// <param name="lineNumber">The line number of the first line of the fragment.</param>
        /// <param name="sourceFilePath">The path of the source file.</param>
        /// <returns>The number of lines that were extracted from the fragment.</returns>
        /// <exception cref="ArgumentNullException">The specified <paramref name="fragment"/> is <c>null</c>.</exception>
        internal int AppendFragment(string fragment, int lineNumber, string sourceFilePath)
        {
            if (fragment == null)
            {
                throw new ArgumentNullException("fragment");
            }

            if (IncludeSourceFileReferences && !GetIsLocalPath(sourceFilePath))
            {
                // Write the GUID of the registered source file path instead of the path itself.
                // This circumvents the apparent issue of the compiler not allowing network
                // locations that do not use drive letters (\\Server\path) resulting in the
                // FileName/Row/Index properties of the CompilerError object to be empty.
                // The GUID can be remapped back to the appropriate path using the
                // GetSourceFilePath method.
                lock (sourceFilePathsLock)
                {
                    Guid guid;

                    if (!sourceFilePaths.Any(n => n.Value.Equals(sourceFilePath, StringComparison.OrdinalIgnoreCase)))
                    {
                        guid = Guid.NewGuid();
                        sourceFilePaths.Add(guid, sourceFilePath);
                    }
                    else
                    {
                        guid = sourceFilePaths.FirstOrDefault(n => n.Value.Equals(sourceFilePath, StringComparison.OrdinalIgnoreCase)).Key;
                    }

                    sourceFilePath = guid.ToString();

                    // A copy of the source file is maintained in the temp directory for the debugger
                    // being able to find it.
                    string tempSourcePath = Path.Combine(Path.GetTempPath(), guid.ToString());

                    if (!File.Exists(tempSourcePath))
                    {
                        File.Copy(GetSourceFilePath(guid), tempSourcePath);
                    }
                }
            }

            string[] lines = fragment.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                if (IncludeSourceFileReferences && lines[i].Length > 0)
                {
                    script.AppendLine();
                    script.AppendFormat(CultureInfo.InvariantCulture, "#line {0} \"{1}\"", lineNumber, sourceFilePath);
                    script.AppendLine();
                }

                Append(lines[i]);

                if (i < lines.Length - 1)
                {
                    Append("\n");
                }

                lineNumber++;
            }

            return lines.Length;
        }

        /// <summary>
        ///     Returns the source file path that matches the specified GUID.
        /// </summary>
        /// <param name="guid">The GUID of the source file path.</param>
        /// <returns>The source file path.</returns>
        /// <remarks>
        ///     This method is intended to resolve GUIDs that are found in the
        ///     <see cref="CompilerError.FileName"/> property.
        /// </remarks>
        internal string GetSourceFilePath(Guid guid)
        {
            lock (sourceFilePathsLock)
            {
                return sourceFilePaths[guid];
            }
        }

        /// <summary>
        ///     Returns the composed script.
        /// </summary>
        /// <returns>The script.</returns>
        internal string ComposeScript()
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

            if (_mainMethodScript != null)
            {
                output.AppendFormat(
                    CultureInfo.InvariantCulture,
                    "public void {0}() {{",
                    MainMethodName);

                output.AppendLine();
                output.AppendLine(MainMethodScript.ComposeScript());
                output.AppendLine("}");
            }

            output.AppendLine(script.ToString());
            output.AppendLine();

            while (nestingLevel-- > 0)
            {
                output.Append("}");
            }

            return output.ToString();
        }

        /// <summary>
        ///     Returns a value indicating whether the specified path is a local path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c>, if the path is local;<c>false</c>, otherwise.</returns>
        private bool GetIsLocalPath(string path)
        {
            return true;
        }
    }
}
