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
using System.Collections.Generic;
using MaterialDesignThemes.Wpf;

namespace CleanBudget.ViewModels
{
    public class AccountViewModel : BaseViewModel
    {
        #region Variables
        //Service
        private IMessenger _messenger;
        private UserRepository _userRepository;
        private AccountRepository _accountRepository;
        private CurrencyRepository _currencyRepository;

        //Model
        public Account CurrentAccount { get; set; } = null;
        public Currency PrevCurrency { get; set; } = null;
        public Currency CurrentCurrency { get; set; } = null;
        public Guid CurrentAccountId { get; set; } = Guid.Empty;
        public List<Currency> Currencies { get; set; } = new List<Currency>();

        //Form
        public string Email { get; set; } = string.Empty;
        public string Firstname { get; set; } = string.Empty;
        public string PrevFirstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string PrevLastname { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;
        public string FullnameValidation { get; set; } = string.Empty;
        public string PasswordValidation { get; set; } = string.Empty;
        public bool Checker { get; set; } = true;

        //Commands
        public RelayCommand LoadCommand { get; set; }
        public RelayCommand LogoutCommand { get; set; }
        public RelayCommand ChangeFullnameCommand { get; set; }
        public RelayCommand ChangeCurrencyCommand { get; set; }
        public RelayCommand<object> ChangePasswordCommand { get; set; }
        public RelayCommand<PasswordBox> NewPasswordChangedCommand { get; set; }
        public RelayCommand<PasswordBox> OldPasswordChangedCommand { get; set; }
        #endregion

        public AccountViewModel(IMessenger messenger, UserRepository userRepository, AccountRepository accountRepository, CurrencyRepository currencyRepository)
        {
            _messenger = messenger;
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _currencyRepository = currencyRepository;
            _messenger.Register<SendAccount>(this, ReceiveAccount, true);

            LoadCommand = new RelayCommand(Loaded);
            LogoutCommand = new RelayCommand(Logout);
            ChangeCurrencyCommand = new RelayCommand(ChangeCurrency);
            ChangeFullnameCommand = new RelayCommand(ChangeFullName);
            ChangePasswordCommand = new RelayCommand<object>(ChangePassword);
            OldPasswordChangedCommand = new RelayCommand<PasswordBox>(OldPasswordChanged);
            NewPasswordChangedCommand = new RelayCommand<PasswordBox>(NewPasswordChanged);
        }

        private void Loaded() => Task.Run(() =>
        {
            if (CurrentAccount == null)
            {
                Currencies = (List<Currency>)_currencyRepository.GetAll();
                CurrentAccount = _accountRepository.GetById(CurrentAccountId);
                Email = CurrentAccount.User.Email;
                PrevFirstname = Firstname = CurrentAccount.User.Firstname;
                PrevLastname = Lastname = CurrentAccount.User.Lastname;
                PrevCurrency = CurrentCurrency = Currencies[Currencies.FindIndex(c => c.Id == CurrentAccount.CurrencyId)];
            }
        });
        
        private void Logout()
        {
            if (Currencies.Count != 0 && CurrentAccount != null)
            {
                Currencies.Clear();
                CurrentAccount = null;
                CurrentAccountId = Guid.Empty;
                PrevCurrency = CurrentCurrency = null;
                _messenger.Send(new Navigation(typeof(LoginViewModel)));
            }
        }

        private void ChangeCurrency()
        {
            if (PrevCurrency != CurrentCurrency)
            {
                PrevCurrency = CurrentCurrency;
                Task.Run(() => _accountRepository.UpdateCurrency(CurrentAccountId, CurrentCurrency));
            }
        }

        private async void ChangeFullName()
        {
            if (!PrevFirstname.Equals(Firstname) && !PrevLastname.Equals(Lastname))
            {
                Checker = true;

                //Fullname
                if (Firstname == string.Empty || Lastname == string.Empty)
                {
                    Checker = false;
                    FullnameValidation = "* Fill Full name fields";
                }
                else if (!ValidatorExtensions.IsNameValid(Firstname) || !ValidatorExtensions.IsNameValid(Lastname))
                {
                    Checker = false;
                    FullnameValidation = "* Invalid format of full name";
                }
                else FullnameValidation = string.Empty;

                if (Checker) await Task.Run(() => TryChangeFullname());
            }
        }
        private void TryChangeFullname()
        {
            try
            {
                var user = _userRepository.GetById(CurrentAccount.User.Id);
                if (user != null)
                {
                    user.Firstname = Firstname;
                    user.Lastname = Lastname;
                    _userRepository.Update(user);
                    PrevFirstname = Firstname;
                    PrevLastname = Lastname;
                }
            }
            catch (Exception) { }
            FullnameValidation = string.Empty;
        }
        
        private void ReceiveAccount(SendAccount obj) => CurrentAccountId = obj.AccountId;

        private async void ChangePassword(object paramaters)
        {
            Checker = true;

            //Password
            if (OldPassword == string.Empty || NewPassword == string.Empty)
            {
                Checker = false;
                PasswordValidation = "* Fill Password fields";
            }
            else if (!ValidatorExtensions.IsPasswordValid(OldPassword) || !ValidatorExtensions.IsPasswordValid(NewPassword))
            {
                Checker = false;
                PasswordValidation = "* Invalid format of password";
            }
            else PasswordValidation = string.Empty;

            if (Checker)
            {
                var param = (Tuple<object, object>)paramaters;
                var param1 = (PasswordBox)param.Item1;
                var param2 = (PasswordBox)param.Item2;
                var transaction = await Task.Run(() => TryChangePassword(param1, param2));
                if (transaction) PasswordValidation = string.Empty;
            }
        }
        private Task<bool> TryChangePassword(PasswordBox oldpwdbox, PasswordBox newpwdbox)
        {
            bool flag = true;
            try
            {
                var hash = UserRepository.Decrypt(OldPassword, CurrentAccount.User.Salt);
                if (hash.Equals(CurrentAccount.User.Hash))
                {
                    var user = _userRepository.GetById(CurrentAccount.User.Id);
                    if (user != null)
                    {
                        var result = UserRepository.Encrypt(NewPassword);
                        user.Hash = result.Item1;
                        user.Salt = result.Item2;
                        _userRepository.Update(user);
                    }
                    else
                    {
                        flag = false;
                        PasswordValidation = "* Old password wrong";
                    }
                }
            }
            catch (Exception) { }
            PasswordClear(oldpwdbox, newpwdbox);
            return Task.FromResult(flag);
        }

        private void OldPasswordChanged(PasswordBox pwdbox) { if (pwdbox != null) OldPassword = pwdbox.Password; }
        private void NewPasswordChanged(PasswordBox pwdbox) { if (pwdbox != null) NewPassword = pwdbox.Password; }
        private void PasswordClear(PasswordBox oldpwdbox, PasswordBox newpwdbox) => Application.Current.Dispatcher.Invoke(() => oldpwdbox.Password = newpwdbox.Password = string.Empty);
    }
}