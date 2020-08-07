using eduProjectDesktop.Model.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eduProjectDesktop.Model.Display
{
    public class ClosedProjectOverview : ProjectOverview
    {
        public ICollection<CollaboratorOverview> CollaboratorOverviews { get; set; }

        public static ClosedProjectOverview FromProject(Project project, User user, ICollection<User> collaborators)
        {
            ClosedProjectOverview overview = new ClosedProjectOverview();
            overview.MapBasicInformation(project, user);

            overview.CollaboratorOverviews = new List<CollaboratorOverview>();

            foreach (var collaborator in collaborators)
                overview.CollaboratorOverviews.Add(CollaboratorOverview.FromUser(collaborator));

            return overview;
        }
    }
}
