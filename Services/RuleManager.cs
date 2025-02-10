using Services.Rules;
using Services.Rules.Set;

namespace Services
{
    public class RuleManager
    {
        public readonly IEnumerable<ISchemaValidator> Validators =
        [
            new LeghthValidationRule(),
            new MandatoryValidationRule(),
        ];
    }
}
