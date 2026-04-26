using VisitorService.Application.Interfaces;
using VisitorService.Application.Shared.results;
using VisitorService.Domain.Enums;
using VisitorService.Domain.Interfaces;


namespace VisitorService.Application.UseCases.Visits.Commands
{
    public class UpdateVisitStatusHandler : IUpdateVisitStatusHandler
{
    private readonly IVisitRepository _visitRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public UpdateVisitStatusHandler(
        IVisitRepository visitRepository,
        IUserRepository userRepository,
        IEmailService emailService)
    {
        _visitRepository = visitRepository;
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public async Task<Result<VisitUpdatedResponse>> Handle(UpdateVisitStatusCommand dto, Guid managerId)
    {
        var isManager = await _userRepository.IsUserInRoleAsync(managerId, RoleType.Manager);

        if (!isManager)
            return Result<VisitUpdatedResponse>.Fail("Apenas gestores podem aprovar ou rejeitar visitas.");

        var visit = await _visitRepository.GetByIdAsync(dto.VisitId);

        if (visit == null)
            return Result<VisitUpdatedResponse>.Fail("Visita não encontrada.");

        var statusCased = char.ToUpper(dto.Status[0]) + dto.Status.Substring(1).ToLower();
        
        visit.UpdateStatus(statusCased);

        if (visit.HasErrors)
            return Result<VisitUpdatedResponse>.Fail(visit.Errors);

        await _visitRepository.UpdateAsync(visit);

        await _emailService.SendAsync(
            "deividgoncalves.dev@gmail.com",
            "Atualização da sua visita",
            $"<p>Sua visita foi marcada como <strong>{visit.Status}</strong>.</p>"
        );

    var response = new VisitUpdatedResponse {
        Id = visit.Id,
        Status = visit.Status.ToString(),
        VisitorName = visit.User?.Name?.Value ?? "Visitante"
    };

    return Result<VisitUpdatedResponse>.Success(response);
}
}

}