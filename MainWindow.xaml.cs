using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PainterApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point start = new Point { X = 0, Y = 0 };
        Point end = new Point { X = 0, Y = 0 };

        Color strokeColor = Colors.Black;
        Color fillColor = Colors.Transparent;

        int strokeThiness = 1;

        string shapeType = "Line";

        public MainWindow()
        {
            InitializeComponent();
            strokeColorPicker.SelectedColor = strokeColor;
            fillColorPicker.SelectedColor = fillColor;
        }

        private void ShapeButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void strokeColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            strokeColor = strokeColorPicker.SelectedColor.Value;
            DisplayStatus();
        }

        private void DisplayStatus()
        {
            if (statusLabel != null) statusLabel.Content = $"Start:({start.X},{start.Y}) End:({end.X},{end.Y}";
            if (colorLabel != null) colorLabel.Content = $"筆刷色彩: {strokeColor} 填滿色彩: {fillColor} 筆刷粗細: {strokeThiness}";
            if (shapeLabel != null) shapeLabel.Content = $"形狀: {shapeType}";
        }

        private void fillColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            fillColor = fillColorPicker.SelectedColor.Value;
            DisplayStatus();
        }

        private void ShapeButton_Checked(object sender, RoutedEventArgs e)
        {
            var targetRadioButton = sender as RadioButton;
            shapeType = targetRadioButton.Tag.ToString();
            DisplayStatus();
        }

        private void strokeThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            strokeThiness = (int)strokeThicknessSlider.Value;
            DisplayStatus();
        }
    }
}