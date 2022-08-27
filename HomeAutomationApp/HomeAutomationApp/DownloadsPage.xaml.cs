using Home.Common.Model;
using HomeAutomationApp.Business;
using System;
using System.Collections.ObjectModel;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeAutomationApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DownloadsPage : ContentPage
    {
        public class DownloadItemInPage
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string MainTag { get; set; }
            public string Resolution { get; set; }
            public string Language { get; set; }
            public bool HasLinkedItem { get; set; }
            public string Pertinence { get; set; }
            public bool IsPertinenceHigh { get; set; }
            public bool IsPertinenceMedium { get; set; }

            public DownloadItem RealItem { get; set; }
        }

        public ObservableCollection<DownloadItemInPage> Items { get; set; }
        public ObservableCollection<DownloadItemInPage> MutatingItems { get; set; }

        public DownloadsPage()
        {
            Items = new ObservableCollection<DownloadItemInPage>();
            MutatingItems = new ObservableCollection<DownloadItemInPage>();
            InitializeComponent();

            MyListView.ItemsSource = Items;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();


            var t = NativeAppHelper.Instance.GetSavedCredentials();


            var tmpIt = await new DownloadBll().GetDownloads("new");
            Items.Clear();
            foreach (var it in tmpIt)
            {
                if (!string.IsNullOrEmpty(it.LinkedDownloadId))
                    continue;

                var lit = new DownloadItemInPage()
                {
                    Id = it.Id,
                    Title = it.Title,
                    RealItem = it,
                    Language = it.Language,
                    Resolution = it.VideoMetadata?.Resolution
                };

                if (lit.Language != null && lit.Language == "*-*")
                    lit.Language = null;

                lit.HasLinkedItem = (from z in tmpIt
                                     where z.LinkedDownloadId != null && z.LinkedDownloadId == it.Id
                                     select z).Count() > 0;

                if (t.UserId != null && it.Pertinences!=null)
                {
                    var pert = (from z in it.Pertinences
                                where z.UserId.Equals(t.UserId, StringComparison.CurrentCultureIgnoreCase)
                                select z).FirstOrDefault();
                    if(pert!=null)
                    {
                        lit.Pertinence = pert.Pertinence.ToString("0.00");
                        if (pert.Pertinence > 1.5M)
                            lit.IsPertinenceHigh = true;
                        else if (pert.Pertinence > 0.98M)
                            lit.IsPertinenceMedium = true;
                    }
                }

                lit.MainTag = it.Tags?.FirstOrDefault();

                Items.Add(lit);
            }
        }

        void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;
        }

        private void SwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e)
        {

        }

        private async void btnDownload_Clicked(object sender, EventArgs e)
        {
            var btn = sender as Button;
            var dn = btn.BindingContext as DownloadItemInPage;

            MutatingItems.Add(dn);
            Items.Remove(dn);

            bool okUpdate = await new DownloadBll().QueueDownload(dn.Id);

            MutatingItems.Remove(dn);
            if (!okUpdate)
                Items.Add(dn);

        }

        private async void btnSuppr_Clicked(object sender, EventArgs e)
        {
            var btn = sender as Button;
            var dn = btn.BindingContext as DownloadItemInPage;

            MutatingItems.Add(dn);
            Items.Remove(dn);

            bool okUpdate = await new DownloadBll().AbandonDownload(dn.Id);

            MutatingItems.Remove(dn);
            if (!okUpdate)
                Items.Add(dn);
        }
    }
}
