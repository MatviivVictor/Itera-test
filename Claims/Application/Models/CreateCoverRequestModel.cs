using Claims.Application.Interfaces;
using Claims.Domain.Entities;

namespace Claims.Application.Models;

public class CreateCoverRequestModel : ICoverPeriod
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public CoverType Type { get; set; }
}