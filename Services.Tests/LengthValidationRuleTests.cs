using Services.Rules.Set;

public class LengthValidationRuleTests
{
    private readonly LeghthValidationRule _rule;

    public LengthValidationRuleTests()
    {
        _rule = new LeghthValidationRule();
    }

    [Theory]
    [InlineData("test", 5, true)]
    [InlineData("test", 4, true)]
    [InlineData("test", 3, false)]
    [InlineData("", 1, true)]
    public void Validate_WithVariousLengths_ReturnsExpectedResult(
        string value,
        int maxLength,
        bool expectedIsValid
    )
    {
        // Arrange
        var ruleParams = new Dictionary<string, object> { ["length"] = maxLength };

        // Act
        var result = _rule.Validate("testField", value, ruleParams);

        // Assert
        Assert.Equal(expectedIsValid, result.IsValid);
    }

    [Fact]
    public void Validate_WithInvalidLengthParameter_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var ruleParams = new Dictionary<string, object> { ["length"] = "not a number" };

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => _rule.Validate("testField", "test", ruleParams)
        );
    }
}
