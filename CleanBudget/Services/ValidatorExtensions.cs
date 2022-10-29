using System;
using System.Text.RegularExpressions;

namespace CleanBudget.Services
{
    public class ValidatorExtensions
    {
        private static Regex? Regex { get; set; }

        public static bool IsEmailValid(string value)
        {
            Regex = new Regex(@"^[a-z]{1}([a-z]|[.]|[0-9]){3,25}[@][a-z]{2,6}[.][a-z]{2,5}$");
            return Regex.IsMatch(value);
        }

        public static bool IsPasswordValid(string value)
        {
            Regex = new Regex(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-_]).{8,20}$");
            return Regex.IsMatch(value);
        }

        public static bool IsNameValid(string value)
        {
            Regex = new Regex(@"^([A-Z]){1}([a-z]){2,24}$");
            return Regex.IsMatch(value);
        }
    }
}
