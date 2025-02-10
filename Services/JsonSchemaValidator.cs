using System.Buffers;
using System.Text;
using System.Text.Json;

namespace Services
{
    public class JsonSchemaValidator
    {
        /// <summary>
        /// Validates Json file against schema and providing full path to the validation output file
        /// </summary>
        /// <param name="jsonFilePath"></param>
        /// <param name="schemaRuleSet"></param>
        /// <returns>Full path to the validation output file</returns>
        /// <exception cref="JsonException"></exception>
        public async Task<string> ValidateJson(
            string jsonFilePath,
            Dictionary<string, Dictionary<string, object>> schemaRuleSet
        )
        {
            var ruleManager = new RuleManager();
            using var outputWriter = new ValidationOutputWriter("output.json");

            var jsonFileFileInfo = new FileInfo(jsonFilePath);
            long totalBytes = jsonFileFileInfo.Length;

            using var fileStream = new FileStream(jsonFilePath, FileMode.Open, FileAccess.Read);
            using var reader = new StreamReader(fileStream, Encoding.UTF8);

            try
            {
                var buffer = new char[10240];
                int bytesRead;
                long processedBytes = 0;

                var foundProperties = new HashSet<string>();

                while ((bytesRead = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    var byteSpan = Encoding.UTF8.GetBytes(buffer, 0, bytesRead);
                    processedBytes += bytesRead;

                    var sequence = new ReadOnlySequence<byte>(byteSpan);
                    var options = new JsonReaderOptions
                    {
                        CommentHandling = JsonCommentHandling.Skip,
                    };
                    var jsonReader = new Utf8JsonReader(sequence, options);

                    while (jsonReader.Read())
                    {
                        if (jsonReader.TokenType == JsonTokenType.PropertyName)
                        {
                            string? propertyName = jsonReader.GetString();

                            if (
                                !schemaRuleSet.TryGetValue(
                                    propertyName!,
                                    out Dictionary<string, object>? rules
                                )
                            )
                            {
                                throw new JsonException(
                                    $"Property {propertyName} not supported by the schema."
                                );
                            }

                            foundProperties.Add(propertyName!);

                            if (jsonReader.Read())
                            {
                                string? propertyValue = jsonReader.GetString();

                                foreach (var rule in ruleManager.Validators)
                                {
                                    var result = rule.Validate(
                                        propertyName!,
                                        propertyValue!,
                                        schemaRuleSet[propertyName!]
                                    );
                                    outputWriter.WriteValidationOutput(result);
                                }
                            }
                        }
                    }

                    double progress = (double)processedBytes / totalBytes * 100;
                    Console.Write($"\rProgress: {progress:F2}%. {processedBytes} bytes processed");
                }

                var missingFields = schemaRuleSet
                    .Select(x => x.Key)
                    .Where(x => !foundProperties.TryGetValue(x, out string? entry));

                if (missingFields.Any())
                {
                    throw new JsonException(
                        $"Missing fields found in json file. First 10 missing fields are: {string.Join("; ", missingFields)}"
                    );
                }

                Console.WriteLine();

                return Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    outputWriter.OutputFileName
                );
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"JSON Error: {jsonEx.Message}");
                throw;
            }
            catch (InvalidOperationException invalidOpEx)
            {
                Console.WriteLine($"Invalid Operation: {invalidOpEx.Message}");
                throw;
            }
            catch (FormatException formatEx)
            {
                Console.WriteLine($"Format Error: {formatEx.Message}");
                throw;
            }
            catch (OverflowException overflowEx)
            {
                Console.WriteLine($"Overflow Error: {overflowEx.Message}");
                throw;
            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"IO Error: {ioEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
                throw;
            }
        }
    }
}
