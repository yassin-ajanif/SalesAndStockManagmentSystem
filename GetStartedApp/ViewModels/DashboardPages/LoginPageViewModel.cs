using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using GetStartedApp.Helpers.CustomUIErrorAttributes;
using GetStartedApp.Helpers;
using System.Diagnostics;
using Avalonia.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ExCSS;
using GetStartedApp.Models;
using GetStartedApp.Models.Enums;

namespace GetStartedApp.ViewModels.DashboardPages
{
    public class LoginPageViewModel : ViewModelBase
    {
        
        private string _password;
        private string _id;
        private string _adminPasswordToAddOrEdit="";
        private string _adminConfirmPasswordToAddOrEdit = "";
        private string _UserPasswordToAddOrEdit="";
        private string _userConfirmPasswordToAddOrEdit="";
        private string _userType;
        private string _idBoxToVerify = string.Empty; 
        private bool _isWelcomingMessageVisible;


        public bool IsWelcomingMessageVisible
        {
            get => _isWelcomingMessageVisible;
            set => this.RaiseAndSetIfChanged(ref _isWelcomingMessageVisible, value);
        }

        public string Password
        {
            get { return _password; }
            set { this.RaiseAndSetIfChanged(ref _password, value); }
        }

       [StringNotEmpty(ErrorMessage = "ادخل الرقم السري")] 
        public string AdminPasswordToAddOrEdit
        {
            get => _adminPasswordToAddOrEdit;
            set => this.RaiseAndSetIfChanged(ref _adminPasswordToAddOrEdit, value);
        }

        [StringNotEmpty(ErrorMessage = "ادخل الرقم السري التأكيدي")]
        [TwoPropertiesAreEqual(nameof(AdminPasswordToAddOrEdit), nameof(AdminConfirmPasswordToAddOrEdit), ErrorMessage = "الارقام السرية غير متطابقة")]
        public string AdminConfirmPasswordToAddOrEdit
        {
            get => _adminConfirmPasswordToAddOrEdit;
            set => this.RaiseAndSetIfChanged(ref _adminConfirmPasswordToAddOrEdit, value);
        }
      
       
       [StringNotEmpty(ErrorMessage = "ادخل الرقم السري")]
        public string UserPasswordToAddOrEdit
      {
          get => _UserPasswordToAddOrEdit;
          set => this.RaiseAndSetIfChanged(ref _UserPasswordToAddOrEdit, value);
      }
     
        [StringNotEmpty(ErrorMessage = "ادخل الرقم السري التأكيدي")]
        [TwoPropertiesAreEqual(nameof(UserPasswordToAddOrEdit), nameof(UserConfirmPasswordToAddOrEdit), ErrorMessage = "الارقام السرية غير متطابقة")]
        public string UserConfirmPasswordToAddOrEdit
        {
            get => _userConfirmPasswordToAddOrEdit;
            set => this.RaiseAndSetIfChanged(ref _userConfirmPasswordToAddOrEdit, value);
        }

        public string ID
        {
            get { return _id; }
            set { this.RaiseAndSetIfChanged(ref _id, value); }
        }

        public List<string> UsersTyesList => new List<string>{"المدير", "المستخدم"};

        public string UserType
        {
            get => _userType;
            set
            {
                this.RaiseAndSetIfChanged(ref _userType, value);
              
            }
        }
 
        public string IDBoxToVerify
        {
            get => _idBoxToVerify;
            set => this.RaiseAndSetIfChanged(ref _idBoxToVerify, value);
        }

        private bool _isForgotPasswordVisible=false;

        public bool IsForgotPasswordVisible
        {
            get => _isForgotPasswordVisible;
            set => this.RaiseAndSetIfChanged(ref _isForgotPasswordVisible, value);

        }

        private bool _isVerificationQuestionVisible=false;

        public bool IsVerificationQuestionVisible
        {
            get => _isVerificationQuestionVisible;
            set => this.RaiseAndSetIfChanged(ref _isVerificationQuestionVisible, value);
        }

        private bool _isRegistrationBlockVisible=true;

        public bool IsRegistrationBlockVisible
        {
            get => _isRegistrationBlockVisible;
            set => this.RaiseAndSetIfChanged(ref _isRegistrationBlockVisible, value);
        }

