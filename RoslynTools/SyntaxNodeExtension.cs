using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoslynTools
{
    public static class SyntaxNodeExtension
    {
        public static string Identifier(this SyntaxNode node)
        {
            if (node is ClassDeclarationSyntax)
            {
                return ((ClassDeclarationSyntax)node).Identifier.ValueText;
            }
            else
            {
                throw new NotSupportedException("Node not recognized");
            }
        }
    }
}
