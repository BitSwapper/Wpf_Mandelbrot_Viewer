namespace WpfMandelbrot;

public class RenderSettings
{
    public int MaxIterations { get; set; }
    public ColorMode ColorMode { get; set; }
    public RenderQuality Quality { get; set; }
    public bool SmoothColors { get; set; }
}