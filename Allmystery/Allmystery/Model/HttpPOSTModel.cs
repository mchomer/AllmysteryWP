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
using System.IO;
using System.Text;
using Microsoft.Phone.Reactive;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Net.NetworkInformation;
namespace Allmystery.Model
{
    /// <summary>
    /// Diese Klasse ist für die Ausführung von HTTP-Post-Requests zuständig. Der Aufruf erfolgt prinzipiell asynchron.
    /// </summary>
    public class HttpPOSTModel
    {
        public delegate void postcompletedCallback(JObject response);
        public postcompletedCallback postCompleted;
        private string baseaddress = "https://api.allmystery.de"; // Basis-Adresse der Allmystery-API.
        private string postdata = "";
        /// <summary>
        /// Diese Methode stößt den eigentlichen HTTP-Post-Requst an. Hierbei geschieht dies asynchron.
        /// </summary>
        /// <param name="address">Http-Post-Request-String ohne Allmystery-Basis-Adresse (https://api.allmystery.de).</param>
        /// <param name="postdata">Zu übermittelnde Textfolge.</param>
        /// <param name="cookie">Cookie, welcher ggf. Sessionsinformationen enthält. Optional.</param>
        public void dopost(string address, string postdata, Cookie cookie = null)
        {
            try 
            {
                this.postdata = postdata;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.baseaddress + address);
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows Phone OS 7.5; Trident/5.0; IEMobile/9.0)";
                request.CookieContainer = new CookieContainer();
                if (App.ViewModel.authToken.Name == "" || App.ViewModel.authToken.Name == null)
                    App.ViewModel.settingsViewModel.getSettings();
                if (cookie == null)
                    request.CookieContainer.Add(new Uri(App.ViewModel.baseAddress), App.ViewModel.authToken);
                else
                    request.CookieContainer.Add(new Uri(App.ViewModel.baseAddress), cookie);
                request.Method = "POST";
                request.BeginGetRequestStream(new AsyncCallback(this.getreqstream), request);
            }
            catch
            {
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    this.postCompleted(null);
                });
            }
        }
        /// <summary>
        /// Diese Methode füllt den POST-Request mit der zu übertragenden Zeichenkette.
        /// </summary>
        /// <param name="asynchronousresult">Http-Post-Request.</param>
        private void getreqstream(IAsyncResult asynchronousresult)
        {
            try 
            {
                HttpWebRequest request = (HttpWebRequest)asynchronousresult.AsyncState;
                Stream poststream = request.EndGetRequestStream(asynchronousresult);
                string postdata = this.postdata;
                byte[] byteArray = Encoding.UTF8.GetBytes(postdata);
                poststream.Write(byteArray, 0, byteArray.Length);
                poststream.Close();
                request.BeginGetResponse(new AsyncCallback(this.getresponse), request);
            }
            catch (Exception e)
            {
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    this.postCompleted(null);
                });
            }
        }
        /// <summary>
        /// Diese Methode wertet den Response, des, in den vorherigen Schritten angestoßenen HTTP-Post-Request aus.
        /// </summary>
        /// <param name="asynchronousresult">Ergebnis des HTTP-Post-Requests.</param>
        private void getresponse(IAsyncResult asynchronousresult)
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
                this.postCompleted(jresponse);
            }
            catch
            {
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    this.postCompleted(null);
                });
            }
        }        
    }
}
