using System;
using System.Windows;
using CleanBudget.Models;
using CleanBudget.Messages;
using CleanBudget.Services;
using System.Threading.Tasks;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using CleanBudget.Services.Repositories;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System.Reflection.Metadata;
using System.Data.Common;

namespace CleanBudget.ViewModels
{
    public class AccountViewModel : BaseViewModel
    {
        #region Properties
        //Service
        private IMessenger _messenger;
        private UserRepository _userRepository;

        //Form
        public string Email { get; set; } = string.Empty;
        public string Firstname { get; set; } = string.Empty;
        public string PrevFirstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string PrevLastname { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;
        public bool Checker { get; set; } = true;
        public string FullnameValidation { get; set; } = string.Empty;
        public string PasswordValidation { get; set; } = string.Empty;
        public Account? CurrentAccount { get; set; } = null;

        //Commands
        public RelayCommand LoadCommand { get; set; }
        public RelayCommand ChangeFullnameCommand { get; set; }
        public RelayCommand LogoutCommand { get; set; }
        public RelayCommand<object> ChangePasswordCommand { get; set; }
        public RelayCommand<PasswordBox> NewPasswordChangedCommand { get; set; }
        public RelayCommand<PasswordBox> OldPasswordChangedCommand { get; set; }
        #endregion

        public AccountViewModel(IMessenger messenger, UserRepository userRepository)
        {
            _messenger = messenger;
            _userRepository = userRepository;
            _messenger.Register<SendAccount>(this, ReceiveAccount, true);
            LogoutCommand = new RelayCommand(Logout);
            ChangePasswordCommand = new RelayCommand<object>(ChangePassword);
            ChangeFullnameCommand = new RelayCommand(ChangeFullName);
            OldPasswordChangedCommand = new RelayCommand<PasswordBox>(OldPasswordChanged);
            NewPasswordChangedCommand = new RelayCommand<PasswordBox>(NewPasswordChanged);
        }

        private void Logout()
        {
            CurrentAccount = null;
            _messenger.Send<Navigation>(new Navigation() { ViewModelType = typeof(LoginViewModel) });
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
            catch (Exception ex) { }
            FullnameValidation = string.Empty;
        }

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
                var transaction = await Task.Run<bool>(() => TryChangePassword(param1, param2));
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
            catch (Exception ex) { }
            PasswordClear(oldpwdbox, newpwdbox);
            return Task.FromResult(flag);
        }

        private void ReceiveAccount(SendAccount obj)
        {
            CurrentAccount = obj.Account;
            Email = CurrentAccount.User.Email;
            PrevFirstname = Firstname = CurrentAccount.User.Firstname;
            PrevLastname = Lastname = CurrentAccount.User.Lastname;
        }
        private void OldPasswordChanged(PasswordBox pwdbox) { if (pwdbox != null) OldPassword = pwdbox.Password; }
        private void NewPasswordChanged(PasswordBox pwdbox) { if (pwdbox != null) NewPassword = pwdbox.Password; }
        private void PasswordClear(PasswordBox oldpwdbox, PasswordBox newpwdbox) => Application.Current.Dispatcher.Invoke(() => oldpwdbox.Password = newpwdbox.Password = string.Empty);
    }
}