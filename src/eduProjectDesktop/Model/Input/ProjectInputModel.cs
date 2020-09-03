using eduProjectDesktop.Model.Display;
using eduProjectDesktop.Model.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace eduProjectDesktop.Model.Input
{
    public class ProjectInputModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string StudyFieldName { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public List<string> TagNames { get; set; } = new List<string>();
        public List<CollaboratorProfileInputModel> CollaboratorProfileInputModels { get; set; } = new List<CollaboratorProfileInputModel>();

        public static Project ToProject(ProjectInputModel model)
        {
            Project project = new Project
            {
                AuthorId = User.CurrentUserId,
                Title = model.Title,
                ProjectStatus = ProjectStatus.Active,
                Description = model.Description,
                StudyField = ((App)App.Current).faculties.GetAllStudyFields().First(sf => sf.Name == model.StudyFieldName)
            };

            if (model.StartDate != null)
            {
                project.StartDate = model.StartDate.DateTime;
            }

            if (model.EndDate != null)
            {
                project.EndDate = model.EndDate.Date;
            }

            foreach (var item in model.CollaboratorProfileInputModels)
            {
                if (item is StudentProfileInputModel sInputModel)
                {
                    project.CollaboratorProfiles.Add(StudentProfileInputModel.ToStudentProfile(sInputModel));
                }
                else if (item is FacultyMemberProfileInputModel fmInputModel)
                {
                    project.CollaboratorProfiles.Add(FacultyMemberProfileInputModel.ToFacultyMemberProfile(fmInputModel));
                }
            }

            foreach (var tagName in model.TagNames)
            {
                project.Tags.Add(((App)App.Current).tags.GetAll().First(t => t.Name == tagName));
            }

            return project;
        }

    }
}
