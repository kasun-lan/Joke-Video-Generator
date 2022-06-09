using System;
using System.Collections.Generic;
using System.Data;
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
        static void Main(string[] args)
        {



            using (Context context = new Context())
            {
                var text = context.Jokes.Where(j => j.id == "32w6d2").FirstOrDefault().body;
                var c = GetTextAnimationData(text, 15, 20);

                ApplyTextAnimationOnVideo(c, 120, 1, "");


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
            string command = "-i video.mp4 -filter_complex \"drawtext=text='Summer Video':fontfile=Arial.ttf:x=0:y=0:enable='between(t,2,6)',fade=t=in:start_time=2.0:d=0.5:alpha=1,fade=t=out:start_time=5.5:d=0.5:alpha=1\" -c:a copy video1.mp4";
            ExecuteFFMpeg(command);

            //[fg];[0][fg]overlay=format=(auto),format=(yuv420p)

            // Json();
            //Json1();

            //SynthesizeText("we are testing this", "texttts.mp3");

            var story = File.ReadAllText("story1.txt");

            var temo = 0;

            var xx = story.Split('\n');

            var sdfsdfds = 0;

        }

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


            using (Context context = new Context())
            {
                var list = context.Jokes.ToList();

                foreach (var joke in list)
                {
                    joke.no_of_charachter = joke.body.Length;
                }

                context.SaveChanges();

            }




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

        public static List<Tuple<int, int, string, int, int>> GetTextAnimationData(string text, int canvasHeight, int canvasWidth)
        {
            List<string> lines = text.Split('\n').ToList();
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

        public static void ApplyTextAnimationOnVideo(List<Tuple<int, int, string, int, int>> textData, int voiceClipPlayTime, string VideoPath)
        {
            var sectionIds = textData.Select(t => t.Item1).Distinct().ToList();

            double intervelBetweenWords = (double)voiceClipPlayTime / textData.Count();


            Dictionary<int, double> sectionsData = new Dictionary<int, double>();

            foreach (int sectionId in sectionIds)
            {
                int sdfsd = textData.Where(t => t.Item1 == sectionId).Count();
                int eefd = textData.Count();
                double sdfse = ((double)sdfsd / (double)eefd) * voiceClipPlayTime;
                sectionsData.Add(sectionId, sdfse);
            }

            double ExternelCumilatedSecounds = 0;

            foreach(int sectionId in sectionIds)
            {
                var currentSectionRelatedData = textData.Where(t => t.Item1 == sectionId).ToList();
                double startTime = cumilatedSecounds + 

            }




            foreach (var word in textData)
            {

                //timing calculations

                //  string command = $"-i video.mp4 -filter_complex \"drawtext=text='{word}':fontfile=Arial.ttf:x={}:y={}:enable='between(t,2,6)',fade=t=in:start_time=2.0:d=0.5:alpha=1,fade=t=out:start_time=5.5:d=0.5:alpha=1\" -c:a copy video1.mp4";
            }
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



        //public DataTable GetDataTable(int columns, int canvasHeight)
        //{
        //    DataTable dataTable = new();

        //    for(int x=0; x<columns; x++)
        //    {
        //        dataTable.Columns.Add(new DataColumn(x.ToString()));
        //    }
        //    for(int y=0;y<canvasHeight;y++)
        //    {
        //        dataTable.Rows.Add()
        //    }

        //    return dataTable;
        //}


        //public DataTable GetDataTableFromJoke(Joke joke,int canvasHeight, int canvasWidth)
        //{
        //    DataTable dataTable = new DataTable();

        //    for(int x=0; x<canvasWidth; x++)
        //    {
        //        dataTable.Columns.Add(new DataColumn(x.ToString()));
        //    }



        //}

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
