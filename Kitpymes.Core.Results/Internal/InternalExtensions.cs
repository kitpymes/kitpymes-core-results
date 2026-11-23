// -----------------------------------------------------------------------
// <copyright file="InternalExtensions.cs" company="Kitpymes">
// Copyright (c) Kitpymes. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project docs folder for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Kitpymes.Core.Results.Internal;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

/*
    Lo que Results usaba de Kitpymes.Core.Shared, copiado como interno para no depender de Shared
    (y, con él, del framework de ASP.NET Core). Mismo comportamiento que Shared 2.0.1, salvo la
    excepción: donde Shared lanzaba SharedException, aquí se lanza ArgumentException con el mismo mensaje.
*/

/// <summary>
/// Extensiones internas de Results.
/// </summary>
internal static class InternalExtensions
{
    /// <summary>
    /// Verifica si una colección es nula o no contiene elementos (antes <c>ToIsNullOrAny</c> de Shared).
    /// </summary>
    /// <param name="argument">Colección a verificar.</param>
    /// <returns>true si es nula o está vacía.</returns>
    internal static bool ToIsNullOrAny(this IEnumerable? argument)
        => argument is null || !argument.Cast<object>().Any();

    /// <summary>
    /// Agrega un valor a la lista de una clave (antes <c>ToAddOrUpdate</c> de Shared).
    /// Si la clave ya existe y el valor no está en su lista, la lista pasa a tener sólo ese valor.
    /// </summary>
    /// <param name="dictionary">Diccionario.</param>
    /// <param name="key">Clave.</param>
    /// <param name="value">Valor.</param>
    /// <exception cref="ArgumentException">Si la clave o el valor son nulos, vacíos o sólo espacios.</exception>
    internal static void ToAddOrUpdate(this IDictionary<string, IEnumerable<string>> dictionary, string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("El campo key es requerido");
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("El campo value es requerido");
        }

        if (dictionary.TryGetValue(key, out var list))
        {
            if (!list.Contains(value))
            {
                dictionary[key] = new List<string> { value };
            }
        }
        else
        {
            dictionary.Add(key, new List<string> { value });
        }
    }

    /// <summary>
    /// Crea un Guid secuencial a partir de otro (antes <c>ToSecuencial</c> de Shared).
    /// Los 8 últimos bytes llevan la hora actual en ticks.
    /// </summary>
    /// <param name="input">Guid de partida.</param>
    /// <returns>Guid secuencial.</returns>
    internal static Guid ToSecuencial(this Guid input)
    {
        var counter = DateTime.UtcNow.Ticks;

        var bytes = input.ToByteArray();

        var counterBytes = BitConverter.GetBytes(Interlocked.Increment(ref counter));

        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(counterBytes);
        }

        bytes[08] = counterBytes[1];
        bytes[09] = counterBytes[0];
        bytes[10] = counterBytes[7];
        bytes[11] = counterBytes[6];
        bytes[12] = counterBytes[5];
        bytes[13] = counterBytes[4];
        bytes[14] = counterBytes[3];
        bytes[15] = counterBytes[2];

        return new Guid(bytes);
    }
}
