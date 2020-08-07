using eduProjectDesktop.Model.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eduProjectDesktop.Model.Display
{
    public class CollaboratorOverview
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public static CollaboratorOverview FromUser(User user)
        {
            return new CollaboratorOverview
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }
    }
}
