using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Subspace.TextTemplating.ScriptBuilding
{
    /// <summary>
    ///     Provides means of building a script that is suitable for execution.
    /// </summary>
    internal abstract class ScriptBuilder : IOuterScriptBuilder, IInnerScriptBuilder
    {
        private static readonly object sourceFilePathsLock = new object();
        private static IDictionary<Guid, string> sourceFilePaths = new Dictionary<Guid, string>();

        private string namespaceName;
        private string className;
        private IInnerScriptBuilder _innerScriptBuilder;

        /// <summary>
        ///     Gets a value indicating whether this instance is empty.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return Script.Length == 0;
            }
        }

        /// <summary>
        ///     Gets the inner script builder.
        /// </summary>
        public IInnerScriptBuilder InnerScriptBuilder
        {
            get
            {
                if (_innerScriptBuilder == null)
                {
                    _innerScriptBuilder = ScriptBuilderFactory.Create(ScriptLanguage, namespaceName, className);
                }

                return _innerScriptBuilder;
            }
        }

        /// <summary>
        ///     Gets the namespace references to include in the script output.
        /// </summary>
        public List<NamespaceReference> NamespaceReferences
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets or sets value indicating whether to include source file references in the
        ///     generated code.
        /// </summary>
        public bool IncludeSourceFileReferences
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets the name of the main method.
        /// </summary>
        public string MainMethodName
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the script language.
        /// </summary>
        protected abstract ScriptLanguage ScriptLanguage
        {
            get;
        }

        /// <summary>
        ///     Gets the script string builder.
        /// </summary>
        protected StringBuilder Script
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the script nesting stack.
        /// </summary>
        protected Stack<string> ScriptNestingStack
        {
            get;
            private set;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ScriptBuilder"/> class.
        /// </summary>
        /// <param name="namespaceName">The namespace name.</param>
        /// <param name="className">The class name.</param>
        public ScriptBuilder(string namespaceName, string className)
        {
            this.namespaceName = namespaceName;
            this.className = className;

            Script = new StringBuilder();
            ScriptNestingStack = new Stack<string>();
            MainMethodName = Constants.MainMethodName;
            NamespaceReferences = new List<NamespaceReference>();
        }

        /// <summary>
        ///     Appends an empty line.
        /// </summary>
        public void AppendLine()
        {
            Script.AppendLine();
        }

        /// <summary>
        ///     Appends the specified line.
        /// </summary>
        /// <param name="text">The text to write.</param>
        public void AppendLine(string text)
        {
            Script.AppendLine(text);
        }

        /// <summary>
        ///     Appends the specified script fragment.
        /// </summary>
        /// <param name="fragment">The fragment.</param>
        /// <param name="lineNumber">The line number of the first line of the fragment.</param>
        /// <param name="sourceFilePath">The path of the source file.</param>
        /// <returns>The number of lines that were extracted from the fragment.</returns>
        /// <exception cref="ArgumentNullException">The specified <paramref name="fragment"/> is <c>null</c>.</exception>
        public int AppendScriptFragment(string fragment, int lineNumber, string sourceFilePath)
        {
            if (fragment == null)
            {
                throw new ArgumentNullException("fragment");
            }
            else if (lineNumber < 0)
            {
                throw new ArgumentOutOfRangeException("lineNumber");
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
                    AppendSourceFileReference(sourceFilePath, lineNumber);
                }

                Script.Append(lines[i]);

                if (i < lines.Length - 1)
                {
                    Script.Append("\n");
                }

                lineNumber++;
            }

            return lines.Length;
        }

        /// <summary>
        ///     Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            if (!InnerScriptBuilder.IsEmpty)
            {
                string script = this.Script.ToString();
                this.Script.Clear();

                foreach (NamespaceReference namespaceReference in NamespaceReferences)
                {
                    AppendNamespaceReference(namespaceReference);
                }

                AppendNamespaceDeclaration(namespaceName);
                AppendClassDefinition(className);
                AppendLine(script);
                AppendMainMethod(MainMethodName);

                while (ScriptNestingStack.Count > 0)
                {
                    AppendNestingTerminator(ScriptNestingStack.Pop());
                }
            }

            return Script.ToString();
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
        public string GetSourceFilePath(Guid guid)
        {
            lock (sourceFilePathsLock)
            {
                return sourceFilePaths[guid];
            }
        }

        /// <summary>
        ///     Returns the specified text as a remark.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The remark.</returns>
        public abstract string GetAsRemark(string text);

        /// <summary>
        ///     Appends the private property declaration.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="typeName">The name of the type.</param>
        public abstract void AppendPrivatePropertyDeclaration(string propertyName, string typeName);

        /// <summary>
        ///     Appends the initialization method script.
        /// </summary>
        /// <param name="contextTypeName">The name of the context type.</param>
        public abstract void AppendInitializationMethodScript(string contextTypeName);

        /// <summary>
        ///     Appends the script that appends the specified properties.
        /// </summary>
        /// <param name="properties">The properties.</param>
        public abstract void AppendPropertiesScript(List<ScriptProperty> properties);

        /// <summary>
        ///     Appends the output write script that writes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        public abstract void AppendOutputWriteScript(string text);

        /// <summary>
        ///     Appends the output write script that writes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        public abstract void AppendOutputWriteScript(string text, int lineNumber, string sourceFilePath);

        /// <summary>
        ///     Appends the output write script that writes an empty line.
        /// </summary>
        public abstract void AppendOutputWriteLineScript();

        /// <summary>
        ///     Appends the output write script that writes the specified text as a line.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        public abstract void AppendOutputWriteLineScript(string text, int lineNumber, string sourceFilePath);

        /// <summary>
        ///     Appends the output write script that writes the specified literal text as a line.
        /// </summary>
        /// <param name="text">The text.</param>
        public abstract void AppendOutputWriteLineScriptLiteral(string text);

        /// <summary>
        ///     Appends a source file reference.
        /// </summary>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <param name="lineNumber">The line number.</param>
        protected abstract void AppendSourceFileReference(string sourceFilePath, int lineNumber);

        /// <summary>
        ///     Appends the specified namespace reference.
        /// </summary>
        /// <param name="namespaceReference">The namespace reference.</param>
        protected abstract void AppendNamespaceReference(NamespaceReference namespaceReference);

        /// <summary>
        ///     Appends the namespace declaration.
        /// </summary>
        /// <param name="namespaceName">The name of the namespace.</param>
        protected abstract void AppendNamespaceDeclaration(string namespaceName);

        /// <summary>
        ///     Appends the class definition.
        /// </summary>
        /// <param name="className">The class name.</param>
        protected abstract void AppendClassDefinition(string className);

        /// <summary>
        ///     Appends the main method.
        /// </summary>
        /// <param name="mainMethodName">The name of the main method.</param>
        protected abstract void AppendMainMethod(string mainMethodName);

        /// <summary>
        ///     Appends a nesting terminator.
        /// </summary>
        /// <param name="tag">The nesting tag.</param>
        protected abstract void AppendNestingTerminator(string tag);

        /// <summary>
        ///     Returns a value indicating whether the specified path is a local path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c>, if the path is local;<c>false</c>, otherwise.</returns>
        private bool GetIsLocalPath(string path)
        {
            //TODO: implement
            return true;
        }
    }
}
