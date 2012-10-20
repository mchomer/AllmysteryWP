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
using System.Text;
using System.Collections.Generic;

namespace Allmystery.ViewModel
{
    public class SingleThreadViewModel
    {
        public delegate void gotpostsCallback(bool result);
        public delegate void postcreatedCallback(bool result);
        public gotpostsCallback gotPosts;
        public postcreatedCallback postCreated;
        private GetHttp gethttp;
        private string markerdate = "";
        public string markerDate
        {
            get
            {
                return this.markerdate;
            }
        }
        private string markerpostid = "";
        public string markerPostID
        {
            get
            {
                return this.markerpostid;
            }
        }
        private string thread;
        public string Thread
        {
            set
            {
                this.thread = value;
            }
            get
            {
                return this.thread;
            }
        }
        private HttpPOSTModel httppostmodel;
        private int threadpages = 0;
        public int threadPages
        {
            
            get
            {
                return this.threadpages;
            }
        }
        private string title;
        public string Title
        {
            get
            {
                return this.title;
            }
        }

        private int actualpage = 0;
        public int actualPage
        {
            get
            {
                return this.actualpage;
            }

        }
        private List<Post> posts;
        public List<Post> Posts
        {
            set
            {
                this.posts = value;
            }
            get
            {
                return this.posts;
            }
        }
        public SingleThreadViewModel()
        {
            this.gethttp = new GetHttp();
            this.httppostmodel = new HttpPOSTModel();
            this.posts = new List<Post>();
            this.thread = "";
        }

        public void createPost(string text)
        {
            if (this.thread != "" && text != "")
            {
                text = "text=" + text;
                this.httppostmodel.postCompleted = this.postcreated;
                this.httppostmodel.dopost("/thread/" + this.thread, text);
            }
        }

        private void postcreated(JObject response)
        {

            bool success = false;
            if (response != null)
                success = (bool)response["success"];
            if (success)
            {
                this.postCreated(true);
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    this.postCreated(false);
                });
            }

        }

        public void getPosts(int page, string thread, bool pageabsolute = false)
        {
            this.thread = thread;
            if (!pageabsolute)
                this.actualpage += page;
            else
                this.actualpage = page;
            if (this.actualpage > 0)
            {
                if (this.threadpages != 0)
                    if (this.actualpage > this.threadpages - 1)
                        this.actualpage = this.threadpages - 1;
                thread += "?page=" + (actualpage + 1).ToString();
            }
            else
            {
                this.actualpage -= 1;
                if (this.actualpage < 0)
                    this.actualpage = 0;
            }
            //if (this.actualpage == 0 && App.ViewModel.settingsViewModel.Marker)
            //    thread += "?at_marker=1";
            this.gethttp.gotResponse = this.gotposts;
            this.gethttp.doHttpGET("/thread/" + thread);
        }

        private void gotposts(JObject pos)
        {
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                this.Posts.Clear();
                if (pos != null)
                {
                    JArray jarray = (JArray)pos["posts"];
                    foreach (JObject jobject in jarray)
                    {
                        Post post = new Post();
                        post.Username = (string)jobject["username"];
                        post.userID = Convert.ToString(jobject["userid"]);
                        post.Pic = (string)jobject["pic"];
                        post.Date = (string)jobject["date"];
                        TimeZoneInfo timezoneinfo = TimeZoneInfo.Local;
                        DateTime datetime = DateTime.Parse(post.Date);
                        datetime = TimeZoneInfo.ConvertTime(datetime, timezoneinfo);
                        post.Date = String.Format("{0:f}", datetime); 
                        post.Text = Convert.ToString(jobject["text"]);
                        post.postID = Convert.ToString(jobject["post_id"]);
                        post.Usercolor = Convert.ToString(jobject["usercolor"]);
                        if (jobject["marker"] != null && Convert.ToString(jobject["marker"]) != "")
                        {
                            this.markerdate = Convert.ToString(jobject["marker"]);
                            this.actualpage = (int)jobject["marker_page"] - 1;
                            this.markerdate = DateTime.Parse(this.markerdate).ToString("U");
                            this.markerpostid = post.postID;
                        }
                        this.Posts.Add(post);
                    }
                    this.threadpages = (int)pos["thread_pages"];
                    this.title = (string)pos["thread_title"];
                    /*string messages = "", bookmarks = "";
                    JObject userdata = (JObject)pos["userdata"];
                    messages = Convert.ToString(userdata["unread_messages"]);
                    bookmarks = Convert.ToString(userdata["unread_bookmarks"]);
                    App.ViewModel.unreadMessages = messages;
                    App.ViewModel.unreadBookmarks = bookmarks;
                    App.ViewModel.Status = string.Format("{0}: {1}, {2}: {3}", Application.Current.Resources["lngUnreadMessages"].ToString(), messages, Application.Current.Resources["lngUnreadBookmarks"].ToString(), bookmarks);*/
                    this.gotPosts(true);
                }
                else
                    this.gotPosts(false);
            });
        }
    }
}
