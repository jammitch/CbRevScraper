using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RevScraper
{
    class Program
    {
        private static readonly Regex _isBanapassId = new Regex("^[0-9]{20}$");
        private static readonly Regex _isPin = new Regex("^[0-9]{4}$");

        private static string MusicScoresFilenamePattern = "{0}_MusicScores.csv";
        private static string ChallengeCoursesFilenamePattern = "{0}_ChallengeCourses.csv";
        private static string EventsFilenamePattern = "{0}_Events.csv";

        static void Main(string[] args)
        {
            Console.WriteLine($"NOTE: This program is throttled to make one web request every {RevScraperClient.ThrottleRate} seconds.");

            string banapassId = ReadString(
@"Enter your Aime / BANAPASSPORT ID.
Twenty digits without dashes or separators.",
                _isBanapassId
            );

            if (banapassId == null)
            {
                return;
            }

            string pin = ReadString(
 @"Enter your PIN.
Four digits without dashes or separators.
This will not be stored, but it WILL be visible as you type.",
                _isPin
            );

            if (pin == null)
            {
                return;
            }

            using (RevScraperClient client = new RevScraperClient())
            {
                Console.WriteLine("Logging in...");

                bool loginResult = client.Login(banapassId, pin);
                if (!loginResult)
                {
                    // TODO: Re-enter REPL.
                    Console.WriteLine("Invalid credentials. Press any key to exit.");
                    Console.Read();
                    return;
                }

                GetMusicScores(banapassId, client);
                GetChallengeCourses(banapassId, client);
                GetEvents(banapassId, client);

                Console.WriteLine("Done! Press any key to exit.");
                Console.Read();
            }
        }

        private static string ReadString(string prompt, Regex validationRegex)
        {
            string input = null;
            do
            {
                Console.WriteLine(prompt);
                input = Console.ReadLine();

                if (input == string.Empty)
                {
                    // Quit
                    return null;
                }

                if (!validationRegex.IsMatch(input))
                {
                    Console.WriteLine("Invalid input.");
                    input = null;
                }

            } while (input == null);

            return input;
        }

        private static void GetMusicScores(string banapassId, RevScraperClient client)
        {
            Console.WriteLine("Getting songs...");
            IReadOnlyCollection<Uri> songUris = client.GetSongUris();
            Console.WriteLine($"Got {songUris.Count} songs.");

            List<MusicDetail> musicDetails = new List<MusicDetail>(songUris.Count);
            int i = 1;
            foreach (Uri songUri in songUris)
            {
                Console.WriteLine($"Getting song {i} of {songUris.Count}...");
                musicDetails.Add(client.GetMusicDetail(songUri));
                i++;
            }

            string musicScoresFilename = string.Format(CultureInfo.InvariantCulture, MusicScoresFilenamePattern, banapassId);

            Console.WriteLine($"Writing scores to {musicScoresFilename}...");

            using (FileStream stream = File.Open(musicScoresFilename, FileMode.Create))
            {
                using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8))
                {
                    MusicDetail.SerializeHeadersToCSV(streamWriter);

                    foreach (MusicDetail musicDetail in musicDetails)
                    {
                        musicDetail.SerializeToCSV(streamWriter);
                    }
                }
            }
        }

        private static void GetChallengeCourses(string banapassId, RevScraperClient client)
        {
            Console.WriteLine("Getting courses...");
            IReadOnlyCollection<Uri> courseUris = client.GetCourseUris();
            Console.WriteLine($"Got {courseUris.Count} courses.");

            List<ChallengeCourse> challengeCourses = new List<ChallengeCourse>(courseUris.Count);

            int i = 1;
            foreach (Uri courseUri in courseUris)
            {
                Console.WriteLine($"Getting course {i} of {courseUris.Count}...");
                challengeCourses.Add(client.GetChallengeCourse(courseUri));
                i++;
            }

            string challengeCoursesFilename = string.Format(CultureInfo.InvariantCulture, ChallengeCoursesFilenamePattern, banapassId);

            Console.WriteLine($"Writing scores to {challengeCoursesFilename}...");

            using (FileStream stream = File.Open(challengeCoursesFilename, FileMode.Create))
            {
                using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8))
                {
                    ChallengeCourse.SerializeHeadersToCSV(streamWriter);

                    foreach (ChallengeCourse challengeCourse in challengeCourses)
                    {
                        challengeCourse.SerializeToCSV(streamWriter);
                    }
                }
            }
        }

        private static void GetEvents(string banapassId, RevScraperClient client)
        {
            Console.WriteLine("Getting events...");
            IReadOnlyCollection<Uri> eventUris = client.GetEventUris();
            Console.WriteLine($"Got {eventUris.Count} events.");

            List<Event> events = new List<Event>(eventUris.Count);

            int i = 1;
            foreach (Uri eventUri in eventUris)
            {
                Console.WriteLine($"Getting event {i} of {eventUris.Count}...");
                events.Add(client.GetEvent(eventUri));
                i++;
            }

            string eventsFilename = string.Format(CultureInfo.InvariantCulture, EventsFilenamePattern, banapassId);

            Console.WriteLine($"Writing scores to {eventsFilename}...");

            using (FileStream stream = File.Open(eventsFilename, FileMode.Create))
            {
                using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8))
                {
                    Event.SerializeHeadersToCSV(streamWriter);

                    foreach (Event eventObject in events)
                    {
                        eventObject.SerializeToCSV(streamWriter);
                    }
                }
            }
        }
    }
}
