namespace KBSBoot.Model
{
    public class RegisterEventArgs
    {
        public string Name { get; }
        public string Username { get; }

        public RegisterEventArgs(string name, string username)
        {
            Name = name;
            Username = username;
        }
    }
}