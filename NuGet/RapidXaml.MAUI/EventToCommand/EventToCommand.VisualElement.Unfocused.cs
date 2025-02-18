using System.Windows.Input;

namespace RapidXaml;

public partial class EventToCommand : BindableObject
{
	public static ICommand GetUnfocused(BindableObject obj)
		=> (ICommand)obj.GetValue(UnfocusedProperty);

	public static void SetUnfocused(BindableObject obj, ICommand value)
		=> obj.SetValue(UnfocusedProperty, value);

	public static readonly BindableProperty UnfocusedProperty =
		BindableProperty.CreateAttached("Unfocused", typeof(ICommand), typeof(EventToCommand), defaultValue: null, propertyChanged: OnUnfocusedChanged);

	private static void OnUnfocusedChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is VisualElement ve)
		{
			if (oldValue is ICommand oldCmd)
			{
				for (int i = ve.Behaviors.Count; i >= 0; i--)
				{
					if (ve.Behaviors[i] is EventToCommandUnfocusedBehavior oldBehavior && oldBehavior.CommandToInvoke == oldCmd)
					{
						ve.Behaviors.Remove(oldBehavior);
						break;
					}
				}
			}

			if (newValue is ICommand newCmd)
			{
				ve.Behaviors.Add(new EventToCommandUnfocusedBehavior { CommandToInvoke = newCmd });
			}
		}
	}

	internal partial class EventToCommandUnfocusedBehavior : Behavior<VisualElement>
	{
		public ICommand? CommandToInvoke { get; set; }

		protected override void OnAttachedTo(VisualElement vizElement)
		{
			vizElement.Unfocused += OnVisualElementUnfocused;
			base.OnAttachedTo(vizElement);
		}

		protected override void OnDetachingFrom(VisualElement vizElement)
		{
			vizElement.Unfocused -= OnVisualElementUnfocused;
			base.OnDetachingFrom(vizElement);
		}

		private void OnVisualElementUnfocused(object? sender, EventArgs e)
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
