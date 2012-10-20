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
using System.ComponentModel;
using Allmystery.Model;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Shell;
using System.IO;
using System.IO.IsolatedStorage;

namespace Allmystery.ViewModel
{
    public class SettingsViewModel : BaseViewModel
    {
        private SettingsModel settingsmodel = new SettingsModel();
        private string username = "";
        private string password = "";
        private bool pushmessages = false;
        private bool pushthreads = false;
        private bool pushsystem = false;
        private byte starttopics = 0;
        private bool marker = false;
        private bool autofavourite = false;
        public bool autoFavourite
        {
            get
            {
                return this.autofavourite;
            }
            set
            {
                this.autofavourite = value;
                this.OnPropertyChanged("autoFavourite");
            }
        }

        public bool Marker
        {
            set
            {
                this.marker = value;
                this.OnPropertyChanged("Marker");
            }
            get
            {
                return this.marker;
            }
        }
        public byte startTopics
        {
            set
            {
                this.starttopics = value;
                this.OnPropertyChanged("startTopics");
            }
            get
            {
                return this.starttopics;
            }
        }

        public string userName
        {
            set
            {
                this.username = value;
                this.OnPropertyChanged("userName");
            }
            get
            {
                return this.username;
            }
        }
        public string passWord
        {
            set
            {
                this.password = value;
                this.OnPropertyChanged("passWord");
            }
            get
            {
                return this.password;
            }
        }
        public bool pushMessages
        {
            set
            {
                this.pushmessages = value;
                this.OnPropertyChanged("pushMessages");
            }
            get
            {
                return this.pushmessages;
            }
        }
        public bool pushThreads
        {
            set
            {
                this.pushthreads = value;
                this.OnPropertyChanged("pushThreads");
            }
            get
            {
                return this.pushthreads;
            }
        }
        public bool pushSystem
        {
            set
            {

                this.pushsystem = value;
                this.OnPropertyChanged("pushSystem");
            }
            get
            {
                return this.pushsystem;
            }
        }

        public SettingsViewModel()
        {
            
        }

        public void writeSettings()
        {
            string[] sets = {   this.userName, this.passWord, this.pushMessages == true ? "1" : "0", this.pushThreads == true ? "1" : "0",
                                this.pushSystem == true ? "1" : "0", Convert.ToString(this.starttopics), 
                                this.Marker == true ? "1" : "0", App.ViewModel.authToken == null ? "" : App.ViewModel.authToken.Name, App.ViewModel.authToken == null ? "" : App.ViewModel.authToken.Value, this.autoFavourite == true ? "1" : "0" };
            this.settingsmodel.writeSets(sets);
        }

        public void getSettings()
        {
            string[] sets = this.settingsmodel.readSets();

            this.userName = sets[0];
            this.passWord = sets[1];
            this.pushMessages = sets[2] == "1" ? true : false;
            this.pushThreads = sets[3] == "1" ? true : false;
            this.pushSystem = sets[4] == "1" ? true : false;
            this.startTopics = Convert.ToByte(sets[5]);
            this.Marker = sets[6] == "1" ? true : false;
            string name = "", value = "";
            name = sets[7];
            value = sets[8];
            if (name != "" && value != "" && name != null && value != null)
            {
                App.ViewModel.authToken = new Cookie(name, value);
            }
            this.autoFavourite = sets[9] == "1" ? true : false;
        }
        /// <summary>
        /// Diese Methode stößt die Aktualisierung des View-Binding-Elements der Views an.
        /// </summary>
        /// <param name="propertyName"></param>
        public new void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            string[] sets = {   this.userName, this.passWord, this.pushMessages == true ? "1" : "0", this.pushThreads == true ? "1" : "0",
                                this.pushSystem == true ? "1" : "0", Convert.ToString(this.starttopics), 
                                this.Marker == true ? "1" : "0", App.ViewModel.authToken == null ? "" : App.ViewModel.authToken.Name, App.ViewModel.authToken == null ? "" : App.ViewModel.authToken.Value, this.autoFavourite == true ? "1" : "0" };
            this.settingsmodel.writeSets(sets);
        }        
    }
}
