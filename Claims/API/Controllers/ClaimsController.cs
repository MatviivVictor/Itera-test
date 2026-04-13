using Claims.Application.Interfaces;
using Claims.Application.Models;
using Claims.Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Claims.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly ILogger<ClaimsController> _logger;
        private readonly IClaimsService _claimsService;

        public ClaimsController(ILogger<ClaimsController> logger, IClaimsService claimsService)
        {
            _logger = logger;
            _claimsService = claimsService;
        }

        /// <summary>
        /// Retrieves a list of all claims.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains an action result wrapping an enumerable of Claim objects.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Claim>>> GetAsync(CancellationToken cancellationToken)
        {
            var claims = await _claimsService.GetClaimsAsync(cancellationToken);
            return Ok(claims);
        }

        /// <summary>
        /// Creates a new claim.
        /// </summary>
        /// <param name="claim">The claim object to be created.</param>
        /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains an action result wrapping the created Claim object.</returns>
        [HttpPost]
        public async Task<ActionResult<Claim>> CreateAsync([FromBody] CreateClaimRequestModel model, CancellationToken cancellationToken)
        {
            var claim = await _claimsService.CreateClaimAsync(model, "POST", cancellationToken);
            return Ok(claim);
        }

        /// <summary>
        /// Deletes a claim specified by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the claim to be deleted.</param>
        /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains an action result indicating success or failure of the deletion.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string id, CancellationToken cancellationToken)
        {
            await _claimsService.RemoveClaimAsync(id, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Retrieves a specific claim by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the claim to retrieve.</param>
        /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains an action result wrapping the Claim object if found, or a NotFound result otherwise.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync([FromRoute] string id, CancellationToken cancellationToken)
        {
            var claim = await _claimsService.GetClaimAsync(id, cancellationToken);
            if (claim is not null)
            {
                return Ok(claim);
            }

            return NotFound();
        }
    }
}