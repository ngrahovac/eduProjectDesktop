using eduProjectDesktop.Model.Domain;
using MySqlConnector;
using System;
using System.Collections.Generic;
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

            string insertProjectApplicationCommandText = @"INSERT INTO project_application
                                                           (applicant_comment, collaborator_profile_id, user_id)
                                                           VALUES
                                                           (@applicantComment, @collaboratorProfileId, @applicantId)";

            MySqlCommand command = new MySqlCommand();
            command.CommandText = insertProjectApplicationCommandText;
            command.Parameters.Add(new MySqlParameter { DbType = DbType.String, ParameterName = "@applicantComment", Value = applicantComment });
            command.Parameters.Add(new MySqlParameter { DbType = DbType.Int32, ParameterName = "@collaboratorProfileId", Value = collaboratorProfileId });
            command.Parameters.Add(new MySqlParameter { DbType = DbType.Int32, ParameterName = "@applicantId", Value = applicantId });


            using (MySqlConnection dbConnection = new MySqlConnection(Config.dbConnectionString))
            {
                await dbConnection.OpenAsync();

                command.Connection = dbConnection;

                await command.ExecuteNonQueryAsync();

                await dbConnection.CloseAsync();

            }
        }
    }
}
