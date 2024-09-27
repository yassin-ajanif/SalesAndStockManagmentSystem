using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using QuestPDF.Helpers;
using System.ComponentModel.Design;
using SalesProductsManagmentSystemDataLayer;
using System.Data.SqlClient;


namespace SalesProductsManagmentSystemBusinessLayer
{
    public class ClsDevisGenerator : IDocument
    {

        // these info are about compnay client
        protected string My_ICE { get; set; } = string.Empty;
        protected string My_Patente { get; set; } = string.Empty;
        protected string My_IFS { get; set; } = string.Empty;
        protected byte[] My_CompanyLogo { get; set; } = null;  // or new byte[0];
        protected string My_RC { get; set; } = string.Empty;
        protected string My_CompanyName { get; set; } = string.Empty;
        protected string My_CompanyLocation { get; set; } = string.Empty;
        protected string My_Email { get; set; } = string.Empty;

        // Info about individual client or company to target
        protected int ID_To_GenerateBillFor { get; set; }  // Default value for int is 0
        protected string CompanyOrClientName_To_GenerateBillFor { get; set; } = string.Empty;
        protected string ICE_To_GenerateBillFor { get; set; } = string.Empty;
        protected string Patente_To_GenerateBillFor { get; set; } = string.Empty;
        protected string IFS_To_GenerateBillFor { get; set; } = string.Empty;
        protected byte[] CompanyLogo_To_GenerateBillFor { get; set; } = null;  // or new byte[0];
        protected string RC_To_GenerateBillFor { get; set; } = string.Empty;       
        protected string CompanyLocation_To_GenerateBillFor { get; set; } = string.Empty;
        protected string Email_To_GenerateBillFor { get; set; } = string.Empty;



        DataTable TableOfProductsBoughts;

        public ClsDevisGenerator(DataTable TableOfProductsBoughts,int ClientID)
        {
            this.TableOfProductsBoughts = TableOfProductsBoughts;

            LoadMyCompanyInfo();
            LoadInfosOfClientToDisplay(ClientID);
        }

        public ClsDevisGenerator(int CompanyID,DataTable TableOfProductsBoughts )
        {
            this.TableOfProductsBoughts = TableOfProductsBoughts;

            LoadMyCompanyInfo();
            LoadCompanyInfoToDisplay(CompanyID);
        }

        private void LoadMyCompanyInfo()
        {
                // Call the business layer to retrieve the SqlDataReader
                SqlDataReader reader = null;
                int myCompanyId = 1;

                try
                {
                    // Call the business layer function to get the SqlDataReader
                    reader = SalesProductsManagmentSystemBusinessLayer.ClsCompanies.RetrieveCompanyInfo(myCompanyId); // Assuming the business layer provides the reader

                    // Check if the reader has any rows (data)
                    if (reader.Read())
                    {
                        // Populate only the "My" properties with data from the reader
                        My_CompanyLogo = reader["CompanyLogo"] != DBNull.Value ? (byte[])reader["CompanyLogo"] : null;
                        My_CompanyName = reader.GetString(reader.GetOrdinal("CompanyName"));
                        My_CompanyLocation = reader.GetString(reader.GetOrdinal("CompanyLocation"));
                        My_ICE = reader["ICE"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("ICE")) : string.Empty;
                        My_IFS = reader["IFs"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("IFs")) : string.Empty;
                        My_Email = reader["Email"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("Email")) : string.Empty;
                        My_Patente = reader["Patente"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("Patente")) : string.Empty;
                        My_RC = reader["RC"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("RC")) : string.Empty;
                    }
                }
                finally
                {
                    // Ensure the reader is closed even in case of an exception
                    reader?.Close();
                }
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
                        x.Span($"ICE : {My_ICE} - N DE TAXE PROFESSIONELLE : {My_Patente} - IDENTIFIANT FISCAL : {My_IFS}")
                         .FontSize(10); // Set your desired font size her

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
                    if(My_CompanyLogo!=null) column.Item().MaxHeight(3, Unit.Centimetre).Image(My_CompanyLogo);

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
                    column.Item().AlignCenter().Text($"Code Client: {ID_To_GenerateBillFor}").Style(titleStyle);
                    column.Item().AlignCenter().Text($"{CompanyOrClientName_To_GenerateBillFor}").Style(titleStyle);
                    column.Item().AlignCenter().Text(CompanyLocation_To_GenerateBillFor).Style(titleStyle);
                    column.Item().AlignCenter().Text(Email_To_GenerateBillFor).Style(titleStyle);
                    column.Item().AlignCenter().Text($"ICE  :  {ICE_To_GenerateBillFor}").Style(titleStyle);

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
                    columns.ConstantColumn(25);       // For the "#" column
                    columns.RelativeColumn(4);        // For the "Article" column
                    columns.RelativeColumn(2);        // For the "Prix" column
                    columns.RelativeColumn(2);        // For the "Quantité" column
                    columns.RelativeColumn(2);        // For the "Total" column
                    columns.RelativeColumn(2);        // For the "CodeArticle" column
                    columns.RelativeColumn(2);        // For the "Remise" column
                    columns.RelativeColumn(2);        // For the "Montant" column
                    columns.RelativeColumn(2);        // For the "TVA" column
                });

