using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using DDDAnalyzers;

namespace DDDAnalyzers.Test
{
    [TestClass]
    public class PropertiesCannotBePublicUnitTests : CodeFixVerifier
    {

        //No diagnostics expected to show up
        [TestMethod]
        public void TestMethod1()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public void TestMethod2()
        {
            var test = @"
    using System;
    using DDDCore;

    namespace MyDomain
    {
        public class MyAggregateRoot : IAggregateRoot<int>
        {
            public int AggregateRootId { get; set; }

            int IAggregateRoot<int>.Id
            {
                get
                {
                    return AggregateRootId;
                }
            }
        }
    }
";
            var expected = new DiagnosticResult
            {
                Id = "PropertiesCannotBePublic",
                Message = String.Format("Property '{0}' in '{1}' cannot not be public", "AggregateRootId", "MyAggregateRoot"),
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 9, 24)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
    using System;
    using DDDCore;

    namespace MyDomain
    {
        public class MyAggregateRoot : IAggregateRoot<int>
        {
            private int AggregateRootId { get; set; }

            int IAggregateRoot<int>.Id
            {
                get
                {
                    return AggregateRootId;
                }
            }
        }
    }
";
            VerifyCSharpFix(test, fixtest);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new PropertiesCannotBePublicCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new PropertiesCannotBePublicAnalyzer();
        }
    }
}