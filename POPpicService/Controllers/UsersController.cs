using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using POPpicService.Models;
using System.IO;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Microsoft.WindowsAzure.Mobile.Service.Notifications;
using System.Threading.Tasks;
using POPpicService.DataObjects;

namespace POPpicService.Controllers
{
    public class UsersController : ApiController
    {
        public ApiServices Services { get; set; }
        POPpicContext context;
        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            this.context = new POPpicContext();
        }

        // GET api/Users
        public string Get()
        {
            Services.Log.Info("Hello from custom controller!");
            return "Hello";
        }

        [Route("api/Users/me")]
        public string GetMe()
        {
            string userId = Utilities.GetUserId(this.User);
            var currentUser = this.context.POPpicUsers.FirstOrDefault(u => u.Id == userId);
            if (currentUser == null)
            {
                currentUser = Utilities.InitializeNewUser(this.User, this.Services.Log);
                currentUser = context.POPpicUsers.Add(currentUser);
                context.SaveChanges();
            }

            string output =  CrossPlatformUtilities.SerializeItem<POPpicUser>(currentUser);
            return output;
        }

        [Route("api/Users/signIn")]
        public string PostSignIn()
        {
            return GetMe();
        }

        [Route("api/Users/registerDevice")]
        public string GetRegisterDevice()
        {
            return "";
        }

        [Route("api/Users/facebookFriends")]
        public async Task<string> GetFacebookFriends()
        {
			var query = @"SELECT uid FROM user WHERE is_app_user = '1' AND uid IN (SELECT uid2 FROM friend WHERE uid1 = me())";
            var facebookClient = Utilities.InitializeFacebookClient(this.User, this.Services.Log);
			var results = (IDictionary<string, object>)(await facebookClient.GetTaskAsync ("fql", new { q = query }));

			var friendsFacebookIds = new HashSet<string> ();
			var list = (IEnumerable<object>)results ["data"];
			foreach (var friend in list) {
				var friendData = (IDictionary<string, object>)friend;
				var userId = (string)friendData ["uid"];
				friendsFacebookIds.Add (userId);
			}

            var facebookFriends = this.context.POPpicUsers.Where(u => friendsFacebookIds.Contains(u.Id)).ToList();

            var writer = new StringWriter();
            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            serializer.Serialize(writer, facebookFriends);
            return writer.ToString();
        }

    }
}
