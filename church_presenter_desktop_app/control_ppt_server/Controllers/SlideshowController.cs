using control_ppt_server.Models;
using control_ppt_server.Models.Requests;
using control_ppt_server.Models.Responses;
using control_ppt_server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Office.Interop.PowerPoint;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;

namespace control_ppt_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SlideshowController : ControllerBase
    {

        private readonly ISlideshowControlService _controlService;
        public SlideshowController(ISlideshowControlService controlService)
        {
            _controlService = controlService;
        }


        [HttpGet("get-slideshows")]
        public IActionResult GetSlideshows()
        {
            var response = new GetSlideshowsResponse();
            try
            {
                response.presentations = _controlService.GetActiveSlideshows();
                return Ok(response);
            }
            catch
            {
                return Problem("Error connecting to PowerPoint. Check if PowerPoint is open.");
            }
        }


        [HttpPost("change-slide")]
        public IActionResult ChangeSlide([FromBody] SlideChangeRequest req)
        {
            var response = new SlideChangeResponse();
            var presentations = new Dictionary<string, (Presentation,int)>();

            foreach (var presId in req.PresentationIds)
            {
                try
                {
                    var presentation = _controlService.DoesSlideExist(presId, req.SlideChange);
                    if (presentation.Item2 < 1)
                    {
                        Debug.WriteLine("${presId} ${ slideshowView.Item2}");
                        response.Success = false;
                        if (presentation.Item2 == 0)
                            response.Presentations.Add(OperationResult.CreateError(405, "Slide does not exist",new PresentationInfo(presId,presentation.Item1!)));
                        if (presentation.Item2 == -1)
                            response.Presentations.Add(OperationResult.CreateError(404, "Presentation does not exist"));
                    }
                    else
                    {
                        presentations[presId] = (presentation.Item1!, presentation.Item2);
                    }
                }
                catch(Exception e)
                {
                    Debug.WriteLine(e.ToString());
                    response.Presentations.Add(OperationResult.CreateError(500, "Error"));
                }
            }
            if (!response.Success)
            {
                return BadRequest(response);
            }
            else
            {
                foreach (var presId in presentations.Keys)
                {
                    try
                    {
                        _controlService.ChangeSlide(presentations[presId].Item1, presentations[presId].Item2, req.Options);
                        response.Presentations.Add(
                            OperationResult.CreateSuccess(info: new PresentationInfo(presId, presentations[presId].Item1))
                        );
                    }
                    catch
                    {
                        response.Presentations.Add(
                            OperationResult.CreateError(500, "Error changing slide", new PresentationInfo(presId, presentations[presId].Item1))
                        );
                    }
                }
                return Ok(response);
            }
        }
    }
}
