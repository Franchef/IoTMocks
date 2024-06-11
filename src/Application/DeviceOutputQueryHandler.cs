using Domain;

using MediatR;

namespace Application
{
    public class DeviceOutputQueryHandler : IRequestHandler<DeviceOutputQuery, Result<Output>>
    {
        private readonly Device _device;

        public DeviceOutputQueryHandler(Device device)
        {
            _device = device;
        }
        public Task<Result<Output>> Handle(DeviceOutputQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_device.GetOutput(request.Index));
        }
    }
}
