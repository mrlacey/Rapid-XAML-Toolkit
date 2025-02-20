namespace RapidXaml;

internal class CircularGradientExtension : IMarkupExtension<Brush>, IMarkupExtension
{
	public required Color Center { get; set; } = Colors.White;

	public required Color Edge { get; set; } = Colors.Black;

	public required double CenterRadius { get; set; } = 0.5;

	Brush IMarkupExtension<Brush>.ProvideValue(IServiceProvider serviceProvider)
		=> new RadialGradientBrush(
				gradientStops: [
					new GradientStop(Center, 0.1f),
					new GradientStop(Edge, 1.0f),
					],
				radius: CenterRadius
				);

	public object ProvideValue(IServiceProvider serviceProvider)
		=> ((IMarkupExtension<Brush>)this).ProvideValue(serviceProvider);
}
