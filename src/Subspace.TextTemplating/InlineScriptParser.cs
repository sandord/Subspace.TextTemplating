/*
This code is released under the Creative Commons Attribute 3.0 Unported license.
You are free to share and reuse this code as long as you keep a reference to the author.
See http://creativecommons.org/licenses/by/3.0/
*/

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.CSharp;

using Subspace.TextTemplating.Properties;

namespace Subspace.TextTemplating
{
    /// <summary>
    ///     Provides methods for parsing inline scripts.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Inline scripts are written in C# and delimited by <c>&lt;#</c> and <c>#&gt;</c>.
    ///         Function definitions are delimited however, by <c>&lt;#!</c> and <c>#&gt;</c>.
    ///     </para>
    ///     <para>
    ///         External script files can be included by applying <c>&lt;#@include 'path' #&gt;</c>.
    ///         Please note that external script files may not contain inline code, thus allowing
    ///         class members only. A class is generated internally and should not be declared.
    ///     </para>
    ///     <para>
    ///         The following instances are available by default:
    ///         <list type="table">
    ///             <listheader>
    ///                 <term>Instance</term>
    ///                 <description>Description</description>
    ///             </listheader>
    ///             <item>
    ///                 <term>Context</term>
    ///                 <description>
    ///                     The context object.
    ///                 </description>
    ///             </item>
    ///             <item>
    ///                 <term><see cref="TemplateOutputWriter">Output</see></term>
    ///                 <description>Provides a means to write to the script output.</description>
    ///             </item>
    ///         </list>
    ///     </para>
    /// </remarks>
    public sealed class InlineScriptParser
    {
        private object context;
        private string baseDirectory;
        private List<string> additionalReferences = new List<string>();
        private string inputText;
        private string sourceFilePath;

        private InlineScript documentScript;
        private List<string> scriptParameters = new List<string>();
        private object[] parameterValues = new object[0];

        private static object regexLock;
        private static Regex remarksRegex;
        private static Regex scriptsRegex;

