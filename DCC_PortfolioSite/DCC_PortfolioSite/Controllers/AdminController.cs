﻿using DCC_PortfolioSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DCC_PortfolioSite.Controllers
{
    public class AdminController : Controller
    {

        AlumniDBModel db = new AlumniDBModel();

        // GET: Admin
        public ActionResult Index()
        {
            AdminViewModel adminViewModel = new AdminViewModel();

            string UserID = User.Identity.Name;

            
            adminViewModel.ContactProfile = db.ContactProfiles.FirstOrDefault(p => p.PrimaryEmail == UserID);
            adminViewModel.UserResume = db.UserResumes.FirstOrDefault(p => p.ContactProfile.PrimaryEmail == UserID);
            adminViewModel.ProjectSpotlight = db.ProjectSpotlights.FirstOrDefault(p => p.ContactProfile.PrimaryEmail == UserID);
            
            //adminViewModel.UserProfile = db.UserProfiles.FirstOrDefault(p => p.UserID == 2);
            //adminViewModel.ContactProfile = db.ContactProfiles.FirstOrDefault(p => p.ProfileId == 2);
            //adminViewModel.UserResume = db.UserResumes.FirstOrDefault(p => p.UserResumeID == 2);
            //adminViewModel.ProjectSpotlight = db.ProjectSpotlights.FirstOrDefault(p => p.ProjectSpotlightID == 3);

            return View(adminViewModel);
        }

        // REDIRECT: to Edit page
        public ActionResult Edit()
        {
            AdminViewModel adminViewModel = new AdminViewModel();

            string UserID = User.Identity.Name;


            adminViewModel.ContactProfile = db.ContactProfiles.FirstOrDefault(p => p.PrimaryEmail == UserID);
            adminViewModel.UserResume = db.UserResumes.FirstOrDefault(p => p.ContactProfile.PrimaryEmail == UserID);
            adminViewModel.ProjectSpotlight = db.ProjectSpotlights.FirstOrDefault(p => p.ContactProfile.PrimaryEmail == UserID);
            
            return View("Index_Edit", adminViewModel);
        }

        // GET: Resume
        public ActionResult Resume()
        {
            UserResume results = db.UserResumes.FirstOrDefault(r => r.ProfileID == 2);
                            

            return View("Resume", results);
        }


    }
}