using System;
using GalaSoft.MvvmLight.Messaging;

namespace CleanBudget.Messages
{
    public class NavBar : Messenger
    {
        public string View { get; set; }

        public NavBar() { }

        public NavBar(string view) => View = view;
    }
}
