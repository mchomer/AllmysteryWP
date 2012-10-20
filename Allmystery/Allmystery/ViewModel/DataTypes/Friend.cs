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
    public class Friend
    {
        private string fontcolor;
        public string fontColor
        {
            set
            {
                this.fontcolor = value;
            }
            get
            {
                return this.fontcolor;
            }
        }
        private string color;
        public string Color
        {
            set
            {
                this.color = value;
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
            }
            get
            {
                return this.username;
            }
        }
        private string busy;
        public string Busy
        {
            set
            {
                this.busy = value;
            }
            get
            {
                return this.busy;
            }
        }
        private string pic;
        public string Pic
        {
            set
            {
                this.pic = value;
            }
            get
            {
                return this.pic;
            }
        }
        private string statusmsg;
        public string statusMsg
        {
            set
            {
                this.statusmsg = value;
            }
            get
            {
                return this.statusmsg;
            }
        }
        private string online;
        public string Online
        {
            set
            {
                this.online = value;
            }
            get
            {
                return this.online;
            }
        }
        private string userid;
        public string userID
        {
            set
            {
                this.userid = value;
            }
            get
            {
                return this.userid;
            }
        }
        private string mobile;
        public string Mobile
        {
            set
            {
                this.mobile = value;
            }
            get
            {
                return this.mobile;
            }
        }
        
    }
}
