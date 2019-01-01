using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LiquidVictor.Output.RevealJs.Layout.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SlidesController : ControllerBase
    {
        // GET api/slides
        [HttpGet]
        public ActionResult<string> Get([FromServices] Interfaces.ILayoutStrategy strategy)
        {
            return strategy.GetType().FullName;
        }

        // POST api/values
        [HttpPost]
        public string Post([FromServices] Interfaces.ILayoutStrategy strategy, [FromBody] Entities.Slide slide)
        {
            return strategy.Layout(slide);
        }

    }
}
