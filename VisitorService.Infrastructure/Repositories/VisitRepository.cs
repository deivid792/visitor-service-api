using Microsoft.EntityFrameworkCore;
using VisitorService.Domain.Entities;
using VisitorService.Domain.Interfaces;
using VisitorService.Infrastructure.Context;

namespace VisitorService.Infrastructure.Repositories
{
    public class VisitRepository : IVisitRepository
    {
        private readonly AppDbContext _context;

        public VisitRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Visit>> GetPendingAndRecentAsync(int limit = 10)
        {
            return await _context.Visits
                .Include(v => v.User)
                .Where(v => v.Status == "Pending" || v.Status == "Pendente")
                .OrderByDescending(v => v.Date)
                .ThenByDescending(v => v.Time)
                .Take(limit)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Visit?> GetByIdAsync(Guid id)
        {
            return await _context.Visits
                .Include(v => v.User)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task UpdateAsync(Visit visit)
        {
            _context.Visits.Update(visit);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsVisitInDateAndTime(DateOnly date, TimeOnly time)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            return await _context.Visits
                .Where(v =>
                    v.Date == date &&
                    v.Time == time &&

                    v.Date >= today &&

                    (v.Status == "Pendente" || v.Status == "Aprovada")

                )
                .AnyAsync();
        }

        public async Task<IEnumerable<Visit>> GetApprovedByDateAsync(DateOnly date)
        {
            return await _context.Visits
                .Where(v => v.Date == date && v.Status == "Aprovada")
                .Include(v => v.User)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
