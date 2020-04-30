// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Reflection;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace RapidXaml.BuildAnalysis
{
    public class Program : ToolTask
    {
        // This is the project file to analyze
        public string Command { get; set; }

        public string NuGetPath { get; set; }

        protected override string ToolName => "RapidXaml.AnalysisExe.exe";

#pragma warning disable IDE0060 // Remove unused parameter
        public static void Main(string[] args)
#pragma warning restore IDE0060 // Remove unused parameter
        {
        }

        public override bool Execute()
        {
            this.Log.LogMessage(MessageImportance.Normal, $"Executing {this.ToolName} {this.Command}");

            return base.Execute();
        }

        protected override string GenerateFullPathToTool()
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), this.ToolExe);
        }

        protected override string GenerateCommandLineCommands()
        {
            return this.Command;
        }

        protected override string GetWorkingDirectory()
        {
            return base.GetWorkingDirectory();
        }

        protected override int ExecuteTool(string pathToTool, string responseFileCommands, string commandLineCommands)
        {
            return base.ExecuteTool(pathToTool, responseFileCommands, commandLineCommands);
        }
    }
}
