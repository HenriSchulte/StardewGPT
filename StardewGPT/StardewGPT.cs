using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;


// TODO:
// - The whole AI bit

namespace StardewGPT
{
    internal sealed class ModEntry : Mod
    {

        private Dialogue dialogue;

        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if (Game1.activeClickableMenu != null || (!Context.IsPlayerFree)) return;

            // print button presses to the console window
            this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);

            // Display our UI if user presses F10
            if (e.Button == SButton.F10)
            {
                this.showDialogueMenu($"Hey, {Game1.player.Name}!$e", "Alex");
            }
        }

        private void onInputSubmit(string text)
        {
            this.Monitor.Log(text, LogLevel.Debug);
            // TODO: call AI and get response
            this.showDialogueMenu(text, "Alex"); // this should not use text, but instead the AI response
        }

        private void showDialogueMenu(string text, string character)
        {
            dialogue = new Dialogue(text, Game1.getCharacterFromName(character));
            Game1.activeClickableMenu = new GptDialogueBox(dialogue, showInputMenu);
        }

        private void showInputMenu()
        {
            Game1.activeClickableMenu = new GptInputMenu(onInputSubmit);
        }
    }
}