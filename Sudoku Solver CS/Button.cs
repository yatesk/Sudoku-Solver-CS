using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Sudoku_Solver_CS
{
    class Button
    {
        private Texture2D texture;
        private SpriteFont font;
        private Vector2 position;

        private string text;
        private Color textColor;
        private string textureName;

        private bool mouseOver;
        private bool toggleable;
        public bool toggle;

        public Button(string _textureName, string fontName, Vector2 _position, string _text, Color _textColor, ContentManager content)
        {
            textureName = _textureName;
            texture = content.Load<Texture2D>(textureName);
            font = content.Load<SpriteFont>(fontName);
            position = _position;
            text = _text;
            textColor = _textColor;
        }

        public Button(string _textureName, Vector2 _position, ContentManager content)
        {
            textureName = _textureName;
            texture = content.Load<Texture2D>(textureName);
            position = _position;
        }

        public Button(string _textureName, Vector2 _position, ContentManager content, bool _toggleable)
        {
            textureName = _textureName;
            texture = content.Load<Texture2D>(textureName);
            position = _position;

            toggleable = _toggleable;
            toggle = false;
        }

        public bool Clicked(int _x, int _y)
        {
            if (_x >= position.X && _x <= position.X + texture.Width && _y >= position.Y && _y <= position.Y + texture.Height)
            {
                if (toggleable)
                    toggle = !toggle;

                return true;
            }
            else
                return false;
        }

        public void Update(int _x, int _y)
        {
            if (_x >= position.X && _x <= position.X + texture.Width && _y >= position.Y && _y <= position.Y + texture.Height)
                mouseOver = true;
            else
                mouseOver = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (mouseOver || toggle)
            {
                spriteBatch.Draw(texture, position, Color.Gray);
            }
            else
                spriteBatch.Draw(texture, position);

            if (!string.IsNullOrEmpty(text))
            {
                float x = position.X + (texture.Width / 2) - (font.MeasureString(text).X / 2);
                float y = position.Y + (texture.Height / 2) - (font.MeasureString(text).Y / 2);

                spriteBatch.DrawString(font, text, new Vector2(x, y), textColor);
            }
        }

        public string getText()
        {
            if (!string.IsNullOrEmpty(text))
                return text;
            else
                return textureName;
        }

        public Color getTextColor()
        {
            return textColor;
        }
    }
}