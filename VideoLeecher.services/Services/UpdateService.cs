﻿using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Net;
using System.Text;
using VideoLeecher.core.Models;
using VideoLeecher.services.Interfaces;
using VideoLeecher.shared.Extensions;
using VideoLeecher.shared.Reflection;

namespace VideoLeecher.services.Services
{
    internal class UpdateService :  IUpdateService
    {
        #region  კონსტანტები

        private const string latestReleaseUrl = "https://github.com/Franiac/TwitchLeecher/releases/tag/v{0}";
        private const string releasesApiUrl = "https://api.github.com/repos/Franiac/TwitchLeecher/releases";

        #endregion კონსტანტები

        #region მეთოდები

        public  UpdateInfo CheckForUpdate()
        {
            try
            {
                using (WebClient webClient = new WebClient() { Encoding = Encoding.UTF8 })
                {
                    webClient.Headers.Add(HttpRequestHeader.UserAgent, "TwitchLeecher");

                    string result = webClient.DownloadString(releasesApiUrl);

                    JToken releasesJson = JToken.Parse(result);

                    foreach(JToken  releaseJson in releasesJson)
                    {
                        bool draft = releaseJson.Value<bool>("draft");
                        bool prerelease = releaseJson.Value<bool>("prerelease");

                        if (!draft  &&  !prerelease)
                        {
                            string tagStr = releaseJson.Value<string>("tag_name");
                            string releasedStr = releaseJson.Value<string>("published_at");
                            string infoStr = releaseJson.Value<string>("body");

                            Version releaseVersion = Version.Parse(tagStr.Substring(1));

                            Version localVersion = AssemblyUtils.Get.GetAssemblyVersion();

                            DateTime released = DateTime.Parse(releasedStr,  CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

                            string downloadUrl = string.Format(latestReleaseUrl,  releaseVersion.Trim().ToString());

                            if (releaseVersion > localVersion)
                            {
                                return new UpdateInfo(releaseVersion, released, downloadUrl, infoStr);
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //  Update  check should not  distract  the  application.
            }

            return null;
        }
        
        #endregion  მეთოდები
    }
}
