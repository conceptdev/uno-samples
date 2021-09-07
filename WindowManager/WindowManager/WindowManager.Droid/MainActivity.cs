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
	public class MainActivity : Windows.UI.Xaml.ApplicationActivity, INativeFoldableActivityProvider, IConsumer
	{
		const string TAG = "JWM"; // Jetpack Window Manager
		WindowInfoRepositoryCallbackAdapter wir;
		IWindowMetricsCalculator wmc;

		// HACK: expose properties for FoldableApplicationViewSpanningRects
		public bool HasFoldFeature { get; set; }
		public bool IsSeparating { get; set; }
		public bool IsFoldVertical { get; set; }
		public Android.Graphics.Rect FoldBounds { get; set; }
		[Obsolete("Can't surface this in a platform agnostic way")]
		public SurfaceOrientation Orientation = SurfaceOrientation.Rotation0;
		public FoldingFeatureState FoldState;
		public FoldingFeatureOcclusionType FoldOcclusionType;
		public FoldingFeatureOrientation FoldOrientation;

		private EventHandler<NativeFold> _layoutChanged;
		// ENDHACK
		public event EventHandler<NativeFold> LayoutChanged
		{
			add
			{
				var isFirstSubscriber = _layoutChanged == null;
				_layoutChanged += value;
			}
			remove
			{
				_layoutChanged -= value;
			}
		}
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

			FoldBounds = null;
			IsSeparating = false;
			HasFoldFeature = false;

			NativeFold lastFoldingFeature = null;

			foreach (var displayFeature in newLayoutInfo.DisplayFeatures)
			{
				var foldingFeature = displayFeature.JavaCast<IFoldingFeature>();

				if (foldingFeature != null) // HACK: requires JavaCast as shown above
				{
					// Set properties for FoldableApplicationViewSpanningRects to reference
					HasFoldFeature = true;
					IsSeparating = foldingFeature.IsSeparating;
					FoldBounds = foldingFeature.Bounds;
					FoldState = foldingFeature.State;
					FoldOcclusionType = foldingFeature.OcclusionType;

					if (foldingFeature.Orientation == FoldingFeatureOrientation.Horizontal)
					{
						//Orientation = SurfaceOrientation.Rotation90;
						IsFoldVertical = false;
						FoldOrientation = FoldingFeatureOrientation.Horizontal;
					}
					else
					{
						//Orientation = SurfaceOrientation.Rotation0; // HACK: what about 180 and 270?
						IsFoldVertical = true;
						FoldOrientation = FoldingFeatureOrientation.Vertical;
					}

					lastFoldingFeature = new NativeFold { Bounds = FoldBounds, 
						IsOccluding = foldingFeature.OcclusionType == FoldingFeatureOcclusionType.Full, 
						IsFlat = foldingFeature.State == FoldingFeatureState.Flat, 
						IsVertical = IsFoldVertical };
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


			_layoutChanged?.Invoke(this, lastFoldingFeature);
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

