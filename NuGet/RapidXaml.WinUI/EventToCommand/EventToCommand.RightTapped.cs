//<auto-generated />
using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.Xaml.Interactivity;
using Microsoft.Xaml.Interactions.Core;

namespace RapidXaml;

public partial class EventToCommand : DependencyObject
{
    public static ICommand GetRightTapped(DependencyObject obj)
        => (ICommand)obj.GetValue(RightTappedProperty);

    public static void SetRightTapped(DependencyObject obj, ICommand value)
        => obj.SetValue(RightTappedProperty, value);

    public static readonly DependencyProperty RightTappedProperty =
        DependencyProperty.RegisterAttached("RightTapped", typeof(ICommand), typeof(EventToCommand), new PropertyMetadata(null, OnRightTappedChanged));

    private static void OnRightTappedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UIElement uie)
        {
            // Clear any existing behavior(s)
            Interaction.SetBehaviors(uie, null);

            // Add the new one if there is one
            if (e.NewValue is ICommand newCmd)
            {
                var etb = new EventTriggerBehavior { EventName = "RightTapped" };
                etb.Actions.Add(new InvokeCommandAction { Command = newCmd });

                Interaction.SetBehaviors(uie, new BehaviorCollection { etb });
            }
        }
    }
}
