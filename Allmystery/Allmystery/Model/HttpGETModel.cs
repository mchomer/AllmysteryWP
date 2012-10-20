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
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Text;
using System.IO;
using Microsoft.Phone.Reactive;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Net.NetworkInformation;

namespace Allmystery.Model
{
    /// <summary>
    /// Diese Klasse ist für den Aufruf von einfachen Http-GET-Requests zuständig. Hierbei erfolgt dieser in jedem Fall asynchron.
    /// </summary>
    public class GetHttp
    {
        public delegate void gotresponseCallback(JObject reponse);
        public gotresponseCallback gotResponse;
        /// <summary>
        /// Stößt den Http-GET-Request an. Dieser erfolgt asynchorn und wird mit gotcategories abgeschlossen.
        /// </summary>
        /// <param name="where">Http-Request-String.</param>
        public void doHttpGET(string where)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(App.ViewModel.baseAddress + where);
                request.CookieContainer = new CookieContainer();
                if (App.ViewModel.authToken.Name == "" || App.ViewModel.authToken.Name == null)
                    App.ViewModel.settingsViewModel.getSettings();
                request.CookieContainer.Add(new Uri(App.ViewModel.baseAddress), App.ViewModel.authToken);
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows Phone OS 7.5; Trident/5.0; IEMobile/9.0)";
                request.Method = "GET";
                request.BeginGetResponse(new AsyncCallback(this.gotcategories), request);
            }
            catch
            {
                // Notwendig um Cross-Threading-Error zu vermeiden.
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    this.gotResponse(null);
                });
            }
        }
        /// <summary>
        /// Diese Methode wird ausgeführt, sobald eine Antwort auf den vorher gestarteten Http-Request erfolgt ist.
        /// </summary>
        /// <param name="asynchronousresult">Ergebnis des Requests.</param>
        private void gotcategories(IAsyncResult asynchronousresult)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)asynchronousresult.AsyncState;
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousresult);
                Stream streamresponse = response.GetResponseStream();
                StreamReader streamreader = new StreamReader(streamresponse);
                string responsestring = streamreader.ReadToEnd();
                JObject jresponse = JObject.Parse(responsestring);
                streamresponse.Close();
                streamreader.Close();
                response.Close();
                this.gotResponse(jresponse);
            }
            catch
            {
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    this.gotResponse(null);
                });
            }
        }
    }
}
