using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace RapidXamlBuildAnalysis
{
    public class Program 
    {
      //  protected override string ToolName => "RapidXaml.BuildAnalysis";

        public string Command { get; set; }

        static void Main(string[] args)
        {
           //Log.LogMessage(MessageImportance.High, "In custom tool");
            Console.WriteLine("Hello World!");
            Console.WriteLine($"Arguments: {args[0]}");
        }

        //protected override string GenerateFullPathToTool()
        //{
        //    return this.Command;
        //}
    }
}
