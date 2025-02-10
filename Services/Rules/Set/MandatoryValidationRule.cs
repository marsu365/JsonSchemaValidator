using Services.ValidationResults;

namespace Services.Rules.Set
{
    public class MandatoryValidationRule : ISchemaValidator
    {
        public string Name { get; set; } = "mandatory";

        public ValidationResult Validate(
            string fieldName,
            string value,
            IDictionary<string, object> ruleParams
        )
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return new(
                    fieldName,
                    false,
                    $"Field {fieldName} is mandatory and cannot be empty."
                );
            }
            return new(fieldName);
        }
    }
}
