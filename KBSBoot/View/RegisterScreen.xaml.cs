﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KBSBoot.Model;

namespace KBSBoot.View
{
    /// <summary>
    /// Interaction logic for RegisterScreen.xaml
    /// </summary>
    public partial class RegisterScreen : UserControl
    {
        public delegate void RegisterD(object source, RegisterEventArgs e);
        public event RegisterD OnRegister;

        public RegisterScreen()
        {
            InitializeComponent();
        }

        private void OKbtn_Click(object sender, RoutedEventArgs e)
        {
            Member member = new Member();
            OnRegister += member.OnRegisterOKButtonIsPressed;

            var NameText = Name.Text;
            var UsernameText = Username.Text;

            OnRegisterOKButtonIsPressed(NameText, UsernameText);
        }

        protected virtual void OnRegisterOKButtonIsPressed(string name, string username)
        {
            OnRegister?.Invoke(this, new RegisterEventArgs(name, username));
        }
    }
}
