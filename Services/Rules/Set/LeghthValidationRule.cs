using Services.ValidationResults;

namespace Services.Rules.Set
{
    public class LeghthValidationRule : ISchemaValidator
    {
        public string Name { get; set; } = "length";

        public ValidationResult Validate(
            string fieldName,
            string value,
            IDictionary<string, object> ruleParams
        )
        {
            if (ruleParams.TryGetValue(Name, out var lengthObj))
            {
                if (!int.TryParse(lengthObj.ToString(), out int maxLength))
                {
                    throw new ArgumentOutOfRangeException(
                        $"Wrong rule parameter for {Name} rule. Param: {lengthObj}"
                    );
                }

                if (value.Length > maxLength)
                {
                    return new(
                        fieldName,
                        false,
                        $"Field {fieldName} exceeds maximum length of {maxLength}."
                    );
                }
            }
            return new(fieldName);
        }
    }
}
