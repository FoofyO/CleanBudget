using System;
using GalaSoft.MvvmLight.Messaging;

namespace CleanBudget.Messages
{
    public class RefreshOperations : Messenger
    {
        public bool Refresh { get; set; }

        public RefreshOperations() { }

        public RefreshOperations(bool refresh) => Refresh = refresh;
    }
}
