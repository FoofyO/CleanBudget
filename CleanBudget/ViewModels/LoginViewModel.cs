using System;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using CleanBudget.Messages;
using CleanBudget.Services;
using GalaSoft.MvvmLight.Command;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using CleanBudget.Services.Repositories;

namespace CleanBudget.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private IMessenger messenger;
        private UserRepository repository;
        public string Email { get; set; }
        public string Password { get; set; }
        public string EmailValidation { get; set; }
        public string PasswordValidation { get; set; }
        public bool IsSpin { get; set; }
        public bool Checker { get; set; }
        public string IsVisible { get; set; }

        public RelayCommand LoadCommand { get; set; }
        public RelayCommand RegisterCommand { get; set; }
        public RelayCommand<PasswordBox> LoginCommand { get; set; }
        public RelayCommand<PasswordBox> PasswordChangedCommand { get; set; }

        public LoginViewModel(IMessenger messenger, UserRepository repository)
        {
            this.messenger = messenger;
            this.repository = repository;
            IsVisible = "Hidden";
            IsSpin = false; Checker = true;
            Email = EmailValidation = Password = PasswordValidation = string.Empty;
            LoadCommand = new RelayCommand(Loaded);
            RegisterCommand = new RelayCommand(Registration);
            LoginCommand = new RelayCommand<PasswordBox>(Login);
            PasswordChangedCommand = new RelayCommand<PasswordBox>(PasswordChanged);
        }

        public void Login(PasswordBox pwdbox)
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
                Task.Run(()=> TryLogin(pwdbox));
            }
        }

        public void TryLogin(PasswordBox pwdbox)
        {
            string id = repository.GetId(Email);
            if (id != null)
            {
                var user = repository.Login(id, Password);

                if (user != null)
                {
                    IsSpin = false;
                    IsVisible = "Hidden";
                    PasswordClear(pwdbox);
                    PasswordValidation = Email = string.Empty;
                    //messenger.Send<SendUserMessage>(new SendUserMessage { User = user });
                    //messenger.Send<Navigation>(new Navigation { ViewModelType = typeof(LoginViewModel) });
                }
                else
                {
                    IsSpin = false;
                    IsVisible = "Hidden";
                    PasswordClear(pwdbox);
                    PasswordValidation = "* Invalid email address or password";
                }
            }
            else
            {
                IsSpin = false;
                IsVisible = "Hidden";
                PasswordClear(pwdbox);
                PasswordValidation = "* Invalid email address or password";
            }
        }

        private void PasswordClear(PasswordBox pwdbox) => Application.Current.Dispatcher.Invoke(() => pwdbox.Password = string.Empty);

        private void PasswordChanged(PasswordBox pwdbox)
        {
            if (pwdbox != null) Password = pwdbox.Password;
        }

        public void Registration()
        {
            Email = EmailValidation = PasswordValidation = string.Empty;
            messenger.Send<Navigation>(new Navigation { ViewModelType = typeof(RegisterViewModel) });
        }
        
        public void Loaded() => messenger.Send<Resize>(new Resize(530, 450, false));
    }
}
