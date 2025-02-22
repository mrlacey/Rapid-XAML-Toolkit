using System.Windows.Input;

namespace RapidXaml;

public partial class EventToCommand : BindableObject
{
	public static ICommand GetRefreshing(BindableObject obj)
		=> (ICommand)obj.GetValue(RefreshingProperty);

	public static void SetRefreshing(BindableObject obj, ICommand value)
		=> obj.SetValue(RefreshingProperty, value);

	public static readonly BindableProperty RefreshingProperty =
		BindableProperty.CreateAttached("Refreshing", typeof(ICommand), typeof(EventToCommand), defaultValue: null, propertyChanged: OnRefreshingChanged);

	private static void OnRefreshingChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is RefreshView rv)
		{
			if (oldValue is ICommand oldCmd)
			{
				for (int i = rv.Behaviors.Count; i >= 0; i--)
				{
					if (rv.Behaviors[i] is EventToCommandRefreshingBehavior oldBehavior && oldBehavior.CommandToInvoke == oldCmd)
					{
						rv.Behaviors.Remove(oldBehavior);
						break;
					}
				}
			}

			if (newValue is ICommand newCmd)
			{
				rv.Behaviors.Add(new EventToCommandRefreshingBehavior { CommandToInvoke = newCmd });
			}
		}
	}

	internal partial class EventToCommandRefreshingBehavior : Behavior<RefreshView>
	{
		public ICommand? CommandToInvoke { get; set; }

		protected override void OnAttachedTo(RefreshView rv)
		{
			rv.Refreshing += OnRefreshViewRefreshing;
			base.OnAttachedTo(rv);
		}

		protected override void OnDetachingFrom(RefreshView rv)
		{
			rv.Refreshing -= OnRefreshViewRefreshing;
			base.OnDetachingFrom(rv);
		}

		private void OnRefreshViewRefreshing(object? sender, EventArgs e)
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
