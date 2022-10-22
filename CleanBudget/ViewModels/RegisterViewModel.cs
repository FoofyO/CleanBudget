using System;
using System.Windows;
using CleanBudget.Models;
using CleanBudget.Messages;
using CleanBudget.Services;
using System.Threading.Tasks;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using CleanBudget.Services.Repositories;

namespace CleanBudget.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private IMessenger messenger;
        private UserRepository repository;
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Password { get; set; }
        public string EmailValidation { get; set; }
        public string FirstnameValidation { get; set; }
        public string LastnameValidation { get; set; }
        public string PasswordValidation { get; set; }
        public bool IsSpin { get; set; }
        public bool Checker { get; set; }
        public string IsVisible { get; set; }

        public RelayCommand LoadCommand { get; set; }
        public RelayCommand LoginCommand { get; set; }
        public RelayCommand<PasswordBox> RegisterCommand { get; set; }
        public RelayCommand<PasswordBox> PasswordChangedCommand { get; set; }

        public RegisterViewModel(IMessenger messenger, UserRepository repository)
        {
            this.messenger = messenger;
            this.repository = repository;
            IsVisible = "Hidden"; IsSpin = false;
            Email = Firstname = Lastname = Password = EmailValidation =
            FirstnameValidation = LastnameValidation = PasswordValidation = string.Empty;

            LoginCommand = new RelayCommand(Login);
            LoadCommand = new RelayCommand(Loaded);
            RegisterCommand = new RelayCommand<PasswordBox>(Registration);
            PasswordChangedCommand = new RelayCommand<PasswordBox>(PasswordChanged);
        }

        public void Registration(PasswordBox pwdbox)
        {
            Checker = true;
            
            //Email
            if (Email == string.Empty)
            {
                Checker = false;
                EmailValidation = "* Fill Email field";
            }
            else if (!ValidatorExtensions.IsEmailValid(Email))
            {
                Checker = false;
                EmailValidation = "* Invalid Email";
            }
            else EmailValidation = string.Empty;

            //First name
            if (Firstname == string.Empty)
            {
                Checker = false;
                FirstnameValidation = "* Fill First name field";
            }
            else if (!ValidatorExtensions.IsNameValid(Firstname))
            {
                Checker = false;
                FirstnameValidation = "* Invalid First name";
            }
            else FirstnameValidation = string.Empty;

            //Last name
            if (Lastname == string.Empty)
            {
                Checker = false;
                LastnameValidation = "* Fill Last name field";
            }
            else if (!ValidatorExtensions.IsNameValid(Lastname))
            {
                Checker = false;
                LastnameValidation = "* Invalid Last name";
            }
            else LastnameValidation = string.Empty;

            //Password
            if (Password == string.Empty)
            {
                Checker = false;
                PasswordValidation = "* Fill Password field";
            }
            else if (!ValidatorExtensions.IsPasswordValid(Password))
            {
                Checker = false;
                PasswordValidation = "* The password is very easy";
            }
            else PasswordValidation = string.Empty;

            if (Checker)
            {
                IsSpin = true;
                IsVisible = "Visible";
                Task.Run(() => TryRegister(pwdbox));
            }
        }

        public void TryRegister(PasswordBox pwdbox)
        {
            if (repository.GetId(Email) == null)
            {
                var crypt = UserRepository.Encrypt(Password);
                repository.Create(new User(Email, Firstname, Lastname, crypt.Item1, crypt.Item2));
                IsSpin = false;
                IsVisible = "Hidden";
                PasswordClear(pwdbox);
                PasswordValidation = Email = Firstname = Lastname = string.Empty;
                //messenger.Send<SendUserMessage>(new SendUserMessage { User = user });
                //messenger.Send<Navigation>(new Navigation { ViewModelType = typeof(LoginViewModel) });
            }
            else
            {
                IsSpin = false;
                IsVisible = "Hidden";
                PasswordClear(pwdbox);
                PasswordValidation = "* This Email addres already registered";
            }
        }

        private void PasswordClear(PasswordBox pwdbox) => Application.Current.Dispatcher.Invoke(() => pwdbox.Password = string.Empty);

        private void PasswordChanged(PasswordBox pwdbox)
        {
            if (pwdbox != null) Password = pwdbox.Password;
        }

        public void Login()
        {
            Email = Firstname = Lastname = Password = EmailValidation = FirstnameValidation = LastnameValidation = PasswordValidation = string.Empty;
            messenger.Send<Navigation>(new Navigation { ViewModelType = typeof(LoginViewModel) });
        }

        public void Loaded() => messenger.Send<Resize>(new Resize(700, 500, false));
    }
}
