using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Sudoku_Solver_CS
{
    public abstract class State
    {
        protected Game1 game;
        protected ContentManager content;

        public State(Game1 _game, ContentManager _content)
        {
            game = _game;
            content = _content;
        }

        public abstract void LoadContent();

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}