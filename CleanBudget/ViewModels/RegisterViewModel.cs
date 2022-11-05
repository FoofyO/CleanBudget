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
        #region Properties
        //Services
        private IMessenger _messenger;
        private UserRepository _userRepository;
        private AccountRepository _accountRepository;

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
        public Account NewAccount { get; set; }

        //Commands
        public RelayCommand LoadCommand { get; set; } 
        public RelayCommand LoginCommand { get; set; }
        public RelayCommand<PasswordBox> RegisterCommand { get; set; }
        public RelayCommand<PasswordBox> PasswordChangedCommand { get; set; }
        #endregion

        public RegisterViewModel(IMessenger messenger, UserRepository userRepository, AccountRepository accountRepository)
        {
            _messenger = messenger;
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            LoadCommand = new RelayCommand(Loaded);
            LoginCommand = new RelayCommand(Login);
            RegisterCommand = new RelayCommand<PasswordBox>(Registration);
            PasswordChangedCommand = new RelayCommand<PasswordBox>(PasswordChanged);
        }
        
        public async void Registration(PasswordBox pwdbox)
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
            else if (Password.Length < 8)
            {
                Checker = false;
                PasswordValidation = "* The password is too short";
            }
            else if (Password.Length > 20)
            {
                Checker = false;
                PasswordValidation = "* The password is too long";
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
                var register = await Task.Run<bool>(() => TryRegister(pwdbox));
                if (register)
                {
                    PasswordValidation = Email = Firstname = Lastname = string.Empty;
                    _messenger.Send<SendAccount>(new SendAccount { Account = NewAccount });
                    _messenger.Send<Navigation>(new Navigation { ViewModelType = typeof(HomeViewModel) });
                }
            }
        }

        public Task<bool> TryRegister(PasswordBox pwdbox)
        {
            bool flag = true;
            try
            {
                if (_userRepository.GetId(Email) == Guid.Empty)
                {
                    try
                    {
                        var crypt = UserRepository.Encrypt(Password);
                        var user = new User(Email, Firstname, Lastname, crypt.Item1, crypt.Item2);
                        var account = new Account(user);
                        user.SetAccount(account);
                        _userRepository.Create(user);
                        _accountRepository.Create(account);
                        user.Account = account;
                        account.User = user;
                        NewAccount = account;
                    }
                    catch (Exception ex) { }
                }
                else
                {
                    flag = false;
                    PasswordValidation = "* This Email addres already registered";
                }
            }
            catch(Exception ex) { }
            
            IsSpin = false;
            IsVisible = "Hidden";
            PasswordClear(pwdbox);
            return Task.FromResult(flag);
        }

        public void Login()
        {
            Email = Firstname = Lastname = Password = EmailValidation = FirstnameValidation = LastnameValidation = PasswordValidation = string.Empty;
            _messenger.Send<Navigation>(new Navigation { ViewModelType = typeof(LoginViewModel) });
        }

        public void Loaded() => _messenger.Send<Resize>(new Resize(700, 500, false));
        
        private void PasswordChanged(PasswordBox pwdbox) { if (pwdbox != null) Password = pwdbox.Password; }

        private void PasswordClear(PasswordBox pwdbox) => Application.Current.Dispatcher.Invoke(() => pwdbox.Password = string.Empty);
    }
}
