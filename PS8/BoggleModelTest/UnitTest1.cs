using System;
using BoggleAPIClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoggleModelTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            BoggleModel test = new BoggleModel("http://bogglecs3500s16.azurewebsites.net/BoggleService.svc/");
            test.createUser("Joe");
            test.createGame(50);
            test.cancelJoinRequest();
        }
    }
}
