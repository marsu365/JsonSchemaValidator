# JsonSchemaValidator

A lightweight .NET application for validating JSON files against custom schema rules. The validator supports custom validation rules including length validation and mandatory field checks.

## Features

- Custom JSON schema validation
- Streaming large JSON file support
- Progress tracking during validation
- Extensible validation rule system
- Detailed validation output in JSON format
- Console interface for easy interaction

## Getting Started

### Prerequisites

- .NET 9.0 or higher
- A JSON file to validate
- A schema definition file

### Schema File Format

The schema file should be a JSON file containing validation rules for each field. Example:

``` json
{
	"firstName": {
		"mandatory": true,
		"length": 50
	},
	"lastName": {
		"mandatory": true,
		"length": 50
	},
	"email": {
		"mandatory": true,
		"length": 50
	}
}
```

### Supported Validation Rules

1. **Length Rule** (`length`)
   - Validates that a field's value doesn't exceed the specified maximum length
   - Parameter: Maximum allowed length as integer

2. **Mandatory Rule** (`mandatory`)
   - Ensures that a field is present and not empty
   - Parameter: Boolean value (true/false)

### Usage

1. Run the application
2. Enter the path to your schema file when prompted
3. Enter the path to the JSON file you want to validate
4. The application will process the file and generate validation results

### Output

Validation results are written to `output.json` in the following format:

``` json
[
	{
		"Field": "fieldName",
		"IsValid": true/false,
		"ErrorMessage": "Error description if validation failed"
	}
]
```

## Project Structure

- `Program.cs` - Console application entry point
- `JsonSchemaValidator.cs` - Core validation logic
- `ValidationOutputWriter.cs` - Handles validation result output
- `Rules/` - Contains validation rule implementations
  - `ISchemaValidator.cs` - Interface for validation rules
  - `Set/LeghthValidationRule.cs` - Length validation implementation
  - `Set/MandatoryValidationRule.cs` - Mandatory field validation implementation

## Error Handling

The application handles various types of errors:
- Invalid JSON format
- Missing schema fields
- Invalid rule parameters
- File I/O errors

## Contributing

To add new validation rules:

1. Create a new class implementing `ISchemaValidator`
2. Add the rule name to `RuleType` enum
3. Implement the validation logic
4. The rule will be automatically included in validation

## License

MIT

## Authors

marsu365