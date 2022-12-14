using System;
using CleanBudget.Models;
using CleanBudget.Services;
using CleanBudget.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace CleanBudget.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        #region Variables
        //Services
        private IMessenger _messenger;
        public BaseViewModel CurrentViewModel { get; set; }

        //Commands
        public RelayCommand LoadCommand { get; set; }
        public RelayCommand<string> NavigationCommand { get; set; }
        #endregion

        public HomeViewModel(IMessenger messenger)
        {
            _messenger = messenger;
            LoadCommand = new RelayCommand(Loaded);
            _messenger.Register<NavBar>(this, NavBar, true);
            NavigationCommand = new RelayCommand<string>(Navigation);
        }

        private void Navigation(string message)
        {
            if (message.Equals("Account")) CurrentViewModel = App.accountViewModel;
            else if (message.Equals("Cards")) CurrentViewModel = App.cardsViewModel;
            else if (message.Equals("AddCard")) CurrentViewModel = App.addCardViewModel;
            else if (message.Equals("EditCard")) CurrentViewModel = App.editCardViewModel;
            else if (message.Equals("Operations")) CurrentViewModel = App.operationsViewModel;
            else if (message.Equals("Categories")) CurrentViewModel = App.categoriesViewModel;
            else if (message.Equals("AddCategory")) CurrentViewModel = App.addCategoryViewModel;
            else if (message.Equals("EditCategory")) CurrentViewModel = App.editCategoryViewModel;
            else if (message.Equals("Exit")) _messenger.Send(new Exit());
        }

        private void Loaded()
        {
            CurrentViewModel = App.accountViewModel;
            _messenger.Send(new Resize(450, 1100, false, true));
        }
        
        private void NavBar(NavBar obj) => Navigation(obj.View);
    }
}
