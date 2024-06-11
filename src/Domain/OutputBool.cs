namespace Domain
{
    public record OutputBool : Output
    {
        public bool Value { get; init; }
    }
}
