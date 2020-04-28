// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.IO;
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

        public static void Main(string[] args)
        {
        }

        public override bool Execute()
        {
            this.Log.LogMessage(MessageImportance.High, $"In {this.ToolName} tool : {this.Command}");
            this.Log.LogWarning($"Command : {this.Command}");
            this.Log.LogWarning($"{this.ToolName} 38");

            return base.Execute();
        }

        protected override string GenerateFullPathToTool()
        {
            // TODO: need to remove the need to hardcode this path with the version number
            return Path.Combine(this.NuGetPath, "rapidxaml.buildanalysis", "0.1.37", "tools", "net472", this.ToolExe);
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
            this.Log.LogWarning($"ExecuteTool>pathToTool: {pathToTool} ");
            this.Log.LogMessage(MessageImportance.High, $"In ExecuteTool : {this.GenerateFullPathToTool()}");
            return base.ExecuteTool(pathToTool, responseFileCommands, commandLineCommands);
        }
    }
}
