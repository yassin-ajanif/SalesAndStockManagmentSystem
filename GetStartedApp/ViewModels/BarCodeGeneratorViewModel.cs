using ReactiveUI;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using GetStartedApp.Helpers;
using System.IO;
//using Microsoft.CodeAnalysis.CSharp.Syntax;
using GetStartedApp.Helpers.CustomUIErrorAttributes;
using System.Reactive.Linq;
using Svg.Skia;


namespace GetStartedApp.ViewModels
{
    public class BarCodeGeneratorViewModel : ViewModelBase
    {

        private string _ProductIdConvertedToString;
        const int maxNumberOfRowsA4SheetCanHave = 100;
        const int maxNumberOfColumnsA4SheetCanHave = 100;
        const long maxBarCodeNumberCanBeGenerated = 1_000_000_000_000_000_000;

        private Bitmap _barCodeImagePreviewImage;
        public Bitmap BarCodeImagePreviewImage
        {
            get { return _barCodeImagePreviewImage; }
            set { this.RaiseAndSetIfChanged(ref _barCodeImagePreviewImage, value); }
        }

        private int _numRows;
        private string _entredNumRows;
        [PositiveIntRange(1, maxNumberOfRowsA4SheetCanHave, ErrorMessage = "أدخل رقمًا موجبًا أقل من 100")]  
        public string NumRows
        {
            get { return _entredNumRows; }
            set
            {
               
                DataEntryPropertyLoader.ConvertStringToIntAndLoadPrivateProperty(value, ref _numRows);

                this.RaiseAndSetIfChanged(ref _entredNumRows, value);
            }
        }


        private int _numColumns;
        private string _enteredNumColumns;
        [PositiveIntRange(1, maxNumberOfColumnsA4SheetCanHave, ErrorMessage = "أدخل رقمًا موجبًا أقل من 100")]
        public string NumColumns
        {
            get { return _enteredNumColumns; }
            set
            {

                DataEntryPropertyLoader.ConvertStringToIntAndLoadPrivateProperty(value, ref _numColumns);
                this.RaiseAndSetIfChanged(ref _enteredNumColumns, value);
               
            }
        }

        private float _spacing;
        private string _enteredSpacing;
        [PositiveFloatRange(0, maxNumberOfColumnsA4SheetCanHave, ErrorMessage = "أدخل رقمًا موجبًا أقل من 100 وبدون فاصلة ")]
        public string Spacing
        {
            get { return _enteredSpacing; }
            set
            {
                
                DataEntryPropertyLoader.ConvertStringToFloatAndLoadPrivateProperty(value, ref _spacing);
                this.RaiseAndSetIfChanged(ref _enteredSpacing, value);
            }
        }

        private int _startRow;
        private string _enteredStartRow;
        [PositiveIntRange(0, maxNumberOfColumnsA4SheetCanHave, ErrorMessage = "أدخل رقمًا موجبًا أقل من 100 وبدون فاصلة ")]
        public string StartRow
        {
            get { return _enteredStartRow; }
            set
            {
                
                DataEntryPropertyLoader.ConvertStringToIntAndLoadPrivateProperty_Allowing_ZeroValue(value, ref _startRow);
                this.RaiseAndSetIfChanged(ref _enteredStartRow, value);
            }
        }

        private int _startColumn;
        private string _enteredStartColumn;
        [PositiveIntRange(0, maxNumberOfColumnsA4SheetCanHave, ErrorMessage = "أدخل رقمًا موجبًا أقل من 100 وبدون فاصلة ")]
        public string StartColumn
        {
            get { return _enteredStartColumn; }
            set
            {
               
                DataEntryPropertyLoader.ConvertStringToIntAndLoadPrivateProperty_Allowing_ZeroValue(value, ref _startColumn);
                this.RaiseAndSetIfChanged(ref _enteredStartColumn, value);
            }
        }

        private int _numBarcodes;
        private string _enteredNumBarcodes;
        [PositiveIntRange(1, maxNumberOfColumnsA4SheetCanHave, ErrorMessage = "أدخل رقمًا موجبًا أقل من 100 وبدون فاصلة ")]
        public string NumBarcodes
        {
            get { return _enteredNumBarcodes; }
            set
            {
               
                DataEntryPropertyLoader.ConvertStringToIntAndLoadPrivateProperty(value, ref _numBarcodes);
                this.RaiseAndSetIfChanged(ref _enteredNumBarcodes, value);
            }
        }

