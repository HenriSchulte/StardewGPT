using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley;
using StardewModdingAPI;

namespace StardewGPT
{
	public class GptInputMenu : IClickableMenu
	{
		public delegate Task submitBehavior(string s);
	
		private submitBehavior onSubmit;

		private int x;

		private int y;

		private TextBox textBox;

		public const int region_submitButton = 102;

		public ClickableTextureComponent submitButton;

		public ClickableTextureComponent exitButton;

		public string inputPrefix = $"{Game1.player.Name}: ";

		public GptInputMenu(submitBehavior callback)
		{
			this.onSubmit = callback;
			this.x = Game1.uiViewport.Width / 2 - 600;
			this.y = Game1.uiViewport.Height / 2 + 92;
			base.width = 1200;
			base.height = 384;
			this.textBox = new GptTextBox(null, null, Game1.dialogueFont, Game1.textColor, this.inputPrefix);
			this.textBox.X = x;
			this.textBox.Y = y;
			this.textBox.Width = width;
			this.textBox.Height = height;
			this.textBox.OnEnterPressed += textBoxEnter;
			this.textBox.Text = this.inputPrefix;
			this.textBox.SelectMe();
			this.submitButton = new ClickableTextureComponent(new Rectangle(x + width - 64 - 4, y + height - 64 - 4, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f)
			{
				myID = 102
			};
			this.exitButton = new ClickableTextureComponent(new Rectangle(x + width - 64 - 4, y + height - 128 - 8, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47), 1f)
			{
				myID = 103
			};
		}

		public void textBoxEnter(TextBox sender)
		{
			string text = sender.Text.Trim();
			if (text != this.inputPrefix.Trim())
			{
				// Text is not empty
				this.onSubmit(text);
			}
		}

		public void exitMenu()
		{
			base.exitThisMenu();
		}

		public override void receiveKeyPress(Keys key)
		{
			// Leave empty to prevent typing from opening menus
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			base.receiveLeftClick(x, y, playSound);
			this.textBox.Update();
			if (this.submitButton.containsPoint(x, y))
			{
				this.textBoxEnter(this.textBox);
				Game1.playSound("smallSelect");
			}
			else if (this.exitButton.containsPoint(x, y))
			{
				this.exitMenu();
			}
		}

		public override void performHoverAction(int x, int y)
		{
			base.performHoverAction(x, y);
			if (this.submitButton != null)
			{
				if (this.submitButton.containsPoint(x, y))
				{
					this.submitButton.scale = Math.Min(1.1f, this.submitButton.scale + 0.05f);
				}
				else
				{
					this.submitButton.scale = Math.Max(1f, this.submitButton.scale - 0.05f);
				}
			}
			if (this.exitButton != null)
			{
				if (this.exitButton.containsPoint(x, y))
				{
					this.exitButton.scale = Math.Min(1.1f, this.exitButton.scale + 0.05f);
				}
				else
				{
					this.exitButton.scale = Math.Max(1f, this.exitButton.scale - 0.05f);
				}
			}
			
		}

		public override void draw(SpriteBatch b)
		{
			base.draw(b);
			this.drawBox(b, this.x, this.y, this.width, this.height);
			this.textBox.Draw(b);
			this.submitButton.draw(b);
			this.exitButton.draw(b);
			base.drawMouse(b);
		}

		public void drawBox(SpriteBatch b, int xPos, int yPos, int boxWidth, int boxHeight)
		{
			b.Draw(Game1.mouseCursors, new Rectangle(xPos, yPos, boxWidth, boxHeight), new Rectangle(306, 320, 16, 16), Color.White);
			b.Draw(Game1.mouseCursors, new Rectangle(xPos, yPos - 20, boxWidth, 24), new Rectangle(275, 313, 1, 6), Color.White);
			b.Draw(Game1.mouseCursors, new Rectangle(xPos + 12, yPos + boxHeight, boxWidth - 20, 32), new Rectangle(275, 328, 1, 8), Color.White);
			b.Draw(Game1.mouseCursors, new Rectangle(xPos - 32, yPos + 24, 32, boxHeight - 28), new Rectangle(264, 325, 8, 1), Color.White);
			b.Draw(Game1.mouseCursors, new Rectangle(xPos + boxWidth, yPos, 28, boxHeight), new Rectangle(293, 324, 7, 1), Color.White);
			b.Draw(Game1.mouseCursors, new Vector2(xPos - 44, yPos - 28), new Rectangle(261, 311, 14, 13), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
			b.Draw(Game1.mouseCursors, new Vector2(xPos + boxWidth - 8, yPos - 28), new Rectangle(291, 311, 12, 11), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
			b.Draw(Game1.mouseCursors, new Vector2(xPos + boxWidth - 8, yPos + boxHeight - 8), new Rectangle(291, 326, 12, 12), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
			b.Draw(Game1.mouseCursors, new Vector2(xPos - 44, yPos + boxHeight - 4), new Rectangle(261, 327, 14, 11), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.87f);
		}
	}
}