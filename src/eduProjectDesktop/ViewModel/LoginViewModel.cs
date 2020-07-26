using eduProjectDesktop.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace eduProjectDesktop.ViewModel
{
    public class LoginViewModel
    {
        public void LoginButton_Click(object sender, RoutedEventArgs args)
        {
            // TODO: provjera kredencijala here
            Frame frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(Homepage));

        }
    }
}