        private bool _CanIsetBarCodeNumbers;
        public bool CanIsetBarCodeNumbers
        {
            get { return _CanIsetBarCodeNumbers; }
            set { this.RaiseAndSetIfChanged(ref _CanIsetBarCodeNumbers, value); }
        }

        
        private string _numberToPrintAsBarcodeNumber;
        [PositiveIntRange(1, maxBarCodeNumberCanBeGenerated, ErrorMessage = "أدخل رقمًا موجبًا أقل من ترليون وبدون فاصلة ")]
        public string NumberToPrintAsBarcodeNumber
        {
            get { return _numberToPrintAsBarcodeNumber; }
            set {  this.RaiseAndSetIfChanged(ref _numberToPrintAsBarcodeNumber, value);}
        }

        private float _offsetX;
        public string OffsetX
        {
            get { return _offsetX.ToString(); }
            set { if(float.TryParse(value, out float parsedValue) && parsedValue>=0) this.RaiseAndSetIfChanged(ref _offsetX, parsedValue); }
        }

        private float _offsetY;
        public string OffsetY
        {
            get { return _offsetY.ToString(); }
            set{ if (float.TryParse(value, out float parsedValue) && parsedValue>=0) this.RaiseAndSetIfChanged(ref _offsetY, parsedValue);  }
        }

        public ReactiveCommand<Unit, Unit> IncrementOffsetXCommand { get; }
        public ReactiveCommand<Unit, Unit> IncrementOffsetYCommand { get; }

        public ReactiveCommand<Unit, Unit> DecrementOffsetXCommand { get; }
        public ReactiveCommand<Unit, Unit> DecrementOffsetYCommand { get; }
        public ReactiveCommand<Unit, Unit> PrintPdfBarcodeCommand { get; }

        

        public BarCodeGeneratorViewModel(string barcodeNumber,string NumBarCodes,bool CanIchangeBarcodeNumbers)
        {
            _ProductIdConvertedToString = NumberToPrintAsBarcodeNumber = barcodeNumber;

            this.NumBarcodes = NumBarCodes;

            // in this case we enable or diable a possiblity for a user to edit or not edit barcode
            // this is depending on the context where the barcodegenerator is used
            // if the barcodegenrator is used with adding product, we set canIChangeBarCodeNumebr to false prevent making mistakes
            // so the number of products or quatity a user added to stock will be a number of barcode tickets he can print
            this.CanIsetBarCodeNumbers = CanIchangeBarcodeNumbers;

            settingDefaultBarcodeImageParameters();

            whenUserChangeAnyParameter_RenderNewImage();

            IncrementOffsetXCommand = ReactiveCommand.Create(IncrementOffsetX,CheckIfBarCodeUiParametersAreFilledCorrectly());
            IncrementOffsetYCommand = ReactiveCommand.Create(IncrementOffsetY,CheckIfBarCodeUiParametersAreFilledCorrectly());
            DecrementOffsetXCommand = ReactiveCommand.Create(DecrementOffsetX,CheckIfBarCodeUiParametersAreFilledCorrectly());
            DecrementOffsetYCommand = ReactiveCommand.Create(DecrementOffsetY, CheckIfBarCodeUiParametersAreFilledCorrectly());

            PrintPdfBarcodeCommand = ReactiveCommand.Create(PrintPdfBarcode, CheckIfBarCodeUiParametersAreFilledCorrectly());
        }

        private void settingDefaultBarcodeImageParameters()
        {
           NumRows = "16";
           NumColumns = "7";
           Spacing = "2";
           StartRow = "0";
           StartColumn = "0";
           NumBarcodes = NumBarcodes;
           OffsetX = "0";
           OffsetY = "0";

        }

        private bool isValidBarCodeNumber()
        {
            return !string.IsNullOrEmpty(NumberToPrintAsBarcodeNumber)
                && !string.IsNullOrWhiteSpace(NumberToPrintAsBarcodeNumber)
                &&  long.TryParse(NumberToPrintAsBarcodeNumber, out long parsedBarcodeNumber) 
                && parsedBarcodeNumber > 0 
                && parsedBarcodeNumber<maxBarCodeNumberCanBeGenerated;
        }
      public void whenUserChangeAnyParameter_RenderNewImage()
      {
            // i split this whenAny value properties to watch becuase it seems theere is a bug
            // in the when any value function that throws an error red line when properties are bigger tahn 7

            this.WhenAnyValue(
                x => x.NumRows,
                x => x.NumColumns,
                x => x.Spacing)
                .Where(tuple => isValidBarCodeNumber())
                .Subscribe(_ => GenerateBarcodeImage());

            this.WhenAnyValue(
                x => x.StartRow,
                x => x.StartColumn,
                x => x.NumBarcodes)
                .Where(tuple => isValidBarCodeNumber())
                .Subscribe(_ => GenerateBarcodeImage());


            this.WhenAnyValue(
                 x => x.NumberToPrintAsBarcodeNumber,
                 x => x.OffsetX,
                 x => x.OffsetY)
                 .Where(tuple => isValidBarCodeNumber()) // Use the custom function here
                 .Subscribe(_ => GenerateBarcodeImage());
   

        }

