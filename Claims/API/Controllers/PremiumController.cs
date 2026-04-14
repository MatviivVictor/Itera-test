using Claims.Application.Interfaces;
using Claims.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Claims.API.Controllers;

[ApiController]
[Route("[controller]")]
public class PremiumController : ControllerBase
{
    private readonly ILogger<CoversController> _logger;
    private readonly IPremiumService _premiumService;

    public PremiumController(ILogger<CoversController> logger, IPremiumService premiumService)
    {
        _logger = logger;
        _premiumService = premiumService;
    }

    /// <summary>
    /// Computes the premium based on the provided premium calculation request model.
    /// </summary>
    /// <param name="model">The request model containing the details required to compute the premium, such as start date, end date, and cover type.</param>
    /// <returns>The result contains the computed premium as a decimal value.</returns>
    [HttpPost("compute")]
    public async Task<ActionResult<decimal>> ComputePremiumAsync([FromBody] PremiumComputeRequestModel model) =>
        Ok(_premiumService.ComputePremium(model));
}