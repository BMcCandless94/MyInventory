namespace MILibrary.Password
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public static class MIPassword
    {
        public static readonly List<string> SpecialCharacters = new List<string>()
        {
            "!",
            "@",
            "#",
            "$",
            "&",
            "*"
        };

        public static readonly List<string> PasswordRules = new List<string>()
        {
            "Must contain one uppercase letter.",
            "Must contain one lowercase letter.",
            "Must contain one number.",
            "Must contain one of the following special characters: " + string.Join(", ", SpecialCharacters)
        };

        public static bool CheckPassword(string Password)
        {
            Regex rx = new Regex(string.Format("^([^0-9]*|[^A-Z]*|[^a-z]*|[a-zA-Z0-9]*|[#@!]*)$", string.Join("", SpecialCharacters)));
            return !rx.IsMatch(Password);
            }
    }
}

namespace MILibrary.Constants
{
    public static class Constants
    {
        //User Constants
        public const int USR_PASSWORD_MINLENGTH = 10;
        public const int USR_PASSWORD_MAXLENGTH = 30;
        public const int USR_EMAIL_MAXLENGTH = 150;
        public const int USR_FIRSTNAME_MAXLENGTH = 100;
        public const int USR_LASTNAME_MAXLENGTH = 150;

        //Warehouse Cosntants
        public const int WH_NAME_MAXLENGTH = 100;
        public const int WH_DESC_MAXLENGTH = 250;

        //Item Constants
        public const int ITM_NAME_MAXLENGTH = 100;
        public const int ITM_DESC_MAXLENGTH = 250;
        public const int ITM_UOM_MAXLENGTH = 20;
    }
}
