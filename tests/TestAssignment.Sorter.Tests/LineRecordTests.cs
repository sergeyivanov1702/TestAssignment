namespace TestAssignment.Sorter.Tests;

public sealed class LineRecordTests
{
    [Theory]
    [InlineData("1. Apple", 1, "Apple")]
    [InlineData("100. Banana", 100, "Banana")]
    [InlineData("42. The quick brown fox", 42, "The quick brown fox")]
    public void Parse_WhenCalled_ShouldParseValidLine(string line, int expectedNumber, string expectedText)
    {
        // Arrange && Act
        var record = LineRecord.Parse(line);

        // Assert
        Assert.Equal(expectedNumber, record.Number);
        Assert.Equal(line, record.OriginalLine);
        Assert.Equal(0, record.CompareTo(LineRecord.Parse($"{record.Number}. {expectedText}")));
    }

    [Fact]
    public void Parse_WithInvalidLine_ShouldThrow()
    {
        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => LineRecord.Parse("DoesNotContainSeparator"));
        Assert.Throws<FormatException>(() => LineRecord.Parse("NotANumber. Text"));
    }

    [Fact]
    public void CompareTo_ShouldSortByTextThenByNumber()
    {
        // Arrange && Act
        var record1 = LineRecord.Parse("415. Apple");
        var record2 = LineRecord.Parse("3. Apple");
        var record3 = LineRecord.Parse("4. Banana");
        var record4 = LineRecord.Parse("1. Cherry");

        // Assert
        Assert.True(record2.CompareTo(record1) < 0); // 3. Apple < 415. Apple
        Assert.True(record1.CompareTo(record2) > 0); // 415. Apple > 3. Apple
        Assert.True(record1.CompareTo(record3) < 0); // Apple < Banana
        Assert.True(record3.CompareTo(record4) < 0); // Banana < Cherry

        var list = new List<LineRecord> { record1, record2, record3, record4 };
        list.Sort();

        Assert.Equal(new[] { record2, record1, record3, record4 }, list);
    }
}
