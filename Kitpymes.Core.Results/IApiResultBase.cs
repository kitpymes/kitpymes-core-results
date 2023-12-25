// -----------------------------------------------------------------------
// <copyright file="IApiResultBase.cs" company="Kitpymes">
// Copyright (c) Kitpymes. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project docs folder for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Kitpymes.Core.Results;

/// <summary>
/// Resultado.
/// </summary>
public interface IApiResultBase
{
    /// <summary>
    /// Obtiene un valor que indica si el proceso fue correcto.
    /// </summary>
    bool Success { get; }

    /// <summary>
    /// Obtiene un valor que indica el código del estado de la solicitud HTTP.
    /// </summary>
    int? Status { get; }

    /// <summary>
    /// Obtiene un valor que indica el título del resultado.
    /// </summary>
    string? Title { get; }
}
