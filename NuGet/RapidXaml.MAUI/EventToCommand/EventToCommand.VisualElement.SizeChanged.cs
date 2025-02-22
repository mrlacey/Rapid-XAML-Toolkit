using System.Windows.Input;

namespace RapidXaml;

public partial class EventToCommand : BindableObject
{
	public static ICommand GetSizeChanged(BindableObject obj)
		=> (ICommand)obj.GetValue(SizeChangedProperty);

	public static void SetSizeChanged(BindableObject obj, ICommand value)
		=> obj.SetValue(SizeChangedProperty, value);

	public static readonly BindableProperty SizeChangedProperty =
		BindableProperty.CreateAttached("SizeChanged", typeof(ICommand), typeof(EventToCommand), defaultValue: null, propertyChanged: OnSizeChangedChanged);

	private static void OnSizeChangedChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is VisualElement ve)
		{
			if (oldValue is ICommand oldCmd)
			{
				for (int i = ve.Behaviors.Count; i >= 0; i--)
				{
					if (ve.Behaviors[i] is EventToCommandSizeChangedBehavior oldBehavior && oldBehavior.CommandToInvoke == oldCmd)
					{
						ve.Behaviors.Remove(oldBehavior);
						break;
					}
				}
			}

			if (newValue is ICommand newCmd)
			{
				ve.Behaviors.Add(new EventToCommandSizeChangedBehavior { CommandToInvoke = newCmd });
			}
		}
	}

	internal partial class EventToCommandSizeChangedBehavior : Behavior<VisualElement>
	{
		public ICommand? CommandToInvoke { get; set; }

		protected override void OnAttachedTo(VisualElement vizElement)
		{
			vizElement.SizeChanged += OnVisualElementSizeChanged;
			base.OnAttachedTo(vizElement);
		}

		protected override void OnDetachingFrom(VisualElement vizElement)
		{
			vizElement.SizeChanged -= OnVisualElementSizeChanged;
			base.OnDetachingFrom(vizElement);
		}

		private void OnVisualElementSizeChanged(object? sender, EventArgs e)
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
