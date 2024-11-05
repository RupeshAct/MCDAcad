using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Word;
using wd = Microsoft.Office.Interop.Word;
//using IBM.Data.DB2;
using MySql.Data.MySqlClient;
using System.Data.SqlTypes;


namespace MCD
{
    class ValidateReport
    {
        public void validReport(string APP_ID, string Filename)
        {
            bool retval = false;
            MySqlConnection con = new MySqlConnection(FunctionsNvar.Constr);
            try
            {
                con.Open();
            }
            catch (Exception ex)
            {

                System.Windows.Forms.MessageBox.Show("Server Connection Not found please contact administrator \n error: " + ex.StackTrace, "MCD Building Plan",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }
            Application WordApp = new Application();
            WordApp.Visible = false;
            object readOnly = false;
            object isVisible = true;
            object missing = System.Reflection.Missing.Value;
            Document doc = WordApp.Documents.Add(ref missing, ref missing, ref missing, ref isVisible);
            object savechanges = false;
            doc.PageSetup.Orientation = WdOrientation.wdOrientLandscape;
            doc.Sections[1].Borders.Enable = 1;
            Object start = Type.Missing;
            Object end = Type.Missing;
            Object unit = Type.Missing;
            Object count = Type.Missing;
            doc.Range(ref start, ref end).
            Delete(ref unit, ref count);
            start = 0;
            end = 0;
            object oEndOfDoc = "\\endofdoc";

            string imagePath = @"D:\Old_File\GTACAD\Src\Logo (2).jpg";

            Range rng = doc.Range(ref start, ref end);
            rng.InsertParagraphAfter();
            rng.InlineShapes.AddPicture(imagePath, ref missing, ref missing, ref missing);
            rng.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            //imagePath = @"D:\MCD\src\mcd-hindi.bmp";
            ////rng.InsertParagraphBefore();
            //rng.InsertParagraphAfter();
            //rng.InlineShapes.AddPicture(imagePath, ref missing, ref missing, ref missing);

            //rng.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            //rng.InsertParagraphBefore(); 
            //rng.InsertParagraphAfter();

            rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            //rng.InsertParagraphBefore();
            rng.InsertParagraphAfter();
            rng.Paragraphs.Add(ref missing);
            //rng.InsertParagraphAfter();
            rng.Text = "Act Global Pvt Limited";

            rng.Font.Name = "Verdana";
            //rng.Font.Name = "Cambria";
            rng.Font.Bold = 1;
            rng.Font.Size = 16;
            rng.Font.Color = WdColor.wdColorAqua;
            //rng.Font.Color = WdColor.wdColorPaleBlue;
            rng.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            //rng.Font.Position = 1;

            //rng.InsertParagraphBefore ();
            rng.InsertParagraphAfter();
            rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            //rng.InlineShapes.AddHorizontalLineStandard(ref missing);

            object orng = rng;
            InlineShape horizontalLine = doc.InlineShapes.AddHorizontalLineStandard(ref orng);
            horizontalLine.Width = 400;
            rng.Font.Color = WdColor.wdColorAqua;
            rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            //rng.InsertParagraphAfter();
            rng.InsertParagraphAfter();
            MySqlCommand AppCmd = new MySqlCommand( ";SELECT * FROM file_watch where FR_ID_VER ='" + APP_ID + "';commit;", con);
            MySqlDataReader Appreader = AppCmd.ExecuteReader();
            int buildTypeId = 0;
            bool appRead = false;
            while (Appreader.Read())
            {
                buildTypeId = int.Parse(Appreader.GetValue(4).ToString());
                appRead = true;
            }
            if (appRead == false)
            {
                WordApp.Quit(ref savechanges, ref  missing, ref missing);
                return;
            }
            Appreader.Close();
            string ID = APP_ID;
            ID = ID.Remove(ID.Length - 2);
            //MySqlCommand PropCmd = new MySqlCommand( ";SELECT * FROM file_record where FR_ID = '" + ID + "';commit;", con);
            //MySqlDataReader Propreader = PropCmd.ExecuteReader();
            //if (Propreader.Read() == false)
            //{
            //    WordApp.Quit(ref savechanges, ref  missing, ref missing);
            //    return;

            //}
            //MySqlCommand DwgCmd = new MySqlCommand( ";SELECT * FROM file where FR_ID = '" + ID + "'  order by  DWG_VER DESC;commit;", con);
            //MySqlDataReader Dwgreader = DwgCmd.ExecuteReader();
            //if (Dwgreader.Read() == false)
            //{
            //    WordApp.Quit(ref savechanges, ref  missing, ref missing);
            //    return;

            //}
            MySqlCommand Cmd1 = new MySqlCommand(";select pid from file_watch where fr_id_ver ='" + APP_ID + "';commit;", con);
            int pid = Convert.ToInt16(Cmd1.ExecuteScalar());
            object objAutoFitFixed2 = WdAutoFitBehavior.wdAutoFitWindow;
            Table tbl2 = doc.Tables.Add(rng, 4, 4, ref missing, ref objAutoFitFixed2);
            tbl2.Rows.HeightRule = WdRowHeightRule.wdRowHeightAuto;
            tbl2.Range.Font.Size = 8;
            Object style = "Table Grid 1";
            tbl2.set_Style(ref style);

            //-->>Included Two New columns Architect Name and Architect CA No in the Report on 24th Sept 2013 By Kiran Bishaj.

            MySqlCommand PropCmd = new MySqlCommand(";SELECT * FROM file_record where FR_ID = '" + ID + "';commit;", con);
            MySqlDataReader Propreader = PropCmd.ExecuteReader();
            if (Propreader.Read() == false)
            {
                WordApp.Quit(ref savechanges, ref missing, ref missing);
                return;

            }
            tbl2.Cell(1, 1).Range.Text = "Architect Name :";
            tbl2.Cell(1, 1).Range.Bold = 1;
            tbl2.Cell(1, 2).Range.Text = Propreader.GetValue(15).ToString();
            tbl2.Cell(1, 3).Range.Text = "Architect CA No :";
            tbl2.Cell(1, 3).Range.Bold = 1;
            tbl2.Cell(1, 4).Range.Text = Propreader.GetValue(16).ToString();

            tbl2.Cell(2, 1).Range.Text = "Applicant Name :";
            tbl2.Cell(2, 1).Range.Bold = 1;
            tbl2.Cell(2, 2).Range.Text = Propreader.GetValue(15).ToString();
            tbl2.Cell(2, 3).Range.Text = "Address :";
            tbl2.Cell(2, 3).Range.Bold = 1;
            tbl2.Cell(2, 4).Range.Text = Propreader.GetValue(11).ToString();
            tbl2.Cell(3, 1).Range.Text = "Building Type :";
            tbl2.Cell(3, 1).Range.Bold = 1;
            Propreader.Close();

            MySqlCommand DwgCmd = new MySqlCommand(";SELECT * FROM file where FR_ID = '" + ID + "'  order by  DWG_VER DESC;commit;", con);
            MySqlDataReader Dwgreader = DwgCmd.ExecuteReader();
            if (Dwgreader.Read() == false)
            {
                WordApp.Quit(ref savechanges, ref missing, ref missing);
                return;

            }

            if (buildTypeId == 101)
            {
                if (pid == 1)
                {
                    tbl2.Cell(3, 2).Range.Text = "Residential ";
                }
                if (pid == 2)
                {
                    tbl2.Cell(3, 2).Range.Text = "Residential_CC ";
                }
                if (pid == 3)
                {
                    tbl2.Cell(3, 2).Range.Text = "Residential_Revised ";
                }
                if (pid == 4)
                {
                    tbl2.Cell(3, 2).Range.Text = "Residential_Regularized ";
                }
                if (pid == 5)
                {
                    tbl2.Cell(3, 2).Range.Text = "Residential_AA ";
                }
                if (pid == 6)
                {
                    tbl2.Cell(3, 2).Range.Text = "Residential_REVDN ";
                }
                if (pid == 7)
                {
                    tbl2.Cell(3, 2).Range.Text = "Residential_SARAL_Revise ";
                }
            }
            if (buildTypeId == 102)
            {

                if (pid == 1)
                {
                    tbl2.Cell(3, 2).Range.Text = "URC";
                }
                if (pid == 2)
                {
                    tbl2.Cell(3, 2).Range.Text = "URC_CC ";
                }
                if (pid == 3)
                {
                    tbl2.Cell(3, 2).Range.Text = "URC_Revised ";
                }
                if (pid == 4)
                {
                    tbl2.Cell(3, 2).Range.Text = "URC_Regularized ";
                }
                if (pid == 5)
                {
                    tbl2.Cell(3, 2).Range.Text = "URC_AA ";
                }
                if (pid == 6)
                {
                    tbl2.Cell(3, 2).Range.Text = "URC_REVDN ";
                }
                if (pid == 7)
                {
                    tbl2.Cell(3, 2).Range.Text = "URC_SARAL_Revise ";
                }
            }
            if (buildTypeId == 103)
            {

                if (pid == 1)
                {
                    tbl2.Cell(3, 2).Range.Text = "Village Abadi ";
                }
                if (pid == 2)
                {
                    tbl2.Cell(3, 2).Range.Text = "Village Abadi_CC ";
                }
                if (pid == 3)
                {
                    tbl2.Cell(3, 2).Range.Text = "Village Abadi_Revised ";
                }
                if (pid == 4)
                {
                    tbl2.Cell(3, 2).Range.Text = "Village Abadi_Regularized ";
                }
                if (pid == 5)
                {
                    tbl2.Cell(3, 2).Range.Text = "Village Abadi_AA ";
                }
                if (pid == 6)
                {
                    tbl2.Cell(3, 2).Range.Text = "Village Abadi_REVDN ";
                }
                if (pid == 7)
                {
                    tbl2.Cell(3, 2).Range.Text = "Village Abadi_SARAL_Revise ";
                }
            }
            if (buildTypeId == 104)
            {

                if (pid == 1)
                {
                    tbl2.Cell(3, 2).Range.Text = "City Area ";
                }
                if (pid == 2)
                {
                    tbl2.Cell(3, 2).Range.Text = "City Area _CC ";
                }
                if (pid == 3)
                {
                    tbl2.Cell(3, 2).Range.Text = "City Area _Revised ";
                }
                if (pid == 4)
                {
                    tbl2.Cell(3, 2).Range.Text = "City Area _Regularized ";
                }
                if (pid == 5)
                {
                    tbl2.Cell(3, 2).Range.Text = "City Area _AA ";
                }
                if (pid == 6)
                {
                    tbl2.Cell(3, 2).Range.Text = "City Area _REVDN ";
                }
                if (pid == 7)
                {
                    tbl2.Cell(3, 2).Range.Text = "City Area _SARAL_Revise ";
                }
            }
            //if (buildTypeId == 10185)
            //{

            //    if (pid == 1)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Lutyens Bungalow Zone Area ";
            //    }
            //    if (pid == 2)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Lutyens Bungalow Zone Area _CC ";
            //    }
            //    if (pid == 3)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Lutyens Bungalow Zone Area _Revised ";
            //    }
            //    if (pid == 4)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Lutyens Bungalow Zone Area _Regularized ";
            //    }
            //    if (pid == 5)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Lutyens Bungalow Zone Area _AA ";
            //    }
            //    if (pid == 6)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Lutyens Bungalow Zone Area _REVDN ";
            //    }
            //    if (pid == 7)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Lutyens Bungalow Zone Area _SARAL_Revise ";
            //    }
            //}
            //if (buildTypeId == 10186)
            //{

            //    if (pid == 1)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Civil Line Bungalow Area ";
            //    }
            //    if (pid == 2)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Civil Line Bungalow Area _CC ";
            //    }
            //    if (pid == 3)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Civil Line Bungalow Area _Revised ";
            //    }
            //    if (pid == 4)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Civil Line Bungalow Area _Regularized ";
            //    }
            //    if (pid == 5)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Civil Line Bungalow Area _AA ";
            //    }
            //    if (pid == 6)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Civil Line Bungalow Area _REVDN ";
            //    }
            //    if (pid == 7)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Civil Line Bungalow Area _SARAL_Revise ";
            //    }
            //}
            if (buildTypeId == 105)
            {

                if (pid == 1)
                {
                    tbl2.Cell(3, 2).Range.Text = "Standard Plan ";
                }
                if (pid == 2)
                {
                    tbl2.Cell(3, 2).Range.Text = "Standard Plan_CC ";
                }
                if (pid == 3)
                {
                    tbl2.Cell(3, 2).Range.Text = "Standard Plan_Revised ";
                }
                if (pid == 4)
                {
                    tbl2.Cell(3, 2).Range.Text = "Standard Plan_Regularized ";
                }
                if (pid == 5)
                {
                    tbl2.Cell(3, 2).Range.Text = "Standard Plan_AA ";
                }
                if (pid == 6)
                {
                    tbl2.Cell(3, 2).Range.Text = "Standard Plan_REVDN ";
                }
                if (pid == 7)
                {
                    tbl2.Cell(3, 2).Range.Text = "Standard Plan_SARAL_Revise ";
                }
            }
            if (buildTypeId == 106)
            {

                if (pid == 1)
                {
                    tbl2.Cell(3, 2).Range.Text = "Residential Group Housing ";
                }
                if (pid == 2)
                {
                    tbl2.Cell(3, 2).Range.Text = "Residential Group Housing_CC ";
                }
                if (pid == 3)
                {
                    tbl2.Cell(3, 2).Range.Text = "Residential Group Housing_Revised ";
                }
                if (pid == 4)
                {
                    tbl2.Cell(3, 2).Range.Text = "Residential Group Housing_Regularized ";
                }
                if (pid == 5)
                {
                    tbl2.Cell(3, 2).Range.Text = "Resedential Group Housing_AA ";
                }
                if (pid == 6)
                {
                    tbl2.Cell(3, 2).Range.Text = "Resedential Group Housing_REVDN ";
                }
                if (pid == 7)
                {
                    tbl2.Cell(3, 2).Range.Text = "Resedential Group Housing_SARAL_Revise ";
                }
            }

            if (buildTypeId == 107)
            {
                if (pid == 1)
                {
                    tbl2.Cell(3, 2).Range.Text = "Notified-Commercial ";
                }
                if (pid == 2)
                {
                    tbl2.Cell(3, 2).Range.Text = "Notified-Commercial_CC ";
                }
                if (pid == 3)
                {
                    tbl2.Cell(3, 2).Range.Text = "Notified-Commercial_Revised ";
                }
                if (pid == 4)
                {
                    tbl2.Cell(3, 2).Range.Text = "Notified-Commercial_Regularized ";
                }
                if (pid == 5)
                {
                    tbl2.Cell(3, 2).Range.Text = "Notified-Commercial_AA ";
                }
                if (pid == 6)
                {
                    tbl2.Cell(3, 2).Range.Text = "Notified-Commercial_REVDN ";
                }
                if (pid == 7)
                {
                    tbl2.Cell(3, 2).Range.Text = "Notified-Commercial_SARAL_Revise ";
                }
            }

            if (buildTypeId == 108)
            {
                if (pid == 1)
                {
                    tbl2.Cell(3, 2).Range.Text = "Notified - MLU ";
                }
                if (pid == 2)
                {
                    tbl2.Cell(3, 2).Range.Text = "Notified - MLU_CC ";
                }
                if (pid == 3)
                {
                    tbl2.Cell(3, 2).Range.Text = "Notified - MLU_Revised ";
                }
                if (pid == 4)
                {
                    tbl2.Cell(3, 2).Range.Text = "Notified - MLU_Regularized ";
                }
                if (pid == 5)
                {
                    tbl2.Cell(3, 2).Range.Text = "Notified - MLU_AA ";
                }
                if (pid == 6)
                {
                    tbl2.Cell(3, 2).Range.Text = "Notified - MLU_REVDN ";
                }
                if (pid == 7)
                {
                    tbl2.Cell(3, 2).Range.Text = "Notified - MLU_SARAL_Revise ";
                }
            }


            else if (buildTypeId == 117)
            {
                if (pid == 1)
                {
                    tbl2.Cell(3, 2).Range.Text = "Industrial ";
                }
                if (pid == 2)
                {
                    tbl2.Cell(3, 2).Range.Text = "Industrial_CC ";
                }
                if (pid == 3)
                {
                    tbl2.Cell(3, 2).Range.Text = "Industrial_Revised ";
                }
                if (pid == 4)
                {
                    tbl2.Cell(3, 2).Range.Text = "Industrial_Regularized ";
                }
                if (pid == 5)
                {
                    tbl2.Cell(3, 2).Range.Text = "Industrial_AA ";
                }
                if (pid == 6)
                {
                    tbl2.Cell(3, 2).Range.Text = "Industrial_REVDN ";
                }
                if (pid == 7)
                {
                    tbl2.Cell(3, 2).Range.Text = "Industrial_SARAL_Revise";
                }
            }
            tbl2.Cell(3, 3).Range.Text = "Plot Area :";
            tbl2.Cell(3, 3).Range.Bold = 1;
            tbl2.Cell(3, 4).Range.Text = Dwgreader.GetValue(5).ToString();
            tbl2.Cell(4, 1).Range.Text = "Application ID :";
            tbl2.Cell(4, 1).Range.Bold = 1;
            tbl2.Cell(4, 2).Range.Text = ID;
            tbl2.Cell(4, 3).Range.Text = "Date :";
            tbl2.Cell(4, 3).Range.Bold = 1;
            tbl2.Cell(4, 4).Range.Text = Dwgreader.GetMySqlDateTime(8).ToString();
            tbl2.Cell(5, 1).Range.Text = "Drawing Name :";
            tbl2.Cell(5, 1).Range.Bold = 1;
            tbl2.Cell(5, 2).Range.Text = Dwgreader.GetValue(10).ToString();
            //rng.InsertParagraphAfter();
            tbl2.Cell(5, 3).Range.Text = " In order/Not in order :";
            tbl2.Cell(5, 3).Range.Bold = 1;
            tbl2.Cell(5, 4).Range.Text = "Not in order";
            //<<--Included Two New columns Architect Name and Architect CA No in the Report on 24th Sept 2013 By Kiran Bishaj.

            /*******************************for summary Table******************/

            Paragraph oPara4;
            rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            object parang = rng;
            oPara4 = doc.Content.Paragraphs.Add(ref parang);
            oPara4.Range.InsertParagraphBefore();
            oPara4.Range.Text = "Validation errors";
            oPara4.Range.Font.Underline = WdUnderline.wdUnderlineSingle;
            oPara4.Range.Font.Size = 16;
            oPara4.Range.Font.Color = WdColor.wdColorDarkRed;
            oPara4.Range.InsertParagraphAfter();

            System.IO.StreamReader fname = new System.IO.StreamReader(Filename);
            int no = 0;
            while (fname.EndOfStream == false)
            {
                string error = fname.ReadLine();
                no++;
                rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                parang = rng;
                oPara4 = doc.Content.Paragraphs.Add(ref parang);
                //oPara4.Range.InsertParagraphBefore();
                oPara4.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
                oPara4.BaseLineAlignment = WdBaselineAlignment.wdBaselineAlignAuto;
                oPara4.Range.Text = no.ToString() + ") " + error;
                oPara4.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                oPara4.Range.Font.Size = 11;
                oPara4.Range.Font.Color = WdColor.wdColorAutomatic;
                oPara4.Range.InsertParagraphAfter();
            }
            fname.Close();
            //********To export pdf****************

            string ver = APP_ID.Substring(APP_ID.Length - 2);
            try
            {
                string paramExportFilePath = "\\\\192.168.1.14\\Shared\\Validation\\fileprocessed\\" + ID + "_" + ver + "_ValidationReport.PDF";
                string paramExportFilePath2 = "\\\\192.168.1.14\\Shared\\Validation\\fileprocessed\\" + Dwgreader.GetValue(1).ToString() + "-" + ID + "_" + ver + "_ValidationReport.PDF";
                WdExportFormat paramExportFormat = WdExportFormat.wdExportFormatPDF;
                bool paramOpenAfterExport = false;
                WdExportOptimizeFor paramExportOptimizeFor = WdExportOptimizeFor.wdExportOptimizeForPrint;
                WdExportRange paramExportRange = WdExportRange.wdExportAllDocument;
                int paramStartPage = 0;
                int paramEndPage = 0;
                WdExportItem paramExportItem = WdExportItem.wdExportDocumentContent;
                bool paramIncludeDocProps = true;
                bool paramKeepIRM = true;
                WdExportCreateBookmarks paramCreateBookmarks = WdExportCreateBookmarks.wdExportCreateWordBookmarks;
                bool paramDocStructureTags = true;
                bool paramBitmapMissingFonts = true;
                bool paramUseISO19005_1 = false;

                doc.ExportAsFixedFormat(paramExportFilePath,
                    paramExportFormat, paramOpenAfterExport,
                    paramExportOptimizeFor, paramExportRange, paramStartPage,
                    paramEndPage, paramExportItem, paramIncludeDocProps,
                    paramKeepIRM, paramCreateBookmarks, paramDocStructureTags,
                    paramBitmapMissingFonts, paramUseISO19005_1,
                    ref missing);
                System.IO.File.Copy(paramExportFilePath, paramExportFilePath2, true);
                switch (pid)
                {
                    case 1:
                        //System.IO.File.Copy(paramExportFilePath, "D:\\To-Erp\\" + ID + "_" + ver + "_ValidationReport.PDF", true);
                        System.IO.File.Copy(paramExportFilePath, "\\\\192.168.1.14\\Shared\\Validation\\fileprocessed\\" + ID + "_" + ver + "_ValidationReport.PDF", true);
                        break;
                    case 2:
                        System.IO.File.Copy(paramExportFilePath, "\\\\192.168.1.14\\Shared\\Validation\\fileprocessed\\" + ID + "_" + ver + "_ValidationReport_CC.PDF", true);
                        break;
                    case 3:
                        System.IO.File.Copy(paramExportFilePath, "\\\\192.168.1.14\\Shared\\Validation\\fileprocessed\\" + ID + "_" + ver + "_ValidationReport_Revised.PDF", true);
                        break;
                    case 4:
                        System.IO.File.Copy(paramExportFilePath, "\\\\192.168.1.14\\Shared\\Validation\\fileprocessed\\" + ID + "_" + ver + "_ValidationReport_Regularized.PDF", true);
                        break;
                    case 5:
                        System.IO.File.Copy(paramExportFilePath, "\\\\192.168.1.14\\Shared\\Validation\\fileprocessed\\" + ID + "_" + ver + "_ValidationReport_AA.PDF", true);
                        break;
                    case 6:
                        System.IO.File.Copy(paramExportFilePath, "\\\\192.168.1.14\\Shared\\Validation\\fileprocessed\\" + ID + "_" + ver + "_ValidationReport_REVDN.PDF", true);
                        break;
                    case 7:
                        System.IO.File.Copy(paramExportFilePath, "\\\\192.168.1.14\\Shared\\Validation\\fileprocessed\\" + ID + "_" + ver + "_ValidationReport_SARAL_Revise.PDF", true);
                        break;
                    case 8:
                        System.IO.File.Copy(paramExportFilePath, "\\\\192.168.1.14\\Shared\\Validation\\fileprocessed\\" + ID + "_" + ver + "_ValidationReport_SANCTION_Up_To_500_Sqmt.PDF", true);
                        break;
                    case 9:
                        System.IO.File.Copy(paramExportFilePath, "\\\\192.168.1.14\\Shared\\Validation\\fileprocessed\\" + ID + "_" + ver + "_ValidationReport_Revised_SANCTION_Up_To_500_Sqmt.PDF", true);
                        break;
                }

            }
            catch
            {
                object DocFilename = "\\\\192.168.1.14\\Shared\\Validation\\fileprocessed\\" + Dwgreader.GetValue(1).ToString() + "-" + ID + "_" + ver + "_ValidationReport.DOC"; ;
                doc.SaveAs(ref DocFilename, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);
            }
            //doc.Close(ref savechanges, ref  missing, ref missing);
            WordApp.Quit(ref savechanges, ref  missing, ref missing);
        }
    }
}
