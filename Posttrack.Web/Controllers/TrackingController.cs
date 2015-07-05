using Posttrack.Web.Models;
using Posttrack.BLL.Interfaces;
using System.Web.Http;

namespace Posttrack.Web.Controllers
{
    public class TrackingController : ApiController
    {
        private readonly IPackagePresentationService service;

        public TrackingController(IPackagePresentationService service)
        {
            this.service = service;
        }

        [HttpPost]
        public OperationResult Index(SaveTrackingModel model)
        {
            if (ModelState.IsValid)
            {
                service.Register(model.Map());
                return new OperationResult(true);
            }

            return new OperationResult(false, ModelState);
        }
    }
}