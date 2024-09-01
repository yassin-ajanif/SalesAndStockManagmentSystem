using Avalonia.Controls;
using Avalonia.Media.Imaging;
using GetStartedApp.ViewModels.ProductPages;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Reactive;
using MsBox.Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.VisualTree;
using GetStartedApp.ViewModels;




namespace GetStartedApp.Views.ProductPages
{
    public partial class AddProductView : ReactiveUserControl<AddProductViewModel>
    {


        public AddProductView()
        {
            InitializeComponent();
            RegisterShowMessageBoxDialogProductEvents();
 
        }


        private void RegisterShowMessageBoxDialogProductEvents()
        {
            this.WhenActivated(action => {

                action(ViewModel!.ShowMessageBoxDialog.RegisterHandler(ShowMessageBox));

                action(ViewModel!.ShowMessageBoxDialog_For_BarCodePrintingPersmission.RegisterHandler(ShowMessageBoxForBarCodePrintingPermission));

                action(ViewModel!.ShowBarCodePrinterPage.RegisterHandler(ShowBarCodePrinterView));

                }
            ) ;
        }
         
        private async Task ShowMessageBox(InteractionContext<string, Unit> context)
               {

            
           var window = this.GetVisualRoot() as Window;
           var dialog = new ShowMessageBoxContainer(context.Input);

            await dialog.ShowDialog(window);

          //  window.Close();          
            context.SetOutput(Unit.Default);

            // we won't close windows if the user is going to print barcode , becuase from this windows were going to open barcode widnows
            // closing that will cause crash in the program
            if (ViewModel!.isUserAllowedToPrintBarCodes()) return;

            window.Close();

               }

        private async Task ShowMessageBoxForBarCodePrintingPermission(InteractionContext<string, bool> context)
        {


            var window = this.GetVisualRoot() as Window;
            bool validatonBtnsAreVisible = true;
            var dialog = new ShowMessageBoxContainer(context.Input, validatonBtnsAreVisible);

            var messageToShow = context.Input;
           
            bool userWantToPrintBarCodes = await dialog.ShowDialog<bool>(window);
            bool userDosentWantToPrintBarCodes = !userWantToPrintBarCodes;


            context.SetOutput(userWantToPrintBarCodes);

           if(userDosentWantToPrintBarCodes) window.Close();

        }

        private async Task ShowBarCodePrinterView(InteractionContext<Unit, Unit> context)
        {
            var dialog = new DialogContainerView();
            int barCodeNumbers = ViewModel!.NumberOfBarcodes;

            // we don't allow a user in this case to increment the number of bar
            bool userCanEditNumberOfBarCodeToPrint = false;

            // CurrentDialogOpened = dialog;
            // Create an instance of AddOrEditOrKnowProductView and set it as the Content of the dialog
            dialog.Content = new BarCodeGeneratorView()
            {
                DataContext = new BarCodeGeneratorViewModel(ViewModel!.BarCodeSerieNumber, barCodeNumbers.ToString(), userCanEditNumberOfBarCodeToPrint)
            };

            var window = this.GetVisualRoot() as Window;
            if (window == null)
            {
                throw new InvalidOperationException("Cannot show dialog because this control is not contained within a Window.");
            }

             await dialog.ShowDialog<Unit>(window);

            context.SetOutput(Unit.Default);

            window.Close();

        }


        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);

            if (DataContext is AddProductViewModel myMainWindowViewModel)
            {
                myMainWindowViewModel.PickProductImageFunc = OnPickImageButtonClick;
            }
        }

        public async Task<string> OnPickImageButtonClick()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select Image";
            openFileDialog.Filters.Add(new FileDialogFilter() { Name = "Image Files", Extensions = { "png", "jpg", "jpeg", "gif" } });

            var result = await openFileDialog.ShowAsync((Window)this.VisualRoot);

            if (result != null && result.Length > 0)
            {
                // Handle the selected image file
                string selectedImagePath = result[0];
                // Do something with the selected image path, e.g., display it in an Image control

                return selectedImagePath;
            }

            return "";
        }



       


    }
}
