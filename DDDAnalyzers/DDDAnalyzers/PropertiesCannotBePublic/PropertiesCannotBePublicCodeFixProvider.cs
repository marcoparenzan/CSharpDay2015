using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;

namespace DDDAnalyzers
{
    // https://msdn.microsoft.com/en-us/magazine/dn904670.aspx

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PropertiesCannotBePublicCodeFixProvider)), Shared]
    public class PropertiesCannotBePublicCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(PropertiesCannotBePublicAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<PropertyDeclarationSyntax>().First();

            var title = "Make it private";
            context.RegisterCodeFix(
                CodeAction.Create(
                    title
                    , c => MakePrivateAsync(context.Document, declaration, c)
                    , title)
                , diagnostic);
        }

        private async Task<Document> MakePrivateAsync(Document document, PropertyDeclarationSyntax property, CancellationToken cancellationToken)
        {
            var newPropertyDeclaration = 
                SyntaxFactory.PropertyDeclaration(property.Type, property.Identifier)
                .AddModifiers(SyntaxFactory.Token(SyntaxTriviaList.Empty, SyntaxKind.PrivateKeyword, SyntaxTriviaList.Create(SyntaxFactory.Space)))
                .WithLeadingTrivia(property.GetLeadingTrivia())
                .WithAccessorList(property.AccessorList)
                ;

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(property, newPropertyDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}