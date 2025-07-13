using EShop.Application.Features.basketing.Commnads.AddItemToBasket;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BasketController : ControllerBase
{
    private readonly IMediator _mediator;

    public BasketController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddToBasket(AddItemToBasketCommnad command)
    {
        var result = await _mediator.Send(command);
        if (result.IsSuccess)
            return Ok(result.Value);

        return BadRequest(result.Errors.Select(x => x.Message));
    }
}
