using System;
using GalaSoft.MvvmLight.Messaging;

namespace CleanBudget.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private IMessenger messenger;

        public HomeViewModel(IMessenger messenger)
        {
            this.messenger = messenger;
        }
    }
}
