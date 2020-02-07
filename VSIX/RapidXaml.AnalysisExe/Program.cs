using System;

namespace RapidXaml.AnalysisExe
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!  25");

            if (args.Length < 1)
            {
                Console.WriteLine("expecting a project file as first command argument.");
            }
            else
            {
                Console.WriteLine(args[0]);
                // TODO: check file exists
                // TODO: check file is a project file

                var ProjectPath = args[0];

                var projFileLines = System.IO.File.ReadAllLines(ProjectPath);

                foreach (var line in projFileLines)
                {
                    var endPos = line.IndexOf(".xaml\"");
                    if (endPos > 1)
                    {
                        var startPos = line.IndexOf("Include");

                        if (startPos > 1)
                        {
                            var relativeFilePath = line.Substring(startPos + 9, endPos + 5 - startPos - 9);

                            //Log.LogMessage(MessageImportance.High, $"- {relativeFilePath}");
                            Console.WriteLine($"Warning: {relativeFilePath}");
                        }
                    }
                }
            }
        }
    }
}
