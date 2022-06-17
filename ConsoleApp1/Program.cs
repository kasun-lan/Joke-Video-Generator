using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        private static List<List<string>> filtered7;
        private static List<Joke> noOfLinesFiltered;

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
