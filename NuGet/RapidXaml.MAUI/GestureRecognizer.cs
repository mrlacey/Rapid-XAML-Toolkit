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
}

public partial class Gesture : GestureRecognizer
{
	// Inherited version with a simpler name
}
