namespace RapidXaml;

public partial class EventToCommand : BindableObject
{
	public static Command<SelectionChangedEventArgs> GetSelectionChanged(BindableObject obj)
		=> (Command<SelectionChangedEventArgs>)obj.GetValue(SelectionChangedProperty);

	public static void SetSelectionChanged(BindableObject obj, Command<SelectionChangedEventArgs> value)
		=> obj.SetValue(SelectionChangedProperty, value);

	public static readonly BindableProperty SelectionChangedProperty =
		BindableProperty.CreateAttached("SelectionChanged", typeof(Command<SelectionChangedEventArgs>), typeof(EventToCommand), defaultValue: null, propertyChanged: OnSelectionChangedChanged);

	private static void OnSelectionChangedChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is SelectableItemsView siv)
		{
			if (oldValue is Command<SelectionChangedEventArgs> oldCmd)
			{
				for (int i = siv.Behaviors.Count - 1; i >= 0; i--)
				{
					if (siv.Behaviors[i] is EventToCommandSelectionChangedBehavior oldBehavior && oldBehavior.CommandToInvoke == oldCmd)
					{
						siv.Behaviors.Remove(oldBehavior);
						break;
					}
				}
			}

			if (newValue is Command<SelectionChangedEventArgs> newCmd)
			{
				siv.Behaviors.Add(new EventToCommandSelectionChangedBehavior { CommandToInvoke = newCmd });
			}
		}
	}

	internal partial class EventToCommandSelectionChangedBehavior : Behavior<SelectableItemsView>
	{
		public Command<SelectionChangedEventArgs>? CommandToInvoke { get; set; }

		protected override void OnAttachedTo(SelectableItemsView selItemsView)
		{
			selItemsView.SelectionChanged += OnSelectableItemsViewSelectionChanged;
			base.OnAttachedTo(selItemsView);
		}

		protected override void OnDetachingFrom(SelectableItemsView selItemsView)
		{
			selItemsView.Focused -= OnSelectableItemsViewSelectionChanged;
			base.OnDetachingFrom(selItemsView);
		}

		private void OnSelectableItemsViewSelectionChanged(object? sender, EventArgs e)
		{
			if (this.CommandToInvoke != null)
			{
				if (this.CommandToInvoke.CanExecute(null))
				{
					this.CommandToInvoke.Execute(e);
				}
			}
		}
	}
}
