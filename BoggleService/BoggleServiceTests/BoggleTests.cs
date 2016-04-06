using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Dynamic;
using static System.Net.HttpStatusCode;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading;

namespace Boggle
{
    /// <summary>
    /// Provides a way to start and stop the IIS web server from within the test
    /// cases.  If something prevents the test cases from stopping the web server,
    /// subsequent tests may not work properly until the stray process is killed
    /// manually.
    /// </summary>
    public static class IISAgent
    {
        // Reference to the running process
        private static Process process = null;

        /// <summary>
        /// Starts IIS
        /// </summary>
        public static void Start(string arguments)
        {
            if (process == null)
            {
                ProcessStartInfo info = new ProcessStartInfo(Properties.Resources.IIS_EXECUTABLE, arguments);
                info.WindowStyle = ProcessWindowStyle.Minimized;
                info.UseShellExecute = false;
                process = Process.Start(info);
            }
        }

        /// <summary>
        ///  Stops IIS
        /// </summary>
        public static void Stop()
        {
            if (process != null)
            {
                process.Kill();
            }
        }
    }
    [TestClass]
    public class BoggleTests
    {
        /// <summary>
        /// This is automatically run prior to all the tests to start the server
        /// </summary>
        [ClassInitialize()]
        public static void StartIIS(TestContext testContext)
        {
            IISAgent.Start(@"/site:""BoggleService"" /apppool:""Clr4IntegratedAppPool"" /config:""..\..\..\.vs\config\applicationhost.config""");
        }

        /// <summary>
        /// This is automatically run when all tests have completed to stop the server
        /// </summary>
        [ClassCleanup()]
        public static void StopIIS()
        {
            IISAgent.Stop();
        }

        private RestTestClient client = new RestTestClient("http://localhost:60000/");
        
        /// <summary>
        /// Attempts to create users with invalid name. Expects forbidden stats
        /// </summary>
        [TestMethod]
        public void TestMethod0()
        {
            dynamic p1 = new ExpandoObject();
            p1.Nickname = "    ";

            Response r1 = client.DoPostAsync("/users", p1).Result;

            Assert.AreEqual(Forbidden, r1.Status);

        }
        /// <summary>
        /// More invalid types of names. Expects Forbidden Status 
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            dynamic p1 = new ExpandoObject();
            p1.Nickname = "\t\t\n";

            Response r1 = client.DoPostAsync("/users", p1).Result;

            Assert.AreEqual(Forbidden, r1.Status);

        }
        /// <summary>
        /// Master Test. Creates two users, pits them in a game, has them play all possible words, as well as repeats, and invalids,
        /// waits 11 seconds to ensure the 10 second game infact ends, and checks to see that the GameState is actually completed.
        /// Mimics a full game, and code coverage can vary depending on the board that gets created. Possible words change per test run
        /// and as a result, code coverage can vary slightly. Multiple asserts throughout the code to ensure correct status codes are being handed back.
        /// </summary>
        [TestMethod]
        public void TestMethod3()
        {
            dynamic p1 = new ExpandoObject();
            dynamic p2 = new ExpandoObject();
            dynamic game = new ExpandoObject();
            p1.Nickname = "Mark";
            p2.Nickname = "Bob";

            Response r1 = client.DoPostAsync("/users", p1).Result;
            Response r2 = client.DoPostAsync("/users", p2).Result;

            p1.UserToken = r1.Data.UserToken;
            p2.UserToken = r2.Data.UserToken;

            p1.TimeLimit = "10";
            p2.TimeLimit = "10";

            r1 = client.DoPostAsync("/games", p1).Result;
            r2 = client.DoPostAsync("/games", p2).Result;
            
            Assert.AreEqual(Accepted, r1.Status);
            Assert.AreEqual(Created, r2.Status);

            p1.GameID = r1.Data.GameID;
            p2.GameID = r2.Data.GameID;

            game = client.DoGetAsync("/games/" + p1.GameID).Result;
            BoggleBoard board = new BoggleBoard(game.Data.Board.ToString());

            HashSet<string> testDictionary = new HashSet<string>();
            List<string> potentialWords = new List<string>();
            foreach (string i in File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "../../../\\dictionary.txt"))
            {
                testDictionary.Add(i);
                if (board.CanBeFormed(i))
                {
                    potentialWords.Add(i);
                }
            }
            Random rand = new Random();

