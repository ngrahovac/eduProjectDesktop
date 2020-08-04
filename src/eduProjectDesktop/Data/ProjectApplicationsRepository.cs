using eduProjectDesktop.Model.Domain;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eduProjectDesktop.Data
{
    public class ProjectApplicationsRepository
    {
        public async Task AddAsync(ProjectApplication application)
        {
            string applicantComment = application.ApplicantComment;
            int collaboratorProfileId = application.CollaboratorProfileId;
            int applicantId = application.ApplicantId;

            string commandText = @"INSERT INTO project_application
                                   (applicant_comment, collaborator_profile_id, user_id)
                                   VALUES
                                   (@applicantComment, @collaboratorProfileId, @applicantId)";

            MySqlCommand command = new MySqlCommand
            {
                CommandText = commandText
            };

            command.Parameters.Add(new MySqlParameter
            {
                DbType = DbType.String,
                ParameterName = "@applicantComment",
                Value = applicantComment
            });

            command.Parameters.Add(new MySqlParameter
            {
                DbType = DbType.Int32,
                ParameterName = "@collaboratorProfileId",
                Value = collaboratorProfileId
            });

            command.Parameters.Add(new MySqlParameter
            {
                DbType = DbType.Int32,
                ParameterName = "@applicantId",
                Value = applicantId
            });


            using (var connection = new MySqlConnection(Config.dbConnectionString))
            {
                await connection.OpenAsync();
                command.Connection = connection;

                await command.ExecuteNonQueryAsync();

                await connection.CloseAsync();
            }
        }

        public async Task<IEnumerable<ProjectApplication>> GetByUserAsync(int id)
        {
            string commandText = @"SELECT *
                                   FROM project_application
                                   WHERE user_id = @id";

            MySqlCommand command = new MySqlCommand
            {
                CommandText = commandText
            };

            command.Parameters.Add(new MySqlParameter
            {
                DbType = DbType.Int32,
                ParameterName = "@id",
                Value = id
            });

            List<ProjectApplication> applications = new List<ProjectApplication>();

            using (MySqlConnection connection = new MySqlConnection(Config.dbConnectionString))
            {
                await connection.OpenAsync();
                command.Connection = connection;

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            ProjectApplication application = GetProjectApplicationFromRow(reader);
                            applications.Add(application);
                        }
                    }
                }

                await connection.CloseAsync();
            }

            return applications;

        }

        public async Task<IEnumerable<ProjectApplication>> GetByCollaboratorProfileAsync(int id)
        {
            string commandText = @"SELECT *
                                   FROM project_application
                                   WHERE collaborator_profile_id = @id";

            MySqlCommand command = new MySqlCommand
            {
                CommandText = commandText
            };

            command.Parameters.Add(new MySqlParameter
            {
                DbType = DbType.Int32,
                ParameterName = "@id",
                Value = id
            });

            List<ProjectApplication> applications = new List<ProjectApplication>();

            using (MySqlConnection connection = new MySqlConnection(Config.dbConnectionString))
            {
                await connection.OpenAsync();
                command.Connection = connection;

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            ProjectApplication application = GetProjectApplicationFromRow(reader);
                            applications.Add(application);
                        }
                    }
                }

                await connection.CloseAsync();
            }

            return applications;
        }

        public async Task<IEnumerable<ProjectApplication>> GetByAuthorAsync(int id)
        {
            string commandText = @"SELECT project_application_id,
		                                  applicant_comment,
                                          author_comment,
                                          project_application_status_id,
                                          project_application.collaborator_profile_id,
                                          project_application.user_id
                                   FROM project
                                   INNER JOIN collaborator_profile USING(project_id)
                                   INNER JOIN project_application USING(collaborator_profile_id)
                                   WHERE project.user_id = @id";

            MySqlCommand command = new MySqlCommand
            {
                CommandText = commandText
            };

            command.Parameters.Add(new MySqlParameter
            {
                DbType = DbType.Int32,
                ParameterName = "@id",
                Value = id
            });

            List<ProjectApplication> applications = new List<ProjectApplication>();

            using (var connection = new MySqlConnection(Config.dbConnectionString))
            {
                await connection.OpenAsync();
                command.Connection = connection;

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            ProjectApplication application = GetProjectApplicationFromRow(reader);
                            applications.Add(application);
                        }
                    }
                }
                await connection.CloseAsync();
            }

            return applications;
        }

        private ProjectApplication GetProjectApplicationFromRow(MySqlDataReader reader)
        {
            return new ProjectApplication
            {
                ProjectApplicationId = reader.GetInt32(0),
                ApplicantComment = !reader.IsDBNull(1) ? reader.GetString(1) : null,
                AuthorComment = !reader.IsDBNull(2) ? reader.GetString(2) : null,
                ProjectApplicationStatus = (ProjectApplicationStatus)Enum.ToObject(typeof(ProjectApplicationStatus), reader.GetInt32(3)),
                CollaboratorProfileId = reader.GetInt32(4),
                ApplicantId = reader.GetInt32(4)
            };
        }

    }
}
