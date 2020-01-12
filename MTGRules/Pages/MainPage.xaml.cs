using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Store;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using MTGRules.Pages;
using Microsoft.Services.Store.Engagement;

namespace MTGRules {

    public sealed partial class MainPage
    {
        private class ProgressRingViewer : IDisposable {
            public static MainPage page;

            private static int counter = 0;

            public static ProgressRingViewer Show() {
                return new ProgressRingViewer();
            }

            private ProgressRingViewer() {
                if (counter++ == 0) {
                    page.contentGrid.Visibility = Visibility.Collapsed;
                    page.progressRing.IsActive = true;
                }
            }

            public void Dispose()
            {
                counter -= 1;
                if (counter < 0)
                    counter = 0;
                if (counter == 0) {
                    page.progressRing.IsActive = false;
                    page.contentGrid.Visibility = Visibility.Visible;
                }
            }
        }

        private static readonly StoreServicesCustomEventLogger CustomEventlogger = StoreServicesCustomEventLogger.GetDefault();

        public static MainPage ActualInstance; // Temporal
        
        static readonly List<RulesSource> rulesSources = new List<RulesSource> {
            new RulesSource("MagicCompRules_20150123.txt",
                            new Uri("http://media.wizards.com/2015/docs/MagicCompRules_20150123.txt"),
                            new DateTime(2015, 1, 23),
                            Encoding.GetEncoding("windows-1252")),

            new RulesSource("MagicCompRules_20160722.txt",
                            new Uri("http://media.wizards.com/2016/docs/MagicCompRules_20160722.txt"),
                            new DateTime(2016, 7, 22),
                            Encoding.GetEncoding("windows-1252")),

            new RulesSource("MagicCompRules_CN2_Update_20160826.txt",
                            new Uri("http://media.wizards.com/2016/docs/MagicCompRules_CN2_Update_20160826.txt"),
                            new DateTime(2016, 8, 26),
                            Encoding.GetEncoding("UTF-16")),

            new RulesSource("MagicCompRules_20160930.txt",
                            new Uri("http://media.wizards.com/2016/docs/MagicCompRules_20160930.txt"),
                            new DateTime(2016, 9, 30),
                            Encoding.GetEncoding("UTF-16")),

            new RulesSource("MagicCompRules_20161111.txt",
                            new Uri("http://media.wizards.com/2016/docs/MagicCompRules_20161111.txt"),
                            new DateTime(2016, 11, 11),
                            Encoding.GetEncoding("windows-1252")),

            new RulesSource("MagicCompRules_20170119.txt",
                            new Uri("http://media.wizards.com/2017/downloads/MagicCompRules_20170119.txt"),
                            new DateTime(2017, 1, 19),
                            Encoding.GetEncoding("windows-1252")),

            new RulesSource("MagicCompRules_20170428.txt",
                            new Uri("http://media.wizards.com/2017/downloads/MagicCompRules_20170428.txt"),
                            new DateTime(2017, 4, 28),
                            Encoding.GetEncoding("windows-1252")),

            new RulesSource("MagicCompRules_20170605.txt",
                            new Uri("http://media.wizards.com/2017/downloads/MagicCompRules_20170605.txt"),
                            new DateTime(2017, 6, 5),
                            Encoding.GetEncoding("windows-1252")),

            new RulesSource("MagicCompRules_20170707.txt",
                            new Uri("http://media.wizards.com/2017/downloads/MagicCompRules_20170707.txt"),
                            new DateTime(2017, 7, 7),
                            Encoding.GetEncoding("windows-1252")),

            new RulesSource("MagicCompRules 20170825.txt",
                            new Uri("http://media.wizards.com/2017/downloads/MagicCompRules%2020170825.txt"),
                            new DateTime(2017, 8, 25),
                            Encoding.GetEncoding("windows-1252")),

            new RulesSource("MagicCompRules 20170925.txt",
                            new Uri("http://media.wizards.com/2017/downloads/MagicCompRules%2020170925.txt"),
                            new DateTime(2017, 9, 25),
                            Encoding.GetEncoding("windows-1252")),

            new RulesSource("MagicCompRules%2020180119.txt",
                            new Uri("http://media.wizards.com/2018/downloads/MagicCompRules%2020180119.txt"),
                            new DateTime(2018, 1, 19),
                            Encoding.GetEncoding("windows-1252")),

            new RulesSource("MagicCompRules%2020180413.txt",
                            new Uri("http://media.wizards.com/2018/downloads/MagicCompRules%2020180413.txt"),
                            new DateTime(2018, 4, 13),
                            Encoding.GetEncoding("windows-1252")),

            new RulesSource("MagicCompRules%2020180608.txt",
                            new Uri("http://media.wizards.com/2018/downloads/MagicCompRules%2020180608.txt"),
                            new DateTime(2018, 6, 8),
                            Encoding.GetEncoding("windows-1252")),

            new RulesSource("MagicCompRules%2020180713.txt",
                            new Uri("http://media.wizards.com/2018/downloads/MagicCompRules%2020180713.txt"),
                            new DateTime(2018, 7, 13),
                            Encoding.GetEncoding("windows-1252")),

            new RulesSource("MagicCompRules%2020180810.txt",
                            new Uri("http://media.wizards.com/2018/downloads/MagicCompRules%2020180810.txt"),
                            new DateTime(2018, 8, 10),
                            Encoding.GetEncoding("windows-1252")),

            new RulesSource("MagicCompRules%2020181005.txt",
                            new Uri("http://media.wizards.com/2018/downloads/MagicCompRules%2020181005.txt"),
                            new DateTime(2018, 10, 5),
                            Encoding.GetEncoding("windows-1252")),

            new RulesSource("MagicCompRules%2020190125.txt",
                            new Uri("http://media.wizards.com/2019/downloads/MagicCompRules%2020190125.txt"),
                            new DateTime(2019, 1, 25),
                            Encoding.GetEncoding("windows-1252")),

            new RulesSource("MagicCompRules%2020190712.txt",
                            new Uri("https://media.wizards.com/2019/downloads/MagicCompRules%2020190712.txt"),
                            new DateTime(2019, 7, 12),
                            Encoding.GetEncoding("UTF-8")),

            new RulesSource("MagicCompRules%2020190823.txt",
                            new Uri("https://media.wizards.com/2019/downloads/MagicCompRules%2020190823.txt"),
                            new DateTime(2019, 8, 23),
                            Encoding.GetEncoding("UTF-8")),

            new RulesSource("MagicCompRules%2020191004.txt",
                            new Uri("https://media.wizards.com/2019/downloads/MagicCompRules%2020191004.txt"),
                            new DateTime(2019, 10, 04),
                            Encoding.GetEncoding("UTF-8"))
        };

