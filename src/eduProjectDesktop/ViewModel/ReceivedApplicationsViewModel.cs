using eduProjectDesktop.Model.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace eduProjectDesktop.ViewModel
{
    public class ReceivedApplicationsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ProjectApplication> receivedApplications;
        public ObservableCollection<ProjectApplication> ReceivedApplications { get { return receivedApplications; } set { receivedApplications = value; OnPropertyChanged(); } }

        public ProjectApplication SelectedApplication { get; set; }

        private Visibility acceptApplicationButtonVisibility = Visibility.Collapsed;

        public Visibility AcceptApplicationButtonVisibility
        {
            get { return acceptApplicationButtonVisibility; }

            set
            {
                acceptApplicationButtonVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility rejectApplicationButtonVisibility = Visibility.Collapsed;

        public Visibility RejectApplicationButtonVisibility
        {
            get { return rejectApplicationButtonVisibility; }

            set
            {
                rejectApplicationButtonVisibility = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public async Task LoadReceivedApplicationsAsync()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            async () =>
            {
                ReceivedApplications = new ObservableCollection<ProjectApplication>(await ((App)App.Current).applications.GetAllOnHoldByAuthorAsync(9));
            });
        }

        public async Task SetControlsVisibilityAsync()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                // ako je projekat zatvoren, ne mozemo editovati?
                AcceptApplicationButtonVisibility = Visibility.Visible;
                RejectApplicationButtonVisibility = Visibility.Visible;
            });
        }

        public async Task AcceptApplicationAsync()
        {
            if (SelectedApplication != null)
            {
                SelectedApplication.ProjectApplicationStatus = ProjectApplicationStatus.Accepted;
                await ((App)App.Current).applications.UpdateAsync(SelectedApplication);

                // registering collaborator

                Project project = await ((App)App.Current).projects.GetByCollaboratorProfileAsync(SelectedApplication.CollaboratorProfileId);
                project.AddCollaboratorId(SelectedApplication.ApplicantId);

                await ((App)App.Current).projects.UpdateAsync(project);
            }
        }

        public async Task RejectApplicationAsync()
        {
            if (SelectedApplication != null)
            {
                SelectedApplication.ProjectApplicationStatus = ProjectApplicationStatus.Rejected;
                await ((App)App.Current).applications.UpdateAsync(SelectedApplication);
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
