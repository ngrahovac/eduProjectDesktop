using eduProjectDesktop.Model.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eduProjectDesktop.Model.Display
{
    public class ActiveProjectOverview : ProjectOverview
    {
        public ObservableCollection<FacultyMemberProfileOverview> FacultyMemberProfileOverviews { get; set; } = new ObservableCollection<FacultyMemberProfileOverview>();// ZORANE bolji naziv?
        public ObservableCollection<StudentProfileOverview> StudentProfileOverviews { get; set; } = new ObservableCollection<StudentProfileOverview>();

        public static ActiveProjectOverview FromProject(Project project, User user)
        {
            ActiveProjectOverview overview = new ActiveProjectOverview();
            overview.MapBasicInformation(project, user);

            return overview;
        }

        public void SetCollaboratorProfileOverviews(Project project)
        {
            foreach (var profile in project.CollaboratorProfiles)
            {
                if (profile is StudentProfile)
                {
                    StudentProfileOverviews.Add(StudentProfileOverview.FromStudentProfile((StudentProfile)profile));
                }
                else if (profile is FacultyMemberProfile)
                {
                    FacultyMemberProfileOverviews.Add(FacultyMemberProfileOverview.FromFacultyMemberProfile((FacultyMemberProfile)profile));
                }
            }

        }
    }
}
