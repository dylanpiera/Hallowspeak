namespace Hallowspeak.Data.Models
{
    /// <summary>
    /// Used for OAuth Project Authentication
    /// </summary>
    public struct DiscordClientCredentials
    {
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
    }
}
