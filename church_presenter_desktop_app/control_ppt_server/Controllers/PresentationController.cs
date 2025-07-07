using Common.Models.Requests;
using control_ppt_server.Models.Requests;
using control_ppt_server.Services;
using control_ppt_server.utils;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace control_ppt_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PresentationController: ControllerBase
    {

        private readonly ICreatePresentationService _createService;
        public PresentationController(ICreatePresentationService createService)
        {
            _createService = createService;
        }

        [HttpPost("create-presentation")]
        public IActionResult CreatePresentation([FromBody] CreatePresentationRequest req)
        {
            Debug.WriteLine("create presentation");
            try
            {
                var tempFilePath = Path.GetTempFileName() + ".pptx";

                // Create presentation using your existing utility
                using (var presentationDocument = PowerPointUtils.CreatePresentation(tempFilePath))
                {
                    // Add slides for each song
                    foreach (var song in req.Songs)
                    {
                        var titleText = song.Title;
                        SlideHelper.AddSlideWithTextBox(presentationDocument, titleText, song.Text2);
                    }

                    presentationDocument.PresentationPart?.Presentation.Save();
                }

                // Return the file
                var fileBytes = System.IO.File.ReadAllBytes(tempFilePath);
                System.IO.File.Delete(tempFilePath); // Clean up

                return File(fileBytes, "application/vnd.openxmlformats-officedocument.presentationml.presentation", "lyrics_presentation.pptx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generating presentation: {ex.Message}");
            }
        }
    }
}
