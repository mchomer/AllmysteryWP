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

namespace Allmystery.Model
{
    /// <summary>
    /// Diese Klasse stellt alle Methoden bereit, die für die Interaktion mit dem Isolated-Storage der App von Nöten sind.
    /// </summary>
    public class SettingsModel
    {
        /// <summary>
        /// Diese Methode schreibt die Einstellungen der App in die vorgesehene Datei im IsolatedStorage.
        /// </summary>
        /// <param name="sets">App-Einstellung in einem Array zusammengefasst.</param>
        public void writeSets(string[] sets)
        {
            // Diese Methode schreibt die gewählten Einstellungen in eine Textdatei.
            try
            {
                IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
                using (StreamWriter write = new StreamWriter(new IsolatedStorageFileStream(App.ViewModel.settingsFileName, FileMode.Create, FileAccess.Write, file)))
                {
                    foreach (string str in sets)
                        write.WriteLine(str);
                    write.Close();
                }
            }
            catch
            {
            }
        }
        /// <summary>
        /// Diese Methode liest die Einstellungen der App aus der vorgesehenen Datei aus dem IsolatedStorage.
        /// </summary>
        /// <returns>Gibt die gelesenen Einstellungen in einem Array zurück.</returns>
        public string[] readSets()
        {
            // Diese Methode liest die gespeicherten Einstellungen.
            string[] sets = { "", "", "1", "1", "1", "0", "1", "", "", "1" };
            try
            {
                IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
                if (file.FileExists(App.ViewModel.settingsFileName))
                {
                    string[] sets2 = new string[sets.Length];
                    IsolatedStorageFileStream read = file.OpenFile(App.ViewModel.settingsFileName, FileMode.Open, FileAccess.Read);
                    using (StreamReader reader = new StreamReader(read))
                    {
                        for (int i = 0; i < sets.Length; i++)
                            sets2[i] = reader.ReadLine();
                    }
                    sets = sets2;
                }
            }
            catch
            {
            }
            return sets;
        }
    }
}