        private bool AreAllPropertiesAttributeValid()
        {
            // we're chckeing all the attributed assigne to this properties 
            // so if one is raising an error it won't call render a new image
            return UiAttributeChecker.AreThesesAttributesPropertiesValid
                (this,
                nameof(NumRows),
                nameof(NumColumns),
                nameof(StartRow),
                nameof(StartColumn),
                nameof(NumBarcodes),
                nameof(Spacing)) 
                && 
                isValidBarCodeNumber();
        }
        private IObservable<bool> CheckIfBarCodeUiParametersAreFilledCorrectly()
        {
            var AreBarCodeUiParametersAreFilledCorrectly = this.WhenAnyValue(
                x => x.NumRows,
                x => x.NumColumns,
                x => x.StartRow,
                x => x.StartColumn,
                x => x.NumBarcodes,
                x => x.OffsetX,
                x => x.OffsetY,
                x => x.Spacing,
                x => x.NumberToPrintAsBarcodeNumber,
                (numRows, numColumns, startRow, startColumn, numBarcodes, offsetX, offsetY, spacing,barcodeNumber) =>

                !string.IsNullOrEmpty(numRows) &&
                !string.IsNullOrEmpty(numColumns) &&
                !string.IsNullOrEmpty(startRow) &&
                !string.IsNullOrEmpty(startColumn) &&
                !string.IsNullOrEmpty(numBarcodes) &&
                !string.IsNullOrEmpty(offsetX) &&
                !string.IsNullOrEmpty(offsetY) &&
                !string.IsNullOrEmpty(spacing)&&
                AreAllPropertiesAttributeValid()
            );
            return AreBarCodeUiParametersAreFilledCorrectly;
        }


    private void IncrementOffsetX()
        {
            OffsetX = (float.Parse(OffsetX) + 2).ToString();
        }

        private void IncrementOffsetY()
        {
            OffsetY = (float.Parse(OffsetY) + 2).ToString();
        }

        private void DecrementOffsetX()
        {
            OffsetX = (float.Parse(OffsetX) - 2).ToString();
        }

        private void DecrementOffsetY()
        {
            OffsetY = (float.Parse(OffsetY) - 2).ToString();
        }

        public void PrintPdfBarcode()
        {
            SalesProductsManagmentSystemBusinessLayer.ClsBarCodeManager.
               GenerateBarCodePdf(NumberToPrintAsBarcodeNumber, _numRows, _numColumns, _spacing, _startRow, _startColumn, _numBarcodes, _offsetX, _offsetY);
        }

        private static Bitmap ConvertSkBitmapToBitmap(SKBitmap skBitmap)
        {
            using var stream = new MemoryStream();
            using var skImage = SKImage.FromBitmap(skBitmap);
            using var data = skImage.Encode(SKEncodedImageFormat.Png, 100);
            data.SaveTo(stream);
            stream.Position = 0;
            return new Bitmap(stream);
        }

        public void GenerateBarcodeImage()
        {
            // if a user has entred a wrong input data in one of the input we don't call the image barcode maker to not fall into errors
            if(AreAllPropertiesAttributeValid()) { 

            //getting the image previiw from class libary function
           SKBitmap BarCodeImagePreviewImage_InSkBitmap = SalesProductsManagmentSystemBusinessLayer.
                ClsBarCodeManager.AddBarcodeToPreview(NumberToPrintAsBarcodeNumber, _numRows,_numColumns,_spacing,_startRow,_startColumn,_numBarcodes,_offsetX,_offsetY);
            // converted to bitmap to bind it to avalonia
           BarCodeImagePreviewImage = ConvertSkBitmapToBitmap(BarCodeImagePreviewImage_InSkBitmap);

            }

        }
      
      
    }


}