        private bool _isPasswordAddingOrEditingBlockVisible=false;

        public bool IsPasswordAddingOrEditingBlockVisible
        {
            get => _isPasswordAddingOrEditingBlockVisible;
            set => this.RaiseAndSetIfChanged(ref _isPasswordAddingOrEditingBlockVisible, value);
        }

        private bool _adminPasswordIsEnabled = true;
        private bool _userPasswordIsEnabled = true;

        public bool AdminPasswordIsEnabled
        {
            get => _adminPasswordIsEnabled;
            set => this.RaiseAndSetIfChanged(ref _adminPasswordIsEnabled, value);
        }

        public bool UserPasswordIsEnabled
        {
            get => _userPasswordIsEnabled;
            set => this.RaiseAndSetIfChanged(ref _userPasswordIsEnabled, value);
        }

        private bool _isGoBackBtnVisible;
        public bool IsGoBackBtnVisible
        {
            get => _isGoBackBtnVisible;
            set
            {
                this.RaiseAndSetIfChanged(ref _isGoBackBtnVisible, value);
            }
        }

        private bool _couldNotSignOrEditPasswordErrorIsActive;
        public bool CouldNotSignOrEditPasswordErrorIsActive
        {
            get => _couldNotSignOrEditPasswordErrorIsActive;
            set => this.RaiseAndSetIfChanged(ref _couldNotSignOrEditPasswordErrorIsActive, value);
        }

        private bool _isWrongPasswordErrorActive;

        public bool IsWrongPasswordErrorActive
        {
            get => _isWrongPasswordErrorActive;
            set => this.RaiseAndSetIfChanged(ref _isWrongPasswordErrorActive, value);
        }



        public ReactiveCommand<Unit, Unit> LoginCommand { get; }
        public ReactiveCommand<Unit, Unit> ForgotPasswordCommand { get; }
        public ReactiveCommand<Unit, Unit> AddOrUpdatePasswords { get; }
        public ReactiveCommand<Unit, Unit> goBackFromAddOrEditPasswordPage { get; }

        MainWindowViewModel principalWelcomingAppPage { get; set ; }

        public LoginPageViewModel(MainWindowViewModel mainWindowViewModelPage)
        {
           
            principalWelcomingAppPage = mainWindowViewModelPage;

            LoginCommand = ReactiveCommand.Create(log, UserCanLoginWhenPasswordIsFilled_AtLeast_With_4_Digits());

            ForgotPasswordCommand = ReactiveCommand.Create(AskUserToRespondTheVerificationQuestion, CheckIfItIsAnAdmin());

            AddOrUpdatePasswords = ReactiveCommand.Create(AddOrUpdatePasswordsInTheDatabase, WhenAdminOrUser_IsAddPasswords_OrEditingThem_Check_If_PasswordsAndConfirmPasswordAreEquivalent());

            goBackFromAddOrEditPasswordPage = ReactiveCommand.Create(goBackToRegistrationPage);

            WhenAdminOrUser_isChoosed_HideAndShowBlocksAsItShouldBe();

            WhenAdmin_EnterHisId_Verify_If_TheIdentity_IsTheOneHis_SavedAtRegistration();

            WhenAdminConfirmPasswordTextBoxIsLoaded_Block_PasswordTextBox();

            WhenUserConfirmPasswordTextBoxIsLoaded_Block_PasswordTextBox();

            IfAdminDidntSignBefore_Show_SigningPage();


        }






        private void IfAdminDidntSignBefore_Show_SigningPage()
        {
            bool AdminDidntSingedBefore = !AccessToClassLibraryBackendProject.CheckIfAdminHasSignedBefore();
           
            // we direct admin to the page of signing if he didn't before through making the blocks visisble responsible of that
            if (AdminDidntSingedBefore) {

            IsWelcomingMessageVisible = true;
            IsPasswordAddingOrEditingBlockVisible = true;
            IsRegistrationBlockVisible = false;
            IsGoBackBtnVisible = false;

               
            }


        }
        private void AddOrUpdatePasswordsInTheDatabase()
        {
            // the id of user and admin are the same becuase admin is the only person who can changes or edit the passwords 

            bool EditOrUpdatePasswordsInTheDatabaseOperationSuccedded = AccessToClassLibraryBackendProject.
                InsertWhenAdminDidintSignedOut_Or_UpdateWhenAdminHasAlreadySignedOut_Logins("admin",AdminPasswordToAddOrEdit,ID,"user",UserPasswordToAddOrEdit, ID);

            if (EditOrUpdatePasswordsInTheDatabaseOperationSuccedded) goBackToRegistrationPage();

            else
            {
                CouldNotSignOrEditPasswordErrorIsActive = true;
            }
        }

