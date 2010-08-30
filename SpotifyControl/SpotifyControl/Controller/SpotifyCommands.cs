using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using CG.SpotifyControl.Interfaces;

namespace CG.SpotifyControl.Controller
{
	public class SpotifyCommands : ICommand
	{
		private readonly ISpotifyActions _spotifyActions;

		public SpotifyCommands(ISpotifyActions spotifyActions)
		{
			_spotifyActions = spotifyActions;
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
				case CommandType.PreviousTrack:
					_spotifyActions.PlayPrev();
					break;

				case CommandType.NextTrack:
					_spotifyActions.PlayNext();
					break;
			}
		}
	}

	public enum CommandType
	{
		PreviousTrack,
		NextTrack
	}
}
