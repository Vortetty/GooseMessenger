// 1. Added the "GooseModdingAPI" project as a reference.
// 2. Compile this.
// 3. Create a folder with this DLL in the root, and *no GooseModdingAPI DLL*
using GooseShared;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Xml;
using Hjson;
using ProfanityFilter;

namespace GooseMessaging
{
    public class ModEntryPoint : IMod
    {
        // Gets called automatically, passes in a class that contains pointers to
        // useful functions we need to interface with the goose.
        void IMod.Init()
        {
            //config stuff
            string configpath = Path.Combine(API.Helper.getModDirectory(this), "config.hjson");

            if (!File.Exists(configpath))
            {
                using (StreamWriter sw = File.CreateText(configpath))
                {
                    sw.Write("");
                    sw.Close();
                }
                var jsonObject = new JsonObject
                {
                  { "profanitiesAllowed", false }
                };
                HjsonValue.Save(jsonObject, configpath);
            }

            //if (!config.Item1)
            //{
            //    genSwears();
            //}
            //if (!config.Item2)
            //{
            //    genSexuals();
            //}
            //if (!config.Item3)
            //{
            //    genOther();
            //}

            // Subscribe to whatever events we want
            InjectionPoints.PostTickEvent += PostTick;
        }

        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(Keys vKey);
        bool failsafe = false;
        string input;
        //List<List<string>> swearList = new List<List<string>>();

        public bool loadConfig()
        {
            string configpath = Path.Combine(API.Helper.getModDirectory(this), "config.hjson");
            var jsonObject = HjsonValue.Load(configpath).Qo();
            return jsonObject.Qb("profanitiesAllowed");
        }

        /*public void genSwears()
        {
            swearList.Add(new List<string> { "ass", "a**" });
            swearList.Add(new List<string> { "bastard", "b******" });
            swearList.Add(new List<string> { "bitch", "b****" });
            swearList.Add(new List<string> { "cunt", "c***" });
            swearList.Add(new List<string> { "fuck", "f***" });
            swearList.Add(new List<string> { "shit", "s***" });
            swearList.Add(new List<string> { "slut", "sl**" });
            swearList.Add(new List<string> { "whore", "w***e" });
            swearList.Add(new List<string> { "twat", "t**t" });
            swearList.Add(new List<string> { "shag", "passionately hug" });
            swearList.Add(new List<string> { "sex", "passionately hug" });
        }

        public void genSexuals()
        {
            swearList.Add(new List<string> { "anal", "a**l" });
            swearList.Add(new List<string> { "alabama hot pocket", "alabama hot pocket (dont look it up)" });
            swearList.Add(new List<string> { "penis", "p***s" });
            swearList.Add(new List<string> { "vagina", "v****a" });
            swearList.Add(new List<string> { "erotic", "s**ual" });
            swearList.Add(new List<string> { "twat", "t**t" });
            swearList.Add(new List<string> { "dick", "d**k" });
            swearList.Add(new List<string> { "pussy", "p***y" });
            swearList.Add(new List<string> { "tit", "t*t" });
            swearList.Add(new List<string> { "tiddy", "t***y" });
            swearList.Add(new List<string> { "knockers", "breasts" });
            swearList.Add(new List<string> { "boobs", "breasts" });
            swearList.Add(new List<string> { "tiddy", "t***y" });
            swearList.Add(new List<string> { "blowjob", "t***y" });
            swearList.Add(new List<string> { "blow job", "t***y" });
            swearList.Add(new List<string> { "bondage", "b*****e" });
            swearList.Add(new List<string> { "boner", "b***r" });
            swearList.Add(new List<string> { "buttplug", "butt p**g" });
            swearList.Add(new List<string> { "cameltoe", "c******e" });
            swearList.Add(new List<string> { "clit", "cl*t" });
            swearList.Add(new List<string> { "dildo", "d***o" });
        }

        public void genOther()
        {
        }*/

        public static bool haswifi()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    return true;
            }
            catch
            {
                return false;
            }
        }

        public static String stripNonUTF8(string inputValue)
        {
            return Regex.Replace(inputValue, @"[^\u0000-\u00FF]", String.Empty);
        }

        static string stripDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        static string replaceSwears(string text, List<List<string>> swears)
        {
            foreach (List<string> replaceVal in swears)
            {
                Regex.Replace(text, replaceVal[0], replaceVal[1], RegexOptions.IgnoreCase);
            }
            return text;
        }

        public void PostTick(GooseEntity g)
        {
            if ((GetAsyncKeyState(Keys.F9) != 0) && !failsafe)
            {
                failsafe = true;
                if (haswifi())
                {
                    new Thread(() =>
                    {
                        var guiForm = new Form1();
                        DialogResult result = guiForm.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            input = guiForm.inputText;
                        }
                        //string input = Interaction.InputBox("What text would you like to send to other geese?", "Send Message", "Default");
                        if (!String.IsNullOrWhiteSpace(input))
                        {
                            XmlDocument document = new XmlDocument();
                            document.Load("url here" + System.Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(input)));
                            string webData = document.InnerText;
                            if (webData == "saved")
                            {
                                MessageBox.Show("Message sent successfully!", "Message Sent");
                                var getmessage = new Form2();
                                DialogResult dialogResult = getmessage.ShowDialog();
                                if (dialogResult == DialogResult.Yes)
                                {
                                    XmlDocument document1 = new XmlDocument();
                                    document1.Load("url here");
                                    string webData1 = System.Text.ASCIIEncoding.ASCII.GetString(System.Convert.FromBase64String(document1.InnerText));
                                    if (loadConfig() != true)
                                    {
                                        var filter = new ProfanityFilter.ProfanityFilter();
                                        webData1 = filter.CensorString(webData1);
                                    }
                                    MessageBox.Show(webData1, "Message From Other Geese");
                                }
                            }
                        }
                        else
                        {
                            if (result == DialogResult.OK)
                            {
                                MessageBox.Show("Blank messages arent allowed", "Blank Message Warning");
                            }
                        }
                    }).Start();
                }
                else
                {
                    new Thread(() => {
                        var files = Directory.GetFiles(Path.Combine(API.Helper.getModDirectory(this), "offlineMessages"), "*.txt");
                        var message = File.ReadAllText(files[new Random().Next(files.Length)]);
                        MessageBox.Show("Unable to contact Goose Mail, here is a sample message\n\n" + message, "A premade message");
                    }).Start();
                }
            }

            if ((GetAsyncKeyState(Keys.F10) != 0) && !failsafe)
            {
                failsafe = true;
                new Thread(() => {
                    var files = Directory.GetFiles(Path.Combine(API.Helper.getModDirectory(this), "offlineMessages"), "*.txt");
                    var message = File.ReadAllText(files[new Random().Next(files.Length)]);
                    MessageBox.Show("Here is a premade message:\n\n" + message, "A premade message");
                }).Start();
            }
            
            
            if (GetAsyncKeyState(Keys.F10) == 0 && GetAsyncKeyState(Keys.F9) == 0)
            {
                failsafe = false;
            }
        }
    }
}
