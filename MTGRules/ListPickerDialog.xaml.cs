using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace MTGRules {
    public sealed partial class ListPickerDialog : ContentDialog {


        public static async Task<int> ShowAsync(string title, object listSource, int defaultItem = 0) {
            ListPickerDialog dialog = new ListPickerDialog(title, listSource, defaultItem);
            await dialog.ShowAsync();
            return dialog.SelectedItem;
        }


        public int SelectedItem = -1;

        public ListPickerDialog(string title, object listSource, int defaultItem = 0) {
            this.InitializeComponent();

            this.Title = title;
            
            list.ItemsSource = listSource;

            if(defaultItem < 0)
                defaultItem = 0;
            if(list.Items.Count > 0 && defaultItem < list.Items.Count) {
                list.SelectedIndex = defaultItem;
                SelectedItem = defaultItem;
            }

            PrimaryButtonText = ResourceLoader.GetForCurrentView().GetString("/ListPickerDialog/primaryButtonText");
            SecondaryButtonText = ResourceLoader.GetForCurrentView().GetString("/ListPickerDialog/secondaryButtonText");
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
            SelectedItem = list.SelectedIndex;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
            SelectedItem = -1;
        }
    }
}
