using System;
using GalaSoft.MvvmLight.Messaging;

namespace CleanBudget.Messages
{
    public class Resize : Messenger
    {
        public double Height { get; set; }
        public double Width { get; set; }
        public bool CanResize { get; set; }

        public Resize() { }

        public Resize(double height, double width, bool canResize)
        {
            this.Height = height;
            this.Width = width;
            this.CanResize = canResize;
        }
    }
}
