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
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Text;
using Allmystery.ViewModel;
using Allmystery.ViewModel.DataTypes;
using Allmystery.ViewModel.Extensions;
using Microsoft.Phone.Shell;
namespace Allmystery
{
    public partial class ConversationPage : PhoneApplicationPage
    {
        ShellTile tile = ShellTile.ActiveTiles.First();
        string conversation = "";
        string username = "";
        bool focus = false;
        int offset = 0;
        ProgressBar progressbar;
        bool newmessage = false;
        SingleConversationViewModel singleconversationviewmodel;
        public ConversationPage()
        {
            InitializeComponent();
            this.singleconversationviewmodel = new SingleConversationViewModel();
            this.progressbar = new ProgressBar();
            this.DataContext = App.ViewModel;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.TryGetValue("conversation", out this.conversation) && NavigationContext.QueryString.TryGetValue("username", out this.username))
            {

                this.PageTitle.Text = username;
                App.ViewModel.pushConversation = this.conversation;
                App.ViewModel.refreshConversation = this.refreshpush;
                this.openprogressbar();
                this.singleconversationviewmodel.gotSingleConversations = this.gotsingleconversation;
                this.singleconversationviewmodel.getSingleConversation(this.conversation);
            }
        }

        private void refreshpush()
        {
            Dispatcher.BeginInvoke(delegate
            {
                this.singleconversationviewmodel.gotSingleConversations = this.gotsingleconversation;
                this.singleconversationviewmodel.getSingleConversation(this.conversation);
            });
        }

        private void gotsingleconversation(bool result = true)
        {
            
            Dispatcher.BeginInvoke(delegate
            {
                if (result)
                {
                    if (this.newmessage)
                    {
                        this.tbomessage.Text = "";
                        this.newmessage = false;
                        this.Focus();
                    }
                    HTMLViewModel htmlviewmodel = new HTMLViewModel();
                    string content = "", message = "", messages = "", scrollid = "";
                    foreach (SingleConversation con in this.singleconversationviewmodel.SingleConversation)
                    {
                        if (con.Username != App.ViewModel.settingsViewModel.userName)
                            message = htmlviewmodel.meBubble;
                        else
                            message = htmlviewmodel.youBubble;
                        message = message.Replace("{0}", con.Message);
                        message = message.Replace("{1}", con.Date);
                        message = message.Replace("{2}", con.Username);
                        message = message.Replace("{3}", "id" + con.messageID);
                        message = message.Replace("{4}", con.Pic);
                        scrollid = "id" + con.messageID;
                        messages += message;
                    }
                    content = htmlviewmodel.conversationPage;
                    string scrollto = htmlviewmodel.scrollTo.Replace("{0}", scrollid);
                    content = content.Replace("{0}", messages);
                    if (scrollid != "")
                        content = content.Replace("{1}", scrollto);
                    else
                        content = content.Replace("{1}", "");
                    this.browser.NavigateToString(content);
                    if (this.singleconversationviewmodel.isFriend)
                    {
                        ((ApplicationBarMenuItem)ApplicationBar.MenuItems[0]).IsEnabled = true;
                        ((ApplicationBarMenuItem)ApplicationBar.MenuItems[1]).IsEnabled = false;
                    }
                    else
                    {
                        ((ApplicationBarMenuItem)ApplicationBar.MenuItems[0]).IsEnabled = false;
                        ((ApplicationBarMenuItem)ApplicationBar.MenuItems[1]).IsEnabled = true;
                    }
                    this.closeprogressbar();
                }
                else
                {
                    this.closeprogressbar();
                    MessageBox.Show(string.Format("{0}", Application.Current.Resources["lngConnectionError"].ToString()), Application.Current.Resources["lngConnectionErrorTitle"].ToString(), MessageBoxButton.OK);
                }
            });
        }

        private void closeprogressbar()
        {
            this.progressbar.IsIndeterminate = false;
            this.progressbar.Visibility = System.Windows.Visibility.Collapsed;
            this.ContentPanel.Children.Remove(this.progressbar);
            this.IsEnabled = true;
            this.IsHitTestVisible = true;
            this.tbomessage.IsEnabled = true;
            this.ApplicationBar.Enable();
            try
            {
                this.tile.Update(new StandardTileData
                {
                    BackContent = string.Format("{0} Nachrichten", App.ViewModel.unreadMessages),
                    Count = Convert.ToInt32(App.ViewModel.unreadMessages),
                    BackBackgroundImage = null
                });
            }
            catch
            {
                // Bei genügend Lust wird hier noch ein Error-Logging  via Library eingefügt.
            }
        }

        private void openprogressbar()
        {
            if (this.progressbar.IsIndeterminate != true)
            {
                this.tbomessage.IsEnabled = false;
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

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            this.openprogressbar();
            this.newmessage = true;
            this.singleconversationviewmodel.postMessage("message=" + tbomessage.Text);
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            this.openprogressbar();
            this.singleconversationviewmodel.gotSingleConversations = this.gotsingleconversation;
            this.singleconversationviewmodel.getSingleConversation(this.conversation);
        }

        private void browser_Navigating(object sender, NavigatingEventArgs e)
        {
            WebBrowserTask webbrowsertask = new WebBrowserTask();
            if (e.Uri.AbsoluteUri.ToString().Substring(0, 6) != "about:")
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
                link = link.Replace("#", "|");
                NavigationService.Navigate(new Uri("/SingleThreadPage.xaml?threadid=" + link + "&title=" + "<-", UriKind.Relative));
            }
            else
                e.Cancel = true;          
        }

