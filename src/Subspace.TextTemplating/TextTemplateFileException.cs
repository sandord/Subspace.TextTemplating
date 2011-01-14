using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Subspace.TextTemplating
{
    /// <summary>
    ///     The exception that is thrown when processing of a text template file has failed.
    /// </summary>
    [Serializable]
    public class TextTemplateFileException : Exception
    {
        /// <summary>
        ///     Gets a reference to a line in the related template file.
        /// </summary>
        public TextTemplateFileSourceReference SourceReference
        {
            get;
            private set;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextTemplateFileException"/> class.
        /// </summary>
        public TextTemplateFileException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextTemplateFileException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public TextTemplateFileException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextTemplateFileException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="sourceReference">The source reference.</param>
        public TextTemplateFileException(string message, TextTemplateFileSourceReference sourceReference)
            : this(message)
        {
            this.SourceReference = sourceReference;
        }

        /// <summary>
        ///     Sets the <see cref="SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is a null reference (Nothing in Visual Basic). </exception>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("SourceReference", SourceReference);

            base.GetObjectData(info, context);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextTemplateFileException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected TextTemplateFileException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            SourceReference = (TextTemplateFileSourceReference)info.GetValue("SourceReference", typeof(TextTemplateFileSourceReference));
        }
    }
}
