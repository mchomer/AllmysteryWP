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
using System.IO.IsolatedStorage;
using System.Collections.ObjectModel;
using Allmystery.ViewModel.DataTypes;
using System.Text.RegularExpressions;

namespace Allmystery.Model
{
    /// <summary>
    /// Diese Klasse beinhaltet alle Methoden, die Entwürfe in den Isolated-Storage schreiben und auslesen.
    /// </summary>
    public class TemplateModel
    {
        private string filename = "templates.tmp";
        /// <summary>
        /// Diese Methode schreibt den übergebenen Entwurf in den Isolated-Storage.
        /// </summary>
        /// <param name="sets">Zur Speicherung ausgewählter Entwurf.</param>
        public void writeSets(ObservableCollection<Template> sets)
        {
            // Diese Methode schreibt die gewählten Einstellungen in eine Textdatei.
            try
            {
                IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
                using (StreamWriter write = new StreamWriter(new IsolatedStorageFileStream(filename, FileMode.Create, FileAccess.Write, file)))
                {
                    foreach (Template str in sets)
                    {
                        string templine = str.threadID + "<||>" + @str.threadTitle + "<||>" + str.Date + "<||>" + @str.Text.Replace("\n",">!n!<").Replace("\r",">!r!<");
                        write.WriteLine(templine);
                    }
                    write.Close();
                }
            }
            catch
            {
            }
        }
        /// <summary>
        /// Diese Methode liest alle gespeicherten Entwürfe aus dem Isolated-Storage und stellt sie zur weiteren Verwendung bereit.
        /// </summary>
        /// <returns>Collection mit allen gespeicherten Entwürfen.</returns>
        public ObservableCollection<Template> readSets()
        {
            // Diese Methode liest die gespeicherten Einstellungen.
            ObservableCollection<Template> sets = new ObservableCollection<Template>();
            try
            {
                int i = 0;
                IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
                if (file.FileExists(this.filename))
                {
                    string readtext = "";
                    IsolatedStorageFileStream read = file.OpenFile(filename, FileMode.Open, FileAccess.Read);
                    using (StreamReader reader = new StreamReader(read))
                    {
                        
                        do
                        {
                            readtext = reader.ReadLine();
                            string[] splittext = new string[4];
                            splittext = readtext.Split(new string[] { "<||>" }, StringSplitOptions.None);
                            string color = "";
                                if (i % 2 != 0)
                                    color = "#2B2B2B";
                                else
                                    color = "#404040";
                                sets.Add(new Template(splittext[2], splittext[0], splittext[1], splittext[3], color));
                              
                            i++;
                        } while (readtext != null);
                        reader.Close();
                    }            
                }
            }
            catch
            {
            }
            return sets;
        }
        /// <summary>
        /// Diese Methode löscht die gespeicherten Entwürfe im Isolated-Storage.
        /// </summary>
        public void removeFile()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            if (file.FileExists(this.filename))
            {
                file.DeleteFile(this.filename);
            }
        }
    }
}
