using MediatR;
using MentorDesk.Application.Features.Decks.Import;
using Microsoft.AspNetCore.Mvc;
using MentorDesk.Application.Service;

namespace MentorDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DecksController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IDeckService _deckService;

    public DecksController(IMediator mediator, IDeckService deckService)
    {
        _mediator = mediator;
        _deckService = deckService;
    }

    [HttpPost]
    [Route("{deckId}/import")]
    public async Task<ActionResult<ImportResult>> ImportDeck(Guid deckId, [FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        using var stream = file.OpenReadStream();

        var result = await _mediator.Send(new ImportDeckCommand(deckId, stream), cancellationToken);

        return Ok(result);
    }

    [HttpGet] // GetAllDecksAsync
    public async Task<IActionResult> GetDecksAsync()
    {
        var decks = await _deckService.GetAllDecksAsync();

        return Ok(decks);
    }

    [HttpPost] // CreateDeckAsync


    [HttpGet("{id}")] // GetByDeckIdAsync
    public async Task<IActionResult> GetDeck([FromBody] Guid id)
    {
        var deck = await _deckService.GetByIdAsync(id);

        if (deck is null)
        {
            return NotFound();
        }

        return Ok(deck);
    }


    [HttpPut("{id}")]
    // UpdateDeckAsync

    [HttpDelete("{id}")]
    // DeleteDeckAsync

    [HttpGet("{id}/cards")]
    // GetCardsAsync

    [HttpPost("{id}/cards")]
    // CreateCardAsync
}
