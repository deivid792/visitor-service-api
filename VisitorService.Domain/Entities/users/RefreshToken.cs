using VisitorService.Domain.Shared;

namespace VisitorService.Domain.Entities
{

    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiryDate { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public RefreshToken(string token, DateTime expiryDate) : base()
        {
            Token = token;
            ExpiryDate = expiryDate;
        }
    }

}