                // Step 2: Define header
                table.Header(header =>
                {
                    header.Cell().Element(HeaderCellStyle).Text("#");
                    header.Cell().Element(HeaderCellStyle).Text("Article");
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Prix");
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Quantité");
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Total");
                    header.Cell().Element(HeaderCellStyle).Text("CodeArticle");
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Remise");
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("Montant");
                    header.Cell().Element(HeaderCellStyle).AlignRight().Text("TVA");
                });


                // Step 3: Add rows and calculate total
                decimal totalAmount = 0;

                for (int i = 0; i < TableOfProductsBoughts.Rows.Count; i++)
                {
                    DataRow row = TableOfProductsBoughts.Rows[i];
                    decimal price = Convert.ToDecimal(row["UnitPrice"]);
                    int quantity = Convert.ToInt32(row["QuantitySold"]);
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

        private void LoadInfosOfClientToDisplay(int ClientID)
        {
            int IDofChoosedClient = ClientID;
            string ClientName = string.Empty;
            string PhoneNumber = string.Empty;
            string Email = "";

            ClsClients.GetClientInfoById(IDofChoosedClient, ref ClientName, ref PhoneNumber, ref Email);

            ID_To_GenerateBillFor = ClientID;
            CompanyOrClientName_To_GenerateBillFor = ClientName;

        }

        // Method to load company info into the properties
        public void LoadCompanyInfoToDisplay(int companyId)
        {
            // Call the business layer to retrieve the SqlDataReader
            SqlDataReader reader = null;

            try
            {
                // Call the business layer function to get the SqlDataReader
                reader = SalesProductsManagmentSystemBusinessLayer.ClsCompanies.RetrieveCompanyInfo(companyId); // Assuming the business layer provides the reader

                // Check if the reader has any rows (data)
                if (reader.Read())
                {
                    // Populate the properties with data from the reader
                    ID_To_GenerateBillFor = reader.GetInt32(reader.GetOrdinal("CompanyID"));
                    CompanyLogo_To_GenerateBillFor = reader["CompanyLogo"] != DBNull.Value ? (byte[])reader["CompanyLogo"] : null;
                    CompanyOrClientName_To_GenerateBillFor = reader.GetString(reader.GetOrdinal("CompanyName"));
                    CompanyLocation_To_GenerateBillFor = reader.GetString(reader.GetOrdinal("CompanyLocation"));
                    ICE_To_GenerateBillFor = reader["ICE"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("ICE")) : string.Empty;
                    IFS_To_GenerateBillFor = reader["IFs"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("IFs")) : string.Empty;
                    Email_To_GenerateBillFor = reader["Email"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("Email")) : string.Empty;
                    Patente_To_GenerateBillFor = reader["Patente"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("Patente")) : string.Empty;
                    RC_To_GenerateBillFor = reader["RC"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("RC")) : string.Empty;
                }
            }
            finally
            {
                // Ensure the reader is closed even in case of an exception
                reader?.Close();
            }
        }

        public void GenerateDevis_ForClient(int ClientID)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            //  var document = new BlsPdf(ProductSoldTable, companyName, companyLogo, companyLocation, ICE, ProfessionalTaxID, TaxID, lastSaleClientID, lastSaleClientName);

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(desktopPath, $"BilanAchats_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.pdf");

           
            // Generate and save the PDF
            new ClsDevisGenerator(TableOfProductsBoughts,ClientID).GeneratePdf(filePath);

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

        public void GenerateDevis_ForCompany(int CompanyID)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            //  var document = new BlsPdf(ProductSoldTable, companyName, companyLogo, companyLocation, ICE, ProfessionalTaxID, TaxID, lastSaleClientID, lastSaleClientName);

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(desktopPath, $"BilanAchats_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.pdf");

            LoadCompanyInfoToDisplay(CompanyID);
            // Generate and save the PDF
            new ClsDevisGenerator(CompanyID,TableOfProductsBoughts).GeneratePdf(filePath);

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
