using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RapidXaml;

internal class FontImage : IMarkupExtension<ImageSource>, IMarkupExtension
{
	public required string? Glyph { get; set; }

	public required Color? Color { get; set; }

	public required string? FontFamily { get; set; }

	public required double? Size { get; set; }

	ImageSource IMarkupExtension<ImageSource>.ProvideValue(IServiceProvider serviceProvider)
	{
		var result = new FontImageSource();

		if (this.Glyph is not null)
		{
			result.Glyph = this.Glyph;
		}

		if (this.Color is not null)
		{
			result.Color = this.Color;
		}

		if (this.FontFamily is not null)
		{
			result.FontFamily = this.FontFamily;
		}

		if (this.Size is not null && this.Size.HasValue)
		{
			result.Size = this.Size.Value;
		}

		return result;
	}

	public object ProvideValue(IServiceProvider serviceProvider)
		=> ((IMarkupExtension<ImageSource>)this).ProvideValue(serviceProvider);
}