        /// <summary>
        ///     Gets or sets a value indicating whether debug mode is enabled.
        /// </summary>
        public bool DebugMode
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to include source file references in the
        ///     generated code, to allow the Visual Studio debugger to locate the source file.
        /// </summary>
        public bool IncludeSourceFileReferences
        {
            get;
            set;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="InlineScriptParser"/> class.
        /// </summary>
        public InlineScriptParser()
            : this(null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="InlineScriptParser"/> class.
        /// </summary>
        /// <param name="context">The project builder that is building the project.</param>
        /// <exception cref="ArgumentNullException">
        ///     The specified <paramref name="context"/> is <c>null</c>.
        /// </exception>
        public InlineScriptParser(object context)
            : this(context, Environment.CurrentDirectory)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="InlineScriptParser"/> class.
        /// </summary>
        /// <param name="context">The project builder that is building the project.</param>
        /// <param name="baseDirectory">The base directory of the script source files.</param>
        /// <exception cref="ArgumentNullException">
        ///     The specified <paramref name="context"/> is <c>null</c>.
        ///     -or-
        ///     The specified <paramref name="baseDirectory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">The specified <paramref name="baseDirectory"/> is an empty string.</exception>
        public InlineScriptParser(object context, string baseDirectory)
            : this(context, baseDirectory, null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="InlineScriptParser"/> class.
        /// </summary>
        /// <param name="context">The project builder that is building the project.</param>
        /// <param name="additionalReferences">A list of additional library references.</param>
        /// <exception cref="ArgumentNullException">
        ///     The specified <paramref name="context"/> is <c>null</c>.
        /// </exception>
        public InlineScriptParser(object context, IEnumerable<string> additionalReferences)
            : this(context, Environment.CurrentDirectory, additionalReferences)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="InlineScriptParser"/> class.
        /// </summary>
        /// <param name="context">The project builder that is building the project.</param>
        /// <param name="baseDirectory">The base directory of the script source files.</param>
        /// <param name="additionalReferences">A list of additional library references.</param>
        /// <exception cref="ArgumentNullException">
        ///     The specified <paramref name="context"/> is <c>null</c>.
        ///     -or-
        ///     The specified <paramref name="baseDirectory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">The specified <paramref name="baseDirectory"/> is an empty string.</exception>
        public InlineScriptParser(object context, string baseDirectory, IEnumerable<string> additionalReferences)
        {
            if (baseDirectory == null)
            {
                throw new ArgumentNullException("baseDirectory");
            }
            else if (string.IsNullOrWhiteSpace(baseDirectory))
            {
                throw new ArgumentException(InternalExceptionStrings.ArgumentException_EmptyOrWhitespaceString, "baseDirectory");
            }

            Type contextType;
            string contextTypeName = null;
            string contextNamespace = null;

            if (context != null)
            {
                contextType = context.GetType();

                if (contextType.IsSpecialName)
                {
                    throw new ArgumentException(InternalExceptionStrings.ArgumentException_TypeIsSpecialName, "context");
                }
                else if (contextType.IsNotPublic)
                {
                    throw new ArgumentException(InternalExceptionStrings.ArgumentException_TypeIsNotPublic, "context");
                }

                this.context = context;
                contextTypeName = contextType.Name;
                contextNamespace = contextType.Namespace;
            }

            this.baseDirectory = baseDirectory;

            if (additionalReferences != null)
            {
                this.additionalReferences = additionalReferences.ToList();
            }

            documentScript = new InlineScript()
            {
                NamespaceName = Constants.NamespaceName,
                ClassName = Constants.ClassName
            };

            if (context != null)
            {
                if (contextNamespace != null)
                {
                    documentScript.NamespaceReferences.Add(new NamespaceReference(contextNamespace));
                    this.additionalReferences.Add(context.GetType().Assembly.Location);
                }
            }

            if (contextTypeName == null)
            {
                contextTypeName = "object";
            }

            documentScript.AppendFormat("private {0} Context;", contextTypeName);
            documentScript.AppendLine();

            documentScript.AppendLine("private TemplateOutputWriter Output;");

            StringBuilder format = new StringBuilder();
            format.AppendLine("public void {0}(ref TemplateOutputWriter output, {1} context)");
            format.AppendLine("{{");
            format.AppendLine("   Context = context;");
            format.AppendLine("   Output = output;");
            format.AppendLine("}}");

            documentScript.AppendFormat(format.ToString(), Constants.InitializationMethodName, contextTypeName);
        }

        /// <summary>
        ///     Initializes the <see cref="InlineScriptParser"/> class.
        /// </summary>
        static InlineScriptParser()
        {
            remarksRegex = new Regex(
                string.Format(
                    CultureInfo.InvariantCulture,
                    @"{0}--(?<1>)[\s\S]*?--{1}",
                    Constants.ScriptStartMarker,
                    Constants.ScriptEndMarker),
                RegexOptions.Multiline);

            scriptsRegex = new Regex(
                string.Format(
                    CultureInfo.InvariantCulture,
                    @"((?:{0})[\s\S]*?(?:{1}))",
                    Constants.ScriptStartMarker,
                    Constants.ScriptEndMarker),
                RegexOptions.Multiline);

            regexLock = new object();
        }

        /// <summary>
        ///     Transforms the specified file and returns the result.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <exception cref="ArgumentNullException">The specified path is <c>null</c>.</exception>
        public string TransformFile(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            return Transform(File.ReadAllText(path), path);
        }

        /// <summary>
        ///     Transforms the specified text and returns the result.
        /// </summary>
        /// <param name="text">The text to transform.</param>
        /// <param name="sourcePath">The path of the source file.</param>
        /// <exception cref="ArgumentNullException">The specified <paramref name="text"/> is <c>null</c>.</exception>
        public string Transform(string text, string sourcePath)
        {
            Parse(text, sourcePath);

            return Execute(null);
        }

        /// <summary>
        ///     Parses any inline code within the specified <paramref name="text"/>.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <param name="sourceFilePath">The path of the source file.</param>
        /// <exception cref="ArgumentNullException">The specified <paramref name="text"/> is <c>null</c>.</exception>
        private void Parse(string text, string sourceFilePath)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            // Store the specified arguments for later use in the Execute method.
            inputText = text;
            this.sourceFilePath = sourceFilePath;

            lock (regexLock)
            {
                char[] inputChars = inputText.ToCharArray();

                // Disable all scripted remark sections while leaving the character locations
                // within the document intact.
                foreach (Match match in remarksRegex.Matches(inputText))
                {
                    int startIndex = match.Index + Constants.ScriptStartMarker.Length;
                    int endIndex = match.Index + match.Length - Constants.ScriptEndMarker.Length;

                    for (int i = startIndex; i < endIndex; i++)
                    {
                        if (!char.IsControl(inputText[i]))
                        {
                            inputChars[i] = ' ';
                        }
                    }
                }

                inputText = new string(inputChars);

                // Determine the character range for each line.
                Dictionary<int, CharacterRange> lineMap = new Dictionary<int, CharacterRange>();

                int lineNumber = 1;
                int characterIndex = 0;
                string[] lines = inputText.Split(new char[] { '\n' });

                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];

                    if (i < lines.Length - 1)
                    {
                        line += '\n';
                    }

                    lineMap[lineNumber++] = new CharacterRange()
                    {
                        Offset = characterIndex,
                        Length = line.Length
                    };
                    characterIndex += line.Length;
                }

                int index = 0;
                string[] fragments = scriptsRegex.Split(inputText);
                MatchCollection matches = scriptsRegex.Matches(inputText);

                // Parse all fragments.
                foreach (string fragment in fragments)
                {
                    if (!fragment.Equals(Constants.ScriptStartMarker, StringComparison.InvariantCultureIgnoreCase)
                        && !fragment.Equals(Constants.ScriptEndMarker, StringComparison.InvariantCultureIgnoreCase))
                    {
                        ParseFragment(
                            fragment,
                            lineMap.First(n => index >= n.Value.Offset && index < n.Value.Offset + n.Value.Length).Key,
                            sourceFilePath);
                    }

                    index += fragment.Length;
                }
            }
        }

