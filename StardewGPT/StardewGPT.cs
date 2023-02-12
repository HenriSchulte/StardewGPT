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

        public string CharacterName;

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

            // Display our UI if user presses middle mouse
            if (e.Button == SButton.MouseMiddle )
            {
                NPC dialogueTarget = null;
                foreach (NPC npc in Game1.currentLocation.characters)
                {
                    // Select first character that's in range - regardless of where player clicked
                    var x = (int) npc.Position.X;
                    var y = (int) npc.Position.Y;
                    bool inRange = Utility.withinRadiusOfPlayer(x, y, 1, Game1.player);
                    if (inRange)
                    {
                        dialogueTarget = npc;
                        continue;
                    }
                }
                if (dialogueTarget != null)
                {
                    this.CharacterName = dialogueTarget.Name;
                    string greeting = dialogueTarget.getHi(Game1.player.Name);
                    this.ConversationHistory.AppendLine($"{this.CharacterName}: {greeting}");
                    this.showDialogueMenu(greeting);
                }
            }
        }

        private async Task onInputSubmit(string text)
        {
            this.ConversationHistory.AppendLine(text);
            // Show empty dialogue box while fetching response
            this.showWaitingMenu();
            string prompt = this.ConstructPrompt(text);
            this.Monitor.Log(prompt, LogLevel.Debug);
            string response = await this.Api.GetCompletionAsync(prompt);
            this.Monitor.Log(response, LogLevel.Debug);
            this.ConversationHistory.AppendLine(response);
            this.showDialogueMenu(response);
        }

        private string ConstructPrompt(string text)
        {
            NPC npc = Game1.getCharacterFromName(this.CharacterName);
            string conversationHistory = this.ConversationHistory.ToString();
            string prefix = $"A conversation between two characters in the video game Stardew Valley, the farmer, {Game1.player.Name}, and {this.CharacterName}.";
            string emotions = $"Every message from {this.CharacterName} ends with an emotion token, e.g. $k. Neutral $k, happy $h, sad $s, love $l, or angry $a.";
            string time = $"The time is {this.GetTimeString()} on {this.GetDateString()}.";
            string relation = this.GetRelationshipString(npc);
            string personality = this.GetPersonalityString(npc);
            string prompt = $"{prefix} {emotions} {personality} {relation} {time}\n{conversationHistory}\n{this.CharacterName}: ";
            return prompt;
        }

        private string GetTimeString()
        {
            int hours = Game1.timeOfDay / 100;
            int min = Game1.timeOfDay % 100;
            string minPrefix = min > 9 ? "" : "0";
            return $"{hours}:{minPrefix}{min}";
        }

        private string GetDateString()
        {
            var weekdays = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            int day = Game1.dayOfMonth;
            string weekday = weekdays[(day - 1) % 7];
            return $"{weekday}, the {day} of {Game1.currentSeason}, {Game1.year} years after {Game1.player.Name} moved to the valley";
        }

        private string GetRelationshipString(NPC npc)
        {
            Farmer farmer = Game1.player;
            string relation = "";
            if (farmer.spouse == npc.Name)
            {
                if (farmer.isMarried())
                {
                    relation = $"married for {farmer.GetDaysMarried()} days";
                }
                else if (farmer.isEngaged())
                {
                    relation = "engaged";
                }
            }
            else if (npc.datingFarmer)
            {
                relation = "dating";
            }
            else if (npc.divorcedFromFarmer)
            {
                relation = "divorced";
            }
            else 
            {
                int heartLevel = farmer.getFriendshipHeartLevelForNPC(npc.Name);
                if (heartLevel < 5)
                {
                    relation = "acquaintances";
                }
                else if (heartLevel < 10) {
                    relation = "friends";
                }
                else {
                    relation = "best friends";
                }
            }
            return $"{npc.Name} and {farmer.Name} are {relation}.";
        }

        private string GetPersonalityString(NPC npc)
        {
            var adjectives = new List<string>();
            switch (npc.Manners)
            {
                case 1:
                    adjectives.Add("polite");
                    break;
                case 2:
                    adjectives.Add("rude");
                    break;
            }
            switch (npc.SocialAnxiety)
            {
                case 0:
                    adjectives.Add("outgoing");
                    break;
                case 1:
                    adjectives.Add("shy");
                    break;
            }
            switch (npc.Optimism)
            {
                case 0:
                    adjectives.Add("positive");
                    break;
                case 1:
                    adjectives.Add("negative");
                    break;
            }
            string age = "";
            switch (npc.Age)
            {
                case 0:
                    age = "adult";
                    break;
                case 1:
                    age = "teen";
                    break;
                case 2:
                    age = "child";
                    break;
            }
            string joined = String.Join(", ", adjectives);
            return $"{npc.Name} is a {joined} {age}.";
        }

        private void showDialogueMenu(string text)
        {
            this.Dialogue = new Dialogue(text, Game1.getCharacterFromName(this.CharacterName));
            this.Dialogue.onFinish = showInputMenu;
            Game1.activeClickableMenu = new DialogueBox(this.Dialogue);
        }

        private void showWaitingMenu()
        {
            Game1.activeClickableMenu = new GptWaitingMenu(Game1.getCharacterFromName(this.CharacterName));
        }

        private void showInputMenu()
        {
            Game1.activeClickableMenu = new GptInputMenu(onInputSubmit);
        }
    }
}