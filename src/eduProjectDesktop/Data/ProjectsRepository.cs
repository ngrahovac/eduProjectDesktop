using eduProjectDesktop.Model.Domain;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Composition;

namespace eduProjectDesktop.Data
{
    public class ProjectsRepository
    {
        public async Task<Project> GetAsync(int id)
        {
            Project project = new Project();

            using (var connection = new MySqlConnection(Config.dbConnectionString))
            {
                await connection.OpenAsync();

                MySqlCommand command = new MySqlCommand
                {
                    Connection = connection
                };

                await ReadBasicProjectInfo(command, id, project);
                await ReadCollaboratorProfilesInfo(command, id, project);
                await ReadTagsInfo(command, id, project);
                await ReadCollaboratorIds(command, id, project);

                await connection.CloseAsync();
            }

            return project;
        }

        public async Task<Project> GetByCollaboratorProfileAsync(int collaboratorProfileId)
        {
            Project project = new Project();

            using (var connection = new MySqlConnection(Config.dbConnectionString))
            {
                await connection.OpenAsync();

                MySqlCommand command = new MySqlCommand
                {
                    Connection = connection
                };

                int id = await GetProjectIdByCollaboratorProfile(command, collaboratorProfileId);

                await ReadBasicProjectInfo(command, id, project);
                await ReadCollaboratorProfilesInfo(command, id, project);
                await ReadTagsInfo(command, id, project);
                await ReadCollaboratorIds(command, id, project);

                await connection.CloseAsync();
            }

            return project;
        }

        private async Task<int> GetProjectIdByCollaboratorProfile(MySqlCommand command, int id)
        {
            int projectId = -1;

            string commandText = @"SELECT project_id FROM collaborator_profile WHERE collaborator_profile_id = @id";
            command.CommandText = commandText;

            command.Parameters.Clear();

            command.Parameters.Add(new MySqlParameter
            {
                DbType = DbType.Int32,
                ParameterName = "@id",
                Value = id
            });

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (reader.HasRows)
                {
                    await reader.ReadAsync();
                    projectId = reader.GetInt32(0);
                }
            }

            return projectId;
        }

