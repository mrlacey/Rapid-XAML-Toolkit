namespace RapidXaml;

internal class VerticalGradientExtension : IMarkupExtension<Brush>, IMarkupExtension
{
	public required Color Top { get; set; } = Colors.White;

	public required Color Bottom { get; set; } = Colors.Black;

	Brush IMarkupExtension<Brush>.ProvideValue(IServiceProvider serviceProvider)
		=> new LinearGradientBrush(
				gradientStops: [
					new GradientStop(Top, 0.1f),
					new GradientStop(Bottom, 1.0f),
					],
				startPoint: new Point(1, 0),
				endPoint: new Point(1, 1)
				);

	public object ProvideValue(IServiceProvider serviceProvider)
		=> ((IMarkupExtension<Brush>)this).ProvideValue(serviceProvider);
}
