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

namespace eduProjectDesktop.ViewModel
{
    public class HomepageViewModel : INotifyPropertyChanged
    {
        public ProjectPageViewModel ProjectPageViewModel { get; set; }

        /*
        private readonly static Dictionary<Tuple<bool, ProjectStatus>, Type> overviews
            = new Dictionary<Tuple<bool, ProjectStatus>, Type>(new List<KeyValuePair<Tuple<bool, ProjectStatus>, Type>>
            {
                new KeyValuePair<Tuple<bool, ProjectStatus>, Type>(new Tuple<bool, ProjectStatus>(true, ProjectStatus.Active), typeof(ProjectOverview)),
                new KeyValuePair<Tuple<bool, ProjectStatus>, Type>(new Tuple<bool, ProjectStatus>(true, ProjectStatus.Active), typeof(ProjectOverview))
            });
            */

        public event PropertyChangedEventHandler PropertyChanged;
        public string SectionName { get; set; } = "Sekcija";
        public ObservableCollection<ProjectSnippet> ProjectSnippets { get; set; } = new ObservableCollection<ProjectSnippet>();

        private int selectedSnippetIndex = -1;
        public int SelectedSnippetIndex { get { return selectedSnippetIndex; } set { selectedSnippetIndex = value; OnPropertyChanged(); } }
        public ProjectSnippet SelectedSnippet { get; set; }

        private Visibility isHomePageVisible = Visibility.Visible;
        public Visibility IsHomepageVisible { get { return isHomePageVisible; } set { isHomePageVisible = value; OnPropertyChanged(); } }

        private Visibility isProjectPageVisible = Visibility.Collapsed;
        public Visibility IsProjectPageVisible
        {
            get { return isProjectPageVisible; }
            set { isProjectPageVisible = value; OnPropertyChanged(); }
        }
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
