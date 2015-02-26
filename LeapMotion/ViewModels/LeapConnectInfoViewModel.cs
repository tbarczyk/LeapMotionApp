using Caliburn.Micro;
using LeapMotion.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapMotion.ViewModels
{
    [Export(typeof(LeapConnectInfoViewModel))]
    public class LeapConnectInfoViewModel
    {
        private readonly IEventAggregator _events;

        [ImportingConstructor]
        public LeapConnectInfoViewModel(IEventAggregator events)
        {
            _events = events;
        }
    }
}
