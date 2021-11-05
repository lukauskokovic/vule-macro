using System;
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

namespace vule_macro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MacroScript script = new MacroScript();
            script.ParseFile(@"C:\Users\uskok\source\repos\New Text Document.txt");
            //script.Execute();
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
            brush = null;
        }
        #endregion
    }
}
