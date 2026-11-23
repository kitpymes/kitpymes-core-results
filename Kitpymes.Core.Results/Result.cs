// -----------------------------------------------------------------------
// <copyright file="Result.cs" company="Kitpymes">
// Copyright (c) Kitpymes. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project docs folder for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Kitpymes.Core.Results;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

/// <summary>
/// Resultado de una operación de dominio o de aplicación, sin nada de HTTP.
/// Los errores de negocio se devuelven aquí, nunca como excepciones.
/// </summary>
/// <remarks>
/// La jerarquía está cerrada: sólo existen <see cref="Result"/> y <see cref="Result{T}"/>.
/// Para la respuesta HTTP se traduce con <see cref="ResultExtensions.ToApiResult(Result)"/>.
/// </remarks>
public class Result
{
    private static readonly Result SuccessInstance = new (null);

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="Result"/>.
    /// </summary>
    /// <param name="error">Error, o <c>null</c> si es un éxito.</param>
    private protected Result(Error? error) => Error = error;

    /// <summary>
    /// Obtiene un valor que indica si la operación terminó bien.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess => Error is null;

    /// <summary>
    /// Obtiene un valor que indica si la operación falló.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsFailure => Error is not null;

    /// <summary>
    /// Obtiene el error, o <c>null</c> si la operación terminó bien.
    /// </summary>
    public Error? Error { get; }

    /// <summary>
    /// Convierte un error en un resultado fallido.
    /// </summary>
    /// <param name="error">Error.</param>
    public static implicit operator Result(Error error) => Failure(error);

    /// <summary>
    /// Crea un resultado correcto sin valor.
    /// </summary>
    /// <returns>El resultado.</returns>
    public static Result Success() => SuccessInstance;

    /// <summary>
    /// Crea un resultado correcto con valor.
    /// </summary>
    /// <typeparam name="T">Tipo del valor.</typeparam>
    /// <param name="value">Valor.</param>
    /// <returns>El resultado.</returns>
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);

    /// <summary>
    /// Crea un resultado fallido.
    /// </summary>
    /// <param name="error">Error.</param>
    /// <returns>El resultado.</returns>
    public static Result Failure(Error error) => new (NotNull(error));

    /// <summary>
    /// Crea un resultado fallido del tipo pedido, sea <see cref="Result"/> o <see cref="Result{T}"/>.
    /// Pensado para quien sólo conoce el tipo genérico (p. ej. el mediador cuando captura algo inesperado).
    /// Usa reflexión una sola vez por tipo; no está pensado para NativeAOT.
    /// </summary>
    /// <typeparam name="TResult"><see cref="Result"/> o <see cref="Result{T}"/>.</typeparam>
    /// <param name="error">Error.</param>
    /// <returns>El resultado.</returns>
    public static TResult FailureFor<TResult>(Error error)
        where TResult : Result
        => FailureFactory<TResult>.Create(NotNull(error));

    /// <summary>
    /// Comprueba que el error no es nulo.
    /// </summary>
    /// <param name="error">Error.</param>
    /// <returns>El mismo error.</returns>
    private protected static Error NotNull(Error error)
        => error ?? throw new ArgumentNullException(nameof(error));

    /// <summary>
    /// Fábrica de resultados fallidos por tipo, creada una sola vez por tipo.
    /// </summary>
    /// <typeparam name="TResult">Tipo del resultado.</typeparam>
    private static class FailureFactory<TResult>
        where TResult : Result
    {
        private static readonly Func<Error, TResult> Factory = Build();

        public static TResult Create(Error error) => Factory(error);

        private static Func<Error, TResult> Build()
        {
            // La jerarquía está cerrada: TResult es Result o Result<T>, y los dos tienen
            // 'public static <su tipo> Failure(Error)'.
            var method = typeof(TResult).GetMethod(nameof(Failure), BindingFlags.Public | BindingFlags.Static, new[] { typeof(Error) });

            return (Func<Error, TResult>)Delegate.CreateDelegate(typeof(Func<Error, TResult>), method!);
        }
    }
}
