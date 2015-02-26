using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using Caliburn.Micro;
using LeapMotion.Navigation;
using System.Windows.Media;
using System.Windows.Threading;

namespace LeapMotion.ViewModels
{
    [Export(typeof(ColorViewModel))]
    public class ColorViewModel : PropertyChangedBase, IHandle<String>
    {
        private readonly IEventAggregator _events;
        private string _text;

       
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                NotifyOfPropertyChange(() => Text);
            }
        }
        [ImportingConstructor]
        public ColorViewModel(IEventAggregator events)
        {
            _events = events;
            _events.Subscribe(this);
        }

        public void Handle(string message)
        {
            Text = message;
        }

        public void Red()
        {
            _events.Publish(new ColorEvent(new SolidColorBrush(Colors.Red)));
        }

        public void Green()
        {
            _events.Publish(new ColorEvent(new SolidColorBrush(Colors.Green)));
        }

        public void Blue()
        {
            _events.Publish(new ColorEvent(new SolidColorBrush(Colors.Blue)));
        }
    }
}
