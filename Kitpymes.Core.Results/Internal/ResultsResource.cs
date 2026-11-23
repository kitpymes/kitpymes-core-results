// -----------------------------------------------------------------------
// <copyright file="ResultsResource.cs" company="Kitpymes">
// Copyright (c) Kitpymes. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project docs folder for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Kitpymes.Core.Results.Internal;

using System.Resources;

/*
    Textos propios de Results (en español por defecto y en inglés).
    Hasta la 1.1.1 venían de SharedResource de Kitpymes.Core.Shared; se copian con los mismos
    valores y las mismas culturas para que Results no dependa de Shared.
*/

/// <summary>
/// Textos de los resultados, con la cultura de la interfaz del hilo (<c>CurrentUICulture</c>).
/// </summary>
internal static class ResultsResource
{
    private static readonly ResourceManager Manager = new ("Kitpymes.Core.Results.Internal.ResultsResource", typeof(ResultsResource).Assembly);

    /// <summary>
    /// Obtiene el título de un resultado correcto.
    /// </summary>
    internal static string MsgProcessRanSuccessfully => Get(nameof(MsgProcessRanSuccessfully));

    /// <summary>
    /// Obtiene el título de un resultado con errores de validación.
    /// </summary>
    internal static string MsgValidationsError => Get(nameof(MsgValidationsError));

    private static string Get(string name) => Manager.GetString(name) ?? name;
}
