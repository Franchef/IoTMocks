using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using IoT;

namespace Infrastructure
{
    public class DeviceServiceImplementation : IoT.DeviceService.DeviceServiceBase
    {
        public override Task<DeviceInfo> GetDeviceInformation(Empty request, ServerCallContext context)
        {
            var result = new DeviceInfo();

            result.Id = $"{Guid.Empty}";
            result.HealthCheck = DeviceHealthCheck.None;
            result.Name = "IoT Mock Device";

            var digitalOutput = new DeviceInfo.Types.DeviceOutput { Index = 0, Name = "Digital output", Digital = new OutputTypes.Types.Digital { } };
            digitalOutput.Modes.Add(DeviceInfo.Types.OutputMode.Read);

            result.Outputs.Add(digitalOutput);

            var analogOutput = new DeviceInfo.Types.DeviceOutput { Index = 0, Name = "Digital output", Analog = new OutputTypes.Types.Analog { MinValue = 0, MaxValue = 1023 } };
            digitalOutput.Modes.Add(DeviceInfo.Types.OutputMode.Read);
            digitalOutput.Modes.Add(DeviceInfo.Types.OutputMode.ReadStream);

            result.Outputs.Add(analogOutput);

            return Task.FromResult(result);
        }
    }
}
