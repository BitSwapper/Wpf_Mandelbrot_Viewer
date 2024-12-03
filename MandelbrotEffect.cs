using System.Windows.Media.Effects;
using System.Windows.Media;
using System.Windows;
using System.IO;

public class MandelbrotEffect : ShaderEffect
{
    private static PixelShader pixelShader = new PixelShader();

    public static readonly DependencyProperty InputProperty =
        RegisterPixelShaderSamplerProperty("input", typeof(MandelbrotEffect), 0);

    public MandelbrotEffect()
    {
        try
        {
            string shaderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mandelbrot.ps");
            pixelShader.UriSource = new Uri(shaderPath);
            this.PixelShader = pixelShader;

            // Important: Set input to null since we're using the Rectangle's Fill
            this.SetValue(InputProperty, null);
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
}
