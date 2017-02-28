using CostDaily.Common;
using CostDaily.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace CostDaily
{

    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.CategoriesContext.SizeChanged += CategoriesContext_SizeChanged;
            this.MainPage_Pivot.SizeChanged += MainPage_Pivot_SizeChanged;

            EllipseClickAnimation.Completed += (s, e) =>
            {
                Services.Navigation.GotoSettingsPage();
            };
            EllipseButtonBorder.Tapped += (s, e) =>
            {
                EllipseClickAnimation.Begin();           
            };

            App.DataModel.DataRefreshed += RedrawCharts;           
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.None;
            _updateAppScaleFactor();

            if (this.DataContext==null) this.DataContext = await App.DataModel.GetDataBase();

        }
        private void RedrawCharts(object sender, EventArgs e)
        {
            double temporaryPageWidth = this.ActualWidth;
            App.DataModel.UpdateResponsiveChartParametersForSummary((int)Math.Floor(temporaryPageWidth), .8f);
        }  
            
        private void _addCost_Click(object sender, RoutedEventArgs e)
        {

            CategoryWithStats tempContainer = (
                (CategoryWithStats)
                    (
                        (ItemClickEventArgs)e
                    ).ClickedItem
                );
            Services.Navigation.GotoAddCostPage(tempContainer);
        }
        private void _cost_Holding(object sender, HoldingRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            //FrameworkElement parentGrid = senderElement.Parent as FrameworkElement; odkomentuj i zmien sendery na parent

            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);
        }
        private void _deleteButton_Click(object sender, RoutedEventArgs e)
        {
            Cost tempContainer = (e.OriginalSource as FrameworkElement).DataContext as Cost;
            App.DataModel.RemoveCost(tempContainer);
        }
 
        private void MainPage_Pivot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            App.DataModel.UpdateResponsivePivotHeaderWidth((int)Math.Floor(senderElement.ActualWidth), HeaderListView.Items.Count);
        }
        private void CategoriesContext_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            App.DataModel.UpdateResponsiveCategoryImageSize((int)Math.Floor(senderElement.ActualWidth));
        }
        private void NotificationTextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TextBlock notificationTextBlock = sender as TextBlock;
            Grid parentGrid = notificationTextBlock.Parent as Grid;
            Polygon currentNotification = parentGrid.Children.FirstOrDefault(p => p is Polygon) as Polygon;

            double desiredWidth = notificationTextBlock.ActualWidth*App.ScaleFactor;
            double desiredHeight = notificationTextBlock.ActualHeight*App.ScaleFactor;

            if (desiredWidth < 25) desiredWidth = 25;

            if (currentNotification != null)
            {
                parentGrid.Children.Remove(currentNotification);
                parentGrid.Children.Insert(0, new NotificationShape().ReturnNotificationPolygon(parentGrid, desiredHeight, desiredWidth));
            }
            else
            {
                parentGrid.Children.Insert(0, new NotificationShape().ReturnNotificationPolygon(parentGrid, desiredHeight, desiredWidth));
            }
        }
        private void Rectangle_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Rectangle measuringRectangle = sender as Rectangle;
            double temporaryWidth;

            if (measuringRectangle != null)
            {
                temporaryWidth = measuringRectangle.ActualWidth;
            }
            else
            {
                temporaryWidth = this.ActualWidth * 0.6d;
            }

            App.DataModel.UpdateResponsiveChartParametersForSummary((int)Math.Floor(temporaryWidth), 0.95);
        }

        private void AdjustableListViewItemPresenter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement adjustableItemPresenter = sender as FrameworkElement;
            FrameworkElement ancestor = VisualTreeHelper.GetParent(adjustableItemPresenter) as FrameworkElement;

            while (ancestor as ListView == null)
            {
                ancestor = VisualTreeHelper.GetParent(ancestor) as FrameworkElement;
            }

            FrameworkElement footer = ((ListView)ancestor).Footer as FrameworkElement;
            if (footer == null) return;

            FrameworkElement topAppMenu = this.FindName("TopAppMenu") as FrameworkElement;

            List<double> elementList = new List<double>();
            elementList.Add(adjustableItemPresenter.ActualHeight);
            elementList.Add(topAppMenu.ActualHeight);

            FooterAdjustment(footer.Name, elementList);

        }
        private void FooterAdjustment(string footerName, List<double> measuredElements)
        {
            FrameworkElement footer = this.FindName(footerName) as FrameworkElement;
            double screenSize = this.ActualHeight;
            double measuredElementsSum = 0.0d;
            
            foreach (double element in measuredElements)
            {
                measuredElementsSum += element;
            }

            double desiredHeight = screenSize - measuredElementsSum + footer.ActualHeight;

            if (desiredHeight > 0)
            {
                footer.Height = desiredHeight;
            }
            else
            {
                if (desiredHeight + footer.ActualHeight > 0)
                {
                    footer.Height += desiredHeight;
                }
                else
                {
                    footer.Height = 0;
                }
            }
        }

        private void _settings_Click(object sender, RoutedEventArgs e)
        {
            Services.Navigation.GotoSettingsPage();
        }

        private void _updateAppScaleFactor()
        {
            if (DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel != double.NaN)
            {
                App.ScaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            }
        }
    }
}
