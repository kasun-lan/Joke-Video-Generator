using Google.Cloud.TextToSpeech.V1;
using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech;
using System.Speech.Recognition;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System.Threading;
using Google.Apis.YouTube.v3.Data;
using System.Drawing.Imaging;
using System.Web;

namespace Joke_Animation_Video_Generator
{
    public partial class Form1 : Form
    {
        const int _minimumNoOfLinesForStory = 4;
        CancellationTokenSource _cts = null;
        List<DateTime> _uploadTimes = null;
        private static string _tts_json_link = "json/tts.json";
        Bitmap _bitmapNewBackgroundImage = null;

        public Form1()
        {
            InitializeComponent();
            InitializeTimePickers();

        }
        private DateTimePicker timePicker1;
        private DateTimePicker timePicker2;
        private DateTimePicker timePicker3;
        private DateTimePicker timePicker4;

        private void InitializeTimePickers()
        {
            timePicker1 = new DateTimePicker();
            timePicker1.Format = DateTimePickerFormat.Time;
            timePicker1.ShowUpDown = true;
            timePicker1.Location = new Point(0, 0);
            timePicker1.Width = 150;
            panel1.Controls.Add(timePicker1);

            timePicker2 = new DateTimePicker();
            timePicker2.Format = DateTimePickerFormat.Time;
            timePicker2.ShowUpDown = true;
            timePicker2.Location = new Point(0, 0);
            timePicker2.Width = 150;
            panel2.Controls.Add(timePicker2);

            timePicker3 = new DateTimePicker();
            timePicker3.Format = DateTimePickerFormat.Time;
            timePicker3.ShowUpDown = true;
            timePicker3.Location = new Point(0, 0);
            timePicker3.Width = 150;
            panel3.Controls.Add(timePicker3);

            timePicker4 = new DateTimePicker();
            timePicker4.Format = DateTimePickerFormat.Time;
            timePicker4.ShowUpDown = true;
            timePicker4.Location = new Point(0, 0);
            timePicker4.Width = 150;
            panel4.Controls.Add(timePicker4);




        }





        void testdb()
        {
            using (Context context = new Context())
            {
                var list = context.Jokes.ToList();
            }
        }



        private void addOverlay(Image imageBackground, string overlayPath, string resultSavPath, int posX, int posY, double scale)
        {
            Image imageOverlay = Image.FromFile(overlayPath);
            Image scaled = new Bitmap(imageOverlay, (int)(imageOverlay.Width * scale), (int)(imageOverlay.Height * scale));

            Image img = new Bitmap(imageBackground.Width, imageBackground.Height);
            using (Graphics gr = Graphics.FromImage(img))
            {
                gr.DrawImage(imageBackground, new Point(0, 0));
                gr.DrawImage(scaled, new Point(posX, posY));
            }
            img.Save(resultSavPath, ImageFormat.Png);
        }


        private async void Form1_Load(object sender, EventArgs e)
        {
            //temp

           /// ExecuteFFMpeg(@"-f concat -i temp/videofiles.txt -c copy temp/contcated.mp4", false);

            //ExecuteFFMpeg("-i temp/output.mp4 -c copy -bsf h264_mp4toannexb  temp/vid1.ts", true);
            //ExecuteFFMpeg("-i temp/outro2.mp4 -c copy -bsf h264_mp4toannexb  temp/vid2.ts", true);
            //ExecuteFFMpeg("-i \"concat: vid1.ts | vid2.ts\" -c copy temp/concated.mp4", true);

           //  ExecuteFFMpeg("-i temp/output.mp4 -i temp/outro2.mp4 - filter_complex \"[0:v] [0:a] [1:v] [1:a] [2:v] [2:a] concat = n = 3:v = 1:a = 1[v][a]\" - map \"[v]\" - map \"[a]\" temp/output.mp4", true);

         ///   NReco.VideoInfo.FFProbe instance = new NReco.VideoInfo.FFProbe();

            int sdfjsldkfjsldf = 0;


            //



            await Youtube.LoadYoutubeServices();

            button2.Enabled = false;

            //   UploadVideo("output/output.mp4", "youtube titles", false);


            //set datagridview
            using (Context context = new Context())
            {
                dataGridView1.DataSource = context.Jokes.ToList();

            }


            //display current background image


            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            Image image = GetCopyImage("textfiles\\animation_images\\1.png");
            pictureBox1.Image = image;


            dataGridView1.Columns[5].ReadOnly = true;
        }

