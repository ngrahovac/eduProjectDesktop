using eduProjectDesktop.Data;
using eduProjectDesktop.Model.Domain;
using eduProjectDesktop.View;
using System.Threading;
using System.Threading.Tasks;
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
        public readonly ProjectApplicationsRepository applications;
        public readonly TagsRepository tags;
        public readonly FacultiesRepository faculties;

        public App()
        {
            // Nina
            User.CurrentUserId = 9; // TODO: implement logging

            InitializeComponent();
            Suspending += OnSuspending;

            projects = new ProjectsRepository();
            users = new UsersRepository();
            applications = new ProjectApplicationsRepository();

            // FIX
            Task.Run(() => TagsRepository.CreateAsync()).Wait();
            tags = TagsRepository.instance;

            Task.Run(() => FacultiesRepository.CreateAsync()).Wait();
            faculties = FacultiesRepository.instance;
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
