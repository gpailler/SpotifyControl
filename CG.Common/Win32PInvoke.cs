using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Windows.Forms;

namespace CG.Common
{
	/// <summary>
	/// Wrap win32 functions
	/// </summary>
	/// <remarks>
	/// Sources: pinvoke.net, msdn.microsoft.com
	/// </remarks>
	public class Win32PInvoke
	{
		#region Private Interop

		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll", EntryPoint = "PostMessage", SetLastError = true)]
		private static extern bool PostMessageInternal(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);


		[return: MarshalAs(UnmanagedType.SysInt)]
		[DllImport("user32.dll", EntryPoint = "SetFocus", SetLastError = true)]
		private static extern IntPtr SetFocusInternal(IntPtr hWnd);


		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll", EntryPoint = "ShowWindow")]
		private static extern bool ShowWindowInternal(IntPtr hWnd, WindowShowStyle nCmdShow);


		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
		private static extern bool SetForegroundWindowInternal(IntPtr hWnd);


		[return: MarshalAs(UnmanagedType.I4)]
		[DllImport("user32.dll", EntryPoint = "GetWindowText", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int GetWindowTextInternal(IntPtr hWnd, StringBuilder lpString, int nMaxCount);


		[return: MarshalAs(UnmanagedType.I4)]
		[DllImport("user32.dll", EntryPoint = "GetWindowTextLength", SetLastError = true, CharSet = CharSet.Auto)]
		private static extern int GetWindowTextLengthInternal(IntPtr hWnd);


		[return: MarshalAs(UnmanagedType.SysInt)]
		[DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
		private static extern IntPtr FindWindowInternal(string lpClassName, string lpWindowName);


		[DllImport("user32.dll", EntryPoint = "keybd_event")]
		private static extern void keybd_eventInternal(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);


		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll", EntryPoint = "IsWindowVisible")]
		private static extern bool IsWindowVisibleInternal(IntPtr hWnd);


		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
		private static extern bool DeleteObjectInternal(IntPtr hObject);

		#endregion

		#region Public functions

		/// <summary>
		/// Places (posts) a message in the message queue associated with the thread
		/// that created the specified window and returns without waiting for
		/// the thread to process the message.
		/// </summary>
		/// <param name="hWnd">A handle to the window whose window procedure is to receive the message.</param>
		/// <param name="msg">The message to be posted.</param>
		/// <param name="wParam">Additional message-specific information.</param>
		/// <param name="lParam">Additional message-specific information.</param>
		/// <exception cref="System.ComponentModel.Win32Exception">Throw exception if function fails.</exception>
		/// <exception cref="System.ArgumentNullException">hWnd is null</exception>
		public static void PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
		{
			if (hWnd == null)
				throw new ArgumentNullException("hWnd");

			if (!PostMessageInternal(hWnd, msg, wParam, lParam))
				throw new Win32Exception(Marshal.GetLastWin32Error());
		}


		/// <summary>
		/// Sets the keyboard focus to the specified window. The window must be attached
		/// to the calling thread's message queue. 
		/// </summary>
		/// <param name="hWnd">
		/// A handle to the window that will receive the keyboard input.
		/// </param>
		/// <returns>
		/// Return value is the handle to the window that previously had the keyboard focus.
		/// </returns>
		/// <exception cref="System.ComponentModel.Win32Exception">
		/// Throw exception if window is not attached to the calling thread's message queue.
		/// </exception>
		/// <exception cref="System.ArgumentNullException">hWnd is null</exception>
		public static IntPtr SetFocus(IntPtr hWnd)
		{
			if (hWnd == null)
				throw new ArgumentNullException("hWnd");

			IntPtr result = SetFocusInternal(hWnd);

			if (result == IntPtr.Zero)
				throw new Win32Exception(Marshal.GetLastWin32Error());

			return result;
		}


		/// <summary>Shows a Window</summary>
		/// <remarks>
		/// <para>To perform certain special effects when showing or hiding a
		/// window, use AnimateWindow.</para>
		///<para>The first time an application calls ShowWindow, it should use
		///the WinMain function's nCmdShow parameter as its nCmdShow parameter.
		///Subsequent calls to ShowWindow must use one of the values in the
		///given list, instead of the one specified by the WinMain function's
		///nCmdShow parameter.</para>
		///<para>As noted in the discussion of the nCmdShow parameter, the
		///nCmdShow value is ignored in the first call to ShowWindow if the
		///program that launched the application specifies startup information
		///in the structure. In this case, ShowWindow uses the information
		///specified in the STARTUPINFO structure to show the window. On
		///subsequent calls, the application must call ShowWindow with nCmdShow
		///set to SW_SHOWDEFAULT to use the startup information provided by the
		///program that launched the application. This behavior is designed for
		///the following situations: </para>
		///<list type="">
		///    <item>Applications create their main window by calling CreateWindow
		///    with the WS_VISIBLE flag set. </item>
		///    <item>Applications create their main window by calling CreateWindow
		///    with the WS_VISIBLE flag cleared, and later call ShowWindow with the
		///    SW_SHOW flag set to make it visible.</item>
		///</list></remarks>
		/// <param name="hWnd">Handle to the window.</param>
		/// <param name="nCmdShow">Specifies how the window is to be shown.
		/// This parameter is ignored the first time an application calls
		/// ShowWindow, if the program that launched the application provides a
		/// STARTUPINFO structure. Otherwise, the first time ShowWindow is called,
		/// the value should be the value obtained by the WinMain function in its
		/// nCmdShow parameter. In subsequent calls, this parameter can be one of
		/// the WindowShowStyle members.</param>
		/// <returns>
		/// If the window was previously visible, the return value is nonzero.
		/// If the window was previously hidden, the return value is zero.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">hWnd is null</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">nCmdShow is invalid</exception>
		public static bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow)
		{
			if (hWnd == null)
				throw new ArgumentNullException("hWnd");

			if (!Enum.IsDefined(typeof(WindowShowStyle), nCmdShow))
				throw new ArgumentOutOfRangeException("nCmdShow");

			return ShowWindowInternal(hWnd, nCmdShow);
		}


		/// <summary>Enumeration of the different ways of showing a window using
		/// ShowWindow</summary>
		public enum WindowShowStyle : uint
		{
			/// <summary>Hides the window and activates another window.</summary>
			/// <remarks>See SW_HIDE</remarks>
			Hide = 0,
			/// <summary>Activates and displays a window. If the window is minimized
			/// or maximized, the system restores it to its original size and
			/// position. An application should specify this flag when displaying
			/// the window for the first time.</summary>
			/// <remarks>See SW_SHOWNORMAL</remarks>
			ShowNormal = 1,
			/// <summary>Activates the window and displays it as a minimized window.</summary>
			/// <remarks>See SW_SHOWMINIMIZED</remarks>
			ShowMinimized = 2,
			/// <summary>Activates the window and displays it as a maximized window.</summary>
			/// <remarks>See SW_SHOWMAXIMIZED</remarks>
			ShowMaximized = 3,
			/// <summary>Maximizes the specified window.</summary>
			/// <remarks>See SW_MAXIMIZE</remarks>
			Maximize = 3,
			/// <summary>Displays a window in its most recent size and position.
			/// This value is similar to "ShowNormal", except the window is not
			/// actived.</summary>
			/// <remarks>See SW_SHOWNOACTIVATE</remarks>
			ShowNormalNoActivate = 4,
			/// <summary>Activates the window and displays it in its current size
			/// and position.</summary>
			/// <remarks>See SW_SHOW</remarks>
			Show = 5,
			/// <summary>Minimizes the specified window and activates the next
			/// top-level window in the Z order.</summary>
			/// <remarks>See SW_MINIMIZE</remarks>
			Minimize = 6,
			/// <summary>Displays the window as a minimized window. This value is
			/// similar to "ShowMinimized", except the window is not activated.</summary>
			/// <remarks>See SW_SHOWMINNOACTIVE</remarks>
			ShowMinNoActivate = 7,
			/// <summary>Displays the window in its current size and position. This
			/// value is similar to "Show", except the window is not activated.</summary>
			/// <remarks>See SW_SHOWNA</remarks>
			ShowNoActivate = 8,
			/// <summary>Activates and displays the window. If the window is
			/// minimized or maximized, the system restores it to its original size
			/// and position. An application should specify this flag when restoring
			/// a minimized window.</summary>
			/// <remarks>See SW_RESTORE</remarks>
			Restore = 9,
			/// <summary>Sets the show state based on the SW_ value specified in the
			/// STARTUPINFO structure passed to the CreateProcess function by the
			/// program that started the application.</summary>
			/// <remarks>See SW_SHOWDEFAULT</remarks>
			ShowDefault = 10,
			/// <summary>Windows 2000/XP: Minimizes a window, even if the thread
			/// that owns the window is hung. This flag should only be used when
			/// minimizing windows from a different thread.</summary>
			/// <remarks>See SW_FORCEMINIMIZE</remarks>
			ForceMinimized = 11
		}


		/// <summary>
		/// Brings the thread that created the specified window into the foreground
		/// and activates the window. Keyboard input is directed to the window, 
		/// and various visual cues are changed for the user. The system assigns 
		/// a slightly higher priority to the thread that created the foreground 
		/// window than it does to other threads. 
		/// </summary>
		/// <param name="hWnd">A handle to the window that should be activated and brought to the foreground.</param>
		/// <returns>
		/// If the window was brought to the foreground, the return value is true.
		/// If the window was not brought to the foreground, the return value is false.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">hWnd is null</exception>
		public static bool SetForegroundWindow(IntPtr hWnd)
		{
			if (hWnd == null)
				throw new ArgumentNullException("hWnd");

			return SetForegroundWindowInternal(hWnd);
		}


		/// <summary>
		/// Copies the text of the specified window's title bar (if it has one) into a buffer.
		/// If the specified window is a control, the text of the control is copied.
		/// However, GetWindowText cannot retrieve the text of a control in another application.
		/// </summary>
		/// <param name="hWnd">A handle to the window or control containing the text.</param>
		/// <param name="lpString">
		/// The buffer that will receive the text. If the string is as long
		/// or longer than the buffer, the string is truncated and terminated with a null character.
		/// </param>
		/// <param name="nMaxCount">
		/// The maximum number of characters to copy to the buffer, including the null character.
		/// If the text exceeds this limit, it is truncated.
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is the length, in characters,
		/// of the copied string, not including the terminating null character.
		/// </returns>
		/// <exception cref="System.ComponentModel.Win32Exception">
		/// Throw an exception if the window has no title bar or text, if the title bar is empty,
		/// or if the window or control handle is invalid.
		/// </exception>
		/// <exception cref="System.ArgumentNullException">hWnd is null</exception>
		public static int GetWindowText(IntPtr hWnd, ref StringBuilder lpString, int nMaxCount)
		{
			if (hWnd == null)
				throw new ArgumentNullException("hWnd");

			int result = GetWindowTextInternal(hWnd, lpString, nMaxCount);

			if (result == 0)
				throw new Win32Exception(Marshal.GetLastWin32Error());

			return result;
		}


		/// <summary>
		/// Copies the text of the specified window's title bar (if it has one) into a buffer.
		/// If the specified window is a control, the text of the control is copied.
		/// However, GetWindowText cannot retrieve the text of a control in another application.
		/// </summary>
		/// <remarks>
		/// This function wraps original GetWindowText function to simplify usage.
		/// </remarks>
		/// <param name="hWnd">A handle to the window or control containing the text.</param>
		/// <returns>
		/// If the function succeeds, the return value is the window text.
		/// </returns>
		/// <exception cref="System.ComponentModel.Win32Exception">
		/// Throw an exception if the window has no title bar or text, if the title bar is empty,
		/// or if the window or control handle is invalid.
		/// </exception>
		/// <exception cref="System.ArgumentNullException">hWnd is null</exception>
		public static string GetWindowText(IntPtr hWnd)
		{
			if (hWnd == null)
				throw new ArgumentNullException("hWnd");

			int length = GetWindowTextLengthInternal(hWnd);
			StringBuilder sb = new StringBuilder(length + 1);

			int result = GetWindowTextInternal(hWnd, sb, sb.Capacity);
			if (result == 0 || result != length)
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}

			return sb.ToString();
		}

		/// <summary>
		/// Retrieves a handle to the top-level window whose class name and window name match 
		/// the specified strings. This function does not search child windows.
		/// This function does not perform a case-sensitive search.
		/// </summary>
		/// <param name="lpClassName">
		/// The class name or a class atom created by a previous call to the RegisterClass or RegisterClassEx function.
		/// The atom must be in the low-order word of lpClassName; the high-order word must be zero.
		/// If lpClassName points to a string, it specifies the window class name. 
		/// The class name can be any name registered with RegisterClass or RegisterClassEx, 
		/// or any of the predefined control-class names.
		/// If lpClassName is null, it finds any window whose title matches the lpWindowName parameter. 
		/// </param>
		/// <param name="lpWindowName">
		/// The window name (the window's title). If this parameter is null, all window names match. 
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is a handle to the window that has the specified class name and window name.
		/// If the function fails, the return value is null.
		/// </returns>
		public static IntPtr FindWindow(string lpClassName, string lpWindowName)
		{
			return FindWindowInternal(lpClassName, lpWindowName);
		}


		/// <summary>
		/// Synthesizes a keystroke. The system can use such a synthesized keystroke to generate
		/// a WM_KEYUP or WM_KEYDOWN message. The keyboard driver's interrupt handler
		/// calls the keybd_event function.
		/// </summary>
		/// <param name="bVk">A virtual-key code. The code must be a value in the range 1 to 254.</param>
		/// <param name="bScan">A hardware scan code for the key.</param>
		/// <param name="dwFlags">Controls various aspects of function operation.</param>
		/// <param name="dwExtraInfo">An additional value associated with the key stroke. </param>
		public static void keybd_event(Keys bVk, byte bScan, KeyEvent keyEvent, UIntPtr dwExtraInfo)
		{
			keybd_eventInternal((byte)bVk, bScan, (uint)keyEvent, dwExtraInfo);
		}

		public enum KeyEvent : uint
		{
			/// <summary>
			/// Default
			/// </summary>
			None = 0,

			/// <summary>
			/// The scan code was preceded by a prefix byte having the value 0xE0 (224).
			/// </summary>
			ExtendedKey = 1,

			/// <summary>
			/// The key is being released
			/// </summary>
			KeyUp = 2
		}

		/// <summary>
		/// Determines the visibility state of the specified window.
		/// </summary>
		/// <param name="hWnd">A handle to the window to be tested.</param>
		/// <returns>
		/// If the specified window, its parent window, its parent's parent window, and so forth, 
		/// have the WS_VISIBLE style, the return value is true.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">hWnd is null</exception>
		public static bool IsWindowVisible(IntPtr hWnd)
		{
			if (hWnd == null)
				throw new ArgumentNullException("hWnd");

			return IsWindowVisibleInternal(hWnd);
		}


		/// <summary>
		/// The DeleteObject function deletes a logical pen, brush, font, bitmap, region, 
		/// or palette, freeing all system resources associated with the object. 
		/// After the object is deleted, the specified handle is no longer valid.
		/// </summary>
		/// <param name="hObject">A handle to a logical pen, brush, font, bitmap, region, or palette.</param>
		/// <returns>true if function succeeds, false else</returns>
		/// <exception cref="System.ArgumentException">hObject is invalid</exception>
		public static bool DeleteObject(IntPtr hObject)
		{
			if (hObject == IntPtr.Zero)
				throw new ArgumentException("hObject");

			return DeleteObjectInternal(hObject);
		}

		#endregion
	}
}
