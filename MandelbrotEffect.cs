using System.Windows.Media.Effects;
using System.Windows.Media;
using System.Windows;
using System.IO;

public class MandelbrotEffect : ShaderEffect
{
    private static PixelShader pixelShader = new PixelShader();

    public static readonly DependencyProperty InputProperty =
        RegisterPixelShaderSamplerProperty("input", typeof(MandelbrotEffect), 0);

    public static readonly DependencyProperty CenterProperty =
        DependencyProperty.Register("Center", typeof(Point), typeof(MandelbrotEffect),
            new UIPropertyMetadata(new Point(0, 0), PixelShaderConstantCallback(0)));

    public static readonly DependencyProperty ZoomProperty =
        DependencyProperty.Register("Zoom", typeof(double), typeof(MandelbrotEffect),
            new UIPropertyMetadata(1.0, PixelShaderConstantCallback(1)));

    public static readonly DependencyProperty MaxIterationsProperty =
        DependencyProperty.Register("MaxIterations", typeof(double), typeof(MandelbrotEffect),
            new UIPropertyMetadata(1000.0, PixelShaderConstantCallback(2)));

    public static readonly DependencyProperty ResolutionProperty =
        DependencyProperty.Register("Resolution", typeof(Point), typeof(MandelbrotEffect),
            new UIPropertyMetadata(new Point(1920, 1080), PixelShaderConstantCallback(3)));

    public MandelbrotEffect()
    {
        try
        {
            string shaderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mandelbrot.ps");
            pixelShader.UriSource = new Uri(shaderPath);
            this.PixelShader = pixelShader;

            // Important: Set input to null since we're using the Rectangle's Fill
            this.SetValue(InputProperty, null);

            // Initialize default values
            UpdateShaderValue(CenterProperty);
            UpdateShaderValue(ZoomProperty);
            UpdateShaderValue(MaxIterationsProperty);
            UpdateShaderValue(ResolutionProperty);
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Shader initialization failed: {ex.Message}");
        }
    }

    public Brush Input
    {
        get { return (Brush)GetValue(InputProperty); }
        set { SetValue(InputProperty, value); }
    }

    public Point Center
    {
        get { return (Point)GetValue(CenterProperty); }
        set { SetValue(CenterProperty, value); }
    }

    public double Zoom
    {
        get { return (double)GetValue(ZoomProperty); }
        set { SetValue(ZoomProperty, value); }
    }

    public double MaxIterations
    {
        get { return (double)GetValue(MaxIterationsProperty); }
        set { SetValue(MaxIterationsProperty, value); }
    }

    public Point Resolution
    {
        get { return (Point)GetValue(ResolutionProperty); }
        set { SetValue(ResolutionProperty, value); }
    }
}