using Avalonia.Media.Imaging;
using GetStartedApp.Helpers;
using GetStartedApp.Models.Objects;
using GetStartedApp.ViewModels.DashboardPages;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Reactive;
using System.Threading.Tasks;


namespace GetStartedApp.ViewModels
{
    public class BLViewModel : ViewModelBase
    {

        //public Func<Task<string>> PickCompanyLogoImageFunc { get; set; }
        //// Existing property
        //private Bitmap _SelectedLogoImageToDisplay;
        //public Bitmap SelectedLogoImageToDisplay
        //{
            //get { return _SelectedLogoImageToDisplay; }
      /*      set { this.RaiseAndSetIfChanged(ref _SelectedLogoImageToDisplay value); },*/
        //}

        //// New properties
        //private string companyName;
        //public string CompanyName
        //{
            //get => companyName;
            //set => this.RaiseAndSetIfChanged(ref companyName, value);
        //}

        //private string companyLocation;
        //public string CompanyLocation
        //{
            //get => companyLocation;
            //set => this.RaiseAndSetIfChanged(ref companyLocation, value);
        //}

        //private string _ice;
        //private string _nTaxProfessional;
        //private string _identifiantFiscal;

        //public string ICE
        //{
            //get => _ice;
            //set => this.RaiseAndSetIfChanged(ref _ice, value);
        //}

        //public string NTaxProfessional
        //{
            //get => _nTaxProfessional;
            //set => this.RaiseAndSetIfChanged(ref _nTaxProfessional, value);
        //}

        //public string IdentifiantFiscal
        //{
            //get => _identifiantFiscal;
      /*      set => this.RaiseAndSetIfChanged(ref _identifiantFiscal, value);*/
        //}
        //// Commands
        //public ReactiveCommand<Unit, Unit> PickLogoImageCommand { get; }
        //public ReactiveCommand<Unit, Unit> DeleteLogoImageCommand { get; }
        public ReactiveCommand<Unit, Unit> GenerateBlsCommand { get; }

        private int _lastSaleClientID;
        private string _lastSaleClientName;

       // private DataTable CompanyLogo_Name_Address_ICI_ProfessonalTax_TaxID { get; }
       //
       // private string _selectedCompanyLogoPath;

        DataTable TableProductToPrintInBl;

        MainWindowViewModel mainWindowViewModel;
        MakeSaleViewModel makeSaleViewModel;

        public BLViewModel(MainWindowViewModel mainWindowViewModel,MakeSaleViewModel makeSaleViewModel, 
            ObservableCollection<ProductsScannedInfo_ToSale> ProductsListScanned, int lastSaleClientID, string lastSaleClientName)
        {
           
                this.mainWindowViewModel = mainWindowViewModel;
                this.makeSaleViewModel = makeSaleViewModel;

                _lastSaleClientID   = lastSaleClientID;
                _lastSaleClientName = lastSaleClientName;

                // Initialize commands with appropriate logic
                 //PickLogoImageCommand = ReactiveCommand.CreateFromTask(OnPickLogoImage);
                 //DeleteLogoImageCommand = ReactiveCommand.CreateFromTask(OnDeleteLogoImage);
              //  GenerateBlsCommand = ReactiveCommand.Create(OnGenerateBls, CheckIfUserHasEntredCompanyInfo());

                //CompanyLogo_Name_Address_ICI_ProfessonalTax_TaxID = AccessToClassLibraryBackendProject.GetCompanyDetails();
              
                //DisplayLogoImageIfExist_IfNotDisplayNoImage();
              
                //DisplayCompany_Name_Address_IfExist();
              
                //DisplayCompany_ICI_ProfessionalTaxID_TaxID();
              
                TableProductToPrintInBl = GetFromProductListScanned_PrName_Price_Quantity_TotalPrPrice(ProductsListScanned);
            
        }

   //     private IObservable<bool> CheckIfUserHasEntredCompanyInfo()
   //     {
   //         return this.WhenAnyValue(
   //                          x => x.CompanyName,
   //                          x => x.CompanyLocation,
   //
   //                          (EntredCompanyName, EntredCompanyLocation ) =>
   //                          !string.IsNullOrEmpty(EntredCompanyName) && !string.IsNullOrWhiteSpace(EntredCompanyName) &&
   //                          !string.IsNullOrEmpty(EntredCompanyLocation) && !string.IsNullOrWhiteSpace(EntredCompanyLocation));                                                                        
   //     }

