using System.Windows.Input;

namespace RapidXaml;

public partial class EventToCommand : BindableObject
{
	public static ICommand GetValueChanged(BindableObject obj)
		=> (ICommand)obj.GetValue(ValueChangedProperty);

	public static void SetValueChanged(BindableObject obj, ICommand value)
		=> obj.SetValue(ValueChangedProperty, value);

	public static readonly BindableProperty ValueChangedProperty =
		BindableProperty.CreateAttached("ValueChanged", typeof(ICommand), typeof(EventToCommand), defaultValue: null, propertyChanged: OnValueChangedChanged);

	private static void OnValueChangedChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is Slider sldr)
		{
			if (oldValue is ICommand oldCmd)
			{
				for (int i = sldr.Behaviors.Count; i >= 0; i--)
				{
					if (sldr.Behaviors[i] is EventToCommandSliderValueChangedBehavior oldBehavior && oldBehavior.CommandToInvoke == oldCmd)
					{
						sldr.Behaviors.Remove(oldBehavior);
						break;
					}
				}
			}

			if (newValue is ICommand newCmd)
			{
				sldr.Behaviors.Add(new EventToCommandSliderValueChangedBehavior { CommandToInvoke = newCmd });
			}
		}
		else
		if (bindable is Stepper stpr)
		{
			if (oldValue is ICommand oldCmd)
			{
				for (int i = stpr.Behaviors.Count; i >= 0; i--)
				{
					if (stpr.Behaviors[i] is EventToCommandStepperValueChangedBehavior oldBehavior && oldBehavior.CommandToInvoke == oldCmd)
					{
						stpr.Behaviors.Remove(oldBehavior);
						break;
					}
				}
			}

			if (newValue is ICommand newCmd)
			{
				stpr.Behaviors.Add(new EventToCommandStepperValueChangedBehavior { CommandToInvoke = newCmd });
			}
		}
	}

	internal partial class EventToCommandSliderValueChangedBehavior : Behavior<Slider>
	{
		public ICommand? CommandToInvoke { get; set; }

		protected override void OnAttachedTo(Slider sldr)
		{
			sldr.ValueChanged += OnSliderValueChanged;
			base.OnAttachedTo(sldr);
		}

		protected override void OnDetachingFrom(Slider sldr)
		{
			sldr.ValueChanged -= OnSliderValueChanged;
			base.OnDetachingFrom(sldr);
		}

		private void OnSliderValueChanged(object? sender, EventArgs e)
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
