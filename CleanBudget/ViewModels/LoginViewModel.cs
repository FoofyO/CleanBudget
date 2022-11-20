using System;
using System.Windows;
using CleanBudget.Messages;
using CleanBudget.Services;
using System.Threading.Tasks;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using CleanBudget.Services.Repositories;
using CleanBudget.Models;
using CleanBudget.Database;

namespace CleanBudget.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        #region Variables
        //Services
        private IMessenger _messenger;
        private UserRepository _repository;

        //Spinner
        public bool Checker { get; set; } = true;
        public bool IsSpin { get; set; } = false;
        public string IsVisible { get; set; } = "Hidden";

        //Form
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string EmailValidation { get; set; } = string.Empty;
        public string PasswordValidation { get; set; } = string.Empty;

        //Commands
        public RelayCommand LoadCommand { get; set; }
        public RelayCommand RegisterCommand { get; set; }
        public RelayCommand<PasswordBox> LoginCommand { get; set; }
        public RelayCommand<PasswordBox> PasswordChangedCommand { get; set; }
        #endregion

        public LoginViewModel(IMessenger messenger, UserRepository repository)
        {
            _messenger = messenger;
            _repository = repository;
            LoadCommand = new RelayCommand(Loaded);
            RegisterCommand = new RelayCommand(Registration);
            LoginCommand = new RelayCommand<PasswordBox>(Login);
            PasswordChangedCommand = new RelayCommand<PasswordBox>(PasswordChanged);
        }

        private async void Login(PasswordBox pwdbox)
        {
            Checker = true;

            //Email
            if (Email == string.Empty)
            {
                Checker = false;
                EmailValidation = "* Fill Email Field";
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
                PasswordValidation = "* Fill Password Field";
            }
            else if (Password.Length < 8)
            {
                Checker = false;
                PasswordValidation = "* Password Is Too Short";
            }
            else if (Password.Length > 20)
            {
                Checker = false;
                PasswordValidation = "* Password Is Too Long";
            }
            else if (!ValidatorExtensions.IsPasswordValid(Password))
            {
                Checker = false;
                PasswordValidation = "* Password Is Very Easy";
            }
            else PasswordValidation = string.Empty;

            if (Checker)
            {
                IsSpin = true;
                IsVisible = "Visible";
                var account = await Task.Run<Account>(() => TryLogin(pwdbox));
                if (account != null)
                {
                    PasswordValidation = Email = string.Empty;
                    _messenger.Send(new SendAccount(account.Id));
                    _messenger.Send(new Navigation(typeof(HomeViewModel)));
                }
            }
        }

        private Task<Account> TryLogin(PasswordBox pwdbox)
        {
            Account account = null;
            try
            {
                var id = _repository.GetId(Email);
                if (id != Guid.Empty)
                {
                    var user = _repository.Login(id, Password);
                    if (user == null) PasswordValidation = "* Invalid Email Address Or Password";
                    else account = user.Account;
                }
                else PasswordValidation = "* Invalid Email Address Or Password";
            }
            catch (Exception ex) { }

            IsSpin = false;
            IsVisible = "Hidden";
            PasswordClear(pwdbox);
            return Task.FromResult(account);
        }

        private void Registration()
        {
            Email = EmailValidation = PasswordValidation = string.Empty;
            _messenger.Send<Navigation>(new Navigation(typeof(RegisterViewModel)));
        }

        private void Loaded() => _messenger.Send(new Resize(530, 450, false, false));

        private void PasswordChanged(PasswordBox pwdbox) { if (pwdbox != null) Password = pwdbox.Password; }
        
        private void PasswordClear(PasswordBox pwdbox) => Application.Current.Dispatcher.Invoke(() => pwdbox.Password = string.Empty);
    }
}
