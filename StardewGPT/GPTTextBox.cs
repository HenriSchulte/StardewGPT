using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley;

namespace StardewGPT
{
    public class GPTTextBox : TextBox
    {
        public new string Text
        {
            get
            {
                return this._text;
            }
            set
            {
                this._text = value;
                if (this._text == null)
                {
                    this._text = "";
                }
                if (!(this._text != ""))
                {
                    return;
                }
                if (this.limitWidth && GPTSpriteText.getLastLineWidth(this._text) > (this.Width - 21))
                {
                    this.Text = GPTSpriteText.breakLastLine(this._text);
                }
            }
        }

        private string _text;

        public GPTTextBox(Texture2D textBoxTexture, Texture2D caretTexture, SpriteFont font, Color textColor, string text = "")
		: base(textBoxTexture, caretTexture, font, textColor)
        {
            this._text = text;
        }

        public override void RecieveTextInput(char inputChar)
        {
            this.Text += inputChar;
        }

        public override void RecieveTextInput(string text)
        {
            this.Text += text;
        }

        public override void RecieveCommandInput(char command)
        {
            if (command == '\b' && this._text.Length > 0)
            {
                this.Text = this._text.Remove(this._text.Length - 1);
            }
            else
            {
                base.RecieveCommandInput(command);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, bool drawShadow = true) {
            bool caretVisible = true;
            caretVisible = !(Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1000.0 < 500.0);
            if (caretVisible && base.Selected)
            {
                int textHeight = SpriteText.getHeightOfString(this.Text);
                int textWidth = GPTSpriteText.getLastLineWidth(this.Text);
                spriteBatch.Draw(Game1.staminaRect, new Rectangle(base.X + 16 + textWidth + 2, base.Y + textHeight - 30, 4, 32), base._textColor);
            }
            // spriteBatch.DrawString(base._font, $"Alex: Hello, World!\n{Game1.player.Name}: ", new Vector2((float)base.X + 12f), base._textColor);
            SpriteText.drawString(spriteBatch, this.Text, base.X + 8, base.Y + 12);
        }
    }
}