using Microsoft.AspNetCore.Mvc;
using Posttrack.BLL.Interfaces;
using Posttrack.Web.Models;
using System.Threading.Tasks;

namespace Posttrack.Web.Controllers
{
    public class TrackingController : Controller
    {
        private readonly IPackagePresentationService service;

        public TrackingController(IPackagePresentationService service)
        {
            this.service = service;
        }

        [HttpPost, Route("tracking")]
        public async Task<OperationResult> Post(SaveTrackingModel model)
        {
            if (ModelState.IsValid)
            {
                await service.Register(model.Map());
                return new OperationResult(true);
            }

            return new OperationResult(false, ModelState);
        }
    }
}