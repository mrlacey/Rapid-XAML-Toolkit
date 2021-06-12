// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RapidXamlToolkit.RoslynAnalyzers;
using TestHelper;

namespace RapidXamlToolkit.Tests.RoslynAnalyzers
{
    [TestClass]
    public class ExpandAutoPropertyTestsCs : CodeFixVerifier
    {
        [TestMethod]
        public void PrivateSetter_NotIdentified()
        {
            var test = @"public string Property1 { get; private set; }";

            this.VerifyCSharpDiagnostic(this.ClassWithOnPropertyChanged(test));
        }

        [TestMethod]
        public void NoAccessorList_NotIdentified()
        {
            var test = @"public string Property1 => $""Some static value"";";

            this.VerifyCSharpDiagnostic(this.ClassWithOnPropertyChanged(test));
        }

        [TestMethod]
        public void ClassWithOnPropertyChanged_Identified()
        {
            var test = @"public string Property1 { get; set; }";

            var expected = this.CreateDiagnosticResult(CsExpandAutoPropertiesAnalyzer.OnPropertyChangedDiagnosticId);

            this.VerifyCSharpDiagnostic(this.ClassWithOnPropertyChanged(test), expected);
        }

        [TestMethod]
        public void ClassWithSet_Identified()
        {
            var test = @"public string Property1 { get; set; }";

            var expected2 = this.CreateDiagnosticResult(CsExpandAutoPropertiesAnalyzer.SetDiagnosticId);
            var expected1 = this.CreateDiagnosticResult(CsExpandAutoPropertiesAnalyzer.OnPropertyChangedDiagnosticId);

            this.VerifyCSharpDiagnostic(this.ClassWithSetAndOnPropertyChanged(test), expected2, expected1);
        }

        [TestMethod]
        public void ClassWithSetProperty_Identified()
        {
            var test = @"public string Property1 { get; set; }";

            var expected3 = this.CreateDiagnosticResult(CsExpandAutoPropertiesAnalyzer.SetPropertyDiagnosticId);
            var expected1 = this.CreateDiagnosticResult(CsExpandAutoPropertiesAnalyzer.OnPropertyChangedDiagnosticId);

            this.VerifyCSharpDiagnostic(this.ClassWithSetPropertyAndOnPropertyChanged(test), expected3, expected1);
        }

        [TestMethod]
        public void ClassInheritsFromDependencyObject_Identified()
        {
            var test = @"public string Property1 { get; set; }";

            var expected4 = this.CreateDiagnosticResult(CsExpandAutoPropertiesAnalyzer.DependencyPropertyDiagnosticId);

            this.VerifyCSharpDiagnostic(this.ClassInheritsFromDependencyObject(test), expected4);
        }

        [TestMethod]
        public void ClassInheritsFromBindableObject_Identified()
        {
            var test = @"public string Property1 { get; set; }";

            var expected5 = this.CreateDiagnosticResult(CsExpandAutoPropertiesAnalyzer.BindablePropertyDiagnosticId);
            var expected15 = this.CreateDiagnosticResult(CsExpandAutoPropertiesAnalyzer.BindablePropertySimplifiedDiagnosticId);

            this.VerifyCSharpDiagnostic(this.ClassInheritsFromBindableObject(test), expected5, expected15);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new CsExpandAutoPropertiesAnalyzer();
        }

        private DiagnosticResult CreateDiagnosticResult(string id)
        {
            return new DiagnosticResult
            {
                Id = id,
                Message = "Expand property 'Property1'",
                Severity = DiagnosticSeverity.Hidden,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 3, 19),
                    },
            };
        }

        private string ClassWithOnPropertyChanged(string property)
        {
            return $"public class MyClass : INotifyPropertyChanged\n" +
                $"{{\n" +
                $"    {property}\n" +
                $"\n" +
                $"    public event PropertyChangedEventHandler PropertyChanged;\n" +
                $"\n" +
                $"    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));\n" +
                $"}}";
        }

        private string ClassWithSetAndOnPropertyChanged(string property)
        {
            return $"public class MyClass : INotifyPropertyChanged\n" +
                $"{{\n" +
                $"    {property}\n" +
                $"\n" +
                $"    public event PropertyChangedEventHandler PropertyChanged;\n" +
                $"\n" +
                $"    protected void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)\n" +
                $"    {{\n" +
                $"        if (Equals(storage, value))" +
                $"        {{" +
                $"            return;\n" +
                $"        }}\n" +
                $"\n" +
                $"        storage = value;\n" +
                $"        OnPropertyChanged(propertyName);\n" +
                $"    }}\n" +
                $"\n" +
                $"    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));\n" +
                $"}}";
        }

        private string ClassWithSetPropertyAndOnPropertyChanged(string property)
        {
            return $"public class MyClass : INotifyPropertyChanged\n" +
                $"{{\n" +
                $"    {property}\n" +
                $"\n" +
                $"    public event PropertyChangedEventHandler PropertyChanged;\n" +
                $"\n" +
                $"    protected void SetProperty<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)\n" +
                $"    {{\n" +
                $"        if (Equals(storage, value))" +
                $"        {{" +
                $"            return;\n" +
                $"        }}\n" +
                $"\n" +
                $"        storage = value;\n" +
                $"        OnPropertyChanged(propertyName);\n" +
                $"    }}\n" +
                $"\n" +
                $"    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));\n" +
                $"}}";
        }

        private string ClassInheritsFromDependencyObject(string property)
        {
            return $"public class MyClass : DependencyObject\n" +
                $"{{\n" +
                $"    {property}\n" +
                $"}}";
        }

        private string ClassInheritsFromBindableObject(string property)
        {
            return $"public class MyClass : BindableObject\n" +
                $"{{\n" +
                $"    {property}\n" +
                $"}}";
        }
    }
}
