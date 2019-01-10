using System;

namespace KBSBoot.Model
{
    public class LoginEventArgs : EventArgs
    {
        public string Name { get; }
        
        public LoginEventArgs(string name)
        {
            Name = name;
        }
    }
}