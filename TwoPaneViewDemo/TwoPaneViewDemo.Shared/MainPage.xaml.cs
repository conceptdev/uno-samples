using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TwoPaneViewDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            PanePriority.ItemsSource = Enum.GetValues(typeof(TwoPaneViewPriority));
            TallModeConfiguration.ItemsSource = Enum.GetValues(typeof(TwoPaneViewTallModeConfiguration));
            WideModeConfiguration.ItemsSource = Enum.GetValues(typeof(TwoPaneViewWideModeConfiguration));
            TwoPane.PanePriority = TwoPaneViewPriority.Pane1;

            PanePriority.SelectedIndex = 0;
            TallModeConfiguration.SelectedIndex = 1;
            WideModeConfiguration.SelectedIndex = 1;
        }

        

        void MinTallModeHeight_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            TwoPane.MinTallModeHeight = e.NewValue;
        }
        void MinWideModeWidth_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            TwoPane.MinWideModeWidth = e.NewValue;
        }
        void Pane1Length_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            TwoPane.Pane1Length = new GridLength(e.NewValue, GridUnitType.Star);
        }
        void Pane2Length_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            TwoPane.Pane2Length = new GridLength (e.NewValue, GridUnitType.Star);
        }

        void PanePriority_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TwoPane.PanePriority = (TwoPaneViewPriority)((ComboBox)sender).SelectedItem;
        }

        void TallModeConfiguration_SelectionChanged(object sender, SelectionChangedEventArgs e) 
        {
            TwoPane.TallModeConfiguration = (TwoPaneViewTallModeConfiguration)((ComboBox)sender).SelectedItem;
        }

        void WideModeConfiguration_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TwoPane.WideModeConfiguration = (TwoPaneViewWideModeConfiguration)((ComboBox)sender).SelectedItem;
        }

        void Reset_Click (object sender, RoutedEventArgs e)
        {
            TwoPane.MinTallModeHeight = 680;
            TwoPane.MinWideModeWidth = 540;

            TwoPane.PanePriority = TwoPaneViewPriority.Pane1;
            Pane1Length.Value = 0.5;
            Pane2Length.Value = 0.5;

            PanePriority.SelectedIndex = 0;
            TallModeConfiguration.SelectedIndex = 1;
            WideModeConfiguration.SelectedIndex = 1;
        }
    }
}
