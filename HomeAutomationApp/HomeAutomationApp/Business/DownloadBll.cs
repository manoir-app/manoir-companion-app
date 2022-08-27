using Home.Common.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HomeAutomationApp.Business
{
    public class DownloadBll : BaseBll
    {
        public async Task<List<DownloadItem>> GetDownloads(string state)
        {
            var url = $"/v1.0/downloads/{state}";
            return await DownloadData<List<DownloadItem>>(url);
        }

        public async Task<bool> QueueDownload(string downloadId)
        {
            var url = $"/v1.0/downloads/{downloadId}/queue";
            return await DownloadData<bool>(url);
        }

        public async Task<bool> AbandonDownload(string downloadId)
        {
            var url = $"/v1.0/downloads/{downloadId}/abandon";
            return await DownloadData<bool>(url);
        }

    }
}
