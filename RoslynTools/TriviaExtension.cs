using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoslynTools
{
    public static class TriviaExtension
    {
        public static StatementSyntax Trivia(this StatementSyntax node, StatementSyntax that)
        {
            return node.WithLeadingTrivia(that.GetLeadingTrivia()).WithTrailingTrivia(that.GetTrailingTrivia());
        }

        public static TypeSyntax Trivia(this TypeSyntax node, TypeSyntax that)
        {
            return node.WithLeadingTrivia(that.GetLeadingTrivia()).WithTrailingTrivia(that.GetTrailingTrivia());
        }

        public static MethodDeclarationSyntax Trivia(this MethodDeclarationSyntax node, MethodDeclarationSyntax that)
        {
            return node.WithLeadingTrivia(that.GetLeadingTrivia()).WithTrailingTrivia(that.GetTrailingTrivia());
        }

        public static BlockSyntax Trivia(this BlockSyntax node, BlockSyntax that)
        {
            return node.WithLeadingTrivia(that.GetLeadingTrivia()).WithTrailingTrivia(that.GetTrailingTrivia());
        }
    }
}
