using eduProjectDesktop.Model.Display;
using eduProjectDesktop.Model.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace eduProjectDesktop.ViewModel
{
    public class ProjectPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Project SelectedProject { get; set; }

        private ProjectOverview projectOverview = null;
        public ProjectOverview ProjectOverview
        {
            get { return projectOverview; }
            set
            {
                projectOverview = value;
                OnPropertyChanged();
            }
        }

        public CollaboratorProfileOverview SelectedProfile { get; set; } // ne raisati property changed na twoway bound selected item!!!!!!!!!!!!!!

        private int selectedStudentProfileIndex = -1; // ako ne setujem na -1 baca stackoverflow

        public int SelectedStudentProfileIndex { get { return selectedStudentProfileIndex; } set { selectedStudentProfileIndex = value; OnPropertyChanged(); } }

        private int selectedFacultyMemberProfileIndex = -1;

        public int SelectedFacultyMemberProfileIndex { get { return selectedFacultyMemberProfileIndex; } set { selectedFacultyMemberProfileIndex = value; OnPropertyChanged(); } }


        public async void ApplyForPosition()
        {
            if (SelectedProfile == null)
                throw new NotImplementedException();
            else
            {
                ProjectApplication application = new ProjectApplication
                {
                    ApplicantId = User.CurrentUserId,
                    ApplicantComment = ""
                };

                // imamo indeks profila iz liste
                // sad trebamo iz project.CollaboratorProfiles naci id onog koji je stvarno izabran
                if (selectedFacultyMemberProfileIndex != -1)
                {
                    application.CollaboratorProfileId = SelectedProject.CollaboratorProfiles.Where(p => p is FacultyMemberProfile).ElementAt(selectedFacultyMemberProfileIndex).CollaboratorProfileId;

                }
                else if (selectedStudentProfileIndex != -1)
                {
                    application.CollaboratorProfileId = SelectedProject.CollaboratorProfiles.Where(p => p is StudentProfile).ElementAt(selectedStudentProfileIndex).CollaboratorProfileId;

                }

                // REGISTRUJ PRIJAVU
                await ((App)App.Current).applications.AddAsync(application);
            }
        }

        public async void StudentProfileSelected()
        {
            // ako je i profil nastavnog saradnika izabran, deselektuj ga

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                selectedFacultyMemberProfileIndex = -1;
            });

        }

        public async void FacultyMemberProfileSelected()
        {
            // ako je i profil studenta izabran, deselektuj ga
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                selectedStudentProfileIndex = -1;
            });

        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
