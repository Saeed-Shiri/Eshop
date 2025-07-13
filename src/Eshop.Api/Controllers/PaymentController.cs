
using EShop.Application.Features.PaymentFeatures.Commands.StartPayment;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartPayment([FromBody] StartPaymentCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok("درخواست پرداخت ارسال شد");

        return BadRequest(result.Errors.Select(x => x.Message));
    }
}
