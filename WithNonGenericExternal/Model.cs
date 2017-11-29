using System.ComponentModel;
using System.Runtime.CompilerServices;
using Validar;

namespace WithNonGenericExternal
{
    [InjectValidation]
    public class Model : INotifyPropertyChanged
    {
        string property1;
        string property2;

        public string Property1        {
            get => property1;            set
            {
                property1 = value;
                OnPropertyChanged();
            }
        }

        public string Property2        {
            get => property2;            set
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