namespace vnenterprises.Models
{
    public class Home
    {
        

    }

    public class AccountModelHandle
    {
        public LoginViewModel login { get; set; }
        public ForgotPasswordModel forgotpassword { get; set; }
        public MpinModel mpin { get; set; }
    }
    public class LoginViewModel
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public int MPIN { get; set; }
    }
    public class ForgotPasswordModel
    {
        public string PhoneNumber { get; set; }
        public int MPIN { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
    public class RegisterModel
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
    public class MpinModel
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Mpin { get; set; }
        public string ConfirmMpin { get; set; }
    }

    public class LogInResponse
    {
        public int UserId { get; set; }
        public int AccessType { get; set; }
    }
}
