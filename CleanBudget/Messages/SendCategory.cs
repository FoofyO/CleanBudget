using System;
using GalaSoft.MvvmLight.Messaging;

namespace CleanBudget.Messages
{
    public class SendCategory : Messenger
    {
        public Guid CategoryId { get; set; }

        public SendCategory() { }

        public SendCategory(Guid categoryId) => CategoryId = categoryId;
    }
}
