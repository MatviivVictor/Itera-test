using Claims.Application.Interfaces;
using Claims.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Claims.API.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly ILogger<CoversController> _logger;
    private readonly ICoversService _coversService;

    public CoversController(ILogger<CoversController> logger, ICoversService coversService)
    {
        _logger = logger;
        _coversService = coversService;
    }

    /// <summary>
    /// Retrieves the list of available covers asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="ActionResult"/> with the list of covers.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CoverModel>>> GetAsync(CancellationToken cancellationToken)
    {
        var covers = await _coversService.GetCoversAsync(cancellationToken);
        return Ok(covers);
    }

    /// <summary>
    /// Retrieves a specific cover by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the cover to retrieve.</param>
    /// <param name="cancellationToken">Token used to signal request cancellation.</param>
    /// <returns>An <see cref="ActionResult"/> containing the requested cover details, or a not found result if the cover is not available.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<CoverModel>> GetAsync([FromRoute] string id, CancellationToken cancellationToken)
    {
        var cover = await _coversService.GetCoverAsync(id, cancellationToken);

        if (cover is not null)
        {
            return Ok(cover);
        }

        return NotFound();
    }

    /// <summary>
    /// Creates a new cover entity based on the provided details.
    /// </summary>
    /// <param name="model">The request model containing the necessary details for creating a cover.</param>
    /// <param name="cancellationToken">A token to allow the operation to be canceled.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="ActionResult{T}"/> with the created cover entity.</returns>
    [HttpPost]
    public async Task<ActionResult<CoverModel>> CreateAsync([FromBody] CreateCoverRequestModel model,
        CancellationToken cancellationToken)
    {
        var cover = await _coversService.CreateCoverAsync(model, cancellationToken);
        return Ok(cover);
    }

    /// <summary>
    /// Deletes a cover with the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the cover to be deleted.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="ActionResult"/> indicating the result of the delete operation.</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync([FromRoute] string id, CancellationToken cancellationToken)
    {
        await _coversService.DeleteCoverAsync(id, cancellationToken);
        return Ok();
    }
}