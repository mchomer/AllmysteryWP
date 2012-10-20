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
    public class Category
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
        public string Cat
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
        private string titlelong;
        public string titleLong
        {
            set
            {
                this.titlelong = value;
            }
            get
            {
                return this.titlelong;
            }
        }
        private string titleshort;
        public string titleShort
        {
            set
            {
                this.titleshort = value;
            }
            get
            {
                return this.titleshort;
            }
        }
        private string lastpostthreadid;
        public string lastPostThreadID
        {
            set
            {
                this.lastpostthreadid = value;
            }
            get
            {
                return this.lastpostthreadid;
            }
        }
        private string lastposttitle;
        public string lastPostTitle
        {
            set
            {
                this.lastposttitle = value;
            }
            get
            {
                return this.lastposttitle;
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
        private string lastpostpage;
        public string lastPostPage
        {
            set
            {
                this.lastpostpage = value;
            }
            get
            {
                return this.lastpostpage;
            }
        }
        private string count;
        public string Count
        {
            set
            {
                this.count = value;
            }
            get
            {
                return this.count;
            }
        }
        private string lastpostid;
        public string lastPostID
        {
            set
            {
                this.lastpostid = value;
            }
            get
            {
                return this.lastpostid;
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
        private string defaulttags;
        public string defaultTags
        {
            set
            {
                this.defaulttags = value;
            }
            get
            {
                return this.defaulttags;
            }
        }
    }
    
}
