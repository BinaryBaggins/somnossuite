using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SomnosSuite.Application.StunningDevices.CreateStunningDevice;
using SomnosSuite.Application.StunningDevices.GetStunningDeviceDetails;
using SomnosSuite.Application.StunningDevices;

namespace SomnosSuite.Presentation.StunningDevices;

[ApiController]
[Route("api/stunning-devices")]
public sealed class StunningDevicesController(
    ISender sender) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var query = new GetAllStunningDevicesQuery();

        var devices = await sender.Send(
            query,
            cancellationToken);

        return Ok(devices);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetStunningDeviceDetailsQuery(id);

        var result = await sender.Send(
            query,
            cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(new
            {
                code = result.Error.Code,
                message = result.Error.Message
            });
        }

        return Ok(result.Value);
    }


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