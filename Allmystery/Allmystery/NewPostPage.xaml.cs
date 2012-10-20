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
using Allmystery.ViewModel;
using Allmystery.ViewModel.DataTypes;
using System.Text;
using Allmystery.ViewModel.Extensions;

namespace Allmystery
{
    public partial class NewPostPage : PhoneApplicationPage
    {
        string actualtext = "", topicurl = "", topic = "";
        SingleThreadViewModel singlethreadviewmodel;
        ProgressBar progressbar;
        public NewPostPage()
        {
            InitializeComponent();
            this.singlethreadviewmodel = new SingleThreadViewModel();
            this.progressbar = new ProgressBar();
            this.DataContext = App.ViewModel;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (NavigationContext.QueryString.TryGetValue("topicurl", out this.topicurl) && NavigationContext.QueryString.TryGetValue("topic", out this.topic)
                && NavigationContext.QueryString.TryGetValue("actualtext", out this.actualtext))
            {
                if (this.tbonewpost.Text == "")
                {
                    this.tbonewpost.Text = this.actualtext;
                    this.tbonewpost.Text = this.tbonewpost.Text.Replace(">!n!<", "\n").Replace(">!r!<", "\r");
                }
            }
        }

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            if (this.Orientation == PageOrientation.PortraitUp || this.Orientation == PageOrientation.PortraitDown)
            {
                this.tbonewpost.Height = 612;
                this.tbonewpost.Width = 480;
            }
            else if (this.Orientation == PageOrientation.LandscapeLeft || this.Orientation == PageOrientation.LandscapeRight)
            {
                this.tbonewpost.Height = 400;
                this.tbonewpost.Width = 644;
            }
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            this.openprogressbar();
            this.IsHitTestVisible = false;
            this.ApplicationBar.IsMenuEnabled = false;
            if (this.actualtext != "")
                App.ViewModel.templateViewModel.addTemplate(new Template(DateTime.Now.ToString(), this.topicurl, this.topic, this.actualtext, ""));
            this.singlethreadviewmodel.postCreated = this.postcreated;
            this.singlethreadviewmodel.Thread = this.topicurl;
            this.singlethreadviewmodel.createPost(Convert.ToString(this.tbonewpost.Text));
        }

        private void postcreated(bool result = false)
        {
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                if (result)
                {
                    if (App.ViewModel.settingsViewModel.autoFavourite)
                    {
                        BookmarksViewModel bookmarksviewmodel = new BookmarksViewModel();
                        bookmarksviewmodel.bookmarkAdded = this.bookmarkadded;
                        bookmarksviewmodel.addBookmark(this.singlethreadviewmodel.Thread);
                    }
                    if (App.ViewModel.templateViewModel.Templates.Where(t => t.threadID == this.topicurl).Count() > 0)
                        App.ViewModel.templateViewModel.removeTemplate(App.ViewModel.templateViewModel.Templates.Where(t => t.threadID == this.topicurl).First());
                    this.IsHitTestVisible = true;
                    this.ApplicationBar.IsMenuEnabled = true;
                    this.closeprogressbar();
                    NavigationService.GoBack();
                }
                else
                {
                    this.closeprogressbar();
                    MessageBox.Show(Application.Current.Resources["lngFailPostPost"].ToString(), Application.Current.Resources["lngFailPostPostTitle"].ToString(), MessageBoxButton.OK);
                }
            });
        }
        private void closeprogressbar(bool result = false)
        {
            this.progressbar.IsIndeterminate = false;
            this.progressbar.Visibility = System.Windows.Visibility.Collapsed;
            this.ContentPanel.Children.Remove(this.progressbar);
            this.IsEnabled = true;
            this.IsHitTestVisible = true;
            this.tbonewpost.IsEnabled = true;
            this.ApplicationBar.Enable();
        }

        private void openprogressbar()
        {
            if (this.progressbar.IsIndeterminate != true)
            {
                this.progressbar.IsIndeterminate = true;
                this.IsHitTestVisible = false;
                this.tbonewpost.IsEnabled = false;
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

        private void bookmarkadded(bool result = false)
        {
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            this.Focus();
        }

        private void tbonewpost_LostFocus(object sender, RoutedEventArgs e)
        {
            this.actualtext = this.tbonewpost.Text;
            if (this.actualtext != "")
                App.ViewModel.templateViewModel.addTemplate(new Template(DateTime.Now.ToString(), this.topicurl, this.topic, this.actualtext, ""));
        }
    }
}