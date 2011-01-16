using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using Subspace.TextTemplating.Properties;
using Subspace.TextTemplating.ScriptBuilding;

namespace Subspace.TextTemplating
{
    /// <summary>
    ///     Transforms text templates at runtime.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Text templates allow for embedded progam code delimited by <c>&lt;#</c> and
    ///         <c>#&gt;</c>. Function definitions and other fragments that should end up in the
    ///         body of the generated class, are delimited however, by <c>&lt;#+</c> and
    ///         <c>#&gt;</c>.
    ///     </para>
    ///     <para>
    ///         External template files can be included by applying <c>&lt;# @include path="..." #&gt;</c>.
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
    ///                 <term><see cref="TextTemplateOutputWriter">Output</see></term>
    ///                 <description>Provides a means to write to the script output.</description>
    ///             </item>
    ///         </list>
    ///     </para>
    /// </remarks>
    public sealed class TextTemplateTransformer : IInlineTransformer
    {
        private object context;
        private string baseDirectory;
        private string scriptDirectory;
        private string inputText;
        private List<string> additionalReferences = new List<string>();
        private ScriptLanguage scriptLanguage = ScriptLanguage.CSharp;
        private string scriptLanguageVersion = "v4.0";

        private IOuterScriptBuilder scriptBuilder;
        private List<ScriptProperty> scriptProperties = new List<ScriptProperty>();
        private object[] propertyValues = new object[0];
        private string contextTypeName;
        private string contextNamespace;

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
        ///     Initializes a new instance of the <see cref="TextTemplateTransformer"/> class.
        /// </summary>
        public TextTemplateTransformer()
            : this(null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextTemplateTransformer"/> class.
        /// </summary>
        /// <param name="context">The project builder that is building the project.</param>
        /// <exception cref="ArgumentNullException">
        ///     The specified <paramref name="context"/> is <c>null</c>.
        /// </exception>
        public TextTemplateTransformer(object context)
            : this(context, (string)null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextTemplateTransformer"/> class.
        /// </summary>
        /// <param name="context">The project builder that is building the project.</param>
        /// <param name="baseDirectory">The base directory of the template source files.</param>
        /// <exception cref="ArgumentNullException">
        ///     The specified <paramref name="context"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">The specified <paramref name="baseDirectory"/> is an empty string.</exception>
        public TextTemplateTransformer(object context, string baseDirectory)
            : this(context, baseDirectory, null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextTemplateTransformer"/> class.
        /// </summary>
        /// <param name="context">The project builder that is building the project.</param>
        /// <param name="additionalReferences">A list of additional library references.</param>
        /// <exception cref="ArgumentNullException">
        ///     The specified <paramref name="context"/> is <c>null</c>.
        /// </exception>
        public TextTemplateTransformer(object context, IEnumerable<string> additionalReferences)
            : this(context, null, additionalReferences)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextTemplateTransformer"/> class.
        /// </summary>
        /// <param name="context">The project builder that is building the project.</param>
        /// <param name="baseDirectory">The base directory of the template source files.</param>
        /// <param name="additionalReferences">A list of additional library references.</param>
        /// <exception cref="ArgumentNullException">
        ///     The specified <paramref name="context"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">The specified <paramref name="baseDirectory"/> is an empty string.</exception>
        public TextTemplateTransformer(object context, string baseDirectory, IEnumerable<string> additionalReferences)
        {
            if (baseDirectory == null)
            {
                baseDirectory = Environment.CurrentDirectory;
            }
            else if (string.IsNullOrWhiteSpace(baseDirectory))
            {
                throw new ArgumentException(InternalExceptionStrings.ArgumentException_EmptyOrWhitespaceString, "baseDirectory");
            }

            Type contextType;

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

            if (contextTypeName == null)
            {
                contextTypeName = "object";
            }

            this.baseDirectory = baseDirectory;

            if (additionalReferences != null)
            {
                this.additionalReferences = additionalReferences.ToList();
            }

            scriptBuilder = ScriptBuilderFactory.Create(scriptLanguage, Constants.NamespaceName, Constants.ClassName);

            if (context != null && contextNamespace != null)
            {
                scriptBuilder.NamespaceReferences.Add(new NamespaceReference(contextNamespace));
                this.additionalReferences.Add(context.GetType().Assembly.Location);
            }

            scriptBuilder.AppendPrivatePropertyDeclaration(Constants.ContextPropertyName, contextTypeName);
            scriptBuilder.AppendLine();

            scriptBuilder.AppendPrivatePropertyDeclaration(Constants.OutputPropertyName, typeof(TextTemplateOutputWriter).Name);
            scriptBuilder.AppendLine();

            scriptBuilder.AppendPrivatePropertyDeclaration(Constants.TransformerPropertyName, typeof(IInlineTransformer).Name);
            scriptBuilder.AppendLine();

            scriptBuilder.AppendInitializationMethodScript(contextTypeName);
        }

        /// <summary>
        ///     Initializes the <see cref="TextTemplateTransformer"/> class.
        /// </summary>
        static TextTemplateTransformer()
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
        ///     Transforms the specified text file and returns the result.
        /// </summary>
        /// <param name="path">The path of the text file.</param>
        /// <returns>The result of the transformation.</returns>
        /// <exception cref="ArgumentNullException">The specified path is <c>null</c>.</exception>
        public string TransformFile(string path)
        {
            return TransformFile(path, new object[0]);
        }

        /// <summary>
        ///     Transforms the specified text file and returns the result.
        /// </summary>
        /// <param name="path">The path of the text file.</param>
        /// <param name="propertyValues">The property values.</param>
        /// <returns>The result of the transformation.</returns>
        /// <exception cref="ArgumentNullException">The specified path is <c>null</c>.</exception>
        public string TransformFile(string path, params object[] propertyValues)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            scriptDirectory = Path.Combine(baseDirectory, Path.GetDirectoryName(path));

            return TransformText(File.ReadAllText(path), path, propertyValues);
        }

        /// <summary>
        ///     Transforms the specified text and returns the result.
        /// </summary>
        /// <param name="text">The text to transform.</param>
        /// <param name="sourcePath">The path of the source file.</param>
        /// <returns>The result of the transformation.</returns>
        /// <exception cref="ArgumentNullException">The specified <paramref name="text"/> is <c>null</c>.</exception>
        public string TransformText(string text, string sourcePath)
        {
            return TransformText(text, sourcePath, new object[0]);
        }

        /// <summary>
        ///     Transforms the specified text and returns the result.
        /// </summary>
        /// <param name="text">The text to transform.</param>
        /// <param name="sourcePath">The path of the source file.</param>
        /// <param name="propertyValues">The property values.</param>
        /// <returns>The result of the transformation.</returns>
        /// <exception cref="ArgumentNullException">The specified <paramref name="text"/> is <c>null</c>.</exception>
        public string TransformText(string text, string sourcePath, params object[] propertyValues)
        {
            this.propertyValues = propertyValues;
            scriptBuilder.IncludeSourceFileReferences = IncludeSourceFileReferences;

            Parse(text, sourcePath);

            return Execute(null);
        }

        /// <summary>
        ///     Transforms the specified text file and returns the result.
        /// </summary>
        /// <param name="relativePath">The relative path.</param>
        /// <param name="propertyValues">The property values.</param>
        /// <returns>The result of the transformation.</returns>
        public string TransformRelativeFile(string relativePath, params object[] propertyValues)
        {
            if (relativePath == null)
            {
                throw new ArgumentNullException("relativePath");
            }
            else if (string.IsNullOrWhiteSpace(relativePath))
            {
                throw new ArgumentException(InternalExceptionStrings.ArgumentException_EmptyOrWhitespaceString, "relativePath");
            }

            TextTemplateTransformer transformer = new TextTemplateTransformer();

            return transformer.TransformFile(Path.Combine(scriptDirectory, relativePath), propertyValues);
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

            inputText = text;

            lock (regexLock)
            {
                char[] inputChars = inputText.ToCharArray();

                // Disable all scripted remark sections while leaving the character locations
                // within the document intact. This way the line numbers in the source file
                // references will remain correct.
                foreach (Match match in remarksRegex.Matches(inputText))
                {
                    int startIndex = match.Index + Constants.ScriptStartMarker.Length;
                    int endIndex = match.Index + match.Length - Constants.ScriptEndMarker.Length;

                    for (int i = startIndex; i < endIndex; i++)
                    {
                        if (!char.IsControl(inputText[i]))
                        {
                            inputChars[i] = '\0';
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
                    if (lines[i].Length > 0)
                    {
                        lines[i] = lines[i].Substring(0, lines[i].Length - 1);
                    }
                }

                inputText = string.Join("", lines.Select((n, idx) =>
                    ((n.TrimStart().StartsWith(Constants.ScriptStartMarker, StringComparison.Ordinal)
                    && !n.TrimStart().StartsWith(Constants.ScriptStartMarker + Constants.ScriptAutoWriteMarker, StringComparison.Ordinal)
                    && n.TrimEnd().EndsWith(Constants.ScriptEndMarker))
                    || idx == lines.Length - 1)
                    ? n : n + "\r\n"));

                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    int length = line.Length + ((i < lines.Length - 1) ? 1 : 0);

                    lineMap[lineNumber++] = new CharacterRange()
                    {
                        Offset = characterIndex,
                        Length = length
                    };

                    characterIndex += length;
                }

                // Parse all fragments.
                int index = 0;
                string[] fragments = scriptsRegex.Split(inputText);
                
                foreach (string fragment in fragments)
                {
                    if (!fragment.Equals(Constants.ScriptStartMarker, StringComparison.Ordinal)
                        && !fragment.Equals(Constants.ScriptEndMarker, StringComparison.Ordinal))
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
        /// <param name="outputPath">The path of the file to store the resulting text in.</param>
        /// <returns>The resulting document.</returns>
        private string Execute(string outputPath)
        {
            scriptBuilder.AppendPropertiesScript(scriptProperties);

            string script = scriptBuilder.ToString();

            if (!string.IsNullOrEmpty(outputPath))
            {
                WriteScript(script, outputPath);
            }

            CompilerResults results = CompileScript(script);

            foreach (CompilerError error in results.Errors.Cast<CompilerError>().OrderBy(n => n.Line))
            {
                TextTemplateFileSourceReference sourceReference = new TextTemplateFileSourceReference()
                {
                    Path = GetSourceFilePath(error.FileName),
                    Line = error.Line
                };

                if (!error.IsWarning)
                {
                    TextTemplateFileException templateFileException = new TextTemplateFileException(GetCompilerErrorMessage(error), sourceReference);

                    throw templateFileException;
                }
            }

            Assembly assembly = results.CompiledAssembly;
            TextTemplateOutputWriter outputWriter = new TextTemplateOutputWriter();
            object documentScriptsInstance = assembly.CreateInstance(Constants.NamespaceName + "." + Constants.ClassName);

            // Call the property intialization method.
            Type classType = assembly.GetType(Constants.NamespaceName + "." + Constants.ClassName);
            MethodInfo initMethod = classType.GetMethod(Constants.PropertyInitializationMethodName);
            initMethod.Invoke(documentScriptsInstance, propertyValues);

            // Call the intialization method.
            classType = assembly.GetType(Constants.NamespaceName + "." + Constants.ClassName);
            initMethod = classType.GetMethod(Constants.InitializationMethodName);
            initMethod.Invoke(documentScriptsInstance, new[] { outputWriter, context, this });

            // Execute the main method.
            outputWriter.StartCapture();

            MethodInfo mainMethod = classType.GetMethod(scriptBuilder.MainMethodName);
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
            else if (!fragment.StartsWith(startMarker, StringComparison.Ordinal))
            {
                fragmentType = FragmentType.Markup;
            }
            // Determine whether the script should automatically write the result to the output.
            else if (fragment.StartsWith(startMarker + Constants.ScriptAutoWriteMarker, StringComparison.Ordinal))
            {
                startMarker += Constants.ScriptAutoWriteMarker;
                fragmentType = FragmentType.AutoWriteScript;
            }
            // Determine whether the script is a function definition.
            else if (fragment.StartsWith(startMarker + Constants.ScriptClassBodyMarker, StringComparison.Ordinal))
            {
                startMarker += Constants.ScriptClassBodyMarker;
                fragmentType = FragmentType.ClassBody;
            }
            // Determine whether the script is an include statement.
            else if (fragment.StartsWith(startMarker + Constants.DirectiveMarker + Constants.ScriptIncludeMarker, StringComparison.Ordinal))
            {
                startMarker += Constants.DirectiveMarker + Constants.ScriptIncludeMarker;
                fragmentType = FragmentType.Include;
            }
            // Determine whether the script is a namespace import statement.
            else if (fragment.StartsWith(startMarker + Constants.DirectiveMarker + Constants.TemplateMarker, StringComparison.Ordinal))
            {
                startMarker += Constants.DirectiveMarker + Constants.TemplateMarker;
                fragmentType = FragmentType.Template;
            }
            // Determine whether the script is a namespace import statement.
            else if (fragment.StartsWith(startMarker + Constants.DirectiveMarker + Constants.NamespaceImportMarker, StringComparison.Ordinal))
            {
                startMarker += Constants.DirectiveMarker + Constants.NamespaceImportMarker;
                fragmentType = FragmentType.NamespaceImport;
            }
            else if (fragment.StartsWith(startMarker + Constants.DirectiveMarker + Constants.ScriptPropertyMarker, StringComparison.Ordinal))
            {
                startMarker += Constants.ScriptPropertyMarker;
                fragmentType = FragmentType.Property;
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
                ParseScript(fragment, lineNumber, sourceFilePath);
            }
            else if (fragmentType == FragmentType.AutoWriteScript)
            {
                ParseAutoWriteScript(fragment, lineNumber, sourceFilePath);
            }
            else if (fragmentType == FragmentType.ClassBody)
            {
                scriptBuilder.AppendScriptFragment(fragment, lineNumber, sourceFilePath);
            }
            else if (fragmentType == FragmentType.Template)
            {
                ParseTemplateDirective(fragment);
            }
            else if (fragmentType == FragmentType.Include)
            {
                ParseIncludeDirective(fragment);
            }
            else if (fragmentType == FragmentType.NamespaceImport)
            {
                ParseNamespaceDirective(fragment);
            }
            else if (fragmentType == FragmentType.Property)
            {
                ParseScriptPropertyDirective(fragment);
            }
            else if (fragmentType == FragmentType.Markup)
            {
                ParseMarkup(fragment);
            }
        }

        /// <summary>
        ///     Parses the template directive from the specified fragment.
        /// </summary>
        /// <param name="fragment">The fragment.</param>
        private void ParseTemplateDirective(string fragment)
        {
            string language = ExtractDirectiveAttribute(fragment, Constants.LanguageAttributeName) ?? "";

            if (language.StartsWith(Constants.CSharpLanguageIdentifier, StringComparison.Ordinal))
            {
                scriptLanguage = ScriptLanguage.CSharp;

                if (language.Length > 2)
                {
                    scriptLanguageVersion = language.Substring(2);
                }
            }
            else if (language.StartsWith(Constants.VisualBasicLanguageIdentifier, StringComparison.Ordinal))
            {
                scriptLanguage = ScriptLanguage.VisualBasic;

                if (language.Length > 2)
                {
                    scriptLanguageVersion = language.Substring(2);
                }
            }
            else
            {
                throw new InvalidOperationException(InternalExceptionStrings.InvalidOperationException_UnrecognizedLanguageIdentifier);
            }
        }

        /// <summary>
        ///     Parses the include directive from the specified fragment.
        /// </summary>
        /// <param name="fragment">The fragment.</param>
        private void ParseIncludeDirective(string fragment)
        {
            string path = ExtractDirectiveAttribute(fragment, Constants.FileAttributeName);
            path = Path.Combine(scriptDirectory, path.Trim(new char[] { '\'', '\"' }));

            ParseFragment(File.ReadAllText(path), 1, path);
        }

        /// <summary>
        ///     Parses the namespace directive from the specified fragment.
        /// </summary>
        /// <param name="fragment">The fragment.</param>
        private void ParseNamespaceDirective(string fragment)
        {
            scriptBuilder.NamespaceReferences.Add(
                new NamespaceReference(ExtractDirectiveAttribute(fragment, Constants.NamespaceAttributeName)));
        }

        /// <summary>
        ///     Parses the script property directive from the specified fragment.
        /// </summary>
        /// <param name="fragment">The fragment.</param>
        private void ParseScriptPropertyDirective(string fragment)
        {
            scriptProperties.Add(
                new ScriptProperty(
                    ExtractDirectiveAttribute(fragment, Constants.NameAttributeName),
                    ExtractDirectiveAttribute(fragment, Constants.TypeAttributeName)));
        }

        /// <summary>
        ///     Parses the script from the specified fragment.
        /// </summary>
        /// <param name="fragment">The fragment.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        private void ParseScript(string fragment, int lineNumber, string sourceFilePath)
        {
            scriptBuilder.InnerScriptBuilder.AppendScriptFragment(fragment, lineNumber, sourceFilePath);
        }

        /// <summary>
        ///     Parses the auto write script from the specified fragment.
        /// </summary>
        /// <param name="fragment">The fragment.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        private void ParseAutoWriteScript(string fragment, int lineNumber, string sourceFilePath)
        {
            scriptBuilder.InnerScriptBuilder.AppendOutputWriteLineScript(fragment, lineNumber, sourceFilePath);
        }

        /// <summary>
        ///     Parses the markup from the specified fragment.
        /// </summary>
        /// <param name="fragment">The fragment.</param>
        private void ParseMarkup(string fragment)
        {
            while (fragment.StartsWith(Environment.NewLine, StringComparison.Ordinal))
            {
                scriptBuilder.InnerScriptBuilder.AppendOutputWriteLineScript();
                scriptBuilder.InnerScriptBuilder.AppendLine();

                fragment = fragment.Substring(Environment.NewLine.Length);
            }

            if (fragment.Length > 0)
            {
                scriptBuilder.InnerScriptBuilder.AppendOutputWriteLineScriptLiteral(fragment);
            }
        }

        /// <summary>
        ///     Extracts the value of the specified attribute from the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <returns>The extracted attribute value.</returns>
        private static string ExtractDirectiveAttribute(string text, string attributeName)
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
        private void WriteScript(string script, string path)
        {
            string directory = Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            StringBuilder output = new StringBuilder();

            output.AppendLine(scriptBuilder.GetAsRemark(new string('*', 94)));
            output.AppendLine(scriptBuilder.GetAsRemark(Resources.FileGeneratedByTool));
            output.AppendLine(scriptBuilder.GetAsRemark(new string('*', 94)));

            output.AppendLine();
            output.Append(script);

            File.WriteAllText(path, script);
        }

        /// <summary>
        ///     Compiles the specified script.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <returns>The compiler results.</returns>
        private CompilerResults CompileScript(string script)
        {
            using (CodeDomProvider provider = CodeDomProviderFactory.Create(scriptLanguage, scriptLanguageVersion))
            {
                CompilerParameters compilerParameters = new CompilerParameters();
                compilerParameters.GenerateExecutable = false;
                compilerParameters.GenerateInMemory = true;
                compilerParameters.ReferencedAssemblies.AddRange(GetAssemblyReferences());
                compilerParameters.IncludeDebugInformation = DebugMode;
                compilerParameters.WarningLevel = (DebugMode ? 3 : 1);
                compilerParameters.CompilerOptions = (DebugMode ? "" : "/optimize");

                return provider.CompileAssemblyFromSource(compilerParameters, script);
            }
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
                        filename = scriptBuilder.GetSourceFilePath(sourceFileGuid);
                    }
                }
            }

            return filename;
        }
    }
}
