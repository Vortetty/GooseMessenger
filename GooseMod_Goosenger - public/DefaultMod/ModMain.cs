// 1. Added the "GooseModdingAPI" project as a reference.
// 2. Compile this.
// 3. Create a folder with this DLL in the root, and *no GooseModdingAPI DLL*
using GooseShared;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Xml;

namespace GooseMessaging
{
    public class ModEntryPoint : IMod
    {
        // Gets called automatically, passes in a class that contains pointers to
        // useful functions we need to interface with the goose.
        void IMod.Init()
        {
            // Subscribe to whatever events we want
            InjectionPoints.PostTickEvent += PostTick;
        }

        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(Keys vKey);
        bool failsafe = false;
        string input;

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
                            document.Load("php submit url goes here" + System.Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(input)));
                            string webData = document.InnerText;
                            if (webData == "saved")
                            {
                                MessageBox.Show("Message sent successfully!", "Message Sent");
                                var getmessage = new Form2();
                                DialogResult dialogResult = getmessage.ShowDialog();
                                if (dialogResult == DialogResult.Yes)
                                {
                                    XmlDocument document1 = new XmlDocument();
                                    document1.Load("php retrive url goes here");
                                    string webData1 = document1.InnerText;
                                    MessageBox.Show(System.Text.ASCIIEncoding.ASCII.GetString(System.Convert.FromBase64String(webData1)), "Message From Other Geese");
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
                        var messages = new JavaScriptSerializer().Deserialize<List<string>>(File.ReadAllText(Path.Combine(API.Helper.getModDirectory(this), "messages.json")));
                        MessageBox.Show("Unable to contact Goose Mail, here is a sample message\n\n" + messages[new Random().Next(messages.Count)], "A premade message");
                    }).Start();
                }
            }
            else if (GetAsyncKeyState(Keys.F9) == 0)
            {
                failsafe = false;
            }
        }
    }
}
