using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

using System.Net;

namespace Sudoku_Solver_CS
{
    class Board
    {
        public static ContentManager content;
        private readonly Random r = new Random();
        private readonly int boardMargin = 35;

        private int correctBlocks = 0;
        public Block[,] grid = new Block[9, 9];

        private Texture2D backgroundImage;
        private Vector2 backgroundPosition;

        public Button showCandidatesButton;

        public Button highlightNakedSinglesButton;
        public Button highlightHiddenSinglesButton;

        public string puzzleSource;
        public string puzzleDifficulty;

        public Board(ContentManager _content, string _source = "", string _difficulty = "")
        {
            puzzleSource = _source;
            puzzleDifficulty = _difficulty;

            content = _content;
            backgroundPosition = new Vector2(boardMargin, boardMargin);

            // Fill blockGrid
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    int gridMarginX = 7;
                    int gridMarginY = 7;

                    // better way?
                    if (i < 3)
                    {
                        gridMarginX += 3 * i;
                    }
                    else if (i < 6)
                    {
                        gridMarginX += (3 * i) + 3;  // subgrid margin - Block margin = 3
                    }
                    else if (i < 9)
                    {
                        gridMarginX += (3 * i) + 6;  // (subgrid margin - Block margin) * 2 = 6
                    }

                    if (j < 3)
                    {
                        gridMarginY += 3 * j;
                    }
                    else if (j < 6)
                    {
                        gridMarginY += (3 * j) + 3;
                    }
                    else if (j < 9)
                    {
                        gridMarginY += (3 * j) + 6;
                    }

                    grid[i, j] = new Block(new Vector2((i * 84) + boardMargin + gridMarginX, boardMargin + (j * 84) + gridMarginY), false, 0);
                }
            }

            showCandidatesButton = new Button("showCandidate", new Vector2(775, 0), content, true);

            highlightHiddenSinglesButton = new Button("highlightHiddenSingles", new Vector2(575, 0), content, true);
            highlightNakedSinglesButton = new Button("highlightNakedSingles", new Vector2(650, 0), content, true);

