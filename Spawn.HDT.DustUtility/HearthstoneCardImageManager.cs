using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Spawn.HDT.DustUtility
{
    public static class HearthstoneCardImageManager
    {
        #region Constants
        private const string BaseUrl = "https://omgvamp-hearthstone-v1.p.mashape.com";
        private const string ApiKey = "T63EJR1RqumshjNsE8mLzycYVpVIp1PIHqLjsnTaibC4T4grpP";
        private const string CacheFolderName = "image_cache";
        #endregion

        #region Static Properties
        public static int MaxCacheCount = 30;
        #endregion

        #region GetStreamAsync
        public static async Task<Stream> GetStreamAsync(string strCardId, bool blnPremium)
        {
            Stream retVal = null;

            if (!string.IsNullOrEmpty(strCardId))
            {
                string strPath = GetLocalCachePath(strCardId, blnPremium);

                if (File.Exists(strPath))
                {
                    try
                    {
                        retVal = File.Open(strPath, FileMode.Open);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }
                }
                else { }

                if (retVal == null)
                {
                    try
                    {
                        HttpWebRequest request = CreateCardDataRequest(strCardId);

                        HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            string strJson;

                            using (Stream responseStream = response.GetResponseStream())
                            {
                                using (StreamReader reader = new StreamReader(responseStream))
                                {
                                    strJson = await reader.ReadToEndAsync();
                                }
                            }

                            if (!string.IsNullOrEmpty(strJson))
                            {
                                var cardData = JsonConvert.DeserializeObject<JArray>(strJson)[0];

                                string strUrl = cardData.Value<string>("img");

                                if (blnPremium)
                                {
                                    strUrl = cardData.Value<string>("imgGold");
                                }
                                else { }

                                HttpWebRequest imageRequest = CreateImageRequest(strUrl);

                                HttpWebResponse imageResponse = await imageRequest.GetResponseAsync() as HttpWebResponse;

                                if (response.StatusCode == HttpStatusCode.OK)
                                {
                                    using (Stream responseStream = imageResponse.GetResponseStream())
                                    {
                                        retVal = new MemoryStream();

                                        await responseStream.CopyToAsync(retVal);
                                    }

                                    retVal.Position = 0;

                                    if (Settings.LocalImageCache)
                                    {
                                        using (FileStream fs = File.Open(strPath, FileMode.Create))
                                        {
                                            await retVal.CopyToAsync(fs);
                                        }

                                        retVal.Position = 0;
                                    }
                                    else { }
                                }
                                else { }
                            }
                            else { }
                        }
                        else { }
                    }
                    catch
                    {
                        //No internet connection
                    }
                }
                else { }
            }
            else { }

            return retVal;
        }
        #endregion

        #region GetBitmapAsync
        public static async Task<Bitmap> GetBitmapAsync(string strCardId, bool blnPremium)
        {
            Stream imageStream = await GetStreamAsync(strCardId, blnPremium);

            return Image.FromStream(imageStream) as Bitmap;
        }
        #endregion

        #region Requests
        #region Json Data
        #region CreateCardDataRequest
        private static HttpWebRequest CreateCardDataRequest(string strCardId)
        {
            return CreateJsonDataRequest($"/cards/{strCardId}");
        }
        #endregion

        #region CreateCardBackImageDataRequest
        private static HttpWebRequest CreateCardBackImageDataRequest()
        {
            return CreateJsonDataRequest("/cardbacks");
        }
        #endregion

        #region CreateJsonDataRequest
        private static HttpWebRequest CreateJsonDataRequest(string strUrl)
        {
            HttpWebRequest retVal = WebRequest.CreateHttp($"{BaseUrl}{strUrl}");

            retVal.Accept = "application/json";

            retVal.Headers.Add("X-Mashape-Key", ApiKey);

            return retVal;
        }
        #endregion
        #endregion

        #region CreateImageRequest
        private static HttpWebRequest CreateImageRequest(string strUrl)
        {
            HttpWebRequest retVal = WebRequest.CreateHttp(strUrl);

            string strExtension = Path.GetExtension(strUrl).Remove(0, 1);

            retVal.Accept = $"image/{strExtension}";

            return retVal;
        }
        #endregion
        #endregion

        #region ClearLocalCache
        public static void ClearLocalCache()
        {
            Directory.Delete(Path.Combine(DustUtilityPlugin.DataDirectory, CacheFolderName), true);
        }
        #endregion

        #region GetLocalCachePath
        private static string GetLocalCachePath(string strCardId, bool blnPremium)
        {
            string strRet = Path.Combine(DustUtilityPlugin.DataDirectory, CacheFolderName);

            if (blnPremium)
            {
                strRet = Path.Combine(strRet, "premium");
            }
            else
            {
                strRet = Path.Combine(strRet, "default");
            }

            if (!Directory.Exists(strRet))
            {
                Directory.CreateDirectory(strRet);
            }
            else { }

            if (blnPremium)
            {
                strRet = Path.Combine(strRet, $"{strCardId}.gif");
            }
            else
            {
                strRet = Path.Combine(strRet, $"{strCardId}.png");
            }

            return strRet;
        }
        #endregion
    }
}