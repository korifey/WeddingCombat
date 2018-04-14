using System.ComponentModel;
using System.Runtime.CompilerServices;
using WeddingCombat.Annotations;

namespace WeddingCombat
{
    public class CreditItem : INotifyPropertyChanged
    {
        public string Job { get; }
        public string Name { get;  }
        public string Image { get;  }
        public string Source => $"img/{Image}.png";

        public CreditItem Extra { get; set; }

        
        private double _opacity = 0.0;

        public CreditItem(string job, string name, string image, CreditItem extra = null)
        {
            Job = job;
            Name = name;
            Image = image;
            Extra = extra;
        }

        public double Opacity
        {
            get => _opacity;
            set
            {
                _opacity = value;
                OnPropertyChanged(nameof(Opacity));              
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}