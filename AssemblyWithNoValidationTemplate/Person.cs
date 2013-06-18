using System.ComponentModel;
using Validar;

[InjectValidation]
public class Person : INotifyPropertyChanged
{

	public string GivenNames;
	public string FamilyName;

    public event PropertyChangedEventHandler PropertyChanged;
}