﻿namespace MoonWorks.Input
{
	public struct ButtonState
	{
		private ButtonStatus ButtonStatus { get; set; }

		public bool IsPressed => ButtonStatus == ButtonStatus.Pressed;
		public bool IsHeld => ButtonStatus == ButtonStatus.Held;
		public bool IsDown => ButtonStatus == ButtonStatus.Pressed || ButtonStatus == ButtonStatus.Held;
		public bool IsReleased => ButtonStatus == ButtonStatus.Released;

		public ButtonState(ButtonStatus buttonStatus)
		{
			ButtonStatus = buttonStatus;
		}

		internal ButtonState Update(bool isPressed)
		{
			if (isPressed)
			{
				if (ButtonStatus == ButtonStatus.Pressed)
				{
					return new ButtonState(ButtonStatus.Held);
				}
				else if (ButtonStatus == ButtonStatus.Released)
				{
					return new ButtonState(ButtonStatus.Pressed);
				}
				else if (ButtonStatus == ButtonStatus.Held)
				{
					return new ButtonState(ButtonStatus.Held);
				}
			}

			return new ButtonState(ButtonStatus.Released);
		}
	}
}
