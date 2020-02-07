using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
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
            // TODO: generate the full path to the exe.
            return Path.Combine(this.NuGetPath, "rapidxaml.buildanalysis", "0.1.37", "tools", "net472", ToolExe);
        }

        protected override string GenerateCommandLineCommands()
        {
            return this.Command;
        }

        protected override string GetWorkingDirectory()
        {
            // TODO: set the path to that of the project file to make references work correctly
            return base.GetWorkingDirectory();
        }

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, $"In {ToolName} tool : {Command}");
            Log.LogWarning($"Command : {Command}");
            Log.LogWarning($"NuGetPath : {NuGetPath}");
            Log.LogWarning($"Missing {ToolName} functionality 37");
            Log.LogWarning($"ToolPath: {ToolPath} ");
            Log.LogWarning($"GetWorkingDirectory: {GetWorkingDirectory()} ");
            Log.LogWarning($"GenerateFullPathToTool: {GenerateFullPathToTool()} ");

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
