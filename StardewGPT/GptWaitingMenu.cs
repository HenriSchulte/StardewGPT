using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Menus;
using StardewValley;

namespace StardewGPT
{
	public class GptWaitingMenu : DialogueBox
	{
		public GptWaitingMenu(NPC speaker) : base(new Dialogue($"*{speaker.Name} is thinking...*", speaker))
		{
	
		}

        public override void receiveKeyPress(Keys key)
        {
			// leave empty to suppress any input
        }
		
		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			// leave empty to suppress any input
		}

		public override void receiveRightClick(int x, int y, bool playSound = true)
		{
			// leave empty to suppress any input
		}
	}
}