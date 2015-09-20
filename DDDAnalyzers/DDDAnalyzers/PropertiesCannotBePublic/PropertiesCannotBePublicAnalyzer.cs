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
    public class PropertiesCannotBePublicAnalyzer : DiagnosticAnalyzer
    {
        public static readonly string DiagnosticId = "PropertiesCannotBePublic";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.PropertiesCannotBePublicTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.PropertiesCannotBePublicMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.PropertiesCannotBePublicDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Naming";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title,
            MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Property);
        }

        private static void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            // TODO: Replace the following code with your own analysis, generating Diagnostic objects for any issues you find
            var propertySymbol = (IPropertySymbol)context.Symbol;

            if (propertySymbol.DeclaredAccessibility != Accessibility.Public) return;

            // For all such symbols, produce a diagnostic.
            var diagnostic = Diagnostic.Create(
                Rule
                , propertySymbol.Locations[0]
                , propertySymbol.Name
                , propertySymbol.ContainingType.Name
            );

            context.ReportDiagnostic(diagnostic);
        }
    }
}