        public async Task<IEnumerable<Project>> GetAllAsync()
        {
            List<Project> projects = new List<Project>();
            List<int> projectIds = new List<int>();


            using (var connection = new MySqlConnection(Config.dbConnectionString))
            {
                await connection.OpenAsync();

                // dohvatamo sve projekte, idemo id po id i dohvatamo sve detalje za njega

                MySqlCommand command = new MySqlCommand
                {
                    Connection = connection
                };

                command.CommandText = @"SELECT project_id FROM project";

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            projectIds.Add(reader.GetInt32(0));
                        }
                    }
                }

                await connection.CloseAsync();
            }


            using (var connection = new MySqlConnection(Config.dbConnectionString))
            {
                await connection.OpenAsync();

                MySqlCommand command = new MySqlCommand
                {
                    Connection = connection
                };

                foreach (int id in projectIds)
                {
                    Project project = new Project();

                    await ReadBasicProjectInfo(command, id, project);
                    await ReadCollaboratorProfilesInfo(command, id, project);
                    await ReadTagsInfo(command, id, project);
                    await ReadCollaboratorIds(command, id, project);

                    projects.Add(project);
                }

                await connection.CloseAsync();
            }



            return projects;
        }

        private async Task ReadBasicProjectInfo(MySqlCommand command, int id, Project project)
        {
            string commandText = @"SELECT project_id, title,  start_date, end_date, project.description, project.study_field_id, project_status_id, user_id
                                   FROM project
                                   INNER JOIN study_field ON project.study_field_id = study_field.study_field_id
                                   WHERE project.project_id = @id";

            command.CommandText = commandText;

            command.Parameters.Clear();

            command.Parameters.Add(new MySqlParameter
            {
                DbType = DbType.Int32,
                ParameterName = "@id",
                Value = id
            });

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        project.ProjectId = reader.GetInt32(0);
                        project.Title = reader.GetString(1);

                        project.StartDate = !reader.IsDBNull(2) ? (DateTime?)reader.GetDateTime(2) : null;
                        project.EndDate = !reader.IsDBNull(3) ? (DateTime?)reader.GetDateTime(3) : null;

                        project.Description = reader.GetString(4);
                        project.ProjectStatus = (ProjectStatus)Enum.ToObject(typeof(ProjectStatus), reader.GetInt32(6));
                        project.AuthorId = reader.GetInt32(7);
                        project.StudyField = ((App)App.Current).faculties.GetStudyFieldById(reader.GetInt32(5));
                    }
                }
            }
        }

        private async Task ReadCollaboratorProfilesInfo(MySqlCommand command, int id, Project project)
        {
            string commandText = @"SELECT collaborator_profile_id, collaborator_profile.description, user_account_type_id, 
	                               student_profile.cycle, study_year,
	                               faculty.faculty_id, study_program.study_program_id, study_program_specialization.study_program_specialization_id,       
	                               study_field.study_field_id
                                   FROM collaborator_profile
                                   LEFT OUTER JOIN student_profile USING(collaborator_profile_id)
                                   LEFT OUTER JOIN faculty_member_profile USING(collaborator_profile_id)
                                   LEFT OUTER JOIN faculty ON student_profile.faculty_id = faculty.faculty_id OR faculty_member_profile.faculty_id = faculty.faculty_id
                                   LEFT OUTER JOIN study_program ON student_profile.study_program_id = study_program.study_program_id
                                   LEFT OUTER JOIN study_program_specialization ON student_profile.study_program_specialization_id = study_program_specialization.study_program_specialization_id
                                   LEFT OUTER JOIN study_field ON faculty_member_profile.study_field_id = study_field.study_field_id
                                   WHERE project_id = @id";

            command.CommandText = commandText;
            command.Parameters.Clear();
            command.Parameters.Add(new MySqlParameter
            {
                DbType = DbType.Int32,
                ParameterName = "@id",
                Value = id
            });

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        CollaboratorProfileType profileType = (CollaboratorProfileType)Enum.ToObject(typeof(CollaboratorProfileType), reader.GetInt32(2));

                        if (profileType is CollaboratorProfileType.Student)
                        {
                            StudentProfile profile = new StudentProfile
                            {
                                CollaboratorProfileId = reader.GetInt32(0),
                                Description = !reader.IsDBNull(1) ? reader.GetString(1) : null,
                                StudyCycle = !reader.IsDBNull(3) ? (int?)reader.GetInt32(3) : null,
                                StudyYear = !reader.IsDBNull(4) ? (int?)reader.GetInt32(4) : null,

                                Added = false
                            };

                            int? facultyId = !reader.IsDBNull(5) ? (int?)reader.GetInt32(5) : null;
                            if (facultyId != null)
                            {
                                profile.Faculty = ((App)App.Current).faculties.GetFacultyById((int)facultyId);

                                int? programId = !reader.IsDBNull(6) ? (int?)reader.GetInt32(6) : null;
                                if (programId != null)
                                {
                                    profile.StudyProgram = profile.Faculty.StudyPrograms[(int)programId];

                                    int? specializationId = !reader.IsDBNull(7) ? (int?)reader.GetInt32(7) : null;
                                    if (specializationId != null)
                                    {
                                        profile.StudyProgramSpecialization = profile.StudyProgram.StudyProgramSpecializations[(int)specializationId];
                                    }
                                }
                            }

                            project.CollaboratorProfiles.Add(profile);
                        }
                        else if (profileType is CollaboratorProfileType.FacultyMember)
                        {
                            FacultyMemberProfile profile = new FacultyMemberProfile
                            {
                                CollaboratorProfileId = reader.GetInt32(0),
                                Description = !reader.IsDBNull(1) ? reader.GetString(1) : null,

                                Added = false
                            };

                            int? facultyId = !reader.IsDBNull(5) ? (int?)reader.GetInt32(5) : null;
                            if (facultyId != null)
                            {
                                profile.Faculty = ((App)App.Current).faculties.GetFacultyById((int)facultyId);
                            }

                            int? studyFieldId = !reader.IsDBNull(8) ? (int?)reader.GetInt32(8) : null;
                            if (studyFieldId != null)
                            {
                                profile.StudyField = ((App)App.Current).faculties.GetStudyFieldById((int)studyFieldId);
                            }

                            project.CollaboratorProfiles.Add(profile);
                        }
                    }
                }
            }
        }

        private async Task ReadTagsInfo(MySqlCommand command, int id, Project project)
        {
            string commandText = @"SELECT tag_id
                                   FROM tag
                                   INNER JOIN project_tag USING(tag_id)
                                   WHERE project_tag.project_id = @id";

            command.CommandText = commandText;
            command.Parameters.Clear();
            command.Parameters.Add(new MySqlParameter
            {
                DbType = DbType.Int32,
                ParameterName = "@id",
                Value = id
            });

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        project.Tags.Add(((App)App.Current).tags.GetTagById(reader.GetInt32(0)));
                    }
                }
            }
        }

        private async Task ReadCollaboratorIds(MySqlCommand command, int id, Project project)
        {
            string commandText = @"SELECT user_id
                                   FROM project_collaborator
                                   WHERE project_collaborator.project_id = @id";

            command.CommandText = commandText;
            command.Parameters.Clear();
            command.Parameters.Add(new MySqlParameter
            {
                DbType = DbType.Int32,
                ParameterName = "@id",
                Value = id
            });

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        project.CollaboratorIds.Add(reader.GetInt32(0));
                    }
                }
            }
        }

        public async Task CreateAsync(Project project)
        {
            using (var connection = new MySqlConnection(Config.dbConnectionString))
            {
                MySqlCommand command = new MySqlCommand
                {
                    Connection = connection
                };

                await connection.OpenAsync();

                await AddBasicProjectInfo(command, project);
                await AddCollaboratorProfiles(command, project);
                await AddTags(command, project);

                await connection.CloseAsync();
            }
        }

        private async Task AddBasicProjectInfo(MySqlCommand command, Project project)
        {
            string commandText = @"INSERT INTO project
                                   (user_id, title, project_status_id, description,
                                   study_field_id, start_date, end_date)
                                   VALUES
                                   (@userId, @title, @statusId, @description,
                                   @studyFieldId, @startDate, @endDate)";

            command.CommandText = commandText;

            command.Parameters.Clear();

            command.Parameters.Add(new MySqlParameter
            {
                DbType = DbType.Int32,
                ParameterName = "@userId",
                Value = project.AuthorId
            });

            command.Parameters.Add(new MySqlParameter
            {
                DbType = DbType.String,
                ParameterName = "@title",
                Value = project.Title
            });

            command.Parameters.Add(new MySqlParameter
            {
                DbType = DbType.Int32,
                ParameterName = "@statusId",
                Value = (int)project.ProjectStatus
            });

            command.Parameters.Add(new MySqlParameter
            {
                DbType = DbType.String,
                ParameterName = "@description",
                Value = project.Description
            });

            command.Parameters.Add(new MySqlParameter
            {
                DbType = DbType.String,
                ParameterName = "@studyFieldId",
                Value = ((App)App.Current).faculties.GetStudyFieldId(project.StudyField)
            });

            command.Parameters.Add(new MySqlParameter
            {
                DbType = DbType.String,
                ParameterName = "@startDate",
                Value = project.StartDate
            });

            command.Parameters.Add(new MySqlParameter
            {
                DbType = DbType.String,
                ParameterName = "@endDate",
                Value = project.EndDate
            });

            await command.ExecuteNonQueryAsync();

            project.ProjectId = (int)command.LastInsertedId;
        }

        private async Task AddCollaboratorProfiles(MySqlCommand command, Project project)
        {
            var studentProfiles = project.CollaboratorProfiles.Where(p => p is StudentProfile);
            var facultyMemberProfiles = project.CollaboratorProfiles.Where(p => p is FacultyMemberProfile);

            string commandText = @"INSERT INTO collaborator_profile
                                   (description, project_id, user_account_type_id)
                                   VALUES
                                   (@description, @projectId, @accountTypeId)";

            foreach (var profile in project.CollaboratorProfiles)
            {
                command.CommandText = commandText;

                command.Parameters.Clear();

                command.Parameters.Add(new MySqlParameter
                {
                    DbType = DbType.String,
                    ParameterName = "@description",
                    Value = profile.Description
                });

                command.Parameters.Add(new MySqlParameter
                {
                    DbType = DbType.Int32,
                    ParameterName = "@projectId",
                    Value = project.ProjectId
                });

                command.Parameters.Add(new MySqlParameter
                {
                    DbType = DbType.Int32,
                    ParameterName = "@accountTypeId",
                    Value = profile is StudentProfile ? 1 : 2
                });

                await command.ExecuteNonQueryAsync();

                int collaboratorProfileId = (int)command.LastInsertedId; // TODO: make long

                if (profile is StudentProfile sp)
                {
                    commandText = @"INSERT INTO student_profile
                                    (collaborator_profile_id, cycle, study_year,
                                    faculty_id, study_program_id, study_program_specialization_id)
                                    VALUES
                                    (@collaboratorProfileId, @cycle, @studyYear,
                                    @facultyId, @studyProgramId, @studyProgramSpecializationId)";

                    command.CommandText = commandText;

                    command.Parameters.Clear();

                    command.Parameters.Add(new MySqlParameter
                    {
                        DbType = DbType.Int32,
                        ParameterName = "@collaboratorProfileId",
                        Value = collaboratorProfileId
                    });

                    command.Parameters.Add(new MySqlParameter
                    {
                        DbType = DbType.Int32,
                        ParameterName = "@cycle",
                        Value = sp.StudyCycle
                    });

                    command.Parameters.Add(new MySqlParameter
                    {
                        DbType = DbType.Int32,
                        ParameterName = "@studyYear",
                        Value = sp.StudyYear
                    });

                    command.Parameters.Add(new MySqlParameter
                    {
                        DbType = DbType.Int32,
                        ParameterName = "@facultyId",
                        Value = sp.Faculty != null ? (int?)((App)App.Current).faculties.GetFacultyId(sp.Faculty) : null
                    });

                    command.Parameters.Add(new MySqlParameter
                    {
                        DbType = DbType.Int32,
                        ParameterName = "@studyProgramId",
                        Value = sp.StudyProgram != null ? (int?)((App)App.Current).faculties.GetStudyProgramId(sp.Faculty, sp.StudyProgram) : null
                    });

                    command.Parameters.Add(new MySqlParameter
                    {
                        DbType = DbType.Int32,
                        ParameterName = "@studyProgramSpecializationId",
                        Value = sp.StudyProgramSpecialization != null ? (int?)((App)App.Current).faculties.GetStudyProgramSpecializationId(sp.StudyProgram, sp.StudyProgramSpecialization) : null
                    });

                    await command.ExecuteNonQueryAsync();
                }

                else if (profile is FacultyMemberProfile fp)
                {
                    commandText = @"INSERT INTO faculty_member_profile
                                    (collaborator_profile_id, faculty_id, study_field_id)
                                    VALUES
                                    (@collaboratorProfileId, @facultyId, @studyFieldId)";

                    command.CommandText = commandText;

                    command.Parameters.Clear();

                    command.Parameters.Add(new MySqlParameter
                    {
                        DbType = DbType.Int32,
                        ParameterName = "@collaboratorProfileId",
                        Value = collaboratorProfileId
                    });

                    command.Parameters.Add(new MySqlParameter
                    {
                        DbType = DbType.Int32,
                        ParameterName = "@facultyId",
                        Value = fp.Faculty != null ? (int?)((App)App.Current).faculties.GetFacultyId(fp.Faculty) : null
                    });

                    command.Parameters.Add(new MySqlParameter
                    {
                        DbType = DbType.Int32,
                        ParameterName = "@studyFieldId",
                        Value = fp.StudyField != null ? (int?)((App)App.Current).faculties.GetStudyFieldId(fp.StudyField) : null
                    });

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        private async Task AddTags(MySqlCommand command, Project project)
        {
            string commandText = @"INSERT INTO project_tag
                                   (project_id, tag_id)
                                   VALUES
                                   (@projectId, @tagId)";

            command.CommandText = commandText;

            foreach (var tag in project.Tags)
            {
                command.Parameters.Clear();

                command.Parameters.Add(new MySqlParameter
                {
                    DbType = DbType.Int32,
                    ParameterName = "@projectId",
                    Value = project.ProjectId
                });

                command.Parameters.Add(new MySqlParameter
                {
                    DbType = DbType.Int32,
                    ParameterName = "@tagId",
                    Value = ((App)App.Current).tags.GetTagId(tag)
                });
            }

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Project project)
        {
            using (var connection = new MySqlConnection(Config.dbConnectionString))
            {
                await connection.OpenAsync();
                MySqlCommand command = new MySqlCommand
                {
                    Connection = connection
                };

                await UpdateBasicProjectInfo(command, project);
                await UpdateProjectTags(command, project);
                await UpdateCollaboratorIds(command, project);

                await connection.CloseAsync();
            }
        }

        private async Task UpdateBasicProjectInfo(MySqlCommand command, Project project)
        {
            string commandText = @"UPDATE project
                                   SET
                                   title = @title,
                                   project_status_id = @statusId,
                                   description = @description,
                                   start_date = @startDate,
                                   end_date = @endDate                                   
                                   WHERE project_id = @projectId";

            command.CommandText = commandText;

            command.Parameters.Clear();

            command.Parameters.Add(new MySqlParameter
            {
                DbType = DbType.String,
                ParameterName = "@title",
                Value = project.Title
            });

            command.Parameters.Add(new MySqlParameter
            {
                DbType = DbType.Int32,
                ParameterName = "@statusId",
                Value = (int)project.ProjectStatus
            });

            command.Parameters.Add(new MySqlParameter
            {
                DbType = DbType.String,
                ParameterName = "@description",
                Value = project.Description
            });

            command.Parameters.Add(new MySqlParameter
            {
                DbType = DbType.Int32,
                ParameterName = "@studyFieldId",
                Value = ((App)App.Current).faculties.GetStudyFieldId(project.StudyField)
            });


            command.Parameters.Add(new MySqlParameter
            {
                DbType = DbType.DateTime,
                ParameterName = "@startDate",
                Value = project.StartDate ?? null
            });

            command.Parameters.Add(new MySqlParameter
            {
                DbType = DbType.DateTime,
                ParameterName = "@endDate",
                Value = project.EndDate ?? null
            });

            command.Parameters.Add(new MySqlParameter
            {
                DbType = DbType.Int32,
                ParameterName = "@projectId",
                Value = project.ProjectId
            });

            await command.ExecuteNonQueryAsync();
        }

        private async Task UpdateProjectTags(MySqlCommand command, Project project)
        {
            string commandText = @"INSERT INTO project_tag
                                   (project_id, tag_id)
                                   VALUES
                                   (@projectId, @tagId)
                                   ON DUPLICATE KEY UPDATE project_id = @projectId";

            foreach (var tag in project.Tags)
            {
                command.CommandText = commandText;

                command.Parameters.Clear();

                command.Parameters.Add(new MySqlParameter
                {
                    DbType = DbType.Int32,
                    ParameterName = "@projectId",
                    Value = project.ProjectId
                });

                command.Parameters.Add(new MySqlParameter
                {
                    DbType = DbType.Int32,
                    ParameterName = "@tagId",
                    Value = ((App)App.Current).tags.GetTagId(tag)
                });

                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task UpdateCollaboratorIds(MySqlCommand command, Project project)
        {
            string commandText = @"INSERT INTO project_collaborator
                                   (project_id, user_id)
                                   VALUES
                                   (@projectId, @userId)
                                   ON DUPLICATE KEY UPDATE project_id = @projectId";

            command.CommandText = commandText;

            foreach (int id in project.CollaboratorIds)
            {
                command.Parameters.Clear();

                command.Parameters.Add(new MySqlParameter
                {
                    DbType = DbType.Int32,
                    ParameterName = "@projectId",
                    Value = project.ProjectId
                });

                command.Parameters.Add(new MySqlParameter
                {
                    DbType = DbType.Int32,
                    ParameterName = "@userId",
                    Value = id
                });

                await command.ExecuteNonQueryAsync();
            }
        }

    }
}
