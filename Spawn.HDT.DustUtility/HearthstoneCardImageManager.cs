using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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

        #region Static Member Variables
        private static Dictionary<string, Stream> s_dDefaultCache;
        private static Dictionary<string, Stream> s_dPremiumCache;

        private static Dictionary<string, Stream> DefaultCache => s_dDefaultCache ?? (s_dDefaultCache = new Dictionary<string, Stream>());
        private static Dictionary<string, Stream> PremiumCache => s_dPremiumCache ?? (s_dPremiumCache = new Dictionary<string, Stream>());
        #endregion

        #region GetStreamAsync
        public static async Task<Stream> GetStreamAsync(string strCardId, bool blnPremium)
        {
            Stream retVal = null;

            if (!string.IsNullOrEmpty(strCardId))
            {
                if (blnPremium && PremiumCache.ContainsKey(strCardId))
                {
                    retVal = PremiumCache[strCardId];
                }
                else if (!blnPremium && DefaultCache.ContainsKey(strCardId))
                {
                    retVal = DefaultCache[strCardId];
                }
                else { }

                if (retVal == null)
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

                                        if (Settings.UseImageCache)
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

                    if (retVal != null)
                    {
                        if (blnPremium)
                        {
                            PremiumCache[strCardId] = retVal;
                        }
                        else
                        {
                            DefaultCache[strCardId] = retVal;
                        }
                    }
                    else { }
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

        #region ClearMemoryCache
        public static void ClearMemoryCache()
        {
            foreach (var item in DefaultCache)
            {
                item.Value.Dispose();
            }

            DefaultCache.Clear();

            foreach (var item in PremiumCache)
            {
                item.Value.Dispose();
            }

            PremiumCache.Clear();
        }
        #endregion

        #region ClearLocalCache
        public static void ClearLocalCache()
        {
            ClearMemoryCache();

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

        #region Dispose
        public static void Dispose()
        {
            if (s_dDefaultCache != null)
            {
                foreach (var item in s_dDefaultCache)
                {
                    item.Value.Dispose();
                }
            }
            else { }

            s_dDefaultCache?.Clear();
            s_dDefaultCache = null;

            if (s_dPremiumCache != null)
            {
                foreach (var item in s_dPremiumCache)
                {
                    item.Value.Dispose();
                }
            }
            else { }

            s_dPremiumCache?.Clear();
            s_dPremiumCache = null;
        }
        #endregion
    }
}