// -----------------------------------------------------------------------
// <copyright file="FieldError.cs" company="Kitpymes">
// Copyright (c) Kitpymes. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project docs folder for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Kitpymes.Core.Results;

using System;

/// <summary>
/// Error de un campo concreto, pensado para los formularios.
/// </summary>
public sealed class FieldError
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="FieldError"/>.
    /// </summary>
    /// <param name="field">Nombre del campo.</param>
    /// <param name="code">Código estable del error (p. ej. <c>Validations.Requerido</c>).</param>
    /// <param name="message">Mensaje legible.</param>
    public FieldError(string field, string code, string message)
    {
        Field = Required(field, nameof(field));
        Code = Required(code, nameof(code));
        Message = Required(message, nameof(message));
    }

    /// <summary>
    /// Obtiene el nombre del campo.
    /// </summary>
    public string Field { get; }

    /// <summary>
    /// Obtiene el código estable del error.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Obtiene el mensaje legible.
    /// </summary>
    public string Message { get; }

    /// <inheritdoc/>
    public override string ToString() => $"{Field}: {Code} - {Message}";

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"El parámetro {name} no puede estar vacío.", name);
        }

        return value;
    }
}
