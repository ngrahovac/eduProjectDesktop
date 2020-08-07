using eduProjectDesktop.Model.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eduProjectDesktop.Model.Display
{
    public class ActiveProjectOverview : ProjectOverview
    {
        public ICollection<FacultyMemberProfileOverview> FacultyMemberProfileOverviews { get; set; } = new HashSet<FacultyMemberProfileOverview>();// ZORANE bolji naziv?
        public ICollection<StudentProfileOverview> StudentProfileOverviews { get; set; } = new HashSet<StudentProfileOverview>();

        public static ActiveProjectOverview FromProject(Project project, User user)
        {
            ActiveProjectOverview overview = new ActiveProjectOverview();
            overview.MapBasicInformation(project, user);

            foreach (var profile in project.CollaboratorProfiles)
            {
                if (profile is StudentProfile)
                {
                    overview.StudentProfileOverviews.Add(StudentProfileOverview.FromStudentProfile((StudentProfile)profile));
                }
                else if (profile is FacultyMemberProfile)
                {
                    overview.FacultyMemberProfileOverviews.Add(FacultyMemberProfileOverview.FromFacultyMemberProfile((FacultyMemberProfile)profile));
                }
            }

            return overview;
        }
    }
}
