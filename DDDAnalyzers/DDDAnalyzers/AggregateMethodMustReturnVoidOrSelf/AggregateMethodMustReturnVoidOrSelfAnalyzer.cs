using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DDDAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AggregateMethodMustReturnVoidOrSelfAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AggregateMethodMustReturnVoidOrSelf";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AggregateMethodMustReturnVoidOrSelfTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AggregateMethodMustReturnVoidOrSelfMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AggregateMethodMustReturnVoidOrSelfDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title,
            MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Method);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            // TODO: Replace the following code with your own analysis, generating Diagnostic objects for any issues you find
            var methodSymbol = (IMethodSymbol)context.Symbol;
            if (!methodSymbol.ContainingType.Interfaces.Any(xx => xx.Name.StartsWith("IAggregateRoot"))
                && !methodSymbol.ContainingType.BaseType.Name.StartsWith("IAggregateRoot")
                ) return;
            if (methodSymbol.DeclaredAccessibility != Accessibility.Public) return;

            if (methodSymbol.ReturnsVoid) return;
            if (methodSymbol.ReturnType == methodSymbol.ContainingType) return;
            if (methodSymbol.Name.StartsWith("get_")) return;
            if (methodSymbol.Name.StartsWith("set_")) return;

            // For all such symbols, produce a diagnostic.
            var diagnostic = Diagnostic.Create(
                Rule
                , methodSymbol.Locations[0]
                , methodSymbol.Name
                , methodSymbol.ContainingType.Name
            );

            context.ReportDiagnostic(diagnostic);
        }
    }
}
