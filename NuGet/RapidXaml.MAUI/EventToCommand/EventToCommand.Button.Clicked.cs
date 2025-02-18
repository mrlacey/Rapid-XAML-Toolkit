using System.Windows.Input;

namespace RapidXaml;

public partial class EventToCommand : BindableObject
{
	public static ICommand GetClicked(BindableObject obj)
		=> (ICommand)obj.GetValue(ClickedProperty);

	public static void SetClicked(BindableObject obj, ICommand value)
		=> obj.SetValue(ClickedProperty, value);

	public static readonly BindableProperty ClickedProperty =
		BindableProperty.CreateAttached("Clicked", typeof(ICommand), typeof(EventToCommand), defaultValue: null, propertyChanged: OnClickedChanged);

	private static void OnClickedChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is Button btn)
		{
			if (oldValue is ICommand oldCmd)
			{
				for (int i = btn.Behaviors.Count; i >= 0; i--)
				{
					if (btn.Behaviors[i] is EventToCommandClickBehavior oldBehavior && oldBehavior.CommandToInvoke == oldCmd)
					{
						btn.Behaviors.Remove(oldBehavior);
						break;
					}
				}
			}

			if (newValue is ICommand newCmd)
			{
				btn.Behaviors.Add(new EventToCommandClickBehavior { CommandToInvoke = newCmd });
			}
		}
	}

	internal partial class EventToCommandClickBehavior : Behavior<Button>
	{
		public ICommand? CommandToInvoke { get; set; }

		protected override void OnAttachedTo(Button btn)
		{
			btn.Clicked += OnButtonClicked;
			base.OnAttachedTo(btn);
		}

		protected override void OnDetachingFrom(Button btn)
		{
			btn.Clicked -= OnButtonClicked;
			base.OnDetachingFrom(btn);
		}

		private void OnButtonClicked(object? sender, EventArgs e)
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
