using ReactiveUI;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ExCSS;
using GetStartedApp.Models.Enums;
using System.Reactive;

namespace GetStartedApp.ViewModels;

/// <summary>
/// public class ViewModelBase : ReactiveObject
/// </summary>
public class ViewModelBase : ReactiveObject ,INotifyDataErrorInfo
{
 
    public static eLoginMode AppLoginMode { get; set; }


  private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
 
  public bool HasErrors => _errors.Count > 0;
 
  public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

   private List<string> propertyAttributeErrors;

    // we get error from custom attributes
    private List<string> GetAttributeErrors(string propertyName)
    {
        var errors = new List<string>();
        var property = GetType().GetProperty(propertyName);

        if (property == null) return errors;

        // Get the value of the property
        var propertyValue = property.GetValue(this);

        // Simplify retrieval and processing of attributes
        var attributes = property.GetCustomAttributes(typeof(ValidationAttribute), true);

        foreach (ValidationAttribute attribute in attributes)
        {
            var validationResult = attribute.GetValidationResult(propertyValue, new ValidationContext(this));
            if (validationResult != ValidationResult.Success)
            {
                errors.Add(validationResult.ErrorMessage);
            }
        }

        return errors;
    }

    public IEnumerable GetErrors(string propertyName)
  {
      
      
        if (string.IsNullOrEmpty(propertyName) || !_errors.ContainsKey(propertyName))
      {
          return null;
      }
      return _errors[propertyName];
  }
 
  protected void OnErrorsChanged(string propertyName)
  {
      ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
  }
 
     private void GetActuallAttributesErros_And_LoadThem(string propertyName,string errorMessage)
    {
        propertyAttributeErrors = GetAttributeErrors(propertyName);
        
        _errors[propertyName] = propertyAttributeErrors;
    }
     protected void ShowUiError(string propertyName, string errorMessage)
     {
           // Clear existing errors for the property
        
   
           GetActuallAttributesErros_And_LoadThem(propertyName,errorMessage);
           propertyAttributeErrors.Add(errorMessage);
     
           OnErrorsChanged(propertyName);
        
    }



    protected void DeleteUiError(string propertyName, string errorMessage)
    {
        
            GetActuallAttributesErros_And_LoadThem(propertyName, errorMessage);

        if (_errors.ContainsKey(propertyName))
       {
     
          // Remove the specific error message if it exists
           if (propertyAttributeErrors.Contains(errorMessage))
           {
                _errors.Remove(errorMessage);
               
            }

            OnErrorsChanged(propertyName);
        }
    
    }

    protected void DeleteAllUiErrorsProperty(string propertyName)
    {
        // Load the actual attribute errors for the property
        GetActuallAttributesErros_And_LoadThem(propertyName, null); // errorMessage not needed

        // Check if the property has any errors
        if (_errors.ContainsKey(propertyName))
        {
            // Remove all errors for the specified property
            _errors[propertyName].Clear();

            // Optionally remove the property from the dictionary if no errors are left
            _errors.Remove(propertyName);

            // Notify UI that the errors have changed
            OnErrorsChanged(propertyName);
        }
    }


    /////////////////////////////////////
    /// <summary>
    /// this part is for all the ui elements that need time display
    /// </summary>
    /// 

    public ViewModelBase()
    {
        setTheGrayColorOfAllBtns();
        WhenUserPickTime_DisplayTheColorBtnThatIndicate_IfIsDayOrThisWeekOrThisMonth();
    }
    private DateTimeOffset _startDate = new DateTimeOffset(DateTime.Now);

    public DateTimeOffset StartDate
    {
        get => _startDate;
        set => this.RaiseAndSetIfChanged(ref _startDate, value);
    }

    private DateTimeOffset _endDate = new DateTimeOffset(DateTime.Now);
    public DateTimeOffset EndDate
    {
        get => _endDate;
        set => this.RaiseAndSetIfChanged(ref _endDate, value);
    }


    private string _isClickedOrNotColorOfThisDayBtn;
    public string IsClickedOrNotColorOfThisDayBtn
    {
        get => _isClickedOrNotColorOfThisDayBtn;
        set => this.RaiseAndSetIfChanged(ref _isClickedOrNotColorOfThisDayBtn, value);
    }

