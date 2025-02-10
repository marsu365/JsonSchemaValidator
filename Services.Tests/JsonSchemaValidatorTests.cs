using System.Text.Json;
using Services;

public class JsonSchemaValidatorTests
{
    private readonly JsonSchemaValidator _validator;
    private readonly string _testDataPath;

    public JsonSchemaValidatorTests()
    {
        _validator = new JsonSchemaValidator();
        _testDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData");
        Directory.CreateDirectory(_testDataPath);
    }

    [Theory]
    [InlineData("testfiles\\input.json", "testfiles\\schema.json")]
    public async Task JsonSchemaValidator_ValidateJson(string jsonFilePath, string schemaFilePath)
    {
        // Arrange
        using var fs = File.Open(schemaFilePath, FileMode.Open);
        var schemaRuleSet = JsonSerializer.Deserialize<
            Dictionary<string, Dictionary<string, object>>
        >(fs);

        // Act
        var outputFilePath = await _validator.ValidateJson(jsonFilePath, schemaRuleSet!);

        // Assert
        Assert.True(File.Exists(outputFilePath));
    }

    [Fact]
    public async Task ValidateJson_ValidInput_ReturnsSuccessfulValidation()
    {
        // Arrange
        var jsonContent =
            @"{
                ""firstName"": ""John"",
                ""lastName"": ""Doe"",
                ""email"": ""john@example.com""
            }";
        var schemaRules = new Dictionary<string, Dictionary<string, object>>
        {
            ["firstName"] = new() { ["mandatory"] = true, ["length"] = 50 },
            ["lastName"] = new() { ["mandatory"] = true, ["length"] = 50 },
            ["email"] = new() { ["mandatory"] = true },
        };

        var testFile = Path.Combine(_testDataPath, "valid.json");
        await File.WriteAllTextAsync(testFile, jsonContent);

        // Act
        var result = await _validator.ValidateJson(testFile, schemaRules);

        // Assert
        Assert.NotNull(result);
        Assert.True(File.Exists(result));
    }

    [Fact]
    public async Task ValidateJson_MissingMandatoryField_ThrowsJsonException()
    {
        // Arrange
        var jsonContent =
            @"{
                ""firstName"": ""John"",
                ""email"": ""john@example.com""
            }";
        var schemaRules = new Dictionary<string, Dictionary<string, object>>
        {
            ["firstName"] = new() { ["mandatory"] = true },
            ["lastName"] = new() { ["mandatory"] = true },
            ["email"] = new() { ["mandatory"] = true },
        };

        var testFile = Path.Combine(_testDataPath, "missing_mandatory.json");
        await File.WriteAllTextAsync(testFile, jsonContent);

        // Act & Assert
        await Assert.ThrowsAsync<JsonException>(
            () => _validator.ValidateJson(testFile, schemaRules)
        );
    }

    [Fact]
    public async Task ValidateJson_ExceedsMaxLength_ReturnsValidationError()
    {
        // Arrange
        var jsonContent =
            @"{
                ""firstName"": ""ThisIsAVeryLongNameThatExceedsTheMaximumLength"",
                ""lastName"": ""Doe"",
                ""email"": ""john@example.com""
            }";
        var schemaRules = new Dictionary<string, Dictionary<string, object>>
        {
            ["firstName"] = new() { ["length"] = 10 },
            ["lastName"] = new() { ["mandatory"] = true },
            ["email"] = new() { ["mandatory"] = true },
        };

        var testFile = Path.Combine(_testDataPath, "exceeds_length.json");
        await File.WriteAllTextAsync(testFile, jsonContent);

        // Act
        var result = await _validator.ValidateJson(testFile, schemaRules);

        // Assert
        Assert.NotNull(result);
        var outputContent = await File.ReadAllTextAsync(result);
        Assert.Contains("exceeds maximum length", outputContent);
    }

    [Fact]
    public async Task ValidateJson_InvalidJsonFormat_ThrowsJsonException()
    {
        // Arrange
        var jsonContent =
            @"{
                ""firstName"": ""John"",
                ""lastName"": ""Doe"",
                ""email"": ""john@example.com"" // missing closing brace";

        var schemaRules = new Dictionary<string, Dictionary<string, object>>
        {
            ["firstName"] = new() { ["mandatory"] = true },
        };

        var testFile = Path.Combine(_testDataPath, "invalid_format.json");
        await File.WriteAllTextAsync(testFile, jsonContent);

        // Act & Assert
        await Assert.ThrowsAsync<JsonException>(
            () => _validator.ValidateJson(testFile, schemaRules)
        );
    }

    [Fact]
    public async Task ValidateJson_EmptyFile_ThrowsJsonException()
    {
        // Arrange
        var testFile = Path.Combine(_testDataPath, "empty.json");
        await File.WriteAllTextAsync(testFile, "");

        var schemaRules = new Dictionary<string, Dictionary<string, object>>
        {
            ["firstName"] = new() { ["mandatory"] = true },
        };

        // Act & Assert
        await Assert.ThrowsAsync<JsonException>(
            () => _validator.ValidateJson(testFile, schemaRules)
        );
    }
}
