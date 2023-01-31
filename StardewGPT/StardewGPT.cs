using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;


namespace StardewGPT
{
    internal sealed class ModEntry : Mod
    {

        public Dialogue Dialogue;

        public StringBuilder ConversationHistory = new StringBuilder();

        public GptApi Api = new GptApi();

        public string CharacterName = "Alex";

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
                string greeting = $"Hey, {Game1.player.Name}!";
                this.ConversationHistory.AppendLine($"{this.CharacterName}: {greeting}");
                this.showDialogueMenu(greeting);
            }
        }

        private async Task onInputSubmit(string text)
        {
            this.Monitor.Log(text, LogLevel.Debug);
            this.ConversationHistory.AppendLine(text);
            // Show empty dialogue box while fetching response
            this.showDialogueMenu("...");
            string prompt = this.ConstructPrompt(text);
            this.Monitor.Log(prompt, LogLevel.Debug);
            string response = await this.Api.GetCompletionAsync(prompt);
            this.Monitor.Log(response, LogLevel.Debug);
            this.ConversationHistory.AppendLine(response);
            this.showDialogueMenu(response);
        }

        private string ConstructPrompt(string text)
        {
            string conversationHistory = this.ConversationHistory.ToString();
            string prefix = $"A conversation between two characters in the video game Stardew Valley, the new farmer, {Game1.player.Name}, and {this.CharacterName}.";
            string prompt = $"{prefix}\n{conversationHistory}\n{this.CharacterName}: ";
            return prompt;
        }

        private void showDialogueMenu(string text)
        {
            this.Dialogue = new Dialogue(text, Game1.getCharacterFromName(this.CharacterName));
            Game1.activeClickableMenu = new GptDialogueBox(this.Dialogue, showInputMenu);
        }

        private void showInputMenu()
        {
            Game1.activeClickableMenu = new GptInputMenu(onInputSubmit);
        }
    }
}