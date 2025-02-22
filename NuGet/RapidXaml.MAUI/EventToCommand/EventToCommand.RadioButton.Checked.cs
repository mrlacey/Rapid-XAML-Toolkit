using System.Windows.Input;

namespace RapidXaml;

public partial class EventToCommand : BindableObject
{
	internal partial class EventToCommandRadioButtonCheckedBehavior : Behavior<RadioButton>
	{
		public ICommand? CommandToInvoke { get; set; }

		protected override void OnAttachedTo(RadioButton rb)
		{
			rb.CheckedChanged += OnRadioButtonChecked;
			base.OnAttachedTo(rb);
		}

		protected override void OnDetachingFrom(RadioButton rb)
		{
			rb.CheckedChanged -= OnRadioButtonChecked;
			base.OnDetachingFrom(rb);
		}

		private void OnRadioButtonChecked(object? sender, EventArgs e)
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
