using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SomnosSuite.Application.StunningDevices.CreateStunningDevice;

namespace SomnosSuite.Presentation.StunningDevices;

[ApiController]
[Route("api/stunning-devices")]
public sealed class StunningDevicesController(
    ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateStunningDeviceRequestDto requestDto,
        CancellationToken cancellationToken)
    {
        var command = new CreateStunningDeviceCommand(
            requestDto.DeviceType,
            requestDto.Manufacturer,
            requestDto.SerialNumber,
            requestDto.Model,
            requestDto.AnimalCategory,
            requestDto.LastInspectionDate);

        var result =
            await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new
            {
                code = result.Error.Code,
                message = result.Error.Message
            });
        }

        return StatusCode(
            StatusCodes.Status201Created,
            new { id = result.Value });
    }
}