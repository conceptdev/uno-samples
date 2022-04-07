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

namespace SimpleCalculator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        int currentState = 1;
        string mathOperator;
        double firstNumber, secondNumber;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ellipse.Margin = new Thickness { Left = -ActualWidth * 2, Top = -ActualHeight * 2, Right = -ActualWidth * 2, Bottom = -ActualHeight * 2 };
            ellipse.Height = ActualHeight * 4;
            ellipse.Width = ActualWidth * 4;
            var transform = ellipse.RenderTransform as CompositeTransform;
            transform.CenterX = ActualWidth * 2;
            transform.CenterY = ActualHeight * 2;
        }

        void OnSelectNumber(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string pressed = button.Content as string;

            if (this.resultText.Text == "0" || currentState < 0)
            {
                this.resultText.Text = "";
                if (currentState < 0)
                    currentState *= -1;
            }

            if (pressed == ".")
                pressed = ".00";// not optimistic

            this.resultText.Text += pressed;

            double number;
            if (double.TryParse(this.resultText.Text, out number))
            {
                this.resultText.Text = number.ToString("N0");
                if (currentState == 1)
                {
                    firstNumber = number;
                }
                else
                {
                    secondNumber = number;
                }
            }
        }

        void OnSelectOperator(object sender, RoutedEventArgs e)
        {
            currentState = -2;
            Button button = (Button)sender;
            string pressed = button.Content as string;
            mathOperator = pressed;
        }

        void OnClear(object sender, RoutedEventArgs e)
        {
            firstNumber = 0;
            secondNumber = 0;
            currentState = 1;
            this.resultText.Text = "0";
            this.history.Text = "";
        }

        void OnCalculate(object sender, RoutedEventArgs e)
        {
            if (currentState == 2)
            {
                double result = Calculator.Calculate(firstNumber, secondNumber, mathOperator);

                this.CurrentCalculation.Text = $"{firstNumber} {mathOperator} {secondNumber}";
                this.history.Text += $"\n{firstNumber} {mathOperator} {secondNumber}";

                this.resultText.Text = result.ToTrimmedString();
                this.history.Text += $"\n{result.ToTrimmedString()}";
                firstNumber = result;
                currentState = -1;


            }
        }



        void OnNegative(object sender, RoutedEventArgs e)
        {
            if (currentState == 1)
            {
                secondNumber = -1;
                mathOperator = "×";
                currentState = 2;
                OnCalculate(this, null);
            }
        }

        void OnPercentage(object sender, RoutedEventArgs e)
        {
            if (currentState == 1)
            {
                secondNumber = 0.01;
                mathOperator = "×";
                currentState = 2;
                OnCalculate(this, null);
            }

        }

        int themeIndex = 0;

        ResourceDictionary[] themes = new ResourceDictionary[]
        {
            //new ClayTheme(),
            //new DesertTheme(),
            //new LavaTheme(),
            //new SunTheme(),
            //new OceanTheme()
        };

        async void ThemeSwitcher_Clicked(System.Object sender, RoutedEventArgs e)
        {
            themeIndex += 1;
            if (themeIndex >= themes.Length)
            {
                themeIndex = 0;
            }

            var transform = ellipse.RenderTransform as CompositeTransform;
            transform.ScaleX = 0;
            transform.ScaleY = 0;

            // Switch current themee
            var newResources = themes[themeIndex];
            App.Current.Resources.MergedDictionaries.Clear();
            App.Current.Resources.MergedDictionaries.Add(newResources);


            // Hack: force themes to be reapplied by switching between Dark and Light themes on root frame
            if (this.Frame.RequestedTheme == ElementTheme.Dark)
            {
                this.Frame.RequestedTheme = ElementTheme.Light;
                this.Frame.RequestedTheme = ElementTheme.Dark;
            }
            else
            {
                this.Frame.RequestedTheme = ElementTheme.Dark;
                this.Frame.RequestedTheme = ElementTheme.Light;
            }

            // Run the transition animation
            //            await themeTransitionStoryboard.RunAsync();

            Background = ellipse.Fill;
        }
    }
}
