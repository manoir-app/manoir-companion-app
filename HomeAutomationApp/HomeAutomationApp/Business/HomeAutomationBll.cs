using Home.Common.Model;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Home.Common.Messages;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HomeAutomationApp.Business
{
    public class SceneGroupDetails : SceneGroup
    {
        public SceneGroupDetails() : base()
        {
            Scenes = new List<SceneWithState>();
        }

        public List<SceneWithState> Scenes { get; set; }
    }
    public class SceneWithState : Scene, INotifyPropertyChanged
    {
        private bool _isActive = false;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged("IsActive");
                }
            }
        }

        private bool _isActivating = false;
        public bool IsActivating
        {
            get { return _isActivating; }
            set
            {
                if (_isActivating != value)
                {
                    _isActivating = value;
                    OnPropertyChanged("IsActivating");
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new
                PropertyChangedEventArgs(propertyName));
        }
    }

    public class DeviceWithDetails : Device
    {

    }

    public class HomeAutomationBll : BaseBll
    {
        public async Task<List<SceneGroupDetails>> GetGroupsAndScenes()
        {
            var url = $"/v1.0/homeautomation/scenes/groups";
            var grps = await DownloadData<List<SceneGroupDetails>>(url);

            url = $"/v1.0/homeautomation/scenes/scenes";
            var scs = await DownloadData<List<SceneWithState>>(url);

            grps.Sort((a, b) => a.Order.GetValueOrDefault().CompareTo(b.Order.GetValueOrDefault()));

            foreach (var grp in grps)
            {
                grp.Scenes = (from z in scs
                              where z.GroupId.Equals(grp.Id, StringComparison.InvariantCultureIgnoreCase)
                              orderby z.OrderInGroup.GetValueOrDefault()
                              select z).ToList();
                foreach (var sc in grp.Scenes)
                {
                    if (grp.CurrentActiveScenes != null && grp.CurrentActiveScenes.Contains(sc.Id))
                        sc.IsActive = true;
                }
            }

            return grps;
        }

        public async Task<bool> ActivateScene(string sceneId)
        {
            string url = $"/v1.0/agents/all/send/homeautomation.scenario.execute";
            var scs = await UploadData<bool, ExecuteScenarioHomeAutomationMessage>(url,
                new ExecuteScenarioHomeAutomationMessage()
                {
                    SceneId = sceneId
                }, "POST");

            await UserInfosBll.NotifyUserActivity("homeautomation");

            return scs;
        }

        public async Task<List<DeviceWithDetails>> GetAllDevices()
        {

            var url = $"/v1.0/devices/find?kind=homeautomation";
            var grps = await DownloadData<List<DeviceWithDetails>>(url);

            return grps;
        }

    }
}
