using System.ComponentModel;
using System.Runtime.CompilerServices;
using Validar;

namespace WithGenericExternal
{
    [InjectValidation]
    public sealed class MyModel : INotifyPropertyChanged
    {
        string property2;
        string property1;

        public string Property1
        {
            get { return property1; }
            set
            {
                property1 = value;
                OnPropertyChanged();
            }
        }

        public string Property2
        {
            get { return property2; }
            set
            {
                property2 = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}