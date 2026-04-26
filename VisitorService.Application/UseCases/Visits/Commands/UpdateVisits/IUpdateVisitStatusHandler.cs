using VisitorService.Application.Shared.results;
using VisitorService.Domain.Entities;

namespace VisitorService.Application.UseCases.Visits.Commands
{
    public interface IUpdateVisitStatusHandler
    {
        Task<Result<VisitUpdatedResponse>> Handle(UpdateVisitStatusCommand dto, Guid gestorId);
    };
}