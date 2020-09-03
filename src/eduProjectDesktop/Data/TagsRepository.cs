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
        private Dictionary<int, Tag> tags = new Dictionary<int, Tag>();

        public static TagsRepository instance;


        public async static Task CreateAsync()
        {
            var repository = new TagsRepository();

            string commandText = @"SELECT tag_id, name, description FROM tag";

            using (MySqlConnection connection = new MySqlConnection(Config.dbConnectionString))
            {
                MySqlCommand command = new MySqlCommand
                {
                    CommandText = commandText
                };

                command.Connection = connection;

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            repository.tags.Add(reader.GetInt32(0), new Tag
                            {
                                Name = reader.GetString(1),
                                Description = !reader.IsDBNull(2) ? reader.GetString(2) : null
                            });
                        }
                    }
                }

                await connection.CloseAsync();
            }

            instance = repository;
        }

        public IEnumerable<Tag> GetAll()
        {
            return tags.Values.ToList();
        }

        public Tag GetTagById(int id)
        {
            if (tags.ContainsKey(id))
                return tags[id];
            else
                throw new Exception();
        }

        public int GetTagId(Tag tag)
        {
            foreach (var item in tags)
            {
                if (item.Value.Name == tag.Name && item.Value.Description == tag.Description)
                    return item.Key;
            }

            return -1;
        }
    }
}
