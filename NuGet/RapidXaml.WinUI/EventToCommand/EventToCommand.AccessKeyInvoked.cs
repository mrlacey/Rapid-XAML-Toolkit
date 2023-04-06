//<auto-generated />
using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.Xaml.Interactivity;
using Microsoft.Xaml.Interactions.Core;

namespace RapidXaml.WinUI;

public partial class EventToCommand : DependencyObject
{
    public static ICommand GetAccessKeyInvoked(DependencyObject obj)
        => (ICommand)obj.GetValue(AccessKeyInvokedProperty);

    public static void SetAccessKeyInvoked(DependencyObject obj, ICommand value)
        => obj.SetValue(AccessKeyInvokedProperty, value);

    public static readonly DependencyProperty AccessKeyInvokedProperty =
        DependencyProperty.RegisterAttached("AccessKeyInvoked", typeof(ICommand), typeof(EventToCommand), new PropertyMetadata(null, OnAccessKeyInvokedChanged));

    private static void OnAccessKeyInvokedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UIElement uie)
        {
            // Clear any existing behavior(s)
            Interaction.SetBehaviors(uie, null);

            // Add the new one if there is one
            if (e.NewValue is ICommand newCmd)
            {
                var etb = new EventTriggerBehavior { EventName = "AccessKeyInvoked" };
                etb.Actions.Add(new InvokeCommandAction { Command = newCmd });

                Interaction.SetBehaviors(uie, new BehaviorCollection { etb });
            }
        }
    }
}
