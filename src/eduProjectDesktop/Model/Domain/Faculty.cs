using System.Collections.Generic;

namespace eduProjectDesktop.Model.Domain
{
    public class Faculty : IValueObject
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public Dictionary<int, StudyProgram> StudyPrograms { get; set; } = new Dictionary<int, StudyProgram>();

        public void AddStudyProgram(int id, StudyProgram program)
        {
            StudyPrograms.Add(id, program);
        }


    }
}
