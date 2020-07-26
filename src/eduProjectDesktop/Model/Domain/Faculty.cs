using System.Collections.Generic;

namespace eduProjectDesktop.Model.Domain
{
    public class Faculty : IValueObject
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public ICollection<StudyProgram> StudyPrograms { get; set; }

        public Faculty()
        {
            StudyPrograms = new HashSet<StudyProgram>();
        }
        public void AddStudyProgram(StudyProgram program)
        {
            StudyPrograms.Add(program);
        }
        // ograniciti programe na 5 ?
    }
}
