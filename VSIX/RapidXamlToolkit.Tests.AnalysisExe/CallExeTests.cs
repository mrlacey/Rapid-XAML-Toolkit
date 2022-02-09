using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RapidXamlToolkit.Tests.AnalysisExe
{
    [TestClass]
    public class CallExeTests
    {
        [TestMethod]
        public void GiveCorrectErrorCodeWhen_NoInputFileProvided()
        {
            var (exitCode, _) = RunApp("");

            Assert.AreEqual(1, exitCode);
        }

        [TestMethod]
        public void GiveCorrectErrorCodeWhen_InputFileDoesNotExist()
        {
            var (exitCode, _) = RunApp(@"..\..\..\BuildAnalysisUwpTestApp\does.not.exist");

            Assert.AreEqual(2, exitCode);
        }

        [TestMethod]
        public void GiveCorrectErrorCodeWhen_InputFileIsNotASupportedType()
        {
            var (exitCode, _) = RunApp(@"..\..\..\BuildAnalysisUwpTestApp\package.appxmanifest");

            Assert.AreEqual(3, exitCode);
        }

        [TestMethod]
        public void TestThatGetOutputFromRealProject()
        {
            var args = @"..\..\..\BuildAnalysisUwpTestApp\BuildAnalysisUwpTestApp.csproj";

            var (exitCode, output) = RunApp(args);

            Assert.AreEqual(0, exitCode);
            Assert.IsTrue(output.Length > 200);
        }

        private (int exitCode, string output) RunApp(string args)
        {
#if DEBUG
            var relExePath = @"..\..\..\RapidXaml.AnalysisExe\bin\debug\net472\RapidXaml.AnalysisExe.exe";
#else
            var relExePath = @"..\..\..\RapidXaml.AnalysisExe\bin\Release\net472\RapidXaml.AnalysisExe.exe";
#endif

            var startInfo = new ProcessStartInfo(relExePath)
            {
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
            };

            var process = Process.Start(startInfo);

            var output = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            return (process.ExitCode, output);
        }
    }
}
