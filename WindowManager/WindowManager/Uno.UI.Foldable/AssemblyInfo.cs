using Windows.UI.ViewManagement;
using Uno.Devices.Sensors;
using Uno.Foundation.Extensibility;
using Uno.UI.Foldable;

// Jetpack Window Manager - TODO these aren't working, see manual setup in **App.xaml.cs**
// Could be as simple as "not in a NuGet package", according to
// https://platform.uno/docs/articles/uno-development/api-extensions.html
// which suggests these are only generated in App.InitializeComponent() when found in a NuGet
// (and not in the current application, as in this case)...
[assembly: ApiExtension(typeof(IApplicationViewSpanningRects), typeof(FoldableApplicationViewSpanningRects))]
[assembly: ApiExtension(typeof(INativeFoldableProvider), typeof(FoldableApplicationViewSpanningRects))]
[assembly: ApiExtension(typeof(INativeHingeAngleSensor), typeof(FoldableHingeAngleSensor))]