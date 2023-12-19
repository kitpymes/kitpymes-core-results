// -----------------------------------------------------------------------
// <copyright file="ResultSettingsError.cs" company="Kitpymes">
// Copyright (c) Kitpymes. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project docs folder for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Kitpymes.Core.Results.Settings;

using Kitpymes.Core.Shared.Extensions;
using System.Collections.Generic;

/*
  Clase de configuración de las propiedades a devolver de ResultSettingsError
  Contiene los propiedades que se devuelven en un resultado
*/

/// <summary>
/// Clase de configuración de las propiedades a devolverr <c>ResultSettingsError</c>.
/// Contiene los propiedades que se devuelven en un resultado.
/// </summary>
/// <remarks>
/// <para>En esta clase se pueden agregar todas las propiedades que se devuelven en un resultado.</para>
/// </remarks>
public class ResultSettingsError
{
    /// <summary>
    /// Obtiene o establece un valor que indica el número del estado de la solicitud HTTP.
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// Obtiene o establece un valor que indica el título del resultado.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Obtiene o establece un valor que indica el id del error.
    /// </summary>
    public string? TraceId { get; set; }

    /// <summary>
    /// Obtiene o establece un valor que indica el tipo de excepción.
    /// </summary>
    public string? Exception { get; set; }

    /// <summary>
    /// Obtiene la lista de errores.
    /// </summary>
    public IList<string>? Messages { get; private set; }

    /// <summary>
    /// Obtiene la lista de errores de un modelo.
    /// </summary>
    public IDictionary<string, IEnumerable<string>>? Errors { get; private set; }

    /// <summary>
    /// Obtiene o establece un detalle.
    /// </summary>
    public object? Details { get; set; }

    /// <summary>
    /// Agrega un mensaje al resultado.
    /// </summary>
    /// <param name="message">Mensaje de error.</param>
    public void AddMessage(string message)
    {
        if (!message.ToIsNullOrEmpty())
        {
            Messages ??= new List<string>();

            if (!Messages.Contains(message))
            {
                Messages.Add(message);
            }
        }
    }

    /// <summary>
    /// Agrega uno o varios mensajes al resultado.
    /// </summary>
    /// <param name="messages">Lista de mensajes de error.</param>
    public void AddMessages(IEnumerable<string> messages)
    {
        if (!messages.ToIsNullOrAny())
        {
            foreach (var message in messages)
            {
                AddMessage(message);
            }
        }
    }

    /// <summary>
    /// Agrega uno o varios mensajes al resultado.
    /// </summary>
    /// <param name="errors">Lista de mensajes de error.</param>
    public void AddErrors(IDictionary<string, IEnumerable<string>>? errors)
    {
        if (!errors.ToIsNullOrAny())
        {
            Errors ??= new Dictionary<string, IEnumerable<string>>();

            Errors = errors;
        }
    }
}
