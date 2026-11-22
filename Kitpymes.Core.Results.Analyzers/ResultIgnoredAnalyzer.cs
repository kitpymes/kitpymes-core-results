// -----------------------------------------------------------------------
// <copyright file="ResultIgnoredAnalyzer.cs" company="Kitpymes">
// Copyright (c) Kitpymes. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project docs folder for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace Kitpymes.Core.Results.Analyzers;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

/// <summary>
/// KPY0003: un <c>Result</c> (o una tarea que lo devuelve) se calcula y se tira sin mirarlo.
/// </summary>
/// <remarks>
/// Marca toda instrucción cuyo valor sea <c>Result</c>, <c>Result&lt;T&gt;</c>, un parámetro de tipo restringido a
/// <c>Result</c>, o un <c>Task</c>/<c>ValueTask</c> de cualquiera de ellos, también tras <c>await</c>
/// (con o sin <c>ConfigureAwait</c>) y con acceso condicional (<c>x?.Metodo()</c>).
/// No marca asignaciones: <c>_ = Metodo();</c> es el descarte explícito.
/// </remarks>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ResultIgnoredAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Identificador del diagnóstico.
    /// </summary>
    public const string DiagnosticId = "KPY0003";

    private static readonly DiagnosticDescriptor Rule = new (
        id: DiagnosticId,
        title: "No ignores el Result",
        messageFormat: "El {0} devuelto se ignora: compruébalo o descártalo de forma explícita con '_ ='",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Un Result lleva los errores de negocio. Tirarlo sin mirarlo esconde el fallo.",
        helpLinkUri: "https://github.com/kitpymes/kitpymes-core-results#kpy0003");

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(start =>
        {
            var result = start.Compilation.GetTypeByMetadataName("Kitpymes.Core.Results.Result");

            if (result is null)
            {
                return;
            }

            var task = start.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1");
            var valueTask = start.Compilation.GetTypeByMetadataName("System.Threading.Tasks.ValueTask`1");

            start.RegisterOperationAction(
                operationContext => Analyze(operationContext, result, task, valueTask),
                OperationKind.ExpressionStatement);
        });
    }

    private static void Analyze(OperationAnalysisContext context, INamedTypeSymbol result, INamedTypeSymbol? task, INamedTypeSymbol? valueTask)
    {
        var statement = (IExpressionStatementOperation)context.Operation;
        var operation = statement.Operation;

        // Asignar (incluido el descarte '_ =') es usar el valor.
        if (operation is IAssignmentOperation or IDeconstructionAssignmentOperation)
        {
            return;
        }

        var type = operation.Type;

        if (type is null)
        {
            return;
        }

        if (IsResult(type, result))
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule, operation.Syntax.GetLocation(), "Result"));
            return;
        }

        if (type is INamedTypeSymbol named
            && named.IsGenericType
            && named.TypeArguments.Length == 1
            && (SymbolEqualityComparer.Default.Equals(named.OriginalDefinition, task) || SymbolEqualityComparer.Default.Equals(named.OriginalDefinition, valueTask))
            && IsResult(named.TypeArguments[0], result))
        {
            context.ReportDiagnostic(Diagnostic.Create(Rule, operation.Syntax.GetLocation(), named.Name + "<Result>"));
        }
    }

    private static bool IsResult(ITypeSymbol type, INamedTypeSymbol result)
    {
        if (type is ITypeParameterSymbol parameter)
        {
            foreach (var constraint in parameter.ConstraintTypes)
            {
                if (IsResult(constraint, result))
                {
                    return true;
                }
            }

            return false;
        }

        for (var current = type; current is not null; current = current.BaseType)
        {
            if (SymbolEqualityComparer.Default.Equals(current.OriginalDefinition, result))
            {
                return true;
            }
        }

        return false;
    }
}
