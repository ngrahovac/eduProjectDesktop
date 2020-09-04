using eduProjectDesktop.Model.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eduProjectDesktop.Model.Input
{
    public class StudentProfileInputModel : CollaboratorProfileInputModel
    {
        public string FacultyName { get; set; }
        public string StudyProgramName { get; set; }
        public string StudyProgramSpecializationName { get; set; }
        public int? StudyCycle { get; set; }
        public int? StudyYear { get; set; }

        public static StudentProfile ToStudentProfile(StudentProfileInputModel model)
        {
            StudentProfile profile = new StudentProfile();

            profile.Description = model.Description;

            if (model.FacultyName != null)
            {
                profile.Faculty = ((App)App.Current).faculties.GetAll().First(f => f.Name == model.FacultyName);

                if (model.StudyProgramName != null)
                {
                    profile.StudyProgram = profile.Faculty.StudyPrograms.Values.First(sp => sp.Name == model.StudyProgramName);

                    if (model.StudyProgramSpecializationName != null)
                    {
                        profile.StudyProgramSpecialization = profile.StudyProgram.StudyProgramSpecializations.Values.First(sps => sps.Name == model.StudyProgramSpecializationName);
                    }
                }
            }

            profile.StudyCycle = model.StudyCycle ?? null;
            profile.StudyYear = model.StudyYear ?? null;

            return profile;
        }
    }
}
