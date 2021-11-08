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
    /// Interaction logic for VuleButton.xaml
    /// </summary>
    public partial class VuleButton : UserControl
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(VuleButton));

        public static readonly DependencyProperty BackColorProperty =
            DependencyProperty.Register("BackColor", typeof(Brush), typeof(VuleButton));
        public event EventHandler OnClick;

        public VuleButton() => InitializeComponent();

        public Brush BackColor
        {
            get { return (Brush)GetValue(BackColorProperty); }
            set { SetValue(BackColorProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            Border border = (Border)sender;
            SolidColorBrush brush = border.Background as SolidColorBrush;
            if (brush == null) return;
            var newBrush = new SolidColorBrush(Color.FromArgb(100, brush.Color.R, brush.Color.G, brush.Color.B));
            border.Background = newBrush;
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            Border border = (Border)sender;
            SolidColorBrush brush = border.Background as SolidColorBrush;
            if (brush == null) return;
            var newBrush = new SolidColorBrush(Color.FromArgb(255, brush.Color.R, brush.Color.G, brush.Color.B));
            border.Background = newBrush;
        }

        private void Control_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && OnClick != null) OnClick(this, EventArgs.Empty);
        }
    }
}
