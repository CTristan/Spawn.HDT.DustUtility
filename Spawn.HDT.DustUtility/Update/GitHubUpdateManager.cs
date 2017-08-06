using System;
using System.Net;
using System.Threading.Tasks;

namespace Spawn.HDT.DustUtility.Update
{
    public static class GitHubUpdateManager
    {
        private const string LatestReleaseUrl = "https://github.com/CLJunge/Spawn.HDT.DustUtility/releases/latest";

        public static async Task<bool> CheckForUpdate()
        {
            bool blnRet = false;

            HttpWebRequest request = WebRequest.CreateHttp(LatestReleaseUrl);

            HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;

            if (response.StatusCode == HttpStatusCode.OK)
            {

            }
            else { }

            return blnRet;
        }

        public static void DownloadLatestRelease()
        {
            throw new NotImplementedException();
        }
    }
}
