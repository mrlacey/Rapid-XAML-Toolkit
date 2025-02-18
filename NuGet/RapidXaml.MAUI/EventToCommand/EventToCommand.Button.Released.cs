using System.Windows.Input;

namespace RapidXaml;

public partial class EventToCommand : BindableObject
{
	public static ICommand GetReleased(BindableObject obj)
		=> (ICommand)obj.GetValue(ReleasedProperty);

	public static void SetReleased(BindableObject obj, ICommand value)
		=> obj.SetValue(ReleasedProperty, value);

	public static readonly BindableProperty ReleasedProperty =
		BindableProperty.CreateAttached("Released", typeof(ICommand), typeof(EventToCommand), defaultValue: null, propertyChanged: OnReleasedChanged);

	private static void OnReleasedChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is Button btn)
		{
			if (oldValue is ICommand oldCmd)
			{
				for (int i = btn.Behaviors.Count; i >= 0; i--)
				{
					if (btn.Behaviors[i] is EventToCommandReleasedBehavior oldBehavior && oldBehavior.CommandToInvoke == oldCmd)
					{
						btn.Behaviors.Remove(oldBehavior);
						break;
					}
				}
			}

			if (newValue is ICommand newCmd)
			{
				btn.Behaviors.Add(new EventToCommandReleasedBehavior { CommandToInvoke = newCmd });
			}
		}
	}

	internal partial class EventToCommandReleasedBehavior : Behavior<Button>
	{
		public ICommand? CommandToInvoke { get; set; }

		protected override void OnAttachedTo(Button btn)
		{
			btn.Released += OnButtonReleased;
			base.OnAttachedTo(btn);
		}

		protected override void OnDetachingFrom(Button btn)
		{
			btn.Released -= OnButtonReleased;
			base.OnDetachingFrom(btn);
		}

		private void OnButtonReleased(object? sender, EventArgs e)
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