        private List<Rule> rules;

        private List<HistoryItem> history = new List<HistoryItem>();
        HistoryItem actualSearch = new HistoryItem(HistoryType.Number, 0);

        bool useLightTheme;
        int actualRules;

        MediaElement mediaplayer = new MediaElement();

        public MainPage() {
            InitializeComponent();
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            useLightTheme = (bool)localSettings.Values["useLightTheme"];
            if(useLightTheme) {
                changeThemeButton.Label = ResourceLoader.GetForCurrentView().GetString("useDarkTheme");
            } else {
                changeThemeButton.Label = ResourceLoader.GetForCurrentView().GetString("useLightTheme");
            }

            ActualInstance = this;
        }

        private async void onLoaded(object sender, RoutedEventArgs args) {
            ProgressRingViewer.page = this;
            if(rules == null) {
                using (ProgressRingViewer.Show()) {
                    if (!await loadDataAsync(rulesSources.Count - 1)) {
                        MessageDialog dialog =
                            new MessageDialog(ResourceLoader.GetForCurrentView().GetString("errorLoadingLastRules"));
                        await dialog.ShowAsync();
                    } else {
                        showByNumber(0, false);
                        homeButton.IsEnabled = false;

                        SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                            AppViewBackButtonVisibility.Collapsed;
                        SystemNavigationManager.GetForCurrentView().BackRequested += onBackButtonRequested;
                    }
                }
            }
        }

