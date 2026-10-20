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

    [HttpPut("{id}")] // UpdateCardAsync

    [HttpDelete("{id}")] // DeleteCardAsync
}
