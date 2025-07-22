using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ProdManagement.Presentation.Models.Products;
using ProdManagement.Application.Features.Products.Queries;
using ProdManagement.Application.Features.Products.Commands.CreateProduct;
using ProdManagement.Application.Features.Products.Commands.DeleteProduct;
using ProdManagement.Application.Features.Products.Commands.UpdateProduct;

namespace ProdManagement.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    #region Constructor & DI

    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #endregion Constructor & DI - End

    #region GetAll

    [HttpGet("GetAll")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] string? createdBy)
    {
        var query = new GetAllProductsQuery(createdBy ?? "");
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    #endregion GetAll - End

    #region Create

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        // Set Username value by JWT
        var createdBy = User.Claims
            .FirstOrDefault(c => c.Type == "Username")?.Value
            ?? throw new UnauthorizedAccessException("User not authenticated");

        // Set Email value by JWT
        var manufactureEmail = User.Claims
            .FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")
            ?.Value ?? throw new UnauthorizedAccessException("Email claim missing");

        // Set PhoneNumber value by JWT
        var manufacturePhone = User.Claims
            .FirstOrDefault(c => c.Type == "PhoneNumber")
            ?.Value ?? throw new UnauthorizedAccessException("PhoneNumber claim missing");


        var commandWithUser = command with
        {
            CreatedBy = createdBy,
            ManufactureEmail = manufactureEmail,
            ManufacturePhone = manufacturePhone,
        };

        var result = await _mediator.Send(commandWithUser);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    #endregion Create -End

    #region Delete

    [HttpDelete("Delete")]
    public async Task<IActionResult> DeleteByName([FromQuery] DeleteProductRequest request)
    {
        var username = User.Claims
            .FirstOrDefault(c => c.Type == "Username")
            ?.Value;

        if (string.IsNullOrWhiteSpace(username))
            return Unauthorized("User is not authenticated.");

        var command = new DeleteProductCommand(username, request.ProductName);

        var result = await _mediator.Send(command);

        if (!result)
            return NotFound($"Product with name '{request.ProductName}' not found or access denied.");

        return NoContent();
    }

    #endregion Delete - End

    #region Update

    [HttpPut("Update")]
    public async Task<IActionResult> Update([FromQuery] UpdateProductRequest request)
    {
        var username = User.Claims
            .FirstOrDefault(c => c.Type == "Username")?.Value
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var command = new UpdateProductCommand(
            request.Id,
            request.Name,
            request.IsAvailable,
            username);

        var result = await _mediator.Send(command);

        if (!result)
            return Forbid("You are not allowed to update this product or product not found.");

        return NoContent();
    }

    #endregion Update - End

    #region HiddenFromSwagger - GetById

    [HttpGet("{id}")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetById(Guid id)
    {
        return Ok();
    }

    #endregion HiddenFromSwagger - GetById - End
}
