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
    /// Diese Klasse ermöglicht den Zugridff auf die einzelnen Rubrik-Kategorien.
    /// </summary>
    public class CategoriesViewModel : BaseViewModel
    {
        public delegate void gotCategoriesCallback(bool result);
        public gotCategoriesCallback gotCategories;
        private ObservableCollection<Category> categories;
        
        public ObservableCollection<Category> Categories
        {
            set
            {
                this.categories = value;
            }
            get
            {
                return this.categories;
            }
        }
        private GetHttp gethttp;
        /// <summary>
        /// Dies ist der Konstruktor der Klasse.
        /// </summary>
        public CategoriesViewModel()
        {
            this.gethttp = new GetHttp();
            this.categories = new ObservableCollection<Category>();
        }
        /// <summary>
        /// Mit dieser Methode werden die Kategorien abgerufen.
        /// </summary>
        public void getCategories()
        {
            this.gethttp.gotResponse = this.gotcategories;
            string category = "";
            if (App.ViewModel.settingsViewModel.startTopics == 0)
                category = "/category/list";
            else if (App.ViewModel.settingsViewModel.startTopics == 1)
                category = "/category/list?sort=desc";
            else if (App.ViewModel.settingsViewModel.startTopics == 2)
                category = "/category/list_my";
            this.gethttp.doHttpGET(category);
        }
        /// <summary>
        /// Diese Methode verarbeitet die angeforderten Kategorien und bereitet sie für die Anzeige auf.
        /// </summary>
        /// <param name="cats">Enthält die Kategorien im JSON-Format, andernfalls NULL.</param>
        private void gotcategories(JObject cats)
        {
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                this.Categories.Clear();
                if (cats != null)
                {
                    JArray jarray = null;
                    jarray = (JArray)cats["categories"];
                    int i = 0;
                    foreach (JObject jobject in jarray)
                    {
                        Category category = new Category();
                        category.Cat = (string)jobject["cat"];
                        category.titleLong = (string)jobject["title_long"];
                        category.titleShort = (string)jobject["title_short"];
                        category.Pic = (string)jobject["pic"];
                        category.lastPostThreadID = Convert.ToString(jobject["last_post_threadid"]);
                        category.lastPostTitle = (string)jobject["last_post_title"];
                        category.lastPostDate = (string)jobject["last_post_date"];
                        TimeZoneInfo timezoneinfo = TimeZoneInfo.Local;                        
                        DateTime datetime = DateTime.Parse(category.lastPostDate);
                        datetime = TimeZoneInfo.ConvertTime(datetime, timezoneinfo);
                        category.lastPostDate = String.Format("{0:f}", datetime);                        
                        category.lastPostPage = Convert.ToString(jobject["last_post_page"]);
                        category.Count = Convert.ToString(jobject["count"]);
                        category.lastPostID = Convert.ToString(jobject["last_post_id"]);
                        category.lastPostUsername = (string)jobject["last_post_username"];
                        category.createdUsername = (string)jobject["created_username"];
                        category.defaultTags = (string)jobject["default_tags"];
                        if (i % 2 != 0)
                            category.backgroundColor = "#2B2B2B";
                        else
                            category.backgroundColor = "#404040";
                        this.categories.Add(category);
                        i++;
                    }
                    string messages = "", bookmarks = "";
                    JObject userdata = (JObject)cats["userdata"];
                    messages = Convert.ToString(userdata["unread_messages"]);
                    bookmarks = Convert.ToString(userdata["unread_bookmarks"]);
                    App.ViewModel.unreadMessages = messages;
                    App.ViewModel.unreadBookmarks = bookmarks;
                    App.ViewModel.Status = string.Format("{0}: {1}, {2}: {3}", Application.Current.Resources["lngUnreadMessages"].ToString(), messages, Application.Current.Resources["lngUnreadBookmarks"].ToString(), bookmarks);
                    this.gotCategories(true);
                }
                else
                    this.gotCategories(false);
            });
        }
    }
}
