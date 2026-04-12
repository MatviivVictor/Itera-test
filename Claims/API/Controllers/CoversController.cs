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

    public CoversController(ILogger<CoversController> logger, ICoversService coversService, IValidator<Cover> validator)
    {
        _logger = logger;
        _coversService = coversService;
        _validator = validator;
    }

    [HttpPost("compute")]
    public async Task<ActionResult> ComputePremiumAsync(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        return Ok(_coversService.ComputePremium(startDate, endDate, coverType));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cover>>> GetAsync(CancellationToken cancellationToken)
    {
        var covers = await _coversService.GetCoversAsync(cancellationToken);
        return Ok(covers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Cover>> GetAsync(string id, CancellationToken cancellationToken)
    {
        var cover = await _coversService.GetCoverAsync(id, cancellationToken);

        if (cover is not null)
        {
            return Ok(cover);
        }

        return NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<Cover>> CreateAsync(Cover cover, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(cover, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        cover = await _coversService.CreateCoverAsync(cover, cancellationToken);
        return Ok(cover);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        await _coversService.DeleteCoverAsync(id, cancellationToken);
        return Ok();
    }
}