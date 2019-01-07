namespace KBSBoot.Model
{
    public class RegisterEventArgs
    {
        public string Name { get; set; }
        public string Username { get; set; }

        public RegisterEventArgs(string Name, string Username)
        {
            this.Name = Name;
            this.Username = Username;
        }
    }
}