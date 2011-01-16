using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.CSharp;
using Microsoft.VisualBasic;

namespace Subspace.TextTemplating.ScriptBuilding
{
    /// <summary>
    ///     Creates code DOM provider instances.
    /// </summary>
    internal static class CodeDomProviderFactory
    {
        /// <summary>
        ///     Creates a code DOM provider appropriate for the specified script language.
        /// </summary>
        /// <param name="scriptLanguage">The script language.</param>
        /// <param name="compilerVersion">The compiler version.</param>
        /// <returns>A code DOM provider.</returns>
        public static CodeDomProvider Create(ScriptLanguage scriptLanguage, string compilerVersion)
        {
            if (compilerVersion == null)
            {
                throw new ArgumentNullException("compilerVersion");
            }
            else if (string.IsNullOrWhiteSpace(compilerVersion))
            {
                throw new ArgumentException(InternalExceptionStrings.ArgumentException_EmptyOrWhitespaceString, "compilerVersion");
            }

            CodeDomProvider instance;

            Dictionary<string, string> providerOptions =
                new Dictionary<string, string>()
                    {
                        { "CompilerVersion", compilerVersion }
                    };

            if (scriptLanguage == ScriptLanguage.CSharp)
            {
                instance = new CSharpCodeProvider(providerOptions);
            }
            else if (scriptLanguage == ScriptLanguage.VisualBasic)
            {
                instance = new VBCodeProvider(providerOptions);
            }
            else
            {
                throw new InvalidOperationException(InternalExceptionStrings.InvalidOperationException_UnsupportedLanguage);
            }

            return instance;
        }
    }
}