            dynamic p1words = new ExpandoObject();
            dynamic p2words = new ExpandoObject();

            p1words.UserToken = p1.UserToken;
            p2words.UserToken = p2.UserToken;

            p1words.Word = " ";
            r1 = client.DoPutAsync(p1words, "games/" + p1.GameID).Result;
            p1words.Word = null;
            r1 = client.DoPutAsync(p1words, "games/" + p1.GameID).Result;
            p1words.Word = "kkkkkkkkkkkkk";
            r1 = client.DoPutAsync(p1words, "games/" + p1.GameID).Result;
            p1words.Word = "kkkkkkkkkkkkk";
            r1 = client.DoPutAsync(p1words, "games/" + p1.GameID).Result;

            for (int i = 0; i < potentialWords.Count + 4; i++)
            {
                p1.Word = potentialWords[rand.Next(0,potentialWords.Count)];
                p2.Word = potentialWords[rand.Next(0, potentialWords.Count)];
                p1words.Word = p1.Word;
                p2words.Word = p2.Word;
                r1 = client.DoPutAsync(p1words, "games/" + p1.GameID).Result;
                r2 = client.DoPutAsync(p2words, "games/" + p2.GameID).Result;
                Assert.AreEqual(OK, r1.Status);
                Assert.AreEqual(OK, r2.Status);
            }

            dynamic gameBrief = new ExpandoObject();

            gameBrief = client.DoGetAsync("/games/" + p1.GameID + "?Brief=yes").Result;
            Assert.AreEqual(OK, gameBrief.Status);

            Thread.Sleep(11000);

