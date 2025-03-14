﻿using System.Windows.Input;

namespace RapidXaml;

public partial class EventToCommand : BindableObject
{
	public static ICommand GetChecked(BindableObject obj)
		=> (ICommand)obj.GetValue(CheckedProperty);

	public static void SetChecked(BindableObject obj, ICommand value)
		=> obj.SetValue(CheckedProperty, value);

	public static readonly BindableProperty CheckedProperty =
		BindableProperty.CreateAttached("Checked", typeof(ICommand), typeof(EventToCommand), defaultValue: null, propertyChanged: OnCheckedChanged);

	private static void OnCheckedChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is CheckBox cb)
		{
			if (oldValue is ICommand oldCmd)
			{
				for (int i = cb.Behaviors.Count; i >= 0; i--)
				{
					if (cb.Behaviors[i] is EventToCommandCheckBoxCheckedBehavior oldBehavior && oldBehavior.CommandToInvoke == oldCmd)
					{
						cb.Behaviors.Remove(oldBehavior);
						break;
					}
				}
			}

			if (newValue is ICommand newCmd)
			{
				cb.Behaviors.Add(new EventToCommandCheckBoxCheckedBehavior { CommandToInvoke = newCmd });
			}
		}
		else if (bindable is RadioButton rb)
		{
			if (oldValue is ICommand oldCmd)
			{
				for (int i = rb.Behaviors.Count; i >= 0; i--)
				{
					if (rb.Behaviors[i] is EventToCommandRadioButtonCheckedBehavior oldBehavior && oldBehavior.CommandToInvoke == oldCmd)
					{
						rb.Behaviors.Remove(oldBehavior);
						break;
					}
				}
			}

			if (newValue is ICommand newCmd)
			{
				rb.Behaviors.Add(new EventToCommandRadioButtonCheckedBehavior { CommandToInvoke = newCmd });
			}
		}
	}

	internal partial class EventToCommandCheckBoxCheckedBehavior : Behavior<CheckBox>
	{
		public ICommand? CommandToInvoke { get; set; }

		protected override void OnAttachedTo(CheckBox cb)
		{
			cb.CheckedChanged += OnCheckBoxChecked;
			base.OnAttachedTo(cb);
		}

		protected override void OnDetachingFrom(CheckBox cb)
		{
			cb.CheckedChanged -= OnCheckBoxChecked;
			base.OnDetachingFrom(cb);
		}

		private void OnCheckBoxChecked(object? sender, EventArgs e)
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
