using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using static WindowsAPI;

namespace vule_macro
{
    public partial class ChangeBindControl : UserControl
    {
        public event EventHandler Leaving;
        private ScriptEntry Entry;
        Thread SetBindThread;
        bool ActiveKey = false;
        public ChangeBindControl()
        {
            InitializeComponent();
        }

        public void ChangeScript(ScriptEntry entry) {
            Visibility = Visibility.Visible;
            Entry = entry;
            UIScriptToBindNameLabel.Content = entry.FileName + " bind";
            UIActivateKeyBindLabel.Content = KeyMap.First(x => x.Value == Entry.ActivateKey.KeyID).Key;
            UIDeactivateKeyLabel.Content = KeyMap.First(x => x.Value == Entry.DeactiveKey.KeyID).Key;
            UIKeyPressLabel.Visibility = Visibility.Hidden;
        }

        private void CloseBindButtonClick(object sender, EventArgs e)
        {
            Visibility = Visibility.Hidden;
            Leaving?.Invoke(this, EventArgs.Empty);
            SetBindThread?.Abort();
        }

        private void ChangeKeyBindButton(object sender, EventArgs e)
        {
            ActiveKey = ((VuleButton)sender).Tag.ToString() == "true";
            UIKeyPressLabel.Visibility = Visibility.Visible;
            SetBindThread = new Thread(() => 
            {
                while (true)
                {
                    foreach(var key in KeyMap)
                    {
                        int State = GetKeyState(key.Value);
                        if(State < 0)
                        {
                            Dispatcher.Invoke(() => 
                            {
                                UIKeyPressLabel.Visibility = Visibility.Hidden;
                                (ActiveKey ? UIActivateKeyBindLabel : UIDeactivateKeyLabel).Content = key.Key == "BACKSPACE"? "NONE" : key.Key;
                            });
                            (ActiveKey? Entry.ActivateKey : Entry.DeactiveKey).KeyID = key.Key == "BACKSPACE"? -1 : key.Value;
                            Thread.CurrentThread.Abort();
                        }
                    }
                    Thread.Sleep(20);
                }
            });
            SetBindThread.Start();
        }
    }
}
