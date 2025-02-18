using System.Windows.Input;

namespace RapidXaml;

public partial class EventToCommand : BindableObject
{
	public static ICommand GetFocused(BindableObject obj)
		=> (ICommand)obj.GetValue(FocusedProperty);

	public static void SetFocused(BindableObject obj, ICommand value)
		=> obj.SetValue(FocusedProperty, value);

	public static readonly BindableProperty FocusedProperty =
		BindableProperty.CreateAttached("Focused", typeof(ICommand), typeof(EventToCommand), defaultValue: null, propertyChanged: OnFocusedChanged);

	private static void OnFocusedChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is VisualElement ve)
		{
			if (oldValue is ICommand oldCmd)
			{
				for (int i = ve.Behaviors.Count; i >= 0; i--)
				{
					if (ve.Behaviors[i] is EventToCommandFocusedBehavior oldBehavior && oldBehavior.CommandToInvoke == oldCmd)
					{
						ve.Behaviors.Remove(oldBehavior);
						break;
					}
				}
			}

			if (newValue is ICommand newCmd)
			{
				ve.Behaviors.Add(new EventToCommandFocusedBehavior { CommandToInvoke = newCmd });
			}
		}
	}

	internal partial class EventToCommandFocusedBehavior : Behavior<VisualElement>
	{
		public ICommand? CommandToInvoke { get; set; }

		protected override void OnAttachedTo(VisualElement vizElement)
		{
			vizElement.Focused += OnVisualElementFocused;
			base.OnAttachedTo(vizElement);
		}

		protected override void OnDetachingFrom(VisualElement vizElement)
		{
			vizElement.Focused -= OnVisualElementFocused;
			base.OnDetachingFrom(vizElement);
		}

		private void OnVisualElementFocused(object? sender, EventArgs e)
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
