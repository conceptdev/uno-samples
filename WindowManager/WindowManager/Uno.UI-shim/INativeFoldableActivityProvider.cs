using System;
using System.Collections.Generic;
using System.Text;

namespace Windows.UI.ViewManagement
{
    public interface INativeFoldableActivityProvider
    {
        bool HasFoldFeature { get; }
        bool IsSeparating { get; }
        bool IsFoldVertical { get; }
        Android.Graphics.Rect FoldBounds { get; }

        event EventHandler<NativeFold> LayoutChanged;
    }
}
