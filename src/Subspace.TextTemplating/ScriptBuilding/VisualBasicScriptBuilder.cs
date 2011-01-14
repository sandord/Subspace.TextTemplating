using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subspace.TextTemplating.ScriptBuilding
{
    /// <summary>
    ///     Provides a means of building a Visual Basic script that is suitable for execution.
    /// </summary>
    internal sealed class VisualBasicScriptBuilder : ScriptBuilder
    {
        /// <summary>
        ///     Returns the composed script.
        /// </summary>
        /// <returns>The script.</returns>
        internal override string ComposeScript()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Writes a source file reference.
        /// </summary>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <param name="lineNumber">The line number.</param>
        protected override void WriteSourceFileReference(string sourceFilePath, int lineNumber)
        {
            throw new NotImplementedException();
        }
    }
}
