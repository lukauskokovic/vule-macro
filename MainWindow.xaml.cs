using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using vule_macro.UserControls;
using static WindowsAPI;

namespace vule_macro
{
    public partial class MainWindow : Window
    {
        List<ScriptEntry> ScriptEntries = new List<ScriptEntry>();
        List<ScriptEntryDTO> LoadedSettings = new List<ScriptEntryDTO>();
        UIScriptItem SelectedItem = null;
        const string ScriptsPath = "Scripts";
        const string Extension = ".vulem";

        readonly Thread BindListenerThread;
        public MainWindow()
        {
            InitializeComponent();
            WindowsAPI.InitKeyMap();
            if (!Directory.Exists(ScriptsPath)) Directory.CreateDirectory(ScriptsPath);
            LoadScripts();
            
            BindListenerThread = new Thread(() =>
            {
                while (true)
                {
                    if (UISetBindControl.Visibility == Visibility.Hidden)
                    {
                        foreach (ScriptEntry entry in ScriptEntries)
                        {
                            if(entry.ActivateKey.KeyID == entry.DeactiveKey.KeyID && entry.ActivateKey.Pressed())
                            {
                                if (entry.Running) StopScript(entry);
                                else StartScript(entry);
                            }
                            else if(entry.ActivateKey.KeyID != entry.DeactiveKey.KeyID)
                            {
                                if (entry.ActivateKey.Pressed()) StartScript(entry);
                                else if (entry.DeactiveKey.Pressed()) StopScript(entry);
                            }

                        }

                        Thread.Sleep(20);
                    }
                    else Thread.Sleep(50);
                }
            });
            BindListenerThread.Start();
            #region Hard coded events
            Closing += (s, args) =>
            {
                BindListenerThread.Abort();
                foreach (ScriptEntry entry in ScriptEntries.Where(x => x.Running))
                {
                    Console.WriteLine("Canceling " + entry.FileName);
                    entry.Cancel();
                }
            };
            UICancelButton.OnClick += (s, args) =>
            {
                UICreateNewScriptGrid.Visibility = Visibility.Hidden;
                UIMainGrid.IsEnabled = true;
            };
            UICreateScriptButton.OnClick += (s, args) =>
            {
                string ScriptName = UINewScriptNameTB.Text;
                if (string.IsNullOrWhiteSpace(ScriptName)) return;
                UICreateNewScriptGrid.Visibility = Visibility.Hidden;
                UIMainGrid.IsEnabled = true;
                File.WriteAllText(ScriptsPath + "//" + ScriptName + Extension, "#" + ScriptName + " script");
                UINewScriptNameTB.Clear();
                LoadScripts();
            };
            UISetBindControl.Leaving += (s, args) => {
                SaveSettings();
                UIMainGrid.IsEnabled = true;
            };
            #endregion
        }

        public void SaveSettings()
        {
            string Json = JsonConvert.SerializeObject(ScriptEntries.Select(x => new ScriptEntryDTO(x)));
            File.WriteAllText("settings.json", Json);
            Console.WriteLine("Saving settings");
        }
        void StartScript(ScriptEntry entry)
        {
            Dispatcher.Invoke(() => 
            {
                foreach (UIScriptItem item in UIScriptsStackPanel.Children.OfType<UIScriptItem>())
                {
                    if (item.Entry == entry)
                    {
                        item.SetRunning(true);
                        break;
                    }
                }
            });
            entry.Start();
        }

        void StopScript(ScriptEntry entry)
        {
            Dispatcher.Invoke(() => 
            {
                foreach(UIScriptItem item in UIScriptsStackPanel.Children.OfType<UIScriptItem>())
                {
                    if(item.Entry == entry)
                    { 
                        item.SetRunning(false);
                        break;
                    }
                }
            });
            entry.Cancel();
        }
        void LoadScripts()
        {
            UIScriptsStackPanel.Children.Clear();
            ScriptEntries.Clear();
            if (File.Exists("settings.json"))
            {
                try
                {
                    LoadedSettings = JsonConvert.DeserializeObject<List<ScriptEntryDTO>>(File.ReadAllText("settings.json"));
                }
                catch
                {
                    LoadedSettings = new List<ScriptEntryDTO>();
                }
            }
            foreach (string File in Directory.EnumerateFiles(ScriptsPath).Where(x => System.IO.Path.GetExtension(x) == Extension))
            {
                var entry = new ScriptEntry(File);
                var item = new UIScriptItem(entry);
                item.RefreshEntry += UIItemRefreshEntry;
                entry.ScriptFinished += (s, args) => Dispatcher.Invoke(() => item.SetRunning(false));
                UIScriptsStackPanel.Children.Add(item);

                foreach (ScriptEntryDTO dto in LoadedSettings)
                {
                    if (dto.FileName == entry.FileName)
                    {
                        entry.ActivateKey = new WindowsKey(dto.ActivateKey);
                        entry.DeactiveKey = new WindowsKey(dto.DectivateKey);
                    }
                }
                ScriptEntries.Add(entry);
            }
        }
        #region UI Code
        private void SetBindButtonClick(object sender, EventArgs e)
        {
            if (SelectedItem == null) return;

            UIMainGrid.IsEnabled = false;
            UISetBindControl.ChangeScript(SelectedItem.Entry);
        }
        private void CreatenewButtonOnclick(object sender, EventArgs e)
        {
            if (UICreateNewScriptGrid.Visibility == Visibility.Hidden)
            {
                UICreateNewScriptGrid.Visibility = Visibility.Visible;
                UIMainGrid.IsEnabled = false;
            }
        }
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
        private void ui_set_alpha(byte alpha, object uicontrol)
        {
            Shape element = (Shape)uicontrol;
            SolidColorBrush brush = (SolidColorBrush)element.Fill;
            var newBrush = new SolidColorBrush(Color.FromArgb(alpha, brush.Color.R, brush.Color.G, brush.Color.B));
            element.Fill = newBrush;
        }
        private void ui_set_selected(UIScriptItem item)
        {
            if (SelectedItem != null) SelectedItem.SetSelected(false);

            UIScriptCode.Visibility = item != null ? Visibility.Visible : Visibility.Hidden;
            if (item == null)
            {
                SelectedItem = null;
                return;
            }
            SelectedItem = item;
            SelectedItem.SetSelected(true);
            UIScriptCode.Clear();
            UIScriptCode.Text = File.ReadAllText(SelectedItem.Entry.FileName);
        }
        private void UIItemMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                ui_set_selected(sender as UIScriptItem);
        }
        private void UIItemRefreshEntry(object sender, EventArgs e)
        {
            Console.WriteLine(sender.GetType());
            UIScriptItem item = sender as UIScriptItem;
            StopScript(item.Entry);
            item.Entry.Script.ParseFile(item.Entry.FileName);
            ui_set_selected(item);
            Console.WriteLine("Loaded script again");
        }
        private void UIDeleteScriptButton(object sender, EventArgs e)
        {
            if (SelectedItem == null) return;

            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete " + SelectedItem.Entry.FileName, "Yes or no", MessageBoxButton.YesNo);
            if(result == MessageBoxResult.Yes)
            {
                File.Delete(SelectedItem.Entry.FileName);
                ui_set_selected(null);
                LoadScripts();
            }
        }
        private void UISaveButtonClick(object sender, EventArgs e)
        {
            if (SelectedItem == null) return;

            StopScript(SelectedItem.Entry);
            File.WriteAllText(SelectedItem.Entry.FileName, UIScriptCode.Text);
            SelectedItem.Entry.Script.ParseFile(SelectedItem.Entry.FileName);
        }
        #endregion
    }
}