        private void tbomessage_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.Orientation == PageOrientation.PortraitUp || this.Orientation == PageOrientation.PortraitDown)
            {
                this.browser.Width = 456;
                this.browser.Height = 402;
                this.tbomessage.Height = 144;
                this.tbomessage.Width = 456;
            }
            else if (this.Orientation == PageOrientation.LandscapeLeft || this.Orientation == PageOrientation.LandscapeRight)
            {
                this.browser.Width = 660;
                this.browser.Height = 72;
                this.tbomessage.Height = 120;
                this.tbomessage.Width = 650;
            }

            this.focus = true;
        }

        private void tbomessage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            if (!focus)
                this.tbomessage_LostFocus(this, null);
            else
                this.tbomessage_GotFocus(this, null);
        }

        private void tbomessage_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.Orientation == PageOrientation.PortraitUp || this.Orientation == PageOrientation.PortraitDown)
            {

                this.browser.Width = 456;
                this.browser.Height = 454;
                this.tbomessage.Height = 72;
                this.tbomessage.Width = 456;
            }
            else if (this.Orientation == PageOrientation.LandscapeLeft || this.Orientation == PageOrientation.LandscapeRight)
            {
                this.browser.Width = 660;
                this.browser.Height = 190;
                this.tbomessage.Height = 72;
                this.tbomessage.Width = 650;
            }

            this.focus = false;
        }

        private void ApplicationTitle_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MyConversationsPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click_3(object sender, EventArgs e)
        {
            if (this.offset > 0)
                this.offset -= 15;
            this.openprogressbar();
            this.singleconversationviewmodel.gotSingleConversations = this.gotsingleconversation;
            this.singleconversationviewmodel.getSingleConversation(this.conversation + string.Format("?offset={0}", this.offset.ToString()));
        }

        private void ApplicationBarIconButton_Click_2(object sender, EventArgs e)
        {
            this.offset += 15;
            this.openprogressbar();
            this.singleconversationviewmodel.gotSingleConversations = this.gotsingleconversation;
            this.singleconversationviewmodel.getSingleConversation(this.conversation + string.Format("?offset={0}", this.offset.ToString()));
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            this.openprogressbar();
            this.singleconversationviewmodel.gotfriendsAction = this.gotremovefriendsaction;
            this.singleconversationviewmodel.removeFriend(this.conversation);
        }

        private void gotremovefriendsaction(bool result)
        {
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                this.closeprogressbar();
                if (result)
                {
                    MessageBox.Show(Application.Current.Resources["lngRemoveFriendSuccess"].ToString(), Application.Current.Resources["lngRemoveFriendSuccessTitle"].ToString(), MessageBoxButton.OK);
                    ((ApplicationBarMenuItem)ApplicationBar.MenuItems[0]).IsEnabled = false;
                    ((ApplicationBarMenuItem)ApplicationBar.MenuItems[1]).IsEnabled = true;
                }
                else
                    MessageBox.Show(Application.Current.Resources["lngRemoveFriendFail"].ToString(), Application.Current.Resources["lngRemoveFriendFailTitle"].ToString(), MessageBoxButton.OK);
            });
        }

        private void ApplicationBarMenuItem_Click_1(object sender, EventArgs e)
        {
            this.openprogressbar();
            this.singleconversationviewmodel.gotfriendsAction = this.gotaddfriendsaction;
            this.singleconversationviewmodel.addFriend(this.conversation);
        }
        private void gotaddfriendsaction(bool result)
        {
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                this.closeprogressbar();
                if (result)
                {
                    MessageBox.Show(Application.Current.Resources["lngAddFriendSuccess"].ToString(), Application.Current.Resources["lngAddFriendSuccessTitle"].ToString(), MessageBoxButton.OK);
                    ((ApplicationBarMenuItem)ApplicationBar.MenuItems[0]).IsEnabled = true;
                    ((ApplicationBarMenuItem)ApplicationBar.MenuItems[1]).IsEnabled = false;
                }
                else
                    MessageBox.Show(Application.Current.Resources["lngAddFriendFail"].ToString(), Application.Current.Resources["lngAddFriendFailTitle"].ToString(), MessageBoxButton.OK);
            });
        }

        private void ApplicationBarMenuItem_Click_2(object sender, EventArgs e)
        {
            App.ViewModel.pushConversation = "";
            App.ViewModel.refreshConversation = null;
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarMenuItem_Click_3(object sender, EventArgs e)
        {
            WebBrowserTask webbrowsertask = new WebBrowserTask();
            webbrowsertask.Uri = new Uri(string.Format("http://www.allmystery.de/fcgi/?m=profile&inAppRequest=1&name={0}", this.username));
            webbrowsertask.Show();
        }        
    }
}