using Microsoft.AspNetCore.Identity;

namespace ReaderFast.webui.Services
{
    public class CustomIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresNonAlphanumeric),
                Description = "Your password must contain at least one non-alphanumeric character, like @ # $ . , / etc."
            };
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresDigit),
                Description = "Your password must contain at least one digit ('0'-'9')."
            };
        }
    }
}
