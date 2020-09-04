using eduProjectDesktop.Model.Display;
using eduProjectDesktop.Model.Domain;
using eduProjectDesktop.Model.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace eduProjectDesktop.ViewModel
{
    public class ProjectPageViewModel : INotifyPropertyChanged
    {

        public HomepageViewModel HomepageViewModel { get; set; }

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

        public ObservableCollection<string> AddedTags = new ObservableCollection<string>();
        public ObservableCollection<string> SuggestedTags { get; set; } = new ObservableCollection<string>();

        public void TagSearched(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = SuggestedTags.Where(t => t.StartsWith(sender.Text));
            }
        }
        public async void TagChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                if (!AddedTags.Contains((string)args.SelectedItem))
                {
                    AddedTags.Add((string)args.SelectedItem);
                    ProjectInputModel.TagNames.Add((string)args.SelectedItem);
                }
            });

        }

        public Project SelectedProject { get; set; }

        public ProjectInputModel ProjectInputModel;

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

        private bool isEditDisabled = true;
        public bool IsEditDisabled { get { return isEditDisabled; } set { isEditDisabled = value; OnPropertyChanged(); } }

        public bool isTagSearchEnabled = false;
        public bool IsTagSearchEnabled { get { return isTagSearchEnabled; } set { isTagSearchEnabled = value; OnPropertyChanged(); } }

        public List<string> StudyFieldNames = new List<string>();

        private string selectedStudyFieldName;
        public string SelectedStudyFieldName { get { return selectedStudyFieldName; } set { selectedStudyFieldName = value; OnPropertyChanged(); } }

        public async Task SetSelectedProject(int id)
        {
            // populating combo boxes
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                IEnumerable<Tag> tags = ((App)App.Current).tags.GetAll();
                foreach (var tag in tags)
                    SuggestedTags.Add(tag.Name);

                IEnumerable<string> studyFieldNames = ((App)App.Current).faculties.GetAllStudyFields().Select(sf => sf.Name);
                foreach (var name in studyFieldNames)
                    StudyFieldNames.Add(name);
            });


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
                        ActiveProjectOverview.SetCollaboratorProfileOverviews(SelectedProject);
                        ActiveProjectVisibility = Visibility.Visible;
                        SelectedStudyFieldName = ActiveProjectOverview.StudyFieldName;

                        AddedTags.Clear();
                        foreach (var tag in SelectedProject.Tags)
                        {
                            AddedTags.Add(tag.Name);
                        }
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
                        DeleteButtonVisibility = Visibility.Collapsed;
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

        public void DeleteProject()
        {
            throw new NotImplementedException();
        }

        public async Task CloseProjectAsync()
        {
            SelectedProject.ProjectStatus = ProjectStatus.Closed;
            await ((App)App.Current).projects.UpdateAsync(SelectedProject);
        }

        private bool isEditEnabled;
        public bool IsEditEnabled { get { return isEditEnabled; } set { isEditEnabled = value; OnPropertyChanged(); } }
        public async Task EditProjectAsync()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                IsEditDisabled = false;
                IsEditEnabled = true;

                IsTagSearchEnabled = true;
                ProjectInputModel = new ProjectInputModel
                {
                    Title = SelectedProject.Title,
                    Description = SelectedProject.Description,
                    StudyFieldName = SelectedProject.StudyField.Name
                };

                if (SelectedProject.StartDate != null)
                    ProjectInputModel.StartDate = new DateTimeOffset((DateTime)SelectedProject.StartDate);
                if (SelectedProject.EndDate != null)
                    ProjectInputModel.EndDate = new DateTimeOffset((DateTime)SelectedProject.EndDate);

                foreach (var tag in SelectedProject.Tags)
                    ProjectInputModel.TagNames.Add(tag.Name);
            });
        }

        public async void StudyFieldSelected(object sender, SelectionChangedEventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                ActiveProjectOverview.StudyFieldName = (string)e.AddedItems.ElementAt(0);
                selectedStudyFieldName = (string)e.AddedItems.ElementAt(0);
            });
        }

        public async void SaveChangesAsync()
        {
            Project project = ProjectInputModel.ToProject(ProjectInputModel);
            project.ProjectId = SelectedProject.ProjectId;

            await ((App)App.Current).projects.UpdateAsync(project);

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
           async () =>
           {
               IsEditDisabled = true;
               IsEditEnabled = false;
               SelectedProject = await ((App)App.Current).projects.GetAsync(SelectedProject.ProjectId); ;
           });

        }

        public async void CancelChangesAsync()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            async () =>
            {
                IsEditDisabled = true;
                IsEditEnabled = false;
                await HomepageViewModel.LoadProjects();
            });
        }


        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
