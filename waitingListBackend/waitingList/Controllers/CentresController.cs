using Microsoft.AspNetCore.Mvc;
using waitingList.DTOs;
using waitingList.Services;

namespace waitingList.Controllers
{
    [ApiController]
    [Route("api/centres")]
    public sealed class CentresController(ICentreService centreService) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType<IReadOnlyList<CentreDto>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<CentreDto>>> getCentres(
            CancellationToken cancellationToken)
        {
            var centres = await centreService.getActiveCentresAsync(cancellationToken);
            return Ok(centres);
        }
    }
}
