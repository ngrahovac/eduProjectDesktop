using eduProjectDesktop.Model.Display;
using eduProjectDesktop.Model.Domain;
using eduProjectDesktop.View;
using System;
using System.Collections;
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
    public class HomepageViewModel : INotifyPropertyChanged
    {
        public VisibilityToggle Visibility { get; set; } = new VisibilityToggle();

        public ProjectPageViewModel ProjectPageViewModel { get; set; }

        public SentApplicationsViewModel SentApplicationsViewModel { get; set; }

        public ReceivedApplicationsViewModel ReceivedApplicationsViewModel { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public string SectionName { get; set; } = "Sekcija";

        public ObservableCollection<ProjectSnippet> ProjectSnippets { get; set; } = new ObservableCollection<ProjectSnippet>();

        private int selectedSnippetIndex = -1;

        public int SelectedSnippetIndex { get { return selectedSnippetIndex; } set { selectedSnippetIndex = value; OnPropertyChanged(); } }

        public ProjectSnippet SelectedSnippet { get; set; }

        private object selectedMenuItem;

        public object SelectedMenuItem { get { return selectedMenuItem; } set { selectedMenuItem = value; OnPropertyChanged(); } }

        public HomepageViewModel()
        {

        }

        public async Task LoadProjects()
        {
            Project project = await ((App)App.Current).projects.GetAsync(1); // zasad samo jedan
            User author = await ((App)App.Current).users.GetAsync(project.AuthorId);

            ProjectSnippet snippet = ProjectSnippet.FromProject(project, author);

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                Visibility.ChangeVisibility(true, "Homepage");

                ProjectSnippets.Clear();
                ProjectSnippets.Add(snippet);
            });
        }

        public async void ShowProjectPage()
        {
            // kad treba prikazati project page, u ovom view modelu se obradjuje event na nacin da se sakriju potrebna polja
            // i posalje view modelu sta treba

            if (SelectedSnippet != null)
            {

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    Visibility.ChangeVisibility(true, "ProjectPage");
                });

                Project project = await ((App)App.Current).projects.GetAsync(SelectedSnippet.ProjectId);
                ProjectPageViewModel.SelectedProject = project; // SETUJEMO REF NA PROJEKAT DA VIEWMODEL MOZE REGISTROVATI
                User author = await ((App)App.Current).users.GetAsync(project.AuthorId);
                bool isAuthor = project.AuthorId == User.CurrentUserId;
                ProjectStatus status = project.ProjectStatus;

                // ako je aktivan, trebamo dohvatiti sve usere saradnike i sve to poslati na mapiranje view modelu
                // zasad samo radimo sa aktivnim projektima; display model isti, forme se razlikuju u kontrolama

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    ProjectPageViewModel.ProjectOverview = ProjectOverview.FromProject(project, author, isAuthor);
                });
            }
        }

        public async void CloseProjectPage()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                Visibility.ChangeVisibility(true, "Homepage");
                SelectedSnippetIndex = -1;  // TwoWay bound, so this will clear list item selection
            });
        }

        public async void MenuItemSelectionChanged()
        {
            if (SelectedMenuItem != null)
            {
                string content = (string)((NavigationViewItem)SelectedMenuItem).Content;
                switch (content)
                {
                    case "Moje prijave":
                        {
                            await DisplaySentApplicationsAsync();
                            break;
                        }
                    case "Početna":
                        {
                            await LoadProjects();
                            break;
                        }
                    case "Svi projekti":
                        {
                            break;
                        }
                    case "Moji projekti":
                        {
                            break;
                        }
                    case "Pristigle prijave":
                        {
                            await DisplayReceivedApplicationsAsync();
                            break;
                        }
                }
            }
        }

        private async Task DisplaySentApplicationsAsync()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            async () =>
            {
                Visibility.ChangeVisibility(true, "SentApplications");
                await SentApplicationsViewModel.LoadSentApplications();
            });
        }

        private async Task DisplayReceivedApplicationsAsync()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            async () =>
            {
                Visibility.ChangeVisibility(true, "ReceivedApplications");
                await ReceivedApplicationsViewModel.LoadReceivedApplicationsAsync();
            });
        }


        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
