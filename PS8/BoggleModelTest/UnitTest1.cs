//using System;
//using BoggleAPIClient;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System.Threading.Tasks;

//namespace BoggleModelTest
//{
//    [TestClass]
//    public class UnitTest1
//    {
//        [TestMethod]
//        public void TestMethod1()
//        {
//            runGame();
//        }

//        private async void runGame()
//        {
//            BoggleModel test = new BoggleModel("http://bogglecs3500s16.azurewebsites.net/BoggleService.svc/");
//            Task test2 = new Task(() => test.createUser("hehbolwabowboawognwoq"));
//            test2.Start();
//            test2.Wait();
//            Console.WriteLine("Finished result");
//        }
//    }
//}
