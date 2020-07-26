using eduProjectDesktop.Model.Domain;

namespace eduProjectDesktop.Model.Display
{
    public class ProjectSnippet
    {
        public int ProjectId { get; set; }
        public string ProjectStatus { get; set; }
        public string Title { get; set; }
        public string AuthorFullName { get; set; }

        // public StudyField StudyField { get; set; }
        public string Description { get; set; }
        //public DateTime? StartDate { get; set; }
        //public DateTime? EndDate { get; set; }
        //public ICollection<FacultyMemberProfileDisplayModel> FacultyMemberProfileDisplayModels { get; set; } = new HashSet<FacultyMemberProfileDisplayModel>();// ZORANE bolji naziv?
        //public ICollection<StudentProfileDisplayModel> StudentProfileDisplayModels { get; set; } = new HashSet<StudentProfileDisplayModel>();
        //public ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();


        public static ProjectSnippet FromProject(Project project, User author)
        {
            return new ProjectSnippet
            {
                ProjectId = project.ProjectId,
                ProjectStatus = project.ProjectStatus.ToString(),
                Title = project.Title,
                Description = project.Description,
                AuthorFullName = $"{author.FirstName} {author.LastName}"
            };
        }


    }
}
