namespace eduProjectDesktop.Model.Domain
{
    public class StudentProfile : CollaboratorProfile
    {
        public Faculty Faculty { get; set; }
        public StudyProgram StudyProgram { get; set; }
        public StudyProgramSpecialization StudyProgramSpecialization { get; set; }
        public int? StudyCycle { get; set; }
        public int? StudyYear { get; set; }
    }
}
