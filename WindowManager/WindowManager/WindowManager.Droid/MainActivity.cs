using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using AndroidX.Core.Util;
using AndroidX.Window.Java.Layout;
using AndroidX.Window.Layout;
using Java.Interop;
using Java.Lang;
using Java.Util.Concurrent;
using System;
using Uno.UI.ViewManagement;
using Windows.UI.ViewManagement;


namespace WindowManager.Droid
{
    /// <summary>
    /// This activity contains a lot of foldable-specific stuff that we need to refactor out
    /// </summary>
    /// <remarks>
    /// Code to access Jetpack Window Manager features from Xamarin.AndroidX.Window.WindowJava NuGet
    /// from https://github.com/microsoft/surface-duo-sdk-xamarin-samples/tree/main/WindowManager
    /// Public properties exposed here are consumed by FoldableApplicationViewSpanningRects
    /// which implements IApplicationViewSpanningRects, the interface that is loaded and
    /// used by TwoPaneView
    /// </remarks>
    [Activity(
			MainLauncher = true,
			ConfigurationChanges = global::Uno.UI.ActivityHelper.AllConfigChanges,
			WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden
		)]
	public class MainActivity : Windows.UI.Xaml.ApplicationActivity
	{
		const string TAG = "JWM"; // Jetpack Window Manager

		public MainActivity()
		{
		}
    }
}

