// -----------------------------------------------------------------------
// <copyright file="ResultOfT.cs" company="Kitpymes">
// Copyright (c) Kitpymes. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project docs folder for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Kitpymes.Core.Results;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Resultado de una operación que, si termina bien, devuelve un valor.
/// </summary>
/// <typeparam name="T">Tipo del valor.</typeparam>
/// <remarks>
/// Leer <see cref="Value"/> sin comprobar antes <see cref="IsSuccess"/> es un bug: con tipos de referencia el
/// compilador avisa (análisis de nulos); con tipos de valor no puede, y <see cref="Value"/> lanza
/// <see cref="InvalidOperationException"/> si el resultado es un fallo.
/// No hay conversión implícita desde <typeparamref name="T"/> a propósito: con nulos, interfaces,
/// <c>Result&lt;Error&gt;</c> o <c>Result&lt;Result&gt;</c> daría errores de compilación o éxitos falsos.
/// Se crea con <see cref="Result.Success{T}(T)"/>. Sí hay conversión implícita desde <see cref="Error"/>.
/// </remarks>
public sealed class Result<T> : Result, IResultValue
{
    private readonly T? _value;

    private Result(T value)
        : base(null) => _value = value;

    private Result(Error error)
        : base(error) => _value = default;

    /// <summary>
    /// Obtiene un valor que indica si la operación terminó bien.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public new bool IsSuccess => base.IsSuccess;

    /// <summary>
    /// Obtiene un valor que indica si la operación falló.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Value))]
    [MemberNotNullWhen(true, nameof(Error))]
    public new bool IsFailure => base.IsFailure;

    /// <summary>
    /// Obtiene el error, o <c>null</c> si la operación terminó bien.
    /// </summary>
    /// <remarks>Se redeclara para que el análisis de nulos de <see cref="IsSuccess"/> lo vea en <see cref="Result{T}"/>.</remarks>
    public new Error? Error => base.Error;

    /// <summary>
    /// Obtiene el valor. Lanza <see cref="InvalidOperationException"/> si el resultado es un fallo.
    /// </summary>
    public T? Value => base.IsSuccess
        ? _value
        : throw new InvalidOperationException($"No se puede leer el valor de un resultado fallido ({Error}). Comprueba antes IsSuccess.");

    /// <inheritdoc/>
    object? IResultValue.BoxedValue => Value;

    /// <summary>
    /// Convierte un error en un resultado fallido.
    /// </summary>
    /// <param name="error">Error.</param>
    public static implicit operator Result<T>(Error error) => Failure(error);

    /// <summary>
    /// Crea un resultado correcto.
    /// </summary>
    /// <param name="value">Valor.</param>
    /// <returns>El resultado.</returns>
    public static Result<T> Success(T value) => new (value);

    /// <summary>
    /// Crea un resultado fallido.
    /// </summary>
    /// <param name="error">Error.</param>
    /// <returns>El resultado.</returns>
    public static new Result<T> Failure(Error error) => new (NotNull(error));

    /// <summary>
    /// Intenta leer el valor.
    /// </summary>
    /// <param name="value">Valor, si la operación terminó bien.</param>
    /// <returns><c>true</c> si terminó bien.</returns>
    public bool TryGetValue([MaybeNullWhen(false)] out T value)
    {
        value = _value;
        return base.IsSuccess;
    }
}
