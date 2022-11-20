using System;
using GalaSoft.MvvmLight.Messaging;

namespace CleanBudget.Messages
{
    public class RefreshCategories : Messenger
    {
        public bool Refresh { get; set; }

        public RefreshCategories() { }

        public RefreshCategories(bool refresh) => Refresh = refresh;
    }
}
