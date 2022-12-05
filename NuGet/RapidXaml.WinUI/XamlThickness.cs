namespace RapidXaml;

public class XamlThickness : DependencyObject
{
    public double Left
    {
        get => (double)GetValue(LeftProperty);
        set => SetValue(LeftProperty, value);
    }
    public double Top
    {
        get => (double)GetValue(TopProperty);
        set => SetValue(TopProperty, value);
    }
    public double Right
    {
        get => (double)GetValue(RightProperty);
        set => SetValue(RightProperty, value);
    }
    public double Bottom
    {
        get => (double)GetValue(BottomProperty);
        set => SetValue(BottomProperty, value);
    }

    public static readonly DependencyProperty LeftProperty = DependencyProperty.Register(nameof(Left), typeof(double), typeof(XamlThickness), new PropertyMetadata(0.0));

    public static readonly DependencyProperty TopProperty = DependencyProperty.Register(nameof(Top), typeof(double), typeof(XamlThickness), new PropertyMetadata(0.0));

    public static readonly DependencyProperty RightProperty = DependencyProperty.Register(nameof(Right), typeof(double), typeof(XamlThickness), new PropertyMetadata(0.0));

    public static readonly DependencyProperty BottomProperty = DependencyProperty.Register(nameof(Bottom), typeof(double), typeof(XamlThickness), new PropertyMetadata(0.0));

    public static implicit operator Thickness(XamlThickness xt)
    {
        return new Thickness(xt.Left, xt.Top, xt.Right, xt.Bottom);
    }
}