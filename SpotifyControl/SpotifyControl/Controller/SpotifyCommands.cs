using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace CG.SpotifyControl.Controller
{
	public class SpotifyCommands : ICommand
	{
		private readonly ISpotifyActions _actions;

		public SpotifyCommands(ISpotifyActions actions)
		{
			_actions = actions;
		}

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		public bool CanExecute(object parameter)
		{
			return Enum.IsDefined(typeof(CommandType), parameter);
		}

		public void Execute(object parameter)
		{
			switch ((CommandType)parameter)
			{
				case CommandType.Previous:
					_actions.PlayPrev();
					break;

				case CommandType.Next:
					_actions.PlayNext();
					break;
			}
		}
	}

	public enum CommandType
	{
		Previous,
		Next
	}
}
