namespace UserManagement.Models
{
    public class AuthenticateResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }


        public AuthenticateResponse(UserModel UserModel, string token)
        {
            UserId = UserModel.Id;
            Username = UserModel.Username;
            Email = UserModel.Email;
            Token = token;
        }
    }
    
}
