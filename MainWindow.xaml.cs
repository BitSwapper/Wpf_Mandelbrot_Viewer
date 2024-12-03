using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace WpfMandelbrot
{
    public partial class MainWindow : Window
    {
        private MandelbrotWindow mandelbrotWindow;
        private const double ZOOM_FACTOR = 2.0;
        private ObservableCollection<LocationHistory> navigationHistory;

        public class LocationHistory
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Zoom { get; set; }
            public override string ToString() => $"({X:F4}, {Y:F4}) Zoom: {Zoom:F2}";
        }

        public MainWindow()
        {
            InitializeComponent();
            mandelbrotWindow = new MandelbrotWindow(this);
            mandelbrotWindow.Show();

            navigationHistory = new ObservableCollection<LocationHistory>();
            HistoryList.ItemsSource = navigationHistory;

            // Set default values
            ColorModeCombo.SelectedIndex = 0;
            QualityCombo.SelectedIndex = 1;
            SmoothingCheck.IsChecked = true;

            // Add initial position to history
            AddToHistory();
        }

        public double CenterX
        {
            get { return double.Parse(CenterXTextBox.Text); }
            set { CenterXTextBox.Text = value.ToString("0.000000"); }
        }

        public double CenterY
        {
            get { return double.Parse(CenterYTextBox.Text); }
            set { CenterYTextBox.Text = value.ToString("0.000000"); }
        }

        public double Zoom
        {
            get { return double.Parse(ZoomTextBox.Text); }
            set { ZoomTextBox.Text = value.ToString("0.000000"); }
        }

        public int MaxIterations
        {
            get { return int.Parse(IterationsTextBox.Text); }
            set { IterationsTextBox.Text = value.ToString(); }
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            Zoom *= ZOOM_FACTOR;
            UpdateMandelbrot();
            AddToHistory();
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            Zoom /= ZOOM_FACTOR;
            UpdateMandelbrot();
            AddToHistory();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            CenterX = 0;
            CenterY = 0;
            Zoom = 1;
            MaxIterations = 1000;
            UpdateMandelbrot();
            AddToHistory();
        }

        private void UpdateView_Click(object sender, RoutedEventArgs e)
        {
            UpdateMandelbrot();
            AddToHistory();
        }

        private void ColorMode_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateMandelbrot();
        }

        private void Quality_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateMandelbrot();
        }

        private void Smoothing_Changed(object sender, RoutedEventArgs e)
        {
            UpdateMandelbrot();
        }

        private void History_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(HistoryList.SelectedItem is LocationHistory location)
            {
                CenterX = location.X;
                CenterY = location.Y;
                Zoom = location.Zoom;
                UpdateMandelbrot();
            }
        }

        private void AddToHistory()
        {
            navigationHistory.Add(new LocationHistory
            {
                X = CenterX,
                Y = CenterY,
                Zoom = Zoom
            });
            HistoryList.ScrollIntoView(navigationHistory[navigationHistory.Count - 1]);
        }

        public void UpdateMandelbrot()
        {
            var renderSettings = new RenderSettings
            {
                MaxIterations = MaxIterations,
                ColorMode = (ColorMode)ColorModeCombo.SelectedIndex,
                Quality = (RenderQuality)QualityCombo.SelectedIndex,
                SmoothColors = SmoothingCheck.IsChecked ?? false
            };

            mandelbrotWindow?.UpdateMandelbrot(CenterX, CenterY, Zoom, renderSettings);
        }
    }

    public enum ColorMode
    {
        Rainbow,
        Grayscale,
        BlueYellow
    }

    public enum RenderQuality
    {
        Draft,
        Normal,
        High
    }

    public class RenderSettings
    {
        public int MaxIterations { get; set; }
        public ColorMode ColorMode { get; set; }
        public RenderQuality Quality { get; set; }
        public bool SmoothColors { get; set; }
    }
}