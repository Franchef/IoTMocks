namespace Domain
{
    public class Device
    {
        public Result<Output> GetOutput(ushort index)
        {
            return index switch
            {
                0 => new Result<Output> { Value = new OutputBool { Value = true } },
                1 => new Result<Output> { Value = new OutputInt { Value = new Random().Next(0, 1023) } },
                _ => new Result<Output> { Exception = new ArgumentOutOfRangeException($"Index {index} not handled"), Value = null! }
            };
        }
    }
}
