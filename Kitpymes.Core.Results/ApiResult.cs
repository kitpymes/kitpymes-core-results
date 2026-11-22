// -----------------------------------------------------------------------
// <copyright file="ApiResult.cs" company="Kitpymes">
// Copyright (c) Kitpymes. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project docs folder for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Kitpymes.Core.Results;

using System;
using System.Collections.Generic;
using System.Net;
using Kitpymes.Core.Shared;
using Kitpymes.Core.Results.Settings;
using Kitpymes.Core.Shared.Extensions;
using System.Linq;
using Kitpymes.Core.Shared.Util;

/*
    Clase de resultados ApiResult
    Contiene los métodos que devuelven los resultados
*/

/// <summary>
/// Clase de resultados <c>Result</c>.
/// Contiene los métodos que devuelven los resultados.
/// </summary>
/// <remarks>
/// <para>En esta clase se pueden agregar todos los métodos que devuelven resultados.</para>
/// </remarks>
public class ApiResult : IApiResult
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="ApiResult"/>.
    /// </summary>
    /// <param name="success">Si el proceso se ejecuto correctamente.</param>
    private ApiResult(bool success) => Success = success;

    /// <summary>
    /// Obtiene un valor que indica si el proceso fue correcto.
    /// </summary>
    public bool Success { get; private set; }

    /// <summary>
    /// Obtiene un valor que indica el código del estado de la solicitud HTTP.
    /// </summary>
    public int? Status { get; private set; }

    /// <summary>
    /// Obtiene un valor que indica el título del resultado.
    /// </summary>
    public string? Title { get; private set; }

    /// <inheritdoc/>
    public string? TraceId { get; private set; }

    /// <inheritdoc/>
    public string? Exception { get; private set; }

    /// <inheritdoc/>
    public string? Message { get; private set; }

    /// <inheritdoc/>
    public object? Details { get; private set; }

    /// <inheritdoc/>
    public object? Data { get; private set; }

    /// <inheritdoc/>
    public IDictionary<string, IEnumerable<string>>? Errors { get; private set; }

    /// <inheritdoc/>
    public string? Code { get; private set; }

    /// <inheritdoc/>
    public IReadOnlyList<FieldError>? FieldErrors { get; private set; }

    #region Ok

    /// <summary>
    /// Devuelve un resultado de ejecución correcta.
    /// <para><strong>OK:</strong> 200.</para>
    /// </summary>
    /// <returns>ApiResult.</returns>
    public static ApiResult Ok()
        => Ok(x => x.WithTitle(SharedResource.MsgProcessRanSuccessfully));

    /// <summary>
    /// Agrega uno o varios resultados al resultado.
    /// </summary>
    /// <param name="options">Configuración del resultado.</param>
    /// <returns>ApiResult.</returns>
    public static ApiResult Ok(Action<ResultOptionsOk> options)
    {
        var config = options.ToConfigureOrDefault().ResultSettings;

        return new ApiResult(true)
        {
            Status = (int)HttpStatusCode.OK,
            Title = config.Title ?? SharedResource.MsgProcessRanSuccessfully,
            Data = config.Data,
        };
    }

    #endregion Ok

    #region BadRequest

    /// <summary>
    /// Devuelve un resultado de error de validación.
    /// <para><strong>BadRequest:</strong> 400.</para>
    /// </summary>
    /// <param name="message">Agrega un mensaje al resultado.</param>
    /// <returns>ApiResult.</returns>
    public static ApiResult BadRequest(string message)
        => Error(x => x
            .WithMessage(message)
            .WithStatusCode(HttpStatusCode.BadRequest)
            .WithTitle(SharedResource.MsgValidationsError));

    /// <summary>
    /// Devuelve un resultado de error de validación.
    /// <para><strong>BadRequest:</strong> 400.</para>
    /// </summary>
    /// <param name="message">Agrega un mensaje al resultado.</param>
    /// <param name="details">Detalles que se muestran solo en ambientes distintos al de producción.</param>
    /// <returns>ApiResult.</returns>
    public static ApiResult BadRequest(string message, object details)
        => Error(x => x
            .WithMessage(message)
            .WithDetails(details)
            .WithStatusCode(HttpStatusCode.BadRequest)
            .WithTitle(SharedResource.MsgValidationsError));

    /// <summary>
    /// Devuelve un resultado de error de validación.
    /// <para><strong>BadRequest:</strong> 400.</para>
    /// </summary>
    /// <param name="messages">Agrega mensajes de validación al resultado.</param>
    /// <returns>ApiResult.</returns>
    public static ApiResult BadRequest(IEnumerable<string> messages)
        => Error(x => x
            .WithMessages(messages)
            .WithStatusCode(HttpStatusCode.BadRequest)
            .WithTitle(SharedResource.MsgValidationsError));

    /// <summary>
    /// Devuelve un resultado de error de validación.
    /// <para><strong>BadRequest:</strong> 400.</para>
    /// </summary>
    /// <param name="errors">Agrega los errores de validación al resultado.</param>
    /// <returns>ApiResult.</returns>
    public static ApiResult BadRequest(IDictionary<string, IEnumerable<string>> errors)
        => Error(x => x
            .WithErrors(errors)
            .WithStatusCode(HttpStatusCode.BadRequest)
            .WithTitle(SharedResource.MsgValidationsError));

    /// <summary>
    /// Devuelve un resultado de error de validación.
    /// <para><strong>BadRequest:</strong> 400.</para>
    /// </summary>
    /// <param name="errors">Agrega los errores de validación al resultado.</param>
    /// <returns>ApiResult.</returns>
    public static ApiResult BadRequest(IEnumerable<(string fieldName, string message)> errors)
        => Error(x => x
            .WithErrors(errors)
            .WithStatusCode(HttpStatusCode.BadRequest)
            .WithTitle(SharedResource.MsgValidationsError));

    #endregion BadRequest

    /// <summary>
    /// Agrega uno o varios errores al resultado.
    /// </summary>
    /// <param name="options">Configuración de la lista de errores.</param>
    /// <returns>ApiResult.</returns>
    public static ApiResult Error(Action<ResultOptionsError> options)
    {
        var config = options.ToConfigureOrDefault().ResultSettings;

        Verify.ToThrowIf($"{nameof(config.Messages)} and {nameof(config.Errors)} are null or not contains elements.", () => config.Messages.ToIsNullOrAny() && config.Errors.ToIsNullOrAny());

        return new ApiResult(false)
        {
            Status = config.Status,
            Title = config.Title,
            TraceId = Guid.NewGuid().ToSecuencial().ToString(),
            Details = config.Details,
            Exception = config.Exception,
            Message = config.Messages?.Any() == true ? config.Messages.ToString(", ") : null,
            Errors = config.Errors,
            Code = config.Code,
            FieldErrors = config.FieldErrors,
        };
    }
}
