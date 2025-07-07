using control_ppt_server.Models;
using control_ppt_server.Models.Responses;
using Microsoft.Office.Interop.PowerPoint;

namespace control_ppt_server.Services
{
    public interface ISlideshowControlService : IDisposable
    {
        List<Models.PresentationInfo> GetActiveSlideshows();
        (Presentation?,int) DoesSlideExist(string presentationId, int slideChange);
        void ChangeSlide(Presentation item1, int item2, SlideshowPreviewOptions options);
    }
}
