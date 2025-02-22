using System.Windows.Input;

namespace RapidXaml;

public partial class EventToCommand : BindableObject
{
	public static ICommand GetScrolled(BindableObject obj)
		=> (ICommand)obj.GetValue(ScrolledProperty);

	public static void SetScrolled(BindableObject obj, ICommand value)
		=> obj.SetValue(ScrolledProperty, value);

	public static readonly BindableProperty ScrolledProperty =
		BindableProperty.CreateAttached("Scrolled", typeof(ICommand), typeof(EventToCommand), defaultValue: null, propertyChanged: OnScrolledChanged);

	private static void OnScrolledChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is ScrollView sv)
		{
			if (oldValue is ICommand oldCmd)
			{
				for (int i = sv.Behaviors.Count; i >= 0; i--)
				{
					if (sv.Behaviors[i] is EventToCommandScrolledBehavior oldBehavior && oldBehavior.CommandToInvoke == oldCmd)
					{
						sv.Behaviors.Remove(oldBehavior);
						break;
					}
				}
			}

			if (newValue is ICommand newCmd)
			{
				sv.Behaviors.Add(new EventToCommandScrolledBehavior { CommandToInvoke = newCmd });
			}
		}
	}

	internal partial class EventToCommandScrolledBehavior : Behavior<ScrollView>
	{
		public ICommand? CommandToInvoke { get; set; }

		protected override void OnAttachedTo(ScrollView sv)
		{
			sv.Scrolled += OnScrollViewerScrolled;
			base.OnAttachedTo(sv);
		}

		protected override void OnDetachingFrom(ScrollView sv)
		{
			sv.Scrolled -= OnScrollViewerScrolled;
			base.OnDetachingFrom(sv);
		}

		private void OnScrollViewerScrolled(object? sender, EventArgs e)
		{
			if (this.CommandToInvoke != null)
			{
				if (this.CommandToInvoke.CanExecute(null))
				{
					this.CommandToInvoke.Execute(null);
				}
			}
		}
	}
}
