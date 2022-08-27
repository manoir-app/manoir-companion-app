using Home.Common.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HomeAutomationApp.Business
{
    public class MeshBll : BaseBll
    {
        public async Task<List<InternetConnection>> GetConnection()
        {
            var url = $"v1.0/system/mesh/local/internet-connections";
            return await DownloadData<List<InternetConnection>>(url);
        }

        public async Task<AutomationMesh> Get()
        {
            var url = $"v1.0/system/mesh/local";
            return await DownloadData<AutomationMesh>(url);
        }

    }
}
