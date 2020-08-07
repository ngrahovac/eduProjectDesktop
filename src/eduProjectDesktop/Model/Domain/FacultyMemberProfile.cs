namespace eduProjectDesktop.Model.Domain
{
    public class FacultyMemberProfile : CollaboratorProfile
    {
        public Faculty Faculty { get; set; }
        public StudyField StudyField { get; set; }
    }
}
