﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Funbit.Ets.Telemetry.Server.Controllers
{
    [RoutePrefix("")]
    public class Ets2AppController : StaticFileController
    {
        static readonly log4net.ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public const string TelemetryAppUriPath = "/";

        [HttpGet]
        [Route("config.json", Name = "GetSkinConfig")]
        public HttpResponseMessage GetSkinConfig()
        {
            var skinDirs = EnumerateDirectories("skins");
            var skins = new JArray();
            foreach (var skinDir in skinDirs)
            {
                var configJsonPath = Path.Combine(skinDir, "config.json");
                if (File.Exists(configJsonPath))
                {
                    try
                    {
                        var skinConfigRoot = (JObject)JsonConvert.DeserializeObject(
                            File.ReadAllText(configJsonPath, Encoding.UTF8));
                        var skinConfig = skinConfigRoot["config"];
                        skins.Add(skinConfig);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                }
            }
            var config = new { skins };
            return Request.CreateResponse(HttpStatusCode.OK, config, new JsonMediaTypeFormatter());
        }

        [HttpGet]
        [Route("", Name = "GetRoot")]
        public HttpResponseMessage GetRoot()
        {
            return ServeStaticFile("", "index.html");
        }

        [HttpGet]
        [Route("{fileName:regex(^(?!.*api))}", Name = "GetRootFile")]
        public HttpResponseMessage GetRootFile(
            string fileName)
        {
            return ServeStaticFile("", fileName);
        }

        // we support up to 5 directory levels with the root directory not containing "api" prefix

        [HttpGet]
        [Route("{dirName1:regex(^(?!.*api))}/{fileName}", Name = "GetResourceFile1")]
        public HttpResponseMessage GetResourceFile1(
            string dirName1, string fileName)
        {
            return ServeStaticFile(dirName1, fileName);
        }

        [HttpGet]
        [Route("{dirName1:regex(^(?!.*api))}/{dirName2}/{fileName}", Name = "GetResourceFile2")]
        public HttpResponseMessage GetResourceFile2(
            string dirName1, string dirName2, string fileName)
        {
            return ServeStaticFile(dirName1 + "/" + dirName2, fileName);
        }

        [HttpGet]
        [Route("{dirName1:regex(^(?!.*api))}/{dirName2}/{dirName3}/{fileName}", Name = "GetResourceFile3")]
        public HttpResponseMessage GetResourceFile3(
            string dirName1, string dirName2, string dirName3, string fileName)
        {
            return ServeStaticFile(dirName1 + "/" + dirName2 + "/" + dirName3, fileName);
        }

        [HttpGet]
        [Route("{dirName1:regex(^(?!.*api))}/{dirName2}/{dirName3}/{dirName4}/{fileName}", Name = "GetResourceFile4")]
        public HttpResponseMessage GetResourceFile4(
            string dirName1, string dirName2, string dirName3, string dirName4, string fileName)
        {
            return ServeStaticFile(dirName1 + "/" + dirName2 + "/" + dirName3 + "/" + dirName4, fileName);
        }

        [HttpGet]
        [Route("{dirName1:regex(^(?!.*api))}/{dirName2}/{dirName3}/{dirName4}/{dirName5}/{fileName}", Name = "GetResourceFile5")]
        public HttpResponseMessage GetResourceFile5(
            string dirName1, string dirName2, string dirName3, string dirName4, string dirName5, string fileName)
        {
            return ServeStaticFile(dirName1 + "/" + dirName2 + "/" + dirName3 + "/" + dirName4 + "/" + dirName5, fileName);
        }
    }
}