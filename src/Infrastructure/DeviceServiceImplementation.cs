using Application;

using Domain;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using IoT;

using MediatR;

namespace Infrastructure
{
    public class DeviceServiceImplementation : IoT.DeviceService.DeviceServiceBase
    {
        private readonly IMediator _mediator;

        public DeviceServiceImplementation(IMediator mediator)
        {
            _mediator = mediator;
        }
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

        public override async Task<OutputValue> ReadOutput(OutputReadRequest request, ServerCallContext context)
        {
            OutputValue result = null!;
            var ouput = await  _mediator.Send(new DeviceOutputQuery((ushort)request.Index));
            if (ouput.Success)
            {
                switch (ouput.Value)
                {
                    case OutputBool outputBool:
                        result = new OutputValue { Digital = new OutputValue.Types.DigitalValue { Value = outputBool.Value } };
                        break;
                    case OutputInt outputInt:
                        result = new OutputValue { Analog = new OutputValue.Types.AnalogValue { Value = outputInt.Value } };
                        break;
                    default:
                        throw new RpcException(status: new Status(StatusCode.Internal, $"Unhandled {ouput.GetType().FullName}"));
                        break;
                }
            }
            else
            {
                if(ouput.Exception is ArgumentOutOfRangeException argumentException)
                    throw new RpcException(status: new Status(StatusCode.OutOfRange, argumentException.Message));
                else
                    throw new RpcException(status: new Status(StatusCode.Internal, ouput.Exception.Message));
            }
                
            return result;
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
