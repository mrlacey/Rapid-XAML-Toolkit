using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using RapidXamlToolkit;
using RapidXamlToolkit.XamlAnalysis;
using System;
using System.Collections.Generic;
using System.IO;

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
                var projectPath = args[0];

                if (!File.Exists(projectPath))
                {
                    // TODO: log file not exists
                    Environment.ExitCode = 2;
                    return;
                }

                var fileExt = Path.GetExtension(projectPath);

                if (!fileExt.ToLowerInvariant().Equals(".csproj")
                  & !fileExt.ToLowerInvariant().Equals(".vbproj"))
                {
                    // TODO: check file is a project file
                    Environment.ExitCode = 3;
                    return;
                }

                var projFileLines = File.ReadAllLines(projectPath);

                var projDir = Path.GetDirectoryName(projectPath);
              //  Console.WriteLine($"Warning: {projDir}");

                foreach (var line in projFileLines)
                {
                    var endPos = line.IndexOf(".xaml\"");
                    if (endPos > 1)
                    {
                        var startPos = line.IndexOf("Include");

                        if (startPos > 1)
                        {
                            var relativeFilePath = line.Substring(startPos + 9, endPos + 5 - startPos - 9);

                            var xamlFilePath = Path.Combine(projDir, relativeFilePath);

                            //Log.LogMessage(MessageImportance.High, $"- {relativeFilePath}");
                            Console.WriteLine($"Warning: {xamlFilePath}");


                            var text = File.ReadAllText(xamlFilePath);

                            if (text.IsValidXml())
                            {
                                var result = new RapidXamlDocument();

                                var snapshot = new StubTextSnapshot();

                                XamlElementExtractor.Parse(ProjectType.Any, xamlFilePath, snapshot, text, RapidXamlDocument.GetAllProcessors(ProjectType.Any), result.Tags);

                                Console.WriteLine($"Found {result.Tags.Count} taggable issues in '{xamlFilePath}'.");

                                if (result.Tags.Count > 0)
                                {
                                    Console.WriteLine($"Found {result.Tags.Count} taggable issues in '{xamlFilePath}'.");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Invalid XAML found in '{xamlFilePath}'.");
                            }
                        }
                    }
                }
            }
        }
    }

    public class StubTextSnapshot : ITextSnapshot
    {
        public ITextBuffer TextBuffer { get; }

        public IContentType ContentType { get; }

        public ITextVersion Version { get; }

        public int Length { get; }

        public int LineCount { get; }

        public IEnumerable<ITextSnapshotLine> Lines { get; }

        public char this[int position] => throw new NotImplementedException();

        public string GetText(Span span)
        {
            return string.Empty;
        }

        public string GetText(int startIndex, int length)
        {
            return string.Empty;
        }

        public string GetText()
        {
            return string.Empty;
        }

        public char[] ToCharArray(int startIndex, int length)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
        {
        }

        public ITrackingPoint CreateTrackingPoint(int position, PointTrackingMode trackingMode)
        {
            throw new NotImplementedException();
        }

        public ITrackingPoint CreateTrackingPoint(int position, PointTrackingMode trackingMode, TrackingFidelityMode trackingFidelity)
        {
            throw new NotImplementedException();
        }

        public ITrackingSpan CreateTrackingSpan(Span span, SpanTrackingMode trackingMode)
        {
            throw new NotImplementedException();
        }

        public ITrackingSpan CreateTrackingSpan(Span span, SpanTrackingMode trackingMode, TrackingFidelityMode trackingFidelity)
        {
            throw new NotImplementedException();
        }

        public ITrackingSpan CreateTrackingSpan(int start, int length, SpanTrackingMode trackingMode)
        {
            throw new NotImplementedException();
        }

        public ITrackingSpan CreateTrackingSpan(int start, int length, SpanTrackingMode trackingMode, TrackingFidelityMode trackingFidelity)
        {
            throw new NotImplementedException();
        }

        public ITextSnapshotLine GetLineFromLineNumber(int lineNumber)
        {
            throw new NotImplementedException();
        }

        public ITextSnapshotLine GetLineFromPosition(int position)
        {
            return new StubTextSnapshotLine(this);
        }

        public int GetLineNumberFromPosition(int position)
        {
            // This is sufficient for current testing needs
            return -1;
        }

        public void Write(TextWriter writer, Span span)
        {
        }

        public void Write(TextWriter writer)
        {
        }
    }

    public class StubTextSnapshotLine : ITextSnapshotLine
    {
        private readonly ITextSnapshot snapshot;

        public StubTextSnapshotLine(ITextSnapshot snapshot)
        {
            this.snapshot = snapshot;
        }

        public ITextSnapshot Snapshot { get; }

        public SnapshotSpan Extent { get; }

        public SnapshotSpan ExtentIncludingLineBreak { get; }

        // This is currently sufficient for getting test to pass, but not a long term solution
        public int LineNumber => -1;

        // This is currently sufficient for getting test to pass, but not a long term solution
        public SnapshotPoint Start => new SnapshotPoint(this.snapshot, 0);

        public int Length { get; }

        public int LengthIncludingLineBreak { get; }

        public SnapshotPoint End { get; }

        public SnapshotPoint EndIncludingLineBreak { get; }

        public int LineBreakLength { get; }

        public string GetText()
        {
            return string.Empty;
        }

        public string GetTextIncludingLineBreak()
        {
            return string.Empty;
        }

        public string GetLineBreakText()
        {
            return string.Empty;
        }
    }
}
