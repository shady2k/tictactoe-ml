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
        Random random = new Random();
        Player playerX;
        Player playerO;

        public Game(int frameWidth, int frameHeight)
        {
            board = new Board(frameWidth, frameHeight);
            chat = new Chat();
            ChangeGameState(GameState.ChooseTurn);

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
                    chat.AddBotMessage("Будешь ходить крестиками или ноликами? А еще я могу поиграть сам с собой.");
                    break;
                case GameState.GameEnd:
                    chat.AddBotMessage("Игра окончена! Еще разок?");
                    ChangeGameState(GameState.ChooseTurn);
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
                if (playerX.GetType().Name == "AgentPlayer" && playerO.GetType().Name == "AgentPlayer")
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
                    if ((res == Utils.GameEnd.PlayerXWin && playerX.GetType().Name == "HumanPlayer") ||
                        (res == Utils.GameEnd.PlayerOWin && playerO.GetType().Name == "HumanPlayer"))
                    {
                        chat.AddBotMessage("Поздравляю! Вы выиграли!");
                    }
                    else if ((res == Utils.GameEnd.PlayerXWin && playerX.GetType().Name == "AgentPlayer") ||
                        (res == Utils.GameEnd.PlayerOWin && playerO.GetType().Name == "AgentPlayer"))
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
            board.GenerateBoard();
            if (sign == "X")
            {
                playerO = new AgentPlayer(Utils.PlayerSide.O);
                playerX = new HumanPlayer(Utils.PlayerSide.X);
            }
            else if (sign == "O")
            {
                playerO = new HumanPlayer(Utils.PlayerSide.O);
                playerX = new AgentPlayer(Utils.PlayerSide.X);
            }
            else if (sign == "S")
            {
                playerO = new AgentPlayer(Utils.PlayerSide.O);
                playerX = new AgentPlayer(Utils.PlayerSide.X);
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
            if (gameState == GameState.PlayerXTurn && playerX.GetType().Name == "HumanPlayer")
            {
                if (playerX.MakeMove(x, y, this))
                {
                    ChangeGameState(GameState.CheckAndTransferToPlayerO);
                }
            }
            if (gameState == GameState.PlayerOTurn && playerO.GetType().Name == "HumanPlayer")
            {
                if (playerO.MakeMove(x, y, this))
                {
                    ChangeGameState(GameState.CheckAndTransferToPlayerX);
                }
            }
            return false;
        }
        public Utils.GameAction SendPlayerMessage(string text)
        {
            if (gameState == GameState.ChooseTurn)
            {
                return chat.SendPlayerMessage(text);
            }
            else
            {
                return Utils.GameAction.Unknown;
            }
        }
        public void AddUnknownMessage()
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
        public void PlaceSign(int cell, string sign)
        {
            if (board.CheckMove(cell))
            {
                board.PlaceSign(cell, sign);
            } else
            {
                if ((gameState == GameState.PlayerXTurn || gameState == GameState.PlayerOTurn))
                {
                    if (playerX.GetType().Name == "HumanPlayer")
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
    }
}
