using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestPDF.Infrastructure;
using System;
using System.Data;
using QuestPDF.Fluent;
using System.Globalization;
using QuestPDF.Helpers;
using System.IO;
using System.Diagnostics;

namespace SalesProductsManagmentSystemBusinessLayer
{
    public class clsBonCommandGenerator : ClsBonLivraisonGenerator
    {
        public string SupplierName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string BankAccount { get; set; } = string.Empty;
        public string FiscalIdentifier { get; set; } = string.Empty;
        public string RC { get; set; } = string.Empty;
        public string ICE { get; set; } = string.Empty;
        public string Patented { get; set; } = string.Empty;
        public string CNSS { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public override void ComposeHeader(IContainer container)
        {
            var titleStyle = TextStyle.Default.FontSize(12).SemiBold().FontColor(Colors.Blue.Medium).LineHeight(1.5f);


            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    if (My_CompanyLogo != null) column.Item().MaxHeight(3, Unit.Centimetre).Image(My_CompanyLogo);

                    column.Item().Text(text =>
                    {
                        text.EmptyLine();
                    });

                    column.Item().Text(text =>
                    {

                        text.Span($"{My_City} le: {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", new CultureInfo("fr-FR"))}\n\n\n").SemiBold();
                        text.Span($"Bon De Command : \n\n\n").Bold();
                        text.Span($"Mode De Paiment : ").Bold();
                        text.Span($"{SelectedPaymentMethod}");

                    });
                });

