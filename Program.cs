﻿namespace WoFApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // var gm = new GameManager();
            // gm.StartGame();
            var game = new GameController();
            game.GameStart();
        }
    }
}