        private Image GetCopyImage(string path)
        {
            using (Image image = Image.FromFile(path))
            {
                Bitmap bitmap = new Bitmap(image);
                return bitmap;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="story">there should be many prerequisits.</param>
        /// <returns></returns>
        void CreateAJokeVideo(List<string> storyLines)
        {
            if (storyLines.Count() < _minimumNoOfLinesForStory)
            {
                // throw new Exception("storyLines.Count() < _minimumNoOfLinesForStory");
            }

            ClearFilesInFolder("parts");
            ClearFilesInFolder("output");
            ClearFilesInFolder("sw_output");
            File.Delete("outro/output.mp4");
            File.Delete("outrooutput/output.mp4");



            int partIndex = 0;



            //generate video parts
            foreach (var line in storyLines)
            {
                videoPart(line, partIndex);
                richTextBox1.Invoke(new Action(() => richTextBox1.Text += $"part{partIndex} done. \n"));
                partIndex++;
            }
            //පාර්ට්ස් textfile එක හදනවා
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < storyLines.Count(); i++)
            {
                stringBuilder.AppendLine($"file 'part{i}.mp4'");
            }

            File.WriteAllText("parts/parts.txt", stringBuilder.ToString());


            int sdfsdof = 0;


            //parts ටික එකතු කරලා complete video එක හදනවා
            ExecuteFFMpeg(@"-f concat -safe 0 -i parts/parts.txt -c copy output/output.mp4", false);
            richTextBox1.Invoke(new Action(() => richTextBox1.Text += $"Output done. \n"));

        }


        string EncodeForFFmpeg(string line)
        {
            return line.Replace("\"", "").Replace("'", "").Replace("%", "\\\\\\%").Replace(":", "\\\\\\:");
        }


        bool videoPart(string line, int partIndex)
        {
            //ffmpeg drawtext filter එකේ text එක විදියට පාවිච්චි කරන්න පුළුවන් විදියට input line එක වෙනස් කරගන්නවා.
            line = EncodeForFFmpeg(line);

            //parts වලට අදාළ files delete කරනවා.
            deleteRelatedFiles();



            string audioFilePath = $"audiofiles/{partIndex}.mp3";
            string textFilePath = $"textfiles/{partIndex}.txt";
            string videoWithoutAudio = $"videofiles/without_audio_{partIndex}.mp4";
            string videoWithAudio = $"videofiles/with_audio_{partIndex}.mp4";
            double AudioDuration = 0;


            //line එකට අදල audio file එක tts වලින් අරගෙන save කරගන්නවා
            SynthesizeText(line, audioFilePath);


            //audio file එකේ play time එක ගන්නවා
            AudioDuration = Math.Round(new AudioFileReader(audioFilePath).TotalTime.TotalSeconds, 1, MidpointRounding.AwayFromZero);


            // ඉස්සෙල්ලා ගත්ත audio file එකට අදාළ 'silent' data ගන්නවා
            var data = dbData(audioFilePath);

            // animation එක හදන්න අවශ්‍ය text file එක generate කරගන්නවා
            //, කලින් ගත්ත data පාවිච්චි කරලා
            GenerateTextFileForFfmpeg(data, 20, textFilePath);

            //කලින් generate කරගත්ත text file එක පාවිච්චි කරලා video එක generate කරගනන්නවා
            ExecuteFFMpeg($"-f concat -i {textFilePath} -vsync vfr -pix_fmt yuv420p {videoWithoutAudio}", false);

            //වීඩියෝ file එකයි audio file එකයි එකතු කරගන්නවා.
            ExecuteFFMpeg($"-i {videoWithoutAudio} -i {audioFilePath} -map 0:v -map 1:a -c:v copy -shortest {videoWithAudio}", false);


            //කලින් හදාගත්ත video එක "intermediate" folder එකට copy කරගන්නවා
            File.Copy(videoWithAudio, "intermediate/video1.mp4");

            //"line" එකට අදාළ text animation data ලබාගන්නවා
            var textAnimationData = GetTextAnimationData(line, 5, 50, 5);



            //intermediate folder එකේ මේ part එකට අදාල intemediate video generate කරලා අන්තිමට generate වෙච්ච video එකේ
            // id එකගන්නවා.
            int final_video_id = ApplyTextAnimationLineByLine(textAnimationData, AudioDuration, 600, 100, 25, 50, 40, "white");

            //අන්තිමට intermediate folder එකේ generate වෙච්ච video එක parts folder එකට copy කරගන්නවා.
            File.Copy($"intermediate/video{final_video_id}.mp4", $"parts/part{partIndex}.mp4");

            return true;

        }

        private static TimeSpan GetDuration(string filePath)
        {
            using (var shell = ShellObject.FromParsingName(filePath))
            {
                IShellProperty prop = shell.Properties.System.Media.Duration;
                var t = (ulong)prop.ValueAsObject;
                return TimeSpan.FromTicks((long)t);
            }
        }


        //only charachter animation
        void GenerateVideoFromText(string text)
        {
            //දැනට තියන files එක delete කරනවා
            File.Delete("audiofiles/audio.mp3");

            try
            {
                //අදාළ audio file එක generate කරනවා 
                SynthesizeText(text, "audiofiles/audio.mp3");
            }
            catch (Exception ex)
            {
                throw new Exception("SynthesizeText");
            }

            //audio file එකට අදාළ sample db data ටික ගන්නවා
            var dbdata = dbData("audiofiles/audio.mp3");

            GenerateTextFileForFfmpeg(dbdata, 20, "textfiles/input.txt");



        }

        private void GenerateTextFileForFfmpeg(Dictionary<double, int> dbdata, int dbdataThreshhold, string outputTextfileName)
        {
            StringBuilder stringBuilder = new StringBuilder();
            Random random = new Random();


            for (int x = 1; x <= dbdata.Count; x++)
            {
                if (dbdata[x] > dbdataThreshhold) // still image 
                {


                    stringBuilder.AppendLine($"file 'animation_images/{random.Next(1, 6)}.png'");
                    stringBuilder.AppendLine($"duration 0.1");
                    stringBuilder.AppendLine($"file 'animation_images/{random.Next(1, 6)}.png'");
                    stringBuilder.AppendLine($"duration 0.1");
                    stringBuilder.AppendLine($"file 'animation_images/{random.Next(1, 6)}.png'");
                    stringBuilder.AppendLine($"duration 0.1");
                    stringBuilder.AppendLine($"file 'animation_images/{random.Next(1, 6)}.png'");
                    stringBuilder.AppendLine($"duration 0.1");
                    stringBuilder.AppendLine($"file 'animation_images/{random.Next(1, 6)}.png'");
                    stringBuilder.AppendLine($"duration 0.1");
                }
                else
                {
                    stringBuilder.AppendLine("file 'animation_images/still.png'");
                    stringBuilder.AppendLine("duration 0.1");
                    stringBuilder.AppendLine("file 'animation_images/still.png'");
                    stringBuilder.AppendLine("duration 0.1");
                    stringBuilder.AppendLine("file 'animation_images/still.png'");
                    stringBuilder.AppendLine("duration 0.1");
                    stringBuilder.AppendLine("file 'animation_images/still.png'");
                    stringBuilder.AppendLine("duration 0.1");
                    stringBuilder.AppendLine("file 'animation_images/still.png'");
                    stringBuilder.AppendLine("duration 0.1");
                }

            }

            File.WriteAllText(outputTextfileName, stringBuilder.ToString());

        }

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




        /// <summary>
        /// 
        /// </summary>
        /// <param name="audioFilePath"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static bool IsSilent(string audioFilePath, TimeSpan startTime, TimeSpan endTime, int? threshold)
        {
            return false;
        }


        Dictionary<double, int> dbData(string filePath)
        {
            var silenceDict = new Dictionary<double, int>();
            using (NAudio.Wave.AudioFileReader wave = new NAudio.Wave.AudioFileReader(filePath))
            {
                var samplesPerSecond = wave.WaveFormat.SampleRate * wave.WaveFormat.Channels;
                var readBuffer = new float[samplesPerSecond];
                int samplesRead;
                int i = 1;
                do
                {
                    samplesRead = wave.Read(readBuffer, 0, samplesPerSecond / 2);
                    if (samplesRead == 0) break;
                    var max = readBuffer.Take(samplesRead).Max();

                    if ((int)(max * 100) != 0)
                    {
                        silenceDict.Add(i, (int)(max * 100));
                    }
                    else
                    {
                        silenceDict.Add(i, (int)(max * 100));
                    }

                    i++;
                } while (samplesRead > 0);
            }

            return silenceDict;
        }


        public static void SynthesizeText(string text, string filename)
        {
            try
            {

                TextToSpeechClientBuilder textToSpeechClientBuilder = new TextToSpeechClientBuilder();
                textToSpeechClientBuilder.CredentialsPath = _tts_json_link;
                TextToSpeechClient client = textToSpeechClientBuilder.Build();


                var response = client.SynthesizeSpeech(new SynthesizeSpeechRequest
                {


                    Input = new SynthesisInput
                    {
                        Text = text,

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
                        SpeakingRate = 0.75,

                    }


                });

                using (Stream output = File.Create(filename))
                {
                    response.AudioContent.WriteTo(output);


                }
            }
            catch (Exception ex)
            {
                throw new Exception("tts exception");
            }
        }


        public static bool ExecuteFFMpeg(string arguments)
        {
            try
            {
                Process process = Process.Start("cmd.exe", $@"/k ffmpeg.exe {arguments}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return true;
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

        public static void Json()
        {

            string jsonString = File.ReadAllText("temp/stupidstuff.json");


            var records = JsonConvert.DeserializeObject<List<temp>>(jsonString);






            var records1 = records.Where(r => r.body.Length > 500 && r.body.Length < 2000).ToList();
            var records2 = records1.Where(r => r.body.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList().Count() > 3).ToList();

            foreach (var item in records2)
            {
                item.title = item.body.Split('\n').First().Split('.').First();
                item.no_of_chr = item.body.Length;
            }


            List<Joke> newlist = new List<Joke>();

            foreach (var item in records2)
            {
                Joke joke = new Joke()
                {
                    body = item.body,
                    id = RandomString(10),
                    no_of_charachter = item.no_of_chr,
                    score = 0,
                    title = item.title,
                    used = false
                };

                newlist.Add(joke);
            }


            using (Context context = new Context())
            {
                var currentJOkesList = context.Jokes.ToList();

                context.Jokes.RemoveRange(currentJOkesList);
                context.SaveChanges();


                foreach (var joke in newlist)
                {
                    context.Jokes.Add(joke);
                }

                context.SaveChanges();
            }



        }

        class temp
        {
            public string body { get; set; }
            public string category { get; set; }
            public string id { get; set; }
            public string rating { get; set; }
            public int no_of_chr { get; set; }
            public string title { get; set; }
        }

        public static void Json1()
        {


            //using (Context context = new Context())
            //{
            //    var list = context.Jokes.ToList();

            //    foreach (var joke in list)
            //    {
            //        joke.no_of_charachter = joke.body.Length;
            //    }

            //    context.SaveChanges();

            //}




        }

        //private static void ConvertMp3ToWav(string _inPath_, string _outPath_)
        //{
        //    using (Mp3FileReader mp3 = new Mp3FileReader(_inPath_))
        //    {
        //        using (WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(mp3))
        //        {
        //            WaveFileWriter.CreateWaveFile(_outPath_, pcm);
        //        }
        //    }
        //}


        string SpeechToText(string waveFilePath)
        {
            SpeechRecognitionEngine sre = new SpeechRecognitionEngine();
            Grammar gr = new DictationGrammar();
            sre.LoadGrammar(gr);
            sre.SetInputToWaveFile(waveFilePath);
            sre.BabbleTimeout = new TimeSpan(Int32.MaxValue);
            sre.InitialSilenceTimeout = new TimeSpan(Int32.MaxValue);
            sre.EndSilenceTimeout = new TimeSpan(100000000);
            sre.EndSilenceTimeoutAmbiguous = new TimeSpan(100000000);


            StringBuilder sb = new StringBuilder();
            while (true)
            {
                try
                {
                    var recText = sre.Recognize();
                    if (recText == null)
                    {
                        break;
                    }

                    sb.Append(recText.Text);
                }
                catch (Exception ex)
                {
                    //handle exception      
                    //...

                    break;
                }
            }
            return sb.ToString();
        }


        public static int ApplyTextAnimationLineByLine(List<Tuple<int, int, string, int, int>> tds, double videoPlayTime, int xOff, int yOff, int char_lenght, int line_height, int fontSize, string fontColor)
        {
            //ඔක්කොම session id ටික ගන්නවා
            var sIds = tds.Select(t => t.Item1).Distinct().ToList();
            //වචන අතර time gap එක ගන්නවා (මේ වචන ටිකට අදාළ voice clip එකේ duration එක මත රදාපවතිනවා)
            double timeBetweenWords = videoPlayTime / tds.Count();
            int workCount = 0;

            //queue එකක් හදාගන්නවා (startTime,endTimeForTheSection,line,positionX,positionY)
            Queue<Tuple<double, double, string, int, int>> data = new Queue<Tuple<double, double, string, int, int>>();

            int cumilated_wordcount = 0;


            foreach (var sId in sIds)
            {
                //සලකන section එකට අදාළ data ටික ගන්නවා.
                var related_tds = tds.Where(t => t.Item1 == sId).ToList();

                //මේ section එකේ සියලුම text disapear වෙන වෙලාව ගණනය කරනවා.
                double endTimeForTheSection = timeBetweenWords * (cumilated_wordcount + related_tds.Count());
                if (endTimeForTheSection == videoPlayTime)
                {
                    endTimeForTheSection = endTimeForTheSection -= 0.2;
                }


                //සලකන section එකට අදාළ line no සේරම ගන්නව
                List<int> lineNos = related_tds.Select(td => td.Item4).Distinct().ToList();

                foreach (var lineNo in lineNos)
                {
                    var relatedTdsForLine = related_tds.Where(td => td.Item4 == lineNo).ToList();
                    string line = "";



                    foreach (var relatedTd in relatedTdsForLine)
                    {
                        line += relatedTd.Item3;
                        line += " ";
                    }

                    double startTime = cumilated_wordcount * timeBetweenWords;

                    int positionX = xOff;
                    int positionY = yOff + (line_height * lineNo);

                    data.Enqueue(Tuple.Create(startTime, endTimeForTheSection, line, positionX, positionY));

                    cumilated_wordcount += relatedTdsForLine.Count();
                }


            }

            int sourceVideoPostFix = 1;



            while (0 < data.Count())
            {
                List<Tuple<double, double, string, int, int>> dataForConvertion = new List<Tuple<double, double, string, int, int>>();

                for (int x = 0; x < 20; x++)
                {
                    if (data.Count() == 0) break;
                    dataForConvertion.Add(data.Dequeue());

                }



                var command = generateTextAnimationCommand($"Intermediate/video{sourceVideoPostFix}.mp4", $"Intermediate/video{sourceVideoPostFix + 1}.mp4", dataForConvertion, fontSize, fontColor);
                int res = ExecuteFFMpeg(command, false);
                sourceVideoPostFix++;
            }





            return sourceVideoPostFix;


        }




        //sectionid,lineid,word,line index , position in line
        public static int ApplyTextAnimationWordByWord(List<Tuple<int, int, string, int, int>> tds, double videoPlayTime, int xOff, int yOff, int char_lenght, int line_height, int fontSize, string fontColor)
        {
            //ඔක්කොම session id ටික ගන්නවා
            var sIds = tds.Select(t => t.Item1).Distinct().ToList();
            //වචන අතර time gap එක ගන්නවා (මේ වචන ටිකට අදාළ voice clip එකේ duration එක මත රදාපවතිනවා)
            double timeBetweenWords = videoPlayTime / tds.Count();
            int workCount = 0;

            //queue එකක් හදාගන්නවා (startTime,endTimeForTheSection,word,positionX,positionY)
            Queue<Tuple<double, double, string, int, int>> data = new Queue<Tuple<double, double, string, int, int>>();

            int cumilated_wordcount = 0;


            foreach (var sId in sIds)
            {
                //සලකන section එකට අදාළ data ටික ගන්නවා.
                var related_tds = tds.Where(t => t.Item1 == sId).ToList();

                //මේ section එකේ සියලුම text disapear වෙන වෙලාව ගණනය කරනවා.
                double endTimeForTheSection = timeBetweenWords * (cumilated_wordcount + related_tds.Count());

                foreach (var td in related_tds)
                {
                    //මෙම වචනයට අදාළ startime එක ගණනය කරනවා
                    double startTime = workCount * timeBetweenWords;

                    //මෙමේ වචනයට අදාළ position එක ගණනය කරනවා.
                    int positionX = xOff + (char_lenght * td.Item5);
                    int positionY = yOff + (line_height * td.Item4);

                    //starttime එක දශම ස්ථාන දෙකකට වටයාගන්නවා
                    startTime = Math.Round(startTime, 2, MidpointRounding.AwayFromZero);


                    //data object එකට කලින් හොයාගත්ත data ටික දාගන්නවා.
                    data.Enqueue(Tuple.Create(startTime, endTimeForTheSection, td.Item3, positionX, positionY));

                    //AddWordsFfmpeg(vp, startTime, endTimeForTheSection, td.Item3, positionX, positionY);

                    workCount++;
                }

                cumilated_wordcount += related_tds.Count();

            }

            int sourceVideoPostFix = 1;



            while (0 < data.Count())
            {
                List<Tuple<double, double, string, int, int>> dataForConvertion = new List<Tuple<double, double, string, int, int>>();

                for (int x = 0; x < 20; x++)
                {
                    if (data.Count() == 0) break;
                    dataForConvertion.Add(data.Dequeue());

                }



                var command = generateTextAnimationCommand($"Intermediate/video{sourceVideoPostFix}.mp4", $"Intermediate/video{sourceVideoPostFix + 1}.mp4", dataForConvertion, fontSize, fontColor);
                int res = ExecuteFFMpeg(command, false);
                sourceVideoPostFix++;
            }





            return sourceVideoPostFix;


        }

        private static void copyVideo()
        {
        }

        private static void deleteRelatedFiles()
        {
            ClearFilesInFolder("textfiles");
            ClearFilesInFolder("audiofiles");
            ClearFilesInFolder("intermediate");
            ClearFilesInFolder("videofiles");


        }


        private static void ClearFilesInFolder(string FolderName)
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
                middlePart += $" drawtext=text='{tuple.Item3}':fontfile=Arial.ttf:x={tuple.Item4}:y={tuple.Item5}:fontcolor={fontColor}:shadowy=2:shadowcolor=white:fontsize={fontSize}:enable='between(t,{tuple.Item1.ToString()},{tuple.Item2.ToString()})',fade=t=in:start_time={tuple.Item1.ToString()}:d=0.0:alpha=1,fade=t=out:start_time={tuple.Item2.ToString()}:d=0.0:alpha=1 , ";
                counter++;
            }
            middlePart = middlePart.Trim();
            middlePart = middlePart.Remove(middlePart.Length - 1);
            middlePart = middlePart.Trim();

            string command = startingPart + middlePart + endingPart;

            return command;
        }

        public static List<Section> GetSectionsFromJoke(Joke joke, int canvasHeight, int canvasWidth)
        {
            List<string> lines = joke.body.Split('\n').ToList();
            lines = lines.Select(l => l.Trim()).ToList();



            lines.RemoveAll(l => string.IsNullOrEmpty(l));
            List<Section> sections = new List<Section>();
            //  List<DataTable> dataTables = new List<DataTable>();
            var currentLineIndex = 0;
            var currentLineUsage = 0;
            string currentSectionString = "";
            bool firstTime = true;
            bool sectionEnded = false;

            //  DataTable trackVar = GetDataTable(canvasWidth,canvasHeight);
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

            int length = lines.Count;
            int i = 0;
            bool dontIncrementI = false;

            while (i < length)
            {
                dontIncrementI = false;

                if (firstTime) { firstTime = false; }
                else if (sectionEnded)
                {
                    currentLineIndex = 0;
                    currentLineUsage = 0;
                    sectionEnded = false;

                }
                else
                {
                    currentLineIndex++;
                    currentLineUsage = 0;
                }


                List<string> words = lines[i].Split(' ').ToList();

                foreach (var word in words)
                {
                    if (currentLineUsage + word.Length > canvasWidth) // line එකේ ඉඩ මදි
                    {
                        if (currentLineIndex + 1 < canvasHeight) // line තව තියනවා
                        {

                            currentLineIndex++;
                            currentLineUsage = 0;
                            //draw here
                            keyValuePairs.Add(currentLineIndex + " " + currentLineUsage, word);

                            currentLineUsage = word.Length + 1;

                        }
                        else // line සේරම ඉවරයි
                        {
                            Section section = new Section();
                            section.Text = currentSectionString;
                            section.keyValuePairs = keyValuePairs;
                            keyValuePairs = new Dictionary<string, string>();
                            sections.Add(section);
                            sectionEnded = true;

                            dontIncrementI = true;
                            break;
                        }
                    }
                    else // line එකේ ඉඩ තියනව
                    {
                        //draw here
                        keyValuePairs.Add(currentLineIndex + " " + currentLineUsage, word);
                        currentLineUsage = currentLineUsage + word.Length + 1;
                    }
                }

                if (dontIncrementI == false)
                {
                    i++;
                }
            }


            //foreach (var line in lines)
            //{
            //    if (firstTime) { firstTime = false; }
            //    else
            //    {
            //        currentLineIndex++;
            //        currentLineUsage = 0;
            //    }


            //    List<string> words = line.Split(' ').ToList();

            //    foreach (var word in words)
            //    {
            //        if (currentLineUsage + word.Length > canvasWidth)
            //        {
            //            if (currentLineIndex + 1 < canvasHeight)
            //            {



            //                currentLineIndex++;
            //                currentLineUsage = 0;
            //                //draw here
            //                keyValuePairs.Add(currentLineIndex + " " + currentLineUsage, word);

            //                currentLineUsage = word.Length + 1;

            //            }
            //            else
            //            {
            //                Section section = new Section();
            //                section.Text = currentSectionString;
            //                section.keyValuePairs = keyValuePairs;
            //                keyValuePairs = new Dictionary<string, string>();
            //                sections.Add(section);


            //                break;
            //            }
            //        }
            //        else
            //        {
            //            //draw here
            //            keyValuePairs.Add(currentLineIndex + " " + currentLineUsage, word);
            //            currentLineUsage = currentLineUsage + word.Length + 1;
            //        }
            //    }
            //}

            return sections;

        }

        //start
        private async void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Invoke(new Action(() => richTextBox1.Text += $"{DateTime.Now} Daily Cycle Started!\n"));

            _cts = new CancellationTokenSource();
            button2.Enabled = false;
            _uploadTimes = new List<DateTime>();

            //set upload times
            if (checkBox1.Checked)
            {
                _uploadTimes.Add(timePicker1.Value);
            }
            if (checkBox2.Checked)
            {
                _uploadTimes.Add(timePicker2.Value);
            }
            if (checkBox3.Checked)
            {
                _uploadTimes.Add(timePicker3.Value);
            }
            if (checkBox4.Checked)
            {
                _uploadTimes.Add(timePicker4.Value);
            }




            DiableControls();

            try
            {

                await Task.Run(TestMainLoop);
            }
            catch (Exception ex)
            {

            }
        }

        //stop
        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Invoke(new Action(() => richTextBox1.Text += $"{DateTime.Now}: Daily cycled stopped! \n"));

            _cts.Cancel();
            EnableControls();

        }

