using System;
using System.Text.RegularExpressions;

namespace CleanBudget.Services
{
    public class ValidatorExtensions
    {
        public static Regex regex { get; set; }

        public static bool IsPasswordValid(string value)
        {
            regex = new Regex(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-_]).{8,}$");
            return regex.IsMatch(value);
        }

        public static bool IsEmailValid(string value)
        {
            regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            return regex.IsMatch(value);
        }

        public static bool IsNameValid(string value)
        {
            regex = new Regex(@"^([A-Z]){1}([a-z]){2,15}$");
            return regex.IsMatch(value);
        }
    }
}
