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
using Allmystery.Model;
using Allmystery.ViewModel.DataTypes;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;

namespace Allmystery.ViewModel
{
    public class SingleConversationViewModel : BaseViewModel 
    {
        public delegate void gotsingleconversationCallback(bool result);
        public delegate void messagepostedCallback(JObject response);
        public delegate void friendsAction(bool result);
        public friendsAction gotfriendsAction;
        public gotsingleconversationCallback gotSingleConversations;
        public messagepostedCallback messagePosted;
        private GetHttp gethttp;
        private HttpPOSTModel httppostmodel;
        private string conversation;
        public string chatToken { get; set; }
        private bool isfriend = false;
        public bool isFriend
        {
            get
            {
                return this.isfriend;
            }
        }
        private ObservableCollection<SingleConversation> singleconversation;
        public ObservableCollection<SingleConversation> SingleConversation
        {
            set
            {
                this.singleconversation = value;
                this.OnPropertyChanged("SingleConversation");
            }
            get
            {
                return this.singleconversation;
            }
        }
        public SingleConversationViewModel()
        {
            this.gethttp = new GetHttp();
            this.httppostmodel = new HttpPOSTModel();
            this.singleconversation = new ObservableCollection<SingleConversation>();
        }

        public void addFriend(string userid)
        {
            httppostmodel.postCompleted = this.friendaction;
            httppostmodel.dopost("/friends/", "add_friend=" + userid);
        }

        public void removeFriend(string userid)
        {
            httppostmodel.postCompleted = this.friendaction;
            httppostmodel.dopost("/friends/", "remove_friend=" + userid);
        }

        public void friendaction(JObject response)
        {
            bool success = false;
            if (response != null)
                success = (bool)response["success"];
            if (success)
                this.gotfriendsAction(true);
            else
                this.gotfriendsAction(false);
        }
        public void postMessage(string message)
        {
            httppostmodel.postCompleted = this.messageposted;
            httppostmodel.dopost("/conversation/" + this.conversation, message);
        }

        private void messageposted(JObject response)
        {
            bool success = false;
            if (response != null)
            {
                success = true;           
            }
            if (success)
            {
                this.getSingleConversation(this.conversation);
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    MessageBox.Show(Application.Current.Resources["lngFailPostMessage"].ToString(), Application.Current.Resources["lngFailPostMessageTitle"].ToString(), MessageBoxButton.OK);
                });
            }
        }

        public void getSingleConversation(string conversation)
        {
            this.conversation = conversation;
            this.gethttp.gotResponse = this.gotsingleconversation;
            this.gethttp.doHttpGET("/conversation/" + conversation);
        }

        private void gotsingleconversation(JObject con)
        {
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                this.SingleConversation.Clear();
                if (con != null)
                {
                    
                    this.chatToken = (string)con["chat_token"];
                    string[] values = this.chatToken.Split('\'');
                    this.chatToken = values[0];
                    JArray jarray = (JArray)con["messages"];
                    if (jarray != null)
                        foreach (JObject jobject in jarray)
                        {
                            SingleConversation conversation = new SingleConversation();
                            conversation.Username = (string)jobject["username"];
                            conversation.userID = Convert.ToString(jobject["userid"]);
                            conversation.Pic = (string)jobject["pic"];
                            conversation.Date = (string)jobject["date"];
                            TimeZoneInfo timezoneinfo = TimeZoneInfo.Local;
                            DateTime datetime = DateTime.Parse(conversation.Date);
                            datetime = TimeZoneInfo.ConvertTime(datetime, timezoneinfo);
                            conversation.Date = String.Format("{0:f}", datetime); 
                            conversation.messageID = Convert.ToString(jobject["message_id"]);
                            conversation.Message = (string)jobject["message"];
                            this.SingleConversation.Add(conversation);

                        }
                    string messages = "", bookmarks = "";
                    this.isfriend = (bool)con["is_friend"];
                    JObject userdata = (JObject)con["userdata"];
                    messages = Convert.ToString(userdata["unread_messages"]);
                    bookmarks = Convert.ToString(userdata["unread_bookmarks"]);
                    App.ViewModel.unreadMessages = messages;
                    App.ViewModel.unreadBookmarks = bookmarks;
                    App.ViewModel.Status = string.Format("{0}: {1}, {2}: {3}", Application.Current.Resources["lngUnreadMessages"].ToString(), messages, Application.Current.Resources["lngUnreadBookmarks"].ToString(), bookmarks);
                    this.gotSingleConversations(true);
                }
                else
                    this.gotSingleConversations(false);
            });
        }
    }
}
