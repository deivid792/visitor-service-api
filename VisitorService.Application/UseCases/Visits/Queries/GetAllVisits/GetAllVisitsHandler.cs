using VisitorService.Application.Shared.results;
using VisitorService.Application.DTOS;
using VisitorService.Domain.Interfaces;

namespace VisitorService.Application.UseCases.Visits.Queries
{
    public class GetAllVisitsHandler : IGetAllVisitsHandler
    {
        private readonly IVisitRepository _visitRepository;

        public GetAllVisitsHandler(IVisitRepository visitRepository)
        {
            _visitRepository = visitRepository;
        }

        public async Task<Result<IEnumerable<VisitResponseDTO>>> Handle()
        {
            var visits = await _visitRepository.GetPendingAndRecentAsync(10);

            var response = visits.Select(visit => new VisitResponseDTO
            {
                Id = visit.Id,
                UserId = visit.UserId,
                UserName = visit.User?.Name?.ToString() ?? "Visitante Desconhecido",
                Date = visit.Date,
                Time = visit.Time,
                Reason = visit.Reason,
                Category = visit.Category,
                Status = visit.Status,
                CheckIn = visit.CheckIn,
                CheckOut = visit.CheckOut
            });

            return Result<IEnumerable<VisitResponseDTO>>.Success(response);
        }
    }
}