using Services.Rules.Set;

public class MandatoryValidationRuleTests
{
    private readonly MandatoryValidationRule _rule;

    public MandatoryValidationRuleTests()
    {
        _rule = new MandatoryValidationRule();
    }

    [Theory]
    [InlineData("test", true)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData(null, false)]
    public void Validate_WithVariousValues_ReturnsExpectedResult(string value, bool expectedIsValid)
    {
        // Arrange
        var ruleParams = new Dictionary<string, object>();

        // Act
        var result = _rule.Validate("testField", value, ruleParams);

        // Assert
        Assert.Equal(expectedIsValid, result.IsValid);
        if (!expectedIsValid)
        {
            Assert.Contains("mandatory", result.ErrorMessage);
        }
    }
}
