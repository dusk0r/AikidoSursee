namespace AikidoWebsite.Models
{

    public class ChangePasswordModel {
        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel {
        public string UserName { get; set; }

        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

}
