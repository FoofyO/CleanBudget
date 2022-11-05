using System;
using CleanBudget.Models;
using GalaSoft.MvvmLight.Messaging;

namespace CleanBudget.Messages
{
    public class SendAccount : Messenger
    {
        public Account Account { get; set; }
    }
}
