using eduProjectDesktop.Model.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eduProjectDesktop.Model.Display
{
    public class StudentProfileOverview : CollaboratorProfileOverview
    {
        public string FacultyName { get; set; }
        public string StudyProgramName { get; set; }
        public string StudyProgramSpecializationName { get; set; }
        public int? StudyCycle { get; set; }
        public int? StudyYear { get; set; }

        public StudentProfileOverview()
        {

        }
        public StudentProfileOverview(string facultyName, string studyProgramName, string studyProgramSpecializationName,
                                          int? studyCycle, int? studyYear, string description)
        {
            Description = description;
            FacultyName = facultyName;
            StudyProgramName = studyProgramName;
            StudyProgramSpecializationName = studyProgramSpecializationName;
            StudyCycle = studyCycle;
            StudyYear = studyYear;
        }

        public static StudentProfileOverview FromStudentProfile(StudentProfile profile)
        {
            StudentProfileOverview model = new StudentProfileOverview();
            model.FacultyName = profile.Faculty.Name;
            model.StudyProgramName = profile.StudyProgram.Name;
            model.StudyProgramSpecializationName = profile.StudyProgramSpecialization.Name;
            model.StudyCycle = profile.StudyCycle;
            model.StudyYear = profile.StudyYear;

            return model;
        }
    }
}
