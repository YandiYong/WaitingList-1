using Microsoft.AspNetCore.Mvc;
using waitingList.DTOs;
using waitingList.Services;

namespace waitingList.Controllers
{
    [ApiController]
    [Route("api/check-ins")]
    public sealed class CheckInsController(ICheckInService checkInService) : ControllerBase
    {
        [HttpGet("{qrToken}")]
        [ProducesResponseType<CheckInResultDto>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CheckInResultDto>> getScanResult(
            string qrToken,
            CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await checkInService.getScanResultAsync(qrToken, cancellationToken));
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(createProblem(exception.Message, StatusCodes.Status404NotFound));
            }
            catch (InvalidOperationException exception)
            {
                return Conflict(createProblem(exception.Message, StatusCodes.Status409Conflict));
            }
        }

        [HttpPost]
        [ProducesResponseType<CheckInResultDto>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CheckInResultDto>> checkIn(
            CheckInRequestDto request,
            CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await checkInService.checkInAsync(request, cancellationToken));
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(createProblem(exception.Message, StatusCodes.Status404NotFound));
            }
            catch (InvalidOperationException exception)
            {
                return Conflict(createProblem(exception.Message, StatusCodes.Status409Conflict));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(createProblem(exception.Message, StatusCodes.Status400BadRequest));
            }
        }

        private static ProblemDetails createProblem(string message, int status) => new()
        {
            Title = "Check-in unsuccessful",
            Detail = message,
            Status = status
        };
    }
}
