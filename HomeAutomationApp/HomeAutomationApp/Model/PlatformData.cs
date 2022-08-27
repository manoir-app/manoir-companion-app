using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace HomeAutomationApp.Model
{
    public class PlatformData
    {
        public string PlatformaName { get; set; }
        public string Version { get; set; }

        public PlatformSetupTask[] SetupTasks { get; set; }
    }

    public class PlatformSetupTask : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new
                PropertyChangedEventArgs(propertyName));
        }

        public string Name { get; set; }

        private bool _done;
        public bool Done
        {
            get { return _done; }
            set
            {
                if (value != _done)
                {
                    _done = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Description { get; set; }
        public string Code { get; set; }
        public ImageSource Image { get; set; }

        public void SetImage(string imageName)
        {
            Image = ImageSource.FromResource("HomeAutomationApp.Images." + imageName, typeof(PlatformData).Assembly);
        }
    }
}