                row.RelativeItem().Column(column =>
                {
                    //column.Item().AlignCenter().Text($"Code Client: {ID_To_GenerateBillFor}").Style(titleStyle);
                    column.Item().AlignCenter().Text($"{SupplierName}").Style(titleStyle);
                    column.Item().AlignCenter().Text(Address).Style(titleStyle);
                   // column.Item().AlignCenter().Text(City_To_GenerateBillFor).Style(titleStyle);
                    column.Item().AlignCenter().Text(Email).Style(titleStyle);
                    column.Item().AlignCenter().Text(PhoneNumber).Style(titleStyle);
                    column.Item().AlignCenter().Text($"ICE  :  {ICE}").Style(titleStyle);

                });
            });
        }

        protected override void ComposeTable(IContainer container)
        {
            var TotalBalanceStyle = TextStyle.Default.FontSize(10).SemiBold().FontColor(Colors.Blue.Medium);

            container.Background(Colors.Grey.Lighten4).Padding(5).Table(table =>
            {
                // Step 1: Define columns
                table.ColumnsDefinition(columns =>
                {
                    // Give smaller space to the first few columns (they can overlap or compress)
                    columns.RelativeColumn();  // For "Code Article"
                    columns.ConstantColumn(100);  // For "Article"
                    columns.RelativeColumn(2);  // For "Quantité"

                    // Give more space to te last four columns to avoid overlap
                    columns.RelativeColumn();  // For "PU TTC"
                    columns.RelativeColumn();  // For "Remise"
                    columns.RelativeColumn();  // For "Montant"
               
                });

                // Step 2: Define header
                table.Header(header =>
                {
                    header.Cell().Padding(2).Element(HeaderCellStyle).AlignCenter().Text("Code Article");
                    header.Cell().Padding(2).Element(HeaderCellStyle).AlignCenter().Text("Article");
                    header.Cell().Padding(2).Element(HeaderCellStyle).AlignCenter().Text("Désignation");
                    header.Cell().Padding(2).Element(HeaderCellStyle).AlignCenter().Text("Qté");  
                    header.Cell().Padding(2).Element(HeaderCellStyle).AlignCenter().Text("PU TTC");            
                    header.Cell().Padding(2).Element(HeaderCellStyle).AlignCenter().Text("Montant");
  
                });


                // Step 3: Add rows and calculate total
                decimal Total_TTC = 0;
                decimal Total_HT = 0;
                decimal Total_TVA = 0;


                for (int i = 0; i < TableOfProductsBoughts.Rows.Count; i++)
                {
                    DataRow row = TableOfProductsBoughts.Rows[i];
                    decimal price = Convert.ToDecimal(row["Price"]);
                    int quantity = Convert.ToInt16(row["QuantityToCommand"]);
                    decimal Montant = price * quantity;

                    Total_TTC += Montant;

                    table.Cell().Padding(2).Element(DataCellStyle).Text(row["ProductId"]).FontSize(10);
                    table.Cell().Padding(2).Element(DataCellStyle).Text(row["ProductName"].ToString()).FontSize(10);
                    table.Cell().Padding(2).Element(DataCellStyle).Text(row["Description"].ToString()).FontSize(10);
                    table.Cell().Padding(2).Element(DataCellStyle).AlignRight().Text(quantity.ToString()).FontSize(10);         
                    table.Cell().Padding(2).Element(DataCellStyle).AlignRight().Text($"{price:N2} DH").FontSize(10); // Formatted price
                    table.Cell().Padding(2).Element(DataCellStyle).AlignRight().Text($"{Montant:N2} DH").FontSize(10); // Formatted Montant


                }

                Total_HT = Total_TTC / (1 + (TVA / 100));
                Total_TVA = Total_TTC - Total_HT;
                //  
                //  // Step 4: Add footer row1 for total
                //  // Step 4: Add footer row1 for total
                // Add footer row for Total HT
                table.Cell().Element(DataCellStyle).Text(""); // Empty cell for row number
                table.Cell().Element(DataCellStyle).Text(""); // Empty cell for row number
                table.Cell().Element(DataCellStyle).Text(""); // Empty cell for row number
                table.Cell().Element(DataCellStyle).Text(""); // Empty cell for article name
                table.Cell().Element(DataCellStyle).Text("Total HT:").AlignRight().Style(TotalBalanceStyle);
                table.Cell().Element(DataCellStyle).AlignRight().Text($"{Math.Round(Total_HT, 2, MidpointRounding.AwayFromZero):N2} DH").Style(TotalBalanceStyle);

                // Add footer row for TVA
                table.Cell().Element(DataCellStyle).Text(""); // Empty cell for row number
                table.Cell().Element(DataCellStyle).Text(""); // Empty cell for row number
                table.Cell().Element(DataCellStyle).Text(""); // Empty cell for row number
                table.Cell().Element(DataCellStyle).Text(""); // Empty cell for article name
                table.Cell().Element(DataCellStyle).Text("TVA:").AlignRight().Style(TotalBalanceStyle);
                table.Cell().Element(DataCellStyle).AlignRight().Text($"{Math.Round(Total_TVA, 2, MidpointRounding.AwayFromZero):N2} DH").Style(TotalBalanceStyle);

                // Add footer row for Total TTC
                table.Cell().Element(DataCellStyle).Text(""); // Empty cell for row number
                table.Cell().Element(DataCellStyle).Text(""); // Empty cell for row number
                table.Cell().Element(DataCellStyle).Text(""); // Empty cell for row number
                table.Cell().Element(DataCellStyle).Text(""); // Empty cell for article name
                table.Cell().Element(DataCellStyle).Text("Total TTC:").AlignRight().Style(TotalBalanceStyle);
                table.Cell().Element(DataCellStyle).AlignRight().Text($"{Math.Round(Total_TTC, 2, MidpointRounding.AwayFromZero):N2} DH").Style(TotalBalanceStyle);

            });



        }



        public clsBonCommandGenerator(int CompanyID, DataTable TableOfProductsBoughts, string SelectedPaymentMethodInFrench, decimal TVA, int SaleID, DateTime SalesTime,
            string SupplierName,string PhoneNumber , string Email, string BankAccount, string FiscalIdentifier, string Rc, string ICE, string Patented, string Cnss, string Addres)
           : base(CompanyID, TableOfProductsBoughts, SelectedPaymentMethodInFrench, TVA, SaleID, SalesTime)
        {
            this.SupplierName = SupplierName;
            this.PhoneNumber = PhoneNumber;
            this.Email = Email;
            this.BankAccount = BankAccount;
            this.FiscalIdentifier = FiscalIdentifier;
            this.RC = Rc;
            this.ICE = ICE;
            this.Patented = Patented;
            this.CNSS = Cnss;
            this.Address = Address;

        }


        public void GenerateBonCommand()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            //  var document = new BlsPdf(ProductSoldTable, companyName, companyLogo, companyLocation, ICE, ProfessionalTaxID, TaxID, lastSaleClientID, lastSaleClientName);

            string filePath = set_TheLocationWherePdf_IsgoingToBePrinted();

            this.GeneratePdf(filePath);

            // Open the PDF file
            openThePdfFile(filePath);
        }
    }
}
