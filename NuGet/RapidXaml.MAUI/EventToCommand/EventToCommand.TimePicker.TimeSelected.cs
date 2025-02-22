using System.Windows.Input;

namespace RapidXaml;

public partial class EventToCommand : BindableObject
{
	public static ICommand GetTimeSelected(BindableObject obj)
		=> (ICommand)obj.GetValue(TimeSelectedProperty);

	public static void SetTimeSelected(BindableObject obj, ICommand value)
		=> obj.SetValue(TimeSelectedProperty, value);

	public static readonly BindableProperty TimeSelectedProperty =
		BindableProperty.CreateAttached("TimeSelected", typeof(ICommand), typeof(EventToCommand), defaultValue: null, propertyChanged: OnTimeSelectedChanged);

	private static void OnTimeSelectedChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is TimePicker tp)
		{
			if (oldValue is ICommand oldCmd)
			{
				for (int i = tp.Behaviors.Count; i >= 0; i--)
				{
					if (tp.Behaviors[i] is EventToCommandTimeSelectedBehavior oldBehavior && oldBehavior.CommandToInvoke == oldCmd)
					{
						tp.Behaviors.Remove(oldBehavior);
						break;
					}
				}
			}

			if (newValue is ICommand newCmd)
			{
				tp.Behaviors.Add(new EventToCommandTimeSelectedBehavior { CommandToInvoke = newCmd });
			}
		}
	}

	internal partial class EventToCommandTimeSelectedBehavior : Behavior<TimePicker>
	{
		public ICommand? CommandToInvoke { get; set; }

		protected override void OnAttachedTo(TimePicker tp)
		{
			tp.TimeSelected += OnTimePickerTimeSelected;
			base.OnAttachedTo(tp);
		}

		protected override void OnDetachingFrom(TimePicker tp)
		{
			tp.TimeSelected -= OnTimePickerTimeSelected;
			base.OnDetachingFrom(tp);
		}

		private void OnTimePickerTimeSelected(object? sender, EventArgs e)
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
