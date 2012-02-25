// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Subspace" file="InternalExceptionStrings.cs">
//   Copyright (c) Subspace. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Subspace.TextTemplating
{
    /// <summary>
    ///     Contains exception strings that have a technical nature and are not intended for the end
    ///     user. Therefore they do not need to be implemented as resource strings because
    ///     translations are not required.
    /// </summary>
    internal static class InternalExceptionStrings
    {
        internal const string ArgumentException_EmptyOrWhitespaceString = "Empty string or a string consisting of only whitespace.";        
    }
}
