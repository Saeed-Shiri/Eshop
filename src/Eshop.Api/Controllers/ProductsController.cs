using EShop.Application.Features.ProductFetures.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromBody] GetProductsQuery request)
    {
        var result = await _mediator.Send(request);
        return Ok(result);
    }
}