        private void EnableControls()
        {
            // throw new NotImplementedException();
            button2.Enabled = false;
            button1.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            panel5.Enabled = true;
            checkBox5.Enabled = true;
        }

        private void DiableControls()
        {
            // throw new NotImplementedException();
            button2.Enabled = true;
            button1.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            panel5.Enabled = false;
            checkBox5.Enabled = false;
        }


        async Task TestMainLoop()
        {
            try
            {
                while (true)
                {
                    //richTextBox1.Invoke(new Action(() => richTextBox1.Text += "test \n"));
                    //Thread.Sleep(2000);


                    DateTime? dateTime = GetNextTime();
                    if (dateTime != null)
                    {
                        TimeSpan? timeSpan = TimeFromNow(dateTime);
                        richTextBox1.Invoke(new Action(() => richTextBox1.Text += $"awaiting for {timeSpan.ToString()} \n"));

                        try
                        {
                            await Task.Delay((TimeSpan)timeSpan, _cts.Token);
                        }
                        catch (Exception ex)
                        {

                        }

                        if (_cts.IsCancellationRequested)
                        {
                            break;
                        }



                        richTextBox1.Invoke(new Action(() => richTextBox1.Text += $"{DateTime.Now}: video generated \n"));
                        Thread.Sleep(5000);

                        richTextBox1.Invoke(new Action(() => richTextBox1.Text += $"{DateTime.Now}: video Uploaded \n"));

                    }
                    else
                    {
                        DateTime dateTime1 = new DateTime(DateTime.Now.Year
                            , DateTime.Now.Month
                            , DateTime.Now.Day).AddDays(1);

                        TimeSpan tillEndOfDate = dateTime1 - DateTime.Now;

                        if (_cts.IsCancellationRequested)
                        {
                            break;
                        }
                        richTextBox1.Invoke(new Action(() => richTextBox1.Text += $"awaiting for {tillEndOfDate.ToString()} till day end \n"));


                        await Task.Delay(tillEndOfDate, _cts.Token);

                    }

                }
            }
            catch (Exception ex)
            {
                richTextBox1.Invoke(new Action(() => MessageBox.Show(ex.Message)));
            }
        }



