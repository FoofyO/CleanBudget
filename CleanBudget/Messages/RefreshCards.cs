using GalaSoft.MvvmLight.Messaging;
using System;

namespace CleanBudget.Messages
{
    public class RefreshCards : Messenger 
    {
        public bool Refresh { get; set; }

        public RefreshCards() { }

        public RefreshCards(bool refresh) => Refresh = refresh;
    }
}
