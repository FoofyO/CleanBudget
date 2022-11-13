using System;
using GalaSoft.MvvmLight.Messaging;

namespace CleanBudget.Messages
{
    public class SendCard : Messenger
    {
        public Guid CardId { get; set; }

        public SendCard() { }

        public SendCard(Guid cardId) => CardId = cardId;
    }
}