        async Task MainLoop()
        {
            try
            {
                while (true)
                {
                    //richTextBox1.Invoke(new Action(() => richTextBox1.Text += "test \n"));
                    //Thread.Sleep(2000);

                    DateTime? dateTime = GetNextTime();
                    if (dateTime != null)
                    {
                        TimeSpan? timeSpan = TimeFromNow(dateTime);
                        await Task.Delay((TimeSpan)timeSpan, _cts.Token);
                        if (_cts.IsCancellationRequested)
                        {
                            break;
                        }

                        Joke joke = null;
                        List<string> lines = new List<string>();
                        string title = "";

                        //get a joke from db
                        using (Context context = new Context())
                        {
                            joke = context.Jokes.Where(j => j.used == false).FirstOrDefault();
                            lines = joke.body.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            title = joke.title;

                            joke.used = true;
                            context.SaveChanges();
                        }

                        //lines.Insert(0, title);
                        CreateAJokeVideo(lines);
                        richTextBox1.Invoke(new Action(() => richTextBox1.Text += $"{DateTime.Now}: video generated \n"));

                        //modify title if profanity contains

                        if (linesContainProfaniy(lines))
                        {
                            title = $"(Adult Joke) {title}";

                        }


                        UploadVideo("output/output.mp4", title, checkBox5.Checked);
                    }
                    else
                    {
                        DateTime dateTime1 = new DateTime(DateTime.Now.Year
                            , DateTime.Now.Month
                            , DateTime.Now.Day).AddDays(1);

                        TimeSpan tillEndOfDate = dateTime1 - DateTime.Now;

                        if (_cts.IsCancellationRequested)
                        {
                            break;
                        }
                        await Task.Delay(tillEndOfDate, _cts.Token);

                    }
                }
            }
            catch (Exception ex)
            {
                richTextBox1.Invoke(new Action(() => MessageBox.Show(ex.Message)));
            }
        }