            NewPuzzle();
            LoadContent();
        }

        public void NewPuzzle()
        {
            ClearBoard();

            if (puzzleSource == "QQ Wing")
            {
                String line;
                List<char[]> level = new List<char[]>();

                FileStream fsSource = new FileStream(puzzleDifficulty + ".txt", FileMode.OpenOrCreate, FileAccess.Read);
                using (StreamReader sr = new StreamReader(fsSource))
                {
                    int randomNumber = r.Next(0, 21);

                    // Skip to random puzzle #0-21
                    for (int i = 0; i < randomNumber * 11; i++)
                    {
                        line = sr.ReadLine();
                    }

                    for (int i = 0; i < 9; i++)
                    {
                        line = sr.ReadLine();

                        for (int j = 0; j < 9; j++)
                        {
                            if (line[j] != '.')
                            {
                                grid[i, j].number = (int)char.GetNumericValue(line[j]);
                                grid[i, j].revealed = true;
                                grid[i, j].validNumber = true;
                                correctBlocks++;
                            }
                            else
                            {
                                grid[i, j].number = 0;
                                grid[i, j].revealed = false;
                                grid[i, j].validNumber = false;
                            }
                        }
                    }
                }
            }
        }

        public void LoadContent()
        {
            backgroundImage = content.Load<Texture2D>("background");

            Block.LoadContent();
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(backgroundImage, backgroundPosition);

            // Draw grid
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    grid[i, j].Draw(spriteBatch);

            if (highlightNakedSinglesButton.toggle)
                HighlightNakedSingles(spriteBatch);

            if (highlightHiddenSinglesButton.toggle)
                HighlightHiddenSingles(spriteBatch);

            spriteBatch.DrawString(Block.candidateFont, puzzleDifficulty, new Vector2(boardMargin, 0), Color.Black);
            spriteBatch.DrawString(Block.candidateFont, puzzleSource, new Vector2(boardMargin + 150, 0), Color.Black);

            showCandidatesButton.Draw(spriteBatch);

            highlightNakedSinglesButton.Draw(spriteBatch);
            highlightHiddenSinglesButton.Draw(spriteBatch);
        }

        public void ClearBoard()
        {
            correctBlocks = 0;
            showCandidatesButton.toggle = false;
            highlightNakedSinglesButton.toggle = false;
            highlightHiddenSinglesButton.toggle = false;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    grid[i, j].number = 0;
                    grid[i, j].revealed = false;
                    grid[i, j].validNumber = false;
                    grid[i, j].candidates.Clear();
                }
            }
        }

        public void ValidOrInvalidNumber(int _x, int _y)
        {
            // check horizontal
            int count = 0;
            bool valid = true;

            for (int i = 0; i < 9; i++)
            {
                if (grid[i, _y].number == grid[_x, _y].number)
                {
                    count++;
                }
            }

            if (count != 1)
                valid = false;

            // check vertical
            count = 0;

            for (int i = 0; i < 9; i++)
            {
                if (grid[_x, i].number == grid[_x, _y].number)
                    count++;
            }

            if (count != 1)
                valid = false;

            // check subGrid
            int[] subGridStartingCoords = FindSubGrid(_x, _y);
            count = 0;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (grid[i + subGridStartingCoords[0], j + subGridStartingCoords[1]].number == grid[_x, _y].number)
                        count++;
                }
            }

            if (count != 1)
                valid = false;

            if (grid[_x, _y].validNumber)
            {
                if (!valid)
                {
                    correctBlocks--;
                    grid[_x, _y].validNumber = false;
                }
            }
            else
            {
                if (valid)
                {
                    correctBlocks++;
                    grid[_x, _y].validNumber = true;
                }
            }

            // MOVE???
            if (showCandidatesButton.toggle)
                ShowCandidates();
        }

        public void ShowCandidates()
        {
            // refactor
            // which candidates to remove
            HashSet<int> candidates = new HashSet<int>();

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    candidates.Clear();
                    grid[i, j].AddAllCandidates();

                    for (int column = 0; column < 9; column++)
                    {
                        if (grid[column, j].number != 0)
                            candidates.Add(grid[column, j].number);
                    }

                    for (int row = 0; row < 9; row++)
                    {
                        if (grid[i, row].number != 0)
                            candidates.Add(grid[i, row].number);
                    }

                    int[] subGridStartingCoords = FindSubGrid(i, j);

                    for (int column = 0; column < 3; column++)
                    {
                        for (int row = 0; row < 3; row++)
                        {
                            if (grid[column + subGridStartingCoords[0], row + subGridStartingCoords[1]].number != 0)
                                candidates.Add(grid[column + subGridStartingCoords[0], row + subGridStartingCoords[1]].number);
                        }
                    }

                    foreach (var candidate in candidates)
                    {
                        grid[i, j].candidates.Remove(candidate);
                    }
                }
            }
        }

        public void HighlightNakedSingles(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (grid[i, j].candidates.Count == 1 && grid[i, j].number == 0)
                        spriteBatch.DrawString(Block.candidateFont, grid[i, j].candidates[0].ToString(), new Vector2(grid[i, j].position.X + 5 + (((grid[i, j].candidates[0] - 1) % 3) * 30), grid[i, j].position.Y + ((grid[i, j].candidates[0] - 1) / 3) * 30), Color.Red);
                }
            }
        }

        // Refactor
        public void HighlightHiddenSingles(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    int[] totalCandidates = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                    for (int column = 0; column < 9; column++)
                    {
                        if (grid[column, j].number == 0)
                        {
                            foreach (int candidate in grid[column, j].candidates)
                                totalCandidates[candidate] += 1;
                        }
                    }

                    // DRAW
                    for (int k = 1; k < 10; k++)
                    {
                        if (totalCandidates[k] == 1 && grid[i, j].number == 0 && grid[i, j].candidates.Contains(k))
                            spriteBatch.DrawString(Block.candidateFont, k.ToString(), new Vector2(grid[i, j].position.X + 5 + (((k - 1) % 3) * 30), grid[i, j].position.Y + ((k - 1) / 3) * 30), Color.Red);
                    }

                    Array.Clear(totalCandidates, 0, totalCandidates.Length);

                    for (int row = 0; row < 9; row++)
                    {
                        if (grid[i, row].number == 0)
                        {
                            foreach (int candidate in grid[i, row].candidates)
                                totalCandidates[candidate] += 1;
                        }
                    }

                    // DRAW
                    for (int k = 1; k < 10; k++)
                    {
                        if (totalCandidates[k] == 1 && grid[i, j].number == 0 && grid[i, j].candidates.Contains(k))
                            spriteBatch.DrawString(Block.candidateFont, k.ToString(), new Vector2(grid[i, j].position.X + 5 + (((k - 1) % 3) * 30), grid[i, j].position.Y + ((k - 1) / 3) * 30), Color.Red);
                    }

                    Array.Clear(totalCandidates, 0, totalCandidates.Length);

                    int[] subGridStartingCoords = FindSubGrid(i, j);

                    for (int column = 0; column < 3; column++)
                    {
                        for (int row = 0; row < 3; row++)
                        {
                            if (grid[column + subGridStartingCoords[0], row + subGridStartingCoords[1]].number == 0)
                            {
                                foreach (int candidate in grid[column + subGridStartingCoords[0], row + subGridStartingCoords[1]].candidates)
                                    totalCandidates[candidate] += 1;
                            }
                        }
                    }

                    // DRAW
                    for (int k = 1; k < 10; k++)
                    {
                        if (totalCandidates[k] == 1 && grid[i, j].number == 0 && grid[i, j].candidates.Contains(k))
                            spriteBatch.DrawString(Block.candidateFont, k.ToString(), new Vector2(grid[i, j].position.X + 5 + (((k - 1) % 3) * 30), grid[i, j].position.Y + ((k - 1) / 3) * 30), Color.Red);
                    }
                }
            }
        }

        private int[] FindSubGrid(int x, int y)
        {
            int[] subGridStartingCoords = new int[2];

            if (x >= 0 && x <= 2)
                subGridStartingCoords[0] = 0;
            else if (x >= 3 && x <= 5)
                subGridStartingCoords[0] = 3;
            else if (x >= 6 && x <= 8)
                subGridStartingCoords[0] = 6;

            if (y >= 0 && y <= 2)
                subGridStartingCoords[1] = 0;
            else if (y >= 3 && y <= 5)
                subGridStartingCoords[1] = 3;
            else if (y >= 6 && y <= 8)
                subGridStartingCoords[1] = 6;

            return subGridStartingCoords;
        }
    }
}