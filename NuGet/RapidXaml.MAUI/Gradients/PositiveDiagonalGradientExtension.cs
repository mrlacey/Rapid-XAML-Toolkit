namespace RapidXaml;

internal class PositiveDiagonalGradientExtension : IMarkupExtension<Brush>, IMarkupExtension
{
	public required Color Start { get; set; } = Colors.White;

	public required Color End { get; set; } = Colors.Black;

	Brush IMarkupExtension<Brush>.ProvideValue(IServiceProvider serviceProvider)
		=> new LinearGradientBrush(
				gradientStops: [
					new GradientStop(Start, 0.1f),
					new GradientStop(End, 1.0f),
					],
				startPoint: new Point(0, 1),
				endPoint: new Point(1, 0)
				);

	public object ProvideValue(IServiceProvider serviceProvider)
		=> ((IMarkupExtension<Brush>)this).ProvideValue(serviceProvider);
}
