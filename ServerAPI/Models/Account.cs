namespace ServerAPI.Models
{
    public class Account
    {
        public int? IDAccount { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string? SDT { get; set; }
        public string? PictureLink { get; set; }
    }
}
