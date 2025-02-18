using System.Windows.Input;

namespace RapidXaml;

public partial class EventToCommand : BindableObject
{
	public static ICommand GetLoaded(BindableObject obj)
		=> (ICommand)obj.GetValue(LoadedProperty);

	public static void SetLoaded(BindableObject obj, ICommand value)
		=> obj.SetValue(LoadedProperty, value);

	public static readonly BindableProperty LoadedProperty =
		BindableProperty.CreateAttached("Loaded", typeof(ICommand), typeof(EventToCommand), defaultValue: null, propertyChanged: OnLoadedChanged);

	private static void OnLoadedChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is VisualElement ve)
		{
			if (oldValue is ICommand oldCmd)
			{
				for (int i = ve.Behaviors.Count; i >= 0; i--)
				{
					if (ve.Behaviors[i] is EventToCommandLoadedBehavior oldBehavior && oldBehavior.CommandToInvoke == oldCmd)
					{
						ve.Behaviors.Remove(oldBehavior);
						break;
					}
				}
			}

			if (newValue is ICommand newCmd)
			{
				ve.Behaviors.Add(new EventToCommandLoadedBehavior { CommandToInvoke = newCmd });
			}
		}
	}

	internal partial class EventToCommandLoadedBehavior : Behavior<VisualElement>
	{
		public ICommand? CommandToInvoke { get; set; }

		protected override void OnAttachedTo(VisualElement vizElement)
		{
			vizElement.Loaded += OnVisualElementLoaded;
			base.OnAttachedTo(vizElement);
		}

		protected override void OnDetachingFrom(VisualElement vizElement)
		{
			vizElement.Loaded -= OnVisualElementLoaded;
			base.OnDetachingFrom(vizElement);
		}

		private void OnVisualElementLoaded(object? sender, EventArgs e)
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
