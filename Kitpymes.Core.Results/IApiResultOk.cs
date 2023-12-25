// -----------------------------------------------------------------------
// <copyright file="IApiResultOk.cs" company="Kitpymes">
// Copyright (c) Kitpymes. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project docs folder for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Kitpymes.Core.Results;

/// <summary>
/// Resultado.
/// </summary>
public interface IApiResultOk : IApiResultBase
{
    /// <summary>
    /// Obtiene un objeto con datos.
    /// </summary>
    object? Data { get; }
}
