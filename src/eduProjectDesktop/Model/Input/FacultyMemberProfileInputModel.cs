using eduProjectDesktop.Model.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eduProjectDesktop.Model.Input
{
    public class FacultyMemberProfileInputModel : CollaboratorProfileInputModel
    {
        public string FacultyName { get; set; }
        public string StudyFieldName { get; set; }

        public static FacultyMemberProfile ToFacultyMemberProfile(FacultyMemberProfileInputModel model)
        {
            FacultyMemberProfile profile = new FacultyMemberProfile();

            if (model.FacultyName != null)
                profile.Faculty = ((App)App.Current).faculties.GetAll().First(f => f.Name == model.FacultyName);

            if (model.StudyFieldName != null)
                profile.StudyField = ((App)App.Current).faculties.GetAllStudyFields().First(sf => sf.Name == model.StudyFieldName);

            return profile;
        }

    }
}
