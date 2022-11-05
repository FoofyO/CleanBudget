﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanBudget.Models
{
    [Table("Cards")]
    public class Card
    {
        public Guid Id { get; set; }
        [MaxLength(50)]
        public string Title { get; set; }
        public double Balance { get; set; }
        [MaxLength(50)]
        public string Currency { get; set; }
        [MaxLength(50)]
        public string Description { get; set; }
        [MaxLength(50)]
        public string Icon { get; set; }
        [MaxLength(50)]
        public string Color { get; set; }
        public int Queue { get; set; }
        public Guid? AccountId { get; set; }
        public virtual Account Account { get; set; }

        public Card()
        {
            Id = Guid.NewGuid();
            Queue = 0;
        }

        public Card(string title, double balance, string currency, string description, string icon, string color, Account account) : this()
        {
            Icon = icon;
            Title = title;
            Color = color;
            Balance = balance;
            Account = account;
            Currency = currency;
            Description = description;
            AccountId = account.Id;
        }
    }
}
