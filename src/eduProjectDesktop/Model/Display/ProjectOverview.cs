using eduProjectDesktop.Model.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eduProjectDesktop.Model.Display
{
    public class ProjectOverview
    {
        public int ProjectId { get; set; }
        public string ProjectStatus { get; set; }
        public string Title { get; set; }
        public string AuthorFullName { get; set; }
        public StudyField StudyField { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public ICollection<FacultyMemberProfileOverview> FacultyMemberProfileDisplayModels { get; set; } = new HashSet<FacultyMemberProfileOverview>();// ZORANE bolji naziv?
        public ICollection<StudentProfileOverview> StudentProfileDisplayModels { get; set; } = new HashSet<StudentProfileOverview>();
        public ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();

        public static ProjectOverview FromProject(Project project, User author, bool isAdmin = false)
        {
            ProjectOverview model = new ProjectOverview();

            model.ProjectId = project.ProjectId;
            model.ProjectStatus = project.ProjectStatus.ToString();
            model.Title = project.Title;
            if (author != null)
                model.AuthorFullName = $"{author.FirstName} {author.LastName}";
            model.StudyField = project.StudyField;
            model.Description = project.Description;
            model.StartDate = project.StartDate.ToString(); ;
            model.EndDate = project.EndDate.ToString();

            foreach (var tag in project.Tags)
                model.Tags.Add(tag);

            foreach (var profile in project.CollaboratorProfiles)
            {
                if (profile is StudentProfile)
                {
                    model.StudentProfileDisplayModels.Add(StudentProfileOverview.FromStudentProfile((StudentProfile)profile));
                }
                else if (profile is FacultyMemberProfile)
                {
                    model.FacultyMemberProfileDisplayModels.Add(FacultyMemberProfileOverview.FromFacultyMemberProfile((FacultyMemberProfile)profile));
                }
            }

            return model;
        }
    }
}
