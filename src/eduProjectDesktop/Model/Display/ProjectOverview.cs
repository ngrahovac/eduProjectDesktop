using eduProjectDesktop.Model.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eduProjectDesktop.Model.Display
{
    public abstract class ProjectOverview
    {
        public int ProjectId { get; set; }
        public string ProjectStatus { get; set; }
        public string Title { get; set; }
        public string AuthorFullName { get; set; }
        public StudyField StudyField { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();

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

    }
}
