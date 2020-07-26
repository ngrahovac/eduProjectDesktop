namespace eduProjectDesktop.Model.Domain
{
    public class StudyField : IValueObject
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public StudyField()
        {

        }
        public StudyField(string name, string description)
        {
            Name = name;
            Description = description;
        }


    }
}
