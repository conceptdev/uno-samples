using Windows.UI.ViewManagement;
using Uno.Devices.Sensors;
using Uno.Foundation.Extensibility;
using Uno.UI.Foldable;

// According to
// https://platform.uno/docs/articles/uno-development/api-extensions.html
// these are generated in App.InitializeComponent() 
[assembly: ApiExtension(typeof(IApplicationViewSpanningRects), typeof(FoldableApplicationViewSpanningRects))]
[assembly: ApiExtension(typeof(INativeFoldableProvider), typeof(FoldableApplicationViewSpanningRects))]
[assembly: ApiExtension(typeof(INativeHingeAngleSensor), typeof(FoldableHingeAngleSensor))]