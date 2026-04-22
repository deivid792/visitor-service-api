using VisitorService.Application.DTOS;
using VisitorService.Application.Shared.results;
using VisitorService.Domain.Entities;

namespace VisitorService.Application.UseCases.Visits.Queries
{
    public interface IGetAllVisitsHandler
    {
        Task<Result<IEnumerable<VisitResponseDTO>>> Handle();
    }
}