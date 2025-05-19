namespace RapidXaml.CustomAnalysis.Tests;

[TestClass]
public sealed class AnalysisActionsTests
{
    [TestMethod]
    public void AddOneItemToEmptyList()
    {
        var sut = AnalysisActions.EmptyList;

        sut.Add(AnalysisActions.HighlightDescendantWithoutAction(
                RapidXamlErrorType.Warning,
                code: "RXCAT001",
                description: "Don't do THAT!.",
                new RapidXamlElement()));

        Assert.IsFalse(sut.IsNone);
        Assert.AreEqual(1, sut.Actions.Count);
    }

    [TestMethod]
    public void IncorrectWayOfAddingToAnEmptyList_CreatesDoubleEntries()
    {
        var sut = AnalysisActions.EmptyList;

        // This is adding to the lost and then adding that to the list.
        // Don't do this!
        // This test exists to highlight how doing a bad thing can cause double entries.
        sut.Add(sut.HighlightDescendantWithoutAction(
                RapidXamlErrorType.Warning,
                code: "RXCAT002BAD",
                description: "Don't do THAT!.",
                new RapidXamlElement()));

        Assert.IsFalse(sut.IsNone);
        Assert.AreEqual(2, sut.Actions.Count);
    }
}
