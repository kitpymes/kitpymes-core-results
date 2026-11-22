// -----------------------------------------------------------------------
// <copyright file="ResultExtensions.cs" company="Kitpymes">
// Copyright (c) Kitpymes. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project docs folder for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Kitpymes.Core.Results;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

/// <summary>
/// Traducción de <see cref="Result"/> a <see cref="ApiResult"/>. Sólo se usa en la capa API.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Traduce un resultado a la respuesta HTTP estándar.
    /// </summary>
    /// <remarks>
    /// Validation 400, NotFound 404, Conflict 409, Unauthorized 401, Forbidden 403, NotContracted 403 y Unexpected 500.
    /// Nunca copia detalles internos (excepción, pila) a la respuesta.
    /// </remarks>
    /// <param name="result">Resultado.</param>
    /// <returns>ApiResult.</returns>
    public static ApiResult ToApiResult(this Result result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (result.IsSuccess)
        {
            return result is IResultValue withValue
                ? ApiResult.Ok(options => options.WithData(withValue.BoxedValue))
                : ApiResult.Ok();
        }

        var error = result.Error;

        return ApiResult.Error(options =>
        {
            options
                .WithStatusCode(ToStatusCode(error.Type))
                .WithTitle(ToTitle(error.Type))
                .WithCode(error.Code)
                .WithMessage(error.Message);

            if (error.Fields.Count > 0)
            {
                options
                    .WithFieldErrors(error.Fields)
                    .WithErrors(error.Fields
                        .GroupBy(x => x.Field)
                        .ToDictionary(x => x.Key, x => (IEnumerable<string>)x.Select(f => f.Message).ToList()));
            }
        });
    }

    /// <summary>
    /// Código HTTP de cada tipo de error.
    /// </summary>
    /// <param name="type">Tipo de error.</param>
    /// <returns>Código HTTP.</returns>
    public static HttpStatusCode ToStatusCode(this ErrorType type) => type switch
    {
        ErrorType.Validation => HttpStatusCode.BadRequest,
        ErrorType.NotFound => HttpStatusCode.NotFound,
        ErrorType.Conflict => HttpStatusCode.Conflict,
        ErrorType.Unauthorized => HttpStatusCode.Unauthorized,
        ErrorType.Forbidden => HttpStatusCode.Forbidden,
        ErrorType.NotContracted => HttpStatusCode.Forbidden,
        ErrorType.Unexpected => HttpStatusCode.InternalServerError,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Tipo de error desconocido."),
    };

    private static string ToTitle(ErrorType type) => type switch
    {
        ErrorType.Validation => "Error de validación",
        ErrorType.NotFound => "No encontrado",
        ErrorType.Conflict => "Conflicto",
        ErrorType.Unauthorized => "No identificado",
        ErrorType.Forbidden => "Sin permiso",
        ErrorType.NotContracted => "Módulo no contratado",
        _ => "Error inesperado",
    };
}
