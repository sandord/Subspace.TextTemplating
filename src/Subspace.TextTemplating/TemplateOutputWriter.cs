using System;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Subspace.TextTemplating
{
    /// <summary>
    ///     Provides means for outputing text from a template file.
    /// </summary>
    public sealed class TemplateOutputWriter
    {
        private StringBuilder stringBuilder;
        private bool capturing;
        private int captureOffset;

        /// <summary>
        ///     Gets or sets a value indicating whether to trims all leading and trailing
        ///     whitespace from the output. 
        /// </summary>
        public bool Trim
        {
            get;
            set;
        }

        /// <summary>
        ///     Writes the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="ArgumentNullException">The specified <paramref name="value"/> is <c>null</c>.</exception>
        public void Write(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            stringBuilder.Append(value.ToString());
        }

        /// <summary>
        ///     Writes a formatted string, which contains zero or more format specifications. Each
        ///     format specification is replaced by the string representation of a corresponding
        ///     object argument.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An array of objects to format.</param>
        /// <exception cref="ArgumentNullException">The specified <paramref name="format"/> is <c>null</c>.</exception>
        public void WriteFormat(string format, params object[] args)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }

            stringBuilder.AppendFormat(CultureInfo.CurrentCulture, format, args);
        }

        /// <summary>
        ///     Writes a formatted string, which contains zero or more format specifications. Each
        ///     format specification is replaced by the string representation of a corresponding
        ///     object argument.
        /// </summary>
        /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An array of objects to format.</param>
        /// <exception cref="ArgumentNullException">
        ///     The specified <paramref name="provider"/> is <c>null</c>.
        ///     -or-
        ///     The specified <paramref name="format"/> is <c>null</c>.
        /// </exception>
        public void WriteFormat(IFormatProvider provider, string format, params object[] args)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            else if (format == null)
            {
                throw new ArgumentNullException("format");
            }

            stringBuilder.AppendFormat(provider, format, args);
        }

        /// <summary>
        ///     Starts a new line.
        /// </summary>
        public void WriteLine()
        {
            stringBuilder.AppendLine();
        }

        /// <summary>
        ///     Writes the specified <paramref name="text"/> and starts a new line.
        /// </summary>
        /// <param name="text">The text to write.</param>
        /// <exception cref="ArgumentNullException">The specified <paramref name="text"/> is <c>null</c>.</exception>
        public void WriteLine(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            stringBuilder.AppendLine(text);
        }

        /// <summary>
        ///     Returns a <see cref="String"/> that represents the current <see cref="Object"/>.
        /// </summary>
        /// <returns>A <see cref="String"/> that represents the current <see cref="Object"/>.</returns>
        public override string ToString()
        {
            if (Trim)
            {
                return stringBuilder.ToString().Trim();
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TemplateOutputWriter"/> class.
        /// </summary>
        public TemplateOutputWriter()
        {
            stringBuilder = new StringBuilder();
        }

        /// <summary>
        ///     Starts capturing the output text.
        /// </summary>
        internal void StartCapture()
        {
            capturing = true;
            captureOffset = stringBuilder.Length;
        }

        /// <summary>
        ///     Returns the text that was output since <see cref="StartCapture"/> was called and
        ///     stops capturing.
        /// </summary>
        /// <returns>The captured text.</returns>
        internal string EndCapture()
        {
            if (!capturing)
            {
                throw new InvalidOperationException(InternalExceptionStrings.InvalidOperationException_NotCapturing);
            }

            capturing = false;

            string result = stringBuilder.ToString(captureOffset, stringBuilder.Length - captureOffset);

            if (Trim)
            {
                return result.Trim();
            }

            return result;
        }
    }
}
