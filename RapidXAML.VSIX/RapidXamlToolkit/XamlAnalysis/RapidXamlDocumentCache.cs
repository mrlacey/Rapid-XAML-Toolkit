// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis
{
    public static class RapidXamlDocumentCache
    {
        private static readonly Dictionary<string, RapidXamlDocument> Cache = new Dictionary<string, RapidXamlDocument>();

        public static event EventHandler<RapidXamlParsingEventArgs> Parsed;

        public static void Add(string file, ITextSnapshot snapshot)
        {
            if (Cache.ContainsKey(file))
            {
                Update(file, snapshot);
            }
            else
            {
                var doc = RapidXamlDocument.Create(snapshot);
                Cache.Add(file, doc);

                Parsed?.Invoke(null, new RapidXamlParsingEventArgs(doc, file, snapshot, ParsedAction.Add));
            }
        }

        public static void Update(string file, ITextSnapshot snapshot)
        {
            if (Cache[file].RawText != snapshot.GetText())
            {
                var doc = RapidXamlDocument.Create(snapshot);
                Cache[file] = doc;

                Parsed?.Invoke(null, new RapidXamlParsingEventArgs(doc, file, snapshot, ParsedAction.Update));
            }
        }

        public static List<IRapidXamlAdornmentTag> AdornmentTags(string fileName)
        {
            var result = new List<IRapidXamlAdornmentTag>();

            if (Cache.ContainsKey(fileName))
            {
                result.AddRange(Cache[fileName].Tags);
            }

            return result;
        }

        public static List<IRapidXamlErrorListTag> ErrorListTags(string fileName)
        {
            var result = new List<IRapidXamlErrorListTag>();

            if (Cache.ContainsKey(fileName))
            {
                result.AddRange(Cache[fileName].Tags.OfType<IRapidXamlErrorListTag>());
            }

            return result;
        }
    }
}
