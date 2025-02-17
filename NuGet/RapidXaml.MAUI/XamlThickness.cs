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

	public double All
	{
		get => (double)GetValue(AllProperty);
		set => SetValue(AllProperty, value);
	}

	public double Vertical
	{
		get => (double)GetValue(VerticalProperty);
		set => SetValue(VerticalProperty, value);
	}

	public double Horizontal
	{
		get => (double)GetValue(HorizontalProperty);
		set => SetValue(HorizontalProperty, value);
	}

	public static readonly BindableProperty LeftProperty = BindableProperty.Create(nameof(Left), typeof(double), typeof(XamlThickness), 0.0);

	public static readonly BindableProperty TopProperty = BindableProperty.Create(nameof(Top), typeof(double), typeof(XamlThickness), 0.0);

	public static readonly BindableProperty RightProperty = BindableProperty.Create(nameof(Right), typeof(double), typeof(XamlThickness), 0.0);

	public static readonly BindableProperty BottomProperty = BindableProperty.Create(nameof(Bottom), typeof(double), typeof(XamlThickness), 0.0);

	public static readonly BindableProperty AllProperty = BindableProperty.Create(nameof(All), typeof(double), typeof(XamlThickness), 0.0, propertyChanged: OnAllChanged);

	public static readonly BindableProperty VerticalProperty = BindableProperty.Create(nameof(Vertical), typeof(double), typeof(XamlThickness), 0.0, propertyChanged: OnVerticalChanged);

	public static readonly BindableProperty HorizontalProperty = BindableProperty.Create(nameof(Horizontal), typeof(double), typeof(XamlThickness), 0.0, propertyChanged: OnHorizontalChanged);

	private static void OnAllChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is XamlThickness xt && newValue is double newSize)
		{
			xt.Left = xt.Top = xt.Right = xt.Bottom = newSize;
		}
	}

	private static void OnHorizontalChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is XamlThickness xt && newValue is double newSize)
		{
			xt.Left = xt.Right = newSize;
		}
	}

	private static void OnVerticalChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is XamlThickness xt && newValue is double newSize)
		{
			xt.Top = xt.Bottom = newSize;
		}
	}

	public static implicit operator Thickness(XamlThickness xt)
	{
		return new Thickness(xt.Left, xt.Top, xt.Right, xt.Bottom);
	}
}
