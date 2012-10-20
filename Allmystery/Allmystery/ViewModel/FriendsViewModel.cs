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
using System.Linq;
using Allmystery.Model;
using Allmystery.ViewModel.DataTypes;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace Allmystery.ViewModel
{
    /// <summary>
    /// Diese Klasse ermöglicht den Zugriff, auf die in Allmystery als Freunde hinterlegten User.
    /// </summary>
    public class FriendsViewModel
    {
        public delegate void gotFriendsCallback(bool result);
        public gotFriendsCallback gotmyfriends;
        private GetHttp gethttp;
        private ObservableCollection<Friend> friends;
        public ObservableCollection<Friend> Friends
        {
            set
            {
                this.friends = value;
            }
            get
            {
                return this.friends;
            }
        }
        /// <summary>
        /// Dies ist der Konstruktor der Klasse.
        /// </summary>
        public FriendsViewModel()
        {
            this.gethttp = new GetHttp();
            this.friends = new ObservableCollection<Friend>();
        }
        /// <summary>
        /// Mit dieser Methode wird der Abruf der hinterlegten Freunde angestoßen.
        /// </summary>
        public void getFriends()
        {
            this.gethttp.gotResponse = this.gotfriends;
            this.gethttp.doHttpGET("/friends/");
        }
        /// <summary>
        /// Diese Methode verarbeitet die angeforderten hinterlegten Freunde und bereitet sie für die Anzeige auf.
        /// </summary>
        /// <param name="cats">Enthält die Freunde im JSON-Format, andernfalls NULL.</param>
        private void gotfriends(JObject fr)
        {
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                ObservableCollection<Friend> temp = new ObservableCollection<Friend>();
                this.friends.Clear();
                if (fr != null)
                {
                    JArray jarray = (JArray)fr["users"];
                    foreach (JObject jobject in jarray)
                    {
                        Friend friend = new Friend();
                        friend.Username = (string)jobject["username"];
                        friend.Busy = Convert.ToString(jobject["busy"]);
                        friend.Pic = (string)jobject["pic"];
                        friend.statusMsg = (string)jobject["status_msg"];
                        friend.Online = Convert.ToString(jobject["online"]);
                        if (friend.Online == "True")
                        {
                            friend.Online = "on";
                            friend.fontColor = "Green";
                        }
                        else
                        {
                            friend.Online = "off";
                            friend.fontColor = "Red";
                        }
                        friend.userID = Convert.ToString(jobject["userid"]);
                        friend.Mobile = Convert.ToString(jobject["mobile"]);
                        temp.Add(friend);
                    }

                    int i = 0;
                    foreach (Friend friend in temp.OrderByDescending(t => t.Online))
                    {
                        if (i % 2 == 0)
                            friend.Color = "#2B2B2B";
                        else
                            friend.Color = "#404040";
                        this.Friends.Add(friend);
                        i++;
                    }
                    string messages = "", bookmarks = "";
                    JObject userdata = (JObject)fr["userdata"];
                    messages = Convert.ToString(userdata["unread_messages"]);
                    bookmarks = Convert.ToString(userdata["unread_bookmarks"]);
                    App.ViewModel.unreadMessages = messages;
                    App.ViewModel.unreadBookmarks = bookmarks;
                    App.ViewModel.Status = string.Format("{0}: {1}, {2}: {3}", Application.Current.Resources["lngUnreadMessages"].ToString(), messages, Application.Current.Resources["lngUnreadBookmarks"].ToString(), bookmarks);
                    this.gotmyfriends(true);
                }
                else
                    this.gotmyfriends(false);
            });
        }       
    }
}
