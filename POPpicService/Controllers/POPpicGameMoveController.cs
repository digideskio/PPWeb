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
    public class POPpicGameMoveController : TableController<POPpicGameMove>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            POPpicContext context = new POPpicContext();
            DomainManager = new EntityDomainManager<POPpicGameMove>(context, Request, Services);
        }

        // GET tables/POPpicGameMove
        public IQueryable<POPpicGameMove> GetAllPOPpicGameMove()
        {
            return Query(); 
        }

        // GET tables/POPpicGameMove/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<POPpicGameMove> GetPOPpicGameMove(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/POPpicGameMove/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<POPpicGameMove> PatchPOPpicGameMove(string id, Delta<POPpicGameMove> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/POPpicGameMove/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<IHttpActionResult> PostPOPpicGameMove(POPpicGameMove item)
        {
            POPpicGameMove current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/POPpicGameMove/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeletePOPpicGameMove(string id)
        {
             return DeleteAsync(id);
        }

    }
}