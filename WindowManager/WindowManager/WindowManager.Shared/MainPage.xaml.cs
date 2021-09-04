using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Uno.Foundation.Extensibility;
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

        public MainPage()
        {
            this.InitializeComponent();

            Loaded += (snd, e) => RecalculateRects();
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
            text.Text = "-";
#if __ANDROID__
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
    }
}
