using QuestPDF.Infrastructure;
using System;
using System.Data;
using QuestPDF.Fluent;
using System.Globalization;
using QuestPDF.Helpers;


namespace SalesProductsManagmentSystemBusinessLayer
{
    public class ClsBonLivraisonGenerator : ClsDevisGenerator
    {
        public int SaleID { get; set; } = 0;

        protected virtual void ComposeHeader(IContainer container)
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
                        text.Span($"Bon De Livraison N°: {SaleID}\n\n\n").Bold();
                        text.Span($"Mode De Paiment : ").Bold();
                        text.Span($"{SelectedPaymentMethod}");

                    });
                });

                row.RelativeItem().Column(column =>
                {
                    column.Item().AlignCenter().Text($"Code Client: {ID_To_GenerateBillFor}").Style(titleStyle);
                    column.Item().AlignCenter().Text($"{CompanyOrClientName_To_GenerateBillFor}").Style(titleStyle);
                    column.Item().AlignCenter().Text(CompanyLocation_To_GenerateBillFor).Style(titleStyle);
                    column.Item().AlignCenter().Text(City_To_GenerateBillFor).Style(titleStyle);
                    column.Item().AlignCenter().Text(Email_To_GenerateBillFor).Style(titleStyle);
                    column.Item().AlignCenter().Text(PhoneNumber_To_GenerateBillFor).Style(titleStyle);
                    column.Item().AlignCenter().Text($"ICE  :  {ICE_To_GenerateBillFor}").Style(titleStyle);

                });
            });
        }

        public ClsBonLivraisonGenerator(DataTable TableOfProductsBoughts, int ClientID, string SelectedPaymentMethodInFrench, decimal TVA, int SaleID)
            :base(TableOfProductsBoughts,ClientID, SelectedPaymentMethodInFrench, TVA)
        {
            
        }

        public ClsBonLivraisonGenerator(int CompanyID, DataTable TableOfProductsBoughts, int ClientID, string SelectedPaymentMethodInFrench, decimal TVA, int SaleID)
           : base(TableOfProductsBoughts, ClientID, SelectedPaymentMethodInFrench, TVA)
        {
           

        }

       public void GenerateBlivraison_ForClient(int clientID, decimal TVA, int SaleID)
        {
            this.SaleID = SaleID;

            GenerateDevis_ForClient(clientID, TVA);
        }

        public void GenerateBlivraison_ForCompany(int companyID, decimal TVA, int SaleID)
        {
            this.SaleID = SaleID;
        }

    }
}
