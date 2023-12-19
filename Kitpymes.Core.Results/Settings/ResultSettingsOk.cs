// -----------------------------------------------------------------------
// <copyright file="ResultSettingsOk.cs" company="Kitpymes">
// Copyright (c) Kitpymes. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project docs folder for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Kitpymes.Core.Results.Settings;

/*
  Clase de configuración de las propiedades a devolver de ResultSettingsOk
  Contiene los propiedades que se devuelven en un resultado
*/

/// <summary>
/// Clase de configuración de las propiedades a devolverr <c>ResultSettingsOk</c>.
/// Contiene los propiedades que se devuelven en un resultado.
/// </summary>
/// <remarks>
/// <para>En esta clase se pueden agregar todas las propiedades que se devuelven en un resultado.</para>
/// </remarks>
public class ResultSettingsOk
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
    /// Obtiene o establece un valor que indica dato del resultado.
    /// </summary>
    public object? Data { get; set; }
}