        public void onHyperlinkRuleClick(string text) {
            showByText(text);
        }

        private void onListItemRightTapped(object sender, RightTappedRoutedEventArgs args) {
            Rule text = (Rule)((Grid)sender).DataContext;
            showMenuFlyout(text, (FrameworkElement)sender);
        }

        private void onListItemHolding(object sender, HoldingRoutedEventArgs args) {
            Rule text = (Rule)((Grid)sender).DataContext;
            showMenuFlyout(text, (FrameworkElement)sender);
        }

        private void onRandomRuleButtonClick(object sender, RoutedEventArgs e) {
            showRandomRule();
        }

        private async void onDonateButtonClick(object sender, RoutedEventArgs args){
            using (ProgressRingViewer.Show()) {
                MessageDialog dialog = new MessageDialog("");
                try {
                    ListingInformation info = await CurrentApp.LoadListingInformationAsync();
                    ProductListing donation;
                    if (info.ProductListings.TryGetValue("donation", out donation)) {
                        if (await fulfillDonation()) {
                            PurchaseResults purchaseResult =
                                await CurrentApp.RequestProductPurchaseAsync(donation.ProductId);
                            if (purchaseResult.Status == ProductPurchaseStatus.Succeeded) {
                                dialog.Content = ResourceLoader.GetForCurrentView().GetString("thanksForDonation") +
                                                 "\n" + ResourceLoader.GetForCurrentView().GetString("youCanRate");
                            } else if (purchaseResult.Status == ProductPurchaseStatus.NotPurchased) {
                                dialog.Content =
                                    ResourceLoader.GetForCurrentView().GetString("anywayThanksForUsingApp") +
                                    "\n" + ResourceLoader.GetForCurrentView().GetString("youCanRate");
                            } else {
                                dialog.Content = ResourceLoader.GetForCurrentView().GetString("errorOcurred") +
                                                 "\n" +
                                                 ResourceLoader.GetForCurrentView()
                                                     .GetString("thanksAnywayTryAgainOrRate");
                            }
                        } else {
                            dialog.Content =
                                ResourceLoader.GetForCurrentView().GetString("errorOcurredDonationPendingOrServerError") +
                                "\n" + ResourceLoader.GetForCurrentView().GetString("thanksAnywayTryAgainOrRate");
                        }
                    } else {
                        dialog.Content = ResourceLoader.GetForCurrentView().GetString("noActiveDonations") +
                                         "\n" + ResourceLoader.GetForCurrentView().GetString("youCanRate");
                    }
                } catch {
                    dialog.Content = ResourceLoader.GetForCurrentView().GetString("errorOcurred") +
                                     "\n" + ResourceLoader.GetForCurrentView().GetString("thanksAnywayTryAgainOrRate");
                }

                await dialog.ShowAsync();
            }
        }

        private async Task<bool> fulfillDonation() {
            var unfulfilledConsumables = await CurrentApp.GetUnfulfilledConsumablesAsync();
            foreach( UnfulfilledConsumable consumable in unfulfilledConsumables) {
                if(consumable.ProductId == "donation") {
                    FulfillmentResult fulfillmentResult = await CurrentApp.ReportConsumableFulfillmentAsync(consumable.ProductId, consumable.TransactionId);
                    switch(fulfillmentResult) {
                        case FulfillmentResult.Succeeded:
                        case FulfillmentResult.NothingToFulfill:
                        case FulfillmentResult.PurchaseReverted:
                            return true;
                    }
                    return false;
                }
            }
            return true;
        }

