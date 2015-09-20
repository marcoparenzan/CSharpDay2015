using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDDCodeGeneration
{
    class Program
    {
        static void Main(string[] args)
        {



            IWorkspace workspace = Workspace.LoadSolution("MySolution.sln");
            var originalSolution = workspace.CurrentSolution;
            var project = originalSolution.GetProject(originalSolution.ProjectIds.First());
            IDocument doc = project.AddDocument("index.html", "<html></html>");
            workspace.ApplyChanges(originalSolution, doc.Project.Solution);

            var document = Document
            var root = (CompilationUnitSyntax)document.GetSyntaxRoot();

            // Add the namespace
            var namespaceAnnotation = new SyntaxAnnotation();
            root = root.WithMembers(
                Syntax.NamespaceDeclaration(
                    Syntax.ParseName("ACO"))
                        .NormalizeWhitespace()
                        .WithAdditionalAnnotations(namespaceAnnotation));
            document = document.UpdateSyntaxRoot(root);

            // Add a class to the newly created namespace, and update the document
            var namespaceNode = (NamespaceDeclarationSyntax)root
                .GetAnnotatedNodesAndTokens(namespaceAnnotation)
                .Single()
                .AsNode();

            var classAnnotation = new SyntaxAnnotation();
            var baseTypeName = Syntax.ParseTypeName("System.Windows.Forms.Form");
            SyntaxTokenList syntaxTokenList = new SyntaxTokenList()
        {
            Syntax.Token(SyntaxKind.PublicKeyword)
        };

            var newNamespaceNode = namespaceNode
                .WithMembers(
                    Syntax.List<MemberDeclarationSyntax>(
                        Syntax.ClassDeclaration("MainForm")
                            .WithAdditionalAnnotations(classAnnotation)
                            .AddBaseListTypes(baseTypeName)
                            .WithModifiers(Syntax.Token(SyntaxKind.PublicKeyword))));

            root = root.ReplaceNode(namespaceNode, newNamespaceNode).NormalizeWhitespace();
            document = document.UpdateSyntaxRoot(root);


            var attributes = Syntax.List(Syntax.AttributeDeclaration(Syntax.SeparatedList(Syntax.Attribute(Syntax.ParseName("STAThread")))));


            // Find the class just created, add a method to it and update the document
            var classNode = (ClassDeclarationSyntax)root
                .GetAnnotatedNodesAndTokens(classAnnotation)
                .Single()
                .AsNode();

            var syntaxList = Syntax.List<MemberDeclarationSyntax>(
                    Syntax.MethodDeclaration(
                        Syntax.ParseTypeName("void"), "Main")
                        .WithModifiers(Syntax.TokenList(Syntax.Token(SyntaxKind.PublicKeyword)))
                        .WithAttributes(attributes)
                        .WithBody(
                            Syntax.Block()));
            syntaxList = syntaxList.Add(Syntax.PropertyDeclaration(Syntax.ParseTypeName("System.Windows.Forms.Timer"), "Ticker"));
            var newClassNode = classNode
                .WithMembers(syntaxList);

            root = root.ReplaceNode(classNode, newClassNode).NormalizeWhitespace();
            document = document.UpdateSyntaxRoot(root);
        }
    }
}
