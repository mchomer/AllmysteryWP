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
using Allmystery.Model;
using Allmystery.ViewModel.DataTypes;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
namespace Allmystery.ViewModel
{
    /// <summary>
    /// Diese Klasse ermöglicht die Interaktion mit beobachteten Diskussionen.
    /// </summary>
    public class BookmarksViewModel
    {
        public delegate void gotbookmarksCallback(bool result);
        public gotbookmarksCallback gotBookmarks;
        public gotbookmarksCallback bookmarkAdded;
        private GetHttp gethttp;
        private HttpPOSTModel httppostmodel;    
        private ObservableCollection<Thread> threads;
        public ObservableCollection<Thread> Threads
        {
            set
            {
                this.threads = value;
            }
            get
            {
                return this.threads;
            }
        }
        /// <summary>
        /// Dies ist der Konstruktor der Klasse.
        /// </summary>
        public BookmarksViewModel()
        {
            this.gethttp = new GetHttp();
            this.threads = new ObservableCollection<Thread>();
            this.httppostmodel = new HttpPOSTModel();
        }
        /// <summary>
        /// Mit Hilfe dieser Methode wird ein Thread als Lesezeichen hinzugefügt.
        /// </summary>
        /// <param name="thread">Thread-ID des Threads, dass als Lesezeichen angelegt wird.</param>
        public void addBookmark(string thread)
        {
            httppostmodel.postCompleted = this.bookmarkadded;
            httppostmodel.dopost("/bookmarks/", "add_thread=" + thread);
        }
        /// <summary>
        /// Diese Methode wird ausgeführt, wenn das Lesezeichen hinzugefügt wurde.
        /// </summary>
        /// <param name="response">Status, ob das Lesezeichen erfolgreich hinzugefügt wurde.</param>
        private void bookmarkadded(JObject response)
        {
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                bool success = false;
                if (response != null)
                    success = (bool)response["success"];
                if (success)
                {
                    if (App.ViewModel.settingsViewModel.autoFavourite == false)
                        MessageBox.Show(Application.Current.Resources["lngBookmarkAdded"].ToString(), Application.Current.Resources["lngBookmarkAddedTitle"].ToString(), MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show(Application.Current.Resources["lngBookmarkAddedError"].ToString(), Application.Current.Resources["lngBookmarkAddedErrorTitle"].ToString(), MessageBoxButton.OK);
                }
                this.bookmarkAdded(false);
            });
        }
        /// <summary>
        /// Mit dieser Methode lässt sich ein Lesezeichen entfernen.
        /// </summary>
        /// <param name="thread">Thread-ID des Threads, dass als Lesezeichen entfernt werden soll.</param>
        public void deleteBookmark(string thread)
        {
            httppostmodel.postCompleted = this.bookmarkdeleted;
            httppostmodel.dopost("/bookmarks/", "remove_thread=" + thread);
        }
        /// <summary>
        /// Diese Methode wird ausgeführt, wenn das Lesezeichen entfernt wurde.
        /// </summary>
        /// <param name="response">Status, ob das Lesezeichen erfolgreich entfernt wurde.</param>
        private void bookmarkdeleted(JObject response)
        {
            bool success = false;
            if (response != null)
                success = (bool)response["success"];
            if (success)
            {
                this.getBookmarks();
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    MessageBox.Show(Application.Current.Resources["lngDeleteError"].ToString(), Application.Current.Resources["lngDeleteErrorTitle"].ToString(), MessageBoxButton.OK);
                });
            }
        }
        /// <summary>
        /// Mit Hilfe dieser Methode können gespeicherte Lesezeichen aufgerufen werden.
        /// </summary>
        /// <param name="offset">Keine Angabe, lädt die ersten 15 Lesezeichen. Bei Angabe eines Parameters in 15ner Schritten, werden die jeweils nächsten 15 Lesezeichen geladen.</param>
        public void getBookmarks(string offset = "")
        {
            this.gethttp.gotResponse = this.gotbookmarks;
            this.gethttp.doHttpGET("/category/bookmarks" + offset);
        }
        /// <summary>
        /// Diese Methode wird ausgeführt, wenn die Lesezeichen aufgerufen wurden.
        /// </summary>
        /// <param name="response">Status, ob das Lesezeichen erfolgreich aufgerufen werden konnten.</param>
        private void gotbookmarks(JObject thr)
        {
            Deployment.Current.Dispatcher.BeginInvoke(delegate {
                this.Threads.Clear();
                if (thr != null)
                {
                    int i = 0;
                    JArray jarray = (JArray)thr["threads"];               
                    foreach (JObject jobject in jarray)
                    {
                        Thread thread = new Thread();
                        thread.Title = (string)jobject["title"];
                        thread.threadID = Convert.ToString(jobject["thread_id"]);
                        thread.postsCount = Convert.ToString(jobject["posts_count"]);
                        thread.lastPostDate = (string)jobject["last_post_date"];
                        TimeZoneInfo timezoneinfo = TimeZoneInfo.Local;
                        DateTime datetime = DateTime.Parse(thread.lastPostDate);
                        datetime = TimeZoneInfo.ConvertTime(datetime, timezoneinfo);
                        thread.lastPostDate = String.Format("{0:f}", datetime); 
                        thread.lastPostUsername = Convert.ToString(jobject["last_post_username"]);
                        thread.createdUsername = Convert.ToString(jobject["created_username"]);
                        thread.Pages = Convert.ToString(jobject["pages"]);
                        thread.Category = Convert.ToString(jobject["category"]);
                        thread.Icon = Convert.ToString(jobject["icon"]);
                        thread.Unread = Convert.ToString(jobject["unread"]);
                        if (thread.Unread == "True")
                            thread.Unread = "Bold";
                        else
                            thread.Unread = "";
                        if (i % 2 != 0)
                            thread.backgroundColor = "#2B2B2B";
                        else
                            thread.backgroundColor = "#404040";
                        this.Threads.Add(thread);
                        i++;
                    }
                    string messages = "", bookmarks = "";
                    JObject userdata = (JObject)thr["userdata"];
                    messages = Convert.ToString(userdata["unread_messages"]);
                    bookmarks = Convert.ToString(userdata["unread_bookmarks"]);
                    App.ViewModel.unreadMessages = messages;
                    App.ViewModel.unreadBookmarks = bookmarks;
                    App.ViewModel.Status = string.Format("{0}: {1}, {2}: {3}", Application.Current.Resources["lngUnreadMessages"].ToString(), messages, Application.Current.Resources["lngUnreadBookmarks"].ToString(), bookmarks);
                    this.gotBookmarks(true);
                }
                else 
                    this.gotBookmarks(false);
            });
        }
    }
}
