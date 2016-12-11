using Plugin.TextToSpeech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MorseCode
{
    public partial class MorseCodePage : ContentPage
    {
        private Translate translator;
        private QueueGate queueGate;
        private string result;
        public MorseCodePage()
        {
            InitializeComponent();
            translator = new Translate();
            queueGate = new QueueGate();
            result = "";
            userText.Focus();
            Device.StartTimer(TimeSpan.FromMilliseconds(1000), checkMes);
        }
        private async void onSendButtonClicked(object sender, EventArgs args)
        {
            string res = translator.toMorse(userText.Text);
            resultLabel.Text = res;
            bool ok = await queueGate.SendMessage(res);
        }
        private bool checkMes()
        {
            getMessege();
            return true;
        }

        private async void getMessege()
        {
            string message = await queueGate.GetLastMessage();
            if (message != null)
            {
                if (message == "*")
                {
                    messageLabel.Text = "";
                    result = "";
                }
                else if(message == "#")
                {
    
                    messageLabel.Text = result;
                    CrossTextToSpeech.Current.Speak(result);
                }
                else
                {
                    result += translator.toWord(message);
                    
                }
            }

        }
    }
}
