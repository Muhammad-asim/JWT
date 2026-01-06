namespace JwtAuthDemo.Model
{
    public class RefreshTokenModel
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public bool IsExpired
        {
            get
            {
                return DateTime.Now >= Expires;
            }
        }
    }
}
