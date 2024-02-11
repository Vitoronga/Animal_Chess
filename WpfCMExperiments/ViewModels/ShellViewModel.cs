using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WpfCMExperiments.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        private bool _helpPanelToggleValue;
        
        public bool HelpPanelToggleValue
        {
            get => _helpPanelToggleValue;
            set
            {
                _helpPanelToggleValue = value;
                NotifyOfPropertyChange("HelpPanelToggleValue");
                NotifyOfPropertyChange("HelpPanelVisibility");
            }
        }
        public Visibility HelpPanelVisibility => HelpPanelToggleValue ? Visibility.Visible : Visibility.Collapsed;


        public ShellViewModel()
        {
            HelpPanelToggleValue = false;

            LoadHome();
        }

        public void LoadHome()
        {
            ActivateItemAsync(IoC.Get<HomeViewModel>(), new CancellationToken());
        }

        public void LoadSinglePlayer()
        {
            ActivateItemAsync(IoC.Get<SinglePlayerViewModel>(), new CancellationToken());
        }

        public void LoadLAN()
        {
            ActivateItemAsync(IoC.Get<LANViewModel>(), new CancellationToken());
        }

        public void LoadOnline()
        {
            //ActivateItemAsync(IoC.Get<OnlineViewModel>(), new CancellationToken());
        }

        public void LoadProfile()
        {
            //ActivateItemAsync(IoC.Get<ProfileViewModel>(), new CancellationToken());
        }

        // Direct Loaders; Notifications (Basically things that are better shown over the actual screen)
        public void ToggleHelp()
        {
            // Doing it directly in the shell (so it doesn't disturb the main content)
            HelpPanelToggleValue = !HelpPanelToggleValue;
        }

        public void ToggleSettings()
        {
            //ActivateItemAsync(IoC.Get<SettingsViewModel>(), new CancellationToken());
        }
    }
}
