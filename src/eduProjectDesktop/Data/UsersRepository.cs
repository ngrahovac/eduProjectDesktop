using eduProjectDesktop.Model.Domain;
using MySqlConnector;
using System;
using System.Data;
using System.Threading.Tasks;

namespace eduProjectDesktop.Data
{
    public class UsersRepository
    {
        public async Task<User> GetAsync(int id)
        {
            string commandText = @"SELECT user_id, user_account_type_id, first_name, last_name, phone_number, phone_format,
	                                      student.study_year,	                                            
                                          study_program.faculty_id,
                                          student.study_program_id, student.study_program_specialization_id,
                                          faculty_member.faculty_id,
                                          academic_rank_id,
                                          study_field_id
       
                                    FROM user
                                    INNER JOIN account using(user_id)
                                    LEFT OUTER JOIN student using(user_id)
                                    LEFT OUTER JOIN faculty_member using (user_id)
                                    LEFT OUTER JOIN study_program using(study_program_id)
                                    LEFT OUTER JOIN study_program_specialization using(study_program_specialization_id)
                                    LEFT OUTER JOIN academic_rank using(academic_rank_id)
                                    LEFT OUTER JOIN study_field using(study_field_id)

                                    WHERE user.user_id = @id;";

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

            User user = null;

            using (var connection = new MySqlConnection(Config.dbConnectionString))
            {
                await connection.OpenAsync();
                command.Connection = connection;

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        user = GetUserFromRow(reader);
                    }
                }

                await connection.CloseAsync();
            }

            return user;
        }

        private User GetUserFromRow(MySqlDataReader reader)
        {
            User user = new User();

            UserAccountType accountType = (UserAccountType)Enum.ToObject(typeof(UserAccountType), reader.GetInt32(1));

            if (accountType is UserAccountType.Student)
            {
                Student student = new Student
                {
                    StudyYear = reader.GetInt32(6)
                };

                int? facultyId = !reader.IsDBNull(7) ? (int?)reader.GetInt32(7) : null;
                int? programId = !reader.IsDBNull(8) ? (int?)reader.GetInt32(8) : null;
                int? specializationId = !reader.IsDBNull(9) ? (int?)reader.GetInt32(9) : null;

                if (facultyId != null)
                {
                    Faculty faculty = ((App)App.Current).faculties.GetFacultyById((int)facultyId);

                    if (programId != null)
                    {
                        StudyProgram program = faculty.StudyPrograms[(int)programId];
                        student.StudyProgram = program;

                        if (specializationId != null)
                        {
                            StudyProgramSpecialization specialization = program.StudyProgramSpecializations[(int)specializationId];
                            student.StudyProgramSpecialization = specialization;
                        }
                    }
                }

                user = student;
            }
            else if (accountType is UserAccountType.FacultyMember)
            {
                FacultyMember facultyMember = new FacultyMember
                {
                    Faculty = ((App)App.Current).faculties.GetFacultyById(reader.GetInt32(10)),
                    AcademicRank = (AcademicRank)Enum.ToObject(typeof(AcademicRank), reader.GetInt32(11)),
                    StudyField = ((App)App.Current).faculties.GetStudyFieldById(reader.GetInt32(12))
                };

                user = facultyMember;
            }

            user.UserId = reader.GetInt32(0);
            user.FirstName = reader.GetString(2);
            user.LastName = reader.GetString(3);
            user.PhoneNumber = !reader.IsDBNull(4) ? reader.GetString(4) : null;
            user.PhoneFormat = !reader.IsDBNull(4) ? reader.GetString(5) : null;

            return user;
        }
    }
}
