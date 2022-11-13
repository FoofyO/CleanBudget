using System;
using CleanBudget.Models;
using GalaSoft.MvvmLight.Messaging;

namespace CleanBudget.Messages
{
    public class SendAccount : Messenger
    {
        public Guid AccountId { get; set; }

        public SendAccount() { }

        public SendAccount(Guid accountId) => AccountId = accountId;
    }
}
