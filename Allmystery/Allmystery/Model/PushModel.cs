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
using Microsoft.Phone.Notification;
using System.IO;
using System.Text;
using System.Linq;
using Newtonsoft.Json.Linq;
using Allmystery.ViewModel;
using Microsoft.Phone.Shell;
using System.Net.NetworkInformation;
namespace Allmystery.Model
{
    /// <summary>
    /// Diese Klasse ermöglicht die Push-Funktionalitäten.
    /// </summary>
    public class PushModel
    {
        ShellTile tile = ShellTile.ActiveTiles.First();
        UserDataViewModel userdataviewmodel;
        HttpPOSTModel httppostmodel;
        public delegate void gotChannelUriCallback(bool result);
        public gotChannelUriCallback gotChannel;
        private string channeluri;
        public string channelUri
        {
            get
            {
                return this.channeluri;
            }
        }
        private string notificationchannel;
        public string notificationChannel
        {
            get
            {
                return this.notificationchannel;
            }
        }

        private HttpNotificationChannel channel;
        public HttpNotificationChannel Channel
        {
            get
            {
                
                return this.channel;
            }
        }
        /// <summary>
        /// Dies ist der Konstruktor der Klasse, welcher die nötigsten Klassenfelder setzt.
        /// </summary>
        public PushModel()
        {
            this.notificationchannel = "AllmyPush" + App.ViewModel.settingsViewModel.userName;
            this.httppostmodel = new HttpPOSTModel();
            this.userdataviewmodel = new UserDataViewModel();
        }
        /// <summary>
        /// Mit Hilfe dieser Methode werden die Push-Notifications in der Allmystery-API aktiviert..
        /// </summary>
        private void loginChannel()
        {      
            if (App.ViewModel.authToken.Name == null)
                App.ViewModel.settingsViewModel.getSettings();
            string[] ps = { App.ViewModel.settingsViewModel.pushMessages == true ? "1" : "0", App.ViewModel.settingsViewModel.pushThreads == true ? "1" : "0", App.ViewModel.settingsViewModel.pushSystem == true ? "1" : "0" };
            string token = this.channelUri;
            string postdata = string.Format("push_msgs={0}&push_bookmarks={1}&push_sys={2}&token={3}", ps[0], ps[1], ps[2], token);
            this.httppostmodel.postCompleted = this.loggedchannel;
            this.httppostmodel.dopost("/update_mpns/", postdata, App.ViewModel.authToken);
        }
        /// <summary>
        ///  Methode die ausgeführt wird, wenn die Push-Notifications erfolgreich in der Allmystery-API aktiviert werden konnten.
        /// </summary>
        /// <param name="result">Nimmt ein JSON-Objekt an.</param>
        private void loggedchannel(JObject result)
        {
            if (result != null) 
            {
                this.gotChannel(true);
            }
        }
        /// <summary>
        /// Mit dieser Methode ein Channel für die Push-Notifications registriert.
        /// </summary>
        public void getChannel()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                try
                {

                    this.channel = HttpNotificationChannel.Find(this.notificationchannel);
                    if (channel == null)
                    {
                        this.channel = new HttpNotificationChannel(this.notificationchannel);
                        this.channel.ChannelUriUpdated += this.channel_ChannelUriUpdated;
                        this.channel.ErrorOccurred += this.channel_ErrorOccurred;
                        this.channel.ShellToastNotificationReceived += this.channel_ShellToastNotificationReceived;
                        this.channel.ConnectionStatusChanged += this.channel_ConnectionStatusChanged;
                        this.channel.Open();
                        this.channel.BindToShellToast();
                        this.channel.BindToShellTile();
                    }
                    else
                    {
                        this.channel.ChannelUriUpdated += this.channel_ChannelUriUpdated;
                        this.channel.ErrorOccurred += this.channel_ErrorOccurred;
                        this.channel.ShellToastNotificationReceived += this.channel_ShellToastNotificationReceived;
                        this.channel.ConnectionStatusChanged += this.channel_ConnectionStatusChanged;
                        this.channeluri = this.channel.ChannelUri.ToString();
                        this.loginChannel();
                    }
                }
                catch
                {
                    this.gotChannel(false);
                }
            }
            else
                this.gotChannel(false);
        }
        /// <summary>
        /// Diese Methode wird ausgeführt, sobald sich der Status der Push-Notification-Channel-Verbindung verändert.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void channel_ConnectionStatusChanged(object sender, NotificationChannelConnectionEventArgs e)
        {
            if (e.ConnectionStatus == ChannelConnectionStatus.Disconnected)
            {
                this.closeChannel();
            }
        }
        /// <summary>
        /// Diese Methode wird angestoßen, wenn eine ShellToast-Notification übermittelt wurde.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void channel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {
            string[] conversation = e.Collection.Values.Last().ToString().Replace("/ConversationPage.xaml?conversation=", "").Replace("username=","").Split('&');
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {                
                this.userdataviewmodel.gotUserData = this.gotuserdata;
                this.userdataviewmodel.getUserData();
                if (conversation[0] == App.ViewModel.pushConversation)
                    App.ViewModel.refreshConversation();
                else if (App.ViewModel.pushConversation == "MyConversations")
                    App.ViewModel.refreshConversation();
                
            });
        }
        /// <summary>
        /// Diese Methode wird ausgeführt, sobald die UserData-Informationen empfangen wurden.
        /// </summary>
        /// <param name="result">War die Übermittlung erfolgreich? Bool'scher Rückgabewert.</param>
        private void gotuserdata(bool result)
        {
            if (result)
            {
                App.ViewModel.unreadBookmarks = this.userdataviewmodel.bookmarkCount;
                App.ViewModel.unreadMessages = this.userdataviewmodel.messageCount;
                App.ViewModel.Status = string.Format("{0}: {1}, {2}: {3}", Application.Current.Resources["lngUnreadMessages"].ToString(), this.userdataviewmodel.messageCount, Application.Current.Resources["lngUnreadBookmarks"].ToString(), this.userdataviewmodel.bookmarkCount);
                try
                {
                        this.tile.Update(new StandardTileData
                        {
                            BackContent = string.Format("{0} Nachrichten", App.ViewModel.unreadMessages),
                        });
                }
                catch
                {
                    
                }
            }
            else
            {
            }
        }
        /// <summary>
        /// Diese Methode wird aufgerufen, sobald ein Fehler beim Empfangen von Push-Notifications auftritt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void channel_ErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            switch (e.ErrorType)
            {
                case ChannelErrorType.ChannelOpenFailed:
                    break;
                case ChannelErrorType.MessageBadContent:
                    break;
                case ChannelErrorType.NotificationRateTooHigh:
                    break;
                case ChannelErrorType.PayloadFormatError:
                    break;
                case ChannelErrorType.PowerLevelChanged:
                    break;
                case ChannelErrorType.Unknown:
                    break;
            }
            this.gotChannel(false);
        }
        /// <summary>
        /// Diese Methode wird ausgeführt, wenn erfolgreich eine neue Channel-Uri gezogen werden konnte.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void channel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            this.channeluri = e.ChannelUri.ToString();
            this.loginChannel();
        }
        /// <summary>
        /// Diese Methode schließt den offenen Push-Notifications-Channel.
        /// </summary>
        public void closeChannel()
        {            
            try
            {
                if (this.channel != null)
                {
                    this.channel.UnbindToShellToast();
                    this.channel.UnbindToShellTile();
                    this.channel.Close();
                }
            }
            catch { }
        }
    }
}
