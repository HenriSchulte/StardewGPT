namespace StardewGPT
{
    public class GptMessage
    {
        public string role { get; set; }
        public string content { get; set; }
        public string name { get; set; }

        public GptMessage(string role, string content, string name = "system")
        {
            this.role = role;
            this.content = content;
            this.name = name;
        }

        public override string ToString()
        {
            return $"{this.role} ({this.name}): {this.content}";
        }
    }
}