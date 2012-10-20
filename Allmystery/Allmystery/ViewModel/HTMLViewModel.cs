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
using System.Collections.Generic;
using System.Text;
using Allmystery.ViewModel.DataTypes;
namespace Allmystery.ViewModel
{
    /// <summary>
    /// Diese Klasse beinhaltet HTML-, JS- und CSS-Coding. Mit Hilfe dieser Klasse werden verschiedene Anzeigen für das WebBrowser-Control
    /// generiert.
    /// </summary>
    public class HTMLViewModel
    {
        private List<Smiley> smilies;
        public List<Smiley> Smilies
        {
            get
            {
                return this.smilies;
            }
        }
        private string scrollto;
        public string scrollTo
        {
            get 
            {
                return this.scrollto;
            }
        }

        private string mebubble;
        public string meBubble
        {
            get
            {
                return this.mebubble;
            }
        }
        private string yoububble;
        public string youBubble
        {
            get
            {
                return this.yoububble;
            }
        }
        private string conversationpage;
        public string conversationPage
        {
            get
            {
                return this.conversationpage;
            }
        }
        private string threadpage;
        public string threadPage
        {
            get
            {
                return this.threadpage;
            }
        }
        private string postbox;
        public string postBox
        {
            get
            {
                return this.postbox;
            }
        }

        /// <summary>
        /// Konstruktor der Klasse.
        /// </summary>
        public HTMLViewModel()
        {
            this.conversationpage = "<!DOCTYPE html PUBLIC '-//WAPFORUM//DTD XHTML Mobile 1.2//EN' 'http://www.openmobilealliance.org/tech/DTD/xhtml-mobile12.dtd'><html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=iso-8859-1\"><meta name=\"viewport\" content=\"width=device-width\"/><style type=\"text/css\">body { background-color:black;color:white;font-size: 15px;font-family: Tahoma,Verdana,Arial,sans-serif;} .uicon { background: transparent no-repeat; display:inline-block; text-indent:-9999em; background-position:0px 1px; color:transparent;}  .bubblecontent img { width:98% } iframe { width:98%; height:58% } img {-ms-interpolation-mode: bicubic;} a:link {color: #B1B2C9;} #bobblebottom { background-color: #2B2B2B; padding:10px;} #bobbletop {background-color: #404040;padding:10px;}</style></head><body>{0}{1}</body></html>";
            this.yoububble = "<br><img src=\"{4}\" class=\"useravatar\" height=\"30\" width=\"30\"><div class=\"bubblecontent\" id=\"{3}\"><div id=\"bobblebottom\"><div><i>{2}</i></div><p>{0}</p><b>{1}</b></div></div>";
            this.mebubble = "<br><img src=\"{4}\" class=\"useravatar\" height=\"30\" width=\"30\"><div class=\"bubblecontent\" id=\"{3}\" ><div id=\"bobbletop\"><div><i>{2}</i></div><p>{0}</p><b>{1}</b></div></div>";
            this.threadpage = "<!DOCTYPE html PUBLIC '-//WAPFORUM//DTD XHTML Mobile 1.2//EN' 'http://www.openmobilealliance.org/tech/DTD/xhtml-mobile12.dtd'><html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=iso-8859-1\"><meta name=\"viewport\" content=\"width=device-width\"/><style type=\"text/css\">.postcontent img { width:98% } iframe { width:98%; height:58% } img {-ms-interpolation-mode: bicubic;} body { background-color:black;} .uicon { background: transparent no-repeat; display:inline-block; text-indent:-9999em; background-position:0px 1px; color:transparent;} a:link { color: #B1B2C9;}</style></head><body>{0} {1}<script type=\"text/javascript\"></script></body></html>";
            this.postbox = "<div class=\"postbox1\" id=\"id{8}\" style=\"background-color:{0};color:white;font-size: 15px;font-family: Tahoma,Verdana,Arial,sans-serif\"><div class=\"finfo\"><br><a href=\"{7}\"><div class=\"ava\"><img src=\"{1}\" class=\"useravatar\" height=\"50\" width=\"50\"></a></div><div class=\"nu\" style=\"color:{2}\"><b>{3}</b></a></div><div class=\"finfo_user\" style=\"border-bottom: 1px solid black;\">{4}<div class=\"fbeitrag\"><span class=\"date\" id=\"date_{8}\"><i><b>{5}</b></i></span><p><div class=\"postcontent\">{6}</div><br><br></div></span></div>";
            //this.scrollto = "<script type=\"text/javascript\"> function moveto() { var ypos= document.getElementById('{0}').offsetTop; window.scrollTo(0,ypos); return ypos; } moveto(); </script>";
            //this.scrollto = "<script type=\"text/javascript\">function findPos(obj) { var curtop = 0; if (obj.offsetParent) { do { curtop += obj.offsetTop; } while (obj = obj.offsetParent); return [curtop]; } } var SupportDiv = document.getElementById('{0}'); window.scroll(0,findPos(SupportDiv)); </script>";
            this.scrollto = "<script type=\"text/javascript\"> function moveto() { document.getElementById('{0}').scrollIntoView(); } moveto(); </script>";
        }
        
    }
}
