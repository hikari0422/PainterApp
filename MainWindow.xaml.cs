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
using System.Linq;

namespace PainterApp
{
    public partial class MainWindow : Window
    {
        Point start = new Point { X = 0, Y = 0 };
        Point end = new Point { X = 0, Y = 0 };

        Color strokeColor = Colors.Black;
        Color fillColor = Colors.Transparent;

        int strokeThickness = 1;

        string shapeType = "Line";
        string actionType = "Draw";

        public MainWindow()
        {
            InitializeComponent();
            strokeColorPicker.SelectedColor = strokeColor;
            fillColorPicker.SelectedColor = fillColor;
            lineRadioButton.IsChecked = true;
        }

        private void strokeColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            strokeColor = e.NewValue ?? Colors.Black;
            DisplayStatus();
        }

        private void DisplayStatus()
        {
            if (statusLabel != null) statusLabel.Content = $"工作模式:{actionType} Start:({start.X},{start.Y}) End:({end.X},{end.Y})";
            if (colorLabel != null) colorLabel.Content = $"筆刷色彩: {strokeColor} 填滿色彩: {fillColor} 筆刷粗細: {strokeThickness}";
            if (shapeLabel != null) shapeLabel.Content = $"形狀: {shapeType}";
        }

        private void fillColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            fillColor = e.NewValue ?? Colors.Transparent;
            DisplayStatus();
        }

        private void ShapeButton_Checked(object sender, RoutedEventArgs e)
        {
            var targetRadioButton = sender as RadioButton;
            if (targetRadioButton.Tag != null)
            {
                shapeType = targetRadioButton.Tag.ToString();
                actionType = "Draw";
                DisplayStatus();
            }
        }

        private void strokeThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            strokeThickness = (int)strokeThicknessSlider.Value;
            DisplayStatus();
        }

        private void Eraser_Click(object sender, RoutedEventArgs e)
        {
            actionType = "Eraser";
            DisplayStatus();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            actionType = "Clear";
            MyCanvas.Children.Clear();
            actionType = "Line";
            DisplayStatus();
        }

        private void MyCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            MyCanvas.Cursor = Cursors.Pen;
        }

        private void MyCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyCanvas.Cursor = Cursors.Cross;
            start = e.GetPosition(MyCanvas);
            end = start;

            if (actionType == "Draw")
            {
                switch (shapeType)
                {
                    case "Line":
                        Line line = new Line
                        {
                            X1 = start.X,
                            Y1 = start.Y,
                            X2 = end.X,
                            Y2 = end.Y,
                            Stroke = Brushes.Gray,
                            StrokeThickness = 1,
                            StrokeDashArray = new DoubleCollection() { 4, 2 }
                        };
                        MyCanvas.Children.Add(line);
                        break;
                    case "Rectangle":
                        Rectangle rect = new Rectangle
                        {
                            Stroke = Brushes.Gray,
                            Fill = Brushes.Transparent,
                            StrokeThickness = 1,
                            StrokeDashArray = new DoubleCollection() { 4, 2 }
                        };
                        rect.SetValue(Canvas.LeftProperty, start.X);
                        rect.SetValue(Canvas.TopProperty, start.Y);
                        MyCanvas.Children.Add(rect);
                        break;
                    case "Ellipse":
                        Ellipse ellipse = new Ellipse
                        {
                            Stroke = Brushes.Gray,
                            Fill = Brushes.Transparent,
                            StrokeThickness = 1,
                            StrokeDashArray = new DoubleCollection() { 4, 2 }
                        };
                        ellipse.SetValue(Canvas.LeftProperty, start.X);
                        ellipse.SetValue(Canvas.TopProperty, start.Y);
                        MyCanvas.Children.Add(ellipse);
                        break;
                    case "Polyline":
                        var polyline = new Polyline
                        {
                            Stroke = Brushes.Gray,
                            Fill = Brushes.LightGray,
                        };
                        MyCanvas.Children.Add(polyline);
                        break;
                }
            }
            else if (actionType == "Clear")
            {
                MyCanvas.Children.Clear();
            }

            DisplayStatus();
        }

        private void MyCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            end = e.GetPosition(MyCanvas);

            switch (actionType)
            {
                case "Draw":
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        Point origin = new Point();
                        origin.X = Math.Min(start.X, end.X);
                        origin.Y = Math.Min(start.Y, end.Y);
                        double width = Math.Abs(end.X - start.X);
                        double height = Math.Abs(end.Y - start.Y);

                        switch (shapeType)
                        {
                            case "Line":
                                var line = MyCanvas.Children.OfType<Line>().LastOrDefault();
                                if (line != null)
                                {
                                    line.X2 = end.X;
                                    line.Y2 = end.Y;
                                }
                                break;
                            case "Rectangle":
                                var rect = MyCanvas.Children.OfType<Rectangle>().LastOrDefault();
                                if (rect != null)
                                {
                                    rect.Width = width;
                                    rect.Height = height;
                                    rect.SetValue(Canvas.LeftProperty, origin.X);
                                    rect.SetValue(Canvas.TopProperty, origin.Y);
                                }
                                break;
                            case "Ellipse":
                                var ellipse = MyCanvas.Children.OfType<Ellipse>().LastOrDefault();
                                if (ellipse != null)
                                {
                                    ellipse.Width = width;
                                    ellipse.Height = height;
                                    ellipse.SetValue(Canvas.LeftProperty, origin.X);
                                    ellipse.SetValue(Canvas.TopProperty, origin.Y);
                                }
                                break;
                            case "Polyline":
                                var polyline = MyCanvas.Children.OfType<Polyline>().LastOrDefault();
                                polyline.Points.Add(end);
                                break;
                            
                        }
                    }
                    break;
                case "Eraser":
                    MyCanvas.Cursor = Cursors.Hand;
                    var shape = e.OriginalSource as Shape;
                    MyCanvas.Children.Remove(shape);
                    if (MyCanvas.Children.Count == 0) MyCanvas.Cursor = Cursors.Arrow;
                    break;
            }

            DisplayStatus();
        }

        private void MyCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Brush strokeBrush = new SolidColorBrush(strokeColor);
            Brush fillBrush = new SolidColorBrush(fillColor);

            if (fillColor == Colors.Transparent)
            {
                fillBrush = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));
            }


            switch (shapeType)
            {
                case "Line":
                    var line = MyCanvas.Children.OfType<Line>().LastOrDefault();
                    if (line != null)
                    {
                        line.Stroke = strokeBrush;
                        line.StrokeThickness = strokeThickness;
                        line.StrokeDashArray = null;
                    }
                    break;
                case "Rectangle":
                    var rect = MyCanvas.Children.OfType<Rectangle>().LastOrDefault();
                    if (rect != null)
                    {
                        rect.Stroke = strokeBrush;
                        rect.Fill = fillBrush;
                        rect.StrokeThickness = strokeThickness;
                        rect.StrokeDashArray = null;
                    }
                    break;
                case "Ellipse":
                    var ellipse = MyCanvas.Children.OfType<Ellipse>().LastOrDefault();
                    if (ellipse != null)
                    {
                        ellipse.Stroke = strokeBrush;
                        ellipse.Fill = fillBrush;
                        ellipse.StrokeThickness = strokeThickness;
                        ellipse.StrokeDashArray = null;
                    }
                    break;
                case "Polyline":
                    var polyline = MyCanvas.Children.OfType<Polyline>().LastOrDefault();
                    
                    if (polyline != null)
                    {
                        polyline.Stroke = strokeBrush;
                        polyline.Fill = fillBrush;
                        polyline.StrokeThickness = strokeThickness;
                    }
                    break;
            }
            MyCanvas.Cursor = Cursors.Pen;
            DisplayStatus();
        }
    }
}
