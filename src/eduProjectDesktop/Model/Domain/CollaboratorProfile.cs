namespace eduProjectDesktop.Model.Domain
{
    public class CollaboratorProfile : IEntity
    {
        public int CollaboratorProfileId { get; set; } // TODO: make readonly
        public string Description { get; set; }

        public bool Added { get; set; } = false;
    }
}