        private async void onChangeThemeButtonClick(object sender, RoutedEventArgs args) {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

            useLightTheme = !useLightTheme;
            localSettings.Values["useLightTheme"] = useLightTheme;

            if(useLightTheme) {
                changeThemeButton.Label = ResourceLoader.GetForCurrentView().GetString("useDarkTheme");
            } else {
                changeThemeButton.Label = ResourceLoader.GetForCurrentView().GetString("useLightTheme");
            }

            MessageDialog dialog = new MessageDialog(ResourceLoader.GetForCurrentView().GetString("restartToApply"));
            await dialog.ShowAsync();
        }

        private void onBackButtonRequested(object sender, BackRequestedEventArgs args) {
            if(history.Count >= 1) {
                HistoryItem item = history.Last();
                switch(item.Type) {
                    case HistoryType.Search:
                        search((string)item.Value, false);
                        break;
                    case HistoryType.Number:
                        showByNumber((int)item.Value, false);
                        break;
                    case HistoryType.Text:
                        showByText((string)item.Value, false);
                        break;
                    case HistoryType.Random:
                        showRandomRule((int?)item.Value, false);
                        break;
                }

                list.UpdateLayout();
                ScrollViewer scrollViewer = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(list, 0), 0) as ScrollViewer;
                scrollViewer?.ChangeView(null, item.VerticalOffset, null, true);

                history.RemoveAt(history.Count() - 1);
                actualSearch = item;
                args.Handled = true;
                if(history.Count == 0)
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
        }

        private void onListItemClick(object sender, ItemClickEventArgs args) {
            Rule rule = (Rule)args.ClickedItem;
            if(rule.Title.Length > 0) {
                if(rule.Title[0] >= '1' && rule.Title[0] <= '9') {
                    int n = rule.Title.IndexOf('.');
                    if(n >= 0 && rule.Title.Length-1 == n) {
                        if(int.TryParse(rule.Title.Substring(0, n), out n)) {
                            showByNumber((short)n);
                        }
                    }
                }else if(rule.Title == "Glosary") {
                    showByNumber(10);
                }
            }
        }

        private void onTextBoxKeyDown(object sender, KeyRoutedEventArgs args) {
            if(args.Key == VirtualKey.Enter &&
               args.KeyStatus.RepeatCount == 1) {
                Focus(FocusState.Programmatic);
                search(searchTextBox.Text);
            }
        }

        private async void onClearCacheButtonClick(object sender, RoutedEventArgs args) {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            foreach(RulesSource source in rulesSources) {
                try {
                    if(await localFolder.TryGetItemAsync(source.FileName) != null) {
                        StorageFile file = await localFolder.GetFileAsync(source.FileName);
                        await file.DeleteAsync();
                    }
                } catch {
                    // ignored
                }
            }
            clearCacheButton.IsEnabled = false;
        }

        private async void onChangeRulesButtonClick(object sender, RoutedEventArgs args) {
            using (ProgressRingViewer.Show()) {
                int n = await showRulesListPicker(ResourceLoader.GetForCurrentView().GetString("selectTheRules"));

                if (n >= 0 && n < rulesSources.Count) {
                    if (await loadDataAsync(n)) {
                        showByNumber(0, false);
                        history.Clear();
                    } else {
                        MessageDialog dialog =
                            new MessageDialog(ResourceLoader.GetForCurrentView().GetString("errorLoadingRules"));
                        await dialog.ShowAsync();
                    }
                }
            }
        }

        private void onAboutButtonClick(object sender, RoutedEventArgs args) {
            Frame.Navigate(typeof(AboutPage));
        }
        
