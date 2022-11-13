using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;

namespace CleanBudget.Models
{
    public class Currency
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string ShortName { get; set; }

        public Currency() => Id = Guid.NewGuid();

        public Currency(string fullName, string shortName) : this()
        {
            FullName = fullName;
            ShortName = shortName;
        }

        public override string ToString()
        {
            return $"{ShortName} | {FullName}";
        }
    }
}
