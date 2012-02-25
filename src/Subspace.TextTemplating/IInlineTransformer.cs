// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Subspace" file="IInlineTransformer.cs">
//   Copyright (c) Subspace. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Subspace.TextTemplating
{
    /// <summary>
    ///     Defines the public members of a class that performs transformations from within a
    ///     template script.
    /// </summary>
    public interface IInlineTransformer
    {
        /// <summary>
        ///     Transforms the specified text file and returns the result.
        /// </summary>
        /// <param name="relativePath">The relative path.</param>
        /// <param name="propertyValues">The property values.</param>
        /// <returns>The result of the transformation.</returns>
        string TransformRelativeFile(string relativePath, params object[] propertyValues);
    }
}
