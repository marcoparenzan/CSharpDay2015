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
    public class AggregateMethodMustReturnVoidOrSelfUnitTests : CodeFixVerifier
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
            private int AggregateRootId { get; set; }

            int IAggregateRoot<int>.Id
            {
                get
                {
                    return AggregateRootId;
                }
            }

            public int Do()
            {
                return AggregateRootId;
            }
        }
    }
";
            var expected = new DiagnosticResult
            {
                Id = "AggregateMethodMustReturnVoidOrSelf",
                Message = String.Format("Method  '{0}' in '{1}' must return void on the same aggregate", "Do", "MyAggregateRoot"),
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 19, 24)
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

            public MyAggregateRoot Do()
        {
            return this;
        }
    }
    }
";
            VerifyCSharpFix(test, fixtest);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new AggregateMethodMustReturnVoidOrSelfCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new AggregateMethodMustReturnVoidOrSelfAnalyzer();
        }
    }
}