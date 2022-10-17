using System;
using CleanBudget.Messages;
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
        public string Fistname { get; set; }
        public string Lastname { get; set; }
        public string Password { get; set; }

        public RelayCommand LoginCommand { get; set; }
        public RelayCommand RegisterCommand { get; set; }
        public RelayCommand LoadCommand { get; set; }

        public RegisterViewModel(IMessenger messenger, UserRepository repository)
        {
            this.messenger = messenger;
            this.repository = repository;
            Email = Fistname = Lastname = Password = string.Empty;
            
            LoginCommand = new RelayCommand(Login);
            LoadCommand = new RelayCommand(Loaded);
            RegisterCommand = new RelayCommand(Registration);
        }

        public void Login() => messenger.Send<Navigation>(new Navigation { ViewModelType = typeof(LoginViewModel) });

        public void Loaded() => messenger.Send<Resize>(new Resize(700, 500, false));

        public void Registration()
        {

        }
    }
}
