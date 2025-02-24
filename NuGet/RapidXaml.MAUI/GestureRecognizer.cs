using System.Windows.Input;

namespace RapidXaml;

public partial class GestureRecognizer : BindableObject
{
	/// <summary>
	/// Command to execute when the view is tapped a single time.
	/// </summary>
	public static readonly BindableProperty SingleTapProperty =
		BindableProperty.CreateAttached("SingleTap", typeof(ICommand), typeof(GestureRecognizer), defaultValue: null, propertyChanged: OnSingleTapChanged);

	private static void OnSingleTapChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is View view)
		{
			if (oldValue is ICommand oldCmd)
			{
				for (int i = view.GestureRecognizers.Count; i >= 0; i--)
				{
					if (view.GestureRecognizers[i] is TapGestureRecognizer oldTgr && oldTgr.Command == oldCmd)
					{
						view.GestureRecognizers.Remove(oldTgr);
						break;
					}
				}
			}

			if (newValue is ICommand newCmd)
			{
				view.GestureRecognizers.Add(new TapGestureRecognizer { Command = newCmd });
			}
		}
	}

	public static ICommand GetSingleTap(BindableObject view)
		=> (ICommand)view.GetValue(SingleTapProperty);

	public static void SetSingleTap(BindableObject view, ICommand value)
		=> view.SetValue(SingleTapProperty, value);

	/// <summary>
	/// Command to execute when the view is tapped a single time.
	/// </summary>
	public static readonly BindableProperty TapProperty =
		BindableProperty.CreateAttached("Tap", typeof(ICommand), typeof(GestureRecognizer), defaultValue: null, propertyChanged: OnSingleTapChanged);

	public static ICommand GetTap(BindableObject view)
		=> (ICommand)view.GetValue(TapProperty);

	public static void SetTap(BindableObject view, ICommand value)
		=> view.SetValue(TapProperty, value);

	/// <summary>
	/// Command to execute when the view is tapped twice in quick succession.
	/// </summary>
	public static readonly BindableProperty DoubleTapProperty =
		BindableProperty.CreateAttached("DoubleTap", typeof(ICommand), typeof(GestureRecognizer), defaultValue: null, propertyChanged: OnDoubleTapChanged);

	private static void OnDoubleTapChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is View view)
		{
			if (oldValue is ICommand oldCmd)
			{
				for (int i = view.GestureRecognizers.Count; i >= 0; i--)
				{
					if (view.GestureRecognizers[i] is TapGestureRecognizer oldTgr && oldTgr.Command == oldCmd)
					{
						view.GestureRecognizers.Remove(oldTgr);
						break;
					}
				}
			}

			if (newValue is ICommand newCmd)
			{
				view.GestureRecognizers.Add(new TapGestureRecognizer { Command = newCmd, NumberOfTapsRequired = 2 });
			}
		}
	}

	public static ICommand GetDoubleTap(BindableObject view)
		=> (ICommand)view.GetValue(DoubleTapProperty);

	public static void SetDoubleTap(BindableObject view, ICommand value)
		=> view.SetValue(DoubleTapProperty, value);

	/// <summary>
	/// Command to execute when the view is tapped with the 'secondary' button. This is usually the right mouse button.
	/// </summary>
	public static readonly BindableProperty SecondaryTapProperty =
	 BindableProperty.CreateAttached("SecondaryTap", typeof(ICommand), typeof(GestureRecognizer), defaultValue: null, propertyChanged: OnSecondaryTapChanged);

	private static void OnSecondaryTapChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is View view)
		{
			if (oldValue is ICommand oldCmd)
			{
				for (int i = view.GestureRecognizers.Count; i >= 0; i--)
				{
					if (view.GestureRecognizers[i] is TapGestureRecognizer oldTgr && oldTgr.Command == oldCmd)
					{
						view.GestureRecognizers.Remove(oldTgr);
						break;
					}
				}
			}

			if (newValue is ICommand newCmd)
			{
				view.GestureRecognizers.Add(new TapGestureRecognizer { Command = newCmd, Buttons = ButtonsMask.Secondary });
			}
		}
	}

	public static ICommand GetSecondaryTap(BindableObject view)
		=> (ICommand)view.GetValue(SecondaryTapProperty);

	public static void SetSecondaryTap(BindableObject view, ICommand value)
		=> view.SetValue(SecondaryTapProperty, value);


	/// <summary>
	/// Command to execute when the view is panned. The command will receive a <see cref="PanUpdatedEventArgs"/> as a parameter.
	/// </summary>
	public static readonly BindableProperty PanProperty =
	 BindableProperty.CreateAttached("Pan", typeof(Command<PanUpdatedEventArgs>), typeof(GestureRecognizer), defaultValue: null, propertyChanged: OnPanChanged);

	private static void OnPanChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is View view)
		{
			if (oldValue is ICommand oldCmd)
			{
				for (int i = view.GestureRecognizers.Count; i >= 0; i--)
				{
					if (view.GestureRecognizers[i] is PanGestureRecognizer oldPgr)
					{
						view.GestureRecognizers.Remove(oldPgr);
						break;
					}
				}
			}

			if (newValue is ICommand newCmd)
			{
				var pgr = new PanGestureRecognizer();

				pgr.PanUpdated += (_, e) => { newCmd.Execute(e); };

				view.GestureRecognizers.Add(pgr);
			}
		}
	}

	public static ICommand GetPan(BindableObject view)
		=> (ICommand)view.GetValue(PanProperty);

	public static void SetPan(BindableObject view, ICommand value)
		=> view.SetValue(PanProperty, value);

	/// <summary>
	/// Command to execute when the view has finished being panned. The command will receive a <see cref="PanUpdatedEventArgs"/> as a parameter.
	/// </summary>
	public static readonly BindableProperty PanCompletedProperty =
	 BindableProperty.CreateAttached("PanCompleted", typeof(Command<PanUpdatedEventArgs>), typeof(GestureRecognizer), defaultValue: null, propertyChanged: OnPanCompletedChanged);

	private static void OnPanCompletedChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is View view)
		{
			if (oldValue is ICommand oldCmd)
			{
				for (int i = view.GestureRecognizers.Count; i >= 0; i--)
				{
					if (view.GestureRecognizers[i] is PanGestureRecognizer oldPgr)
					{
						view.GestureRecognizers.Remove(oldPgr);
						break;
					}
				}
			}

			if (newValue is ICommand newCmd)
			{
				var pgr = new PanGestureRecognizer();

				pgr.PanUpdated += (_, e) => { if (e.StatusType == GestureStatus.Completed) { newCmd.Execute(e); } };

				view.GestureRecognizers.Add(pgr);
			}
		}
	}

	public static ICommand GetPanCompleted(BindableObject view)
		=> (ICommand)view.GetValue(PanCompletedProperty);

	public static void SetPanCompleted(BindableObject view, ICommand value)
		=> view.SetValue(PanCompletedProperty, value);


	/// <summary>
	/// Command to execute when the view has been swiped.  The command will receive a <see cref="SwipeDirection"/> as a parameter.
	/// </summary>
	public static readonly BindableProperty SwipeProperty =
	 BindableProperty.CreateAttached("Swipe", typeof(Command<SwipeDirection>), typeof(GestureRecognizer), defaultValue: null, propertyChanged: OnSwipeChanged);

	private static void OnSwipeChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is View view)
		{
			if (oldValue is Command<SwipeDirection> oldCmd)
			{
				for (int i = view.GestureRecognizers.Count; i >= 0; i--)
				{
					if (view.GestureRecognizers[i] is SwipeGestureRecognizer oldSgr)
					{
						view.GestureRecognizers.Remove(oldSgr);
						break;
					}
				}
			}

			if (newValue is Command<SwipeDirection> newCmd)
			{
				var sgr = new SwipeGestureRecognizer() { Direction = SwipeDirection.Right | SwipeDirection.Left | SwipeDirection.Up | SwipeDirection.Down };

				sgr.Swiped += (_, e) => { newCmd.Execute(e.Direction); };

				view.GestureRecognizers.Add(sgr);
			}
		}
	}

	public static Command<SwipeDirection> GetSwipe(BindableObject view)
		=> (Command<SwipeDirection>)view.GetValue(SwipeProperty);

	public static void SetSwipe(BindableObject view, Command<SwipeDirection> value)
		=> view.SetValue(SwipeProperty, value);


	/// <summary>
	/// Command to execute when the view has been swiped to the Left.
	/// </summary>
	public static readonly BindableProperty SwipeLeftProperty =
	 BindableProperty.CreateAttached("SwipeLeft", typeof(ICommand), typeof(GestureRecognizer), defaultValue: null, propertyChanged: OnSwipeLeftChanged);

	private static void OnSwipeLeftChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is View view)
		{
			if (oldValue is ICommand oldCmd)
			{
				for (int i = view.GestureRecognizers.Count; i >= 0; i--)
				{
					if (view.GestureRecognizers[i] is SwipeGestureRecognizer oldSgr)
					{
						view.GestureRecognizers.Remove(oldSgr);
						break;
					}
				}
			}

			if (newValue is ICommand newCmd)
			{
				var sgr = new SwipeGestureRecognizer() { Direction = SwipeDirection.Left };

				sgr.Swiped += (_, e) => { if (e.Direction == SwipeDirection.Left) { newCmd.Execute(e); } };

				view.GestureRecognizers.Add(sgr);
			}
		}
	}

	public static ICommand GetSwipeLeft(BindableObject view)
		=> (ICommand)view.GetValue(SwipeLeftProperty);

	public static void SetSwipeLeft(BindableObject view, ICommand value)
		=> view.SetValue(SwipeLeftProperty, value);


	/// <summary>
	/// Command to execute when the view has been swiped to the Right.
	/// </summary>
	public static readonly BindableProperty SwipeRightProperty =
	 BindableProperty.CreateAttached("SwipeRight", typeof(ICommand), typeof(GestureRecognizer), defaultValue: null, propertyChanged: OnSwipeRightChanged);

	private static void OnSwipeRightChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is View view)
		{
			if (oldValue is ICommand oldCmd)
			{
				for (int i = view.GestureRecognizers.Count; i >= 0; i--)
				{
					if (view.GestureRecognizers[i] is SwipeGestureRecognizer oldSgr)
					{
						view.GestureRecognizers.Remove(oldSgr);
						break;
					}
				}
			}

			if (newValue is ICommand newCmd)
			{
				var sgr = new SwipeGestureRecognizer() { Direction = SwipeDirection.Right };

				sgr.Swiped += (_, e) => { if (e.Direction == SwipeDirection.Right) { newCmd.Execute(e); } };

				view.GestureRecognizers.Add(sgr);
			}
		}
	}

	public static ICommand GetSwipeRight(BindableObject view)
		=> (ICommand)view.GetValue(SwipeRightProperty);

	public static void SetSwipeRight(BindableObject view, ICommand value)
		=> view.SetValue(SwipeRightProperty, value);


	/// <summary>
	/// Command to execute when the view has been swiped Up.
	/// </summary>
	public static readonly BindableProperty SwipeUpProperty =
	 BindableProperty.CreateAttached("SwipeUp", typeof(ICommand), typeof(GestureRecognizer), defaultValue: null, propertyChanged: OnSwipeUpChanged);

	private static void OnSwipeUpChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is View view)
		{
			if (oldValue is ICommand oldCmd)
			{
				for (int i = view.GestureRecognizers.Count; i >= 0; i--)
				{
					if (view.GestureRecognizers[i] is SwipeGestureRecognizer oldSgr)
					{
						view.GestureRecognizers.Remove(oldSgr);
						break;
					}
				}
			}

			if (newValue is ICommand newCmd)
			{
				var sgr = new SwipeGestureRecognizer() { Direction = SwipeDirection.Up };

				sgr.Swiped += (_, e) => { if (e.Direction == SwipeDirection.Up) { newCmd.Execute(e); } };

				view.GestureRecognizers.Add(sgr);
			}
		}
	}

	public static ICommand GetSwipeUp(BindableObject view)
		=> (ICommand)view.GetValue(SwipeUpProperty);

	public static void SetSwipeUp(BindableObject view, ICommand value)
		=> view.SetValue(SwipeUpProperty, value);


	/// <summary>
	/// Command to execute when the view has been swiped Down.
	/// </summary>
	public static readonly BindableProperty SwipeDownProperty =
	 BindableProperty.CreateAttached("SwipeDown", typeof(ICommand), typeof(GestureRecognizer), defaultValue: null, propertyChanged: OnSwipeDownChanged);

	private static void OnSwipeDownChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is View view)
		{
			if (oldValue is ICommand oldCmd)
			{
				for (int i = view.GestureRecognizers.Count; i >= 0; i--)
				{
					if (view.GestureRecognizers[i] is SwipeGestureRecognizer oldSgr)
					{
						view.GestureRecognizers.Remove(oldSgr);
						break;
					}
				}
			}

			if (newValue is ICommand newCmd)
			{
				var sgr = new SwipeGestureRecognizer() { Direction = SwipeDirection.Down };

				sgr.Swiped += (_, e) => { if (e.Direction == SwipeDirection.Down) { newCmd.Execute(e); } };

				view.GestureRecognizers.Add(sgr);
			}
		}
	}

	public static ICommand GetSwipeDown(BindableObject view)
		=> (ICommand)view.GetValue(SwipeDownProperty);

	public static void SetSwipeDown(BindableObject view, ICommand value)
		=> view.SetValue(SwipeDownProperty, value);

}

public partial class Gesture : GestureRecognizer
{
	// Inherited version with a simpler name
}
