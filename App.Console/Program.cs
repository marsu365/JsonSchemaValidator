using System.Text.Json;
using Services;
using Services.Extensions;

Console.WriteLine($"JsonSchemaValidator");

Console.WriteLine($"Please input path to json schema file:");
var schemaFile = Console.ReadLine();

while (!File.Exists(schemaFile))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Error.WriteLine($"Schema file not found: {schemaFile}. Provide correct path");
    Console.ResetColor();
    schemaFile = Console.ReadLine();
}

using var fs = File.Open(schemaFile, FileMode.Open);

Dictionary<string, Dictionary<string, object>>? schemaRuleSet = [];

try
{
    schemaRuleSet = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(fs);
    if (schemaRuleSet is null)
    {
        throw new ArgumentException("No validation rules found in schema file");
    }
}
catch (Exception ex)
{
    ExtendedConsole.ExitApplicationWithMessage(
        $"Wrong or corrupted schema file: {schemaFile}. Error: {ex.Message}"
    );
}

Console.WriteLine($"Please input path to json file:");
var jsonFile = Console.ReadLine();

while (!File.Exists(jsonFile))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Error.WriteLine($"Input file not found: {jsonFile}. Provide correct path");
    Console.ResetColor();
    jsonFile = Console.ReadLine();
}

string outputPath = string.Empty;

try
{
    var validator = new JsonSchemaValidator();
    outputPath = await validator.ValidateJson(jsonFile, schemaRuleSet!);
}
catch (Exception ex)
{
    ExtendedConsole.ExitApplicationWithMessage($"{ex.Message}");
}

Console.WriteLine($"Validation completed successfully!");
Console.WriteLine($"Results written to: {outputPath}");
