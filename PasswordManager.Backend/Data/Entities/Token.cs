namespace PasswordManager.Backend.Data.Entities
{
    public class Token : BaseEntity
    {
        public string? AccessTokenJti { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiryTime { get; set; }
        public string DeviceId { get; set; } = string.Empty;
        public string? PreviousRefreshToken { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