        /// <summary>
        ///     Executes all parsed code and returns the resulting document.
        /// </summary>
        /// <param name="scriptPath">The path of the file to store the resulting script in.</param>
        /// <returns>The resulting document.</returns>
        private string Execute(string scriptPath)
        {
            documentScript.Append(GetAppendParametersScript(scriptParameters));
            documentScript.IncludeSourceFileReferences = IncludeSourceFileReferences;

            string script = documentScript.ComposeScript();

            if (!string.IsNullOrEmpty(scriptPath))
            {
                WriteScript(script, scriptPath);
            }

            CompilerResults results = CompileScript(script);

            foreach (CompilerError error in results.Errors.Cast<CompilerError>().OrderBy(n => n.Line))
            {
                TemplateFileSourceReference sourceReference = new TemplateFileSourceReference()
                {
                    Path = GetSourceFilePath(error.FileName),
                    Line = error.Line
                };

                if (!error.IsWarning)
                {
                    TemplateFileException templateFileException = new TemplateFileException(GetCompilerErrorMessage(error), sourceReference);

                    throw templateFileException;
                }
            }

            Assembly assembly = results.CompiledAssembly;
            TemplateOutputWriter outputWriter = new TemplateOutputWriter();
            object documentScriptsInstance = assembly.CreateInstance(Constants.NamespaceName + "." + Constants.ClassName);

            // Call the parameter intialization method.
            Type classType = assembly.GetType(Constants.NamespaceName + "." + Constants.ClassName);
            MethodInfo initMethod = classType.GetMethod(Constants.ParameterInitializationMethodName);
            initMethod.Invoke(documentScriptsInstance, parameterValues);

            // Call the intialization method.
            classType = assembly.GetType(Constants.NamespaceName + "." + Constants.ClassName);
            initMethod = classType.GetMethod(Constants.InitializationMethodName);
            initMethod.Invoke(documentScriptsInstance, new[] { outputWriter, context });

            // Execute the main method.
            outputWriter.StartCapture();

            MethodInfo mainMethod = classType.GetMethod(InlineScript.MainMethodName);
            mainMethod.Invoke(documentScriptsInstance, new object[] { });

            return outputWriter.EndCapture();
        }

