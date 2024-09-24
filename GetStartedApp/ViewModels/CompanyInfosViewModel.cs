using ReactiveUI;
using System;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using GetStartedApp.Models;
using System.Data;
using GetStartedApp.Helpers;
using System.Reactive.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;


namespace GetStartedApp.ViewModels
{
    public class CompanyInfosViewModel : ViewModelBase
    {
        public Func<Task<string>> PickCompanyLogoImageFunc { get; set; }
        // Existing property
        private Bitmap _SelectedLogoImageToDisplay;
        public Bitmap SelectedLogoImageToDisplay
        {
            get { return _SelectedLogoImageToDisplay; }
            set => this.RaiseAndSetIfChanged(ref _SelectedLogoImageToDisplay,value);
           
        }
          // New properties
          private string _companyName;
        [Required(ErrorMessage = "ادخل اسم الشركة")]
        public string CompanyName
        {
            get => _companyName;
            set => this.RaiseAndSetIfChanged(ref _companyName, value);
        }

        private string companyLocation;

        [Required(ErrorMessage = "اادخل العنوان")]
        public string CompanyLocation
        {
            get => companyLocation;
            set => this.RaiseAndSetIfChanged(ref companyLocation, value);
        }

        private string _ice;
        private string _rc;
        private string _identifiantFiscal;
        private string _cnss;
        private string _patente;
        private string _bankAccountNumber;
        private string _email;

        public string ICE
        {
            get => _ice;
            set => this.RaiseAndSetIfChanged(ref _ice, value);
        }

        public string RC
        {
            get => _rc;
            set => this.RaiseAndSetIfChanged(ref _rc, value);
        }

        public string CNSS
        {
            get => _cnss;
            set => this.RaiseAndSetIfChanged(ref _cnss, value);
        }

        public string Patente
        {
            get => _patente;
            set => this.RaiseAndSetIfChanged(ref _patente, value);
        }

        public string IdentifiantFiscal
        {
            get => _identifiantFiscal;
            set => this.RaiseAndSetIfChanged(ref _identifiantFiscal, value);
          }

        public string Email
        {
            get => _email;
            set => this.RaiseAndSetIfChanged(ref _email, value);
        }

        private string _selectedCompanyLogoPath;
        private int companyIdOfAppUser = 1;
        CompanyInfo myCompanyInfo;

        // Commands
        public ReactiveCommand<Unit, Unit> PickLogoImageCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteLogoImageCommand { get; }
        public ReactiveCommand<Unit, Unit> AddOrUpdateCompanyInfoCommand { get; }
        
        public Interaction <string,Unit> ShowMessageBoxDialog { get; }
        public CompanyInfosViewModel()
        {
            PickLogoImageCommand = ReactiveCommand.CreateFromTask(OnPickLogoImage);
            DeleteLogoImageCommand = ReactiveCommand.CreateFromTask(OnDeleteLogoImage);
            AddOrUpdateCompanyInfoCommand = ReactiveCommand.Create(AddOrUpdateCompanyInfo,CheckIfUserHasEnteredCompanyInfo());
 
            LoadAllOfMyCompanyInfos();

            ShowMessageBoxDialog = new Interaction<string, Unit>();

        }

        private void LoadAllOfMyCompanyInfos()
        {

            myCompanyInfo = AccessToClassLibraryBackendProject.LoadCompanyInfo(companyIdOfAppUser);
            DisplayLogoImageIfExist_IfNotDisplayNoImage();

            if (UserHaventSetYetHisCompanyInfo()) return;

            CompanyName = myCompanyInfo.CompanyName;
            CompanyLocation = myCompanyInfo.CompanyLocation;
            ICE = myCompanyInfo.ICE;
            RC = myCompanyInfo.RC;
            Patente = myCompanyInfo.Patente;
            IdentifiantFiscal = myCompanyInfo.IFS;
            CNSS = myCompanyInfo.CNSS;
            Email = myCompanyInfo.Email;
           
        }

        private bool UserHaventSetYetHisCompanyInfo()
        {
            return myCompanyInfo == null;
        }

        private async void DisplayLogoImageIfExist_IfNotDisplayNoImage()
        {

            if (myCompanyInfo == null) { DisplayNoImage(); return; }

            var myCompanyLogo = myCompanyInfo.CompanyLogo;
          
            SelectedLogoImageToDisplay = myCompanyLogo;
        }

      
        private async Task DisplayNoImage()
        {

            SelectedLogoImageToDisplay = ImageHelper.LoadFromResource(new Uri("avares://GetStartedApp/Assets/Icons/NoImageImage.png"));
        
        }
        private async Task OnPickLogoImage()
        {
            if (PickCompanyLogoImageFunc != null)
            {
                // save the path of image selected 
                _selectedCompanyLogoPath = await PickCompanyLogoImageFunc.Invoke();

                // we check if the user has selected an image path becuase sometimes it can close the window and not selectged an image
                // which can throw error and crash the app
                if (_selectedCompanyLogoPath != "")
                {
                    // display the image selected after resizing it to reduce it memory taken in space
                    // this is for the sake of improving performance
                    SelectedLogoImageToDisplay = await ImageResizerHelper.ResizeImageAsync(_selectedCompanyLogoPath);

                }

            }
        }

        private async Task OnDeleteLogoImage()
        {
            DisplayNoImage();
        }

        private async void AddOrUpdateCompanyInfo()
        {
            var companyLogoConvertedToByteArray = ImageConverter.BitmapToByteArray(SelectedLogoImageToDisplay);

            // the first company is the one of the user of this application which it's id is going to be equal to 1          

           if( AccessToClassLibraryBackendProject.
                AddOrUpdateCompany
                (companyIdOfAppUser, companyLogoConvertedToByteArray,CompanyName,
                CompanyLocation, ICE,IdentifiantFiscal, Email, Patente, RC, CNSS))
            {
                await ShowMessageBoxDialog.Handle("تمت عملية التسجيل بنجاح");
            }

           else await ShowMessageBoxDialog.Handle("لقد حصل خطأ ما");

        }
        private IObservable<bool> CheckIfUserHasEnteredCompanyInfo()
        {
            return this.WhenAnyValue(
                x => x.CompanyName,
                x => x.CompanyLocation,
                x => x.SelectedLogoImageToDisplay,
                x => x.RC,
                x => x.IdentifiantFiscal,
                x => x.Email,
                x => x.CNSS,
                x => x.Patente,
                (companyName, companyLocation, SelectedLogoImageToDisplay,RC, IdentifiantFiscal, Email, CNSS, Patente) =>
                    !string.IsNullOrEmpty(companyName) &&
                    !string.IsNullOrEmpty(companyLocation) 
                    
            ) ;
            
        }

    }
}
