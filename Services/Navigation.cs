using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace CostDaily.Services
{
    public static class Navigation
    {

        private static TransitionCollection _transitions {get; set;}

        private static Frame _frame
        {
            get
            {
                Frame rootFrame = Window.Current.Content as Frame;

                if (rootFrame == null)
                {
                    rootFrame = new Frame();

                    rootFrame.CacheSize = 1;

                    Window.Current.Content = rootFrame;

                }

                if (rootFrame.Content == null)
                {
                    if (rootFrame.ContentTransitions != null)
                    {
                        if (_transitions == null)
                        {
                            _transitions = new TransitionCollection();
                        }
                        else
                        {
                            _transitions.Clear();
                        }
                        
                        foreach (var c in rootFrame.ContentTransitions)
                        {
                            _transitions.Add(c);
                        }
                    }

                    rootFrame.ContentTransitions = null;
                    rootFrame.Navigated += RootFrame_FirstNavigated;
                }

                return rootFrame;
            }
        }

        public static bool GoBack()
        {
            string thisPage = _frame.CurrentSourcePageType.Name;

            if (_frame.CanGoBack && (!(thisPage.Contains("MainPage"))))
            {
                _frame.GoBack();
                return true;
            }
            else
            {
                _frame.BackStack.Clear();
                return false;
            }
        }

        public static bool GotoMainPage()
        {
            return _frame.Navigate(typeof(MainPage));
        }

        public static bool GotoAddCostPage(object parameter)
        {
            return _frame.Navigate(typeof(AddCost), parameter);
        }

        public static bool GotoSettingsPage()
        {
            return _frame.Navigate(typeof(Settings));
        }

        private static void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            _frame.ContentTransitions = _transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            _frame.Navigated -= RootFrame_FirstNavigated;
        }
    }
}
