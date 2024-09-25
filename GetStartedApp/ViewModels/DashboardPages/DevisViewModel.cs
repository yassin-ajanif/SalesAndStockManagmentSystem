﻿using GetStartedApp.Models;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using Avalonia.Media.Imaging;
using System.Data;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using GetStartedApp.Helpers;

namespace GetStartedApp.ViewModels.DashboardPages
{
    public class DevisViewModel : MakeSaleViewModel,IDocument
    {
       
        // this info are for indivisual person
        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public string PhoneNumber { get; set; }
        public string email { get; set; }

        private int _clientOrCompanyID;
        private string _ClientOrCompanyName;

        DataTable TableOfProductsBoughts;
        CompanyInfo companyInfo;
        ClientInfo clientInfo;
        MainWindowViewModel mainWindowViewModel;


        public DevisViewModel
            (MainWindowViewModel mainWindowViewModel):base(mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;         

        }


        private void LoadMyCompanyInfo()
        {
            int myCompanyID = 1;
            companyInfo = AccessToClassLibraryBackendProject.LoadCompanyInfo(myCompanyID);
            
        }

        private void LoadClientInfo()
        {
            //ClientID = clientInfo.ClientID;
            //ClientName = clientInfo.ClientName;
            //PhoneNumber = clientInfo.PhoneNumber;
            //Email = clientInfo.email;
        }
        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(50);

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);

                    page.Footer().AlignCenter().Text(x =>
                    {
                        // Tax information with custom font size
                        x.Span($"ICE : {ICE} - N DE TAXE PROFESSIONELLE : {ICE} - IDENTIFIANT FISCAL : {IFS}")
                         .FontSize(10); // Set your desired font size here

                        // Add extra space
                        x.EmptyLine(); // Adds an empty line to create space
                        x.EmptyLine(); // Adds an empty line to create space

                        // Page number information
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
        }

        void ComposeHeader(IContainer container)
        {
            var titleStyle = TextStyle.Default.FontSize(12).SemiBold().FontColor(Colors.Blue.Medium).LineHeight(1.5f);


            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().MaxHeight(3, Unit.Centimetre).Image(ImageConverter.BitmapToByteArray(CompanyLogo));

                    column.Item().Text(text =>
                    {
                        text.EmptyLine();
                    });

                    column.Item().Text(text =>
                    {
                        //text.Span($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}").SemiBold();
                        //text.Span($"{DateTime.Now:d}");
                        // text.Span("yass is good and is that is good ");
                        text.Span($"{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("fr-FR"))}").SemiBold();

                    });
                });