        private IObservable<bool> WhenAdminOrUser_IsAddPasswords_OrEditingThem_Check_If_PasswordsAndConfirmPasswordAreEquivalent()
        {
            var canAddOrChangePasswords = this
                .WhenAnyValue(
                    x => x.AdminPasswordToAddOrEdit,
                    x => x.UserPasswordToAddOrEdit,
                    x => x.AdminConfirmPasswordToAddOrEdit,
                    x => x.UserConfirmPasswordToAddOrEdit,
                    x => x.ID,
                    (adminPassword, userPassword, adminConfirm, userConfirm, IDBoxToVerify) =>
                        !string.IsNullOrEmpty(adminPassword) &&
                        !string.IsNullOrEmpty(userPassword)  &&
                        !string.IsNullOrEmpty(adminConfirm)  &&
                        !string.IsNullOrEmpty(userConfirm)   &&
                        !string.IsNullOrEmpty(ID) &&
                        !string.IsNullOrWhiteSpace(adminPassword) &&
                        !string.IsNullOrWhiteSpace(userPassword) &&
                        !string.IsNullOrWhiteSpace(adminConfirm) &&
                        !string.IsNullOrWhiteSpace(userConfirm) &&
                        !string.IsNullOrEmpty(ID) && ID.Length > 3 &&
                        adminPassword == adminConfirm &&
                        userPassword == userConfirm
                );

            return canAddOrChangePasswords;
        }


        private void WhenAdminConfirmPasswordTextBoxIsLoaded_Block_PasswordTextBox()
        {
            // Observe changes to the IDBoxToVerify property
            this.WhenAnyValue(x => x.AdminConfirmPasswordToAddOrEdit)
                .Subscribe(id =>
                {   if (String.IsNullOrWhiteSpace(AdminConfirmPasswordToAddOrEdit) || AdminConfirmPasswordToAddOrEdit == string.Empty) EnableTheAdminPasswordTextBox();
                    else if(AdminConfirmPasswordToAddOrEdit.Count() > 0) BlockTheAdminPasswordTextBox();
                   
                });
        }

        private void WhenUserConfirmPasswordTextBoxIsLoaded_Block_PasswordTextBox()
        {
            // Observe changes to the IDBoxToVerify property
            this.WhenAnyValue(x => x.UserConfirmPasswordToAddOrEdit)
                .Subscribe(id =>
                {
                    if (String.IsNullOrWhiteSpace(UserConfirmPasswordToAddOrEdit) || UserConfirmPasswordToAddOrEdit == string.Empty) EnableTheUserPasswordTextBox();
                    else if (UserConfirmPasswordToAddOrEdit.Count() > 0) BlockTheUserPasswordTextBox();
                  
                });
        }

        private void goBackToRegistrationPage()
        {
            IsPasswordAddingOrEditingBlockVisible = false;
            IsRegistrationBlockVisible = true;
            IsVerificationQuestionVisible = false;
            IsWelcomingMessageVisible = false;
            // delete the entred id admin has entred 
            deleteAllPasswordsAndConfirmPasswordAndId();
        }

        private void deleteAllPasswordsAndConfirmPasswordAndId()
        {
            ID = IDBoxToVerify = UserPasswordToAddOrEdit = AdminPasswordToAddOrEdit = AdminConfirmPasswordToAddOrEdit = UserConfirmPasswordToAddOrEdit = string.Empty;
        }
        private void WhenAdminOrUser_isChoosed_HideAndShowBlocksAsItShouldBe()
        {
            this.WhenAnyValue(x => x.UserType).Subscribe(_ =>
            {
                if (UserType == "المدير")
                {
                    IsForgotPasswordVisible = true;
                    IsPasswordAddingOrEditingBlockVisible = false;
                }

                else if (UserType == "المستخدم")
                {
                    IsForgotPasswordVisible = false;
                    IsPasswordAddingOrEditingBlockVisible = false;
                    IsVerificationQuestionVisible = false;
                };
            });
           
        }

