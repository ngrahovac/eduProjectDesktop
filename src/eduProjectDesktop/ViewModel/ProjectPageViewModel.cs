using eduProjectDesktop.Model.Display;
using eduProjectDesktop.Model.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace eduProjectDesktop.ViewModel
{
    public class ProjectPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // controls visibility

        private Visibility editButtonVisibility = Visibility.Collapsed;

        public Visibility EditButtonVisibility
        {
            get { return editButtonVisibility; }

            set
            {
                editButtonVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility deleteButtonVisibility = Visibility.Collapsed;

        public Visibility DeleteButtonVisibility
        {
            get { return deleteButtonVisibility; }

            set
            {
                deleteButtonVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility applyButtonVisibility = Visibility.Collapsed;

        public Visibility ApplyButtonVisibility
        {
            get { return applyButtonVisibility; }

            set
            {
                applyButtonVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility revokeButtonVisibility = Visibility.Collapsed;

        public Visibility RevokeButtonVisibility
        {
            get { return revokeButtonVisibility; }

            set
            {
                revokeButtonVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility saveButtonVisibility = Visibility.Collapsed;

        public Visibility SaveButtonVisibility
        {
            get { return saveButtonVisibility; }

            set
            {
                saveButtonVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility cancelButtonVisibility = Visibility.Collapsed;

        public Visibility CancelButtonVisibility
        {
            get { return cancelButtonVisibility; }

            set
            {
                cancelButtonVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility closeProjectButtonVisibility = Visibility.Collapsed;

        public Visibility CloseProjectButtonVisibility
        {
            get { return closeProjectButtonVisibility; }

            set
            {
                closeProjectButtonVisibility = value;
                OnPropertyChanged();
            }
        }

        // controlling selected project

        public Project SelectedProject { get; set; }

        private Visibility activeProjectVisibility = Visibility.Collapsed;

        public Visibility ActiveProjectVisibility
        {
            get { return activeProjectVisibility; }

            set
            {
                if (value == Visibility.Visible)
                    ClosedProjectVisibility = Visibility.Collapsed;
                activeProjectVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility closedProjectVisibility = Visibility.Collapsed;

        public Visibility ClosedProjectVisibility
        {
            get { return closedProjectVisibility; }
            set
            {
                if (value == Visibility.Visible)
                    ActiveProjectVisibility = Visibility.Collapsed;
                closedProjectVisibility = value;
                OnPropertyChanged();
            }
        }

        private ActiveProjectOverview activeProjectOverview;

        public ActiveProjectOverview ActiveProjectOverview
        {
            get { return activeProjectOverview; }

            set
            {
                activeProjectOverview = value;
                OnPropertyChanged();
            }
        }

        private ClosedProjectOverview closedProjectOverview;

        public ClosedProjectOverview ClosedProjectOverview
        {
            get { return closedProjectOverview; }

            set
            {
                closedProjectOverview = value;
                OnPropertyChanged();
            }
        }

        public async Task SetSelectedProject(int id)
        {
            SelectedProject = await ((App)App.Current).projects.GetAsync(id);
            SetControlsVisibilityAsync();

            User user = await ((App)App.Current).users.GetAsync(SelectedProject.AuthorId);
            ProjectStatus status = SelectedProject.ProjectStatus;

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            async () =>
            {
                if (SelectedProject.ProjectStatus == ProjectStatus.Active)
                {
                    ActiveProjectOverview = ActiveProjectOverview.FromProject(SelectedProject, user);
                    ActiveProjectVisibility = Visibility.Visible;
                }
                else if (SelectedProject.ProjectStatus == ProjectStatus.Closed)
                {
                    List<User> collaborators = new List<User>();
                    foreach (int profileId in SelectedProject.CollaboratorIds)
                        collaborators.Add(await ((App)App.Current).users.GetAsync(profileId));

                    ClosedProjectOverview = ClosedProjectOverview.FromProject(SelectedProject, user, collaborators);
                    ClosedProjectVisibility = Visibility.Visible;
                }
            });
        }

        // sets initial button visibility
        private async void SetControlsVisibilityAsync()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                if (SelectedProject.ProjectStatus == ProjectStatus.Active)
                {
                    if (SelectedProject.AuthorId == User.CurrentUserId)
                    {
                        EditButtonVisibility = Visibility.Visible;
                        DeleteButtonVisibility = Visibility.Visible;
                        SaveButtonVisibility = Visibility.Visible;
                        CancelButtonVisibility = Visibility.Visible;
                        CloseProjectButtonVisibility = Visibility.Visible;
                    }
                    else
                    {
                        ApplyButtonVisibility = Visibility.Visible;
                        RevokeButtonVisibility = Visibility.Visible;
                    }
                }
                else if (SelectedProject.ProjectStatus == ProjectStatus.Closed)
                {
                    if (SelectedProject.AuthorId == User.CurrentUserId)
                    {
                        DeleteButtonVisibility = Visibility.Visible;
                    }
                    else
                    {

                    }
                }
            });

        }

        // selected student or faculty member profile are bound to this property
        //public CollaboratorProfileOverview SelectedProfile { get; set; }

        public StudentProfileOverview SelectedStudentProfileOverview { get; set; }

        public FacultyMemberProfileOverview SelectedFacultyMemberProfileOverview
        {
            get;
            set;
        }

        // TODO: kad se selectuje jedan, da se ugasi drugi

        private int selectedStudentProfileIndex = -1;

        public int SelectedStudentProfileIndex
        {
            get { return selectedStudentProfileIndex; }
            set { selectedStudentProfileIndex = value; OnPropertyChanged(); }
        }

        private int selectedFacultyMemberProfileIndex = -1;

        public int SelectedFacultyMemberProfileIndex
        {
            get { return selectedFacultyMemberProfileIndex; }
            set
            {
                selectedFacultyMemberProfileIndex = value;
                OnPropertyChanged();
            }
        }


        public async void ApplyForPosition()
        {
            CollaboratorProfileOverview profile = null;

            if (SelectedFacultyMemberProfileOverview != null)
                profile = SelectedFacultyMemberProfileOverview;
            else if (SelectedStudentProfileOverview != null)
                profile = SelectedStudentProfileOverview;

            if (profile != null)
            {
                ProjectApplication application = new ProjectApplication
                {
                    ApplicantId = User.CurrentUserId,
                    ApplicantComment = ""
                };

                // searching for selected collaborator profile
                if (selectedFacultyMemberProfileIndex != -1)
                {
                    application.CollaboratorProfileId = SelectedProject.CollaboratorProfiles.Where(p => p is FacultyMemberProfile).ElementAt(selectedFacultyMemberProfileIndex).CollaboratorProfileId;

                }
                else if (selectedStudentProfileIndex != -1)
                {
                    application.CollaboratorProfileId = SelectedProject.CollaboratorProfiles.Where(p => p is StudentProfile).ElementAt(selectedStudentProfileIndex).CollaboratorProfileId;

                }

                Project project = await ((App)App.Current).projects.GetByCollaboratorProfileAsync(application.CollaboratorProfileId);
                if (project.ProjectStatus != ProjectStatus.Closed)
                {
                    await ((App)App.Current).applications.AddAsync(application);
                }
            }
        }

        public async Task RevokeApplicationAsync()
        {
            CollaboratorProfileOverview profile = null;

            if (SelectedFacultyMemberProfileOverview != null)
                profile = SelectedFacultyMemberProfileOverview;
            else if (SelectedStudentProfileOverview != null)
                profile = SelectedStudentProfileOverview;


            if (profile != null)

            {
                ProjectApplication application = new ProjectApplication
                {
                    ApplicantId = User.CurrentUserId,
                    ApplicantComment = ""
                };

                if (selectedFacultyMemberProfileIndex != -1)
                {
                    application.CollaboratorProfileId = SelectedProject.CollaboratorProfiles.Where(p => p is FacultyMemberProfile).ElementAt(selectedFacultyMemberProfileIndex).CollaboratorProfileId;

                }
                else if (selectedStudentProfileIndex != -1)
                {
                    application.CollaboratorProfileId = SelectedProject.CollaboratorProfiles.Where(p => p is StudentProfile).ElementAt(selectedStudentProfileIndex).CollaboratorProfileId;
                }

                await ((App)App.Current).applications.RemoveAsync(application);
            }
        }

        public async void StudentProfileSelected()
        {
            // TODO: fix
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                SelectedFacultyMemberProfileOverview = null;
                selectedFacultyMemberProfileIndex = -1;
            });

        }

        public async void FacultyMemberProfileSelected()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                SelectedStudentProfileOverview = null;
                selectedStudentProfileIndex = -1;
            });

        }

        public void EditProjectAsync()
        {
            // enable edit controls 
            Debug.WriteLine("enable fields for edit");

            // throw new NotImplementedException();
        }

        public void DeleteProject()
        {
            throw new NotImplementedException();
        }

        public async Task CloseProjectAsync()
        {
            SelectedProject.ProjectStatus = ProjectStatus.Closed;
            await ((App)App.Current).projects.UpdateAsync(SelectedProject);
        }

        public async void SaveChangesAsync()
        {
            // edit enablovao kontrole, user izmijenio polja
            // save button updateuje projekat

            SelectedProject.Title = ActiveProjectOverview.Title;
            SelectedProject.Description = ActiveProjectOverview.Description;
            // start date, end date, study field
            // tags
            // status u drugoj fji

            await ((App)App.Current).projects.UpdateAsync(SelectedProject);
        }

        public void CancelChangesAsync()
        {
            throw new NotImplementedException();
        }


        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
