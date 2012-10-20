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
    public class MainViewModelEventArgs : EventArgs
    {
        public string unreadMsgs { get; set; }
        public string unreadThreads { get; set; }

        public MainViewModelEventArgs(string unreadmsgs, string unreadthreads)
        {
            this.unreadMsgs = unreadmsgs;
            this.unreadThreads = unreadthreads;
        }
    }
}
