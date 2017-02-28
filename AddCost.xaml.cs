using System;
using CostDaily.Common;
using CostDaily.DataModel;
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
using Windows.Graphics.Display;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace CostDaily
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddCost : Page
    {

        public ObservableDictionary DefaultViewModel = new ObservableDictionary();
        public AddCost_ViewModel AddCostViewModel = new AddCost_ViewModel();
        double previousWidth;

        public AddCost()
        {
            double temporaryPageWidth = this.ActualWidth;
            previousWidth = temporaryPageWidth;
            AddCostViewModel.UpdateKeyboardButtonSize((int)Math.Floor(temporaryPageWidth));

            this.InitializeComponent();
            this.SizeChanged += AddCost_SizeChanged;
            this.NotificationsButton.Tapped += AnimateNotifications;

            AddCostViewModel.Translated += (s, e) =>
            {
                AnimateBlockTranslation.Begin();
            };

            AddCostViewModel.Sized += (s, e) =>
            {
                AnimateSizeTranslation.Begin();
            };

            AddCostViewModel.FormEnebled += (s, e) =>
            {
                if (AddCostViewModel.IsFormEnebled)
                {
                    AnimateFormEnebled.Begin();
                }
                else
                {
                    AnimateFormDisabled.Begin();
                }
            };

            AddCostViewModel.NotificationRaised += (s, e) =>
            {
                if (AddCostViewModel.IsNotificationRaised)
                {
                    AnimateNotificationRaised.Begin();
                }
                else
                {
                    AnimateNotificationCleared.Begin();
                    AnimateRotationCCW.Begin();
                }
            };
        }

        private void AnimateNotifications(object sender, TappedRoutedEventArgs e)
        {
            //Grid parentGrid = (sender as FrameworkElement).Parent as Grid;
            double actualHeight = NotificationsGrid.ActualHeight;

            if (AddCostViewModel.IsNotificationShown == false)
            {
                if (AddCostViewModel.IsNotificationRaised)
                {
                    AnimateRotationCW.Begin();
                }
                else
                {
                    AnimateRotation.Begin();
                }

                AddCostViewModel.IsNotificationShown = true;
                AddCostViewModel.NotificationsBlockTranslation(actualHeight);
            }
            else
            {
                AnimateRotationCCW.Begin();
                AddCostViewModel.IsNotificationShown = false;
                AddCostViewModel.NotificationsBlockTranslation(actualHeight);
            }
        }

        private void AddCost_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            if (this.ActualWidth <=401)
            {
                inputCostBox.FontSize = 65;
                currencyBox.FontSize = 65;
            }
            
            AddCostViewModel.UpdateKeyboardButtonSize((int)Math.Floor(senderElement.ActualWidth));

        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            CategoryWithStats choosenCategory = (CategoryWithStats)e.Parameter;
            AddCostViewModel.CostTemplate = new Cost(choosenCategory.GroupKey, DateTime.Now.ToString(), 0.0m, choosenCategory.ImagePath);      
            this.DataContext = AddCostViewModel;
            AddCostViewModel.FormCompleted += MainViewModel_FormCompleted;
            AddCostViewModel.UpdateKeyboardButtonSize((int)Math.Floor(this.ActualWidth));
            AddCostViewModel.PopulateCalcGrid(CalculatorGrid);
        }

        private void MainViewModel_FormCompleted(object sender, EventArgs e)
        {
            ReturnHomePage();
        }

        private void ReturnHomePage()
        {
            App.DataModel.AddCost(AddCostViewModel.ReturnEditedCost());
            App.DataModel.SavaDataBase();
            Services.Navigation.GotoMainPage();
        }

        private void NotificationsGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AddCostViewModel.NotificationsBlockSizeTranslation((sender as Grid).ActualHeight);
        }

        private void NotificationsListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AddCostViewModel.NotificationsBlockSizeTranslation((sender as ListView).ActualHeight);
        }
    }
}