        private DataTable GetFromProductListScanned_PrName_Price_Quantity_TotalPrPrice(ObservableCollection<ProductsScannedInfo_ToSale> ProductsListScanned)
        {
            // Create a DataTable to hold the result
            DataTable dataTable = new DataTable();

            // Define the columns for the DataTable
            dataTable.Columns.Add("ProductName", typeof(string));
            dataTable.Columns.Add("Price", typeof(decimal));
            dataTable.Columns.Add("Quantity", typeof(int));
            dataTable.Columns.Add("TotalPrice", typeof(decimal));

            // Iterate over each product in the ObservableCollection
            foreach (var product in ProductsListScanned)
            {
                // Extract data from the product
                var productName = product.ProductInfo.name;
                var price = decimal.Parse(product.PriceOfProductSold);
                var quantity = int.Parse(product.ProductsUnits);
                var totalPrPrice = quantity * price;

                // Add a new row to the DataTable with the obtained values
                DataRow row = dataTable.NewRow();
                row["ProductName"] = productName;
                row["Price"] = price;
                row["Quantity"] = quantity;
                row["TotalPrice"] = totalPrPrice;

                dataTable.Rows.Add(row);
            }

            // Return the populated DataTable
            return dataTable;
        }

        //private async void DisplayLogoImageIfExist_IfNotDisplayNoImage()
        //{

        //    bool imageLogoExists = AccessToClassLibraryBackendProject.IsCompanyLogoExisting();

        //    if (imageLogoExists)
        //    {

        //        byte[] companyLogo = CompanyLogo_Name_Address_ICI_ProfessonalTax_TaxID.Rows[0]["CompanyLogo"] as byte[];

        //        SelectedLogoImageToDisplay = ImageConverter.ByteArrayToBitmap(companyLogo);

        //    }

        //    else { DisplayNoImage(); }

        //}

        //private void DisplayCompany_Name_Address_IfExist()
        //{
        //    string? companyName = CompanyLogo_Name_Address_ICI_ProfessonalTax_TaxID.Rows[0]["CompanyName"] as string;
        //    string? companyAddress = CompanyLogo_Name_Address_ICI_ProfessonalTax_TaxID.Rows[0]["CompanyLocation"] as string;

        //    if (companyName == null) companyName = string.Empty;
        //    else this.CompanyName = companyName;

        //    if (companyAddress == null) companyAddress = string.Empty;
        //    else this.CompanyLocation = companyAddress;
        //}

        //private void DisplayCompany_ICI_ProfessionalTaxID_TaxID()
        //{
        //    string? comapanyICE = CompanyLogo_Name_Address_ICI_ProfessonalTax_TaxID.Rows[0]["ICE"] as string;
        //    string? companyProfessionTaxID = CompanyLogo_Name_Address_ICI_ProfessonalTax_TaxID.Rows[0]["TaxProfessionalID"] as string;
        //    string? companyTaxID = CompanyLogo_Name_Address_ICI_ProfessonalTax_TaxID.Rows[0]["TaxID"] as string;

        //    if (comapanyICE == null) comapanyICE = string.Empty;
        //    else this.ICE = comapanyICE;

        //    if (companyProfessionTaxID == null) companyProfessionTaxID = string.Empty;
        //    else this.NTaxProfessional = companyProfessionTaxID;

        //    if (companyTaxID == null) companyTaxID = string.Empty;
        //    else this.IdentifiantFiscal = companyTaxID;
        //}

        //private async Task DisplayNoImage()
        //{

        //    SelectedLogoImageToDisplay = ImageHelper.LoadFromResource(new Uri("avares://GetStartedApp/Assets/Icons/NoImageImage.png"));

        //}
        //private async Task OnPickLogoImage()
        //{
        //    if (PickCompanyLogoImageFunc != null)
        //    {
        //        // save the path of image selected 
        //        _selectedCompanyLogoPath = await PickCompanyLogoImageFunc.Invoke();

        //        // we check if the user has selected an image path becuase sometimes it can close the window and not selectged an image
        //        // which can throw error and crash the app
        //        if (_selectedCompanyLogoPath != "")
        //        {
        //            // display the image selected after resizing it to reduce it memory taken in space
        //            // this is for the sake of improving performance
        //            SelectedLogoImageToDisplay = await ImageResizerHelper.ResizeImageAsync(_selectedCompanyLogoPath);

        //        }

        //    }
        //}

        //private async Task OnDeleteLogoImage()
        //{
        //    DisplayNoImage();
        //}

        private void GoBackToMakeSalePage()
        {
            mainWindowViewModel.CurrentPage=makeSaleViewModel;
        }

        private async void OnGenerateBls()
        {
            {

          //     var companyLogoConvertedToByteArray = ImageConverter.BitmapToByteArray(SelectedLogoImageToDisplay);
          //    
          //  var CompanyLogoAndLocationAreUpdatedSuccessfully = 
          //      await AccessToClassLibraryBackendProject.UpdateCompanyDetailsAsync(companyLogoConvertedToByteArray, CompanyName, CompanyLocation,ICE,NTaxProfessional,IdentifiantFiscal);
          //    
          //     if (!CompanyLogoAndLocationAreUpdatedSuccessfully) return;
          //
          //      AccessToClassLibraryBackendProject.
          //          GenerateBls(TableProductToPrintInBl, companyName, companyLogoConvertedToByteArray, companyLocation,ICE,NTaxProfessional,IdentifiantFiscal,_lastSaleClientID,_lastSaleClientName);
          //
          //
          //      GoBackToMakeSalePage();
            }

        }
    }
}
