// -----------------------------------------------------------------------
// <copyright file="IResultValue.cs" company="Kitpymes">
// Copyright (c) Kitpymes. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project docs folder for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Kitpymes.Core.Results;

/// <summary>
/// Acceso sin tipo al valor de un <see cref="Result{T}"/>, para la traducción a <see cref="ApiResult"/>.
/// </summary>
internal interface IResultValue
{
    /// <summary>
    /// Obtiene el valor como objeto.
    /// </summary>
    object? BoxedValue { get; }
}
