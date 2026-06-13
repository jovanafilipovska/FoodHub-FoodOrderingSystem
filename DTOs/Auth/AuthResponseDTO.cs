namespace FoodHub.DTOs.Auth
{
    public class AuthResponseDTO
    {
        public string Token { get; set; } = "";

        public DateTime Expiration { get; set; }

        public UserDTO User { get; set; } = null!;

        public IList<string> Roles { get; set; } = new List<string>();
    }
}
