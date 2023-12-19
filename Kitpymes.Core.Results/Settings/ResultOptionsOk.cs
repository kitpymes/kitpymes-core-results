// -----------------------------------------------------------------------
// <copyright file="ResultOptionsOk.cs" company="Kitpymes">
// Copyright (c) Kitpymes. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project docs folder for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Kitpymes.Core.Results.Settings;

/*
  Clase de configuración del resultado a devolver ResultOptionsOk
  Contiene las propiedades a devolver
*/

/// <summary>
/// Clase de configuración del resultado a devolver <c>ResultOptionsOk</c>.
/// Contiene las propiedades a devolver.
/// </summary>
/// <remarks>
/// <para>En esta clase se pueden agregar todas las propiedades que se devuelven.</para>
/// </remarks>
public class ResultOptionsOk
{
    /// <summary>
    /// Obtiene la configuración de los  errores.
    /// </summary>
    public ResultSettingsOk ResultSettings { get; private set; } = new ResultSettingsOk();

    /// <summary>
    /// Agrega un título al resultado.
    /// </summary>
    /// <param name="title">Título.</param>
    /// <returns>ResultOptionsOk.</returns>
    public virtual ResultOptionsOk WithTitle(string title)
    {
        ResultSettings.Title = title;

        return this;
    }

    /// <summary>
    /// Agrega un dato al resultado.
    /// </summary>
    /// <param name="data">datos.</param>
    /// <returns>ResultOptionsOk.</returns>
    public virtual ResultOptionsOk WithData<T>(T data)
    {
        ResultSettings.Data = data;

        return this;
    }
}
