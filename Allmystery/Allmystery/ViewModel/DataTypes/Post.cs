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
    public class Post
    {
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
        private string text;
        public string Text
        {
            set
            {
                this.text = value;
            }
            get
            {
                return this.text;
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
        private string usercolor;
        public string Usercolor
        {
            set
            {
                this.usercolor = value;
            }
            get
            {
                return this.usercolor;
            }
        }
        private string postid;
        public string postID
        {
            set
            {
                this.postid = value;
            }
            get
            {
                return this.postid;
            }
        }
        private string date;
        public string Date
        {
            set
            {
                this.date = value;
            }
            get
            {
                return this.date;
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
    }
}
