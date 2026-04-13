using Claims.Domain.Entities;

namespace Claims.Application.Models;

public class CoverModel
{
    public string Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public CoverType Type { get; set; }
    public decimal Premium { get; set; }
}