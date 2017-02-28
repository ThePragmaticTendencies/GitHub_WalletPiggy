using CostDaily.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace CostDaily
{
    public sealed partial class Splash_Extended : Page
    {
        private bool _menuOn = false;
        private bool _dataLoaded = false;
        private bool _menuDisabled = false;
        private SplashExtended_ViewModel ViewModel = new SplashExtended_ViewModel();
        private bool loadBackUp = false;
        private int _splashDelay = 1000;

        public Splash_Extended()
        {
            this.InitializeComponent();
            this.DataContext = ViewModel;

            SplashExtendedImage.ImageOpened += async (s, e) =>
            {
                Window.Current.Activate();
                await Task.Delay(_splashDelay);
                if (!_menuOn && !loadBackUp)
                {
                    _backUpButtonSlideOutBegin();
                }
            };

            BackUpButtonSlideOut.Completed += async (s, e) =>
            {
                await ViewModel.LoadAppData(loadBackUp);
            };

            BackUpLoadBorder.Tapped += BackUpLoadBorder_Tapped;

            OKButton.Tapped += (s, e) =>
            {
                loadBackUp = true;

                MenuOkButtonAnimation.Begin();

                ViewModel.BackupLoadProgressColor();
                BackUpOKbutton.Tapped += BackUpButton_Tapped;
                BackUpNOOKbutton.Tapped += BackUpButton_Tapped;
                BackUpTERMINATEbutton.Tapped += BackUpButton_Tapped;

                MenuOkButtonAnimation.Completed += (sender, eArgs) =>
                  {
                      ExitMenu();
                  };
            };

            NOOKButton.Tapped += (s, e) =>
            {
                MenuNookButtonAnimation.Begin();
                MenuNookButtonAnimation.Completed += (sender, eArgs) =>
                  {
                      ExitMenu();
                  };
            };

            ViewModel.NotificationPop += (s, e) =>
            {
                NotificationFadeInAnimation.Begin();
                ProgressControl.Opacity = 0;
                if (_menuOn)
                {
                    ViewModel.SwitchOnMenu();
                    MenuButtonsFadeInAnimation.Begin();
                }
                else
                {
                    ViewModel.SwitchOffMenu();
                    ActionButtonsFadeInAnimation.Begin();
                }
            };

            App.DataModel.DataLoaded += OnDataLoaded;
        }

        private void BackUpLoadBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (_menuDisabled)
                return;

            _menuOn = true;
            BackUpButtonClickAnimation.Begin();
            BackUpButtonClickAnimation.Completed += (s, eArgs) =>
            {
                ViewModel.DisplayNewMessage("DoYouWantBackUp");
            };
        }

        private void OnDataLoaded(object sender, EventArgs e)
        {
            _dataLoaded = true;

            if (!loadBackUp)
            {
                Services.Navigation.GotoMainPage();
            }
        }

        private void BackUpButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Border tappedTextBlock = sender as Border;

            if (tappedTextBlock != null)
            {
                NotificationFadeOutAnimation.Begin();

                switch (tappedTextBlock.Name)
                {
                    case "BackUpTERMINATEbutton":
                        BackUpOkButtonAnimation.Begin();
                        BackUpOkButtonAnimation.Completed += (s, eArgs) =>
                        {
                            Application.Current.Exit();
                        };
                        break;

                    case "BackUpOKbutton":
                        BackUpOkButtonAnimation.Begin();
                        BackUpOkButtonAnimation.Completed += (s, eArgs) =>
                        {
                            if (_dataLoaded == true) Services.Navigation.GotoMainPage();
                        };
                        break;

                    case "BackUpNOOKbutton":
                        BackUpNookButtonAnimation.Begin();
                        BackUpNookButtonAnimation.Completed += (s, eArgs) =>
                        {
                            if (_dataLoaded == true) Services.Navigation.GotoMainPage();
                        };
                        break;
                    default:
                        if (_dataLoaded == true) Services.Navigation.GotoMainPage();
                        break;
                }

            }

        }

        private void _backUpButtonSlideOutBegin()
        {
            _menuDisabled = true;
            BackUpLoadBorder.Tapped -= BackUpLoadBorder_Tapped;
            BackUpButtonSlideOut.Begin();
        }

        private void ExitMenu()
        {
            _menuOn = false;
            NotificationFadeOutAnimation.Begin();
            MenuButtonsFadeOutAnimation.Begin();
            MenuButtonsFadeOutAnimation.Completed += (s, e) =>
            {
                hideMenuButtons();
            };
            ProgressControl.Opacity = 1;
            _backUpButtonSlideOutBegin();
        }

        private void hideMenuButtons()
        {
            OKButton.Visibility = Visibility.Collapsed;
            NOOKButton.Visibility = Visibility.Collapsed;
        }

        private void showMenuButtons()
        {
            OKButton.Visibility = Visibility.Visible;
            NOOKButton.Visibility = Visibility.Visible;
        }
    }
}
