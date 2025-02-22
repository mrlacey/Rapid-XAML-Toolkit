using System.Windows.Input;

namespace RapidXaml;

public partial class EventToCommand : BindableObject
{
	public static ICommand GetToggled(BindableObject obj)
		=> (ICommand)obj.GetValue(ToggledProperty);

	public static void SetToggled(BindableObject obj, ICommand value)
		=> obj.SetValue(ToggledProperty, value);

	public static readonly BindableProperty ToggledProperty =
		BindableProperty.CreateAttached("Toggled", typeof(ICommand), typeof(EventToCommand), defaultValue: null, propertyChanged: OnToggledChanged);

	private static void OnToggledChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is Switch swtch)
		{
			if (oldValue is ICommand oldCmd)
			{
				for (int i = swtch.Behaviors.Count; i >= 0; i--)
				{
					if (swtch.Behaviors[i] is EventToCommandToggledBehavior oldBehavior && oldBehavior.CommandToInvoke == oldCmd)
					{
						swtch.Behaviors.Remove(oldBehavior);
						break;
					}
				}
			}

			if (newValue is ICommand newCmd)
			{
				swtch.Behaviors.Add(new EventToCommandToggledBehavior { CommandToInvoke = newCmd });
			}
		}
	}

	internal partial class EventToCommandToggledBehavior : Behavior<Switch>
	{
		public ICommand? CommandToInvoke { get; set; }

		protected override void OnAttachedTo(Switch btn)
		{
			btn.Toggled += OnSwitchToggled;
			base.OnAttachedTo(btn);
		}

		protected override void OnDetachingFrom(Switch btn)
		{
			btn.Toggled -= OnSwitchToggled;
			base.OnDetachingFrom(btn);
		}

		private void OnSwitchToggled(object? sender, EventArgs e)
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
