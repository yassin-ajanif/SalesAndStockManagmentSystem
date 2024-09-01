using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using GetStartedApp.ViewModels.DashboardPages;
using GetStartedApp.Models;
using Avalonia.Metadata;
using System.Reactive.Subjects;
using System.Reactive.Linq;

namespace GetStartedApp.ViewModels
{
    public class LisenceKeyVerificationViewModel : ViewModelBase
    {
        private string _password;
        private bool _isErrorVisible;
        private string _isPasswordCorrectMessage;

        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        public bool IsErrorVisible
        {
            get => _isErrorVisible;
            set => this.RaiseAndSetIfChanged(ref _isErrorVisible, value);
        }

  
        public string IsPasswordCorrectMessage
        {
            get => _isPasswordCorrectMessage;
            set => this.RaiseAndSetIfChanged(ref _isPasswordCorrectMessage, value);
        }

        public ReactiveCommand<Unit, Unit> SubmitCommand { get; }
        public ReactiveCommand<Unit, Unit> SubmitTrialCommand { get; }

        private BehaviorSubject<bool> _canUserHaveTrialOptionSubject = new BehaviorSubject<bool>(false);

        IObservable<bool> canUserHaveTrialOption { get; set; }


        MainWindowViewModel mainWindowViewModel { get; }
        public LisenceKeyVerificationViewModel(MainWindowViewModel mainViewmodel)
        {
            // set false fro an iobservable value 
            canUserHaveTrialOption = _canUserHaveTrialOptionSubject.AsObservable();

            SubmitCommand = ReactiveCommand.Create(Submit);
            SubmitTrialCommand = ReactiveCommand.Create(GoToTrialMode,canUserHaveTrialOption);
            mainWindowViewModel = mainViewmodel;

            CheckIfAdminIsRegistredBeforeOrNot_And_returnThePageToGoOn();
        }

        // we insert the starting time of app in the database to compare it later without our time to
        // detect if trial user has passed the time supported which is 30 days
        private bool InsertFirstRunDateAndSetTrialMode()
        {
            bool RegistrationModeIsPaid = false;
            return AccessToClassLibraryBackendProject.InsertFirstRunDateAndSetTrialOrPaidMode(RegistrationModeIsPaid);
        }

        private bool InsertFirstRunDateAndSetPaidMode()
        {
            bool RegistrationModeIsPaid = true;
            return AccessToClassLibraryBackendProject.InsertFirstRunDateAndSetTrialOrPaidMode(RegistrationModeIsPaid);
        }


        public bool IsTrialPeriodEnded()
        {
            TimeSpan trialDuration = TimeSpan.FromDays(30);
            DateTime? firstRunDateTime = AccessToClassLibraryBackendProject.GetFirstRunDate();

            if (firstRunDateTime.HasValue)
            {
               // DateTime currentDateTime = DateTime.Now; // Current date and time
                DateTime currentDateTime = DateTime.Now; // Current date and time
                DateTime firstRunDateTimeValue = firstRunDateTime.Value;

                // Check if the current time minus the first run time exceeds the trial duration

                return (currentDateTime - firstRunDateTimeValue) >= trialDuration;
            }
            else
            {
                // No first run date found, handle appropriately
                return false; // Assuming trial period hasn't ended if no first run date
            }
        }



        private bool AdminIsInPaidMode()
        {
            return AccessToClassLibraryBackendProject.IsApplicationUsersInPaidMode();
        }

        private bool AdminHasRegistredBeforeToThisApplication()
        {
            return AccessToClassLibraryBackendProject.GetFirstRunDate() != null;
        }

        private void EnableTheTrialBtn()
        {
            // we entable the btn of trial option using true
             _canUserHaveTrialOptionSubject.OnNext(true); 

        }


        private bool IsTheLisenceKeyValid()
        {
            if (Password == "yassinajanif0611662541")
            {
                // Password is correct
                IsErrorVisible = true;
                IsPasswordCorrectMessage = "your password is correct";
                return true;
            }
            else
            {
                // Incorrect password
                IsErrorVisible = true;
                IsPasswordCorrectMessage = "your password is false try again";
                return false;
            }
        }

        private void GoToTrialMode()
        {
            if (InsertFirstRunDateAndSetTrialMode()) { mainWindowViewModel.GoToLoginPage(); }
        }

        private void GoToPaidMode()
        {
            if (InsertFirstRunDateAndSetPaidMode()) { mainWindowViewModel.GoToLoginPage(); }
        }

        private bool IsAnAdminIs_ANewUser()
        {
            return !AdminHasRegistredBeforeToThisApplication();
        }

        private void ShowTheErrorOfExpiredTrialPeriod()
        {
            IsErrorVisible = true;
            IsPasswordCorrectMessage = " لقد انتهت المدة التجربية للبرنامج المرجو ادخال الرقم السري الخاص بالبرنامج ";
        }

        private void DeleteTheErrorOfExpiredTrialPeriod()
        {
            IsErrorVisible = false;
            
        }

        public ViewModelBase CheckIfAdminIsRegistredBeforeOrNot_And_returnThePageToGoOn()
        {  
            ViewModelBase thisPage = this;

           if(IsAnAdminIs_ANewUser()) { EnableTheTrialBtn(); DeleteTheErrorOfExpiredTrialPeriod(); return thisPage; }

            // this code below will be exiecuted if the admin is already regestried to the app

            if (AdminIsInPaidMode()) { DeleteTheErrorOfExpiredTrialPeriod(); return mainWindowViewModel.GoToLoginPage(); }


            if (IsTrialPeriodEnded()) { ShowTheErrorOfExpiredTrialPeriod(); return thisPage; }

            // when the trial error is not yet ended
            DeleteTheErrorOfExpiredTrialPeriod();
            return mainWindowViewModel.GoToLoginPage();

        }
              

        public void Submit()
        {

            if (IsTheLisenceKeyValid()) { GoToPaidMode(); }

        }
    
    }
}
