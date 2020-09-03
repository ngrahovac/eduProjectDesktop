using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace eduProjectDesktop.ViewModel
{
    public class VisibilityToggle : INotifyPropertyChanged
    {
        private Visibility homepageVisibility = Visibility.Visible;
        public Visibility HomepageVisibility { get { return homepageVisibility; } set { homepageVisibility = value; OnPropertyChanged(); } }

        private Visibility projectPageVisibility = Visibility.Collapsed;
        public Visibility ProjectPageVisibility
        {
            get { return projectPageVisibility; }
            set { projectPageVisibility = value; OnPropertyChanged(); }
        }

        private Visibility sentApplicationsVisibility = Visibility.Collapsed;
        public Visibility SentApplicationsVisibility
        {
            get
            {
                return sentApplicationsVisibility;
            }
            set
            {
                sentApplicationsVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility receivedApplicationsVisibility = Visibility.Collapsed;

        public Visibility ReceivedApplicationsVisibility { get { return receivedApplicationsVisibility; } set { receivedApplicationsVisibility = value; OnPropertyChanged(); } }

        private Visibility createProjectVisibility = Visibility.Collapsed;

        public Visibility CreateProjectVisibility { get { return createProjectVisibility; } set { createProjectVisibility = value; OnPropertyChanged(); } }



        public event PropertyChangedEventHandler PropertyChanged;

        public void ChangeVisibility(bool isVisible, string section)
        {
            switch (section)
            {
                case "SentApplications":
                    {
                        SentApplicationsVisibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
                        HomepageVisibility = Visibility.Collapsed;
                        ProjectPageVisibility = Visibility.Collapsed;
                        ReceivedApplicationsVisibility = Visibility.Collapsed;
                        CreateProjectVisibility = Visibility.Collapsed;

                        break;
                    }
                case "Homepage":
                    {
                        HomepageVisibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
                        SentApplicationsVisibility = Visibility.Collapsed;
                        ReceivedApplicationsVisibility = Visibility.Collapsed;
                        ProjectPageVisibility = Visibility.Collapsed;
                        CreateProjectVisibility = Visibility.Collapsed;

                        break;
                    }
                case "ProjectPage":
                    {
                        ProjectPageVisibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
                        SentApplicationsVisibility = Visibility.Collapsed;
                        ReceivedApplicationsVisibility = Visibility.Collapsed;
                        HomepageVisibility = Visibility.Collapsed;
                        CreateProjectVisibility = Visibility.Collapsed;

                        break;
                    }
                case "ReceivedApplications":
                    {
                        ReceivedApplicationsVisibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
                        HomepageVisibility = Visibility.Collapsed;
                        ProjectPageVisibility = Visibility.Collapsed;
                        SentApplicationsVisibility = Visibility.Collapsed;
                        CreateProjectVisibility = Visibility.Collapsed;
                        break;
                    }
                case "CreateProject":
                    {
                        CreateProjectVisibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
                        HomepageVisibility = Visibility.Collapsed;
                        ProjectPageVisibility = Visibility.Collapsed;
                        SentApplicationsVisibility = Visibility.Collapsed;
                        ReceivedApplicationsVisibility = Visibility.Collapsed;
                        break;
                    }
            }


        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
