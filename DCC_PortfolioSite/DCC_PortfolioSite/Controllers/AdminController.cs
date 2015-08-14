﻿using DCC_PortfolioSite.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DCC_PortfolioSite.Controllers
{
    public class AdminController : Controller
    {

        AlumniDBModel db = new AlumniDBModel();

        // SAVE PROFILE
        [HttpPost]
        [Authorize]
        public ActionResult SaveProfile(AdminViewModel model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model.ContactProfile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return View(model);
            }
        }


        // SAVE PROJECT
        [HttpPost]
        [Authorize]
        public ActionResult SaveProject(AdminViewModel model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model.ProjectSpotlight).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return View(model);
            }
        }

        // GET: Admin
        [Authorize]
        public ActionResult Index()
        {
            AdminViewModel adminViewModel = new AdminViewModel();
            string UserID = User.Identity.Name;            

            adminViewModel.ContactProfile = db.ContactProfiles.FirstOrDefault(p => p.PrimaryEmail == UserID);
            adminViewModel.UserResume = db.UserResumes.FirstOrDefault(p => p.ContactProfile.PrimaryEmail == UserID);
            adminViewModel.ProjectSpotlight = db.ProjectSpotlights.FirstOrDefault(p => p.ContactProfile.PrimaryEmail == UserID);
            var projects = (from p in db.ProjectSpotlights
                           where p.ContactProfile.PrimaryEmail == UserID
                           select p).ToList();
            adminViewModel.ProjectSpotlightList = projects;
            
            return View(adminViewModel);
        }

        // REDIRECT: to Edit page
        [Authorize]
        public ActionResult Edit()
        {
            AdminViewModel adminViewModel = new AdminViewModel();

            string UserID = User.Identity.Name;


            adminViewModel.ContactProfile = db.ContactProfiles.FirstOrDefault(p => p.PrimaryEmail == UserID);
            adminViewModel.UserResume = db.UserResumes.FirstOrDefault(p => p.ContactProfile.PrimaryEmail == UserID);
            adminViewModel.ProjectSpotlight = db.ProjectSpotlights.FirstOrDefault(p => p.ContactProfile.PrimaryEmail == UserID);
            var projects = (from p in db.ProjectSpotlights
                            where p.ContactProfile.PrimaryEmail == UserID
                            select p).ToList();
            adminViewModel.ProjectSpotlightList = projects;
            
            return View("Index_Edit", adminViewModel);
        }

        // GET: Resume
        [Authorize]
        public ActionResult Resume()
        {
            UserResume results = db.UserResumes.FirstOrDefault(r => r.ProfileID == 2);
                            

            return View("Resume", results);
        }

        [HttpPost]
        [Authorize]
        public ActionResult ImageUploadPhoto()
        {
            var image = Request.Files["image"];
            if (image == null)
            {
                ViewBag.UploadMessage = "Failed to upload image";
            }
            else
            {
                ViewBag.UploadMessage = String.Format("Got image {0} of type {1} and size {2}",
                    image.FileName, image.ContentType, image.ContentLength);
                // TODO: actually save the image to Azure blob storage

                var connectionString = @"DefaultEndpointsProtocol=https;AccountName=dccportfolio;AccountKey=/MxXUfGzY8W+e0GTYUTQtA4EnlfgaROeUhPipxRFew7ckKk5sXiHDmDZmIOd4AkZ6luZS994UXYaPeRKboHOaA==";
                var account = CloudStorageAccount.Parse(connectionString);

                //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
                CloudBlobClient blobClient = account.CreateCloudBlobClient();

                // Retrieve a reference to a container.
                CloudBlobContainer container = blobClient.GetContainerReference("pictures");

                // Create the container if it doesn't already exist.
                container.CreateIfNotExists();
                container.SetPermissions(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });

                string uniqueBlobName = string.Format("productimages/image_{0}{1}",
                Guid.NewGuid().ToString(), Path.GetExtension(image.FileName));
                CloudBlockBlob blob = container.GetBlockBlobReference(uniqueBlobName);
                blob.Properties.ContentType = image.ContentType;
                blob.UploadFromStream(image.InputStream);
                blob.Uri.ToString();

                string connectionStringDB = "Server=tcp:wx9a1lruht.database.windows.net,1433;Database=DCCPortfolioSite_db;User ID=devcodecamp;Password=heliumdev1!;Trusted_Connection=False;Encrypt=True;Connection Timeout=30";
                using (SqlConnection connection = new SqlConnection(connectionStringDB))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE ContactProfile SET Photo= @Photo WHERE PrimaryEmail = @fNameUser");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@Photo", blob.Uri.ToString());
                    cmd.Parameters.AddWithValue("@fNameUser", User.Identity.Name);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Edit", "Admin");
        }

        [HttpPost]
        [Authorize]
        public ActionResult ResumeUpload()
        {
            AlumniDBModel db = new AlumniDBModel();
            var image = Request.Files["resume"];
            if (image == null)
            {
                ViewBag.UploadMessage = "Failed to upload image";
            }
            else
            {


                var connectionString = @"DefaultEndpointsProtocol=https;AccountName=dccportfolio;AccountKey=/MxXUfGzY8W+e0GTYUTQtA4EnlfgaROeUhPipxRFew7ckKk5sXiHDmDZmIOd4AkZ6luZS994UXYaPeRKboHOaA==";
                var account = CloudStorageAccount.Parse(connectionString);

                //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
                CloudBlobClient blobClient = account.CreateCloudBlobClient();

                //Retrieve a reference to a container.
                CloudBlobContainer container = blobClient.GetContainerReference("pictures");

                //Create the container if it doesn't already exist.
                container.CreateIfNotExists();
                container.SetPermissions(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });

                string uniqueBlobName = string.Format("productimages/image_{0}{1}",
                Guid.NewGuid().ToString(), Path.GetExtension(image.FileName));
                CloudBlockBlob blob = container.GetBlockBlobReference(uniqueBlobName);
                blob.Properties.ContentType = image.ContentType;
                blob.UploadFromStream(image.InputStream);
                blob.Uri.ToString();
                
                string connectionStringDB = "Server=tcp:wx9a1lruht.database.windows.net,1433;Database=DCCPortfolioSite_db;User ID=devcodecamp;Password=heliumdev1!;Trusted_Connection=False;Encrypt=True;Connection Timeout=30";
                using (SqlConnection connection = new SqlConnection(connectionStringDB))
                {
                    connection.Open();
                    ContactProfile profileId = (from contact in db.ContactProfiles
                                     where contact.PrimaryEmail == User.Identity.Name
                                     select contact).FirstOrDefault();
                    SqlCommand cmd = new SqlCommand("UPDATE UserResume SET HtmlUpload = @Resume WHERE ProfileID = @ResumeName");
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = connection;
                    cmd.Parameters.AddWithValue("@Resume", blob.Uri.ToString());
                    cmd.Parameters.AddWithValue("@ResumeName", profileId.ProfileId);

                    cmd.ExecuteNonQuery();
                }


            }
            return RedirectToAction("Edit", "Admin");
        }





        // GET: CREATE PROJECT
        [Authorize]
        public ActionResult CreateProject(int ContactID)
        {
            ProjectSpotlight model = new ProjectSpotlight();
            model.ProfileID = ContactID;
            return View(model);
        }
        // POST: Save Data
        [HttpPost]
        [Authorize]
        public ActionResult CreateProject(ProjectSpotlight model)
        {
            int insertID = -1;
            string connectionStringDB = "Server=tcp:wx9a1lruht.database.windows.net,1433;Database=DCCPortfolioSite_db;User ID=devcodecamp;Password=heliumdev1!;Trusted_Connection=False;Encrypt=True;Connection Timeout=30";
            using (SqlConnection connection = new SqlConnection(connectionStringDB))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("INSERT INTO ProjectSpotlight (ProfileID, ProjectName, Technologies, DevelopmentTime, ProjectDescription, RepoLink, Image_1, Image_2) " +
                    "output INSERTED.ProjectSpotlightID " +
                    "VALUES(@pID, @pName, @tech, @dTime, @pDescrip, @rLink, @i1, @i2)");
                //ProjectName = @pName, Technologies = @tech, DevelopmentTime = @dTime, ProjectDescription = @pDescrip, RepoLink = @rLink, Image_1 = @i1, Image_2 = @i2 WHERE PrimaryEmail = @fNameUser");
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;

                cmd.Parameters.AddWithValue("@pID", (int.Equals(model.ProfileID, null)) ? 0 : model.ProfileID);
                cmd.Parameters.AddWithValue("@pName", (String.IsNullOrEmpty(model.ProjectName)) ? " " : model.ProjectName);
                cmd.Parameters.AddWithValue("@tech", (String.IsNullOrEmpty(model.Technologies)) ? " " : model.Technologies);
                cmd.Parameters.AddWithValue("@dTime", (String.IsNullOrEmpty(model.DevelopmentTime)) ? " " : model.DevelopmentTime);
                cmd.Parameters.AddWithValue("@pDescrip", (String.IsNullOrEmpty(model.ProjectDescription)) ? " " : model.ProjectDescription);
                cmd.Parameters.AddWithValue("@rLink", (String.IsNullOrEmpty(model.RepoLink)) ? " " : model.RepoLink);
                cmd.Parameters.AddWithValue("@i1", (String.IsNullOrEmpty(model.Image_1)) ? " " : model.Image_1);
                cmd.Parameters.AddWithValue("@i2", (String.IsNullOrEmpty(model.Image_2)) ? " " : model.Image_2);

                cmd.Parameters.AddWithValue("@fNameUser", User.Identity.Name);

                insertID = (int)cmd.ExecuteScalar();
            }

            ProjectSpotlight results = db.ProjectSpotlights.FirstOrDefault(r => r.ProjectSpotlightID == insertID);
            return RedirectToAction("EditProject", new { ProjectID = results.ProjectSpotlightID });
        }

        // GET: EDIT PROJECT
        [Authorize]
        public ActionResult EditProject(int ProjectID)
        {
            ProjectSpotlight model = db.ProjectSpotlights.FirstOrDefault(r => r.ProjectSpotlightID == ProjectID);         
            return View("EditProject", model);
        }
        // POST: Update Project Data
        [HttpPost]
        [Authorize]
        public ActionResult EditProject(ProjectSpotlight model)
        {           
            string connectionStringDB = "Server=tcp:wx9a1lruht.database.windows.net,1433;Database=DCCPortfolioSite_db;User ID=devcodecamp;Password=heliumdev1!;Trusted_Connection=False;Encrypt=True;Connection Timeout=30";
            using (SqlConnection connection = new SqlConnection(connectionStringDB))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("UPDATE ProjectSpotlight SET ProjectName = @pName, Technologies = @tech, DevelopmentTime = @dTime, ProjectDescription = @pDescrip, RepoLink = @rLink, Image_1 = @i1, Image_2 = @i2 WHERE ProjectSpotlightID = @ProjectID");
                //ProjectName = @pName, Technologies = @tech, DevelopmentTime = @dTime, ProjectDescription = @pDescrip, RepoLink = @rLink, Image_1 = @i1, Image_2 = @i2 WHERE PrimaryEmail = @fNameUser");
                cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;

                cmd.Parameters.AddWithValue("@pName", (String.IsNullOrEmpty(model.ProjectName)) ? " " : model.ProjectName);
                cmd.Parameters.AddWithValue("@tech", (String.IsNullOrEmpty(model.Technologies)) ? " " : model.Technologies);
                cmd.Parameters.AddWithValue("@dTime", (String.IsNullOrEmpty(model.DevelopmentTime)) ? " " : model.DevelopmentTime);
                cmd.Parameters.AddWithValue("@pDescrip", (String.IsNullOrEmpty(model.ProjectDescription)) ? " " : model.ProjectDescription);
                cmd.Parameters.AddWithValue("@rLink", (String.IsNullOrEmpty(model.RepoLink)) ? " " : model.RepoLink);
                cmd.Parameters.AddWithValue("@i1", (String.IsNullOrEmpty(model.Image_1)) ? " " : model.Image_1);
                cmd.Parameters.AddWithValue("@i2", (String.IsNullOrEmpty(model.Image_2)) ? " " : model.Image_2);

                cmd.Parameters.AddWithValue("@ProjectID", (int.Equals(model.ProjectSpotlightID, null)) ? 0 : model.ProjectSpotlightID);

                cmd.ExecuteNonQuery();
            }

            ProjectSpotlight refreshModel = db.ProjectSpotlights.FirstOrDefault(r => r.ProjectSpotlightID == model.ProjectSpotlightID);
            return View("EditProject", refreshModel);
        }
        // POST: Delete Project Data
        [HttpPost]
        [Authorize]
        public ActionResult DeleteProject(int ProjectSpotlightID)
        {
            ProjectSpotlight project = db.ProjectSpotlights.Single(p => p.ProjectSpotlightID == ProjectSpotlightID);
            if (project == null)
                return View("NotFound");

            db.ProjectSpotlights.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Edit", "Admin");
        }
    }
}