        bool linesContainProfaniy(List<string> lines)
        {
            List<string> profanityWords = new List<string>()
            {
                "fuck","ass","pussy","cunt","fucked","shit","bitch"
            };

            foreach (var line in lines)
            {
                foreach (var prof in profanityWords)
                {
                    if (line.Contains(prof, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }

                }
            }

            return false;
        }

        //youtube developer api
        private void UploadVideo(string path, string title, bool privateVideo)
        {

            string result = Youtube.upload(title, path, GetTags(400), privateVideo, checkBox8.Checked,checkBox9.Checked);

            richTextBox1.Invoke(new Action(() => richTextBox1.Text += $"{DateTime.Now} : {result} \n"));
        }

        private List<string> GetTags(int noOfChar)
        {
            List<string> allTags = new List<string>()
            {
                "try not to laugh",
" funny jokes",
" funny",
" dad jokes",
" joke",
" jokes that will make you laugh so hard",
" comedy",
" best jokes",
" jokes to tell your friends",
" joke of the day",
" humor",
" joke compilation",
" funny videos",
" children jokes",
" clean jokes",
" short jokes",
" funny joke",
" classroom jokes",
" two line jokes",
" jokes about people",
" stand up comedy",
" jimmy carr",
" most",
" анекдоты",
" super jokes",
" top",
" top jokes",
" смешно",
" best jokes ever",
" jokes challenge",
" jokes in english",
" funny joke video",
" funny video",
" hilarious",
" laugh",
" short jokes to tell your friends",
" funny jokes to tell your friends",
" best jokes to tell",
" funny jokes to tell",
" funniest joke",
" jokes that make you laugh so hard",
" jokes on you",
" little johnny jokes",
" jokes video",
" school jokes",
" kid jokes",
" teacher jokes",
" jokes compilation 2019",
" silly jokes",
" chilli jokes",
" dark jokes",
" black jokes",
" offensive jokes",
" louis ck",
" norm macdonald",
" frankie boyle",
" ricky gervais",
" anthony jeselinkdavid cross",
" 9 11 jokes",
" hilarious comedy",
" dark humor",
" top 10 jokes",
" laugh planet",
" best dark jokes",
" jokes 2019",
" top 10 video",
" funny animals jokes",
" new jokes",
" school kid jokes",
" dirty jokes",
" good jokes",
" adult jokes",
" marriage jokes",
" daily jokes",
" corny jokes",
" stupid jokes",
" bar jokes",
" blonde jokes",
" knock knock jokes",
" knock knock jokes funny",
" 30 knock knock jokes",
" knock knock",
" knock knock jokes!",
" funny jokes for children",
" joke telling contest"
};
            List<string> ret = new List<string>();

            Random random = new Random();

            while (true)
            {
                int existingLenght = 0;
                foreach (var str in ret)
                {
                    existingLenght += str.Length;
                }

                string newPossibleTag = allTags[random.Next(0, allTags.Count() - 3)];

                if ((existingLenght + newPossibleTag.Length) > noOfChar)
                {
                    return ret;
                }
                else
                {
                    ret.Add(newPossibleTag);
                }

            }


        }




        //private void GenerateVideo()
        //{
        //    Joke joke = null;
        //    List<string> lines = new List<string>();
        //    string title = "";

        //    //get a joke from db
        //    using (Context context = new Context())
        //    {
        //        joke = context.Jokes.Where(j => j.used == false).FirstOrDefault();
        //        lines = joke.body.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
        //        title = joke.title;

        //        joke.used = true;
        //        context.SaveChanges();
        //    }

        //    lines.Insert(0, title);

        //    CreateAJokeVideo(lines);

        //}




        private TimeSpan? TimeFromNow(DateTime? dateTime)
        {
            return dateTime.Value.TimeOfDay - DateTime.Now.TimeOfDay;


        }

        private DateTime? GetNextTime()
        {
            DateTime now = DateTime.Now;
            DateTime currentPick = DateTime.MaxValue;

            foreach (var time in _uploadTimes)
            {
                if (time.TimeOfDay < now.TimeOfDay)
                    continue;
                if (time.TimeOfDay < currentPick.TimeOfDay)
                    currentPick = time;

            }

            if (currentPick == DateTime.MaxValue)
            {
                return null;
            }
            else
            {
                return currentPick;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure?", "Alert", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.OK)
            {

                using (Context context = new Context())
                {
                    var list = context.Jokes.ToList();

                    foreach (var joke in list)
                    {
                        if (joke.used == true)
                        {
                            joke.used = false;
                        }
                    }

                }
            }
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;


            await Task.Run(() =>
            {
                try
                {
                    Joke joke = null;
                    List<string> lines = new List<string>();
                    string title = "";

                    //get a joke from db
                    using (Context context = new Context())
                    {
                        joke = context.Jokes.Where(j => j.used == false).FirstOrDefault();
                        lines = joke.body.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        title = joke.title;

                        joke.used = true;
                        context.SaveChanges();
                    }

                    //lines.Insert(0, title);  // මේ line එක ඕනේ title එකත් story එකේ part එකක් නම් විතරයි. එකට බලපාන්නේ jokes
                    // database එකේ විදියයි
                    CreateAJokeVideo(lines);
                    richTextBox1.Invoke(new Action(() => richTextBox1.Text += $"{DateTime.Now}: video generated \n"));

                    //modify title if profanity contains

                    if (linesContainProfaniy(lines))
                    {
                        title = $"(Adult Joke) {title}";

                    }



                    UploadVideo("output/output.mp4", title, checkBox5.Checked);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });


            button1.Enabled = true;
            button2.Enabled = false;
            button3.Enabled = true;
            button4.Enabled = true;

        }

        private void button5_Click(object sender, EventArgs e)
        {
            File.WriteAllText("refreshCredentials.txt", RandomString(20));
            this.Close();
        }

        public static string RandomString(int length)
        {
            Random random = new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }


        //add joke to database
        private void button7_Click(object sender, EventArgs e)
        {
            if (richTextBox_new_joke.Text == "") return;


            string inputJoke = richTextBox_new_joke.Text;


            using (Context context = new Context())
            {

                string title = inputJoke.Split('\n').First();
                int no_of_Char = inputJoke.Length;


                string id = RandomString(10);
                if (checkBox7.Checked)
                {
                    if (context.Jokes.Where(joke => joke.used == false).FirstOrDefault() != null)
                    {
                        var FirstNonUsed = context.Jokes.Where(joke => joke.used == false).FirstOrDefault();

                        Joke joke = new Joke()
                        {
                            id = id
                            ,
                            body = FirstNonUsed.body
                            ,
                            no_of_charachter = FirstNonUsed.no_of_charachter
                            ,
                            score = FirstNonUsed.score
                            ,
                            title = FirstNonUsed.title
                            ,
                            used = FirstNonUsed.used
                        };


                        FirstNonUsed.no_of_charachter = no_of_Char;
                        FirstNonUsed.body = inputJoke;
                        FirstNonUsed.score = 0;
                        FirstNonUsed.title = title;
                        FirstNonUsed.used = false;
                        context.SaveChanges();
                        context.Jokes.Add(joke);
                        context.SaveChanges();
                    }
                    else
                    {
                        context.Jokes.Add(new Joke() { id = id, body = inputJoke, no_of_charachter = no_of_Char, score = 0, title = title, used = false });

                    }
                }
                else
                {
                    context.Jokes.Add(new Joke() { id = id, body = inputJoke, no_of_charachter = no_of_Char, score = 0, title = title, used = false });
                }
                context.SaveChanges();

                dataGridView1.DataSource = context.Jokes.ToList();

            }

            MessageBox.Show("Added");

            richTextBox_new_joke.Text = "";

        }


        //remove joke fromd database
        private void button8_Click(object sender, EventArgs e)
        {
            int columnIndex = dataGridView1.CurrentCell.ColumnIndex;
            int rowIndex = dataGridView1.CurrentCell.RowIndex;

            using (Context context = new Context())
            {
                var removingJOke = context.Jokes.ToList()[rowIndex];
                context.Jokes.Remove(removingJOke);
                context.SaveChanges();

                dataGridView1.DataSource = context.Jokes.ToList();

            }
            MessageBox.Show("Removed");
        }

        //add bulk (should be with a spefic formart)
        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        //generate a test video
        private async void button9_Click(object sender, EventArgs e)
        {
            tabControl1.Enabled = false;
            label3.Visible = true;
            richTextBox2.Enabled = false;

            string joketext = richTextBox2.Text;

            var lines = joketext.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            await Task.Run(() => CreateAJokeVideo(lines));

            //open a video with default defaault video player
            try
            {
                //System.Diagnostics.Process.Start(@"output\output.mp4");
                OpenWithDefaultProgram(@"output\output.mp4");
            }
            catch (Exception ex)
            {
                MessageBox.Show("please run output\\output.mp4");
            }


            tabControl1.Enabled = true;
            label3.Visible = false;
            richTextBox2.Enabled = true;

            MessageBox.Show("Done");
        }

        public static void OpenWithDefaultProgram(string path)
        {
            using Process fileopener = new Process();

            fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.Arguments = "\"" + path + "\"";
            fileopener.Start();
        }

        //change backgroud image 
        private void button10_Click(object sender, EventArgs e)
        {
            int ioutput = 0;
            double doutput = 0;
            if (_bitmapNewBackgroundImage == null)
            {
                MessageBox.Show("Background Image is not Selected!");
                return;
            }
            else if (!int.TryParse(textBox1.Text, out ioutput))
            {
                MessageBox.Show("X position value not correct!");
                return;
            }
            else if (!int.TryParse(textBox2.Text, out ioutput))
            {
                MessageBox.Show("Y position value not correct!");
                return;
            }
            else if (!double.TryParse(textBox3.Text, out doutput))
            {
                MessageBox.Show("Scale value not correct!");
                return;
            }

            //delete existing animation images
            var filesInDir = Directory.GetFiles("textfiles\\animation_images").ToList();
            foreach (var file in filesInDir)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error Deleting Exising Images...");
                    return;
                }
            }



            List<string> charachterImagesPaths = Directory.GetFiles("textfiles\\charachter_images").ToList();
            foreach (string imgPath in charachterImagesPaths)
            {
                try
                {
                    var cloned = _bitmapNewBackgroundImage.Clone() as Bitmap;
                    string savePath = $"textfiles\\animation_images\\{imgPath.Split('\\').Last()}";
                    addOverlay(cloned, imgPath, savePath, int.Parse(textBox1.Text), int.Parse(textBox2.Text), double.Parse(textBox3.Text));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error Generating new Images...");
                    return;
                }
            }

            Image image = GetCopyImage("textfiles\\animation_images\\1.png");
            pictureBox1.Image = image;
            label8.Text = "";

        }

        //choose new background image
        private void button11_Click(object sender, EventArgs e)
        {


            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PNG Portable Network Graphics (*.png)|" + "*.png";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _bitmapNewBackgroundImage = Bitmap.FromFile(openFileDialog.FileName) as Bitmap;

                if (_bitmapNewBackgroundImage.Width != 1920 || _bitmapNewBackgroundImage.Height != 1080)
                {
                    MessageBox.Show("Error", "Image is not of 1920*1080 resolution.");
                    _bitmapNewBackgroundImage = null;
                    return;
                }

                label8.Text = openFileDialog.FileName;

                Image image = GetCopyImage(openFileDialog.FileName);
                pictureBox1.Image = image;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Image image = GetCopyImage("textfiles\\animation_images\\1.png");
            pictureBox1.Image = image;
            label8.Text = "";
        }

        private void tabPage5_Click(object sender, EventArgs e)
        {
            string folderPath = "outro";

            //if the folder has a single video with 1920*1080 resolution
            if (Directory.GetFiles(folderPath).ToList().Count() == 1)
            {
                //have to get the file and make sure its a video and its resolution is 1920*1080
                string videoFilePath = Directory.GetFiles(folderPath).First();
                var ffProbe = new NReco.VideoInfo.FFProbe();
                var videoInfo = ffProbe.GetMediaInfo(videoFilePath);
                
                
            }



            //else if folder has no video 

            //else if folder has a video with different resolution

            //folder does not exist
        }

        private void button13_Click(object sender, EventArgs e)
        {
            //ExecuteFFMpeg($"-f concat -i temp1/info.txt -vsync vfr -pix_fmt yuv420p temp1/outro.mp4", true);

          //  ExecuteFFMpeg(@"-f concat -i temp/videofiles.txt -c copy temp/contcated.mp4", true);

            int oidsjfosdif = 0;


            //StringBuilder stringBuilder = new StringBuilder();
            //Random random = new Random();

            //string outputTextfileName = "temp1/info.txt";

            //for (int x = 1; x <= 101; x++)
            //{
            //    string number = "";

            //    if(x.ToString().Length == 1)
            //    {
            //        number = "00" + x.ToString();
            //    }
            //    else if(x.ToString().Length == 2)
            //    {
            //        number = "0" + x.ToString();
            //    }
            //    else
            //    {
            //        number = x.ToString();
            //    }


            //    stringBuilder.AppendLine($"file 'ezgif-frame-{number}.png'");
            //    stringBuilder.AppendLine($"duration 0.1");
            //}
            //File.WriteAllText(outputTextfileName, stringBuilder.ToString());
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox8.Checked == true)
            {
                if (checkBox9.Checked)
                {
                    checkBox9.Checked = false;
                }
            }
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox9.Checked == true)
            {
                if (checkBox8.Checked)
                {
                    checkBox8.Checked = false;
                }
            }
        }
    }
}
