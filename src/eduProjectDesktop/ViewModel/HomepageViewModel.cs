using eduProjectDesktop.Model.Display;
using eduProjectDesktop.Model.Domain;
using eduProjectDesktop.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace eduProjectDesktop.ViewModel
{
    public class HomepageViewModel
    {
        public string SectionName { get; set; } = "Sekcija";

        public ObservableCollection<ProjectSnippet> ProjectSnippets { get; set; } = new ObservableCollection<ProjectSnippet>();

        public HomepageViewModel()
        {

        }

        // kad se ucita forma (event), onda load projects
        public async void LoadProjects()
        {
            Project project = await ((App)App.Current).projects.GetAsync(1);
            User author = await ((App)App.Current).users.GetAsync(project.AuthorId);

            ProjectSnippet snippet = ProjectSnippet.FromProject(project, author);

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                ProjectSnippets.Add(snippet);
            });

            // kontaktira repositoryje da dobije listu projekata, sve ih baci i napravi snippete i baci gore na ProjectSnippets


        }
    }
}
