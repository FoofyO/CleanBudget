using CleanBudget.Messages;
using CleanBudget.Services.Repositories;
using GalaSoft.MvvmLight.Command;
using System;
using GalaSoft.MvvmLight.Messaging;

namespace CleanBudget.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private IMessenger messenger;
        private UserRepository repository;
        public string Email { get; set; }
        public string Password { get; set; }

        public RelayCommand LoadCommand { get; set; }
        public RelayCommand LoginCommand { get; set; }
        public RelayCommand RegisterCommand { get; set; }

        public LoginViewModel(IMessenger messenger, UserRepository repository)
        {
            this.messenger = messenger;
            this.repository = repository;
            Email = Password = string.Empty;
            LoginCommand = new RelayCommand(Login);
            LoadCommand = new RelayCommand(Loaded);
            RegisterCommand = new RelayCommand(Registration);
        }

        public void Login()
        {
        }

        public void Loaded() => messenger.Send<Resize>(new Resize(550, 450, false));

        public void Registration() => messenger.Send<Navigation>(new Navigation { ViewModelType = typeof(RegisterViewModel) });
    }
}
