﻿using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Views;

using AndroidX.AppCompat.App;
using AndroidX.Core.Util;
using AndroidX.Window.Layout;
using AndroidX.Window.Java.Layout;
using Java.Lang;
using Java.Util.Concurrent;
using Java.Interop;
using Android.Util;

namespace WindowManager.Droid
{
	[Activity(
			MainLauncher = true,
			ConfigurationChanges = global::Uno.UI.ActivityHelper.AllConfigChanges,
			WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden
		)]
	public class MainActivity : Windows.UI.Xaml.ApplicationActivity, IConsumer
	{
		const string TAG = "JWM"; // Jetpack Window Manager
		WindowInfoRepositoryCallbackAdapter wir;
		IWindowMetricsCalculator wmc;

		public bool IsSeparating = false;
		public SurfaceOrientation Orientation = SurfaceOrientation.Rotation0;
		public Android.Graphics.Rect FoldBounds;
		public FoldingFeatureState FoldState;
		public FoldingFeatureOcclusionType FoldOcclusionType;
		public FoldingFeatureOrientation FoldOrientation;

		protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
			wir = new WindowInfoRepositoryCallbackAdapter(WindowInfoRepository.Companion.GetOrCreate(this));
			wmc = WindowMetricsCalculator.Companion.OrCreate; // HACK: source method is `getOrCreate`, binding generator munges this badly :(
		}

		protected override void OnStart()
		{
			base.OnStart();
			wir.AddWindowLayoutInfoListener(runOnUiThreadExecutor(), this); // `this` is the IConsumer implementation
		}

		protected override void OnStop()
		{
			base.OnStop();
			wir.RemoveWindowLayoutInfoListener(this);
		}

		void layoutStateChange(WindowLayoutInfo newLayoutInfo)
		{
			Log.Info(TAG, "Current: " + wmc.ComputeCurrentWindowMetrics(this).Bounds.ToString());
			Log.Info(TAG, "Current: " + wmc.ComputeMaximumWindowMetrics(this).Bounds.ToString());

			if (newLayoutInfo.DisplayFeatures.Count == 0)
			{	// HACK: only works if no other feature types (eg. cutout)
				Log.Info(TAG, "One logic/physical display - unspanned");
				FoldBounds = null;
				IsSeparating = false;
			}
			foreach (var displayFeature in newLayoutInfo.DisplayFeatures)
			{
				var foldingFeature = displayFeature.JavaCast<IFoldingFeature>();

				if (foldingFeature != null) // HACK: requires JavaCast as shown above
				{
					IsSeparating = foldingFeature.IsSeparating;
					FoldBounds = foldingFeature.Bounds;
					FoldState = foldingFeature.State;
					FoldOcclusionType = foldingFeature.OcclusionType;
					if (foldingFeature.Orientation == FoldingFeatureOrientation.Horizontal)
					{
						Orientation = SurfaceOrientation.Rotation90;
						FoldOrientation = FoldingFeatureOrientation.Horizontal;
					}
					else
					{
						Orientation = SurfaceOrientation.Rotation0; // HACK: what about 180 and 270?
						FoldOrientation = FoldingFeatureOrientation.Vertical;
					}
					// DEBUG INFO
					if (foldingFeature.OcclusionType == FoldingFeatureOcclusionType.None)
					{
						Log.Info(TAG, "App is spanned across a fold");
					}
					if (foldingFeature.OcclusionType == FoldingFeatureOcclusionType.Full)
					{
						Log.Info(TAG, "App is spanned across a hinge");
					}
					var summary = "\nIsSeparating: " + foldingFeature.IsSeparating
							+ "\nOrientation: " + foldingFeature.Orientation  // FoldingFeatureOrientation.Vertical or Horizontal
							+ "\nState: " + foldingFeature.State; // FoldingFeatureState.Flat or HalfOpened
					Log.Info(TAG, summary);
				}
				else
				{
					Log.Info(TAG, "DisplayFeature is not a fold or hinge");
				}
			}
		}
		#region Used by WindowInfoRepository callback
		IExecutor runOnUiThreadExecutor()
		{
			return new MyExecutor();
		}
		class MyExecutor : Java.Lang.Object, IExecutor
		{
			Handler handler = new Handler(Looper.MainLooper);
			public void Execute(IRunnable r)
			{
				handler.Post(r);
			}
		}

		public void Accept(Java.Lang.Object newLayoutInfo)  // Object will be WindowLayoutInfo
		{
			Log.Info(TAG, "===LayoutStateChangeCallback.Accept");
			Log.Info(TAG, newLayoutInfo.ToString());
			layoutStateChange(newLayoutInfo as WindowLayoutInfo);
		}
		#endregion
	}
}

