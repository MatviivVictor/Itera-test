using Claims.Application.Models;
using Claims.Domain.Entities;

namespace Claims.Application.Factories;

public static class CoverFactory
{
    public static CoverModel Create(Cover cover) => new CoverModel
    {
        Id = cover.Id,
        StartDate = cover.StartDate,
        EndDate = cover.EndDate,
        Type = cover.Type,
        Premium = cover.Premium
    };
    
    public static Cover Create(CreateCoverRequestModel model) => new Cover
    {
        Id = Guid.CreateVersion7().ToString(),
        StartDate = model.StartDate,
        EndDate = model.EndDate,
        Type = model.Type,
    };
}