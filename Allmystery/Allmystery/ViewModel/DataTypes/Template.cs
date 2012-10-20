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
    public class Template
    {
        public string Date { get; set; }
        public string threadID { get; set; }
        public string threadTitle { get; set; }
        public string Text { get; set; }
        public string backgroundColor { get; set; }
        public Template(string date, string threadid, string threadtitle, string text, string backgroundcolor)
        {
            this.Date = date;
            this.threadID = threadid;
            this.threadTitle = threadtitle;
            this.Text = text;
            this.backgroundColor = backgroundcolor;
        }
    }
}
