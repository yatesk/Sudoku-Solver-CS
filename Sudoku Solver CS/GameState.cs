using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;

namespace Sudoku_Solver_CS
{
    class GameState : State
    {
        private MouseInput mouseInput;
        private Board board;
        private Tuple<int, int> clickedBlock;

        public GameState(Game1 game, ContentManager content, string puzzleSource, string puzzleDifficulty) : base(game, content)
        {
            clickedBlock = new Tuple<int, int>(-1, -1);
            mouseInput = new MouseInput(Mouse.GetState());

            board = new Board(content, puzzleSource, puzzleDifficulty);

            LoadContent();
        }

        public override void LoadContent()
        {
        }

        public override void Update(GameTime gameTime)
        {
            mouseInput.Update(Mouse.GetState());

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                game.Exit();

            if (mouseInput.LeftClick() && game.IsActive)
            {
                // check save button
                if (board.showCandidatesButton.Clicked(mouseInput.getMouseX(), mouseInput.getMouseY()))
                {
                    board.ShowCandidates();
                }
                else if (board.highlightNakedSinglesButton.Clicked(mouseInput.getMouseX(), mouseInput.getMouseY()))
                {
                    // refactor
                    int i = 4;
                }
                else if (board.highlightHiddenSinglesButton.Clicked(mouseInput.getMouseX(), mouseInput.getMouseY()))
                {
                    // refactor
                    int i = 4;
                }

                clickedBlock = Block.WhichBlock(mouseInput.getMouseX(), mouseInput.getMouseY());

                if (clickedBlock.Item1 != -1 && board.grid[clickedBlock.Item1, clickedBlock.Item2].revealed != true)
                {
                    board.grid[clickedBlock.Item1, clickedBlock.Item2].ChangeNumberWithMouse(mouseInput.getMouseX(), mouseInput.getMouseY());
                    board.ValidOrInvalidNumber(clickedBlock.Item1, clickedBlock.Item2);
                }
            }

            board.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            board.Draw(spriteBatch);
        }
    }
}