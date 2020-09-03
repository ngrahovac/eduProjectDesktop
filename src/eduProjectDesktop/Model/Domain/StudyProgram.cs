using System.Collections.Generic;

namespace eduProjectDesktop.Model.Domain
{
    public class StudyProgram : IValueObject
    {
        public string Name { get; set; }
        public byte Cycle { get; set; }
        public byte DurationYears { get; set; }
        public Dictionary<int, StudyProgramSpecialization> StudyProgramSpecializations { get; set; } = new Dictionary<int, StudyProgramSpecialization>();

        public void AddStudyProgramSpecialization(int id, StudyProgramSpecialization specialization)
        {
            StudyProgramSpecializations.Add(id, specialization);
        }
    }
}
