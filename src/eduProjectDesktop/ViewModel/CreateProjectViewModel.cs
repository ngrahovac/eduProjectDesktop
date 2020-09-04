using eduProjectDesktop.Model.Domain;
using eduProjectDesktop.Model.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Networking.Sockets;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace eduProjectDesktop.ViewModel
{
    public class CreateProjectViewModel : INotifyPropertyChanged
    {
        public ProjectInputModel ProjectInputModel { get; set; } = new ProjectInputModel();

        public ObservableCollection<string> StudyFieldNames = new ObservableCollection<string>();

        public ObservableCollection<string> TagNames = new ObservableCollection<string>();

        public ObservableCollection<string> AddedTags = new ObservableCollection<string>();

        public ObservableCollection<string> CollaboratorProfileTypes = new ObservableCollection<string>(new List<string> { "Student", "Nastavno osoblje" });

        public ObservableCollection<string> FacultyNames = new ObservableCollection<string>();

        public ObservableCollection<string> ProgramNames = new ObservableCollection<string>();

        public ObservableCollection<string> SpecializationNames = new ObservableCollection<string>();

        public ObservableCollection<int> StudyCycles = new ObservableCollection<int>();

        public ObservableCollection<StudentProfileInputModel> StudentProfileInputModels = new ObservableCollection<StudentProfileInputModel>();

        public ObservableCollection<FacultyMemberProfileInputModel> FacultyMemberProfileInputModels = new ObservableCollection<FacultyMemberProfileInputModel>();

        public string ChosenType { get; set; }

        private bool creatingStudentProfile = false;
        public bool CreatingStudentProfile
        {
            get { return creatingStudentProfile; }
            set { creatingStudentProfile = value; OnPropertyChanged(); }
        }

        public bool creatingFacultyMemberProfile { get; set; } = false;
        public bool CreatingFacultyMemberProfile
        {
            get { return creatingFacultyMemberProfile; }
            set { creatingFacultyMemberProfile = value; OnPropertyChanged(); }
        }

        // ne resetuje se prilikom dodavanja profila; obavezno polje
        private string profileDescription;
        public string ProfileDescription
        {
            get { return profileDescription; }
            set
            {
                profileDescription = value;
                if (CreatingStudentProfile) { StudentProfileInputModel.Description = value; }
                else if (creatingFacultyMemberProfile) { FacultyMemberProfileInputModel.Description = value; }
            }
        }
        public StudentProfileInputModel StudentProfileInputModel { get; set; } = new StudentProfileInputModel();

        public FacultyMemberProfileInputModel FacultyMemberProfileInputModel { get; set; } = new FacultyMemberProfileInputModel();

        private string selectedProjectStudyFieldName;
        public string SelectedProjectStudyFieldName
        {
            get { return selectedProjectStudyFieldName; }
            set { selectedProjectStudyFieldName = value; ProjectInputModel.StudyFieldName = value; }
        }

        private string selectedFacultyMemberStudyFieldName;
        public string SelectedFacultyMemberStudyFieldName
        {
            get { return selectedFacultyMemberStudyFieldName; }
            set { selectedFacultyMemberStudyFieldName = value; FacultyMemberProfileInputModel.StudyFieldName = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task PopulateFieldData()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                var studyFieldNames = new ObservableCollection<string>(((App)App.Current).faculties.GetAllStudyFields().Select(e => e.Name));
                foreach (var item in studyFieldNames)
                    StudyFieldNames.Add(item);

                var tagNames = new ObservableCollection<string>(((App)App.Current).tags.GetAll().Select(e => e.Name));
                foreach (var tag in tagNames)
                    TagNames.Add(tag);

                var facultyNames = new ObservableCollection<string>(((App)App.Current).faculties.GetAll().Select(f => f.Name));
                foreach (var f in facultyNames)
                    FacultyNames.Add(f);
            });
        }

        public async void TagChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            string tagName = (string)args.SelectedItem;
            if (!ProjectInputModel.TagNames.Contains(tagName))
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    ProjectInputModel.TagNames.Add((string)args.SelectedItem);
                    AddedTags.Add(tagName);
                });
            }
        }

        public void TagSearched(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = TagNames.Where(t => t.StartsWith(sender.Text));
            }
        }

        public async void TagRemoved(object sender, ItemClickEventArgs e)
        {
            string tagName = (string)e.ClickedItem;
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                ProjectInputModel.TagNames.Remove(tagName);
                AddedTags.Remove(tagName);
            });
        }

        // profili saradnika

        public async void CollaboratorProfileTypeChosenAsync(object sender, SelectionChangedEventArgs e)
        {
            string type = (string)e.AddedItems.ElementAt(0);
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                CreatingStudentProfile = type == "Student" ? true : false;
                CreatingFacultyMemberProfile = !CreatingStudentProfile;
            });

        }

        public void FacultySearched(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = FacultyNames.Where(f => f.StartsWith(sender.Text));
            }
        }

        public async void FacultyChosenAsync(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            string facultyName = (string)args.SelectedItem;

            if (CreatingStudentProfile)
            {
                StudentProfileInputModel.FacultyName = facultyName;

                var cycles = ((App)App.Current).faculties.GetAll().First(f => f.Name == facultyName).StudyPrograms.Values.Select(sp => sp.Cycle).Distinct();

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    StudyCycles.Clear();
                    foreach (var cycle in cycles)
                        StudyCycles.Add(cycle);
                });
            }
            else if (CreatingFacultyMemberProfile)
            {
                FacultyMemberProfileInputModel.FacultyName = facultyName;
            }

        }

        public async void CycleChosenAsync(object sender, SelectionChangedEventArgs e)
        {
            int cycle = (int)e.AddedItems.ElementAt(0);
            StudentProfileInputModel.StudyCycle = cycle;

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                ProgramNames.Clear();
                var programNames = ((App)App.Current).faculties.GetAll().First(f => f.Name == StudentProfileInputModel.FacultyName)
                                                                        .StudyPrograms.Values.Where(sp => sp.Cycle == cycle).Select(sp => sp.Name);
                foreach (var name in programNames)
                    ProgramNames.Add(name);
            });
        }

        public async void ProgramChosenAsync(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            string programName = (string)args.SelectedItem;
            StudentProfileInputModel.StudyProgramName = programName;

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                SpecializationNames.Clear();
                var specializationNames = ((App)App.Current).faculties.GetAll().First(f => f.Name == StudentProfileInputModel.FacultyName)
                                                                               .StudyPrograms.Values.First(sp => sp.Name == programName)
                                                                               .StudyProgramSpecializations.Values.Select(sps => sps.Name);
                foreach (var name in specializationNames)
                    SpecializationNames.Add(name);
            });
        }

        public void ProgramSearched(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = ProgramNames.Where(p => p.StartsWith(sender.Text));
            }
        }

        public void SpecializationChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            string specializationName = (string)args.SelectedItem;
            StudentProfileInputModel.StudyProgramSpecializationName = specializationName;
        }

        public void SpecializationSearched(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = SpecializationNames.Where(s => s.StartsWith(sender.Text));
            }
        }

        public async void AddProfile()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                if (CreatingStudentProfile)
                {
                    StudentProfileInputModels.Add(StudentProfileInputModel);
                    ProjectInputModel.CollaboratorProfileInputModels.Add(StudentProfileInputModel);
                }
                else if (CreatingFacultyMemberProfile)
                {
                    FacultyMemberProfileInputModels.Add(FacultyMemberProfileInputModel);
                    ProjectInputModel.CollaboratorProfileInputModels.Add(FacultyMemberProfileInputModel);
                }
            });
        }

        public async void SaveProject()
        {
            if (RequiredProjectFieldsFilled())
            {
                Project project = ProjectInputModel.ToProject(ProjectInputModel);
                await ((App)App.Current).projects.CreateAsync(project);

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    AddedTags.Clear();
                });

                ProjectInputModel = new ProjectInputModel();
                StudentProfileInputModel = new StudentProfileInputModel();
                FacultyMemberProfileInputModel = new FacultyMemberProfileInputModel();
            }
        }

        private bool RequiredProjectFieldsFilled()
        {
            if (ProjectInputModel.Title != null && ProjectInputModel.Title.Trim() != ""
                && ProjectInputModel.Description != null && ProjectInputModel.Description.Trim() != ""
                && ProjectInputModel.StudyFieldName != null
                && ProjectInputModel.CollaboratorProfileInputModels.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Cancel()
        {
            ProjectInputModel = new ProjectInputModel();
            StudentProfileInputModel = new StudentProfileInputModel();
            FacultyMemberProfileInputModel = new FacultyMemberProfileInputModel();
        }





    }
}
