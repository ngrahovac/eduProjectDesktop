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
        // section visibility
        public VisibilityToggle Visibility { get; set; } = new VisibilityToggle();

        // referenced view models for passing data
        public ProjectPageViewModel ProjectPageViewModel { get; set; }

        public SentApplicationsViewModel SentApplicationsViewModel { get; set; }

        public ReceivedApplicationsViewModel ReceivedApplicationsViewModel { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public CreateProjectViewModel CreateProjectViewModel { get; set; }

        public string SectionName { get; set; } = "Sekcija";

        // project snippets and selected snippet
        public ObservableCollection<ProjectSnippet> ProjectSnippets { get; set; } = new ObservableCollection<ProjectSnippet>();

        private int selectedSnippetIndex = -1;

        public int SelectedSnippetIndex { get { return selectedSnippetIndex; } set { selectedSnippetIndex = value; OnPropertyChanged(); } }

        public ProjectSnippet SelectedSnippet { get; set; }

        // menu item selection

        private object selectedMenuItem;

        // TODO: trebamo li ovdje zvati event? selekcija se ne mijenja sa user strane.
        public object SelectedMenuItem { get { return selectedMenuItem; } set { selectedMenuItem = value; OnPropertyChanged(); } }

        public HomepageViewModel()
        {

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
                    case "Novi projekat":
                        {
                            await DisplayCreateProjectPageAsync();
                            break;
                        }
                }
            }
        }

        public async Task LoadProjects()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                ProjectSnippets.Clear();
                Visibility.ChangeVisibility(true, "Homepage");
            });

            IEnumerable<Project> projects = await ((App)App.Current).projects.GetAllAsync();

            foreach (var project in projects)
            {
                User author = await ((App)App.Current).users.GetAsync(project.AuthorId);
                ProjectSnippet snippet = ProjectSnippet.FromProject(project, author);

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    ProjectSnippets.Add(snippet);
                });
            }

        }

        public async void ShowProjectPage()
        {
            if (SelectedSnippet != null)
            {

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    Visibility.ChangeVisibility(true, "ProjectPage");
                });

                await ProjectPageViewModel.SetSelectedProject(SelectedSnippet.ProjectId);
            }
        }

        // TODO: zasad je samo kad posjetimo project page, pa da se vratimo na homepage
        public async void CloseProjectPage()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                Visibility.ChangeVisibility(true, "Homepage");
                SelectedSnippetIndex = -1;  // selected index is twoway bound; this deselects list items
            });
        }

        public async Task DisplaySentApplicationsAsync()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            async () =>
            {
                Visibility.ChangeVisibility(true, "SentApplications");
                await SentApplicationsViewModel.LoadSentApplications();
            });
        }

        public async Task DisplayReceivedApplicationsAsync()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            async () =>
            {
                Visibility.ChangeVisibility(true, "ReceivedApplications");
                await ReceivedApplicationsViewModel.LoadReceivedApplicationsAsync();
                await ReceivedApplicationsViewModel.SetControlsVisibilityAsync();
            });
        }

        public async Task DisplayCreateProjectPageAsync()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            async () =>
            {
                Visibility.ChangeVisibility(true, "CreateProject");
                await CreateProjectViewModel.PopulateFieldData();

            });
        }
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
