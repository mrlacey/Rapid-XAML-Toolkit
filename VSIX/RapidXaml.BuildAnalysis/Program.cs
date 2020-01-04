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

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine($"Arguments: {args[0]}");
        }

        protected override string GenerateFullPathToTool()
        {
            // TODO: generate the full path to the exe.
            return this.Command; // Returning the command here causes the project file to be opened.
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
            Log.LogWarning($"Missing {ToolName} functionality");
            return base.Execute();
        }

        protected override int ExecuteTool(string pathToTool, string responseFileCommands, string commandLineCommands)
        {
            return base.ExecuteTool(pathToTool, responseFileCommands, commandLineCommands);
        }
    }
}
