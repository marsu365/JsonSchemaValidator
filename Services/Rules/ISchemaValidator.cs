using Services.ValidationResults;

namespace Services.Rules
{
    public interface ISchemaValidator
    {
        public string Name { get; set; }

        ValidationResult Validate(
            string fieldName,
            string value,
            IDictionary<string, object> ruleParams
        );
    }
}
