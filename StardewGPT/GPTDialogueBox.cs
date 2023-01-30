using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Menus;
using StardewValley;

namespace StardewGPT
{
	public class GptDialogueBox : DialogueBox
	{
		public delegate void closeBehavior();
	
		private closeBehavior onClose;


		public GptDialogueBox(Dialogue dialogue, closeBehavior callback) : base(dialogue)
		{
			this.onClose = callback;
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			if (base.characterDialogue.isOnFinalDialogue())
			{
				this.onClose();
			}
			else
			{
				base.receiveLeftClick(x, y, playSound);
			}
		}
	}
}