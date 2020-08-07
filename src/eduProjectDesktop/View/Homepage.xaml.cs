using eduProjectDesktop.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace eduProjectDesktop.View
{
    public sealed partial class Homepage : Page
    {
        public HomepageViewModel HomepageViewModel { get; set; }

        public ProjectPageViewModel ProjectPageViewModel { get; set; }

        public SentApplicationsViewModel SentApplicationsViewModel { get; set; }

        public ReceivedApplicationsViewModel ReceivedApplicationsViewModel { get; set; }
        public Homepage()
        {
            this.InitializeComponent();
            HomepageViewModel = new HomepageViewModel();

            ProjectPageViewModel = new ProjectPageViewModel();
            HomepageViewModel.ProjectPageViewModel = ProjectPageViewModel;

            SentApplicationsViewModel = new SentApplicationsViewModel();
            HomepageViewModel.SentApplicationsViewModel = SentApplicationsViewModel;

            ReceivedApplicationsViewModel = new ReceivedApplicationsViewModel();
            HomepageViewModel.ReceivedApplicationsViewModel = ReceivedApplicationsViewModel;
        }
    }
}
