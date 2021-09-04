using System;
using Windows.UI.ViewManagement;

namespace WindowManager
{
    public interface INativeFoldableProvider
    {
        // Choosing `bool` to hide custom types in Xamarin.AndroidX.Window
        // currently both these properties only have two values, although
        // this is not properly modelling them and is not future-proof

        /// <summary>
        /// true=FoldingFeatureOcclusionType.Full
        /// false=FoldingFeatureOcclusionType.None
        /// </summary>
        bool IsOccluding { get; }
        /// <summary>
        /// true=FoldingFeatureState.Flat
        /// false=FoldingFeatureState.HalfOpened
        /// </summary>
        bool IsFlat { get; }
        /// <summary>
        /// true=FoldingFeatureOrientation.Vertical
        /// false=FoldingFeatureOrientation.Horizontal
        /// </summary>
        bool IsVertical { get; }
    }
}
