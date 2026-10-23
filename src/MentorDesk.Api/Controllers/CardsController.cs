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
    public async Task<ActionResult<CardResponse>> UpdateCardAsync([FromRoute] Guid id, [FromBody] UpdateCardRequest request, CancellationToken cancellationToken)
    {
        var card = await _cardService.UpdateCardAsync(id, request, cancellationToken);

        if (card is null)
        {
            return NotFound();
        }

        return Ok(card);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCardAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _cardService.DeleteCardAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
