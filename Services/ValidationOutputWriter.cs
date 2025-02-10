using System.Text.Json;
using Services.ValidationResults;

namespace Services
{
    public class ValidationOutputWriter : IDisposable
    {
        public readonly string OutputFileName;

        private readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
        };
        private readonly StreamWriter writer;
        private bool outputInitialized = false;

        public ValidationOutputWriter(string outputFileName)
        {
            OutputFileName = outputFileName;
            writer = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, OutputFileName))
            {
                AutoFlush = true,
            };
            writer.WriteLine("[");
        }

        public void WriteValidationOutput(ValidationResult validationResult)
        {
            var json = JsonSerializer.Serialize(validationResult, jsonSerializerOptions);

            if (outputInitialized)
            {
                writer.WriteLine(",");
            }

            writer.Write(json);

            outputInitialized = true;
        }

        public void Dispose()
        {
            writer.WriteLine();
            writer.Write("]");
            writer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
