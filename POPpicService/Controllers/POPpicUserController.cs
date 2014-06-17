using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using POPpicService.DataObjects;
using POPpicService.Models;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using System.Data.Entity;

namespace POPpicService.Controllers
{
    // [AuthorizeLevel(AuthorizationLevel.User)] 
    public class POPpicUserController : TableController<POPpicUser>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            POPpicContext context = new POPpicContext();
            DomainManager = new EntityDomainManager<POPpicUser>(context, Request, Services);
        }

        // GET tables/POPpicUser
        public IQueryable<POPpicUser> GetAllPOPpicUser()
        {
            return Query(); 
        }

        // GET tables/POPpicUser/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<POPpicUser> GetPOPpicUser(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/POPpicUser/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<POPpicUser> PatchPOPpicUser(string id, Delta<POPpicUser> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/POPpicUser/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<IHttpActionResult> PostPOPpicUser(POPpicUser item)
        {
            POPpicUser current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/POPpicUser/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeletePOPpicUser(string id)
        {
             return DeleteAsync(id);
        }

    }
}