// -----------------------------------------------------------------------
// <copyright file="Error.cs" company="Kitpymes">
// Copyright (c) Kitpymes. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project docs folder for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Kitpymes.Core.Results;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Error de un <see cref="Result"/>: código estable, mensaje, tipo y, si hace falta, errores por campo.
/// </summary>
/// <remarks>
/// El código sigue la forma <c>&lt;Modulo&gt;.&lt;Motivo&gt;</c> (p. ej. <c>Users.EmailDuplicado</c>): PascalCase,
/// ASCII, sin espacios y con al menos un punto. Es contrato con quien lo consume (Angular lo usa para traducir):
/// una vez publicado no se renombra.
/// </remarks>
public sealed class Error
{
    private Error(string code, string message, ErrorType type, IReadOnlyList<FieldError> fields)
    {
        Code = code;
        Message = message;
        Type = type;
        Fields = fields;
    }

    /// <summary>
    /// Obtiene el código estable del error.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Obtiene el mensaje legible.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Obtiene el tipo de error.
    /// </summary>
    public ErrorType Type { get; }

    /// <summary>
    /// Obtiene los errores por campo (vacío si no hay).
    /// </summary>
    public IReadOnlyList<FieldError> Fields { get; }

    /// <summary>
    /// Crea un error de validación.
    /// </summary>
    /// <param name="code">Código estable.</param>
    /// <param name="message">Mensaje legible.</param>
    /// <param name="fields">Errores por campo.</param>
    /// <returns>El error.</returns>
    public static Error Validation(string code, string message, IEnumerable<FieldError>? fields = null)
        => Create(code, message, ErrorType.Validation, fields);

    /// <summary>
    /// Crea un error de "no existe".
    /// </summary>
    /// <param name="code">Código estable.</param>
    /// <param name="message">Mensaje legible.</param>
    /// <returns>El error.</returns>
    public static Error NotFound(string code, string message) => Create(code, message, ErrorType.NotFound, null);

    /// <summary>
    /// Crea un error de conflicto con el estado actual.
    /// </summary>
    /// <param name="code">Código estable.</param>
    /// <param name="message">Mensaje legible.</param>
    /// <returns>El error.</returns>
    public static Error Conflict(string code, string message) => Create(code, message, ErrorType.Conflict, null);

    /// <summary>
    /// Crea un error de "falta identificarse".
    /// </summary>
    /// <param name="code">Código estable.</param>
    /// <param name="message">Mensaje legible.</param>
    /// <returns>El error.</returns>
    public static Error Unauthorized(string code, string message) => Create(code, message, ErrorType.Unauthorized, null);

    /// <summary>
    /// Crea un error de "sin permiso".
    /// </summary>
    /// <param name="code">Código estable.</param>
    /// <param name="message">Mensaje legible.</param>
    /// <returns>El error.</returns>
    public static Error Forbidden(string code, string message) => Create(code, message, ErrorType.Forbidden, null);

    /// <summary>
    /// Crea un error de "módulo no contratado".
    /// </summary>
    /// <param name="code">Código estable.</param>
    /// <param name="message">Mensaje legible.</param>
    /// <returns>El error.</returns>
    public static Error NotContracted(string code, string message) => Create(code, message, ErrorType.NotContracted, null);

    /// <summary>
    /// Crea un error inesperado. El mensaje no debe llevar detalles internos: va a quien llama.
    /// </summary>
    /// <param name="code">Código estable.</param>
    /// <param name="message">Mensaje legible.</param>
    /// <returns>El error.</returns>
    public static Error Unexpected(string code, string message) => Create(code, message, ErrorType.Unexpected, null);

    /// <inheritdoc/>
    public override string ToString() => $"{Type} {Code}: {Message}";

    private static Error Create(string code, string message, ErrorType type, IEnumerable<FieldError>? fields)
    {
        if (!IsValidCode(code))
        {
            throw new ArgumentException($"El código de error '{code}' no sigue la forma <Modulo>.<Motivo> (PascalCase, ASCII, sin espacios).", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("El mensaje del error no puede estar vacío.", nameof(message));
        }

        var list = fields?.ToList() ?? new List<FieldError>();

        if (list.Any(x => x is null))
        {
            throw new ArgumentException("La lista de errores por campo no puede contener nulos.", nameof(fields));
        }

        return new Error(code, message, type, list.AsReadOnly());
    }

    private static bool IsValidCode(string? code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return false;
        }

        var parts = code.Split('.');

        return parts.Length >= 2 && parts.All(IsPascalAscii);
    }

    private static bool IsPascalAscii(string part)
        => part.Length > 0
        && part[0] >= 'A' && part[0] <= 'Z'
        && part.All(c => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'));
}
