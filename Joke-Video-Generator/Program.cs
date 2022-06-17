using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Google.Cloud.TextToSpeech.V1;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Joke_Video_Generator
{
    class Program
    {

        static void test1()
        {
            //List<Tuple<double, double, string, int, int>> data = new List<Tuple<double, double, string, int, int>>();

            //data.Add(Tuple.Create(1.5, 20.0, "first word", 10, 20));
            //data.Add(Tuple.Create(2.5, 20.0, "seound word", 20, 20));
            //data.Add(Tuple.Create(3.5, 20.0, "thrid word", 30, 20));
            //data.Add(Tuple.Create(4.5, 20.0, "fourth word", 40, 20));
            //data.Add(Tuple.Create(5.5, 20.0, "fifth word", 50, 20));
            //data.Add(Tuple.Create(6.5, 20.0, "sixth word", 60, 20));


            //string cmd = generateTextAnimationCommand("video.mp4", "video1.mp4", data);
            //ExecuteFFMpeg(cmd);


            //string command = $"-i video.mp4 -filter_complex " +
            //    $"\"drawtext=text='Summer Video1':fontfile=Arial.ttf:x=0:y=0:enable='between(t,2,20)',fade=t=in:start_time=2.0:d=0.0:alpha=1,fade=t=out:start_time=20.0:d=0.0:alpha=1 , " +
            //    $" drawtext=text='Summer Video2':fontfile=Arial.ttf:x=0:y=20:enable='between(t,10,20)',fade=t=in:start_time=10.0:d=0.0:alpha=1,fade=t=out:start_time=20.0:d=0.0:alpha=1 , " +
            //    $" drawtext=text='Summer Video3':fontfile=Arial.ttf:x=0:y=40:enable='between(t,16,20)',fade=t=in:start_time=16.0:d=0.0:alpha=1,fade=t=out:start_time=20.0:d=0.0:alpha=1\" " +
            //    $" -c:a copy video1.mp4";

            //ExecuteFFMpeg(command);


            using (Context context = new Context())
            {
                var text = context.Jokes.Where(j => j.id == "32w6d2").FirstOrDefault().body;
                var c = GetTextAnimationData(text, 15, 20);

                //    ApplyTextAnimationOnVideo(c, 120, 1, "");


                DataTable dataTable = new DataTable();

                //for (int x = 0; x < 30; x++)
                //{
                //    dataTable.Columns.Add(new DataColumn(x.ToString()));
                //}

                //for (int y = 0; y < 10; y++)
                //{
                //    dataTable.Rows.Add();

                //    for (int x = 0; x < 30; x++)
                //    {

                //        dataTable.Rows[y][x] = "";
                //    }
                //}

                //foreach (var val in c[0].keyValuePairs)
                //{
                //    dataTable.Rows[int.Parse(val.Key.Split(' ').First())][int.Parse(val.Key.Split(' ').Last())] = val.Value;

                //}

                //var sdfd = 0;

                //foreach (var val in c[1].keyValuePairs)
                //{
                //    dataTable.Rows[int.Parse(val.Key.Split(' ').First())][int.Parse(val.Key.Split(' ').Last())] = val.Value;

                //}

                var sfesr = 0;

            }




            // string command = $"-i \"video.mp4\" -vf drawtext=\"fontfile=Arial.ttf:text=hello:x=mod(t*100\\,w):y=(x*h)/w\" \"video1.mp4\" ";
            // string command = File.ReadAllText("temp.txt");
            //string command = "-i video.mp4 -filter_complex \"drawtext=text='Summer Video':fontfile=Arial.ttf:x=0:y=0:enable='between(t,2,6)',fade=t=in:start_time=2.0:d=0.5:alpha=1,fade=t=out:start_time=5.5:d=0.5:alpha=1\" -c:a copy video1.mp4";
            //ExecuteFFMpeg(command);

            //[fg];[0][fg]overlay=format=(auto),format=(yuv420p)

            // Json();
            //Json1();

            //SynthesizeText("we are testing this", "texttts.mp3");

            var story = File.ReadAllText("story1.txt");

            var temo = 0;

            var xx = story.Split('\n');

            var sdfsdfds = 0;
        }



        static void Main(string[] args)
        {



            //  ExecuteFFMpeg("-i video1.mp4 -filter_complex \"drawtext=text='...and Mr. Holmes '\\\\\\''turned'\\\\\\'' to his assistant\\: Tell me, Watson, what do you see?':fontfile=Arial.ttf:x=0:y=0:enable='between(t,2,6)',fade=t=in:start_time=2.0:d=0.5:alpha=1,fade=t=out:start_time=5.5:d=0.5:alpha=1\" -c:a copy video2.mp4", true);


            using ( context = new Context())
            {
               




            }


        }


        //phase <=> line
        //bool videoPart(string line, int partIndex)
        //{
        //    SynthesizeText(line,$"tts/{partIndex}.mp3");
        //}






        // [START tts_synthesize_text]
        /// <summary>
        /// Creates an audio file from the text input.
        /// </summary>
        /// <param name="text">Text to synthesize into audio</param>
        /// <remarks>
        /// Generates a file named 'output.mp3' in project folder.
        /// </remarks>
        public static void SynthesizeText(string text, string filename)
        {

            TextToSpeechClientBuilder textToSpeechClientBuilder = new TextToSpeechClientBuilder();
            textToSpeechClientBuilder.CredentialsPath = "tts5.json";
            TextToSpeechClient client = textToSpeechClientBuilder.Build();


            var response = client.SynthesizeSpeech(new SynthesizeSpeechRequest
            {
                Input = new SynthesisInput
                {
                    Text = text
                },
                // Note: voices can also be specified by name
                Voice = new VoiceSelectionParams
                {
                    LanguageCode = "en-US",
                    SsmlGender = SsmlVoiceGender.Male,

                },
                AudioConfig = new AudioConfig
                {
                    AudioEncoding = AudioEncoding.Mp3,
                    SpeakingRate = 0.75
                }
            });

            using (Stream output = File.Create(filename))
            {
                response.AudioContent.WriteTo(output);
            }
        }


        public static int ExecuteFFMpeg(string arguments, bool usingCMD = false)
        {
            try
            {
                if (usingCMD)
                {
                    Process process = new Process();
                    process.StartInfo.UseShellExecute = true;
                    process = Process.Start("cmd.exe", $@"/k ffmpeg.exe {arguments}");
                    process.WaitForExit();
                    return 123123;
                }

                Process ffmpegProcess = new Process();
                ffmpegProcess.StartInfo.FileName = "ffmpeg.exe";
                ffmpegProcess.StartInfo.CreateNoWindow = true;
                ffmpegProcess.StartInfo.Arguments = arguments;
                ffmpegProcess.Start();
                ffmpegProcess.WaitForExit();
                int x = 0;
                return ffmpegProcess.ExitCode;

            }
            catch (Exception e)
            {
                return -1;
            }

        }

        //starttime,endtime,word,positionx,positiony
        static string generateTextAnimationCommand(string sourceVideoPath, string destinationVideoPath, List<Tuple<double, double, string, int, int>> data, int fontSize, string fontColor)
        {




            string startingPart = $"-i {sourceVideoPath} -filter_complex \"";
            string endingPart = $"\" -c:a copy {destinationVideoPath} ";

            string middlePart = "";

            int counter = 0;

            foreach (var tuple in data)
            {
                if (counter > 40) { break; }
                middlePart += $" drawtext=text='{tuple.Item3}':fontfile=Arial.ttf:x={tuple.Item4}:y={tuple.Item5}:fontcolor={fontColor}:shadowy=2:shadowcolor=white:fontsize={20}:enable='between(t,{tuple.Item1.ToString()},{tuple.Item2.ToString()})',fade=t=in:start_time={tuple.Item1.ToString()}:d=0.0:alpha=1,fade=t=out:start_time={tuple.Item2.ToString()}:d=0.0:alpha=1 , ";
                counter++;
            }
            middlePart = middlePart.Trim();
            middlePart = middlePart.Remove(middlePart.Length - 1);
            middlePart = middlePart.Trim();

            string command = startingPart + middlePart + endingPart;

            return command;
        }


        /// <summary>
        /// Note : List<Tuple<section_id,line_id,word,line_index,linePosition>>
        /// </summary>
        /// <param name="text"></param>
        /// <param name="canvasHeight"></param>
        /// <param name="canvasWidth"></param>
        /// <returns></returns>
        public static List<Tuple<int, int, string, int, int>> GetTextAnimationData(string text, int canvasHeight, int canvasWidth, int? noOfwordsPerLine = null)
        {
            List<string> lines = new List<string>();

            if (lines == null)
            {
                lines = text.Split('\n').ToList();
            }
            else
            {
                Queue<string> queue = new Queue<string>(text.Split(' ').ToList());

                while (queue.Count() > 0)
                {
                    string line = "";

                    if (queue.Count() >= noOfwordsPerLine)
                    {
                        for (int x = 0; x < noOfwordsPerLine; x++)
                        {
                            line += queue.Dequeue();
                            line += " ";
                        }
                    }
                    else
                    {
                        while (queue.Count() > 0)
                        {
                            line += queue.Dequeue();
                            line += " ";
                        }
                    }

                    lines.Add(line);
                }
            }
            lines = lines.Select(l => l.Trim()).ToList();
            lines.RemoveAll(l => string.IsNullOrEmpty(l));

            int canvasLineIndex = 0;
            int canvasLineUsage = 0;
            int currenSectionId = 0;
            //sectionid , line id , word , line index , lineusage 
            List<Tuple<int, int, string, int, int>> tuples = new List<Tuple<int, int, string, int, int>>();
            int i = 0;

            while (i < lines.Count)
            {
                if (NoEnoughSpaceInCanvas(lines[i], canvasWidth, canvasHeight, canvasLineIndex, canvasLineUsage))
                {
                    currenSectionId++;
                    canvasLineIndex = 0;
                    canvasLineUsage = 0;
                }

                List<string> words = lines[i].Split(' ').ToList();

                foreach (string word in words)
                {
                    if (NoEnoughSpaceInCurrentCanasLine(word, canvasLineUsage, canvasWidth))
                    {
                        canvasLineIndex++;
                        canvasLineUsage = 0;
                    }

                    tuples.Add(Tuple.Create(currenSectionId, i, word, canvasLineIndex, canvasLineUsage));
                    canvasLineUsage = canvasLineUsage + word.Length + 1;
                }

                if (canvasLineIndex > (canvasHeight - 1))
                {
                    //
                    currenSectionId++;
                    canvasLineIndex = 0;
                    canvasLineUsage = 0;


                    tuples.RemoveAll(t => t.Item2 == i);

                }
                else
                {
                    i++;
                }
            }




            return tuples;

        }

        //sectionid,lineid,word,line index , position in line
        public static void ApplyTextAnimationOnVideo(List<Tuple<int, int, string, int, int>> tds, double videoPlayTime, int xOff, int yOff, int char_lenght, int line_height, int fontSize, string fontColor)
        {
            deleteAllRelatedVideos();
            copyVideo();


            var sIds = tds.Select(t => t.Item1).Distinct().ToList();
            double timeBetweenWords = videoPlayTime / tds.Count();
            int workCount = 0;
            double intervelBetweenWords = videoPlayTime / tds.Count();

            Queue<Tuple<double, double, string, int, int>> data = new Queue<Tuple<double, double, string, int, int>>();

            int cumilated_wordcount = 0;


            foreach (var sId in sIds)
            {
                var related_tds = tds.Where(t => t.Item1 == sId).ToList();
                double endTimeForTheSection = timeBetweenWords * (cumilated_wordcount + related_tds.Count());

                foreach (var td in related_tds)
                {
                    double startTime = workCount * timeBetweenWords;
                    int positionX = xOff + (char_lenght * td.Item5);
                    int positionY = yOff + (line_height * td.Item4);

                    startTime = Math.Round(startTime, 2, MidpointRounding.AwayFromZero);


                    data.Enqueue(Tuple.Create(startTime, endTimeForTheSection, td.Item3, positionX, positionY));

                    //AddWordsFfmpeg(vp, startTime, endTimeForTheSection, td.Item3, positionX, positionY);

                    workCount++;
                }

                cumilated_wordcount += related_tds.Count();

            }

            int sourceVideoPostFix = 1;
            int wordAmountForOneConvertion = 40;
            bool breakParentLoop = false;


            while (0 < data.Count())
            {
                List<Tuple<double, double, string, int, int>> dataForConvertion = new List<Tuple<double, double, string, int, int>>();

                for (int x = 0; x < 20; x++)
                {
                    if (data.Count() == 0) break;
                    dataForConvertion.Add(data.Dequeue());

                }



                var command = generateTextAnimationCommand($"TextAdded/video{sourceVideoPostFix}.mp4", $"TextAdded/video{sourceVideoPostFix + 1}.mp4", dataForConvertion, fontSize, fontColor);
                int res = ExecuteFFMpeg(command);
                sourceVideoPostFix++;
            }


            var sdfsrer = 0;


        }

        private static void copyVideo()
        {
            File.Copy("VoiceAndChrAdded/video1.mp4", "TextAdded/video1.mp4");
        }

        private static void deleteAllRelatedVideos()
        {
            ClearFolder("TextAdded");
        }


        private static void ClearFolder(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);

            foreach (FileInfo fi in dir.GetFiles())
            {
                try
                {
                    fi.Delete();
                }
                catch (Exception) { } // Ignore all exceptions
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                ClearFolder(di.FullName);
                try
                {
                    di.Delete();
                }
                catch (Exception) { } // Ignore all exceptions
            }
        }

        private static void AddWordFfmpeg(string vp, double startTime, double endTimeForTheSection, string word, int positionX, int positionY)
        {

            string command = $"-i video.mp4 -filter_complex \"drawtext=text='Summer Video':fontfile=Arial.ttf:x=0:y=0:enable='between(t,2,6)',fade=t=in:start_time=2.0:d=0.5:alpha=1,fade=t=out:start_time=5.5:d=0.5:alpha=1\" -c:a copy video1.mp4";


            ExecuteFFMpeg(command);
        }

        private static bool NoEnoughSpaceInCurrentCanasLine(string word, int canvasLineUsage, int canvasWidth)
        {
            if ((canvasWidth - canvasLineUsage) < word.Length)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool NoEnoughSpaceInCanvas(string line, int canvasWidth, int canvasHeight, int canvasLineIndex, int canvasLineUsage)
        {
            int spacesInCurrentLine = canvasWidth - canvasLineUsage;
            int OtherLines = canvasHeight - canvasLineIndex - 1;
            int spacesInFullLines = OtherLines * canvasWidth;
            int allSpacesLeft = spacesInFullLines + spacesInCurrentLine;

            if (allSpacesLeft < line.Length)
            {
                return true;
            }
            else
            {
                return false;
            }
        }




        public static void DrawTexts(Joke joke, int writing_speed)
        {
            ///temp local variables there should be parameters
            int x = 0;
            int y = 0;
            int width = 500;
            int height = 500;




            List<string> lines = joke.body.Split('\n').ToList();
            int lineNo = 0;
            int lineWritePos = 0;


            foreach (var line in lines)
            {
                if (dont_have_enough_space_in_text_panel(line))
                {
                    //erase all text

                    lineNo = 0;
                }
                else
                {
                    lineNo++;
                }

                List<string> words = line.Split(" ").ToList();

                foreach (var word in words) // here word can mean something like "done." too
                {
                    if (dont_have_enough_space_in_current_line(word))
                    {
                        lineNo++;
                        lineWritePos = 0;
                    }

                    //write to the line


                    lineWritePos += word.Length;

                }
            }


            //need to display text at specifc place at specific time span.
            void write_word_to_video(string text, int x, int y, int start_secount, int end_secound)
            {

            }

        }





        private static bool dont_have_enough_space_in_current_line(string word)
        {
            throw new NotImplementedException();
        }



        private static bool dont_have_enough_space_in_text_panel(string line)
        {
            throw new NotImplementedException();
        }
    }


    public class Context : DbContext
    {
        public DbSet<Joke> Jokes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=JokesDb.db;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Joke>().ToTable("Joke");
        }
    }

    public class Section
    {
        public string Text { get; set; }
        public string AudioFilePath { get; set; }
        public int DelayBetweenWords { get; set; }
        public Dictionary<string, string> keyValuePairs { get; set; }

    }

    public class Joke
    {
        public string body { get; set; }
        public string id { get; set; }
        public int score { get; set; }
        public string title { get; set; }
        public int no_of_charachter { get; set; }

    }
}
