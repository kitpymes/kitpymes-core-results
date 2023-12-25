// -----------------------------------------------------------------------
// <copyright file="IApiResultError.cs" company="Kitpymes">
// Copyright (c) Kitpymes. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project docs folder for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Kitpymes.Core.Results;

using System.Collections.Generic;

/// <summary>
/// Resultado.
/// </summary>
public interface IApiResultError : IApiResultBase
{
    /// <summary>
    /// Obtiene un valor que indica el id del error.
    /// </summary>
    string? TraceId { get; }

    /// <summary>
    /// Obtiene un valor que indica el tipo de excepción.
    /// </summary>
    string? Exception { get; }

    /// <summary>
    /// Obtiene un valor que indica el mensaje del resultado.
    /// </summary>
    string? Message { get; }

    /// <summary>
    /// Obtiene la lista de errores de un modelo.
    /// </summary>
    public IDictionary<string, IEnumerable<string>>? Errors { get; }

    /// <summary>
    /// Obtiene un detalle.
    /// </summary>
    object? Details { get; }
}
