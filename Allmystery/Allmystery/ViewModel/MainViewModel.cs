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
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System.Linq;

namespace Allmystery.ViewModel
{
    /// <summary>
    /// Diese Klasse wird EINMALIG Instanziiert und nach MVVM verwendet!
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        public delegate void refreshConversationCallback();
        public string pushConversation { get; set; }
        public bool startone { get; set; }
        public refreshConversationCallback refreshConversation;
        private LoginViewModel loginviewmodel;
        private UserDataViewModel userdataviewmodel;
        private PushModel pushmodel;
        public TemplateViewModel templateViewModel;
        public delegate void MainViewModelEventHandler(object sender, MainViewModelEventArgs e);
        public event MainViewModelEventHandler mainviewmodelevent;
        public delegate void gotChannelUriCallback(bool result);
        public gotChannelUriCallback gotChannel;
        private MainViewModelEventArgs resultsforlivetile;
        public SettingsViewModel settingsViewModel;
        private string sslsource = "https://skydrive.live.com/redir?resid=CB4E548F1F6EB2B6!1722&authkey=!AOatf8UK0MERB4Y";
        public string sslSource
        {
            get
            {
                return this.sslsource;
            }
        }
        public string channelUri
        {
            get
            {
                return this.pushmodel.channelUri;
            }
        }
        
        public string Offset { get; set; }
        public string unreadMessages { get; set; }
        public string unreadBookmarks { get; set; }
        private string status;
        public string Status
        {
            set
            {
                this.status = value;
                this.OnPropertyChanged("Status");
            }
            get
            {
                return this.status;
            }
        }
        private string settingsfilename = "allmy.cfg";
        public string settingsFileName
        {
            get
            {
                return this.settingsfilename;
            }
        }
        private Cookie authtoken = new Cookie();
        public Cookie authToken
        {
            set 
            { 
                this.authtoken = value; 
            }
            get
            {
                return this.authtoken;
            }
        }

        private bool loggedin = false;
        public bool loggedIn
        {
            set
            {
                this.loggedin = value;
            }
            get
            {
                return this.loggedin;
            }
        }
        private string baseaddress = "https://api.allmystery.de";
        public string baseAddress
        {
            set
            {
                this.baseaddress = value;
            }
            get
            {
                return this.baseaddress;
            }
        }

        public MainViewModel()
        {
            this.settingsViewModel = new SettingsViewModel();
            this.userdataviewmodel = new UserDataViewModel();
            this.loginviewmodel = new LoginViewModel();
            this.templateViewModel = new TemplateViewModel();
        }


        public void getUserData()
        {
            this.loginviewmodel.loginCompleted = this.loggedinback;
            this.loginviewmodel.logIn();
        }

        private void loggedinback(bool result)
        {
            if (result)
            {
                this.userdataviewmodel.gotUserData = this.gotuserdata;
                this.userdataviewmodel.getUserData();
            }
            else
            {
                // TODO: Error-Handling hinzufügen.
            }
        }

        private void gotuserdata(bool result = true)
        {
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                if (result)
                {
                    this.resultsforlivetile = new MainViewModelEventArgs(this.userdataviewmodel.messageCount, this.userdataviewmodel.bookmarkCount);
                    this.mainviewmodelevent(this, this.resultsforlivetile);
                }
                else
                { }
            });
        }

        public void getChannel()
        {
            if (this.pushmodel == null)
                this.pushmodel = new PushModel();
            this.pushmodel.gotChannel = this.gotchannel;
            this.pushmodel.getChannel();
        }

        private void gotchannel(bool result = true)
        {
            this.gotChannel(result);
        }

        public void closeChannel()
        {
            this.pushmodel.closeChannel();
        }
    }
}
