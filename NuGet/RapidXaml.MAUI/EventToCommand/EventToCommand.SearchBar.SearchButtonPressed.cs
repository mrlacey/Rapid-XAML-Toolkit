using System.Windows.Input;

namespace RapidXaml;

public partial class EventToCommand : BindableObject
{
	public static ICommand GetSearchButtonPressed(BindableObject obj)
		=> (ICommand)obj.GetValue(SearchButtonPressedProperty);

	public static void SetSearchButtonPressed(BindableObject obj, ICommand value)
		=> obj.SetValue(SearchButtonPressedProperty, value);

	public static readonly BindableProperty SearchButtonPressedProperty =
		BindableProperty.CreateAttached("SearchButtonPressed", typeof(ICommand), typeof(EventToCommand), defaultValue: null, propertyChanged: OnSearchButtonPressedChanged);

	private static void OnSearchButtonPressedChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is SearchBar sb)
		{
			if (oldValue is ICommand oldCmd)
			{
				for (int i = sb.Behaviors.Count; i >= 0; i--)
				{
					if (sb.Behaviors[i] is EventToCommandSearchButtonPressedBehavior oldBehavior && oldBehavior.CommandToInvoke == oldCmd)
					{
						sb.Behaviors.Remove(oldBehavior);
						break;
					}
				}
			}

			if (newValue is ICommand newCmd)
			{
				sb.Behaviors.Add(new EventToCommandSearchButtonPressedBehavior { CommandToInvoke = newCmd });
			}
		}
	}

	internal partial class EventToCommandSearchButtonPressedBehavior : Behavior<SearchBar>
	{
		public ICommand? CommandToInvoke { get; set; }

		protected override void OnAttachedTo(SearchBar sb)
		{
			sb.SearchButtonPressed += OnSearchBarSearchButtonPressed;
			base.OnAttachedTo(sb);
		}

		protected override void OnDetachingFrom(SearchBar sb)
		{
			sb.SearchButtonPressed -= OnSearchBarSearchButtonPressed;
			base.OnDetachingFrom(sb);
		}

		private void OnSearchBarSearchButtonPressed(object? sender, EventArgs e)
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