            dynamic endResult = new ExpandoObject();
            gameBrief = client.DoGetAsync("/games/" + p1.GameID).Result;
            Assert.AreEqual("completed", (string)gameBrief.Data.GameState);
        }
        /// <summary>
        /// Creates two games and runs them simulatenously on the server. Similar to TestMethod3, but with 2 concurrent games
        /// MUST WAIT 10 SECONDS FOR GAMETIMER TO RUN OUT! DELAY IN EXECUTION IS EXPECTED
        /// </summary>
        [TestMethod]
        public void TestMethod4()
        {
            IISAgent.Start(@"/site:""BoggleService"" /apppool:""Clr4IntegratedAppPool"" /config:""..\..\..\.vs\config\applicationhost.config""");

            dynamic p3 = new ExpandoObject();
            dynamic p4 = new ExpandoObject();
            dynamic game2 = new ExpandoObject();
            p3.Nickname = "John";
            p4.Nickname = "June";

            Response r3 = client.DoPostAsync("/users", p3).Result;
            Response r4 = client.DoPostAsync("/users", p4).Result;

            p3.UserToken = r3.Data.UserToken;
            p4.UserToken = r4.Data.UserToken;

            p3.TimeLimit = "10";
            p4.TimeLimit = "10";

            r3 = client.DoPostAsync("/games", p3).Result;
            r4 = client.DoPostAsync("/games", p4).Result;

            Assert.AreEqual(Accepted, r3.Status);
            Assert.AreEqual(Created, r4.Status);

            p3.GameID = r3.Data.GameID;
            p4.GameID = r4.Data.GameID;

            game2 = client.DoGetAsync("/games/" + p3.GameID).Result;
            BoggleBoard board2 = new BoggleBoard(game2.Data.Board.ToString());

            /////////////////////////////////////////////////////////////////////////////////////////////

            dynamic p5 = new ExpandoObject();
            dynamic p6 = new ExpandoObject();
            dynamic game3 = new ExpandoObject();
            p5.Nickname = "Jack";
            p6.Nickname = "Jill";

            Response r5 = client.DoPostAsync("/users", p5).Result;
            Response r6 = client.DoPostAsync("/users", p6).Result;

            p5.UserToken = r5.Data.UserToken;
            p6.UserToken = r6.Data.UserToken;

            p5.TimeLimit = "8";
            p6.TimeLimit = "8";

            r5 = client.DoPostAsync("/games", p5).Result;
            r6 = client.DoPostAsync("/games", p6).Result;

            Assert.AreEqual(Accepted, r5.Status);
            Assert.AreEqual(Created, r6.Status);

            p5.GameID = r5.Data.GameID;
            p6.GameID = r6.Data.GameID;

            game3 = client.DoGetAsync("/games/" + p5.GameID).Result;


            HashSet<string> testDictionary = new HashSet<string>();
            List<string> potentialWords = new List<string>();
            foreach (string i in File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "../../../\\dictionary.txt"))
            {
                testDictionary.Add(i);
                if (board2.CanBeFormed(i))
                {
                    potentialWords.Add(i);
                }
            }
            Random rand = new Random();
            dynamic p3words = new ExpandoObject();
            dynamic p4words = new ExpandoObject();
            dynamic p5words = new ExpandoObject();
            dynamic p6words = new ExpandoObject();

            p3words.UserToken = p3.UserToken;
            p4words.UserToken = p4.UserToken;
            p5words.UserToken = p5.UserToken;
            p6words.UserToken = p6.UserToken;

            for (int i = 0; i < potentialWords.Count + 4; i++)
            {
                p3.Word = potentialWords[rand.Next(0, potentialWords.Count)];
                p4.Word = potentialWords[rand.Next(0, potentialWords.Count)];
                p5.Word = potentialWords[rand.Next(0, potentialWords.Count)];
                p6.Word = potentialWords[rand.Next(0, potentialWords.Count)];
                p3words.Word = p3.Word;
                p4words.Word = p4.Word;
                p5words.Word = p5.Word;
                p6words.Word = p6.Word;
                r3 = client.DoPutAsync(p3words, "games/" + p3.GameID).Result;
                r4 = client.DoPutAsync(p4words, "games/" + p4.GameID).Result;
                r5 = client.DoPutAsync(p5words, "games/" + p5.GameID).Result;
                r6 = client.DoPutAsync(p6words, "games/" + p6.GameID).Result;
                Assert.AreEqual(OK, r3.Status);
                Assert.AreEqual(OK, r4.Status);
                Assert.AreEqual(OK, r5.Status);
                Assert.AreEqual(OK, r6.Status);
            }

            /////////////////////////////////////////////
            dynamic gameBrief2 = new ExpandoObject();

            gameBrief2 = client.DoGetAsync("/games/" + p3.GameID + "?Brief=yes").Result;
            Assert.AreEqual(OK, gameBrief2.Status);

            Thread.Sleep(11000);

            gameBrief2 = client.DoGetAsync("/games/" + p3.GameID).Result;
            Assert.AreEqual("completed", (string)gameBrief2.Data.GameState);
            ////////////////////////////////////////////////
            dynamic gameBrief3 = new ExpandoObject();

            gameBrief3 = client.DoGetAsync("/games/" + p5.GameID + "?Brief=yes").Result;
            Assert.AreEqual(OK, gameBrief3.Status);

            gameBrief3 = client.DoGetAsync("/games/" + p5.GameID).Result;
            Assert.AreEqual("completed", (string)gameBrief3.Data.GameState);

        }
        /// <summary>
        /// Creates a player, puts him into a pending game, and cancels the game.
        /// Attempts to create a new game with more invalid parameters, such as negative timelimits and such.
        /// </summary>
        [TestMethod]
        public void TestMethod2()
        {
            dynamic p1 = new ExpandoObject();
            p1.Nickname = "cancelGuy";

            Response r1 = client.DoPostAsync("/users", p1).Result;
            p1.UserToken = r1.Data.UserToken;
            p1.TimeLimit = "10";
            r1 = client.DoPostAsync("/games", p1).Result;
            Assert.AreEqual(Accepted, r1.Status);

            r1 = client.DoPutAsync(p1, "/games").Result;
            Assert.AreEqual(OK, r1.Status);

            p1.TimeLimit = "-2";
            r1 = client.DoPostAsync("/games", p1).Result;
            Assert.AreEqual(Forbidden, r1.Status);

            p1.TimeLimit = null;
            r1 = client.DoPostAsync("/games", p1).Result;
            Assert.AreEqual(Forbidden, r1.Status);
            p1.TimeLimit = "";
            r1 = client.DoPostAsync("/games", p1).Result;
            Assert.AreEqual(Forbidden, r1.Status);
        }
    }
}
