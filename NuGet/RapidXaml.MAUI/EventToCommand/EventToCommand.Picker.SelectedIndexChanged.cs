using System.Windows.Input;

namespace RapidXaml;

public partial class EventToCommand : BindableObject
{
	public static ICommand GetSelectedIndexChanged(BindableObject obj)
		=> (ICommand)obj.GetValue(SelectedIndexChangedProperty);

	public static void SetSelectedIndexChanged(BindableObject obj, ICommand value)
		=> obj.SetValue(SelectedIndexChangedProperty, value);

	public static readonly BindableProperty SelectedIndexChangedProperty =
		BindableProperty.CreateAttached("SelectedIndexChanged", typeof(ICommand), typeof(EventToCommand), defaultValue: null, propertyChanged: OnSelectedIndexChangedChanged);

	private static void OnSelectedIndexChangedChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is Picker pckr)
		{
			if (oldValue is ICommand oldCmd)
			{
				for (int i = pckr.Behaviors.Count; i >= 0; i--)
				{
					if (pckr.Behaviors[i] is EventToCommandSelectedIndexChangedBehavior oldBehavior && oldBehavior.CommandToInvoke == oldCmd)
					{
						pckr.Behaviors.Remove(oldBehavior);
						break;
					}
				}
			}

			if (newValue is ICommand newCmd)
			{
				pckr.Behaviors.Add(new EventToCommandSelectedIndexChangedBehavior { CommandToInvoke = newCmd });
			}
		}
	}

	internal partial class EventToCommandSelectedIndexChangedBehavior : Behavior<Picker>
	{
		// TODO: Does this need to be passed the index?
		public ICommand? CommandToInvoke { get; set; }

		protected override void OnAttachedTo(Picker pckr)
		{
			pckr.SelectedIndexChanged += OnPickerSelectedIndexChanged;
			base.OnAttachedTo(pckr);
		}

		protected override void OnDetachingFrom(Picker pckr)
		{
			pckr.SelectedIndexChanged -= OnPickerSelectedIndexChanged;
			base.OnDetachingFrom(pckr);
		}

		private void OnPickerSelectedIndexChanged(object? sender, EventArgs e)
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
