using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace RapidXaml.BuildAnalysis
{
    public class Program : ToolTask
    {
        protected override string ToolName => "RapidXaml.AnalysisExe.exe";

        // This is the project file to analyze
        public string Command { get; set; }

        public string NuGetPath { get; set; }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine($"Arguments: {args[0]}");
        }

        protected override string GenerateFullPathToTool()
        {
            // TODO: need to remove the need to hardcode this path with the version number
            return Path.Combine(this.NuGetPath, "rapidxaml.buildanalysis", "0.1.37", "tools", "net472", ToolExe);
        }

        protected override string GenerateCommandLineCommands()
        {
            return this.Command;
        }

        protected override string GetWorkingDirectory()
        {
            return base.GetWorkingDirectory();
        }

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, $"In {ToolName} tool : {Command}");
            Log.LogWarning($"Command : {Command}");
            Log.LogWarning($"{ToolName} 37");

            return base.Execute();
        }

        protected override int ExecuteTool(string pathToTool, string responseFileCommands, string commandLineCommands)
        {
            Log.LogWarning($"ExecuteTool>pathToTool: {pathToTool} ");
            Log.LogMessage(MessageImportance.High, $"In ExecuteTool : {GenerateFullPathToTool()}");
            return base.ExecuteTool(pathToTool, responseFileCommands, commandLineCommands);
        }
    }
}
