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
using System.Linq;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Globalization;
using Allmystery.Model;
using System.IO;
using System.IO.IsolatedStorage;

namespace Allmystery.ViewModel
{
   /// <summary>
   /// Diese Klasse handelt alles ab, was für den Login bei der Allmystery-API von Nöten ist.
   /// </summary>
    public class LoginViewModel
    {
        List<ResourceDictionary> dictionarylist;
        LoginModel loginmodel;
        public delegate void logincompletedCallback(bool result);
        public logincompletedCallback loginCompleted;
        /// <summary>
        /// Konstruktor der Klasse.
        /// </summary>
        public LoginViewModel()
        {
            this.loginmodel = new LoginModel();
            this.dictionarylist = new List<ResourceDictionary>();
            ResourceDictionary resourcedictionary = null;
            foreach (ResourceDictionary dictionary in Application.Current.Resources.MergedDictionaries)
            {
                dictionarylist.Add(dictionary);
            }
            resourcedictionary = dictionarylist.FirstOrDefault(d => d.Source.OriginalString == "Models/Languages/German.xaml");                     

        }
        /// <summary>
        /// Diese Methode stößt den Login-Prozess an.
        /// </summary>
        public void logIn()
        {
            if (App.ViewModel.authToken.Value == "" || App.ViewModel.authToken.Value == null)
            {
                this.loginmodel.loginCompleted = this.logincompleted;
                this.loginmodel.loginToAllmystery(App.ViewModel.settingsViewModel.userName, App.ViewModel.settingsViewModel.passWord);
            }
            else
            {
                this.logincompleted(true);
            }
        }
        /// <summary>
        /// Diese Methode handelt alles ab, was nach erfolgten Login-Versuch nötig ist.
        /// </summary>
        /// <param name="result">Jenach erfolgreichem Login WAHR oder FALSCH.</param>
        private void logincompleted(bool result)
        {
            if (!result)
            {
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    App.ViewModel.loggedIn = false;
                    this.loginCompleted(false);
                });
            }
            else
            {                
                App.ViewModel.loggedIn = true;                
                //App.ViewModel.gotChannel = this.gotchannel;
                //App.ViewModel.getChannel();
                //string test = App.ViewModel.channelUri;
                App.ViewModel.settingsViewModel.writeSettings();
                this.loginCompleted(true);
                
            }
        }
        /// <summary>
        /// Diese Methode wird angestoßen, wenn ein Notification-Channel gezogen wurde. NUR ZU TESTZWECKEN!
        /// </summary>
        /// <param name="result"></param>
        private void gotchannel(bool result = true)
        {
            // TODO: Ordentliche Verwendung für diese Stelle!
            string test = App.ViewModel.channelUri;
        }
    }
}
