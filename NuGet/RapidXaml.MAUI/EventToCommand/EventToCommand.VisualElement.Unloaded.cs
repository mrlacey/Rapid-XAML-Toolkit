using System.Windows.Input;
using static RapidXaml.EventToCommand;

namespace RapidXaml;

public partial class EventToCommand : BindableObject
{
	public static ICommand GetUnloaded(BindableObject obj)
		=> (ICommand)obj.GetValue(UnloadedProperty);

	public static void SetUnloaded(BindableObject obj, ICommand value)
		=> obj.SetValue(UnloadedProperty, value);

	public static readonly BindableProperty UnloadedProperty =
		BindableProperty.CreateAttached("Unloaded", typeof(ICommand), typeof(EventToCommand), defaultValue: null, propertyChanged: OnUnloadedChanged);

	private static void OnUnloadedChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is VisualElement ve)
		{
			if (oldValue is ICommand oldCmd)
			{
				for (int i = ve.Behaviors.Count; i >= 0; i--)
				{
					if (ve.Behaviors[i] is EventToCommandUnloadedBehavior oldBehavior && oldBehavior.CommandToInvoke == oldCmd)
					{
						ve.Behaviors.Remove(oldBehavior);
						break;
					}
				}
			}

			if (newValue is ICommand newCmd)
			{
				ve.Behaviors.Add(new EventToCommandUnloadedBehavior { CommandToInvoke = newCmd });
			}
		}
	}

	internal partial class EventToCommandUnloadedBehavior : Behavior<VisualElement>
	{
		public ICommand? CommandToInvoke { get; set; }

		protected override void OnAttachedTo(VisualElement vizElement)
		{
			vizElement.Unloaded += OnVisualElementUnloaded;
			base.OnAttachedTo(vizElement);
		}

		protected override void OnDetachingFrom(VisualElement vizElement)
		{
			vizElement.Unloaded -= OnVisualElementUnloaded;
			base.OnDetachingFrom(vizElement);
		}

		private void OnVisualElementUnloaded(object? sender, EventArgs e)
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
