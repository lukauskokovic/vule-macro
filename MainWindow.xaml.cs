using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using static WindowsAPI;

namespace vule_macro
{
    public partial class MainWindow : Window
    {
        List<ScriptEntry> ScriptEntries = new List<ScriptEntry>();
        const string ScriptsPath = "Scripts";
        const string Extension = ".vulem";
        int BindKey = -1;
        Thread BindListenerThread;
        Thread SetBindThread;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            WindowsAPI.InitKeyMap();

            if (!Directory.Exists(ScriptsPath)) Directory.CreateDirectory(ScriptsPath);
            LoadScripts();

            BindListenerThread = new Thread(() =>
            {
                while (true)
                {
                    foreach (ScriptEntry entry in ScriptEntries)
                    {
                        int state = GetKeyState(entry.KeyBind);
                        if (state == 1 || state == 0 || state == entry.LastKeyState) 
                            continue;

                        entry.LastKeyState = state;
                        entry.Start();
                    }
                    Thread.Sleep(20);
                }
            });
            BindListenerThread.Start();

            Closing += (s, args) =>
            {
                BindListenerThread.Abort();
                foreach (ScriptEntry entry in ScriptEntries.Where(x => x.Running))
                {
                    Console.WriteLine("Canceling " + entry.FileName);
                    entry.Cancel();
                }
            };

            CancelButton.OnClick += (s, args) =>
            {
                CreateNewScriptGrid.Visibility = Visibility.Hidden;
                MainGrid.IsEnabled = true;
            };

            CreateScriptButton.OnClick += (s, args) =>
            {
                string ScriptName = NewScriptNameTB.Text;
                if (String.IsNullOrWhiteSpace(ScriptName)) return;
                CreateNewScriptGrid.Visibility = Visibility.Hidden;
                MainGrid.IsEnabled = true;
                File.WriteAllText(ScriptsPath + "//" + ScriptName + Extension, "#" + ScriptName + " script");
                LoadScripts();
            };
        }

        void LoadScripts()
        {
            ScriptsListBox.Items.Clear();
            ScriptEntries.Clear();
            foreach (string File in Directory.EnumerateFiles(ScriptsPath).Where(x => System.IO.Path.GetExtension(x) == Extension))
            {
                ScriptsListBox.Items.Add(System.IO.Path.GetFileNameWithoutExtension(File));
                ScriptEntries.Add(new ScriptEntry(File));
            }
        }
        private void SetbindOnClick(object sender, EventArgs e)
        {
            if (ScriptsListBox.SelectedIndex == -1) return;
            SetBindThread = new Thread(() => 
            {
                while (true)
                {
                    foreach(var Key in KeyMap)
                    {
                        int state = GetKeyState(Key.Value);
                        if(state < 0)
                        {
                            BindKey = Key.Value;
                            Dispatcher.Invoke(() => KeyBindText.Content = "Selected button '" + Key.Key + "'");
                            break;
                        }
                    }
                    Thread.Sleep(20);
                }
            });
            SetBindThread.Start();
            SetbindGrid.Visibility = Visibility.Visible;
            MainGrid.IsEnabled = false;
        }

        private void CancelBindButtonOnclick(object sender, EventArgs e)
        {
            MainGrid.IsEnabled = true;
            SetbindGrid.Visibility = Visibility.Hidden;
            try
            {
                SetBindThread.Abort();
            }
            catch { }
        }
        private void SetBindButtonClick(object sender, EventArgs e)
        {
            if (BindKey == -1) return;
            Console.WriteLine("Setting " + ScriptEntries[ScriptsListBox.SelectedIndex].FileName + " bind to " + KeyMap.First(x => x.Value == BindKey).Key);
            ScriptEntries[ScriptsListBox.SelectedIndex].KeyBind = BindKey;
            MainGrid.IsEnabled = true;
            SetbindGrid.Visibility = Visibility.Hidden;
        }
        private void CreatenewButtonOnclick(object sender, EventArgs e)
        {
            if (CreateNewScriptGrid.Visibility == Visibility.Hidden)
            {
                CreateNewScriptGrid.Visibility = Visibility.Visible;
                MainGrid.IsEnabled = false;
            }
        }
        #region UI Code
        private void HeaderMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }
        private void EllipseMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (((Ellipse)sender).Tag.ToString() == "0") Environment.Exit(0);
            else WindowState = WindowState.Minimized;
        }
        private void EllipseMouseEnter(object sender, MouseEventArgs e) => ui_set_alpha(175, sender);
        private void EllipseMouseLeave(object sender, MouseEventArgs e) => ui_set_alpha(255, sender);
        static void ui_set_alpha(byte alpha, object uicontrol)
        {
            Shape element = (Shape)uicontrol;
            SolidColorBrush brush = (SolidColorBrush)element.Fill;
            var newBrush = new SolidColorBrush(Color.FromArgb(alpha, brush.Color.R, brush.Color.G, brush.Color.B));
            element.Fill = newBrush;
        }
        #endregion

       
    }
}
