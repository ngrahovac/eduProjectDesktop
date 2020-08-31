using eduProjectDesktop.Model.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace eduProjectDesktop.Model.Display
{
    public abstract class ProjectOverview : INotifyPropertyChanged
    {
        public int ProjectId { get; set; }
        public string ProjectStatus { get; set; }
        public string Title { get; set; }
        public string AuthorFullName { get; set; }
        public StudyField StudyField { get; set; }

        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public ObservableCollection<Tag> Tags { get; set; } = new ObservableCollection<Tag>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void MapBasicInformation(Project project, User author)
        {
            ProjectId = project.ProjectId;
            ProjectStatus = project.ProjectStatus.ToString();
            Title = project.Title;
            if (author != null)
                AuthorFullName = $"{author.FirstName} {author.LastName}";
            StudyField = project.StudyField;
            Description = project.Description;
            StartDate = project.StartDate.ToString(); ;
            EndDate = project.EndDate.ToString();

            foreach (var tag in project.Tags)
                Tags.Add(tag);
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
