namespace VisitorService.Application.DTOS
{
    public class VisitResponseDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public string? Reason { get; set; }
        public string? Category { get; set; }
        public string? Status { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
    }
}