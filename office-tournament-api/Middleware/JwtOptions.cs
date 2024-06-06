namespace office_tournament_api.Middleware
{
    public class JwtOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }    
        public string SecretKey { get; set; }
    }
}
