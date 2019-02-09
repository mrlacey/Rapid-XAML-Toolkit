// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;

namespace RapidXamlToolkit.XamlAnalysis
{
    public static class RapidXamlDocumentCache
    {
        private static Dictionary<string, RapidXamlDocument> cache = new Dictionary<string, RapidXamlDocument>();

        public static event EventHandler<RapidXamlParsingEventArgs> Parsed;

        public static void Add(string file, ITextSnapshot snapshot)
        {
            if (cache.ContainsKey(file))
            {
                Update(file, snapshot);
            }
            else
            {
                var doc = RapidXamlDocument.Create(snapshot);
                cache.Add(file, doc);

                Parsed?.Invoke(null, new RapidXamlParsingEventArgs(doc, file, snapshot, ParsedAction.Add));
            }
        }

        public static void Update(string file, ITextSnapshot snapshot)
        {
            if (cache[file].RawText != snapshot.GetText())
            {
                var doc = RapidXamlDocument.Create(snapshot);
                cache[file] = doc;

                Parsed?.Invoke(null, new RapidXamlParsingEventArgs(doc, file, snapshot, ParsedAction.Update));
            }
        }

        public static List<IRapidXamlTag> Tags(string fileName)
        {
            var result = new List<IRapidXamlTag>();

            if (cache.ContainsKey(fileName))
            {
                result.AddRange(cache[fileName].SuggestionTags);
            }

            return result;
        }

        public static List<IRapidXamlViewTag> ViewTags(string fileName)
        {
            var result = new List<IRapidXamlViewTag>();

            if (cache.ContainsKey(fileName))
            {
                result.AddRange(cache[fileName].SuggestionTags.OfType<IRapidXamlViewTag>());
            }

            return result;
        }
    }
}
