namespace Auth0WebAPI.Configuration
{
    public class Auth0Configuration
    {
        public string Domain { get; set; }
        public string Audience { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string grant_type { get; set; }
    }
}
