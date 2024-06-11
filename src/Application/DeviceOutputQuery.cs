using Domain;

using MediatR;

namespace Application
{
    public record DeviceOutputQuery : IRequest<Result<Output>>
    {
        public DeviceOutputQuery(ushort index)
        {
            Index = index;
        }

        public ushort Index { get; }
    }
}
