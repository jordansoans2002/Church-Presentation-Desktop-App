using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace create_ppt_app.Model.DriveDTO
{ 
    class FileListResponse
    {
        public DriveFile[] files { get; set; }
        public string NextPageToken { get; set; }
    }
}
