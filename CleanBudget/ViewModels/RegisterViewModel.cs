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
using CleanBudget.Database;

namespace CleanBudget.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        #region Variables
        //Services
        private IMessenger _messenger;
        private UserRepository _userRepository;
        private AccountRepository _accountRepository;
        private CurrencyRepository _currencyRepository;

        //Spinner
        public bool Checker { get; set; } = true;
        public bool IsSpin { get; set; } = false;
        public string IsVisible { get; set; } = "Hidden";

        //Form
        public string Email { get; set; } = string.Empty;
        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string EmailValidation { get; set; } = string.Empty;
        public string FirstnameValidation { get; set; } = string.Empty;
        public string LastnameValidation { get; set; } = string.Empty;
        public string PasswordValidation { get; set; } = string.Empty;

        //Account
        public Guid NewAccountId { get; set; }

        //Commands
        public RelayCommand LoadCommand { get; set; } 
        public RelayCommand LoginCommand { get; set; }
        public RelayCommand<PasswordBox> RegisterCommand { get; set; }
        public RelayCommand<PasswordBox> PasswordChangedCommand { get; set; }
        #endregion

        public RegisterViewModel(IMessenger messenger, UserRepository userRepository, AccountRepository accountRepository, CurrencyRepository currencyRepository)
        {
            _messenger = messenger;
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _currencyRepository = currencyRepository;
            LoadCommand = new RelayCommand(Loaded);
            LoginCommand = new RelayCommand(Login);
            RegisterCommand = new RelayCommand<PasswordBox>(Registration);
            PasswordChangedCommand = new RelayCommand<PasswordBox>(PasswordChanged);
        }

        private async void Registration(PasswordBox pwdbox)
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

            //First name
            if (Firstname == string.Empty)
            {
                Checker = false;
                FirstnameValidation = "* Fill First Name Field";
            }
            else if (!ValidatorExtensions.IsNameValid(Firstname))
            {
                Checker = false;
                FirstnameValidation = "* Invalid First Name";
            }
            else FirstnameValidation = string.Empty;

            //Last name
            if (Lastname == string.Empty)
            {
                Checker = false;
                LastnameValidation = "* Fill Last Name Field";
            }
            else if (!ValidatorExtensions.IsNameValid(Lastname))
            {
                Checker = false;
                LastnameValidation = "* Invalid Last Name";
            }
            else LastnameValidation = string.Empty;

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
                var register = await Task.Run<bool>(() => TryRegister(pwdbox));
                if (register)
                {
                    PasswordValidation = Email = Firstname = Lastname = string.Empty;
                    _messenger.Send(new SendAccount(NewAccountId));
                    _messenger.Send(new Navigation(typeof(HomeViewModel)));
                }
            }
        }

        private Task<bool> TryRegister(PasswordBox pwdbox)
        {
            bool flag = true;
            try
            {
                if (_userRepository.GetId(Email) == Guid.Empty)
                {
                    Account account = null;
                    try
                    {
                        var crypt = UserRepository.Encrypt(Password);
                        var user = new User(Email, Firstname, Lastname, crypt.Item1, crypt.Item2);
                        account = new Account(user, _currencyRepository.GetDollar());
                        user.SetAccount(account);
                        account.User = user;
                        _accountRepository.Create(account);
                        _userRepository.Create(user);
                        
                    }
                    catch (Exception ex) { }
                    finally { NewAccountId = account.Id; }
                }
                else
                {
                    flag = false;
                    PasswordValidation = "* This Email Addres Already Registered";
                }
            }
            catch(Exception ex) { }
            
            IsSpin = false;
            IsVisible = "Hidden";
            PasswordClear(pwdbox);
            return Task.FromResult(flag);
        }

        private void Login()
        {
            Email = Firstname = Lastname = Password = EmailValidation = FirstnameValidation = LastnameValidation = PasswordValidation = string.Empty;
            _messenger.Send<Navigation>(new Navigation(typeof(LoginViewModel)));
        }

        private void Loaded() => _messenger.Send(new Resize(700, 500, false, false));
        
        private void PasswordChanged(PasswordBox pwdbox) { if (pwdbox != null) Password = pwdbox.Password; }

        private void PasswordClear(PasswordBox pwdbox) => Application.Current.Dispatcher.Invoke(() => pwdbox.Password = string.Empty);
    }
}
