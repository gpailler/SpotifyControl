using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CG.SpotifyControl.Interfaces;
using Hardcodet.Wpf.TaskbarNotification;
using CG.Common.Wpf;
using CG.SpotifyControl.Controller;

namespace CG.SpotifyControl
{
	/// <summary>
	/// Interaction logic for FancyBalloon.xaml
	/// </summary>
	public partial class NotifyBalloon : UserControl
	{
		public const int DISPLAY_DELAY = 5000;
		private bool _isClosing = false;

		public static readonly TypeSafeDependencyProperty<NotifyBalloon, ITrackInfos> TrackInfosProperty = TypeSafeDependencyProperty<NotifyBalloon, ITrackInfos>.Register(o => o.TrackInfos);
		
		public ITrackInfos TrackInfos
		{
			get { return TrackInfosProperty.GetValue(this); }
			set { TrackInfosProperty.SetValue(this, value); }
		}
		
		public NotifyBalloon()
		{
			InitializeComponent();
			this.DataContext = this;
			TaskbarIcon.AddBalloonClosingHandler(this, OnBalloonClosing);
		}


		/// <summary>
		/// By subscribing to the <see cref="TaskbarIcon.BalloonClosingEvent"/>
		/// and setting the "Handled" property to true, we suppress the popup
		/// from being closed in order to display the fade-out animation.
		/// </summary>
		private void OnBalloonClosing(object sender, RoutedEventArgs e)
		{
			_isClosing = true;
			e.Handled = true;
		}


		/// <summary>
		/// Resolves the <see cref="TaskbarIcon"/> that displayed
		/// the balloon and requests a close action.
		/// </summary>
		private void imgClose_MouseDown(object sender, MouseButtonEventArgs e)
		{
			//the tray icon assigned this attached property to simplify access
			TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
			taskbarIcon.CloseBalloon();
		}

		/// <summary>
		/// If the user hovers over the balloon, we don't close it.
		/// </summary>
		private void grid_MouseEnter(object sender, MouseEventArgs e)
		{
			//if we're already running the fade-out animation, do not interrupt anymore
			if (_isClosing) return;

			//the tray icon assigned this attached property to simplify access
			TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
			taskbarIcon.ResetBalloonCloseTimer();
		}

		/// <summary>
		/// If the user leaves the balloon, we reset the timer
		/// </summary>
		private void grid_MouseLeave(object sender, MouseEventArgs e)
		{
			//the tray icon assigned this attached property to simplify access
			TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
			taskbarIcon.ChangeBalloonCloseTimer(DISPLAY_DELAY);
		}


		/// <summary>
		/// Closes the popup once the fade-out animation completed.
		/// The animation was triggered in XAML through the attached
		/// BalloonClosing event.
		/// </summary>
		private void OnFadeOutCompleted(object sender, EventArgs e)
		{
			Popup pp = (Popup)Parent;
			pp.IsOpen = false;
		}
	}
}
