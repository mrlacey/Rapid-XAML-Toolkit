namespace RapidXaml;

internal class HorizontalGradientExtension : IMarkupExtension<Brush>, IMarkupExtension
{
	public Color Left { get; set; } = Colors.White;

	public Color Right { get; set; } = Colors.Black;

	Brush IMarkupExtension<Brush>.ProvideValue(IServiceProvider serviceProvider)
		=> new LinearGradientBrush(
				gradientStops: [
					new GradientStop(Left, 0.1f),
					new GradientStop(Right, 1.0f),
					],
				startPoint: new Point(0, 0),
				endPoint: new Point(1, 0)
				);

	public object ProvideValue(IServiceProvider serviceProvider)
		=> ((IMarkupExtension<Brush>)this).ProvideValue(serviceProvider);
}
