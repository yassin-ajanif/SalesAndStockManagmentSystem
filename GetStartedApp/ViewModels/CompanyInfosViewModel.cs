using ReactiveUI;
using System;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using GetStartedApp.Models;
using System.Data;
using GetStartedApp.Helpers;


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
          private string companyName;
        public string CompanyName
        {
            get => companyName;
            set => this.RaiseAndSetIfChanged(ref companyName, value);
        }

        private string companyLocation;
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

        private DataTable CompanyLogo_Name_Address_ICI_ProfessonalTax_TaxID { get; }
        // Commands
        public ReactiveCommand<Unit, Unit> PickLogoImageCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteLogoImageCommand { get; }
        public ReactiveCommand<Unit, Unit> AddOrUpdateCompanyInfoCommand { get; }

        public CompanyInfosViewModel()
        {
            PickLogoImageCommand = ReactiveCommand.CreateFromTask(OnPickLogoImage);
            DeleteLogoImageCommand = ReactiveCommand.CreateFromTask(OnDeleteLogoImage);
            AddOrUpdateCompanyInfoCommand = ReactiveCommand.Create(AddOrUpdateCompanyInfo,CheckIfUserHasEntredCompanyInfo());

           // CompanyLogo_Name_Address_ICI_ProfessonalTax_TaxID = AccessToClassLibraryBackendProject.GetCompanyDetails();
           //
           // DisplayLogoImageIfExist_IfNotDisplayNoImage();
           //
           // DisplayCompany_Name_Address_IfExist();
           //
           // DisplayCompany_ICI_ProfessionalTaxID_TaxID();
        }

        private void LoadAllCompanyInfos()
        {

        }

        private async void DisplayLogoImageIfExist_IfNotDisplayNoImage()
        {

            bool imageLogoExists = AccessToClassLibraryBackendProject.IsCompanyLogoExisting();

            if (imageLogoExists)
            {

                byte[] companyLogo = CompanyLogo_Name_Address_ICI_ProfessonalTax_TaxID.Rows[0]["CompanyLogo"] as byte[];

                SelectedLogoImageToDisplay = ImageConverter.ByteArrayToBitmap(companyLogo);

            }

            else { DisplayNoImage(); }

        }

        private void DisplayCompany_Name_Address_IfExist()
        {
            string? companyName = CompanyLogo_Name_Address_ICI_ProfessonalTax_TaxID.Rows[0]["CompanyName"] as string;
            string? companyAddress = CompanyLogo_Name_Address_ICI_ProfessonalTax_TaxID.Rows[0]["CompanyLocation"] as string;

            if (companyName == null) companyName = string.Empty;
            else this.CompanyName = companyName;

            if (companyAddress == null) companyAddress = string.Empty;
            else this.CompanyLocation = companyAddress;
        }

        private void DisplayCompany_ICI_ProfessionalTaxID_TaxID()
        {
            string? comapanyICE = CompanyLogo_Name_Address_ICI_ProfessonalTax_TaxID.Rows[0]["ICE"] as string;
            string? RC = CompanyLogo_Name_Address_ICI_ProfessonalTax_TaxID.Rows[0]["RC"] as string;
            string? companyTaxID = CompanyLogo_Name_Address_ICI_ProfessonalTax_TaxID.Rows[0]["TaxID"] as string;

            if (comapanyICE == null) comapanyICE = string.Empty;
            else this.ICE = comapanyICE;

            if (RC == null) RC = string.Empty;
            else this.RC = RC;

            if (companyTaxID == null) companyTaxID = string.Empty;
            else this.IdentifiantFiscal = companyTaxID;
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
            int companyIdOfAppUser = 1;

            AccessToClassLibraryBackendProject.
                AddOrUpdateCompany(companyIdOfAppUser, companyLogoConvertedToByteArray,CompanyName, CompanyLocation, ICE,IdentifiantFiscal, Email, Patente, RC, CNSS);
        }
        private IObservable<bool> CheckIfUserHasEntredCompanyInfo()
        {
            return this.WhenAnyValue(
                             x => x.CompanyName,
                             x => x.CompanyLocation,

                             (EntredCompanyName, EntredCompanyLocation) =>
                             !string.IsNullOrEmpty(EntredCompanyName) && !string.IsNullOrWhiteSpace(EntredCompanyName) &&
                             !string.IsNullOrEmpty(EntredCompanyLocation) && !string.IsNullOrWhiteSpace(EntredCompanyLocation));
        }
    }
}
