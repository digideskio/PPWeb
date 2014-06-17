using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using POPpicService.DataObjects;
using POPpicService.Models;

namespace POPpicService.Controllers
{
    public class POPpicGameController : TableController<POPpicGame>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            POPpicContext context = new POPpicContext();
            DomainManager = new EntityDomainManager<POPpicGame>(context, Request, Services);
        }

        // GET tables/POPpicGame
        public IQueryable<POPpicGame> GetAllPOPpicGame()
        {
            return Query(); 
        }

        // GET tables/POPpicGame/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<POPpicGame> GetPOPpicGame(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/POPpicGame/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<POPpicGame> PatchPOPpicGame(string id, Delta<POPpicGame> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/POPpicGame/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<IHttpActionResult> PostPOPpicGame(POPpicGame item)
        {
            POPpicGame current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/POPpicGame/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeletePOPpicGame(string id)
        {
             return DeleteAsync(id);
        }

    }
}