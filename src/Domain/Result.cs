namespace Domain
{
    public record Result
    {
        public bool Success => Exception is null;
        public Exception? Exception { get; init; }
    }

    public record Result<T> : Result
    {
        public required T Value { get; init; }
    }
}
