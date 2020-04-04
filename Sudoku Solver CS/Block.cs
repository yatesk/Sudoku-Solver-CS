using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Sudoku_Solver_CS
{
    class Block
    {
        static public SpriteFont numberFont;
        static public SpriteFont candidateFont;

        static private Texture2D revealedBlockImage;
        static private Texture2D invalidNumberImage;

        public int number;
        public bool validNumber;
        public List<int> candidates = new List<int>();
        public bool revealed;

        public Vector2 position;

        // better way?  Used to determine which grid block is clicked.
        public static int[] gridCoords = new int[] { 42, 129, 216, 306, 393, 480, 570, 657, 744, 831 };

        public static int Size = 84;

        public Block(Vector2 _position, bool _revealed, int _number = 0)
        {
            position = _position;
            revealed = _revealed;
            number = _number;
            validNumber = false;
        }

        // Draw Text Number, canadidate numbers, and background color.
        public void Draw(SpriteBatch spriteBatch)
        {
            if (revealed == true)
            {
                spriteBatch.Draw(revealedBlockImage, position);
            }

            if (number != 0)
            {
                // Kind of centered.
                spriteBatch.DrawString(numberFont, number.ToString(), new Vector2(position.X + 20, position.Y + 2), Color.Black);

                // Draws red circle if the number is invalid.
                if (!validNumber && number != 0)
                {
                    spriteBatch.Draw(invalidNumberImage, new Vector2(position.X + Size - 24, position.Y + Size - 24));
                }
            }
            else
            {
                foreach (int item in candidates)
                {
                    spriteBatch.DrawString(candidateFont, item.ToString(), new Vector2(position.X + 5 + (((item - 1) % 3) * 30), position.Y + ((item - 1) / 3) * 30), Color.Black);
                }
            }
        }

        public void AddOrRemoveCandidate(int _x, int _y)
        {
            int relativeX = _x - (int)position.X;
            int relativeY = _y - (int)position.Y;

            int row = 0;
            int column = 0;

            if (relativeX < 28)
                column = 1;
            else if (relativeX < 56)
                column = 2;
            else if (relativeX < 84)
                column = 3;
            else
                return;

            if (relativeY < 28)
                row = 1;
            else if (relativeY < 56)
                row = 2;
            else if (relativeY < 84)
                row = 3;
            else
                return;

            int whichCanidate = column + (row - 1) * 3;

            if (candidates.Contains(whichCanidate))
            {
                candidates.Remove(whichCanidate);
            }
            else
            {
                candidates.Add(whichCanidate);
            }
        }

        public void ChangeNumberWithMouse(int _x, int _y)
        {
            int relativeX = _x - (int)this.position.X;
            int relativeY = _y - (int)this.position.Y;

            int column = 0;
            int row = 0;

            if (relativeX < 28)
                column = 1;
            else if (relativeX < 56)
                column = 2;
            else if (relativeX < 84)
                column = 3;
            else
                return;

            if (relativeY < 28)
                row = 1;
            else if (relativeY < 56)
                row = 2;
            else if (relativeY < 84)
                row = 3;
            else
                return;

            number = column + (row - 1) * 3;
        }

        public static Tuple<int, int> WhichBlock(int _x, int _y)
        {
            int xIndex = -1;
            int yIndex = -1;

            for (int i = 0; i < gridCoords.Length - 1; i++)
            {
                if (_x >= gridCoords[i] && _x <= gridCoords[i + 1])
                {
                    xIndex = i;
                }
            }

            for (int i = 0; i < gridCoords.Length - 1; i++)
            {
                if (_y >= gridCoords[i] && _y <= gridCoords[i + 1])
                {
                    yIndex = i;
                }
            }

            // If out of bounds return (-1, -1) or else return correct value.
            if (xIndex < 0 || xIndex > 8 || yIndex < 0 || yIndex > 8)
            {
                return new Tuple<int, int>(-1, -1);
            }
            else
            {
                return new Tuple<int, int>(xIndex, yIndex);
            }
        }

        public void AddAllCandidates()
        {
            for (int i = 1; i < 10; i++)
            {
                if (!candidates.Contains(i))
                    candidates.Add(i);
            }
        }

        public static void LoadContent()
        {
            revealedBlockImage = Board.content.Load<Texture2D>("revealedBlock");
            invalidNumberImage = Board.content.Load<Texture2D>("invalidNumber");
            numberFont = Board.content.Load<SpriteFont>("numberFont");
            candidateFont = Board.content.Load<SpriteFont>("candidateFont");
        }
    }
}