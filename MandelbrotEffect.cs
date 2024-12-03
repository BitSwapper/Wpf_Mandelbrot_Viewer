using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

public class MandelbrotEffect : ShaderEffect
{
 static PixelShader pixelShader = new PixelShader();

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

    public static readonly DependencyProperty ColorModeProperty =
    DependencyProperty.Register("ColorMode", typeof(double), typeof(MandelbrotEffect),
        new UIPropertyMetadata(0.0, PixelShaderConstantCallback(4)));


    public MandelbrotEffect()
    {
        try
        {
            string shaderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mandelbrot.ps");
            pixelShader.UriSource = new Uri(shaderPath);
            this.PixelShader = pixelShader;

            this.SetValue(InputProperty, null);

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

    public double ColorMode
    {
        get => (double)GetValue(ColorModeProperty);
        set => SetValue(ColorModeProperty, value);
    }

    public Brush Input
    {
        get => (Brush)GetValue(InputProperty);
        set => SetValue(InputProperty, value);
    }

    public Point Center
    {
        get => (Point)GetValue(CenterProperty);
        set => SetValue(CenterProperty, value);
    }

    public double Zoom
    {
        get => (double)GetValue(ZoomProperty);
        set => SetValue(ZoomProperty, value);
    }

    public double MaxIterations
    {
        get => (double)GetValue(MaxIterationsProperty);
        set => SetValue(MaxIterationsProperty, value);
    }

    public Point Resolution
    {
        get => (Point)GetValue(ResolutionProperty);
        set => SetValue(ResolutionProperty, value);
    }
}

//"C:\Program Files (x86)\Windows Kits\10\bin\10.0.19041.0\x64\fxc.exe" /T ps_3_0 /E main /Fo "F:\Coding Projects\C#\WpfMandelbrot\WpfMandelbrot\bin\Debug\net8.0-windows\Mandelbrot.ps" "F:\Coding Projects\C#\WpfMandelbrot\WpfMandelbrot\Shaders\Mandelbrot.hlsl"