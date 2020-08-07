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

namespace eduProjectDesktop.ViewModel
{
    public class ReceivedApplicationsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ProjectApplication> receivedApplications;
        public ObservableCollection<ProjectApplication> ReceivedApplications { get { return receivedApplications; } set { receivedApplications = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;

        public async Task LoadReceivedApplicationsAsync()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            async () =>
            {
                ReceivedApplications = new ObservableCollection<ProjectApplication>(await ((App)App.Current).applications.GetAllByAuthorAsync(9));
            });
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