        /// <summary>
        ///     Parses a fragment.
        /// </summary>
        /// <param name="fragment">The fragment to parse.</param>
        /// <param name="lineNumber">The line number at which the fragment is located in the source file.</param>
        /// <param name="sourceFilePath">The path of the source file.</param>
        private void ParseFragment(string fragment, int lineNumber, string sourceFilePath)
        {
            string startMarker = Constants.ScriptStartMarker;
            FragmentType fragmentType;

            if (fragment.Length == 0)
            {
                return;
            }
            else if (!fragment.StartsWith(startMarker))
            {
                fragmentType = FragmentType.Markup;
            }
            // Determine whether the script should automatically write the result to the output.
            else if (fragment.StartsWith(startMarker + Constants.ScriptAutoWriteMarker))
            {
                startMarker += Constants.ScriptAutoWriteMarker;
                fragmentType = FragmentType.AutoWriteScript;
            }
            // Determine whether the script is a function definition.
            else if (fragment.StartsWith(startMarker + Constants.ScriptClassBodyMarker))
            {
                startMarker += Constants.ScriptClassBodyMarker;
                fragmentType = FragmentType.ClassBody;
            }
            // Determine whether the script is an include statement.
            else if (fragment.StartsWith(startMarker + Constants.ScriptIncludeMarker))
            {
                startMarker += Constants.ScriptIncludeMarker;
                fragmentType = FragmentType.Include;
            }
            // Determine whether the script is a namespace import statement.
            else if (fragment.StartsWith(startMarker + Constants.NamespaceImportMarker))
            {
                startMarker += Constants.NamespaceImportMarker;
                fragmentType = FragmentType.NamespaceImport;
            }
            else if (fragment.StartsWith(startMarker + Constants.ScriptParameterMarker))
            {
                startMarker += Constants.ScriptParameterMarker;
                fragmentType = FragmentType.ScriptParameter;
            }
            else
            {
                fragmentType = FragmentType.Script;
            }

            // Strip start and end markers.
            if (fragmentType != FragmentType.Markup)
            {
                fragment = fragment.Substring(startMarker.Length);
                fragment = fragment.Substring(0, fragment.Length - Constants.ScriptEndMarker.Length);
                fragment = fragment.Trim();
            }

            // Append the code to the document script.
            if (fragmentType == FragmentType.Script)
            {
                documentScript.MainMethodScript.AppendFragment(fragment, lineNumber, sourceFilePath);
            }
            else if (fragmentType == FragmentType.AutoWriteScript)
            {
                documentScript.MainMethodScript.AppendFragment(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Output.Write({0});",
                        fragment),
                    lineNumber,
                    sourceFilePath);

                documentScript.MainMethodScript.AppendLine();
            }
            else if (fragmentType == FragmentType.ClassBody)
            {
                documentScript.AppendFragment(fragment, lineNumber, sourceFilePath);
            }
            else if (fragmentType == FragmentType.Include)
            {
                string path = ExtractAttribute(fragment, "file");
                path = Path.Combine(baseDirectory, path.Trim(new char[] { '\'', '\"' }));

                ParseFragment(File.ReadAllText(path), 1, path);
            }
            else if (fragmentType == FragmentType.NamespaceImport)
            {
                documentScript.NamespaceReferences.Add(
                    new NamespaceReference(ExtractAttribute(fragment, "namespace")));
            }
            else if (fragmentType == FragmentType.ScriptParameter)
            {
                scriptParameters.Add(ExtractAttribute(fragment, "name"));
            }
            else if (fragmentType == FragmentType.Markup)
            {
                while (fragment.StartsWith(Environment.NewLine, StringComparison.Ordinal))
                {
                    documentScript.MainMethodScript.AppendFormat(
                        "Output.WriteLine();",
                        fragment.Replace("\"", "\"\""));

                    documentScript.MainMethodScript.AppendLine();
                    fragment = fragment.Substring(Environment.NewLine.Length);
                }

                if (fragment.Length > 0)
                {
                    documentScript.MainMethodScript.AppendFormat(
                        "Output.Write(@\"{0}\");",
                        fragment.Replace("\"", "\"\""));
                }

                documentScript.MainMethodScript.AppendLine();
            }
        }