    private string _isClickedOrNotColorOfThisWeekBtn;
    public string IsClickedOrNotColorOfThisWeekBtn
    {
        get => _isClickedOrNotColorOfThisWeekBtn;
        set => this.RaiseAndSetIfChanged(ref _isClickedOrNotColorOfThisWeekBtn, value);
    }

    private string _isClickedOrNotColorOfThisMonthBtn;
    public string IsClickedOrNotColorOfThisMonthBtn
    {
        get => _isClickedOrNotColorOfThisMonthBtn;
        set => this.RaiseAndSetIfChanged(ref _isClickedOrNotColorOfThisMonthBtn, value);
    }

    private bool _isPageActivated = true;

    public bool IsPageActivated
    {
        get => _isPageActivated;
        set => this.RaiseAndSetIfChanged(ref _isPageActivated, value);
    }

    public ReactiveCommand<Unit, Unit> ThisDayBtnCommand { get ; set; }
    public ReactiveCommand<Unit, Unit> ThisWeekBtnCommand { get; set; }
    public ReactiveCommand<Unit, Unit> ThisMonthBtnCommand { get; set; }

    public void ChangeTheColorOfBtnIfIsDayOrThisWeekOrThisMonth()
    {
        // Reset all button colors to Gainsboro
        setTheGrayColorOfAllBtns();

        // Change the color of the clicked button to red based on the condition
        if (StartAndEndDate_Are_ThisDay())
        {
            IsClickedOrNotColorOfThisDayBtn = "Red";
        }
        else if (StartAndEndDate_Are_ThisWeek())
        {
            IsClickedOrNotColorOfThisWeekBtn = "Red";
        }
        else if (StartAndEndDate_Are_ThisMonth())
        {
            IsClickedOrNotColorOfThisMonthBtn = "Red";
        }
    }


    protected void setTheGrayColorOfAllBtns()
    {
        IsClickedOrNotColorOfThisDayBtn = "Gainsboro";
        IsClickedOrNotColorOfThisWeekBtn = "Gainsboro";
        IsClickedOrNotColorOfThisMonthBtn = "Gainsboro";
    }

    protected void setStartAndDateOfToday_WhenTodayBtnIsClicked()
    {
        StartDate = EndDate = DateTime.Today.Date;
    }

    protected void setStartAndDateThisWeek_WhenWeekBtnIsClicked()
    {
        // Set EndDate to today's date
        EndDate = DateTime.Today.Date;

        // Set StartDate to seven days ago
        StartDate = EndDate.AddDays(-7);
    }


    protected void setStartAndDateOfThisMonth_WhenThisMonthBtnIsClicked()
    {
        // Set EndDate to today's date
        EndDate = DateTime.Today.Date;

        // Set StartDate to thirty days ago
        StartDate = EndDate.AddDays(-30);
    }

    private bool StartAndEndDate_Are_ThisDay()
    {
        return StartDate.Date == EndDate.Date && StartDate.Date == DateTime.Today.Date && EndDate.Date == DateTime.Today.Date;
    }

    private bool StartAndEndDate_Are_ThisWeek()
    {
        // Calculate the start date of the week containing StartDate
        DateTime sevenDaysAgo = DateTime.Today.Date.AddDays(-7);


        // Check if StartDate is exactly 7 days ago and EndDate is today (both dates without time component)
        bool isWithinThisWeek = StartDate.Date == sevenDaysAgo && EndDate.Date == DateTime.Today.Date;

        return isWithinThisWeek;
    }


    private bool StartAndEndDate_Are_ThisMonth()
    {
        // Calculate the difference in days between StartDate and EndDate
        TimeSpan difference = EndDate - StartDate;
        bool StartDateIsThisYear = StartDate.Year == DateTime.Today.Year;
        bool EndDateIsThisYear = EndDate.Year == DateTime.Today.Year;

        // Check if the difference is exactly 30 days
        return StartDateIsThisYear && EndDateIsThisYear && difference.Days == 30;
    }

    protected void WhenUserPickTime_DisplayTheColorBtnThatIndicate_IfIsDayOrThisWeekOrThisMonth()
    {
        this.WhenAnyValue(x => x.StartDate, x => x.EndDate)
           .Subscribe(_ =>
           {


               ChangeTheColorOfBtnIfIsDayOrThisWeekOrThisMonth();

           }
               );
    }
}
