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

    public double All
    {
        get { return (double)GetValue(AllProperty); }
        set { SetValue(AllProperty, value); }
    }

    public double Horizontal
    {
        get { return (double)GetValue(HorizontalProperty); }
        set { SetValue(HorizontalProperty, value); }
    }

    public double Vertical
    {
        get { return (double)GetValue(VerticalProperty); }
        set { SetValue(VerticalProperty, value); }
    }

    public static readonly DependencyProperty LeftProperty = DependencyProperty.Register(nameof(Left), typeof(double), typeof(XamlThickness), new PropertyMetadata(0.0));

    public static readonly DependencyProperty TopProperty = DependencyProperty.Register(nameof(Top), typeof(double), typeof(XamlThickness), new PropertyMetadata(0.0));

    public static readonly DependencyProperty RightProperty = DependencyProperty.Register(nameof(Right), typeof(double), typeof(XamlThickness), new PropertyMetadata(0.0));

    public static readonly DependencyProperty BottomProperty = DependencyProperty.Register(nameof(Bottom), typeof(double), typeof(XamlThickness), new PropertyMetadata(0.0));

    public static readonly DependencyProperty AllProperty = DependencyProperty.Register(nameof(All), typeof(double), typeof(XamlThickness), new PropertyMetadata(0.0, OnAllChanged));

    public static readonly DependencyProperty HorizontalProperty = DependencyProperty.Register(nameof(Horizontal), typeof(double), typeof(XamlThickness), new PropertyMetadata(0.0, OnHorizontalChanged));

    public static readonly DependencyProperty VerticalProperty = DependencyProperty.Register(nameof(Vertical), typeof(double), typeof(XamlThickness), new PropertyMetadata(0.0, OnVerticalChanged));

    private static void OnAllChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is XamlThickness xt && e.NewValue is double newSize)
        {
            xt.Left = xt.Top = xt.Right = xt.Bottom = newSize;
        }
    }

    private static void OnHorizontalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is XamlThickness xt && e.NewValue is double newSize)
        {
            xt.Left = xt.Right = newSize;
        }
    }

    private static void OnVerticalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is XamlThickness xt && e.NewValue is double newSize)
        {
            xt.Top = xt.Bottom = newSize;
        }
    }

    public static implicit operator Thickness(XamlThickness xt)
    {
        return new Thickness(xt.Left, xt.Top, xt.Right, xt.Bottom);
    }
}