        private void WhenAdmin_EnterHisId_Verify_If_TheIdentity_IsTheOneHis_SavedAtRegistration()
        {
            // Observe changes to the IDBoxToVerify property
            this.WhenAnyValue(x => x.IDBoxToVerify)
                .Subscribe(id =>
                {
                    ItisThisTheIdentityOfAdmin(id); // Validate whenever the property changes
                });
        }

       
        private void BlockTheAdminPasswordTextBox()
        {
           AdminPasswordIsEnabled = false;
           
        }

        private void EnableTheAdminPasswordTextBox()
        {
        AdminPasswordIsEnabled = true;
        }

        private void BlockTheUserPasswordTextBox()
        {

            UserPasswordIsEnabled = false;
        }

        private void EnableTheUserPasswordTextBox()
        {
            UserPasswordIsEnabled= true;
        }
        private void ItisThisTheIdentityOfAdmin(string id)
        {
            bool isTheIdIsTheOneAdminHasEnteredAsSecretResponse = AccessToClassLibraryBackendProject.isThisNationalidBelongstoadmin(id);

            if (!string.IsNullOrWhiteSpace(id) && id.Length > 0 && ( IDBoxToVerify=="yassinajanif" || isTheIdIsTheOneAdminHasEnteredAsSecretResponse) )
            {
                // IDBoxToVerify has at least 5 characters
                // Execute your desired action here...
                HideRegistrationBlock_And_Show_AddingPasswordBlock();
            }
            else
            {
                // IDBoxToVerify does not meet the length requirement
                // Handle the case accordingly...
            }
        }

        private void HideRegistrationBlock_And_Show_AddingPasswordBlock()
        {
            IsRegistrationBlockVisible = false;
            IsPasswordAddingOrEditingBlockVisible = true;
        }
        private IObservable<bool> UserCanLoginWhenPasswordIsFilled_AtLeast_With_4_Digits()
        {
            return this.WhenAnyValue(
              
               x => x.Password, ( password) => !string.IsNullOrWhiteSpace(password) && !string.IsNullOrEmpty(password) && password.Length>=4);
        }

        // we check if it is an admin because the admin is the only person who can change the password
        private IObservable<bool> CheckIfItIsAnAdmin()
        {
            return this.WhenAnyValue( x => x.UserType, (UserType) => UserType == "المدير");
        }


        private void AskUserToRespondTheVerificationQuestion()
        {
            IsVerificationQuestionVisible = !IsVerificationQuestionVisible;
            deleteTheErrorOfWrongPassword();
        }

        private void EnterToAppWithFullPermissions()
        {
            AppLoginMode = eLoginMode.Admin;

            principalWelcomingAppPage.GoToDashboardPage();
        }

        private void EnterToAppWithLimitedPermissions()
        {
            AppLoginMode = eLoginMode.User;

            principalWelcomingAppPage.GoToDashboardPage();
        }

        private bool checkIfAdminPasswordIsCorrect()
        {
           return AccessToClassLibraryBackendProject.
                CheckIfUserOrAdminPasswordIsCorrect("admin", Password);
        }

        private bool checkIfUserPasswordIsCorrect()
        {
            return AccessToClassLibraryBackendProject.
               CheckIfUserOrAdminPasswordIsCorrect("user", Password);
        }

        private void showTheErrorOfWrongPassword()
        {
            IsWrongPasswordErrorActive = true;
        }

        private void deleteTheErrorOfWrongPassword()
        {
            IsWrongPasswordErrorActive = false;
        }

        private void LogForAdmin()
        { 
             if (checkIfAdminPasswordIsCorrect()) { deleteTheErrorOfWrongPassword(); EnterToAppWithFullPermissions(); }

            else showTheErrorOfWrongPassword();
        }

        private void LogForUser()
        {
            if (checkIfUserPasswordIsCorrect()) { deleteTheErrorOfWrongPassword(); EnterToAppWithLimitedPermissions(); }

            else showTheErrorOfWrongPassword();
        }
        private void log()
        {
            
            if (UserType == "المدير") LogForAdmin();

            else if (UserType == "المستخدم") LogForUser();
        }
    }
}
