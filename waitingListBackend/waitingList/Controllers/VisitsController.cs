using Microsoft.AspNetCore.Mvc;
using waitingList.DTOs;
using waitingList.Services;

namespace waitingList.Controllers
{
    [ApiController]
    [Route("api/visits")]
    public sealed class VisitsController(IVisitService visitService) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType<IReadOnlyList<VisitDto>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<VisitDto>>> getVisits(
            CancellationToken cancellationToken)
        {
            var visits = await visitService.getVisitsAsync(cancellationToken);
            return Ok(visits);
        }

        [HttpPost]
        [ProducesResponseType<VisitDto>(StatusCodes.Status201Created)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<VisitDto>> createVisit(
            CreateVisitRequestDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                var visit = await visitService.createVisitAsync(request, cancellationToken);
                return StatusCode(StatusCodes.Status201Created, visit);
            }
            catch (KeyNotFoundException exception)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid centre",
                    Detail = exception.Message,
                    Status = StatusCodes.Status400BadRequest
                });
            }
            catch (InvalidOperationException exception)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Duplicate visit",
                    Detail = exception.Message,
                    Status = StatusCodes.Status409Conflict
                });
            }
        }
    }
}
