using System;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spawn.HDT.DustUtility.Update
{
    public static class GitHubUpdateManager
    {
        #region Constants
        public const string LatestReleaseUrl = "https://github.com/CLJunge/Spawn.HDT.DustUtility/releases/latest";
        #endregion

        #region Static Member Variables
        private static Regex s_versionRegex;
        private static Version s_newVersion;
        #endregion

        #region Properties
        public static Version NewVersion => s_newVersion;
        #endregion

        #region Ctor
        static GitHubUpdateManager()
        {
            s_versionRegex = new Regex("[0-9]\\.[0-9]{1,2}");
        }
        #endregion

        #region CheckForUpdateAsync
        public static async Task<bool> CheckForUpdateAsync()
        {
            bool blnRet = false;

            HttpWebRequest request = WebRequest.CreateHttp(LatestReleaseUrl);

            HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Match versionMatch = s_versionRegex.Match(response.ResponseUri.AbsoluteUri);

                if (versionMatch.Success)
                {
                    Version newVersion = new Version(versionMatch.Value);

                    blnRet = newVersion > Assembly.GetExecutingAssembly().GetName().Version;
                    //blnRet = newVersion > new Version(0,0);

                    if (blnRet)
                    {
                        s_newVersion = newVersion;
                    }
                    else { }
                }
                else { }
            }
            else { }

            return blnRet;
        } 
        #endregion
    }
}
