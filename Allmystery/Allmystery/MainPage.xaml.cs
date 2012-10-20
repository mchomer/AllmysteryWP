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
using System.IO;
using System.IO.IsolatedStorage;
using Allmystery.ViewModel.DataTypes;
using Allmystery.ViewModel;
using Allmystery.ViewModel.Extensions;
using Microsoft.Phone.Tasks;
namespace Allmystery
{
    public partial class MainPage : PhoneApplicationPage
    {
        private ProgressBar progressbar;
        private LoginViewModel loginviewmodel;
        private CategoriesViewModel categoriesviewmodel;
        private FriendsViewModel friendsviewmodel;
        private ConversationsViewModel conversationsviewmodel;
        // Konstruktor
        public MainPage()
        {
            InitializeComponent();
            this.progressbar = new ProgressBar();
            this.loginviewmodel = new LoginViewModel();
            this.categoriesviewmodel = new CategoriesViewModel();
            this.friendsviewmodel = new FriendsViewModel();
            this.conversationsviewmodel = new ConversationsViewModel();
            this.lbosubjects.ItemsSource = this.categoriesviewmodel.Categories;
            this.DataContext = App.ViewModel;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            App.ViewModel.pushConversation = "";
            App.ViewModel.refreshConversation = null;
            this.lbosubjects.SelectedIndex = -1;
            this.openprogressbar();
            if (App.ViewModel.loggedIn == false)
            {               
                IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
                if (!file.FileExists(App.ViewModel.settingsFileName))
                {
                    App.ViewModel.settingsViewModel.getSettings();   
                    this.closeprogressbar();                   
                    var result = MessageBox.Show(Application.Current.Resources["lngFirstStart"].ToString(), Application.Current.Resources["lngFirstStartTitle"].ToString(), MessageBoxButton.OK);
                    if (MessageBoxResult.OK == result)
                    {
                        WebBrowserTask webbrowsertask = new WebBrowserTask();
                        webbrowsertask.Uri = new Uri(App.ViewModel.sslSource, UriKind.Absolute);
                        webbrowsertask.Show();
                    }
                    NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
                    App.ViewModel.startone = true;
                }
                else
                {
                    App.ViewModel.settingsViewModel.getSettings();
                    this.loginviewmodel.loginCompleted = this.getcategories;
                    this.loginviewmodel.logIn();
                }
            }
            else
            {
                this.categoriesviewmodel.gotCategories = this.gotcategories;
                this.categoriesviewmodel.getCategories();
            }
        }

        private void getcategories(bool result = true)
        {
            if (result)
            {
                this.categoriesviewmodel.gotCategories = this.gotcategories;
                this.categoriesviewmodel.getCategories();
            }
            else
                this.gotcategories(false);
        }

        private void gotcategories(bool result = true)
        {
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                if (result)
                {
                    this.butchoosen.Background = null;
                    this.butall.Background = null;
                    this.butlastones.Background = null;
                    this.butnew.Background = null;
                    SolidColorBrush color = new SolidColorBrush(
                                                Color.FromArgb(
                                                    Convert.ToByte("#FF5A5C75".Substring(1, 2), 16),
                                                    Convert.ToByte("#FF5A5C75".Substring(3, 2), 16),
                                                    Convert.ToByte("#FF5A5C75".Substring(5, 2), 16),
                                                    Convert.ToByte("#FF5A5C75".Substring(7, 2), 16)));
                    if (App.ViewModel.settingsViewModel.startTopics == 0)
                        this.butall.Background = color;
                    else if (App.ViewModel.settingsViewModel.startTopics == 1)
                        this.butlastones.Background = color;
                    else if (App.ViewModel.settingsViewModel.startTopics == 2)
                        this.butchoosen.Background = color;
                    
                    App.ViewModel.Offset = "";
                    this.closeprogressbar();
                }
                else
                {
                    this.closeprogressbar();
                    Dispatcher.BeginInvoke(delegate {
                        if (App.ViewModel.startone == false)
                        {
                            App.ViewModel.startone = true;
                            MessageBox.Show(Application.Current.Resources["lngLoginFail"].ToString(), Application.Current.Resources["lngLoginFailTitle"].ToString(), MessageBoxButton.OK);
                            NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
                        }
                        else
                            MessageBox.Show(string.Format("{0}", Application.Current.Resources["lngConnectionError"].ToString()), Application.Current.Resources["lngConnectionErrorTitle"].ToString(), MessageBoxButton.OK);                          
                    });
                }
            });                                    
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void lbosubjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.lbosubjects.SelectedIndex != -1)
            {
                string[] value = { this.categoriesviewmodel.Categories[this.lbosubjects.SelectedIndex].Cat, 
                                     this.categoriesviewmodel.Categories[this.lbosubjects.SelectedIndex].titleShort };                
                NavigationService.Navigate(new Uri("/ThreadsPage.xaml?subjecturl=" + value[0] + "&title=" + value[1], UriKind.Relative));
            }
        }

        private void bibfriends_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/FriendslistPage.xaml", UriKind.Relative));
        }

        private void bibmessages_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MyConversationsPage.xaml", UriKind.Relative));
        }

        private void bibownthreads_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/BookmarksPage.xaml", UriKind.Relative));
        }


        private void butall_Click(object sender, RoutedEventArgs e)
        {
            this.openprogressbar();
            App.ViewModel.settingsViewModel.startTopics = 0;
            this.categoriesviewmodel.gotCategories = this.gotcategories;
            this.categoriesviewmodel.getCategories();
        }

        private void butlastones_Click(object sender, RoutedEventArgs e)
        {
            this.openprogressbar();
            App.ViewModel.settingsViewModel.startTopics = 1;
            this.categoriesviewmodel.gotCategories = this.gotcategories;
            this.categoriesviewmodel.getCategories();
        }

        private void butchoosen_Click(object sender, RoutedEventArgs e)
        {
            this.openprogressbar();
            App.ViewModel.settingsViewModel.startTopics = 2;
            this.categoriesviewmodel.gotCategories = this.gotcategories;
            this.categoriesviewmodel.getCategories();
        }

        private void bibmore_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MorePage.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            if (this.Orientation == PageOrientation.LandscapeLeft || this.Orientation == PageOrientation.LandscapeRight)
            {
                this.lbosubjects.Height = 100;
            }
            else if (this.Orientation == PageOrientation.Portrait)
            {
                this.lbosubjects.Height = 479;
            }

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

        private void ApplicationTitle_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MyConversationsPage.xaml", UriKind.Relative));
        }

        private void butnew_Click(object sender, RoutedEventArgs e)
        {
            this.openprogressbar();
            NavigationService.Navigate(new Uri("/ThreadsPage.xaml?subjecturl=" + "new" + "&title=" + Application.Current.Resources["lngNewTitle"].ToString(), UriKind.Relative));
        }
    }
}