// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Subspace" file="CodeDomProviderFactory.cs">
//   Copyright (c) Subspace. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Subspace.TextTemplating.ScriptBuilding
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;

    using Microsoft.CSharp;
    using Microsoft.VisualBasic;

    /// <summary>
    ///     Creates code DOM provider instances.
    /// </summary>
    internal static class CodeDomProviderFactory
    {
        private const string InvalidOperationException_UnsupportedLanguage = "Unsupported language.";

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
                throw new InvalidOperationException(InvalidOperationException_UnsupportedLanguage);
            }

            return instance;
        }
    }
}
