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
        /// <param name="scriptLanguageVersion">The script language version.</param>
        /// <returns>A code DOM provider.</returns>
        internal static CodeDomProvider Create(ScriptLanguage scriptLanguage, string scriptLanguageVersion)
        {
            if (scriptLanguageVersion == null)
            {
                throw new ArgumentNullException("scriptLanguageVersion");
            }

            CodeDomProvider instance;

            Dictionary<string, string> providerOptions =
                new Dictionary<string, string>()
                    {
                        { "CompilerVersion", scriptLanguageVersion }
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
