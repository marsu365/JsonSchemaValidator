namespace Services.ValidationResults
{
    public record ValidationResult(
        string Field,
        bool IsValid = true,
        string? ErrorMessage = default
    )
    {
        public string Field { get; set; } = Field;
        public bool IsValid { get; set; } = IsValid;
        public string? ErrorMessage { get; set; } = ErrorMessage;
    }
}
