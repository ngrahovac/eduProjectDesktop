using eduProjectDesktop.Model.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace eduProjectDesktop.Model.Display
{
    public class FacultyMemberProfileOverview : CollaboratorProfileOverview
    {
        public string FacultyName { get; set; }
        public string StudyFieldName { get; set; }

        public FacultyMemberProfileOverview()
        {

        }
        public static FacultyMemberProfileOverview FromFacultyMemberProfile(FacultyMemberProfile profile)
        {
            FacultyMemberProfileOverview model = new FacultyMemberProfileOverview();

            model.FacultyName = profile.Faculty.Name;
            model.StudyFieldName = profile.StudyField.Name;

            return model;
        }
    }
}
