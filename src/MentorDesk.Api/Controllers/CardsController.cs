using FluentValidation;
using MentorDesk.Application.DTOs.Card;
using MentorDesk.Application.Service;
using Microsoft.AspNetCore.Mvc;

namespace MentorDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]

public class CardsController : ControllerBase
{
    private readonly ICardService _cardService;

    public CardsController(ICardService cardService)
    {
        _cardService = cardService;
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CardResponse>> UpdateCardAsync([FromRoute] Guid id, [FromBody] UpdateCardRequest request, [FromServices] IValidator<UpdateCardRequest> validator, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }

        var result = await _cardService.UpdateCardAsync(id, request, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(!result.IsSuccess);
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCardAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _cardService.DeleteCardAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }

        return NoContent();
    }
}
