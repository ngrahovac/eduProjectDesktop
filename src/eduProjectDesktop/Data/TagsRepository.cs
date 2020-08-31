using eduProjectDesktop.Model.Domain;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eduProjectDesktop.Data
{
    public class TagsRepository
    {
        private Dictionary<int, Tag> tags;

        public TagsRepository()
        {

            tags = new Dictionary<int, Tag>()
            {
                {1, new Tag(){Name = "jedan", Description = ""} },
                {2, new Tag(){Name = "dva", Description = ""} },
                {3, new Tag(){Name = "tri", Description = ""} }
            };
        }

        public IEnumerable<Tag> GetAll()
        {
            return tags.Values.ToList();
        }
    }
}
