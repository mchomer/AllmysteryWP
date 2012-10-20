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
using Allmystery.ViewModel;

namespace Allmystery.ViewModel.DataTypes
{
    public class Conversation : BaseViewModel 
    {
        
        private string color;
        public string Color
        {
            set
            {
                this.color = value;
                //this.OnPropertyChanged("Username");
            }
            get
            {
                return this.color;
            }
        }
        private string username;
        public string Username
        {
            set
            {
                this.username = value;
                //this.OnPropertyChanged("Username");
            }
            get
            {
                return this.username;
            }
        }
        private string pic;
        public string Pic
        {
            set
            {
                this.pic = value;
                //this.OnPropertyChanged("Pic");
            }
            get
            {
                return this.pic;
            }
        }
        private string date;
        public string Date
        {
            set
            {
                this.date = value;
                //this.OnPropertyChanged("Date");
            }
            get
            {
                return this.date;
            }
        }
        private string unreadmsg;
        public string unreadMsg
        {
            set
            {
                this.unreadmsg = value;
                //this.OnPropertyChanged("unreadMsg");
            }
            get
            {
                return this.unreadmsg;
            }
        }
        private string userid;
        public string userID
        {
            set
            {
                this.userid = value;
                //this.OnPropertyChanged("userID");
            }
            get
            {
                return this.userid;
            }
        }
    }
}
