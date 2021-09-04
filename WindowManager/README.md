# Jetpack Window Manager for Uno Platfom

> **NOTE:** This is currently a work-in-progress (4-Sep-21). The implementation is tightly coupled to the Android MainActivity and ignores the event-based nature of Window Manager - to be resolved once the features are at least working from a measure/layout perspective.

Use [Xamarin.AndroidX.WindowJava](https://www.nuget.org/packages/Xamarin.AndroidX.Window.WindowJava) NuGet to test Jetpack Window Manager with Uno Platform Android apps.

## Surface Duo

Added a new interface `INativeFoldableProvider` to expose new data available from Window Manager's foldable feature class:

![Surface Duo with Uno Platform and Jetpack Window Manager](Screenshots/surfaceduo-windowmanager-land.png)

Replaced the implementation of `IApplicationViewSpanningRects` with one using Window Manager, and TwoPaneView seems to work^

![Surface Duo with Uno Platform and TwoPaneView](Screenshots/surfaceduo-twopaneview-land.png)

![Surface Duo with Uno Platform and TwoPaneView](Screenshots/surfaceduo-twopaneview-port.png)

_^ still some bugs to fix_

## Galaxy Fold 2

![Galaxy Fold 2 with Uno Platform and Jetpack Window Manager](Screenshots/galaxyfold-windowmanager-port.png)

Replaced the implementation of `IApplicationViewSpanningRects` with one using Window Manager, and TwoPaneView seems to work^

![Galaxy Fold 2 with Uno Platform and TwoPaneView](Screenshots/galaxyfold-twopaneview-land.png)

![Galaxy Fold 2 with Uno Platform and TwoPaneView](Screenshots/galaxyfold-twopaneview-port.png)