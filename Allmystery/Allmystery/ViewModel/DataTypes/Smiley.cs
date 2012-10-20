using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Allmystery.ViewModel.DataTypes
{
    public class Smiley
    {
        public string orgSmiley { get; set; }
        public string wp7Smiley { get; set; }

        public Smiley(string org, string wp7)
        {
            this.orgSmiley = org;
            this.wp7Smiley = wp7;
        }
    }
}
