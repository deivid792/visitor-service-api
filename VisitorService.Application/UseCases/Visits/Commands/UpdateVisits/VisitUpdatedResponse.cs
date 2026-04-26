namespace VisitorService.Application.UseCases.Visits.Commands;

public class VisitUpdatedResponse
{
    public Guid Id { get; set; }
    public string Status { get; set; } = "";
    public string VisitorName { get; set; } = "";
}