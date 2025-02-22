using System.Windows.Input;

namespace RapidXaml;

public partial class EventToCommand : BindableObject
{
	public static ICommand GetDateSelected(BindableObject obj)
		=> (ICommand)obj.GetValue(DateSelectedProperty);

	public static void SetDateSelected(BindableObject obj, ICommand value)
		=> obj.SetValue(DateSelectedProperty, value);

	public static readonly BindableProperty DateSelectedProperty =
		BindableProperty.CreateAttached("DateSelected", typeof(ICommand), typeof(EventToCommand), defaultValue: null, propertyChanged: OnDateSelectedChanged);

	private static void OnDateSelectedChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is DatePicker dp)
		{
			if (oldValue is ICommand oldCmd)
			{
				for (int i = dp.Behaviors.Count; i >= 0; i--)
				{
					if (dp.Behaviors[i] is EventToCommandDateSelectedBehavior oldBehavior && oldBehavior.CommandToInvoke == oldCmd)
					{
						dp.Behaviors.Remove(oldBehavior);
						break;
					}
				}
			}

			if (newValue is ICommand newCmd)
			{
				dp.Behaviors.Add(new EventToCommandDateSelectedBehavior { CommandToInvoke = newCmd });
			}
		}
	}

	internal partial class EventToCommandDateSelectedBehavior : Behavior<DatePicker>
	{
		public ICommand? CommandToInvoke { get; set; }

		protected override void OnAttachedTo(DatePicker dp)
		{
			dp.DateSelected += OnDatePickerDateSelected;
			base.OnAttachedTo(dp);
		}

		protected override void OnDetachingFrom(DatePicker dp)
		{
			dp.DateSelected -= OnDatePickerDateSelected;
			base.OnDetachingFrom(dp);
		}

		private void OnDatePickerDateSelected(object? sender, EventArgs e)
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
