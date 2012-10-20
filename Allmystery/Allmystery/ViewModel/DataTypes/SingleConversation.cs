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
    public class SingleConversation
    {
        public string messageID { get; set; }
        public string userID { get; set; }
        public string Date { get; set; }
        public string Message { get; set; }
        public string Pic { get; set; }
        public string Username { get; set; }
    }
}
