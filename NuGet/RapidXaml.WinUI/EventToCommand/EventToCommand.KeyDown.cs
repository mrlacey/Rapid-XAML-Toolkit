//<auto-generated />
using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.Xaml.Interactivity;
using Microsoft.Xaml.Interactions.Core;

namespace RapidXaml;

public partial class EventToCommand : DependencyObject
{
    public static ICommand GetKeyDown(DependencyObject obj)
        => (ICommand)obj.GetValue(KeyDownProperty);

    public static void SetKeyDown(DependencyObject obj, ICommand value)
        => obj.SetValue(KeyDownProperty, value);

    public static readonly DependencyProperty KeyDownProperty =
        DependencyProperty.RegisterAttached("KeyDown", typeof(ICommand), typeof(EventToCommand), new PropertyMetadata(null, OnKeyDownChanged));

    private static void OnKeyDownChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UIElement uie)
        {
            // Clear any existing behavior(s)
            Interaction.SetBehaviors(uie, null);

            // Add the new one if there is one
            if (e.NewValue is ICommand newCmd)
            {
                var etb = new EventTriggerBehavior { EventName = "KeyDown" };
                etb.Actions.Add(new InvokeCommandAction { Command = newCmd });

                Interaction.SetBehaviors(uie, new BehaviorCollection { etb });
            }
        }
    }
}
