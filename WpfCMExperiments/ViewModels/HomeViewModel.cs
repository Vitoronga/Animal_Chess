using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfCMExperiments.ViewModels
{
    public class HomeViewModel : Screen
    {
		private bool stopTimeUpdater = false;

		public DateTime CurrentTime
		{
			get { return DateTime.Now.ToLocalTime(); }
		}

		public HomeViewModel()
		{
			//Thread timeUpdater = new Thread(TimeUpdater);
			//timeUpdater.Start();
		}

		private void TimeUpdater()
		{
			while (!stopTimeUpdater) { 
				NotifyOfPropertyChange(() => CurrentTime);
				Thread.Sleep(950);
            }
        }
	}
}
