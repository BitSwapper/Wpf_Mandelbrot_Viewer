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
        //mandelbrotEffect.Center = new Point(centerX, centerY);
        //mandelbrotEffect.Zoom = zoom;
        //mandelbrotEffect.MaxIterations = settings.MaxIterations;
        //mandelbrotEffect.Resolution = new Point(MandelbrotImage.ActualWidth, MandelbrotImage.ActualHeight);
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
        double newX = (pos.X - MandelbrotImage.ActualWidth/2) / (MandelbrotImage.ActualWidth/4) / mainWindow.Zoom + mainWindow.CenterX;
        double newY = (pos.Y - MandelbrotImage.ActualHeight/2) / (MandelbrotImage.ActualHeight/4) / mainWindow.Zoom + mainWindow.CenterY;
        mainWindow.CenterX = newX;
        mainWindow.CenterY = newY;
        mainWindow.UpdateMandelbrot();
    }

   

    //private void InitializeBitmap()
    //{
    //    var width = MandelbrotImage.ActualWidth == 0 ? this.Width : MandelbrotImage.ActualWidth;
    //    var height = MandelbrotImage.ActualHeight == 0 ? this.Height : MandelbrotImage.ActualHeight;
    //    mandelbrotBitmap = new WriteableBitmap((int)width, (int)height, 72, 72, PixelFormats.Rgb24, null);
    //    MandelbrotImage.Source = mandelbrotBitmap;
    //}

    //public void UpdateMandelbrot(double centerX, double centerY, double zoom, RenderSettings settings)
    //{
    //    if(mandelbrotBitmap == null) return;

    //    int width = mandelbrotBitmap.PixelWidth;
    //    int height = mandelbrotBitmap.PixelHeight;

    //    // Apply quality settings
    //    int skip = settings.Quality switch
    //    {
    //        RenderQuality.Draft => 4,
    //        RenderQuality.Normal => 1,
    //        RenderQuality.High => 1,
    //        _ => 1
    //    };

    //    byte[] pixels = new byte[width * height * 3];

    //    for(int x = 0; x < width; x += skip)
    //    {
    //        for(int y = 0; y < height; y += skip)
    //        {
    //            double x0 = (x - width/2.0) / (width/4.0) / zoom + centerX;
    //            double y0 = (y - height/2.0) / (height/4.0) / zoom + centerY;

    //            int iteration = CalculateMandelbrot(x0, y0, settings.MaxIterations);
    //            Color color = GetColor(iteration, settings);

    //            // Fill skipped pixels for draft mode
    //            for(int dx = 0; dx < skip && x + dx < width; dx++)
    //            {
    //                for(int dy = 0; dy < skip && y + dy < height; dy++)
    //                {
    //                    int pixelOffset = ((y + dy) * width + (x + dx)) * 3;
    //                    pixels[pixelOffset] = color.R;
    //                    pixels[pixelOffset + 1] = color.G;
    //                    pixels[pixelOffset + 2] = color.B;
    //                }
    //            }
    //        }
    //    }

    //    mandelbrotBitmap.WritePixels(
    //        new Int32Rect(0, 0, width, height),
    //        pixels, width * 3, 0);
    //}
    //private void MandelbrotImage_MouseDown(object sender, MouseButtonEventArgs e)
    //{
    //    Point pos = e.GetPosition(MandelbrotImage);

    //    double newX = (pos.X - MandelbrotImage.ActualWidth/2) / (MandelbrotImage.ActualWidth/4) / mainWindow.Zoom + mainWindow.CenterX;
    //    double newY = (pos.Y - MandelbrotImage.ActualHeight/2) / (MandelbrotImage.ActualHeight/4) / mainWindow.Zoom + mainWindow.CenterY;
    //    mainWindow.CenterX = newX;
    //    mainWindow.CenterY = newY;
    //    mainWindow.UpdateMandelbrot();
    //}
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