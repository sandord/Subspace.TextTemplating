﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns>The result of the transformation.</returns>
        string TransformRelativeFile(string relativePath, params object[] parameterValues);
    }
}