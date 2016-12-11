using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MorseCode
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            imageLogo.Source = Device.OnPlatform(
            iOS: ImageSource.FromFile("Images/morssenger.png"),
            Android: ImageSource.FromFile("Resources/drawable/morssenger.png"),
            WinPhone: ImageSource.FromFile("Assets/morssenger.png"));
        }
        private async void onStartButtonClicked(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new MorseCodePage());
        }
    }
}
