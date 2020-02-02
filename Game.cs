using System;
using System.Collections.Generic;
using System.Drawing;

namespace tictactoe_ml
{
    public class Game
    {
        public enum GameState
        {
            ChooseTurn,
            PlayerXTurn,
            PlayerOTurn,
            CheckAndTransferToPlayerX,
            CheckAndTransferToPlayerO,
            PlayerXWin,
            PlayerOWin,
            Draw,
            GameEnd,
            Unknown
        }

        Board board;
        Chat chat;
        GameState gameState;
        Player playerX;
        Player playerO;
        int iterations = 0;
        int humanWin = 0;
        int agentWin = 0;
        bool isNeedResultSync = true;

        public Game(int frameWidth, int frameHeight)
        {
            board = new Board(frameWidth, frameHeight);
            chat = new Chat();
            ChangeGameState(GameState.ChooseTurn);

        }
        public string GetResult()
        {
            return "Счет: " + humanWin.ToString() + " (человек) : " + agentWin.ToString() + " (агент)";
        }
        private void setHumanWinResult()
        {
            humanWin++;
            isNeedResultSync = true;
        }
        private void setAgentWinResult()
        {
            agentWin++;
            isNeedResultSync = true;
        }
        public void SetIterations(int i)
        {
            iterations = i;
        }
        private void ChangeGameState(GameState _gameState)
        {
            gameState = _gameState;
            chat.AddDebugMessage(gameState.ToString());

            switch (gameState)
            {
                case GameState.PlayerXTurn:
                    if(playerX.MakeMove(this))
                    {
                        ChangeGameState(GameState.CheckAndTransferToPlayerO);
                    }
                    break;
                case GameState.PlayerOTurn:
                    if (playerO.MakeMove(this))
                    {
                        ChangeGameState(GameState.CheckAndTransferToPlayerX);
                    }
                    break;
                case GameState.CheckAndTransferToPlayerO:
                    if (!CheckEnd())
                    {
                        ChangeGameState(GameState.PlayerOTurn);
                    }
                    break;
                case GameState.CheckAndTransferToPlayerX:
                    if (!CheckEnd())
                    {
                        ChangeGameState(GameState.PlayerXTurn);
                    }
                    break;
                case GameState.ChooseTurn:
                    if (iterations > 0)
                    {
                        chat.AddBotMessage("------------------------------------------------------------");
                        chat.AddBotMessage("Итерация " + iterations.ToString());
                        chat.AddBotMessage("------------------------------------------------------------");
                        Console.WriteLine("Итерация " + iterations.ToString());
                        iterations--;
                        HumanChoose("S");
                    }
                    else
                    {
                        chat.AddBotMessage("Будешь ходить крестиками или ноликами? А еще я могу поиграть сам с собой.");
                    }
                    break;
                case GameState.GameEnd:
                    chat.AddBotMessage("Игра окончена! Еще разок?");
                    chat.AddBotMessage("------------------------------------------------------------");
                    ChangeGameState(GameState.ChooseTurn);
                    break;
            }
        }
        private void NotifyPlayersAboutGameResult(Utils.GameEnd res)
        {
            switch(res)
            {
                case Utils.GameEnd.PlayerOWin:
                    if (playerO.isHuman()) setHumanWinResult();
                    if (!playerO.isHuman()) setAgentWinResult();
                    playerO.Win();
                    playerX.Loose();
                    break;
                case Utils.GameEnd.PlayerXWin:
                    if (playerX.isHuman()) setHumanWinResult();
                    if (!playerX.isHuman()) setAgentWinResult();
                    playerX.Win();
                    playerO.Loose();
                    break;
                case Utils.GameEnd.Draw:
                    playerX.Draw();
                    playerO.Draw();
                    break;
            }
        }
        private bool CheckEnd()
        {
            var res = board.CheckEnd();
            if (res == Utils.GameEnd.None)
            {
                return false;
            }
            else
            {
                NotifyPlayersAboutGameResult(res);

                if (!playerX.isHuman() && !playerO.isHuman())
                {
                    if (res == Utils.GameEnd.Draw)
                    {
                        chat.AddBotMessage("У меня ничья.");
                    }
                    else if (res == Utils.GameEnd.PlayerXWin)
                    {
                        chat.AddBotMessage("Отлично поиграл, крестики выиграли.");
                    }
                    else if (res == Utils.GameEnd.PlayerOWin)
                    {
                        chat.AddBotMessage("Отлично поиграл, нолики выиграли.");
                    }
                    else
                    {
                        chat.AddBotMessage("Произошла какая-то ошибка :(");
                    }
                }
                else
                {
                    if ((res == Utils.GameEnd.PlayerXWin && playerX.isHuman()) ||
                        (res == Utils.GameEnd.PlayerOWin && playerO.isHuman()))
                    {
                        chat.AddBotMessage("Поздравляю! Вы выиграли!");
                    }
                    else if ((res == Utils.GameEnd.PlayerXWin && !playerX.isHuman()) ||
                        (res == Utils.GameEnd.PlayerOWin && !playerO.isHuman()))
                    {
                        chat.AddBotMessage("Ну ничего, в следующий раз вам повезет.");
                    }
                    else if (res == Utils.GameEnd.Draw)
                    {
                        chat.AddBotMessage("Отлично сыграли, у нас ничья.");
                    }
                    else
                    {
                        chat.AddBotMessage("Произошла какая-то ошибка :(");
                    }
                }

                ChangeGameState(GameState.GameEnd);
                return true;
            }
        }
        public List<int> GetPossibleMoves()
        {
            return board.GetFreeCells();
        }
        public void HumanChoose(string sign)
        {
            disposePlayers();
            board.GenerateBoard();
            if (sign == "X")
            {
                playerO = new AgentPlayer(Utils.PlayerSide.O, 0.1);
                playerX = new HumanPlayer(Utils.PlayerSide.X);
            }
            else if (sign == "O")
            {
                playerO = new HumanPlayer(Utils.PlayerSide.O);
                playerX = new AgentPlayer(Utils.PlayerSide.X, 0.1);
            }
            else if (sign == "S" || sign == "SM")
            {
                playerO = new AgentPlayer(Utils.PlayerSide.O, 0.3);
                playerX = new AgentPlayer(Utils.PlayerSide.X, 0.8);
            }
            else
            {
                ChangeGameState(GameState.Unknown);
                return;
            }
            ChangeGameState(GameState.PlayerXTurn);
        }
        public bool HumanMakeMove(int x, int y)
        {
            if (gameState == GameState.PlayerXTurn && playerX.isHuman())
            {
                if (playerX.MakeMove(x, y, this))
                {
                    ChangeGameState(GameState.CheckAndTransferToPlayerO);
                }
            }
            if (gameState == GameState.PlayerOTurn && playerO.isHuman())
            {
                if (playerO.MakeMove(x, y, this))
                {
                    ChangeGameState(GameState.CheckAndTransferToPlayerX);
                }
            }
            return false;
        }
        public ChatResponse SendPlayerMessage(string text)
        {
            if (gameState == GameState.ChooseTurn)
            {
                return chat.SendPlayerMessage(text);
            }
            else
            {
                return new ChatResponse(Utils.GameAction.Unknown);
            }
        }
        public void SendUnknownAnswer()
        {
            chat.SendUnknownAnswer();
        }
        public Bitmap GetBoardImage()
        {
            return board.GetBoardImage();
        }
        public bool IsNeedBoardSync()
        {
            return board.NeedSync;
        }
        public List<string> GetChatLog()
        {
            return chat.GetChatLog();
        }
        public bool IsNeedChatSync()
        {
            return chat.NeedSync;
        }
        public bool IsNeedResultSync()
        {
            return isNeedResultSync;
        }
        public int GetBoardCellFromXY(int x, int y)
        {
            return board.GetCellFromXY(x, y);
        }
        public bool CheckMove(int cell)
        {
            return board.CheckMove(cell);
        }
        public void AddPlayerMessage(string text)
        {
            chat.AddPlayerMessage(text);
        }
        public void AddBotMessage(string text)
        {
            chat.AddBotMessage(text);
        }
        public void AddDebugMessage(string text)
        {
            chat.AddDebugMessage(text);
        }
        public string[] GetBoard()
        {
            return board.GetBoard();
        }
        public void PlaceSign(int cell, string sign)
        {
            if (board.CheckMove(cell))
            {
                board.PlaceSign(cell, sign);
            } else
            {
                if ((gameState == GameState.PlayerXTurn || gameState == GameState.PlayerOTurn))
                {
                    if (playerX.isHuman())
                    {
                        return;
                    }
                    else
                    {
                        chat.AddBotMessage("Выполяется недопустимый ход в ячейку " + (cell + 1).ToString());
                        chat.AddBotMessage("Что-то пошло не так...");
                        ChangeGameState(GameState.GameEnd);
                    }
                }
            }
        }
        private void disposePlayers()
        {
            if (playerO != null)
            {
                playerO.Dispose();
                playerO = null;
            }
            if (playerX != null)
            {
                playerX.Dispose();
                playerX = null;
            }
        }
        public void ExitGame()
        {
            disposePlayers();
        }
    }
}