        private async void onExperimentalButtonClick(object sender, RoutedEventArgs args) {
            using (ProgressRingViewer.Show()) {
                int n1 = await showRulesListPicker(ResourceLoader.GetForCurrentView().GetString("selectOldRules"));
                if (n1 < 0 || n1 >= rulesSources.Count)
                    return;

                int n2 = await showRulesListPicker(ResourceLoader.GetForCurrentView().GetString("selectNewRules"));
                if (n2 < 0 || n2 >= rulesSources.Count)
                    return;

                List<Rule> from = await Rule.getRules(rulesSources[n1]),
                           to = await Rule.getRules(rulesSources[n2]);
                if (from == null || to == null) {
                    MessageDialog dialog =
                        new MessageDialog(ResourceLoader.GetForCurrentView().GetString("errorLoadingRules"));
                    await dialog.ShowAsync();
                    return;
                }
                clearCacheButton.IsEnabled = true;

                List<Rule> li = new List<Rule>();

                Rule rule;

                foreach (Rule r1 in from) {
                    rule = compare(r1, findRule(to, r1.Title));
                    if (rule != null)
                        li.Add(rule);
                    foreach (Rule r2 in r1.SubRules) {
                        rule = compare(r2, findRule(to, r2.Title));
                        if (rule != null)
                            li.Add(rule);
                        foreach (Rule r3 in r2.SubRules) {
                            rule = compare(r3, findRule(to, r3.Title));
                            if (rule != null)
                                li.Add(rule);
                        }
                    }
                }

                foreach (Rule r1 in to) {
                    rule = findRule(from, r1.Title);
                    if (rule == null)
                        li.Add(new Rule("(+) " + r1.Title, r1.Text));
                    foreach (Rule r2 in r1.SubRules) {
                        rule = findRule(from, r2.Title);
                        if (rule == null)
                            li.Add(new Rule("(+) " + r2.Title, r2.Text));
                        foreach (Rule r3 in r2.SubRules) {
                            rule = findRule(from, r3.Title);
                            if (rule == null)
                                li.Add(new Rule("(+) " + r3.Title, r3.Text));
                        }
                    }
                }

                list.ItemsSource = li;

                CustomEventlogger.Log("CompareRules");
            }
        }

        private async Task<int> showRulesListPicker(string title)
        {
            List<string> li = new List<string>();

            for (int i = rulesSources.Count - 1; i >= 0; i--)
            {
                li.Add(rulesSources[i].Date.ToString("d") +
                       (i == rulesSources.Count - 1
                           ? " (" + ResourceLoader.GetForCurrentView().GetString("newest") + ")"
                           : ""));
            }

            int n = await ListPickerDialog.ShowAsync(title, li, rulesSources.Count - actualRules - 1);

            return n >= 0 && n < rulesSources.Count
                    ? rulesSources.Count - n - 1
                    : -1;
        }

        private static Rule compare(Rule from, Rule to) {
            if(to == null)
                return new Rule("(-) " + from.Title, from.Text);
            if(from == null)
                return new Rule("(+) " + to.Title, to.Text);
            if(from.Title != to.Title)
                return null;
            if(from.Text != to.Text)
                return new Rule("(M) " + from.Title, from.Text + "\n\n " +
                                                     ResourceLoader.GetForCurrentView().GetString("compareChangedTo") +
                                                     " \n\n" + to.Text);
            return null;
        }

        private static Rule findRule(List<Rule> rules, string title) {
            foreach(Rule r1 in rules) {
                if(r1.Title == title)
                    return r1;
                foreach(Rule r2 in r1.SubRules) {
                    if(r2.Title == title)
                        return r2;
                    foreach(Rule r3 in r2.SubRules) {
                        if(r3.Title == title)
                            return r1;
                    }
                }
            }
            return null;
        }

        private void onHomeButtonClick(object sender, RoutedEventArgs args) {
            showByNumber();
        }

        private void onSearchButtonClick(object sender, RoutedEventArgs args) {
            Focus(FocusState.Programmatic);
            search(searchTextBox.Text);
        }

        private async void speechText(string text) {
            using(var speech = new SpeechSynthesizer()) {
                speech.Voice = (SpeechSynthesizer.DefaultVoice.Language.StartsWith("en") ?
                                SpeechSynthesizer.DefaultVoice
                                : SpeechSynthesizer.AllVoices.FirstOrDefault(
                                    voice => voice.Language.StartsWith("en")
                                ));
                SpeechSynthesisStream stream = await speech.SynthesizeTextToStreamAsync(text);
                mediaplayer.Stop();
                mediaplayer.SetSource(stream, stream.ContentType);
                mediaplayer.Play();
            }

            CustomEventlogger.Log("TextToSpeech");
        }

