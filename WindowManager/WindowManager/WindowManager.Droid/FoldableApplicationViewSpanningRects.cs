﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uno.Extensions;
using Uno.Logging;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Uno.Devices.Sensors;
using Uno.UI;

namespace WindowManager.Droid
{
	/// <summary>
	/// Provides two Rect that represent the two screen dimensions when
	/// an Android application is spanned across a hinge or fold (eg. Surface Duo)
	/// </summary>
	/// <remarks>
	/// Relies on the MainActivity implementing Jetpack Window Manager layout change listener,
	/// and exposing the properties needed to make UI change when required.
	/// HACK: need to implement an event for layout changes, so we can detect folding state
	/// </remarks>
    public class FoldableApplicationViewSpanningRects : IApplicationViewSpanningRects, INativeDualScreenProvider, INativeFoldableProvider
	{
		private (SurfaceOrientation orientation, List<Rect> result) _previousMode = EmptyMode;

		private static readonly (SurfaceOrientation orientation, List<Rect> result) EmptyMode =
			((SurfaceOrientation)(-1), null);

		private readonly List<Rect> _emptyList = new List<Rect>(0);

		public FoldableApplicationViewSpanningRects(object owner)
		{
		}

		public IReadOnlyList<Rect> GetSpanningRects()
		{
			if (ContextHelper.Current is MainActivity currentActivity)
			{
				if (currentActivity.HasFoldFeature) // IsSeparating or just "is fold present?" - changing this will affect the behavior of TwoPaneView on foldable devices
				{
                    _previousMode.orientation = currentActivity.Orientation;
                    _previousMode.result = null;

                    var wuxWindowBounds = ApplicationView.GetForCurrentView().VisibleBounds.LogicalToPhysicalPixels();
                    var wuOrientation = DisplayInformation.GetForCurrentView().CurrentOrientation;

					// TODO: bring the list of all folding features here, for future compatibility
					List<Rect> occludedRects = new List<Rect>
                        {   // Hinge/fold bounds
							new Rect(currentActivity.FoldBounds.Left,
                            currentActivity.FoldBounds.Top,
                            currentActivity.FoldBounds.Width(),
                            currentActivity.FoldBounds.Height())
                        };

					if (occludedRects.Count > 0)
                    {
                        if (occludedRects.Count > 1 && this.Log().IsEnabled(Microsoft.Extensions.Logging.LogLevel.Warning))
                        {
                            this.Log().Warn($"DualMode: Unknown screen layout, more than one occluded region. Only first will be considered. Please report your device to Uno Platform!");
                        }

                        var bounds = wuxWindowBounds;
                        var occludedRect = occludedRects[0];
                        var intersecting = ((Android.Graphics.RectF)bounds).Intersect(occludedRect);

						//if (wuOrientation == DisplayOrientations.Portrait || wuOrientation == DisplayOrientations.PortraitFlipped)  // *Device* portrait assumption works for Surface Duo, but not other foldables which have a vertical hinge in portrait mode
						if (currentActivity.FoldOrientation == AndroidX.Window.Layout.FoldingFeatureOrientation.Horizontal)
                        {
                            // Compensate for the status bar size (the occluded area is rooted on the screen size, whereas
                            // wuxWindowBoundsis rooted on the visible size of the window, unless the status bar is translucent.
                            if ((int)bounds.X == 0 && (int)bounds.Y == 0)
                            {
                                var statusBarRect = StatusBar.GetForCurrentView().OccludedRect.LogicalToPhysicalPixels();
                                occludedRect.Y -= statusBarRect.Height;
                            }
                        }

                        if (intersecting) // Occluded region overlaps the app
                        {
                            if ((int)occludedRect.X == (int)bounds.X)
                            {
                                // Vertical stacking
                                // +---------+
                                // |         |
                                // |         |
                                // +---------+
                                // +---------+
                                // |         |
                                // |         |
                                // +---------+

                                var spanningRects = new List<Rect> {
												// top region
												new Rect(bounds.X, bounds.Y, bounds.Width, occludedRect.Top),
												// bottom region
												new Rect(bounds.X,
                                                    occludedRect.Bottom,
                                                    bounds.Width,
                                                    bounds.Height - occludedRect.Bottom),
                                            };

                                if (this.Log().IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug))
                                {
                                    this.Log().Debug($"DualMode: Horizontal spanning rects: {string.Join(";", spanningRects)}");
                                }

                                _previousMode.result = spanningRects;
                            }
                            else if ((int)occludedRect.Y == (int)bounds.Y)
                            {
                                // Horizontal side-by-side
                                // +-----+ +-----+
                                // |     | |     |
                                // |     | |     |
                                // |     | |     |
                                // |     | |     |
                                // |     | |     |
                                // +-----+ +-----+

                                var spanningRects = new List<Rect> {
												// left region
												new Rect(bounds.X, bounds.Y, occludedRect.X, bounds.Height),
												// right region
												new Rect(occludedRect.Right,
                                                    bounds.Y,
                                                    bounds.Width - occludedRect.Right,
                                                    bounds.Height),
                                            };

                                if (this.Log().IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug))
                                {
                                    this.Log().Debug($"DualMode: Vertical spanning rects: {string.Join(";", spanningRects)}");
                                }

                                _previousMode.result = spanningRects;
                            }
                            else
                            {
                                if (this.Log().IsEnabled(Microsoft.Extensions.Logging.LogLevel.Warning))
                                {
                                    this.Log().Warn($"DualMode: Unknown screen layout");
                                }
                            }
                        }
                        else
                        {
                            if (this.Log().IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug))
                            {
                                this.Log().Debug($"DualMode: Without intersection, single screen");
                            }
                        }
                    }
                    else
                    {
                        if (this.Log().IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug))
                        {
                            this.Log().Debug($"DualMode: Without occlusion");
                        }
                    }
				}
				else
                {
                    _previousMode = EmptyMode;
                }
            }
			return _previousMode.result ?? _emptyList;
		}

		private SurfaceOrientation GetOrientation()
		{
			switch (DisplayInformation.GetForCurrentView().CurrentOrientation)
			{
				default:
				case DisplayOrientations.Portrait:
					return SurfaceOrientation.Rotation0;
				case DisplayOrientations.Landscape:
					return SurfaceOrientation.Rotation90;
				case DisplayOrientations.PortraitFlipped:
					return SurfaceOrientation.Rotation180;
				case DisplayOrientations.LandscapeFlipped:
					return SurfaceOrientation.Rotation270;
			}
		}

		public bool? IsSpanned
		{
			get
			{
				if (ContextHelper.Current is MainActivity currentActivity)
				{
					return currentActivity.IsSeparating;
				}

				return null;
			}
		}

		[Obsolete("Prefer IsSpanned, since it provides a better experience on non-occluding folding features")]
		public bool IsDualScreen
		{
			get
			{
				if (!(ContextHelper.Current is MainActivity currentActivity))
				{
					throw new InvalidOperationException("The API was called too early in the application lifecycle");
				}

				return currentActivity.HasFoldFeature;
			}
		}

        public bool IsOccluding
        {
			get {
				if (ContextHelper.Current is MainActivity currentActivity)
				{
					return currentActivity.FoldOcclusionType == AndroidX.Window.Layout.FoldingFeatureOcclusionType.Full;
				}

				return false;
			}
		}

        public bool IsFlat {
			get
			{
				if (ContextHelper.Current is MainActivity currentActivity)
				{
					return currentActivity.FoldState == AndroidX.Window.Layout.FoldingFeatureState.Flat;
				}

				return true;
			}
		}

		public bool IsVertical
		{
			get
			{
				if (ContextHelper.Current is MainActivity currentActivity)
				{
					return currentActivity.FoldOrientation == AndroidX.Window.Layout.FoldingFeatureOrientation.Vertical;
				}

				return true;
			}
		}

        public Rect Bounds
        {
			get {
				if (ContextHelper.Current is MainActivity currentActivity && currentActivity.FoldBounds != null)
				{
					return new Rect(currentActivity.FoldBounds.Left,
						currentActivity.FoldBounds.Top,
						currentActivity.FoldBounds.Width(),
						currentActivity.FoldBounds.Height());
				}

				return new Rect(0,0,0,0);
			}
		}
    }
}
