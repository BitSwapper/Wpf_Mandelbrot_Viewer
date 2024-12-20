﻿using System.Windows;
using System.Windows.Input;

namespace WpfMandelbrot;

public partial class MandelbrotWindow : Window
{
    MainWindow mainWindow;
    MandelbrotEffect mandelbrotEffect;

    public MandelbrotWindow(MainWindow main)
    {
        InitializeComponent();
        mainWindow = main;

        mandelbrotEffect = new MandelbrotEffect();
        MandelbrotImage.Effect = mandelbrotEffect;

        this.Loaded += (s, e) => UpdateMandelbrot(0, 0, 1, new RenderSettings
        {
            MaxIterations = 600,
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
        mandelbrotEffect.ColorMode = (double)settings.ColorMode;
    }

    void MandelbrotWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if(MandelbrotImage.ActualWidth > 0 && MandelbrotImage.ActualHeight > 0)
        {
            mainWindow.UpdateMandelbrot();
        }
    }

    void MandelbrotImage_MouseDown(object sender, MouseButtonEventArgs e)
    {
        Point pos = e.GetPosition(MandelbrotImage);

        double normalizedX = pos.X / MandelbrotImage.ActualWidth;
        double normalizedY = pos.Y / MandelbrotImage.ActualHeight;

        double minDim = Math.Min(MandelbrotImage.ActualWidth, MandelbrotImage.ActualHeight);
        double newX = (normalizedX - 0.5) * MandelbrotImage.ActualWidth / minDim / (mainWindow.Zoom * 0.5) + mainWindow.CenterX;
        double newY = (normalizedY - 0.5) * MandelbrotImage.ActualHeight / minDim / (mainWindow.Zoom * 0.5) + mainWindow.CenterY;

        mainWindow.CenterX = newX;
        mainWindow.CenterY = newY;
        mainWindow.UpdateMandelbrot();
    }
}