                row.RelativeItem().Column(column =>
                {
                    column.Item().AlignCenter().Text(CompanyName).Style(titleStyle);
                    column.Item().AlignCenter().Text(SplitLocationAndPhoneNumber(CompanyLocation).Location).Style(titleStyle);
                    column.Item().AlignCenter().Text(SplitLocationAndPhoneNumber(CompanyLocation).Telephone).Style(titleStyle);
                    column.Item().AlignCenter().Text($"Code Client: {_clientOrCompanyID}").Style(titleStyle);
                    column.Item().AlignCenter().Text($"Nom de Client:  {_ClientOrCompanyName}").Style(titleStyle);
                });
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(40).Column(column =>
            {
                column.Spacing(5);

                column.Item().Element(ComposeTable);
            });
        }

        void ComposeTable(IContainer container)
        {
            var TotalBalanceStyle = TextStyle.Default.FontSize(10).SemiBold().FontColor(Colors.Blue.Medium);

            container.Table(table =>
            {
                // Step 1: Define columns
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(25);
                    columns.RelativeColumn(6);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                });

                // Step 2: Define header
                table.Header(header =>
                {
                    header.Cell().Element(HeaderCellStyle).Text("#");                  
                    header.Cell().Element(HeaderCellStyle).Text("Article");
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Prix");
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Quantité");
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Total");

                    header.Cell().Element(HeaderCellStyle).Text("CodeArtice");
                    header.Cell().Element(HeaderCellStyle).Text("Remise");
                    header.Cell().Element(HeaderCellStyle).Text("Montant");
                    header.Cell().Element(HeaderCellStyle).Text("TVA");
                });

                // Step 3: Add rows and calculate total
                decimal totalAmount = 0;

                for (int i = 0; i < TableOfProductsBoughts.Rows.Count; i++)
                {
                    DataRow row = TableOfProductsBoughts.Rows[i];
                    decimal price = Convert.ToDecimal(row["Price"]);
                    int quantity = Convert.ToInt32(row["Quantity"]);
                    decimal total = price * quantity;

                    totalAmount += total;

                    table.Cell().Element(DataCellStyle).Text((i + 1).ToString());
                    table.Cell().Element(DataCellStyle).Text(row["ProductName"].ToString());
                    table.Cell().Element(DataCellStyle).AlignRight().Text($"{price}DH");
                    table.Cell().Element(DataCellStyle).AlignRight().Text(quantity.ToString());
                    table.Cell().Element(DataCellStyle).AlignRight().Text($"{total}DH");
                }

                // Step 4: Add footer row for total
                table.Cell().Element(DataCellStyle).Text(""); // Empty cell for row number
                table.Cell().Element(DataCellStyle).Text(""); // Empty cell for article name
                table.Cell().Element(DataCellStyle).Text(""); // Empty cell for price
                table.Cell().Element(DataCellStyle).Text("Total:").AlignRight().Style(TotalBalanceStyle);
                table.Cell().Element(DataCellStyle).AlignRight().Text($"{totalAmount}DH").Style(TotalBalanceStyle);
            });



        }

        // Style methods
        static IContainer HeaderCellStyle(IContainer container)
        {
            return container.DefaultTextStyle(x => x.SemiBold())
                            .PaddingVertical(5)
                            .BorderBottom(1)
                            .BorderColor(Colors.Black);
        }

        static IContainer DataCellStyle(IContainer container)
        {
            return container.BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .PaddingVertical(5);
        }

        public DocumentMetadata GetMetadata()
        {
            return DocumentMetadata.Default;
        }

        public DocumentSettings GetSettings()
        {
            return DocumentSettings.Default;
        }

        public static string GetEuropeanTime()
        {
            try
            {
                string timeZoneId = "Central European Standard Time";
                DateTime utcNow = DateTime.UtcNow;
                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                DateTime europeanTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, timeZone);
                string formattedTime = europeanTime.ToString("MM/dd/yyyy HH:mm:ss");
                return $"Date: {formattedTime}";
            }
            catch (TimeZoneNotFoundException)
            {
                return "Invalid time zone ID.";
            }
            catch (InvalidTimeZoneException)
            {
                return "Invalid time zone data.";
            }
        }

        public static (string Location, string Telephone) SplitLocationAndPhoneNumber(string CompanyLocation)
        {
            string prefix = "TEL";
            string pattern = $@"{prefix}\s*(.+)";
            var match = Regex.Match(CompanyLocation, pattern, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                string location = CompanyLocation.Substring(0, match.Index).Trim();
                string telephone = match.Groups[1].Value.Trim();
                return (location, telephone);
            }

            return (CompanyLocation, string.Empty);
        }

        public void GenerateDevis_ForClient(DataTable ProductSoldTable, ClientInfo clientInfo)
        {
            QuestPDF.Settings.License = LicenseType.Community;
          //  var document = new BlsPdf(ProductSoldTable, companyName, companyLogo, companyLocation, ICE, ProfessionalTaxID, TaxID, lastSaleClientID, lastSaleClientName);

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(desktopPath, $"BilanAchats_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.pdf");


            // Generate and save the PDF
            new DevisViewModel(mainWindowViewModel).GeneratePdf(filePath);

            // Open the PDF file
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., file not found, no PDF viewer installed)
                Console.WriteLine($"Failed to open the file: {ex.Message}");
            }
        }

        public void GenerateDevis_ForCompany(DataTable ProductSoldTable, CompanyInfo companyInfo)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            //  var document = new BlsPdf(ProductSoldTable, companyName, companyLogo, companyLocation, ICE, ProfessionalTaxID, TaxID, lastSaleClientID, lastSaleClientName);

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(desktopPath, $"BilanAchats_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.pdf");


            // Generate and save the PDF
            new DevisViewModel(mainWindowViewModel).GeneratePdf(filePath);

            // Open the PDF file
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., file not found, no PDF viewer installed)
                Console.WriteLine($"Failed to open the file: {ex.Message}");
            }
        }

    }
}
