using System.Windows.Input;

namespace RapidXaml;

public partial class EventToCommand : BindableObject
{
	internal partial class EventToCommandStepperValueChangedBehavior : Behavior<Stepper>
	{
		public ICommand? CommandToInvoke { get; set; }

		protected override void OnAttachedTo(Stepper stpr)
		{
			stpr.ValueChanged += OnSliderValueChanged;
			base.OnAttachedTo(stpr);
		}

		protected override void OnDetachingFrom(Stepper stpr)
		{
			stpr.ValueChanged -= OnSliderValueChanged;
			base.OnDetachingFrom(stpr);
		}

		private void OnSliderValueChanged(object? sender, EventArgs e)
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
