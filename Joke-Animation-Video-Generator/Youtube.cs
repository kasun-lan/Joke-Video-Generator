using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Joke_Animation_Video_Generator
{
    static class Youtube
    {
        public static YouTubeService youtubeService;
        private static string FileDataStorePart;
        private static string _jsonFilePath = "json/youtube.json";

        static Youtube()
        {
        }


        static string _videoId = "";

        public static string upload(string title, string _filePath, List<string> tags, bool isPrivate, bool isShort, bool addOutro)
        {
            var video = new Video();
            video.Snippet = new VideoSnippet();

            if (title.Length > 70)
            {
                title = title.Substring(0, 55);
                title += "....";
            }


            video.Snippet.Title = title;



            video.Snippet.Description = File.ReadAllText("description/description.txt");



            if (addOutro)
            {

                File.Copy("output/output.mp4","outro/output.mp4");
                Form1.ExecuteFFMpeg(@"-f concat -i outro/parts.txt -c copy outrooutput/output.mp4", false);


            }




            if (isShort)
            {

                string input = "";

                if (addOutro) 
                {
                    input = "outrooutput/output.mp4";
                }
                else
                {
                    input = "output/output.mp4";
                }


                video.Snippet.Description += " #Shorts";

                video.Snippet.Title += " #Shorts";

                Form1.ExecuteFFMpeg($"-loop 1 -i sw_back/image.png -i {input} -filter_complex \"[1:v]scale = 1080:-1[fg];[0:v][fg]overlay = (W - w) / 2:(H - h) / 2:shortest = 1\" sw_output/output.mp4", false);

                _filePath = "sw_output/output.mp4";
            }
            else
            {
                if(addOutro)
                {
                    _filePath = "outrooutput/output.mp4";
                }
            }




            video.Snippet.Tags = tags;
            video.Status = new VideoStatus();
            video.Status.PrivacyStatus = (isPrivate) ? "private" : "public"; // or "private" or "public"



            var filePath = _filePath; // Replace with path to actual movie file.



            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                var videosInsertRequest = Youtube.youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");

                videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
                videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;

                var res = videosInsertRequest.Upload();


                var AllImages = Directory.GetFiles("thumbnails").ToList();

                Random random = new Random();

                string selectedImage = AllImages[random.Next(0, AllImages.Count())].Split('\\').Last();

                var thumbnailSet = SetThumbnail(_videoId, $"thumbnails/{selectedImage}");

                _videoId = "";


                if (res.Status == UploadStatus.Completed && thumbnailSet)
                {
                    return "Uploading Successful! and Tumbnail set Successful";
                }
                else if (res.Status == UploadStatus.Completed)
                {
                    return "Uploading Successful! , Thumbnail set failed";
                }
                else
                {
                    return res.Status.ToString() + " \n " + res.Exception.Message;
                }

            }


        }

        public static bool SetThumbnail(string videoId, String url)
        {
            System.Net.WebClient theClient = new WebClient();
            using (var fileStream = theClient.OpenRead(url))
            {
                var videosInsertRequest = youtubeService.Thumbnails.Set(videoId, fileStream, "image/png");
                var res = videosInsertRequest.Upload();

                if (res.Status == UploadStatus.Completed)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        static void videosInsertRequest_ProgressChanged(Google.Apis.Upload.IUploadProgress progress)
        {
            switch (progress.Status)
            {
                case UploadStatus.Uploading:
                    Console.WriteLine("{0} bytes sent.", progress.BytesSent);
                    break;

                case UploadStatus.Failed:
                    Console.WriteLine("An error prevented the upload from completing.\n{0}", progress.Exception);
                    break;
            }
        }

        static void videosInsertRequest_ResponseReceived(Video video)
        {
            _videoId = video.Id;
        }



        public static async Task LoadYoutubeServices()
        {

            FileDataStorePart = File.ReadAllLines("refreshCredentials.txt")[0].ToString();


            // credentials
            UserCredential credential;
            using (var stream = new FileStream(_jsonFilePath, FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,

                    new[] { YouTubeService.Scope.YoutubeForceSsl },
                    "user",
                    CancellationToken.None,
                    new FileDataStore("AnimationVideos" + FileDataStorePart)
                );
            }

            youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "AnimationVideos",

            });
        }
    }
}