        /// <summary>
        ///     Extracts the value of the specified attribute from the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <returns>The extracted attribute value.</returns>
        private static string ExtractAttribute(string text, string attributeName)
        {
            Regex regex = new Regex(
                string.Format(
                    CultureInfo.InvariantCulture,
                    @"\b({0})\s*=\s*""(?<value>([^""])*)""",
                    attributeName), RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

            return regex.Match(text).Groups["value"].Value;
        }

        /// <summary>
        ///     Returns an array of required assembly reference names.
        /// </summary>
        /// <returns>An array of assembly reference names.</returns>
        private string[] GetAssemblyReferences()
        {
            List<string> references = new List<string>();

            references.Add(Assembly.GetExecutingAssembly().Location);

            // Reference System.dll by default.
            references.Add(Assembly.GetAssembly(typeof(System.Uri)).Location);

            references.AddRange(additionalReferences);

            return references.ToArray();
        }

        /// <summary>
        ///     Writes the specified script to a file at the specified path.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <param name="path">The file path.</param>
        private static void WriteScript(string script, string path)
        {
            string scriptDirectory = Path.GetDirectoryName(path);

            if (!Directory.Exists(scriptDirectory))
            {
                Directory.CreateDirectory(scriptDirectory);
            }

            StringBuilder output = new StringBuilder();
            output.Append("// ");
            output.AppendLine(new string('*', 94));
            output.Append("// ");
            output.AppendLine(Resources.FileGeneratedByTool);
            output.Append("// ");
            output.AppendLine(new string('*', 94));
            output.AppendLine();
            output.Append(script);

            File.WriteAllText(path, script);
        }

        /// <summary>
        ///     Returns the script that appends the parameters.
        /// </summary>
        /// <param name="scriptParameters">The script parameters.</param>
        /// <returns>The script.</returns>
        private static string GetAppendParametersScript(List<string> scriptParameters)
        {
            StringBuilder script = new StringBuilder();
            StringBuilder parameterDefinitions = new StringBuilder();
            StringBuilder parameterAssignments = new StringBuilder();

            foreach (string scriptParameter in scriptParameters)
            {
                string[] parts = scriptParameter.Split(new char[] { ':' });
                string name = parts[0].Trim();
                string type = parts[1].Trim();

                script.AppendLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "private {0} {1};",
                        type,
                        name));

                if (parameterDefinitions.Length > 0)
                {
                    parameterDefinitions.Append(", ");
                }

                parameterDefinitions.AppendFormat(
                    CultureInfo.InvariantCulture,
                    "{0} _{1}",
                    type,
                    name);

                parameterAssignments.AppendFormat(
                    CultureInfo.InvariantCulture,
                    "{0} = _{0}{1};",
                    name,
                    Environment.NewLine);
            }

            script.Append(
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

            script.AppendLine();

            return script.ToString();
        }

        /// <summary>
        ///     Compiles the specified script.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <returns>The compiler results.</returns>
        private CompilerResults CompileScript(string script)
        {
            CSharpCodeProvider provider = new CSharpCodeProvider(
                new Dictionary<string, string>()
                {
                    { "CompilerVersion", Constants.InlineScriptingCompilerVersion }
                });

            CompilerParameters compilerParameters = new CompilerParameters();
            compilerParameters.GenerateExecutable = false;
            compilerParameters.GenerateInMemory = true;
            compilerParameters.ReferencedAssemblies.AddRange(GetAssemblyReferences());
            compilerParameters.IncludeDebugInformation = DebugMode;
            compilerParameters.WarningLevel = (DebugMode ? 3 : 1);
            compilerParameters.CompilerOptions = (DebugMode ? "" : "/optimize");

            return provider.CompileAssemblyFromSource(compilerParameters, script);
        }

        /// <summary>
        ///     Gets the compiler error or warning message composed from the specified compiler
        ///     error.
        /// </summary>
        /// <param name="error">The compiler error.</param>
        /// <returns>The compiler error message.</returns>
        private string GetCompilerErrorMessage(CompilerError error)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string sourceFilePath = GetSourceFilePath(error.FileName);

            stringBuilder.AppendFormat(
                error.IsWarning ? Resources.CompilerWarningFormatted : Resources.CompilerErrorFormatted,
                error.ErrorNumber,
                sourceFilePath,
                error.Line > 0 ? error.Line : 0);

            stringBuilder.AppendLine();
            stringBuilder.AppendLine(error.ErrorText);

            return stringBuilder.ToString();
        }

        /// <summary>
        ///     Returns the source file path based on the specified filename, which may be a GUID
        ///     that can be resolved to the original source path.
        /// </summary>
        /// <param name="filename">The filename or source reference GUID string.</param>
        /// <returns>The source file path.</returns>
        private string GetSourceFilePath(string filename)
        {
            if (IncludeSourceFileReferences && !File.Exists(filename))
            {
                // Apparently, the filename refers to a non existing file so assume it is a GUID
                // that references the source path indirectly.
                Guid sourceFileGuid;

                if (Guid.TryParse(filename.Substring(filename.LastIndexOf(Path.DirectorySeparatorChar) + 1), out sourceFileGuid))
                {
                    if (sourceFileGuid != Guid.Empty)
                    {
                        filename = documentScript.GetSourceFilePath(sourceFileGuid);
                    }
                }
            }

            return filename;
        }
    }
}
