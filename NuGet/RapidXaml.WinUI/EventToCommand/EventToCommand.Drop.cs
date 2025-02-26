//<auto-generated />
using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.Xaml.Interactivity;
using Microsoft.Xaml.Interactions.Core;

namespace RapidXaml;

public partial class EventToCommand : DependencyObject
{
    public static ICommand GetDrop(DependencyObject obj)
        => (ICommand)obj.GetValue(DropProperty);

    public static void SetDrop(DependencyObject obj, ICommand value)
        => obj.SetValue(DropProperty, value);

    public static readonly DependencyProperty DropProperty =
        DependencyProperty.RegisterAttached("Drop", typeof(ICommand), typeof(EventToCommand), new PropertyMetadata(null, OnDropChanged));

    private static void OnDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UIElement uie)
        {
            // Clear any existing behavior(s)
            Interaction.SetBehaviors(uie, null);

            // Add the new one if there is one
            if (e.NewValue is ICommand newCmd)
            {
                var etb = new EventTriggerBehavior { EventName = "Drop" };
                etb.Actions.Add(new InvokeCommandAction { Command = newCmd });

                Interaction.SetBehaviors(uie, new BehaviorCollection { etb });
            }
        }
    }
}
