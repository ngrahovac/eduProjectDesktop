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
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ICollection<FacultyMemberProfileDisplayModel> FacultyMemberProfileDisplayModels { get; set; } = new HashSet<FacultyMemberProfileDisplayModel>();// ZORANE bolji naziv?
        public ICollection<StudentProfileDisplayModel> StudentProfileDisplayModels { get; set; } = new HashSet<StudentProfileDisplayModel>();
        public ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();

        public static ProjectOverview FromProject(Project project, User author)
        {
            ProjectOverview model = new ProjectOverview();

            model.ProjectId = project.ProjectId;
            model.ProjectStatus = project.ProjectStatus.ToString();
            model.Title = project.Title;
            if (author != null)
                model.AuthorFullName = $"{author.FirstName} {author.LastName}";
            model.StudyField = project.StudyField;
            model.Description = project.Description;
            model.StartDate = project.StartDate;
            model.EndDate = project.EndDate;

            foreach (var tag in project.Tags)
                model.Tags.Add(tag);

            foreach (var profile in project.CollaboratorProfiles)
            {
                if (profile is StudentProfile)
                {
                    model.StudentProfileDisplayModels.Add(StudentProfileDisplayModel.FromStudentProfile((StudentProfile)profile));
                }
                else if (profile is FacultyMemberProfile)
                {
                    model.FacultyMemberProfileDisplayModels.Add(FacultyMemberProfileDisplayModel.FromFacultyMemberProfile((FacultyMemberProfile)profile));
                }
            }

            return model;
        }
    }
}
