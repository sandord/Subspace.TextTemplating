using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subspace.TextTemplating.ScriptBuilding
{
    /// <summary>
    ///     Defines the public members of a class that provides means of building an inner script,
    ///     which is a script that builds the actual output of a text template.
    /// </summary>
    internal interface IInnerScriptBuilder
    {
        /// <summary>
        ///     Gets a value indicating whether this instance is empty.
        /// </summary>
        bool IsEmpty
        {
            get;
        }

        /// <summary>
        ///     Appends an empty line.
        /// </summary>
        void AppendLine();

        /// <summary>
        ///     Appends the specified line.
        /// </summary>
        /// <param name="text">The text to write.</param>
        void AppendLine(string text);

        /// <summary>
        ///     Appends the output write script that writes an empty line.
        /// </summary>
        void AppendOutputWriteLineScript();

        /// <summary>
        ///     Appends the output write script that writes the specified text as a line.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="sourceFilePath">The source file path.</param>
        void AppendOutputWriteLineScript(string text, int lineNumber, string sourceFilePath);

        /// <summary>
        ///     Appends the output write script that writes the specified literal text as a line.
        /// </summary>
        /// <param name="text">The text.</param>
        void AppendOutputWriteLineScriptLiteral(string text);

        /// <summary>
        ///     Appends the specified script fragment.
        /// </summary>
        /// <param name="fragment">The fragment.</param>
        /// <param name="lineNumber">The line number of the first line of the fragment.</param>
        /// <param name="sourceFilePath">The path of the source file.</param>
        /// <returns>The number of lines that were extracted from the fragment.</returns>
        /// <exception cref="ArgumentNullException">The specified <paramref name="fragment"/> is <c>null</c>.</exception>
        int AppendScriptFragment(string fragment, int lineNumber, string sourceFilePath);
    }
}
