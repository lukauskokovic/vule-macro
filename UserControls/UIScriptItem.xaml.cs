using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace vule_macro.UserControls
{
    public partial class UIScriptItem : UserControl
    {
        public event RoutedEventHandler RefreshEntry;
        public ScriptEntry Entry;
        public UIScriptItem(ScriptEntry entry)
        {
            InitializeComponent();
            UIScriptName.Content = System.IO.Path.GetFileNameWithoutExtension(entry.FileName);
            Entry = entry;
        }

        private void VuleButton_MouseEnter(object sender, MouseEventArgs e)
        {
            VuleButton button = sender as VuleButton;
            button.BackColor.Opacity = 0.7;
        }

        private void VuleButton_MouseLeave(object sender, MouseEventArgs e)
        {
            VuleButton button = sender as VuleButton;
            button.BackColor.Opacity = 1;
        }

        public void SetSelected(bool selected = false) => UIMainBorder.BorderThickness = new Thickness(selected ? 1 : 0);

        private void VuleButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                RefreshEntry?.Invoke(this, null);
        }
    }
}
