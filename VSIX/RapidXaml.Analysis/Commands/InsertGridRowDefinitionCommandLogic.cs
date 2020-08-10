// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using RapidXamlToolkit.Logging;
using RapidXamlToolkit.VisualStudioIntegration;

namespace RapidXamlToolkit.Commands
{
    public class InsertGridRowDefinitionCommandLogic
    {
        private const string RowDefOpening = "<RowDefinition";

        private readonly ILogger logger;
        private readonly IVisualStudioAbstraction vs;

        public InsertGridRowDefinitionCommandLogic(ILogger logger, IVisualStudioAbstraction visualStudioAbstraction)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.vs = visualStudioAbstraction ?? throw new ArgumentNullException(nameof(visualStudioAbstraction));
        }

        public bool ShouldEnableCommand()
        {
            var activeDocText = this.vs.GetActiveDocumentText();

            if (activeDocText.IsValidXml())
            {
                var (cursorIndex, lineNo) = this.vs.GetCursorPositionAndLineNumber();

                // Add lineNo to account for VS adding extra char to line feed & -1 for current line
                // Add XAML Lenght to allow for cursor being in this text
                var searchPoint = cursorIndex + lineNo - 1 + RowDefOpening.Length;

                var docOfInterest = activeDocText.Substring(0, searchPoint);

                var closingDefsPosition = searchPoint + activeDocText.Substring(searchPoint).IndexOf("</Grid.RowDefinitions>");
                var closingGridPosition = searchPoint + activeDocText.Substring(searchPoint).IndexOf("</Grid>");
                var lastDefPosition = docOfInterest.LastIndexOf(RowDefOpening);

                return closingDefsPosition > -1 && lastDefPosition > -1 && closingGridPosition > -1 && lastDefPosition < searchPoint && searchPoint < closingDefsPosition && closingDefsPosition < closingGridPosition;
            }
            else
            {
                return false;
            }
        }

        public int GetRowNumber()
        {
            var activeDocText = this.vs.GetActiveDocumentText();

            if (activeDocText.IsValidXml())
            {
                var (cursorIndex, lineNo) = this.vs.GetCursorPositionAndLineNumber();

                var docOfInterest = activeDocText.Substring(0, cursorIndex + lineNo - 1 + RowDefOpening.Length); // Add lineNo to account for VS adding extra char to line feed & -1 for current line

                var closingDefPosition = docOfInterest.LastIndexOf("</Grid.RowDefinitions>");

                if (closingDefPosition > 0)
                {
                    docOfInterest = docOfInterest.Substring(closingDefPosition);
                }

                var occurrences = docOfInterest.OccurrenceCount(RowDefOpening);

                return occurrences - 1; // Remove one to account for rows being zero indexed
            }
            else
            {
                return -1;
            }
        }

        public int GetTotalRowDefinitions()
        {
            var activeDocText = this.vs.GetActiveDocumentText();
            var (cursorIndex, lineNo) = this.vs.GetCursorPositionAndLineNumber();

            cursorIndex += lineNo - 1; // Add lineNo to account for VS adding extra char to line feed & -1 for current line

            var closingDefPosition = cursorIndex + activeDocText.Substring(cursorIndex).IndexOf("</Grid.RowDefinitions>");
            var openingDefPosition = activeDocText.Substring(0, cursorIndex).LastIndexOf("<Grid.RowDefinitions>");

            var docOfInterest = activeDocText.Substring(openingDefPosition, closingDefPosition - openingDefPosition);

            var occurrences = docOfInterest.OccurrenceCount(RowDefOpening);

            return occurrences;
        }

        public (int start, int end, Dictionary<int, int> exclusions) GetGridBoundary()
        {
            var activeDocText = this.vs.GetActiveDocumentText();

            if (activeDocText.IsValidXml())
            {
                var (cursorIndex, lineNo) = this.vs.GetCursorPositionAndLineNumber();

                cursorIndex += lineNo - 1; // Add lineNo to account for VS adding extra char to line feed & -1 for current line

                const string gridOpenSpace = "<Grid ";
                const string gridOpenComplete = "<Grid>";
                const string gridClose = "</Grid>";

                var openingGridPos = activeDocText.Substring(0, cursorIndex + lineNo - 1).LastIndexOf(gridOpenComplete, gridOpenSpace);

                var closingGridPos = openingGridPos + activeDocText.Substring(openingGridPos).IndexOf(gridClose);

                var exclusions = new Dictionary<int, int>();

                var nextOpening = activeDocText.Substring(cursorIndex).AsSpan().FirstIndexOf(gridOpenComplete, gridOpenSpace);

                if (nextOpening > -1)
                {
                    nextOpening += cursorIndex;
                }

                while (nextOpening > -1 && nextOpening < closingGridPos)
                {
                    exclusions.Add(nextOpening, closingGridPos);

                    var searchFrom = closingGridPos + 1;

                    closingGridPos = searchFrom + activeDocText.Substring(searchFrom).IndexOf(gridClose);

                    searchFrom = nextOpening + 1;

                    nextOpening = activeDocText.Substring(searchFrom).AsSpan().FirstIndexOf(gridOpenComplete, gridOpenSpace);

                    if (nextOpening > -1)
                    {
                        nextOpening += searchFrom;
                    }
                }

                return (openingGridPos, closingGridPos, exclusions);
            }
            else
            {
                return (-1, -1, new Dictionary<int, int>());
            }
        }

        public List<(string find, string replace)> GetReplacements()
        {
            var rowNumber = this.GetRowNumber();

            if (rowNumber > -1)
            {
                var rowDefs = this.GetTotalRowDefinitions();

                var result = new List<(string, string)>();

                // subtract 1 from total rows to allow for zero indexing
                for (int i = rowDefs - 1; i >= rowNumber; i--)
                {
                    result.Add(($" Grid.Row=\"{i}\"", $" Grid.Row=\"{i + 1}\""));
                }

                return result;
            }
            else
            {
                return null;
            }
        }

        public (string definition, int insertPos) GetDefinitionAtCursor()
        {
            var activeDocText = this.vs.GetActiveDocumentText();

            if (activeDocText.IsValidXml())
            {
                var (cursorIndex, lineNo) = this.vs.GetCursorPositionAndLineNumber();

                cursorIndex += lineNo - 1; // Account for extra char in line feed in VS display

                var openingPos = activeDocText.Substring(0, cursorIndex + RowDefOpening.Length)
                    .LastIndexOf(RowDefOpening); // Move on row def length to allow for cursor being anywhere in it
                var length = activeDocText.Substring(openingPos).IndexOf("/>") + 2; // Add 2 for length of search string

                return (activeDocText.Substring(openingPos, length), openingPos);
            }
            else
            {
                return (string.Empty, -1);
            }
        }
    }
}
