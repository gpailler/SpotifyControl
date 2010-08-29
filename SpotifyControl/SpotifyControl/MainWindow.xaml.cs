﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace CG.SpotifyControl
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public Controller.SpotifyController _controller;
		static MainWindow _instance;

		public MainWindow()
		{
			_instance = this;

			InitializeComponent();
			
			this.Left = Properties.Settings.Default.ControllerLeft;
			this.Top = Properties.Settings.Default.ControllerTop;

			_controller = new Controller.SpotifyController();
			_controller.SpotifyStatusChanged += new Action<Controller.SpotifyController, Controller.SpotifyController.eSpotifyStatus>(controler_SpotifyStatusChanged);

			if (_controller.IsPlaying)	
				ShowBalloon(_controller);

			this.DataContext = _controller;
		}

		public static void ShowCustomBalloon(string message, Hardcodet.Wpf.TaskbarNotification.BalloonIcon icon)
		{
			_instance.MyNotifyIcon.ShowBalloonTip((string)_instance.FindResource("ApplicationName"), message, icon);
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			this.Hide();
			e.Cancel = true;
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (e.Source == menuExit)
			{
				Properties.Settings.Default.ControllerLeft = this.Left;
				Properties.Settings.Default.ControllerTop = this.Top;
				Properties.Settings.Default.Save();

				Application.Current.Shutdown();
			}

			if (e.Source == menuSettings)
			{
				MyNotifyIcon.CloseBalloon();
				SettingsWindow frm = new SettingsWindow();
				bool playingState = _controller.IsPlaying;
				_controller.IsPlaying = false;
				frm.ShowDialog();
				_controller.IsPlaying = playingState;
			}
		}



		void controler_SpotifyStatusChanged(Controller.SpotifyController sender, Controller.SpotifyController.eSpotifyStatus obj)
		{
			System.Diagnostics.Debug.WriteLine("Status: " + obj);

			if (obj == Controller.SpotifyController.eSpotifyStatus.TrackChanged || obj == Controller.SpotifyController.eSpotifyStatus.Playing)
			{
				this.Dispatcher.BeginInvoke(new Action<Controller.SpotifyController>(ShowBalloon), sender);
			}
		}

		private void ShowBalloon(object data)
		{
			NotifyBalloon balloon = new NotifyBalloon();
			balloon.TrackInfos = ((Controller.SpotifyController)data).TrackInfos;

			lock (MyNotifyIcon)
			{
				if (MyNotifyIcon.CustomBalloon != null && MyNotifyIcon.CustomBalloon.IsOpen)
				{
					MyNotifyIcon.ChangeBalloonCloseTimer(NotifyBalloon.DISPLAY_DELAY);
				}
				else
				{
					MyNotifyIcon.ShowCustomBalloon(balloon, PopupAnimation.Fade, NotifyBalloon.DISPLAY_DELAY);
				}
			}
		}


		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.DragMove();
		}
	}
}
