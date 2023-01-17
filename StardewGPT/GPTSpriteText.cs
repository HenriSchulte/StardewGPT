using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley;

namespace StardewGPT
{
    public class GPTSpriteText
    {
        public static string getLastLine(string text)
        {
            return text.Split('^')[^1];
        }

        public static int getLastLineWidth(string text)
        {   
            string lastLine = getLastLine(text);
            return SpriteText.getWidthOfString(lastLine);
        }

        public static string breakLastLine(string text)
        {
            int lastSpaceIdx = text.LastIndexOf(" ");
            string newLine = text.Substring(lastSpaceIdx);
            int oldLength = text.Length - newLine.Length;
            return text.Substring(0, oldLength) + "^" + text.Substring(lastSpaceIdx, newLine.Length).Trim();
        }
    }
}