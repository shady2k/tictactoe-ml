using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace tictactoe_ml
{
    public class Board
    {
        int xStep;
        int yStep;

        int frameHeight;
        int frameWidth;

        int frameWidthSize;
        int frameHeightSize;

        public bool NeedSync = false;
        Bitmap img;
        string[] board = new string[9];

        public Board(int frameWidthSize, int frameHeightSize)
        {
            this.frameWidthSize = frameWidthSize;
            this.frameHeightSize = frameHeightSize;

            GenerateBoard();
        }
        public string[] GetBoard()
        {
            return board.ToArray();
        }
        public void GenerateBoard()
        {
            board = new string[9];

            xStep = (frameHeightSize - 1) / 3;
            yStep = (frameWidthSize - 1) / 3;

            frameHeight = yStep * 3;
            frameWidth = xStep * 3;

            img = new Bitmap(frameWidthSize, frameHeightSize);
            using (Graphics graph = Graphics.FromImage(img))
            {
                graph.FillRectangle(Brushes.White, 0, 0, frameWidth, frameHeight);

                for (int i = 0; i <= 3; i++)
                {
                    graph.DrawLine(Pens.Black, i * xStep, 0, i * xStep, frameHeight);
                }
                for (int i = 0; i <= 3; i++)
                {
                    graph.DrawLine(Pens.Black, 0, i * yStep, frameWidth, i * yStep);
                }
            }
            NeedSync = true;
        }
        public List<int> GetFreeCells()
        {
            List<int> res = new List<int>();
            for(int i = 0; i < board.Length; i++)
            {
                if(board[i] == null) res.Add(i);
            }
            return res;
        }
        public bool CheckMove(int cell)
        {
            try
            {
                if (board[cell] != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            } catch
            {
                return false;
            }
        }
        private void DrawWin(int startCell, int endCell)
        {
            Point startCellPoint = GetXYFromCell(startCell);
            Point endCellPoint = GetXYFromCell(endCell);

            if(startCellPoint.Y != endCellPoint.Y && startCellPoint.X != endCellPoint.X)
            {
                if (startCellPoint.Y > endCellPoint.Y)
                {
                    startCellPoint.X += (xStep / 4);
                    endCellPoint.X += (xStep / 2) + (xStep / 4);
                    startCellPoint.Y += (yStep / 2) + (yStep / 4);
                    endCellPoint.Y += (yStep / 4);
                } else
                {
                    startCellPoint.X += (xStep / 4);
                    endCellPoint.X += (xStep / 2) + (xStep / 4);
                    startCellPoint.Y += (yStep / 4);
                    endCellPoint.Y += (yStep / 2) + (yStep / 4);
                }
            }
            else if (startCellPoint.Y == endCellPoint.Y)
            {
                startCellPoint.X += (xStep / 4);
                endCellPoint.X += (xStep / 2) + (xStep / 4);
                startCellPoint.Y += (yStep / 2);
                endCellPoint.Y += (yStep / 2);
            } 
            else if (startCellPoint.Y != endCellPoint.Y)
            {
                startCellPoint.X += (xStep / 2);
                endCellPoint.X += (xStep / 2);
                startCellPoint.Y += (yStep / 4);
                endCellPoint.Y += (yStep / 2) + (yStep / 4);
            }


            try
            {
                using (Graphics graph = Graphics.FromImage(img))
                {
                    Pen tPen = new Pen(Color.Blue, 3);
                    graph.DrawLine(tPen, startCellPoint, endCellPoint);
                }
            } catch
            {

            }
            NeedSync = true;
        }
        public Utils.GameEnd CheckEnd()
        {
            if (board[0] != null && board[0] == board[1] && board[1] == board[2])
            {
                DrawWin(0, 2);
                if (board[0] == "X")
                {
                    return Utils.GameEnd.PlayerXWin;
                } else
                {
                    return Utils.GameEnd.PlayerOWin;
                }
            }
            if (board[3] != null && board[3] == board[4] && board[4] == board[5])
            {
                DrawWin(3, 5);
                if (board[3] == "X")
                {
                    return Utils.GameEnd.PlayerXWin;
                }
                else
                {
                    return Utils.GameEnd.PlayerOWin;
                }
            }
            if (board[6] != null && board[6] == board[7] && board[7] == board[8])
            {
                DrawWin(6, 8);
                if (board[6] == "X")
                {
                    return Utils.GameEnd.PlayerXWin;
                }
                else
                {
                    return Utils.GameEnd.PlayerOWin;
                }
            }

            if (board[0] != null && board[0] == board[3] && board[3] == board[6])
            {
                DrawWin(0, 6);
                if (board[0] == "X")
                {
                    return Utils.GameEnd.PlayerXWin;
                }
                else
                {
                    return Utils.GameEnd.PlayerOWin;
                }
            }
            if (board[1] != null && board[1] == board[4] && board[4] == board[7])
            {
                DrawWin(1, 7);
                if (board[1] == "X")
                {
                    return Utils.GameEnd.PlayerXWin;
                }
                else
                {
                    return Utils.GameEnd.PlayerOWin;
                }
            }
            if (board[2] != null && board[2] == board[5] && board[5] == board[8])
            {
                DrawWin(2, 8);
                if (board[2] == "X")
                {
                    return Utils.GameEnd.PlayerXWin;
                }
                else
                {
                    return Utils.GameEnd.PlayerOWin;
                }
            }

            if (board[0] != null && board[0] == board[4] && board[4]==board[8])
            {
                DrawWin(0, 8);
                if (board[4] == "X")
                {
                    return Utils.GameEnd.PlayerXWin;
                }
                else
                {
                    return Utils.GameEnd.PlayerOWin;
                }
            }
            if (board[2] != null && board[2] == board[4] && board[4] == board[6])
            {
                DrawWin(6, 2);
                if (board[4] == "X")
                {
                    return Utils.GameEnd.PlayerXWin;
                }
                else
                {
                    return Utils.GameEnd.PlayerOWin;
                }
            }

            if (GetFreeCells().Count() == 0)
            {
                return Utils.GameEnd.Draw;
            }

            return Utils.GameEnd.None;
        }
        public int GetCellFromXY(int x, int y)
        {
            int cell = 0;
            int cellX = (x / xStep);
            int cellY = (y / yStep);

            if(cellY == 0)
            {
                cell = cellX;
            } else if (cellY == 1)
            {
                cell = cellX + 3;
            }
            else if (cellY == 2)
            {
                cell = cellX + 6;
            }

            return cell;
        }
        public Point GetXYFromCell(int cell)
        {
            int x = 0;
            int y = 0;
            if (cell >= 0 && cell <= 2)
            {
                x = cell;
                y = 0;
            }
            if (cell >= 3 && cell <= 5)
            {
                x = cell - 3;
                y = 1;
            }
            if (cell >= 6 && cell <= 8)
            {
                x = cell - 6;
                y = 2;
            }

            return new Point(x * xStep, y * yStep);
        }
        public void PlaceSign(int cell, string sign)
        {
            board[cell] = sign;
            Point xy = GetXYFromCell(cell);
            DrawSymbol(xy.X, xy.Y, sign);
        }
        private void DrawSymbol(int x, int y, string symbol)
        {
            try
            {
                using (Graphics graph = Graphics.FromImage(img))
                {
                    graph.DrawString(symbol, new Font("Arial", 60), Brushes.Black, new Point(x, y));
                }
            }
            catch { }
            NeedSync = true;
        }
        public Bitmap GetBoardImage()
        {
            NeedSync = false;
            return img;
        }
    }
}

