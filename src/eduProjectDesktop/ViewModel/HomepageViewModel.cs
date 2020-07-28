using eduProjectDesktop.Model.Display;
using eduProjectDesktop.Model.Domain;
using eduProjectDesktop.View;
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

namespace eduProjectDesktop.ViewModel
{
    public class HomepageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string SectionName { get; set; } = "Sekcija";
        public ObservableCollection<ProjectSnippet> ProjectSnippets { get; set; } = new ObservableCollection<ProjectSnippet>();

        private int selectedSnippetIndex = -1;
        public int SelectedSnippetIndex { get { return selectedSnippetIndex; } set { selectedSnippetIndex = value; OnPropertyChanged(); } }
        public ProjectSnippet SelectedSnippet { get; set; }

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

        private Visibility isHomePageVisible = Visibility.Visible;
        public Visibility IsHomepageVisible { get { return isHomePageVisible; } set { isHomePageVisible = value; OnPropertyChanged(); } }

        private Visibility isProjectPageVisible = Visibility.Collapsed;
        public Visibility IsProjectPageVisible
        {
            get { return isProjectPageVisible; }
            set { isProjectPageVisible = value; OnPropertyChanged(); }
        }

        private bool ifEditing = false;

        public bool IfEditing { get { return ifEditing; } set { ifEditing = value; OnPropertyChanged(); } }
        public HomepageViewModel()
        {

        }
        public async void LoadProjects()
        {
            Project project = await ((App)App.Current).projects.GetAsync(1); // zasad samo jedan
            User author = await ((App)App.Current).users.GetAsync(project.AuthorId);

            ProjectSnippet snippet = ProjectSnippet.FromProject(project, author);

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                ProjectSnippets.Add(snippet);
            });
        }
        public async void ShowProjectPage()
        {
            if (SelectedSnippet != null)
            {

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    IsHomepageVisible = Visibility.Collapsed;
                    IsProjectPageVisible = Visibility.Visible;
                });

                Project project = await ((App)App.Current).projects.GetAsync(SelectedSnippet.ProjectId);
                User author = await ((App)App.Current).users.GetAsync(project.AuthorId);

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    ProjectOverview = ProjectOverview.FromProject(project, author);
                });
            }
        }
        public async void CloseProjectPage()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                IsHomepageVisible = Visibility.Visible;
                IsProjectPageVisible = Visibility.Collapsed;
                SelectedSnippetIndex = -1;  // TwoWay bound, so this will clear list item selection
            });
        }
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
