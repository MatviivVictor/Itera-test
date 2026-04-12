using Claims.Application.Interfaces;
using Claims.Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Claims.API.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly ILogger<CoversController> _logger;
    private readonly ICoversService _coversService;
    private readonly IValidator<Cover> _validator;

    /// <summary>
    /// This controller provides endpoints for managing Covers and calculating premiums.
    /// </summary>
    public CoversController(ILogger<CoversController> logger, ICoversService coversService, IValidator<Cover> validator)
    {
        _logger = logger;
        _coversService = coversService;
        _validator = validator;
    }

    /// <summary>
    /// Computes the premium for a specified cover based on the provided start date, end date, and cover type.
    /// </summary>
    /// <param name="startDate">The start date of the cover period.</param>
    /// <param name="endDate">The end date of the cover period.</param>
    /// <param name="coverType">The type of cover for which the premium is being calculated.</param>
    /// <returns>An <see cref="ActionResult"/> containing the calculated premium.</returns>
    [HttpPost("compute")]
    public async Task<ActionResult> ComputePremiumAsync(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        return Ok(_coversService.ComputePremium(startDate, endDate, coverType));
    }

    /// <summary>
    /// Retrieves a list of all available covers asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="ActionResult"/> with the list of covers.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cover>>> GetAsync(CancellationToken cancellationToken)
    {
        var covers = await _coversService.GetCoversAsync(cancellationToken);
        return Ok(covers);
    }

    /// <summary>
    /// Retrieves a specific cover by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the cover to retrieve.</param>
    /// <param name="cancellationToken">Token to propagate notification that the operation should be canceled.</param>
    /// <returns>An <see cref="ActionResult"/> containing the requested cover information or a not found result if the cover does not exist.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Cover>> GetAsync([FromRoute] string id, CancellationToken cancellationToken)
    {
        var cover = await _coversService.GetCoverAsync(id, cancellationToken);

        if (cover is not null)
        {
            return Ok(cover);
        }

        return NotFound();
    }

    /// <summary>
    /// Creates a new cover entity based on the provided data.
    /// </summary>
    /// <param name="cover">The cover entity containing the details to be created.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>An <see cref="ActionResult{T}"/> containing the created cover entity.</returns>
    [HttpPost]
    public async Task<ActionResult<Cover>> CreateAsync([FromBody]Cover cover, CancellationToken cancellationToken)
    {
        cover = await _coversService.CreateCoverAsync(cover, cancellationToken);
        return Ok(cover);
    }

    /// <summary>
    /// Deletes an existing cover with the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the cover to be deleted.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="ActionResult"/> indicating the result of the delete operation.</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync([FromRoute]string id, CancellationToken cancellationToken)
    {
        await _coversService.DeleteCoverAsync(id, cancellationToken);
        return Ok();
    }
}