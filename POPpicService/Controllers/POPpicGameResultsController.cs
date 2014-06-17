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
    public class POPpicGameResultsController : TableController<POPpicGameResults>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            POPpicContext context = new POPpicContext();
            DomainManager = new EntityDomainManager<POPpicGameResults>(context, Request, Services);
        }

        // GET tables/POPpicGameResults
        public IQueryable<POPpicGameResults> GetAllPOPpicGameResults()
        {
            return Query(); 
        }

        // GET tables/POPpicGameResults/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<POPpicGameResults> GetPOPpicGameResults(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/POPpicGameResults/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<POPpicGameResults> PatchPOPpicGameResults(string id, Delta<POPpicGameResults> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/POPpicGameResults/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<IHttpActionResult> PostPOPpicGameResults(POPpicGameResults item)
        {
            POPpicGameResults current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/POPpicGameResults/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeletePOPpicGameResults(string id)
        {
             return DeleteAsync(id);
        }

    }
}