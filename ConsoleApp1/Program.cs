using Microsoft.EntityFrameworkCore;
using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleApp1
{
    class Program
    {
        private static List<List<string>> filtered7;
        private static List<Joke> noOfLinesFiltered;


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

            int partIndex = 0;

            //generate video parts
            foreach (var line in storyLines)
            {
                videoPart(line, partIndex);
                partIndex++;
            }

            //parts ටික එකතු කරලා complete video එක හදනවා


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

            TextToSpeechClientBuilder textToSpeechClientBuilder = new TextToSpeechClientBuilder();
            textToSpeechClientBuilder.CredentialsPath = "tts5.json";
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

            string jsonString = File.ReadAllText("reddit_jokes.json");



            var records = JsonConvert.DeserializeObject<Joke>(jsonString);
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

        static void Main(string[] args)
        {

            using (Context context = new Context())
            {
                List<Joke> all = context.Jokes.ToList();
                List<Joke> chrsFiltered = all.Where(j => j.body.Length > 1000 && j.body.Length < 2000).ToList();
                noOfLinesFiltered = chrsFiltered
                    .Where(j => j.body.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList()
                        .Select(str => !string.IsNullOrWhiteSpace(str)).ToList().Count() > 3).ToList();

                context.Jokes.RemoveRange(all);
                context.SaveChanges();
                context.Jokes.AddRange(noOfLinesFiltered);
                context.SaveChanges();

            }


            using (Context context = new Context())
            {
                var All = context.Jokes.ToList();
                var filtered2 = All.Where(j => j.body.Length > 1000 && j.body.Length < 2000).ToList();
                
                var filterd6 = filtered2.Select(j => j.body.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList()).ToList();

                for (int x1 = 0; x1 < filterd6.Count(); x1++)
                {
                    filterd6[x1] = filterd6[x1].Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

                }

                 filtered7 = filterd6.Where(i => i.Count() > 3).ToList();
            }

            int x = 0;

         //   var ext = noOfLinesFiltered.Except(filtered7);


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

    public class Joke
    {
        public string body { get; set; }
        public string id { get; set; }
        public int score { get; set; }
        public string title { get; set; }
        public int no_of_charachter { get; set; }

    }
}
