using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfMandelbrot;

public partial class MandelbrotWindow : Window
{
    private MainWindow mainWindow;
    private MandelbrotEffect mandelbrotEffect;

    public MandelbrotWindow(MainWindow main)
    {
        InitializeComponent();
        mainWindow = main;

        // Initialize the effect
        mandelbrotEffect = new MandelbrotEffect();
        MandelbrotImage.Effect = mandelbrotEffect;

        this.Loaded += (s, e) => UpdateMandelbrot(0, 0, 1, new RenderSettings
        {
            MaxIterations = 1000,
            ColorMode = ColorMode.Rainbow,
            Quality = RenderQuality.Normal,
            SmoothColors = true
        });

        SizeChanged += MandelbrotWindow_SizeChanged;
    }

    public void UpdateMandelbrot(double centerX, double centerY, double zoom, RenderSettings settings)
    {
        mandelbrotEffect.Center = new Point(centerX, centerY);
        mandelbrotEffect.Zoom = zoom;
        mandelbrotEffect.MaxIterations = settings.MaxIterations;
        mandelbrotEffect.Resolution = new Point(
            MandelbrotImage.ActualWidth > 0 ? MandelbrotImage.ActualWidth : 640,
            MandelbrotImage.ActualHeight > 0 ? MandelbrotImage.ActualHeight : 480
        );
    }

    private void MandelbrotWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if(MandelbrotImage.ActualWidth > 0 && MandelbrotImage.ActualHeight > 0)
        {
            mainWindow.UpdateMandelbrot();
        }
    }

    private void MandelbrotImage_MouseDown(object sender, MouseButtonEventArgs e)
    {
        Point pos = e.GetPosition(MandelbrotImage);

        // Convert to normalized coordinates (0 to 1)
        double normalizedX = pos.X / MandelbrotImage.ActualWidth;
        double normalizedY = pos.Y / MandelbrotImage.ActualHeight;

        // Transform to match shader coordinates
        double minDim = Math.Min(MandelbrotImage.ActualWidth, MandelbrotImage.ActualHeight);
        double newX = (normalizedX - 0.5) * MandelbrotImage.ActualWidth / minDim / (mainWindow.Zoom * 0.5) + mainWindow.CenterX;
        double newY = (normalizedY - 0.5) * MandelbrotImage.ActualHeight / minDim / (mainWindow.Zoom * 0.5) + mainWindow.CenterY;

        mainWindow.CenterX = newX;
        mainWindow.CenterY = newY;
        mainWindow.UpdateMandelbrot();
    }


    private Color GetColor(int iteration, RenderSettings settings)
    {
        if(iteration == settings.MaxIterations)
            return Colors.Black;

        double hue = (double)iteration / settings.MaxIterations;

        switch(settings.ColorMode)
        {
            case ColorMode.Rainbow:
                return HsvToRgb(hue * 360, 1, 1);

            case ColorMode.Grayscale:
                byte value = (byte)(hue * 255);
                return Color.FromRgb(value, value, value);

            case ColorMode.BlueYellow:
                byte blue = (byte)((1 - hue) * 255);
                byte yellow = (byte)(hue * 255);
                return Color.FromRgb(yellow, yellow, blue);

            default:
                return HsvToRgb(hue * 360, 1, 1);
        }
    }

    private int CalculateMandelbrot(double x0, double y0, int maxIterations)
    {
        double x = 0;
        double y = 0;
        int iteration = 0;

        while(x * x + y * y <= 4 && iteration < maxIterations)
        {
            double xtemp = x*x - y*y + x0;
            y = 2 * x * y + y0;
            x = xtemp;
            iteration++;
        }
        return iteration;
    }



    private static Color HsvToRgb(double hue, double saturation, double value)
    {
        int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
        double f = hue / 60 - Math.Floor(hue / 60);

        value = value * 255;
        byte v = Convert.ToByte(value);
        byte p = Convert.ToByte(value * (1 - saturation));
        byte q = Convert.ToByte(value * (1 - f * saturation));
        byte t = Convert.ToByte(value * (1 - (1 - f) * saturation));

        if(hi == 0)
            return Color.FromRgb(v, t, p);
        else if(hi == 1)
            return Color.FromRgb(q, v, p);
        else if(hi == 2)
            return Color.FromRgb(p, v, t);
        else if(hi == 3)
            return Color.FromRgb(p, q, v);
        else if(hi == 4)
            return Color.FromRgb(t, p, v);
        else
            return Color.FromRgb(v, p, q);
    }
}