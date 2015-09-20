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

using RoslynTools;

namespace DDDAnalyzers
{
    // https://msdn.microsoft.com/en-us/magazine/dn904670.aspx

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PropertiesCannotBePublicCodeFixProvider)), Shared]
    public class AggregateMethodMustReturnVoidOrSelfCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(AggregateMethodMustReturnVoidOrSelfAnalyzer.DiagnosticId); }
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
            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

            var title = "Return void";
            context.RegisterCodeFix(
                CodeAction.Create(
                    title
                    , c => ReturnVoidAsync(context.Document, declaration, c)
                    , title)
                , diagnostic);

            var title2 = "Return self";
            context.RegisterCodeFix(
                CodeAction.Create(
                    title2
                    , c => ReturnSelfAsync(context.Document, declaration, c)
                    , title2)
                , diagnostic);
        }

        private async Task<Document> ReturnVoidAsync(Document document, MethodDeclarationSyntax method, CancellationToken cancellationToken)
        {
            var newStatements = new List<StatementSyntax>();
            foreach (var s in method.Body.Statements)
            {
                if (s is ReturnStatementSyntax)
                {
                    continue;
                }
                else
                    newStatements.Add(s);
            }

            var newMethodDeclaration =
                SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("void").WithLeadingTrivia(method.ReturnType.GetLeadingTrivia()).WithTrailingTrivia(method.ReturnType.GetTrailingTrivia()), method.Identifier)
                .AddModifiers(method.Modifiers.ToArray())
                .WithLeadingTrivia(method.GetLeadingTrivia())
                .WithBody(SyntaxFactory.Block(newStatements))
                ;

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(method, newMethodDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }

        private async Task<Document> ReturnSelfAsync(Document document, MethodDeclarationSyntax method, CancellationToken cancellationToken)
        {
            var syntaxList = method.Body.Statements;
            foreach (var s in syntaxList.ToArray())
            {
                if (s is ReturnStatementSyntax)
                {
                    syntaxList = syntaxList.Replace(s, 
                        SyntaxFactory.ReturnStatement(
                            SyntaxFactory.ThisExpression())
                            .WithLeadingTrivia(
                                s.GetLeadingTrivia()
                            )
                        )
                    ;                
                }
            }

            var newMethod = 
                method
                .WithBody(SyntaxFactory.Block(syntaxList).WithTriviaFrom(method.Body))
                .WithReturnType(SyntaxFactory.ParseTypeName(method.Parent.Identifier()).WithTriviaFrom(method.ReturnType))
            ;

            var root = await document.GetSyntaxRootAsync();
            var newRoot = root.ReplaceNode(method, newMethod);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}