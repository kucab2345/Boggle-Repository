﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoggleAPIClient;

namespace BoggleClient
{
    public class Controller
    {
        BoggleModel mainClient;
        private GameInterface game;
        public Controller(GameInterface view)
        { 
            game = view;
            game.CreateGameEvent += CreateGameHandler;
            game.CancelGameEvent += CancelGameHandler;
        }
        private async void CreateGameHandler(string nickname, string timeLimit, string server)
        {
            mainClient = new BoggleModel(server);
            int gameTime;
            Task createUser = new Task (() =>mainClient.createUser(nickname));
            createUser.Start();
            int.TryParse(timeLimit, out gameTime);
            createUser.Wait();
            Task createGame = new Task(() => mainClient.createGame(gameTime));
            createGame.Start();
            game.Player1Name = mainClient.nickname;
            createGame.Wait();
            game.cancelbutton = true;
        }
        private async void CancelGameHandler()
        {
            Task cancelGame = new Task(() =>mainClient.cancelJoinRequest());
            await cancelGame;
            game.cancelbutton = false;
        }
    }
}
