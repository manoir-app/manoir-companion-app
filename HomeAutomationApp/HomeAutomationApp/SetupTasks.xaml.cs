using HomeAutomationApp.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HomeAutomationApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SetupTasks : ContentPage
    {
        public ObservableCollection<PlatformSetupTask> Items { get; set; }

        public SetupTasks()
        {
            InitializeComponent();
            CollectionView.ItemsSource = (App.Current as App).PlatformData.SetupTasks;

        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var btn = sender as Button;
            var tmp = btn.BindingContext as Model.PlatformSetupTask;
            if(tmp!=null && !tmp.Done)
            {
                (App.Current as App).DoStartupTask(tmp.Code);
            }
        }
    }
}
