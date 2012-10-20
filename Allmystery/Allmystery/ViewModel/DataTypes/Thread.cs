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
    public class Thread
    {
        private string backgroundcolor;
        public string backgroundColor
        {
            set
            {
                this.backgroundcolor = value;
                
            }
            get
            {
                return this.backgroundcolor;
            }
        }
        private string category;
        public string Category
        {
            set
            {
                this.category = value;
            }
            get
            {
                return this.category;
            }
        }
        private string title;
        public string Title
        {
            set
            {
                this.title = value;
            }
            get
            {
                return this.title;
            }
        }
        private string pages;
        public string Pages
        {
            set
            {
                this.pages = value;
            }
            get
            {
                return this.pages;
            }
        }
        private string threadid;
        public string threadID
        {
            set
            {
                this.threadid = value;
            }
            get
            {
                return this.threadid;
            }
        }
        
        private string lastpostdate;
        public string lastPostDate
        {
            set
            {
                this.lastpostdate = value;
            }
            get
            {
                return this.lastpostdate;
            }
        }
        
        private string postscount;
        public string postsCount
        {
            set
            {
                this.postscount = value;
            }
            get
            {
                return this.postscount;
            }
        }
        private string unread;
        public string Unread
        {
            set
            {
                this.unread = value;
            }
            get
            {
                return this.unread;
            }
        }
        private string icon;
        public string Icon
        {
            set
            {
                this.icon = value;
            }
            get
            {
                return this.icon;
            }
        }
        private string lastpostusername;
        public string lastPostUsername
        {
            set
            {
                this.lastpostusername = value;
            }
            get
            {
                return this.lastpostusername;
            }
        }
        private string createdusername;
        public string createdUsername
        {
            set
            {
                this.createdusername = value;
            }
            get
            {
                return this.createdusername;
            }
        }
        
    }
}
