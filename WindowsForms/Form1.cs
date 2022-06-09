using Google.Cloud.TextToSpeech.V1;
//using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            SpeechToText("test1.wav");


            //var x = dbData("test1.mp3");

            //var tempasdfsd = 0;

            // string command = $"-i \"video.mp4\" -vf drawtext=\"fontfile=Arial.ttf:text=hello:x=mod(t*100\\,w):y=(x*h)/w\" \"video1.mp4\" ";
            // string command = File.ReadAllText("temp.txt");
            //string command = "-i video.mp4 -filter_complex \"drawtext=text='Summer Video':fontfile=Arial.ttf:x=0:y=0:enable='between(t,2,6)',fade=t=in:start_time=2.0:d=0.5:alpha=1,fade=t=out:start_time=5.5:d=0.5:alpha=1\" -c:a copy video1.mp4";
            // ExecuteFFMpeg(command);

            //[fg];[0][fg]overlay=format=(auto),format=(yuv420p)

            // Json();
            //Json1();

            //string text = File.ReadAllText("story.txt");

            //SynthesizeText(text, "test2.mp3");

            //var story = File.ReadAllText("story1.txt");

            //var temo = 0;

            //var xx = story.Split('\n');

            //var sdfsdfds = 0;


            var dbdata = dbData("audiofiles/audio.mp3");

            GenerateTextFileForFfmpeg(dbdata, 20, "textfiles/input.txt");
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


                    stringBuilder.AppendLine($"file 'animation_images/{random.Next(1,6)}.png'");
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

            File.WriteAllText("textfiles/input.txt", stringBuilder.ToString());

            ExecuteFFMpeg("-f concat -i textfiles/input.txt -vsync vfr -pix_fmt yuv420p videofiles/output.mp4");


            var sdfsdf = 0;
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
    }
}



//