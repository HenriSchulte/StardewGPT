using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;


// TODO:
// - Fix caret
// - Add line breaks
// - Add submit button
// - The whole AI bit
// - Fix menu size and position
// - Fix letters being recognized as commands (e.g. E for inventory)


namespace StardewGPT
{
    internal sealed class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            // print button presses to the console window
            this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);

            // Display our UI if user presses F10
            if (e.Button == SButton.F10)
                // Game1.activateClickableMenu = new PlayerInputDialogue();
                Game1.activeClickableMenu = new DialogueBox("This is a test.^Oh well...^");
            else if (e.Button == SButton.F11)
                Game1.activeClickableMenu = new GPTInputMenu(log);
            else if (e.Button == SButton.F9)
            {
                Dialogue d = new Dialogue("Hello, World!$h#$b", Game1.getCharacterFromName("Alex"));
                Game1.activeClickableMenu = new GPTDialogueBox(d);
            }
        }

        private void log(string message)
        {
            this.Monitor.Log(message, LogLevel.Debug);
            Game1.exitActiveMenu();
        }
    }
}