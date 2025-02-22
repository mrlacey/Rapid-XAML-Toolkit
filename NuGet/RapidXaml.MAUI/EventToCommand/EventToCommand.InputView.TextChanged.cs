using System.Windows.Input;

namespace RapidXaml;

public partial class EventToCommand : BindableObject
{
	public static ICommand GetTextChanged(BindableObject obj)
		=> (ICommand)obj.GetValue(TextChangedProperty);

	public static void SetTextChanged(BindableObject obj, ICommand value)
		=> obj.SetValue(TextChangedProperty, value);

	public static readonly BindableProperty TextChangedProperty =
		BindableProperty.CreateAttached("TextChanged", typeof(ICommand), typeof(EventToCommand), defaultValue: null, propertyChanged: OnTextChangedChanged);

	private static void OnTextChangedChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is InputView iv)
		{
			if (oldValue is ICommand oldCmd)
			{
				for (int i = iv.Behaviors.Count; i >= 0; i--)
				{
					if (iv.Behaviors[i] is EventToCommandTextChangedBehavior oldBehavior && oldBehavior.CommandToInvoke == oldCmd)
					{
						iv.Behaviors.Remove(oldBehavior);
						break;
					}
				}
			}

			if (newValue is ICommand newCmd)
			{
				iv.Behaviors.Add(new EventToCommandTextChangedBehavior { CommandToInvoke = newCmd });
			}
		}
	}

	internal partial class EventToCommandTextChangedBehavior : Behavior<InputView>
	{
		public ICommand? CommandToInvoke { get; set; }

		protected override void OnAttachedTo(InputView iv)
		{
			iv.TextChanged += OnTextChanged;
			base.OnAttachedTo(iv);
		}

		protected override void OnDetachingFrom(InputView iv)
		{
			iv.TextChanged -= OnTextChanged;
			base.OnDetachingFrom(iv);
		}

		private void OnTextChanged(object? sender, EventArgs e)
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
