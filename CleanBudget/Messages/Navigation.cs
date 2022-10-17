using System;
using GalaSoft.MvvmLight.Messaging;

namespace CleanBudget.Messages
{
    public class Navigation : Messenger
    {
        public Type ViewModelType { get; set; }
    }
}
