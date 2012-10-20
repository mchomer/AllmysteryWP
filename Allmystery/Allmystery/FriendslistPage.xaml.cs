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
using Allmystery.ViewModel.Extensions;

namespace Allmystery
{
    public partial class FriendslistPage : PhoneApplicationPage
    {
        private ProgressBar progressbar;
        private FriendsViewModel friendsviewmodel;
        public FriendslistPage()
        {
            InitializeComponent();
            this.friendsviewmodel = new FriendsViewModel();
            this.progressbar = new ProgressBar();
            this.lbofriends.ItemsSource = this.friendsviewmodel.Friends;
            this.DataContext = App.ViewModel;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.ApplicationBarIconButton_Click(this, null);
        }

        private void gotmyfriends(bool result = true)
        {
            Dispatcher.BeginInvoke(delegate
            {
                this.closeprogressbar();
                if (result == false)
                    MessageBox.Show(string.Format("{0}", Application.Current.Resources["lngConnectionError"].ToString()), Application.Current.Resources["lngConnectionErrorTitle"].ToString(), MessageBoxButton.OK);
            });
        }

        private void closeprogressbar()
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

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            this.openprogressbar();
            this.friendsviewmodel.gotmyfriends = this.gotmyfriends;
            this.friendsviewmodel.getFriends();
        }

        private void lbofriends_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.lbofriends.SelectedIndex != -1)
            {
                NavigationService.Navigate(new Uri("/ConversationPage.xaml?conversation=" + this.friendsviewmodel.Friends[this.lbofriends.SelectedIndex].userID + "&username=" + this.friendsviewmodel.Friends[this.lbofriends.SelectedIndex].Username, UriKind.Relative));
            }
        }

        private void ApplicationTitle_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MyConversationsPage.xaml", UriKind.Relative));
        }

    }
}