using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using ZergTracker.Models;

namespace ZergTracker.Helper
{
    public class UserProjectsHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public bool IsUserOnProject(string userId, int projectId)
        {
            var project = db.Projects.Find(projectId);
            var flag = project.Users.Any(u => u.Id == userId);
            return (flag);
        }

        public ICollection<Project> ListUserProjects(string userId)
        {
            ApplicationUser user = db.Users.Find(userId);

            var projects = user.Projects.ToList();
            return (projects);
        }

        public void AddUserToProject(string userId, int projectId)
        {
            if (!IsUserOnProject(userId, projectId))
            {
                Project proj = db.Projects.Find(projectId);
                var newUser = db.Users.Find(userId);

                proj.Users.Add(newUser);
                db.SaveChanges();
            }
        }

        public void AddPM(string userId, int projectId)
        {
            Project proj = db.Projects.Find(projectId);
            proj.ProjectManagerId = userId;
            
            db.SaveChanges();
        }

        public void RemovePM(string userId, int projectId)
        {
            Project proj = db.Projects.Find(projectId);
            proj.ProjectManagerId = null;

            db.SaveChanges();
        }

        public void RemoveUserFromProject(string userId, int projectId)
        {
            if (IsUserOnProject(userId, projectId))
            {
                var proj = db.Projects.Find(projectId);
                var userThing = db.Users.Find(userId);
                proj.Users.Remove(userThing);
                db.Entry(proj).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public ICollection<ApplicationUser> UsersOnProject(int projectId)
        {
            return db.Projects.Find(projectId).Users;
        }

        public ICollection<ApplicationUser> UsersNotOnProject(string projectId)
        {
            return db.Users.Where(u => u.Projects.All(p => p.Name != projectId)).ToList();
        }
    }
}