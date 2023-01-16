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
        public GPTTextBox(Texture2D textBoxTexture, Texture2D caretTexture, SpriteFont font, Color textColor)
		: base(textBoxTexture, caretTexture, font, textColor)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, bool drawShadow = true) {
            bool caretVisible = true;
            caretVisible = !(Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1000.0 < 500.0);
            if (caretVisible && base.Selected)
            {
                spriteBatch.Draw(Game1.staminaRect, new Rectangle(base.X + 16 + (int)this.Width - 2, base.Y + 8, 4, 32), base._textColor);
            }
            // spriteBatch.DrawString(base._font, $"Alex: Hello, World!\n{Game1.player.Name}: ", new Vector2((float)base.X + 12f), base._textColor);
            SpriteText.drawString(spriteBatch, $"Alex: Hello, World!^{Game1.player.Name}: {base.Text}", base.X + 8, base.Y + 12);
        }
    }
}