        private void onClipboardFlyoutItemClick(object sender, RoutedEventArgs args) {
            TextBlock textBlock = (TextBlock)((MenuFlyout)((MenuFlyoutItem)sender).Parent).Target;
            DataPackage dp = new DataPackage();
            dp.SetText(textBlock.Text);
            Clipboard.SetContent(dp);
        }

        private void onReadTextFlyoutItemClick(object sender, RoutedEventArgs args) {
            TextBlock textBlock = (TextBlock)((MenuFlyout)((MenuFlyoutItem)sender).Parent).Target;
            string text = textBlock.Text;
            if(text.Length > 0) {
                if(text[0] >= '1' && text[0] <= '9')
                    text = text.Substring(text.IndexOf(' ') + 1);
                speechText(text);
            }
        }

        private void showMenuFlyout(Rule text, FrameworkElement elem = null) {
            MenuFlyout menu = new MenuFlyout();
            MenuFlyoutItem item = new MenuFlyoutItem {
                Text = ResourceLoader.GetForCurrentView().GetString("copyToClipboard")
            };
            item.Click += (sender, args) => {
                DataPackage dp = new DataPackage();
                dp.SetText(text.Title + ": " + text.Text);
                Clipboard.SetContent(dp);
            };
            menu.Items.Add(item);

            item = new MenuFlyoutItem {
                Text = ResourceLoader.GetForCurrentView().GetString("readText")
            };
            item.Click += (sender, args) => {
                string txt = text.Text;
                if(txt.Length > 0) {
                    if(txt[0] >= '1' && txt[0] <= '9')
                        txt = txt.Substring(txt.IndexOf(' ') + 1);
                    speechText(txt);
                }
            };
            menu.Items.Add(item);
            menu.ShowAt(elem ?? this);
        }

        private void showRandomRule(int? seed = null, bool addToHistory = true) {
            if(rules == null)
                return;
            if(seed == null)
                seed = DateTime.Now.Millisecond;
            Random random = new Random(seed.Value);
            List<Rule> li = new List<Rule>();
            Rule ru = rules[random.Next(rules.Count)];
            li.Add(ru);
            while(ru.SubRules.Count > 0) {
                ru = ru.SubRules[random.Next(ru.SubRules.Count)];
                li.Add(ru);
            }

            if(addToHistory) {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                ScrollViewer scrollViewer = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(list, 0), 0) as ScrollViewer;
                if (scrollViewer != null)
                    actualSearch.VerticalOffset = scrollViewer.VerticalOffset;
                history.Add(actualSearch);
                actualSearch = new HistoryItem(HistoryType.Random, seed);
            }
            list.ItemsSource = li;

            CustomEventlogger.Log("RandomRule");

            homeButton.IsEnabled = true;
        }

