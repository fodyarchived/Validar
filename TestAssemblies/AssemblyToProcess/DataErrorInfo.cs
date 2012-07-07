using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using FluentValidation;

public class Person 
{
    public string GivenNames { get; set; }
    public string FamilyName { get; set; }

    public string FullName
    {
        get { return string.Format("{0} {1}", GivenNames, FamilyName); }
    }

}

public class PersonViewModelValidator : AbstractValidator<Person>
{
    public PersonViewModelValidator()
    {
        RuleFor(x => x.FamilyName).NotEmpty();
        RuleFor(x => x.GivenNames).NotEmpty();
    }
}
public class PersonValidator : IDataErrorInfo
{
    public Person Target { get; set; }

    public string this[string columnName]
    {
        get
        {
            if (columnName == "FamilyName" && Target.FamilyName == null)
            {
                return "FamilyName is null";
            }
            return null;
        }
    }

    public string Error
    {
        get
        {
            if (Target.FamilyName == null)
            {
                return "FamilyName is null";
            }
            return null;
        }
    }
}