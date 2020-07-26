using eduProjectDesktop.Data;
using eduProjectDesktop.View;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// This code is almost identical to the Blank Application code.
// The key difference is that this project includes the declaration of 
// SupportsMultipleInstances in the package.appxmanifest.

namespace eduProjectDesktop
{
    sealed partial class App : Application
    {
        public readonly ProjectsRepository projects;
        public readonly UsersRepository users;

        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;

            // repositories jer nemam DI u UWP
            projects = new ProjectsRepository();
            users = new UsersRepository();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
            {
                rootFrame = new Frame();
                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application.
                }
                Window.Current.Content = rootFrame;
            }
            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    ApplicationView.PreferredLaunchViewSize = new Size(1200, 700);
                    ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

                    rootFrame.Navigate(typeof(Homepage), e.Arguments);
                }
                Window.Current.Activate();
            }
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity.
            deferral.Complete();
        }


    }
}
