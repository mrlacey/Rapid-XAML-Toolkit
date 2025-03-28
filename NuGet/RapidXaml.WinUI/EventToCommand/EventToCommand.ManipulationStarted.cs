//<auto-generated />
using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.Xaml.Interactivity;
using Microsoft.Xaml.Interactions.Core;

namespace RapidXaml;

public partial class EventToCommand : DependencyObject
{
    public static ICommand GetManipulationStarted(DependencyObject obj)
        => (ICommand)obj.GetValue(ManipulationStartedProperty);

    public static void SetManipulationStarted(DependencyObject obj, ICommand value)
        => obj.SetValue(ManipulationStartedProperty, value);

    public static readonly DependencyProperty ManipulationStartedProperty =
        DependencyProperty.RegisterAttached("ManipulationStarted", typeof(ICommand), typeof(EventToCommand), new PropertyMetadata(null, OnManipulationStartedChanged));

    private static void OnManipulationStartedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UIElement uie)
        {
            // Clear any existing behavior(s)
            Interaction.SetBehaviors(uie, null);

            // Add the new one if there is one
            if (e.NewValue is ICommand newCmd)
            {
                var etb = new EventTriggerBehavior { EventName = "ManipulationStarted" };
                etb.Actions.Add(new InvokeCommandAction { Command = newCmd });

                Interaction.SetBehaviors(uie, new BehaviorCollection { etb });
            }
        }
    }
}
