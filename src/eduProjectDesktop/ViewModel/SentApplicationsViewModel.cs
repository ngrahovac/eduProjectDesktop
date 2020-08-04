using eduProjectDesktop.Model.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace eduProjectDesktop.ViewModel
{
    public class SentApplicationsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ProjectApplication> sentApplications;
        public ObservableCollection<ProjectApplication> SentApplications { get { return sentApplications; } set { sentApplications = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;

        public async Task LoadSentApplications()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            async () =>
            {
                SentApplications = new ObservableCollection<ProjectApplication>(await ((App)App.Current).applications.GetByUserAsync(User.CurrentUserId));
            });
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
