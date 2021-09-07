using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Uno.Foundation.Extensibility;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WindowManager
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        INativeFoldableProvider _foldable;
        bool isFoldable = false;

        private HingeAngleSensor _hinge;
        private bool _readingChangedAttached;
        private double _angle;
        private string _timestamp;

        public MainPage()
        {
            this.InitializeComponent();

            Loaded += PageLoaded;
        }

        async void PageLoaded(object sender, RoutedEventArgs e)
        {
            _hinge = await HingeAngleSensor.GetDefaultAsync();
            if (_hinge != null)
            {
                hingeSensorStatus.Text = "HingeAngleSensor created";
                _hinge.ReadingChanged += HingeAngleSensor_ReadingChanged;
            }
            else
            {
                hingeSensorStatus.Text = "HingeAngleSensor not available on this device";
            }
 
            RecalculateRects();
        }

        async void HingeAngleSensor_ReadingChanged(HingeAngleSensor sender, HingeAngleSensorReadingChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                _angle = args.Reading.AngleInDegrees;
                _timestamp = args.Reading.Timestamp.ToString("R");

                hingeSensorStatus.Text = "HingeAngleSensor " + Math.Round(_angle,0) + "°";
            });
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);

            if (ApiExtensibility.CreateInstance<INativeFoldableProvider>(this, out _foldable))
            {
                isFoldable = true;
            }
            else
            {
                isFoldable = false;
            }

            RecalculateRects();
        }
        private async void RecalculateRects()
        {
            text.Text = "Single screen (not spanned)";

#if __ANDROID__
            // None of this is really Android specific, but we know it's a no-op on other platforms so...
            var spanningRects = ApplicationView.GetForCurrentView().GetSpanningRects();
            if (spanningRects == null || spanningRects.Count == 0)
            {
                return;
            }
            else
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                    //UI code here
                    text.Text = "";
                    foreach (var rect in spanningRects)
                    {
                        text.Text += rect.ToString() + "\n";
                    }
                    if (isFoldable) {
                        text.Text += "Occluding: " + _foldable.IsOccluding + "\n";
                        text.Text += "Flat: " + _foldable.IsFlat + "\n";
                        text.Text += "Vertical: " + _foldable.IsVertical + "\n";
                    }
                });
            }
#endif
        }

        void TwoPaneView_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(TwoPanePage));
        }
    }
}
