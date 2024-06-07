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

            var analogOutput = new DeviceInfo.Types.DeviceOutput { Index = 1, Name = "Analog output", Analog = new OutputTypes.Types.Analog { MinValue = 0, MaxValue = 1023 } };
            digitalOutput.Modes.Add(DeviceInfo.Types.OutputMode.Read);
            digitalOutput.Modes.Add(DeviceInfo.Types.OutputMode.ReadStream);

            result.Outputs.Add(analogOutput);

            return Task.FromResult(result);
        }

        public override Task<OutputValue> ReadOutput(OutputReadRequest request, ServerCallContext context)
        {
            OutputValue result = null!;
            switch (request.Index)
            {
                case 0:
                    result = new OutputValue { Digital = new OutputValue.Types.DigitalValue { Value = true } };
                    break;
                case 1:
                    result = new OutputValue { Analog = new OutputValue.Types.AnalogValue { Value = new Random().Next(0, 1023) } };
                    break;
                default:
                    break;
            }
            if (result is null)
                throw new RpcException(status: new Status(StatusCode.OutOfRange, $"Output {request.Index} not available"));
            return Task.FromResult<OutputValue>(result);
        }

        public override async Task ReadOutputStream(OutputReadRequest request, IServerStreamWriter<OutputValue> responseStream, ServerCallContext context)
        {
            if (request.Index == 1)
            {
                foreach (var i in Enumerable.Range(0, new Random().Next(10, 100)))
                {
                    await Task.Delay(new Random().Next(1, 10) * 1000);
                    if (context.CancellationToken.IsCancellationRequested) break;
                    await responseStream.WriteAsync(new OutputValue { Analog = new OutputValue.Types.AnalogValue { Value = new Random().Next(0, 1023) } });
                }
            }
            else
            {
                throw new RpcException(status: new Status(StatusCode.OutOfRange, $"Output {request.Index} not available"));
            }
        }
    }
}
