using Claims.Domain.Entities;

namespace Claims.Application.Interfaces;

public interface ICoverPeriod
{
    DateTime StartDate { get; }
    DateTime EndDate { get; }
}
