// -----------------------------------------------------------------------
// <copyright file="ResultOptionsError.cs" company="Kitpymes">
// Copyright (c) Kitpymes. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project docs folder for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Kitpymes.Core.Results.Settings;

using Kitpymes.Core.Shared.Extensions;
using System.Collections.Generic;
using System.Net;

/*
  Clase de configuración del resultado a devolver ResultOptionsError
  Contiene las propiedades a devolver
*/

/// <summary>
/// Clase de configuración del resultado a devolver <c>ResultOptionsError</c>.
/// Contiene las propiedades a devolver.
/// </summary>
/// <remarks>
/// <para>En esta clase se pueden agregar todas las propiedades que se devuelven.</para>
/// </remarks>
public class ResultOptionsError
{
    /// <summary>
    /// Obtiene la configuración de los  errores.
    /// </summary>
    public ResultSettingsError ResultSettings { get; private set; } = new ResultSettingsError();

    /// <summary>
    /// Agrega un título al resultado.
    /// </summary>
    /// <param name="title">Título de error.</param>
    /// <returns>ErrorOptions.</returns>
    public ResultOptionsError WithTitle(string title)
    {
        ResultSettings.Title = title;

        return this;
    }

    /// <summary>
    /// Agrega el código de estado HTTP al resultado.
    /// </summary>
    /// <param name="status">Código de estado HTTP del error.</param>
    /// <returns>ErrorOptions.</returns>
    public ResultOptionsError WithStatusCode(HttpStatusCode status)
    {
        ResultSettings.Status = (int)status;

        return this;
    }

    /// <summary>
    /// Agrega un detalle al resultado.
    /// </summary>
    /// <param name="details">Detalle del resultado.</param>
    /// <returns>ErrorOptions.</returns>
    public ResultOptionsError WithDetails(object details)
    {
        ResultSettings.Details = details;

        return this;
    }

    /// <summary>
    /// Agrega el nombre de la excepción al resultado.
    /// </summary>
    /// <param name="exception">Nombre de la excepción.</param>
    /// <returns>ErrorOptions.</returns>
    public ResultOptionsError WithExceptionType(string exception)
    {
        ResultSettings.Exception = exception;

        return this;
    }

    /// <summary>
    /// Agrega un mensaje al resultado.
    /// </summary>
    /// <param name="message">Mensaje de error.</param>
    /// <returns>ErrorOptions.</returns>
    public ResultOptionsError WithMessage(string message)
    {
        ResultSettings.AddMessage(message);

        return this;
    }

    /// <summary>
    /// Agrega uno o varios mensajes al resultado.
    /// </summary>
    /// <param name="messages">Lista de mensajes de error.</param>
    /// <returns>ErrorOptions.</returns>
    public ResultOptionsError WithMessages(IEnumerable<string> messages)
    {
        ResultSettings.AddMessages(messages);

        return this;
    }

    /// <summary>
    /// Agrega uno o varios errores del modelo al resultado.
    /// </summary>
    /// <param name="errors">Lista de errores.</param>
    /// <returns>ErrorOptions.</returns>
    public ResultOptionsError WithErrors(IEnumerable<(string fieldName, string message)> errors)
    {
        if (!errors.ToIsNullOrAny())
        {
            IDictionary<string, IEnumerable<string>> keyValuePairs = new Dictionary<string, IEnumerable<string>>();

            foreach (var (fieldName, message) in errors)
            {
                keyValuePairs.AddOrUpdate(fieldName, message);
            }

            ResultSettings.AddErrors(keyValuePairs);
        }

        return this;
    }

    /// <summary>
    /// Agrega uno o varios errores del modelo al resultado.
    /// </summary>
    /// <param name="errors">Lista de errores.</param>
    /// <returns>ErrorOptions.</returns>
    public ResultOptionsError WithErrors(IDictionary<string, IEnumerable<string>>? errors)
    {
        ResultSettings.AddErrors(errors);

        return this;
    }
}
