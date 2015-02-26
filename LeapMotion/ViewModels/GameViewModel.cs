using Caliburn.Micro;
using LeapMotion.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeapMotion.ViewModels
{
    [Export(typeof(GameViewModel))]
    public class GameViewModel : PropertyChangedBase
    {
        private GameModel gameModel;

        public GameViewModel(/*GameModel gameModel*/)
        {
            gameModel = new GameModel();
        }

        public void Start()
        {
            gameModel.StartGame();
        }

        public void Stop()
        {
            gameModel.StopGame();
        }

        private string _text;

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                NotifyOfPropertyChange(() => Text);
            }
        }


    }
}
