/* ***************************************************************************************************************************************************
 * Copyright (c) 2012 Michel Krämer
 * 
 * Hiermit wird unentgeltlich, jeder Person, die eine Kopie der Software und der zugehörigen Dokumentationen (die "Software") erhält, 
 * die Erlaubnis erteilt, uneingeschränkt zu benutzen, inklusive und ohne Ausnahme, dem Recht, sie zu verwenden, kopieren, ändern, fusionieren, 
 * verlegen, verbreiten, unterlizenzieren und/oder zu verkaufen, und Personen, die diese Software erhalten, diese Rechte zu geben, unter den 
 * folgenden Bedingungen:
 * 
 * Der obige Urheberrechtsvermerk und dieser Erlaubnisvermerk sind in allen Kopien oder Teilkopien der Software beizulegen.
 * 
 * DIE SOFTWARE WIRD OHNE JEDE AUSDRÜCKLICHE ODER IMPLIZIERTE GARANTIE BEREITGESTELLT, EINSCHLIESSLICH DER GARANTIE ZUR BENUTZUNG FÜR DEN 
 * VORGESEHENEN ODER EINEM BESTIMMTEN ZWECK SOWIE JEGLICHER RECHTSVERLETZUNG, JEDOCH NICHT DARAUF BESCHRÄNKT. 
 * IN KEINEM FALL SIND DIE AUTOREN ODER COPYRIGHTINHABER FÜR JEGLICHEN SCHADEN ODER SONSTIGE ANSPRÜCHE HAFTBAR ZU MACHEN, 
 * OB INFOLGE DER ERFÜLLUNG EINES VERTRAGES, EINES DELIKTES ODER ANDERS IM ZUSAMMENHANG MIT DER SOFTWARE ODER SONSTIGER VERWENDUNG DER SOFTWARE 
 * ENTSTANDEN.
 * 
 * ***************************************************************************************************************************************************
 * Copyright (c) 2012 Michel Krämer
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
 * 
 *****************************************************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Allmystery.ViewModel;
using Allmystery.ViewModel.DataTypes;
using System.Text;
using Allmystery.ViewModel.Extensions;
using System.Windows.Data;
namespace Allmystery
{
    public partial class SingleThreadPage : PhoneApplicationPage
    {
        string scrollelement = "";
        SingleThreadViewModel singlethreadviewmodel;
        ProgressBar progressbar;
        bool postednewone = false;
        string actualtext = "";
        string topicurl = "", title = "";
        int selectedpost = -1, actualpage = 0;
        string index = "";
        public SingleThreadPage()
        {
            InitializeComponent();
            this.singlethreadviewmodel = new SingleThreadViewModel();
            this.progressbar = new ProgressBar();
            this.DataContext = App.ViewModel;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            if (NavigationContext.QueryString.TryGetValue("threadid", out topicurl) && NavigationContext.QueryString.TryGetValue("title", out title))
            {
                this.tbltopic.Text = title;
                if (this.topicurl.Contains('|'))
                {
                    string[] scrollto = this.topicurl.Split('|');
                    this.topicurl = scrollto[0];
                    this.scrollelement = scrollto[1];
                    
                }
                if (this.topicurl.Contains('-'))
                {
                    string[] page = this.topicurl.Split('-');
                    this.topicurl = page[0];
                    this.actualpage = Convert.ToInt32(page[1]);
                }
                string thread = "";
                App.ViewModel.settingsViewModel.getSettings();
                if (this.actualpage == 0 && App.ViewModel.settingsViewModel.Marker && this.scrollelement == "")
                    thread = "?at_marker=1";
                this.openprogressbar();
                this.singlethreadviewmodel.gotPosts = this.gotposts;
                this.singlethreadviewmodel.getPosts(this.actualpage, this.topicurl + thread);
            }
        }

        private void gotposts(bool result = true)
        {
           
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                if (result)
                {
                    HTMLViewModel htmlviewmodel = new HTMLViewModel();
                    this.tblpage.Text = (this.singlethreadviewmodel.actualPage + 1).ToString() + "/" + this.singlethreadviewmodel.threadPages.ToString();
                    string backgroundcolor = "";
                    string posts = "", singlepost = "", lastpostid = "";
                    string content = "";
                    if (this.tbltopic.Text == "<-")
                        this.tbltopic.Text = this.singlethreadviewmodel.Title;
                    int i = 0;
                    foreach (Post post in this.singlethreadviewmodel.Posts)
                    {
                        if (i % 2 != 0)
                            backgroundcolor = "#2B2B2B";
                        else
                            backgroundcolor = "#404040";
                        singlepost = htmlviewmodel.postBox;
                        singlepost = singlepost.Replace("{0}", backgroundcolor);
                        singlepost = singlepost.Replace("{1}", post.Pic);
                        singlepost = singlepost.Replace("{2}", post.Usercolor);
                        singlepost = singlepost.Replace("{3}", post.Username);
                        if (this.singlethreadviewmodel.markerDate != "" && post.postID == this.singlethreadviewmodel.markerPostID)
                            singlepost = singlepost.Replace("{4}", this.singlethreadviewmodel.markerDate);
                        else
                            singlepost = singlepost.Replace("{4}", "");
                        singlepost = singlepost.Replace("{5}", post.Date);
                        singlepost = singlepost.Replace("{6}", post.Text);
                        singlepost = singlepost.Replace("{7}", "wp7index=" + i.ToString());
                        singlepost = singlepost.Replace("{8}", post.postID);
                        if (this.postednewone)
                            lastpostid = post.postID;
                        posts += singlepost;
                        i++;
                    }
                    content = htmlviewmodel.threadPage;
                    content = content.Replace("{0}", posts);
                    if (this.scrollelement != "")
                    {

                        string scrollto = htmlviewmodel.scrollTo.Replace("{0}", this.scrollelement);
                        content = content.Replace("{1}", scrollto);
                    }
                    else if (this.singlethreadviewmodel.markerDate != "")
                    {
                        string scrollto = htmlviewmodel.scrollTo.Replace("{0}", "id" + this.singlethreadviewmodel.markerPostID);
                        content = content.Replace("{1}", scrollto);
                    }
                    else if (this.postednewone)
                    {
                        string scrollto = htmlviewmodel.scrollTo.Replace("{0}", "id" + lastpostid);
                        content = content.Replace("{1}", scrollto);
                        this.postednewone = false;
                        lastpostid = "";
                    }
                    else
                        content = content.Replace("{1}", "");
                    this.browser.NavigateToString(content);
                    if (this.scrollelement != "")
                    {
                        this.scrollelement = "";
                    }
                    this.ApplicationTitle.Text = App.ViewModel.Status;                   
                    this.closeprogressbar();
                    if (this.index == "")
                        if (NavigationContext.QueryString.TryGetValue("index", out index))
                        {
                            if (App.ViewModel.templateViewModel.Templates.Count > Convert.ToInt32(index))
                            {
                                this.actualtext = App.ViewModel.templateViewModel.Templates[Convert.ToInt32(index)].Text;
                                this.abbnew_Click(this, null);
                            }
                        }
                }
                else
                {
                    this.closeprogressbar();
                    MessageBox.Show(string.Format("{0}", Application.Current.Resources["lngConnectionError"].ToString()), Application.Current.Resources["lngConnectionErrorTitle"].ToString(), MessageBoxButton.OK);
                }
            });            
        }

        private void OnFlick(object sender, FlickGestureEventArgs e)
        {
            /*if (e.Direction == System.Windows.Controls.Orientation.Horizontal)
            {
                this.openprogressbar();
                if (e.HorizontalVelocity.CompareTo(0.0) < 0)
                {
                    this.singlethreadviewmodel.gotPosts = this.gotposts;
                    this.singlethreadviewmodel.getPosts(1, this.topicurl);
                }
                else if (e.HorizontalVelocity.CompareTo(0.0) > 0)
                {
                    this.singlethreadviewmodel.gotPosts = this.gotposts;
                    this.singlethreadviewmodel.getPosts(-1, this.topicurl);
                }
            }*/
        }

        private void abbbrowser_Click(object sender, EventArgs e)
        {
            WebBrowserTask browser = new WebBrowserTask();
            browser.Uri = new Uri(topicurl);
            browser.Show();
            browser = null;
        }

        private void abbfirstpage_Click(object sender, EventArgs e)
        {
            this.openprogressbar();
            this.singlethreadviewmodel.gotPosts = this.gotposts;
            this.singlethreadviewmodel.getPosts(-1, this.topicurl);
        }

        private void abblastpage_Click(object sender, EventArgs e)
        {
            this.openprogressbar();
            this.singlethreadviewmodel.gotPosts = this.gotposts;
            this.singlethreadviewmodel.getPosts(1, this.topicurl);
        }

        private void abbnew_Click(object sender, EventArgs e)
        {
            this.postednewone = true;
            if (this.actualtext == "")
                if (App.ViewModel.templateViewModel.Templates.Where(t => t.threadID == this.topicurl).Count() > 0)
                    this.actualtext = App.ViewModel.templateViewModel.Templates.Where(t => t.threadID == this.topicurl).First().Text;
            NavigationService.Navigate(new Uri("/NewPostPage.xaml?topicurl=" + this.topicurl + "&topic=" + this.tbltopic.Text + "&actualtext=" + this.actualtext, UriKind.Relative));
            this.actualtext = "";
        }

       


        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (this.avatarpopup.IsOpen)
            {
                e.Cancel = true;
                this.butback2_Click(this, null);
            }
            else if (this.gotopagepopup.IsOpen)
            {
                e.Cancel = true;
                this.tbopage.Text = "";
                this.gotopagepopup.IsOpen = false;
                this.IsHitTestVisible = true;
                this.ApplicationBar.Enable();
            }
            else
                base.OnBackKeyPress(e);
        }

        private void butpost_Click(object sender, RoutedEventArgs e)
        {
            this.postednewone = true;
            NavigationService.Navigate(new Uri("/NewPostPage.xaml?topicurl=" + this.topicurl + "&topic=" + this.tbltopic.Text + "&actualtext=" + this.actualtext, UriKind.Relative));
        }
        private void postcreated()
        {
            
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                this.closeprogressbar();

                this.IsHitTestVisible = true;
                this.ApplicationBar.Enable();
                this.abblastpage_Click(this, null);
                if (App.ViewModel.settingsViewModel.autoFavourite)
                {
                    BookmarksViewModel bookmarksviewmodel = new BookmarksViewModel();
                    bookmarksviewmodel.bookmarkAdded = this.bookmarkadded;
                    bookmarksviewmodel.addBookmark(this.singlethreadviewmodel.Thread);
                }
                App.ViewModel.templateViewModel.removeTemplate(new Template(DateTime.Now.ToString(), this.topicurl, this.tbltopic.Text, this.actualtext, ""));
            });
        }

        private void bookmarkadded(bool result = false)
        {
        }

        private void closeprogressbar(bool result = false)
        {
            this.progressbar.IsIndeterminate = false;
            this.progressbar.Visibility = System.Windows.Visibility.Collapsed;
            this.ContentPanel.Children.Remove(this.progressbar);
            this.IsEnabled = true;
            this.IsHitTestVisible = true;
            this.ApplicationBar.Enable();
        }

        private void openprogressbar()
        {
            if (this.progressbar.IsIndeterminate != true)
            {
                this.progressbar.IsIndeterminate = true;
                this.IsHitTestVisible = false;
                this.ApplicationBar.Disable();
                this.progressbar.Visibility = System.Windows.Visibility.Visible;
                try
                {
                    this.ContentPanel.Children.Add(this.progressbar);
                }
                catch
                {

                }
            }
        }

        private void browser_Navigating(object sender, NavigatingEventArgs e)
        {
            WebBrowserTask webbrowsertask = new WebBrowserTask();
            if (e.Uri.AbsoluteUri.ToString().Substring(0, 15) == "about:wp7index=")
            {
                e.Cancel = true;
                int post = Convert.ToInt32(e.Uri.AbsoluteUri.ToString().Replace("about:wp7index=", ""));
                this.selectedpost = post;
                this.avatarpopup.IsOpen = true;
                this.IsHitTestVisible = false;
                this.ApplicationBar.Disable();

            }
            else if (e.Uri.AbsoluteUri.ToString().Substring(0, 6) != "about:")
            {
                e.Cancel = true;
                webbrowsertask.Uri = e.Uri;
                webbrowsertask.Show();
            }
            else if (e.Uri.AbsoluteUri.ToString().Substring(0, 14) == "about:/themen/")
            {
                e.Cancel = true;
                string link = e.Uri.AbsoluteUri.ToString().Replace("about:/themen/", "");
                link = link.Substring(2, link.Length - 2);
                NavigationService.Navigate(new Uri("/SingleThreadPage.xaml?threadid=" + link + "&title=" + "<-", UriKind.Relative));
            }
            else
                e.Cancel = true;
        }

        private void butanswerto_Click(object sender, RoutedEventArgs e)
        {
            
            if (this.selectedpost != -1)
                this.actualtext += "@" + HttpUtility.HtmlDecode(this.singlethreadviewmodel.Posts[this.selectedpost].Username) + "\n";
            this.butback2_Click(this, null);
        }

        private void butspeakto_Click(object sender, RoutedEventArgs e)
        {
            if (this.selectedpost != -1)            
                NavigationService.Navigate(new Uri("/ConversationPage.xaml?conversation=" + this.singlethreadviewmodel.Posts[this.selectedpost].userID + "&username=" + this.singlethreadviewmodel.Posts[this.selectedpost].Username, UriKind.Relative));
            this.butback2_Click(this, null);
        }

        private void butback2_Click(object sender, RoutedEventArgs e)
        {
            this.avatarpopup.IsOpen = false;
            this.IsHitTestVisible = true;
            this.ApplicationBar.Enable();
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            this.openprogressbar();
            this.singlethreadviewmodel.gotPosts = this.gotposts;
            this.singlethreadviewmodel.getPosts(this.actualpage, this.topicurl);
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            this.openprogressbar();
            BookmarksViewModel bookmarksviewmodel = new BookmarksViewModel();
            bookmarksviewmodel.bookmarkAdded = this.closeprogressbar;
            bookmarksviewmodel.addBookmark(this.singlethreadviewmodel.Thread);
        }

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            if (this.Orientation == PageOrientation.PortraitUp || this.Orientation == PageOrientation.PortraitDown)
            {
                this.browser.Width = 480;
                this.browser.Height = 577;
            }
            else if (this.Orientation == PageOrientation.LandscapeLeft || this.Orientation == PageOrientation.LandscapeRight)
            {
                this.browser.Width = 660;
                this.browser.Height = 400;
            }
        }

        private void ApplicationBarMenuItem_Click_1(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarMenuItem_Click_2(object sender, EventArgs e)
        {
            this.openprogressbar();
            this.singlethreadviewmodel.gotPosts = this.gotposts;
            this.singlethreadviewmodel.getPosts(-this.singlethreadviewmodel.actualPage, this.topicurl);           
        }

        private void ApplicationBarMenuItem_Click_3(object sender, EventArgs e)
        {
            this.openprogressbar();
            this.singlethreadviewmodel.gotPosts = this.gotposts;
            this.singlethreadviewmodel.getPosts(this.singlethreadviewmodel.threadPages, this.topicurl);
        }

        private void ApplicationBarMenuItem_Click_4(object sender, EventArgs e)
        {
            this.gotopagepopup.IsOpen = true;
            this.IsHitTestVisible = false;
            this.ApplicationBar.Disable();
        }

        private void butgoto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int page = Convert.ToInt32(this.tbopage.Text);
                page -= 1;
                this.openprogressbar();
                this.singlethreadviewmodel.gotPosts = this.gotposts;
                this.singlethreadviewmodel.getPosts(page, this.topicurl, true);
            }
            catch
            {

            }
            finally
            {
                this.tbopage.Text = "";
                this.gotopagepopup.IsOpen = false;
                this.IsHitTestVisible = true;
                this.ApplicationBar.Enable();
            }
        }

        private void butprofile_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask webbrowsertask = new WebBrowserTask();
            webbrowsertask.Uri = new Uri(string.Format("http://www.allmystery.de/fcgi/?m=profile&inAppRequest=1&name={0}", this.singlethreadviewmodel.Posts[this.selectedpost].Username));
            webbrowsertask.Show();
        }
    }
}