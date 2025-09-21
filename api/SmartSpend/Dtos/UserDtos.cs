namespace SmartSpend.Dtos
{
    public abstract class UserBaseDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }

    public class UserRegisterDto : UserBaseDto
    {
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class UserResponseDto : UserBaseDto
    {
        public string Id { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
    }
}
