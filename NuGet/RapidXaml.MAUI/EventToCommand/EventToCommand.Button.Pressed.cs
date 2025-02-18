using System.Windows.Input;

namespace RapidXaml;

public partial class EventToCommand : BindableObject
{
	public static ICommand GetPressed(BindableObject obj)
		=> (ICommand)obj.GetValue(PressedProperty);

	public static void SetPressed(BindableObject obj, ICommand value)
		=> obj.SetValue(PressedProperty, value);

	public static readonly BindableProperty PressedProperty =
		BindableProperty.CreateAttached("Pressed", typeof(ICommand), typeof(EventToCommand), defaultValue: null, propertyChanged: OnPressedChanged);

	private static void OnPressedChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is Button btn)
		{
			if (oldValue is ICommand oldCmd)
			{
				for (int i = btn.Behaviors.Count; i >= 0; i--)
				{
					if (btn.Behaviors[i] is EventToCommandPressedBehavior oldBehavior && oldBehavior.CommandToInvoke == oldCmd)
					{
						btn.Behaviors.Remove(oldBehavior);
						break;
					}
				}
			}

			if (newValue is ICommand newCmd)
			{
				btn.Behaviors.Add(new EventToCommandPressedBehavior { CommandToInvoke = newCmd });
			}
		}
	}

	internal partial class EventToCommandPressedBehavior : Behavior<Button>
	{
		public ICommand? CommandToInvoke { get; set; }

		protected override void OnAttachedTo(Button btn)
		{
			btn.Pressed += OnButtonPressed;
			base.OnAttachedTo(btn);
		}

		protected override void OnDetachingFrom(Button btn)
		{
			btn.Pressed -= OnButtonPressed;
			base.OnDetachingFrom(btn);
		}

		private void OnButtonPressed(object? sender, EventArgs e)
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
