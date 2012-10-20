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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Allmystery.ViewModel.DataTypes;
using Allmystery.Model;
using Newtonsoft.Json.Linq;


namespace Allmystery.ViewModel
{
    public class UserViewModel
    {
        public delegate void gotUsersCallback(bool result);
        public gotUsersCallback gotUsers;
        private GetHttp gethttp;
        private ObservableCollection<User> users;
        public ObservableCollection<User> Users
        {
            set
            {
                this.users = value;
            }
            get
            {
                return this.users;
            }
        }
        public UserViewModel()
        {
            this.gethttp = new GetHttp();
            this.users = new ObservableCollection<User>();
        }

        public void getOnlineUsers()
        {
            this.gethttp.gotResponse = this.gotusers;
            this.gethttp.doHttpGET("/online_users");
        }

        public void findUser(string user)
        {
            this.gethttp.gotResponse = this.gotusers;
            this.gethttp.doHttpGET("/search_users/" + user);
        }

        private void gotusers(JObject us)
        {
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                ObservableCollection<Friend> temp = new ObservableCollection<Friend>();
                this.users.Clear();
                if (us != null)
                {
                    int i = 0;
                    JArray jarray = (JArray)us["users"];
                    foreach (JObject jobject in jarray)
                    {
                        User user = new User();
                        user.Username = (string)jobject["username"];
                        if (jobject["pic"] != null)
                            user.Pic = (string)jobject["pic"];
                        if (i % 2 == 0)
                            user.backgroundColor = "#2B2B2B";
                        else
                            user.backgroundColor = "#404040";
                        user.userID = Convert.ToString(jobject["userid"]);
                        users.Add(user);
                        i++;
                    }

                    string messages = "", bookmarks = "";
                    JObject userdata = (JObject)us["userdata"];
                    messages = Convert.ToString(userdata["unread_messages"]);
                    bookmarks = Convert.ToString(userdata["unread_bookmarks"]);
                    App.ViewModel.unreadMessages = messages;
                    App.ViewModel.unreadBookmarks = bookmarks;
                    App.ViewModel.Status = string.Format("{0}: {1}, {2}: {3}", Application.Current.Resources["lngUnreadMessages"].ToString(), messages, Application.Current.Resources["lngUnreadBookmarks"].ToString(), bookmarks);
                    this.gotUsers(true);
                }
                else
                    this.gotUsers(false);
            });
        }
    }
}
