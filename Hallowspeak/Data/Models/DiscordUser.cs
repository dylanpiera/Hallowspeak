namespace Hallowspeak.Data.Models
{
    /// <summary>
    /// Used for oAuth Authentication
    /// </summary>
    public struct DiscordUser
    {
        /// <summary>
        /// Discord User ID
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// Discord Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Website AuthLevel, if set in Database will adopt that number, otherwise be 0 (user)
        /// </summary>
        public AuthLevel AuthLevel { get; set; }
    }

    public enum AuthLevel
    {
        User = 0,
        Writer = 1,
        Administrator = 2
    }
}
