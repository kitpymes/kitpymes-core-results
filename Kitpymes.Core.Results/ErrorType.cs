// -----------------------------------------------------------------------
// <copyright file="ErrorType.cs" company="Kitpymes">
// Copyright (c) Kitpymes. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project docs folder for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Kitpymes.Core.Results;

/// <summary>
/// Tipo de error de un <see cref="Result"/>. No sabe nada de HTTP: sólo la traducción a <see cref="ApiResult"/> lo convierte en código de estado.
/// </summary>
public enum ErrorType
{
    /// <summary>
    /// Los datos de entrada no tienen la forma esperada (vacío, longitud, formato).
    /// </summary>
    Validation = 0,

    /// <summary>
    /// Lo que se busca no existe.
    /// </summary>
    NotFound = 1,

    /// <summary>
    /// La operación choca con el estado actual (duplicado, versión, estado no permitido).
    /// </summary>
    Conflict = 2,

    /// <summary>
    /// Falta identificarse.
    /// </summary>
    Unauthorized = 3,

    /// <summary>
    /// Identificado, pero sin permiso para la operación.
    /// </summary>
    Forbidden = 4,

    /// <summary>
    /// El módulo está instalado pero el cliente no lo tiene contratado.
    /// </summary>
    NotContracted = 5,

    /// <summary>
    /// Fallo inesperado (base de datos caída, bug). Lo devuelve la captura común del mediador.
    /// </summary>
    Unexpected = 6,
}
