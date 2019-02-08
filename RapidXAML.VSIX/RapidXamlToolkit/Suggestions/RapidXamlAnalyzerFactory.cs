using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.Text;

namespace RapidXamlToolkit.Tagging
{
    public static class RapidXamlAnalyzerFactory
    {
        private const string AttachedExceptionKey = "attached-exception";
        public static object _syncRoot = new object();
        private static readonly ConditionalWeakTable<ITextSnapshot, XamlDocument> CachedDocuments = new ConditionalWeakTable<ITextSnapshot, XamlDocument>();

        public static event EventHandler<ParsingEventArgs> Parsed;

        public static IEnumerable<XamlError> Validate(this XamlDocument doc, string file)
        {
            var exception = doc.GetAttachedException();
            if (exception != null)
            {
                yield return new XamlError
                {
                    File = file,
                    Message = "Unexpected error occurred while parsing XAML. Please log an issue to https://github.com/Microsoft/Rapid-XAML-Toolkit/issues Reason: " + exception,
                    Line = 0,
                    Column = 0,
                    ErrorCode = "RXT0000",
                    Fatal = true,
                    Span = new Span(doc.Span.Start, doc.Span.Length),
                };
            }
        }

        public static Exception GetAttachedException(this XamlDocument xamlDocument)
        {
            if (xamlDocument.ContainsData("RowDef"))
            {
                return xamlDocument.GetData("RowDef") as Exception;
            }

            return xamlDocument.GetData(AttachedExceptionKey) as Exception;
        }

        public static XamlDocument ParseToXaml(this ITextSnapshot snapshot, string file = null)
        {
            lock (_syncRoot)
            {
                return CachedDocuments.GetValue(snapshot, key =>
                {
                    var text = key.GetText();
                    var xamlDocument = ParseToXaml(text);
                    Parsed?.Invoke(snapshot, new ParsingEventArgs(xamlDocument, file, snapshot));
                    return xamlDocument;
                });
            }
        }

        public static XamlDocument ParseToXaml(string text)
        {
            XamlDocument xamlDocument;

            try
            {
                //   xamlDocument = RapidXamlDocument.Create(text);
                xamlDocument = new XamlDocument();
            }
            catch (Exception ex)
            {
                xamlDocument = new XamlDocument();

                xamlDocument.Span = new Span(0, text.Length - 1);

                // we attach the exception to the document that will be later displayed to the user
                xamlDocument.SetData(AttachedExceptionKey, ex);
            }

            return xamlDocument;
        }
    }
}
