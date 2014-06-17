﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using POPpicService.DataObjects;
using POPpicService.Models;
using System.Data.Entity.Migrations;
using POPpicService.Migrations;

namespace POPpicService
{
    public static class WebApiConfig
    {
        public static void Register()
        {
            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();

            // Use this class to set WebAPI configuration options
            HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options));

            // To display errors in the browser during development, uncomment the following
            // line. Comment it out again when you deploy your service for production use.
            // config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            
            // Database.SetInitializer(new POPpicInitializer());
            var migrator = new DbMigrator(new Configuration());
            migrator.Update();
        }
    }

    public class POPpicInitializer : DropCreateDatabaseIfModelChanges<POPpicContext>
    {
        protected override void Seed(POPpicContext context)
        {
            base.Seed(context);
        }
    }
}

