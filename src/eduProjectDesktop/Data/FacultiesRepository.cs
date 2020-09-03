using eduProjectDesktop.Model.Domain;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Streaming.Adaptive;
using Windows.UI.Xaml.Automation.Peers;

namespace eduProjectDesktop.Data
{
    public class FacultiesRepository
    {

        private Dictionary<int, Faculty> faculties = new Dictionary<int, Faculty>();
        private Dictionary<int, StudyField> studyFields = new Dictionary<int, StudyField>();

        public static FacultiesRepository instance;

        public async static Task CreateAsync()
        {
            var repository = new FacultiesRepository();

            // retrieving faculties

            string commandText = @"SELECT faculty_id, faculty.name, address, 
	                               study_program_id, cycle, duration_years, study_program.name,
                                   study_program_specialization_id, study_program_specialization.name
                                   FROM faculty
                                   INNER JOIN study_program USING(faculty_id)
                                   LEFT OUTER JOIN study_program_specialization USING(study_program_id)";

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
                            // id-jevi fakulteta, programa i smjera iz tabele rezultata

                            int facultyId = reader.GetInt32(0);
                            int programId = reader.GetInt32(3);
                            int? specializationId = !(reader.IsDBNull(7)) ? (int?)reader.GetInt32(7) : null;

                            if (!repository.faculties.ContainsKey(facultyId))
                            {
                                // dodajemo novi fakultet

                                var faculty = GetFacultyFromRow(reader);
                                repository.faculties.Add(facultyId, faculty);

                                var program = GetStudyProgramFromRow(reader);
                                faculty.AddStudyProgram(programId, program);

                                if (specializationId != null)
                                {
                                    var specialization = GetStudyProgramSpecializationFromRow(reader);
                                    program.AddStudyProgramSpecialization((int)specializationId, specialization);
                                }
                            }
                            else
                            {
                                // fakultet postoji, provjeravamo sadrzi li taj program

                                var faculty = repository.faculties[facultyId];
                                if (!faculty.StudyPrograms.ContainsKey(programId))
                                {
                                    // dodajemo novi studijski program i gledamo da li smjer postoji

                                    var program = GetStudyProgramFromRow(reader);
                                    faculty.AddStudyProgram(programId, program);

                                    if (specializationId != null)
                                    {
                                        var specialization = GetStudyProgramSpecializationFromRow(reader);
                                        program.AddStudyProgramSpecialization((int)specializationId, specialization);
                                    }
                                }
                                else
                                {
                                    // program postoji, smjer mora postojati

                                    var program = faculty.StudyPrograms[programId];
                                    var specialization = GetStudyProgramSpecializationFromRow(reader);
                                    program.AddStudyProgramSpecialization((int)specializationId, specialization);
                                }
                            }
                        }
                    }
                }

                await connection.CloseAsync();
            }

            // retrieving study fields

            commandText = @"SELECT study_field_id, name, description FROM study_field";

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
                            repository.studyFields.Add(reader.GetInt32(0), new StudyField
                            {
                                Name = reader.GetString(1),
                                Description = !reader.IsDBNull(2) ? reader.GetString(1) : null
                            });
                        }
                    }
                }

                await connection.CloseAsync();
            }


            instance = repository;
        }


        // vadi faculty data; ne dodaje programe u listu
        private static Faculty GetFacultyFromRow(MySqlDataReader reader)
        {
            return new Faculty
            {
                Name = reader.GetString(1),
                Address = reader.GetString(2)
            };
        }

        private static StudyProgram GetStudyProgramFromRow(MySqlDataReader reader)
        {
            return new StudyProgram
            {
                Cycle = (byte)reader.GetInt32(4),
                DurationYears = (byte)reader.GetInt32(5),
                Name = reader.GetString(6)
            };
        }

        private static StudyProgramSpecialization GetStudyProgramSpecializationFromRow(MySqlDataReader reader)
        {
            return new StudyProgramSpecialization
            {
                Name = reader.GetString(8)
            };
        }

        public IEnumerable<Faculty> GetAll()
        {
            return faculties.Values.ToList();
        }

        public Faculty GetFacultyById(int id)
        {
            return faculties[id];
        }

        public List<StudyField> GetAllStudyFields()
        {
            return studyFields.Values.ToList();
        }

        public StudyField GetStudyFieldById(int id)
        {
            return studyFields[id];
        }

        public int GetStudyFieldId(StudyField studyField)
        {
            foreach (var item in studyFields)
            {
                if (item.Value.Name == studyField.Name && item.Value.Description == studyField.Description)
                    return item.Key;
            }

            return -1;
        }

        public int GetFacultyId(Faculty faculty)
        {
            foreach (var item in faculties)
            {
                if (item.Value.Name == faculty.Name && item.Value.Address == faculty.Address)
                    return item.Key;
            }

            return -1;
        }

        public int GetStudyProgramId(Faculty faculty, StudyProgram studyProgram)
        {
            foreach (var item in faculty.StudyPrograms)
            {
                if (item.Value.Name == studyProgram.Name)
                    return item.Key;
            }

            return -1;
        }

        public int GetStudyProgramSpecializationId(StudyProgram program, StudyProgramSpecialization specialization)
        {
            foreach (var item in program.StudyProgramSpecializations)
            {
                if (item.Value.Name == specialization.Name)
                    return item.Key;
            }

            return -1;
        }



    }
}
