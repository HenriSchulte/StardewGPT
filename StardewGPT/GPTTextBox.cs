using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley;

namespace StardewGPT
{
    public class GptTextBox : TextBox
    {
        public GptTextBox(Texture2D textBoxTexture, Texture2D caretTexture, SpriteFont font, Color textColor)
		: base(textBoxTexture, caretTexture, font, textColor)
        {
            base.limitWidth = false;
        }

        public override void RecieveTextInput(char inputChar)
        {
            string combined = base.Text + inputChar;
            if (GptSpriteText.getLastLineWidth(combined) > (base.Width - 21))
            {
                base.Text = GptSpriteText.breakLastLine(combined);
            }
            else
            {
                base.Text = combined;
            }
        }

        public override void RecieveTextInput(string text)
        {
            foreach (char c in text)
            {
                RecieveTextInput(c);
            }
        }

        public override void RecieveCommandInput(char command)
        {
            if (command == '\b' && base.Text.Length > 0)
            {
                base.Text = base.Text.Remove(base.Text.Length - 1);
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
                int textHeight = SpriteText.getHeightOfString(base.Text);
                int textWidth = GptSpriteText.getLastLineWidth(base.Text);
                spriteBatch.Draw(Game1.staminaRect, new Rectangle(base.X + 16 + textWidth + 2, base.Y + textHeight - 30, 4, 32), base._textColor);
            }
            SpriteText.drawString(spriteBatch, base.Text, base.X + 8, base.Y + 12);
        }
    }
}