using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace tictactoe_ml
{
    class Board
    {
        int xStep;
        int yStep;

        int frameHeight;
        int frameWidth;

        Bitmap img;

        public Board(int frameWidthSize, int frameHeightSize)
        {
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
        }
        public void PlaceX(int x, int y)
        {
            DrawSymbol((x / xStep) * xStep, (y / yStep) * yStep, "X");
        }
        public void PlaceO(int x, int y)
        {
            DrawSymbol((x / xStep) * xStep, (y / yStep) * yStep, "O");
        }
        private void DrawSymbol(int x, int y, string symbol)
        {
            using (Graphics graph = Graphics.FromImage(img))
            {
                graph.DrawString(symbol, new Font("Arial", 60), Brushes.Black, new Point(x, y));
            }
        }
        public Bitmap GetBoardImage()
        {
            return img;
        }
    }
}
