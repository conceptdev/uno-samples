using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Android.App;

using Windows.UI.ViewManagement;
using Uno.Devices.Sensors;
using Uno.Foundation.Extensibility;

using WindowManager.Droid;
using WindowManager;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("WindowManager.Droid")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("WindowManager.Droid")]
[assembly: AssemblyCopyright("Copyright ©  2021")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]




// Jetpack Window Manager - TODO these aren't working
[assembly: ApiExtension(typeof(IApplicationViewSpanningRects), typeof(FoldableApplicationViewSpanningRects))]
[assembly: ApiExtension(typeof(INativeFoldableProvider), typeof(FoldableApplicationViewSpanningRects))]
