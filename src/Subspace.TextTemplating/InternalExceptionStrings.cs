namespace Subspace.TextTemplating
{
    /// <summary>
    ///     Contains exception strings that have a technical nature and are not intended for the end
    ///     user. Therefore they do not need to be implemented as resource strings because
    ///     translations are not required.
    /// </summary>
    internal static class InternalExceptionStrings
    {
        internal static readonly string ArgumentException_EmptyString =
            "Empty string.";

        internal static readonly string ArgumentException_EmptyOrWhitespaceString =
            "Empty string or a string consisting of only whitespace.";

        internal static readonly string InvalidOperationException_NotCapturing =
            "Not capturing.";

        internal static readonly string ArgumentException_TypeIsSpecialName =
            "Cannot accept the instance type because it is a special name.";

        internal static readonly string ArgumentException_TypeIsNotPublic =
            "Cannot accept the instance type because it is not public.";
    }
}
