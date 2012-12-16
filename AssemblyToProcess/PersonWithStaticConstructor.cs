using System.ComponentModel;

[InjectValidationAttribute]
public class PersonWithStaticConstructor : INotifyPropertyChanged
{
// ReSharper disable EmptyConstructor
    static PersonWithStaticConstructor() { }
// ReSharper restore EmptyConstructor

	public string GivenNames;
	public string FamilyName;
	
    public event PropertyChangedEventHandler PropertyChanged;
}
