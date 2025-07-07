using control_ppt_server.Models;
using Microsoft.Office.Interop.PowerPoint;
using System.Diagnostics;

namespace control_ppt_server.Services
{
    public class CreatePresentationService : ICreatePresentationService
    {
        public void Dispose()
        {
            Debug.Print("dispose");
        }
    }
}
