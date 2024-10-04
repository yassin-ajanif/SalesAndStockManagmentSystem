﻿using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System;
using System.Data;
using System.Globalization;
using QuestPDF.Helpers;

namespace SalesProductsManagmentSystemBusinessLayer
{
    public class ClsInvoiceGenerator : ClsBonLivraisonGenerator
    {

            public int SaleID { get; set; } = 0;
            public int InvoiceID { get; set; } = 0;

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
                            text.Span($"Facture N°: {InvoiceID}\n").Bold();
                            text.Span($"Bon De Livraison N°: {SaleID}\n\n\n").Bold();
                            text.Span($"Mode De Paiement : ").Bold();
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

            public ClsInvoiceGenerator(DataTable TableOfProductsBoughts, int ClientID, string SelectedPaymentMethodInFrench, decimal TVA, int SaleID, int InvoiceID)
                : base(TableOfProductsBoughts, ClientID, SelectedPaymentMethodInFrench, TVA, SaleID)
            {
                this.SaleID = SaleID;
                this.InvoiceID = InvoiceID;
            }

            public ClsInvoiceGenerator(int CompanyID, DataTable TableOfProductsBoughts, string SelectedPaymentMethodInFrench, decimal TVA, int SaleID,int InvoiceID)
                : base(CompanyID, TableOfProductsBoughts, SelectedPaymentMethodInFrench, TVA, SaleID)
            {
                this.SaleID = SaleID;
                this.InvoiceID = InvoiceID;

        }

            public void GenerateInvoice_ForClient()
            {
                QuestPDF.Settings.License = LicenseType.Community;

                string filePath = set_TheLocationWherePdf_IsgoingToBePrinted();

                this.GeneratePdf(filePath);

                // Open the PDF file
                openThePdfFile(filePath);
            }

            public void GenerateInvoice_ForCompany()
            {
                QuestPDF.Settings.License = LicenseType.Community;

                string filePath = set_TheLocationWherePdf_IsgoingToBePrinted();

                this.GeneratePdf(filePath);

                // Open the PDF file
                openThePdfFile(filePath);
            }
        }
    
}