        private void showByText(string text, bool addToHistory = true) {
            int pos = text.IndexOf('.');

            if(pos == -1) {
                showByNumber(int.Parse(text));
            } else {
                List<Rule> source = new List<Rule>();
                Rule mustSee = null;
                foreach(Rule ru in rules) {
                    if(ru.Title.Equals(text.Substring(0, 1) + ".")) {
                        source.Add(ru);
                        foreach(Rule ru2 in ru.SubRules) {
                            if(ru2.Title.Equals(text.Substring(0, pos + 1))) {
                                source.Add(ru2);
                                foreach(Rule ru3 in ru2.SubRules) {
                                    source.Add(ru3);
                                    if(mustSee == null && ru3.Title.StartsWith(text)) {
                                        mustSee = ru3;
                                    }
                                }
                                break;
                            }
                        }
                        break;
                    }
                }

                list.ItemsSource = source;
                if(mustSee != null) {
                    list.ScrollIntoView(mustSee, ScrollIntoViewAlignment.Leading);
                    list.SelectedItem = mustSee;
                    list.UpdateLayout();
                }

                if(addToHistory) {
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                    ScrollViewer scrollViewer = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(list, 0), 0) as ScrollViewer;
                    if (scrollViewer != null)
                        actualSearch.VerticalOffset = scrollViewer.VerticalOffset;
                    history.Add(actualSearch);
                    actualSearch = new HistoryItem(HistoryType.Text, text);
                }
            }
        }

        private void showByNumber(int number = 0, bool addToHistory = true) {
            if(rules == null)
                return;
            List<Rule> source = new List<Rule>();
            if(number == 0) {
                source = rules;
            } else if(number>=1 && number<=9) {
                foreach(var rule in rules) {
                    if(rule.Title.StartsWith(number + ".")) {
                        source.Add(rule);
                        foreach(var rule2 in rule.SubRules) {
                            source.Add(rule2);
                        }
                        break;
                    }
                }
            }else if(number >= 100 && number <= 999) {
                int high = number/100;
                foreach(var rule in rules) {
                    if(rule.Title.StartsWith(high + ".")) {
                        source.Add(rule);
                        foreach(var rule2 in rule.SubRules) {
                            if(rule2.Title.StartsWith(number + ".")) {
                                source.Add(rule2);
                                source.AddRange(rule2.SubRules);
                                break;
                            }
                        }
                        break;
                    }
                }
            }else if(number == 10) {
                source.Add(rules.Last());
                source.AddRange(rules.Last().SubRules);
            }

            if(source.Count > 0) {
                if(addToHistory) {
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                    ScrollViewer scrollViewer = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(list, 0), 0) as ScrollViewer;
                    if (scrollViewer != null)
                        actualSearch.VerticalOffset = scrollViewer.VerticalOffset;
                    history.Add(actualSearch);
                    actualSearch = new HistoryItem(HistoryType.Number, number);
                }
                list.ItemsSource = source;

                homeButton.IsEnabled = number != 0;
            }
        }

        private async void search(string text, bool addToHistory = true) {
            if(rules == null)
                return;
            if(text.Length < 3) {
                var dialog = new MessageDialog("Search term must have at least 3 characters");
                await dialog.ShowAsync();
                return;
            }

            using (ProgressRingViewer.Show()) {

                List<Rule> source = new List<Rule>();

                foreach (var rule in rules) {
                    if (rule.Title.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        rule.Text.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0)
                        source.Add(rule);
                    foreach (var rule2 in rule.SubRules) {
                        if (rule2.Title.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            rule2.Text.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0)
                            source.Add(rule2);
                        source.AddRange(
                            rule2.SubRules.Where(
                                rule3 => rule3.Title.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0
                                      || rule3.Text.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0
                            )
                        );
                    }
                }

                if (addToHistory) {
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                        AppViewBackButtonVisibility.Visible;
                    ScrollViewer scrollViewer =
                        VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(list, 0), 0) as ScrollViewer;
                    if (scrollViewer != null)
                        actualSearch.VerticalOffset = scrollViewer.VerticalOffset;
                    history.Add(actualSearch);
                    actualSearch = new HistoryItem(HistoryType.Search, text);
                }
                list.ItemsSource = source;

                CustomEventlogger.Log("SearchText");

                homeButton.IsEnabled = true;
            }
        }

        private async Task<bool> loadDataAsync(int rulesIndex) {
            RulesSource source = rulesSources[rulesIndex];
            List<Rule> tempRules = await Rule.getRules(source);
            if(tempRules == null) {
                return false;
            }
            clearCacheButton.IsEnabled = true;
            rules = tempRules;
            actualRulesTextBlock.Text = source.Date.ToString("dd/MM/yyyy") +
                                        (rulesIndex == rulesSources.Count - 1 ? " (" + ResourceLoader.GetForCurrentView().GetString("newest") +")" : "");
            actualRules = rulesIndex;
            return true;
        }
    }
}
