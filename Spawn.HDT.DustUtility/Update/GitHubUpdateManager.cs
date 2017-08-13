using System;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hearthstone_Deck_Tracker.Utility.Logging;

namespace Spawn.HDT.DustUtility.Update
{
    public static class GitHubUpdateManager
    {
        #region Constants
        public const string LatestReleaseUrl = "https://github.com/CLJunge/Spawn.HDT.DustUtility/releases/latest";
        #endregion

        #region Static Member Variables
        private static Regex s_versionRegex;
        private static Regex s_updateTextRegex;

        private static Version s_newVersion;
        private static string s_strReleaseNotes;
        #endregion

        #region Properties
        public static Version NewVersion => s_newVersion;
        public static string ReleaseNotes => s_strReleaseNotes;
        #endregion

        #region Ctor
        static GitHubUpdateManager()
        {
            s_versionRegex = new Regex("[0-9]\\.[0-9]{1,2}\\.?[0-9]{0,2}");
            s_updateTextRegex = new Regex("<div class=\"markdown-body\">          <p>(?<Content>.*)</p>        </div>");
        }
        #endregion

        #region CheckForUpdateAsync
        public static async Task<bool> CheckForUpdateAsync()
        {
            bool blnRet = false;

            try
            {
                Log.WriteLine("Checking for updates...", LogType.Info);

                HttpWebRequest request = WebRequest.CreateHttp(LatestReleaseUrl);

                HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Match versionMatch = s_versionRegex.Match(response.ResponseUri.AbsoluteUri);

                    if (versionMatch.Success)
                    {
                        Version newVersion = new Version(versionMatch.Value);

                        blnRet = newVersion > Assembly.GetExecutingAssembly().GetName().Version;
                        //blnRet = newVersion > new Version(0, 0);

                        if (blnRet)
                        {
                            s_newVersion = newVersion;

                            string strResult;

                            using (WebClient webClient = new WebClient())
                            {
                                strResult = await webClient.DownloadStringTaskAsync(response.ResponseUri);

                                strResult = strResult.Trim().Replace("\n", string.Empty).Replace("\r", string.Empty);
                            }

                            Match updateTextMatch = s_updateTextRegex.Match(strResult);

                            if (updateTextMatch.Success)
                            {
                                s_strReleaseNotes = updateTextMatch.Groups["Content"].Value.Replace("<br>", Environment.NewLine);
                            }
                            else { }

                            Log.WriteLine("New release available", LogType.Info);
                        }
                        else
                        {
                            Log.WriteLine("No update available", LogType.Info);
                        }
                    }
                    else { }
                }
                else { }
            }
            catch (Exception ex)
            {
                //No internet connection or github down
                Log.WriteLine($"Couldn't perform update check: {ex}", LogType.Error);
            }

            return blnRet;
        }
        #endregion
    }
}
