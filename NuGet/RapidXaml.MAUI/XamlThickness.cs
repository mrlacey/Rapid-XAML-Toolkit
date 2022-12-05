namespace RapidXaml;

public class XamlThickness : BindableObject
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

    public static readonly BindableProperty LeftProperty = BindableProperty.Create(nameof(Left), typeof(double), typeof(XamlThickness), 0.0);

    public static readonly BindableProperty TopProperty = BindableProperty.Create(nameof(Top), typeof(double), typeof(XamlThickness), 0.0);

    public static readonly BindableProperty RightProperty = BindableProperty.Create(nameof(Right), typeof(double), typeof(XamlThickness), 0.0);

    public static readonly BindableProperty BottomProperty = BindableProperty.Create(nameof(Bottom), typeof(double), typeof(XamlThickness), 0.0);

    public static implicit operator Thickness(XamlThickness xt)
    {
        return new Thickness(xt.Left, xt.Top, xt.Right, xt.Bottom);
    }
}
