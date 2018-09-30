using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MTGRules.Pages {
    public sealed partial class AboutPage : Page {

        public AboutPage() {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += onBackButtonRequested;

            Run versionRun = (Run)this.FindName("versionRun");
            versionRun.Text = GetAppVersion();
        }

        private void onBackButtonRequested(object sender, BackRequestedEventArgs e) {
            e.Handled = true;
        }

        private void onBackButtonClick(object sender, RoutedEventArgs e) {
            this.Frame.GoBack();
        }

        private static string GetAppVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            return string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
        }
    }
}
