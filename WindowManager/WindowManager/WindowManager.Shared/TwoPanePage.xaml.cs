﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WindowManager
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TwoPanePage : Page
    {
        public TwoPanePage()
        {
            this.InitializeComponent();
        }

        void MainPage_Click(object sender, RoutedEventArgs e)
        {
            //IApplicationViewSpanningRects _foldable;
            //if (Uno.Foundation.Extensibility.ApiExtensibility.CreateInstance<IApplicationViewSpanningRects>(this, out var extension))
            //{
            //    _foldable = extension;
            //}

            Frame.Navigate(typeof(MainPage)); // TODO: go 'back'?
        }
    }
}
