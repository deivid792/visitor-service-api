using VisitorService.Domain.Entities;

namespace VisitorService.Domain.Interfaces
{
    public interface IVisitRepository
{
    Task<IEnumerable<Visit>> GetPendingAndRecentAsync(int limit);
    Task<Visit?> GetByIdAsync(Guid id);
    Task UpdateAsync(Visit visit);
    Task<bool> ExistsVisitInDateAndTime(DateOnly date, TimeOnly time);
    Task<IEnumerable<Visit>> GetApprovedByDateAsync(DateOnly date);
}
}