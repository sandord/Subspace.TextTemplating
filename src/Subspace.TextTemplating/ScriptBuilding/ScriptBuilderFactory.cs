using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subspace.TextTemplating.ScriptBuilding
{
    /// <summary>
    ///     Creates script builder instances.
    /// </summary>
    internal static class ScriptBuilderFactory
    {
        /// <summary>
        ///     Creates a script builder appropriate for the specified script language.
        /// </summary>
        /// <param name="scriptLanguage">The script language.</param>
        /// <param name="namespaceName">The name of the namespace to use.</param>
        /// <param name="className">The name of the class to generate.</param>
        /// <returns>A script builder.</returns>
        public static ScriptBuilder Create(ScriptLanguage scriptLanguage, string namespaceName, string className)
        {
            if (namespaceName == null)
            {
                throw new ArgumentNullException("namespaceName");
            }
            else if (string.IsNullOrWhiteSpace(namespaceName))
            {
                throw new ArgumentException(InternalExceptionStrings.ArgumentException_EmptyOrWhitespaceString, "namespaceName");
            }
            else if (className == null)
            {
                throw new ArgumentNullException("className");
            }
            else if (string.IsNullOrWhiteSpace(className))
            {
                throw new ArgumentException(InternalExceptionStrings.ArgumentException_EmptyOrWhitespaceString, "className");
            }

            ScriptBuilder instance;

            if (scriptLanguage == ScriptLanguage.CSharp)
            {
                instance = new CSharpScriptBuilder(namespaceName, className);
            }
            else if (scriptLanguage == ScriptLanguage.VisualBasic)
            {
                instance = new VisualBasicScriptBuilder(namespaceName, className);
            }
            else
            {
                throw new InvalidOperationException(InternalExceptionStrings.InvalidOperationException_UnrecognizedLanguageIdentifier);
            }

            return instance;
        }
    }
}
