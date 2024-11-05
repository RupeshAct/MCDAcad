/// <summary>
/// Modifications done in Following Methods to avoid generation of Inappropriate  By-Law Report.
/// 1.ShopReport,
/// 2.OfficeReport,
/// 3.CarLiftReport,
/// 4.NotifiedRampReport,
/// 5.CommercialFeatureCountReport.
/// 6.BalconyReport
/// 7.RoomReport
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Word;
using wd = Microsoft.Office.Interop.Word;
//using IBM.Data.DB2;
using MySql.Data.MySqlClient;
using System.Data.SqlTypes;
using log4net;
using System.Configuration;
using Autodesk.AutoCAD.Interop.Common;


//[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]

namespace MCD
{
    class ReportDoc
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public List<string> TblNameLst = new List<string>();
        #region Table style for all tables


        wd.WdBorderType verticalBorder = wd.WdBorderType.wdBorderVertical;
        wd.WdBorderType leftBorder = wd.WdBorderType.wdBorderLeft;
        wd.WdBorderType rightBorder = wd.WdBorderType.wdBorderRight;
        wd.WdBorderType topBorder = wd.WdBorderType.wdBorderTop;
        wd.WdBorderType bottomBorder = wd.WdBorderType.wdBorderBottom;

        wd.WdLineStyle doubleBorder = wd.WdLineStyle.wdLineStyleDouble;
        //wd.WdLineStyle noBorder = wd.WdLineStyle.wdLineStyleNone;
        wd.WdLineStyle singleBorder = wd.WdLineStyle.wdLineStyleSingle;

        wd.WdTextureIndex noTexture = wd.WdTextureIndex.wdTextureNone;
        wd.WdColor gray10 = wd.WdColor.wdColorGray10;
        //wd.WdColor gray10 = wd.WdColor.wdColorGray10;
        //wd.WdColor gray70 = wd.WdColor.wdColorGray70;
        wd.WdColor gray70 = wd.WdColor.wdColorTeal;
        wd.WdColorIndex white = wd.WdColorIndex.wdWhite;


        private wd.Style CreateTableStyle(ref wd.Document wdDoc)
        {
            log.Debug("CreateTableStyle() - Started");
            object styleTypeTable = wd.WdStyleType.wdStyleTypeTable;
            wd.Style styl = wdDoc.Styles.Add
                 ("New Table Style", ref styleTypeTable);
            styl.Font.Name = "Arial";
            styl.Font.Size = 10;
            styl.Font.Position = 3;
            wd.TableStyle stylTbl = styl.Table;
            stylTbl.Borders.Enable = 1;

            wd.ConditionalStyle evenRowBanding =
                stylTbl.Condition(wd.WdConditionCode.wdEvenRowBanding);
            evenRowBanding.Shading.Texture = noTexture;
            evenRowBanding.Shading.BackgroundPatternColor = gray10;
            // Borders have to be set specifically for every condition.
            evenRowBanding.Borders[leftBorder].LineStyle = doubleBorder;
            evenRowBanding.Borders[rightBorder].LineStyle = doubleBorder;
            evenRowBanding.Borders[verticalBorder].LineStyle = singleBorder;

            wd.ConditionalStyle firstRow =
                stylTbl.Condition(wd.WdConditionCode.wdFirstRow);
            firstRow.Shading.BackgroundPatternColor = gray70;
            firstRow.Borders[leftBorder].LineStyle = doubleBorder;
            firstRow.Borders[topBorder].LineStyle = doubleBorder;
            firstRow.Borders[rightBorder].LineStyle = doubleBorder;
            firstRow.Font.Size = 8;
            firstRow.Font.ColorIndex = white;
            firstRow.Font.Bold = 1;
            firstRow.Font.Position = 3;
            // Set the number of rows to include in a "band".
            stylTbl.RowStripe = 1;

            log.Debug("CreateTableStyle() - Ended");
            return styl;
        }

        private void FormatAllTables(wd.Document wdDoc)
        {
            log.Debug("FormatAllTables() - Started");
            wd.Style styl = CreateTableStyle(ref wdDoc);
            foreach (wd.Table tbl in wdDoc.Tables)
            {
                object objStyle = styl;
                tbl.Range.set_Style(ref objStyle);
                // If the table ends in an "even band" the border will
                // be missing, so in this case add the border.

                if (SqlInt32.Mod(tbl.Rows.Count, 2) != 0)
                {
                    tbl.Borders[bottomBorder].LineStyle = doubleBorder;
                }
            }
            log.Debug("FormatAllTables() - Ended");
        }



        #endregion

        public bool report(object filename, string APP_ID, int buildTypeId)
        {
            bool retval = false;
            MySqlConnection con = new MySqlConnection(FunctionsNvar.Constr);
            try
            {
                log.Debug("Report() - Started ");
                con.Open();
                log.Debug("Connection opened");
            }
            catch (Exception ex)
            {
                log.Error("report()-Unable to open the connection-Error(" + ex.Message + ")");
                System.Windows.Forms.MessageBox.Show("Server Connection Not found please contact administrator \n error: " + ex.StackTrace, "MCD Building Plan",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return retval;
            }
            Application WordApp = new Application();
            WordApp.Visible = true;
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
            //imagePath = @"D:\GTACAD\Src\Logo (2).jpg";
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

            rng.Font.Name = "Cambria";
            rng.Font.Size = 16;
            rng.Font.Bold = 1;
            //rng.Font.Color = WdColor.wdColorAqua;
            rng.Font.Color = WdColor.wdColorPaleBlue;
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
            MySqlCommand AppCmd = new MySqlCommand( ";SELECT * FROM  file_watch where FR_ID_VER ='" + APP_ID + "';commit;", con);
            MySqlDataReader Appreader = AppCmd.ExecuteReader();
            if (Appreader.Read() == false)
            {
                WordApp.Quit(ref savechanges, ref  missing, ref missing);
                return retval;
            }
            Appreader.Close();
            //string ID = APP_ID;
            //ID = ID.Remove(ID.Length - 2);
            //MySqlCommand PropCmd = new MySqlCommand(";SELECT * FROM file_record where ID = " + ID + ";commit;", con);
            //MySqlDataReader Propreader = PropCmd.ExecuteReader();
            //if (Propreader.Read() == false)
            //{
            //    WordApp.Quit(ref savechanges, ref missing, ref missing);
            //    return retval;

            //}
            //Propreader.Close();
            Int16 approvedval = -1;
            if (buildTypeId == 117 || buildTypeId == 118)
            {
                MySqlCommand ApprovedCmd = new MySqlCommand( ";select TOTAL_ERROR from I117_ERROR_SUMMARY where FR_ID_VER ='" + APP_ID + "';commit;", con);
                MySqlDataReader Approvedreader = ApprovedCmd.ExecuteReader();

                if (Approvedreader.Read() == true)
                {
                    approvedval = Approvedreader.GetInt16(0);
                }
                Appreader.Close();
            }
           // else if (buildTypeId == 101 || buildTypeId == 102 || buildTypeId == 103 || buildTypeId == 104 || buildTypeId == 10185 || buildTypeId == 10186 || buildTypeId == 105 || buildTypeId == 106 || buildTypeId == 110)
            else if (buildTypeId == null || buildTypeId == 2 || buildTypeId == 3 || buildTypeId == 1 || buildTypeId == 101 || buildTypeId == 102 || buildTypeId == 103 || buildTypeId == 104 || buildTypeId == 105 || buildTypeId == 106 || buildTypeId == 110)

                    {

                MySqlCommand ApprovedCmd = new MySqlCommand( ";select TOTAL_ERROR from error_summary where FR_ID_VER ='" + APP_ID + "';commit;", con);
                MySqlDataReader Approvedreader = ApprovedCmd.ExecuteReader();

                if (Approvedreader.Read() == true)
                {
                    approvedval = Approvedreader.GetInt16(0);
                }
                Approvedreader.Close();
            }
            
            else if (buildTypeId == 107 || buildTypeId == 108 || buildTypeId == 121)
            {
                MySqlCommand ApprovedCmd = new MySqlCommand( ";select TOTAL_ERROR from RE_NOTIFIED_ERROR_SUMMARY where FR_ID_VER ='" + APP_ID + "';commit;", con);
                MySqlDataReader Approvedreader = ApprovedCmd.ExecuteReader();

                if (Approvedreader.Read() == true)
                {
                    approvedval = Approvedreader.GetInt16(0);
                }
                Approvedreader.Close ();    
            }

            //MySqlCommand DwgCmd = new MySqlCommand( ";SELECT * FROM DRAWING where FR_ID_VER ='" + APP_ID + "';commit;", con);
            //MySqlDataReader Dwgreader = DwgCmd.ExecuteReader();
            //if (Dwgreader.Read() == false)
            //{
            //    WordApp.Quit(ref savechanges, ref  missing, ref missing);
            //    return retval;

            //}
            //Dwgreader.Close();
            //string appOrRej = "Not in order";
            //if (approvedval == 0)
            //{
            //    appOrRej = "In order";
            //    retval = true;
           // }
            MySqlCommand Cmd1 = new MySqlCommand( ";select pid from  file_watch where FR_ID_VER ='" + APP_ID + "';commit;", con);
            int pid = Convert.ToInt16(Cmd1.ExecuteScalar());
            System.IO.FileSystemInfo fi = new System.IO.FileInfo((string)filename);
            string fname = fi.Name.Replace("_" + APP_ID.ToString(), "");
            object objAutoFitFixed2 = WdAutoFitBehavior.wdAutoFitWindow;
            Table tbl2 = doc.Tables.Add(rng, 4, 4, ref missing, ref objAutoFitFixed2);
            tbl2.Rows.HeightRule = WdRowHeightRule.wdRowHeightAuto;
            tbl2.Range.Font.Size = 8;
            Object style = "Table Grid 1";
            tbl2.set_Style(ref style);
            string ID = APP_ID;
            ID = ID.Remove(ID.Length - 2);
            MySqlCommand PropCmd = new MySqlCommand(";SELECT * FROM file where FR_ID = '" + ID + "';commit;", con);
            MySqlDataReader Propreader = PropCmd.ExecuteReader();
            if (Propreader.Read() == false)
            {
                WordApp.Quit(ref savechanges, ref missing, ref missing);
                return retval;

            }
            //-->>Included Two New columns Architect Name and Architect CA No in the Report on 24th Sept 2013 By Kiran Bishaj.
            tbl2.Cell(1, 1).Range.Text = "Architect Name :";
            tbl2.Cell(1, 1).Range.Bold = 1;
            tbl2.Cell(1, 2).Range.Text = Propreader.GetValue(15).ToString();
            tbl2.Cell(1, 3).Range.Text = "Architect CA No :";
            tbl2.Cell(1, 3).Range.Bold = 1;
            tbl2.Cell(1, 4).Range.Text = Propreader.GetValue(16).ToString();
            //string dwgname = Dwgreader.GetValue(1).ToString();
            tbl2.Cell(2, 1).Range.Text = "Applicant Name :";
            tbl2.Cell(2, 1).Range.Bold = 1;
            tbl2.Cell(2, 2).Range.Text = Propreader.GetValue(15).ToString();
            tbl2.Cell(2, 3).Range.Text = "Address :";
            tbl2.Cell(2, 3).Range.Bold = 1;
            tbl2.Cell(2, 4).Range.Text = Propreader.GetValue(11).ToString();
            tbl2.Cell(3, 1).Range.Text = "Building Type :";
            tbl2.Cell(3, 1).Range.Bold = 1;
            Propreader.Close();

            MySqlCommand DwgCmd = new MySqlCommand(";SELECT * FROM file where FR_ID_VER ='" + APP_ID + "';commit;", con);
            MySqlDataReader Dwgreader = DwgCmd.ExecuteReader();
            if (Dwgreader.Read() == false)
            {
                WordApp.Quit(ref savechanges, ref missing, ref missing);
                return retval;

            }
            
            string appOrRej = "Not in order";
            if (approvedval == 0)
            {
                appOrRej = "In order";
                retval = true;
            }

            string dwgname = Dwgreader.GetValue(1).ToString();

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
                if (pid == 8)
                {
                    tbl2.Cell(3, 2).Range.Text = "Residential_SANCTION_Up_To_500_Sqmt ";
                }
                if (pid == 9)
                {
                    tbl2.Cell(3, 2).Range.Text = "Residential_Revised_SANCTION_Up_To_500_Sqmt ";
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
                if (pid == 8)
                {
                    tbl2.Cell(3, 2).Range.Text = "URC_SANCTION_Up_To_500_Sqmt";
                }
                if (pid == 9)
                {
                    tbl2.Cell(3, 2).Range.Text = "URC_Revised_SANCTION_Up_To_500_Sqmt ";
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
                if (pid == 8)
                {
                    tbl2.Cell(3, 2).Range.Text = "Village Abadi_SANCTION_Up_To_500_Sqmt ";
                }
                if (pid == 9)
                {
                    tbl2.Cell(3, 2).Range.Text = "Village Abadi_Revised_SANCTION_Up_To_500_Sqmt ";
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

                if (pid == 8)
                {
                    tbl2.Cell(3, 2).Range.Text = "City Area_SANCTION_Up_To_500_Sqmt ";
                }
                if (pid == 9)
                {
                    tbl2.Cell(3, 2).Range.Text = "City Area _Revised_SANCTION_Up_To_500_Sqmt ";
                }

            }
            // else if (buildTypeId == 10185)
            //{
            //    if (pid == 1)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Lutyens Bungalow Zone area";
            //    }
            //    if (pid == 2)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Lutyens Bungalow Zone area_CC ";
            //    }
            //    if (pid == 3)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Lutyens Bungalow Zone area_Revised ";
            //    }
            //    if (pid == 4)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Lutyens Bungalow Zone area_Regularized ";
            //    }
            //    if (pid == 5)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Lutyens Bungalow Zone area_AA ";
            //    }
            //    if (pid == 6)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Lutyens Bungalow Zone area_REVDN ";
            //    }
            //    if (pid == 7)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Lutyens Bungalow Zone area_SARAL_Revise";
            //    }
            //    if (pid == 8)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Lutyens Bungalow Zone area_SANCTION_Up_To_500_Sqmt";
            //    }
            //    if (pid == 9)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Lutyens Bungalow Zone area_Revised_SANCTION_Up_To_500_Sqmt ";
            //    }
            //}
            //else if (buildTypeId == 10186)
            //{
            //    if (pid == 1)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Civil Line Bungalow area";
            //    }
            //    if (pid == 2)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Civil Line Bungalow area_CC ";
            //    }
            //    if (pid == 3)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Civil Line Bungalow area_Revised ";
            //    }
            //    if (pid == 4)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Civil Line Bungalow area_Regularized ";
            //    }
            //    if (pid == 5)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Civil Line Bungalow area_AA ";
            //    }
            //    if (pid == 6)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Civil Line Bungalow area_REVDN ";
            //    }
            //    if (pid == 7)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Civil Line Bungalow area_SARAL_Revise";
            //    }
            //    if (pid == 8)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Civil Line Bungalow area_SANCTION_Up_To_500_Sqmt";
            //    }
            //    if (pid == 9)
            //    {
            //        tbl2.Cell(3, 2).Range.Text = "Civil Line Bungalow area_Revised_SANCTION_Up_To_500_Sqmt ";
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
                if (pid == 8)
                {
                    tbl2.Cell(3, 2).Range.Text = "Standard Plan_SANCTION_Up_To_500_Sqmt ";
                }
                if (pid == 9)
                {
                    tbl2.Cell(3, 2).Range.Text = "Standard Plan_Revised_SANCTION_Up_To_500_Sqmt ";
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
                if (pid == 8)
                {
                    tbl2.Cell(3, 2).Range.Text = "Residential Group Housing_SANCTION_Up_To_500_Sqmt ";
                }
                if (pid == 9)
                {
                    tbl2.Cell(3, 2).Range.Text = "Residential Group Housing_Revised_SANCTION_Up_To_500_Sqmt ";
                }
            }

            if (buildTypeId == 107 || buildTypeId == 121)
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
                if (pid == 8)
                {
                    tbl2.Cell(3, 2).Range.Text = "Notified-Commercial_SANCTION_Up_To_500_Sqmt ";
                }
                if (pid == 9)
                {
                    tbl2.Cell(3, 2).Range.Text = "Notified-Commercial_Revised_SANCTION_Up_To_500_Sqmt ";
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
                if (pid == 8)
                {
                    tbl2.Cell(3, 2).Range.Text = "Notified - MLU_SANCTION_Up_To_500_Sqmt ";
                }
                if (pid == 9)
                {
                    tbl2.Cell(3, 2).Range.Text = "Notified - MLU_Revised_SANCTION_Up_To_500_Sqmt ";
                }
            }

            else if (buildTypeId == 110)
            {
                if (pid == 1)
                {
                    tbl2.Cell(3, 2).Range.Text = "FarmHouse ";
                }
                if (pid == 2)
                {
                    tbl2.Cell(3, 2).Range.Text = "FarmHouse_CC ";
                }
                if (pid == 3)
                {
                    tbl2.Cell(3, 2).Range.Text = "FarmHouse_Revised ";
                }
                if (pid == 4)
                {
                    tbl2.Cell(3, 2).Range.Text = "FarmHouse_Regularized ";
                }
                if (pid == 5)
                {
                    tbl2.Cell(3, 2).Range.Text = "FarmHouse_AA ";
                }
                if (pid == 6)
                {
                    tbl2.Cell(3, 2).Range.Text = "FarmHouse_REVDN ";
                }
                if (pid == 7)
                {
                    tbl2.Cell(3, 2).Range.Text = "FarmHouse_SARAL_Revise ";
                }
                if (pid == 8)
                {
                    tbl2.Cell(3, 2).Range.Text = "FarmHouse_SANCTION_Up_To_500_Sqmt ";
                }
                if (pid == 9)
                {
                    tbl2.Cell(3, 2).Range.Text = "FarmHouse_Revised_SANCTION_Up_To_500_Sqmt ";
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
                    tbl2.Cell(3, 2).Range.Text = "Industrial_SARAL_Revise ";
                }
                if (pid == 8)
                {
                    tbl2.Cell(3, 2).Range.Text = "Industrial_SANCTION_Up_To_500_Sqmt ";
                }
                if (pid == 9)
                {
                    tbl2.Cell(3, 2).Range.Text = "Industrial_Revised_SANCTION_Up_To_500_Sqmt ";
                }
            }

            else if (buildTypeId == 118)
            {
                if (pid == 1)
                {
                    tbl2.Cell(3, 2).Range.Text = "FlattedFactory ";
                }
                if (pid == 2)
                {
                    tbl2.Cell(3, 2).Range.Text = "FlattedFactory_CC ";
                }
                if (pid == 3)
                {
                    tbl2.Cell(3, 2).Range.Text = "FlattedFactory_Revised ";
                }
                if (pid == 4)
                {
                    tbl2.Cell(3, 2).Range.Text = "FlattedFactory_Regularized ";
                }
                if (pid == 5)
                {
                    tbl2.Cell(3, 2).Range.Text = "FlattedFactory_AA ";
                }
                if (pid == 6)
                {
                    tbl2.Cell(3, 2).Range.Text = "FlattedFactory_REVDN ";
                }
                if (pid == 7)
                {
                    tbl2.Cell(3, 2).Range.Text = "FlattedFactory_SARAL_Revise ";
                }
                if (pid == 8)
                {
                    tbl2.Cell(3, 2).Range.Text = "FlattedFactory_SANCTION_Up_To_500_Sqmt ";
                }
                if (pid == 9)
                {
                    tbl2.Cell(3, 2).Range.Text = "FlattedFactory_Revised_SANCTION_Up_To_500_Sqmt ";
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
            tbl2.Cell(5, 3).Range.Text = "Not in order / In order :";
            tbl2.Cell(5, 3).Range.Bold = 1;
            tbl2.Cell(5, 4).Range.Text = appOrRej;
            //<<--Included Two New columns Architect Name and Architect CA No in the Report on 24th Sept 2013 By Kiran Bishaj.
            /*******************************for summary Table******************/

            //rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;

            //rng.InsertParagraphAfter();
            //rng.InsertParagraphAfter();
            rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;

            //rng.InsertParagraphAfter();
            rng.Paragraphs.Add(ref missing);
            rng.InsertParagraphAfter();
            rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            rng.Paragraphs.Add(ref missing);
            //rng.InsertParagraphAfter();
            rng.Text = "Summary of Plot";
            rng.Font.Name = "Verdana";
            rng.Font.Size = 14;
            rng.Font.Color = WdColor.wdColorBlue;
            rng.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            rng.InsertParagraphAfter();
            //rng.InsertParagraphAfter();
            Dwgreader.Close();
            MySqlCommand scmd = new MySqlCommand(";select count(*) from ( SELECT RESFLR_BLDG_NO, COUNT(DISTINCT RESDU_NO || FL_NO) AS distinct_count  FROM re_rooms  WHERE FR_ID_VER ='" + APP_ID + "' GROUP BY RESFLR_BLDG_NO ) AS subquery;", con);
            MySqlDataReader sreader = scmd.ExecuteReader();
            int Srowcount = 0;
            if (sreader.Read() == true)
            {
                Srowcount = sreader.GetInt32(0);
            }
            sreader.Close();
            MySqlCommand scmd1 = new MySqlCommand( ";select RESFLR_BLDG_NO,count(1) from re_rooms where FR_ID_VER='" + APP_ID + "' and (ROOM_CODE='K' or Room_code='KD') group by RESFLR_BLDG_NO;", con);
            //scmd1.Parameters.AddWithValue("@appId", APP_ID);
            MySqlDataReader sreader1 = scmd1.ExecuteReader();
            int Srowcount1 = 0;
            if (sreader1.Read() == true)
            {
               // Srowcount1 = sreader.GetInt32(0);
                rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                //rng.InsertParagraphAfter();
                int FC = sreader.FieldCount;
                object objDefaultBehaviorWord8 = WdDefaultTableBehavior.wdWord9TableBehavior;
                object objAutoFitFixed = WdAutoFitBehavior.wdAutoFitFixed;
                Table tbl = null;
                if (buildTypeId == 117 || buildTypeId == 118)
                {
                    tbl = doc.Tables.Add(rng, Srowcount + 2, 4, ref objDefaultBehaviorWord8, ref objAutoFitFixed);
                }
                else
                {
                    tbl = doc.Tables.Add(rng, Srowcount + 2, 5, ref objDefaultBehaviorWord8, ref objAutoFitFixed);
                }

                tbl.Range.Font.Size = 7;
                tbl.ApplyStyleColumnBands = true;
                tbl.set_Style(ref style);
                tbl.Cell(1, 1).Range.Text = "Sr.NO";
                tbl.Cell(1, 2).Range.Text = "Building NO";
                tbl.Cell(1, 3).Range.Text = "No. of Floors";
                if (buildTypeId != 117 && buildTypeId !=118)
                {
                    tbl.Cell(1, 4).Range.Text = "Dwelling count";
                    tbl.Cell(1, 5).Range.Text = "Floor Area";
                }
                else
                {
                    tbl.Cell(1, 4).Range.Text = "Floor Area";
                }
                int nrRow = 2;
                int totalBldgs = 0;
                int totalFlrs = 0;
                int totalDwl = 0;
                decimal totalfloorarea = 0;
                
                do
                {
                    tbl.Cell(nrRow, 1).Range.Text = (nrRow - 1).ToString();
                    short Bldgs = sreader1.GetInt16(0);
                    int dwellings = sreader1.GetInt32(1);
                    decimal floorarea = 0;
                    sreader1.Close();
                    MySqlCommand Flcmd = new MySqlCommand("SELECT count(RESFLR_FLR_NO),SUM(RESFLR_FLR_AREA) FROM DA_RES_FLOOR WHERE  RESFLR_FR_ID_VER = '" + APP_ID + "' AND RESFLR_BLDG_NO ='" + Bldgs + "' and (RESFLR_FLR_CODE = 'O' or RESFLR_FLR_CODE = 'G');", con);
                    MySqlDataReader Flreader = Flcmd.ExecuteReader();
                    int Flrs = 0;
                    if (Flreader.Read() == true)
                    {
                        if (Flreader[1].ToString() != string.Empty)
                        {
                            floorarea = Flreader.GetDecimal(1);
                        }
                        Flrs = Flreader.GetInt32(0);
                    }
                    totalfloorarea += floorarea;
                    tbl.Cell(nrRow, 2).Range.Text = Bldgs.ToString();
                    tbl.Cell(nrRow, 3).Range.Text = Flrs.ToString();
                    if (buildTypeId != 117 && buildTypeId !=118)
                    {
                        tbl.Cell(nrRow, 4).Range.Text = dwellings.ToString();
                        tbl.Cell(nrRow, 5).Range.Text = floorarea.ToString();
                    }
                    else
                    {
                        tbl.Cell(nrRow, 4).Range.Text = floorarea.ToString();
                    }

                    tbl.Rows[nrRow].Alignment = WdRowAlignment.wdAlignRowCenter;
                    totalBldgs++;
                    totalFlrs += Flrs;
                    totalDwl += dwellings;
                    nrRow++;
                    Flreader.Close();   
                } 

                while (sreader1.HasRows && sreader1.Read());
                tbl.Cell(nrRow, 1).Range.Text = "Total";
                tbl.Cell(nrRow, 2).Range.Text = totalBldgs.ToString();
                tbl.Cell(nrRow, 3).Range.Text = totalFlrs.ToString();
                if (buildTypeId != 117 && buildTypeId !=118)
                {
                    tbl.Cell(nrRow, 4).Range.Text = totalDwl.ToString();
                    tbl.Cell(nrRow, 5).Range.Text = totalfloorarea.ToString();
                }
                else
                {
                    tbl.Cell(nrRow, 4).Range.Text = totalfloorarea.ToString();
                }
                tbl.Rows[nrRow].Alignment = WdRowAlignment.wdAlignRowCenter;
                //rng.InsertParagraphAfter();
                sreader1.Close();
            }
            ////rng.InsertParagraphAfter();
            rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            object Objrng = rng;
            Paragraph oPara = doc.Content.Paragraphs.Add(ref Objrng);
            oPara.Range.InsertParagraphBefore();
            oPara.Range.Text = "Note:\tAll Area units are in Sq. Mts and All Linear units are in Mts";

            oPara.Range.Font.Name = "Verdana";
            oPara.Range.Font.Size = 8;
            oPara.Range.Font.Color = WdColor.wdColorBlue;
            oPara.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            oPara.Range.InsertParagraphAfter();
            sreader1.Close();
            /***********************************/

            //starting report tables
            //commented on 22nd march 2012


            if (buildTypeId == 110)
            {
                TblNameLst.Add("RE_R110_FAR");
                //TblNameLst.Add("RE_R110_FEE"); 
            }
            else
            {
                TblNameLst.Add("RE_FAR");
            }
            TblNameLst.Add("RE_COVERAGE");
            sreader1.Close();
            Appreader.Close();
            MySqlCommand db2cmd = new MySqlCommand("SELECT count(1) FROM RE102_PRORATA where FR_ID_VER ='" + APP_ID + "';commit;", con);

            int datacount = Convert.ToInt16(db2cmd.ExecuteScalar());
            if (datacount > 0)
            {
                TblNameLst.Add("RE102_PRORATA");
            }

            if (buildTypeId != 106)
            {
                TblNameLst.Add("RE_HEIGHT");
            }
           
            //if (buildTypeId == 101 || buildTypeId == 102 || buildTypeId == 103 || buildTypeId == 104 || buildTypeId == 105 || buildTypeId == 106 || buildTypeId == 107 || buildTypeId == 108 || buildTypeId == 110 || buildTypeId == 121 || buildTypeId == 10185 || buildTypeId == 10186)
              if (buildTypeId == 1 || buildTypeId == 101 || buildTypeId == 102 || buildTypeId == 103 || buildTypeId == 104 || buildTypeId == 105 || buildTypeId == 106 || buildTypeId == 107 || buildTypeId == 108 || buildTypeId == 110 || buildTypeId == 121)
                {

                TblNameLst.Add("RE_SETBACK");
                TblNameLst.Add("RE_RES_CUPBOARD_SHELVES");
                TblNameLst.Add("RE_RES_TOTAL_CUPBOARD_SHELVES");
                TblNameLst.Add("RE_RES_HEADROOM_STAIRCASE");
                TblNameLst.Add("RE_PERGOLA_TOTAL");
                TblNameLst.Add("RE_PERGOLA");
                TblNameLst.Add("RE_DWELLING_UNIT_COUNT");
                TblNameLst.Add("RE_RES_LEDGE_TAND");
                TblNameLst.Add("RE_RES_LEDGE_TAND_HT");
                TblNameLst.Add("RE_FIREESCAPE_STAIRCASE");
                TblNameLst.Add("RE_LOFT");
                TblNameLst.Add("RE_LOFT_HT");
                TblNameLst.Add("RE_RES_PANTRIES");
                TblNameLst.Add("RE_SERVANT_QUARTERS");
                TblNameLst.Add("RE_RES_GARAGE");
                TblNameLst.Add("RE_RES_SPIRAL_STAIRS");
            }
            if (buildTypeId == 117 || buildTypeId==118)
            {

                TblNameLst.Add("I117_RE_SETBACK");
            }



            if (buildTypeId == 107 || buildTypeId == 108)
            {
                TblNameLst.Add("RE_SHOP");
                TblNameLst.Add("RE_OFFICE");
                TblNameLst.Add("RE_CARLIFT");
                TblNameLst.Add("RE_COMMERCIAL_FEATURES_COUNT");
                TblNameLst.Add("RE_NOTIFIED_RAMPS");
                TblNameLst.Add("RE_NOTIFIED_DWELLING_UNIT_COUNT");
            }
            //TblNameLst.Add("RE_COVERAGE");
            
            MySqlCommand CheckFidCmd = new MySqlCommand("SELECT pid FROM  file_watch where FR_ID_VER ='" + APP_ID + "';commit;", con);
            int CheckFid = Convert.ToInt16(CheckFidCmd.ExecuteScalar());
            if (CheckFid != 1)
            {
                TblNameLst.Add("RE_COVERAGE_DIFF");
            }
            //TblNameLst.Add("RE_COVERAGE_DIFF");
            TblNameLst.Add("RE_PARKING_TOTAL_NO");
            TblNameLst.Add("RE_PARKING");
            TblNameLst.Add("RE_RES_STILT");
            TblNameLst.Add("RE_ROOMS");
            TblNameLst.Add("RE_VENTILATION");
            
            TblNameLst.Add("RE_CORRIDORS");
            TblNameLst.Add("RE_RES_WEATHER_SHD");
            TblNameLst.Add("RE_RES_PROVSION_LIFT");
            TblNameLst.Add("RE_RES_HEADROOM_STAIRCASE");
            TblNameLst.Add("RE_RES_PARAPET_WALL");
            TblNameLst.Add("RE_RES_BNDRY_WALL");
            //    TblNameLst.Add("RE_RES_PANTRIES");
            TblNameLst.Add("RE_FIREESCAPE_STAIRCASE");
            TblNameLst.Add("RE_PASSAGEWAYS_WT");
            TblNameLst.Add("RE_RES_STAIRWAYS");
            TblNameLst.Add("RE_CANOPY");
            TblNameLst.Add("RE_RES_SPIRAL_STAIRS");
            TblNameLst.Add("RE_BALCONY");
            TblNameLst.Add("RE_CANOPY_TOTAL");
            TblNameLst.Add("RE_RES_STORE_ROOM");
            TblNameLst.Add("RE_COURTYARD");
            TblNameLst.Add("RE_SHAFT");


            
            //}
            MySqlCommand command = new MySqlCommand( ";select count(1) from GENERAL_ERRORS WHERE FR_ID_VER = '" + APP_ID + "' AND (COMPLY = 'No'or COMPLY = 'NO');", con);
            int Errcount = Convert.ToInt16(command.ExecuteScalar());
            if (Errcount > 0)
            {
                //Modified If condition added Building Type 101 to get General errors in The Bylaw report by Kiran Bishaj on 7th Jan 2014.
                //if (buildTypeId == 101 || buildTypeId == 102 || buildTypeId == 103 || buildTypeId == 104 || buildTypeId == 105 || buildTypeId == 106 || buildTypeId == 107 || buildTypeId == 108 || buildTypeId == 110 || buildTypeId == 117 || buildTypeId == 118 || buildTypeId == 121 || buildTypeId == 10185 || buildTypeId == 10186)
                if (buildTypeId == 1 || buildTypeId == 101 || buildTypeId == 102 || buildTypeId == 103 || buildTypeId == 104 || buildTypeId == 105 || buildTypeId == 106 || buildTypeId == 107 || buildTypeId == 108 || buildTypeId == 110 || buildTypeId == 117 || buildTypeId == 118 || buildTypeId == 121 )
                    //if (buildTypeId == 103 || buildTypeId == 117 || buildTypeId == 108 || buildTypeId == 107 || buildTypeId == 106 || buildTypeId == 105 || buildTypeId == 102 || buildTypeId == 104)
                    {
                    TblNameLst.Add("GENERAL_ERRORS");
                }
            }
            
            foreach (string TblName in TblNameLst)
            {
                if (TblName == "RE_ROOMS")
                {
                    RoomReport(doc, APP_ID, con, buildTypeId);
                    continue;
                }

            

                if (buildTypeId == 110)
                {
                    if (TblName == "RE_R110_FAR")
                    {
                        FarmHouseFAR(doc, APP_ID, con);
                        continue;
                    }
                    //if (TblName == "RE_R110_FEE")
                    //{
                    //    FarmHouseFEE(doc, APP_ID, con);
                    //    continue;
                    //}                    
                }

                if (TblName == "RE_VENTILATION")
                {
                    VentilationReport(doc, APP_ID, con, buildTypeId);
                    continue;
                }
                //if (TblName == "RE_PARKING")
                //{
                //    ParkingReport(doc, APP_ID, con);
                //    continue;
                //}
                if (TblName == "RE_SETBACK")
                {
                    SetbackReport(doc, APP_ID, con);
                    continue;
                }
                if (TblName == "I117_RE_SETBACK")
                {
                    IndustrialSetbackReport(doc, APP_ID, con);
                    continue;
                }

                if (TblName == "RE_NOTE")
                {
                    //NotesReport(doc, APP_ID, con);
                    continue;
                }
                if ((TblName == "R101_V102_COVERAGEAREA_PROC") || (TblName == "I117_V100_COVERAGEAREA_PROC") || (TblName == "RE_COVERAGE"))
                {
                    CoverageReport(doc, APP_ID, con);
                    continue;
                }

                if (TblName == "RE_BALCONY")
                {
                    BalconyReport(doc, APP_ID, con);
                    continue;
                }

                if (TblName == "RE_SHOP")
                {
                    ShopReport(doc, APP_ID, con);
                    continue;
                }
                if (TblName == "RE_OFFICE")
                {
                    OfficeReport(doc, APP_ID, con);
                    continue;
                }
                if (TblName == "RE_CARLIFT")
                {
                    CarLiftReport(doc, APP_ID, con);
                    continue;
                }
                if (TblName == "RE_NOTIFIED_RAMPS")
                {
                    NotifiedRampReport(doc, APP_ID, con);
                    continue;
                }
                if (TblName == "RE_COMMERCIAL_FEATURES_COUNT")
                {
                    CommercialFeatureCountReport(doc, APP_ID, con);
                    continue;
                }
                if (TblName == "GENERAL_ERRORS")
                {
                    OtherErrorsReport(doc, APP_ID, con, buildTypeId);
                    continue;
                }

                sreader.Close();
                //Propreader.Close();
                MySqlCommand cmd = new MySqlCommand( ";SELECT count(1) FROM " + TblName + " where FR_ID_VER ='" + APP_ID + "';commit;", con);
                MySqlDataReader reader1;
                try
                
                {
                    reader1 = cmd.ExecuteReader();
                }
                catch (Exception)
                {
                    continue;
                }

                int rowcount = 0;
                if (reader1.Read() == true)
                {
                    rowcount = reader1.GetInt32(0);
                }
                reader1.Close();
                MySqlCommand cmd1 = new MySqlCommand( ";SELECT * FROM " + TblName + " where FR_ID_VER ='" + APP_ID + "';commit;", con);
                MySqlDataReader reader;
                try
                {
                    reader = cmd1.ExecuteReader();
                }
                catch (Exception)
                {
                    continue;
                }
                Paragraph oPara4;
                rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                object parang = rng;
                oPara4 = doc.Content.Paragraphs.Add(ref parang);
                oPara4.Range.InsertParagraphBefore();
                if (TblName.StartsWith("RE_RES") && (buildTypeId == 117 || buildTypeId == 118))
                {
                    oPara4.Range.Text = "Report for " + TblName.Remove(0, 7);
                }
                else if (TblName.StartsWith("RE102_PRORATA"))
                {
                    oPara4.Range.Text = "Report For" + TblName.Remove(0, 5);
                }
                else if (TblName.StartsWith("GENERAL_ERRORS") == false)
                {
                    oPara4.Range.Text = "Report for " + TblName.Remove(0, 3);
                }


                oPara4.Range.Font.Underline = WdUnderline.wdUnderlineSingle;
                oPara4.Range.Font.Size = 11;
                oPara4.Range.Font.Color = WdColor.wdColorAutomatic;
                oPara4.Range.InsertParagraphAfter();
                if (reader.Read() == true)
                {
                    

                    rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                    int FC = reader.FieldCount;
                    object objDefaultBehaviorWord8 = WdDefaultTableBehavior.wdWord9TableBehavior;
                    object objAutoFitFixed = WdAutoFitBehavior.wdAutoFitFixed;
                    if (TblName == "GENERAL_ERRORS")
                    {
                        FC += 2;
                    }
                    if (TblName == "RE_COVERAGE_DIFF")
                    {
                        FC += 2;
                    }
                    if (TblName == "RE102_PRORATA")
                    {
                        FC += 2;
                    }

                    Table tbl = doc.Tables.Add(rng, rowcount + 1, FC - 3, ref objDefaultBehaviorWord8, ref objAutoFitFixed);
                    tbl.Range.Font.Size = 7;
                    tbl.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                    tbl.ApplyStyleColumnBands = true;
                    tbl.set_Style(ref style);
                    //inserting field names
                    for (int FieldNo = 1; FieldNo < FC - 2; FieldNo++)
                    {
                        string FName = reader.GetName(FieldNo);
                        string[] FNames = FName.Split('_');
                        StringBuilder stb = new StringBuilder();
                        foreach (string FN in FNames)
                        {
                            stb.Append(" " + FN);
                        }

                        tbl.Cell(1, FieldNo).Range.Text = stb.ToString().Trim();
                    }
                    int nrRow = 1;
                    //inserting field values
                    do
                    {
                        //tbl.Rows.Add(ref missing);
                        nrRow++;
                        for (int nrCol = 1; nrCol < FC - 2; nrCol++)
                        {
                            // Now add the records.
                            string Valstr = reader.GetValue(nrCol).ToString();
                            tbl.Cell(nrRow, nrCol).Range.Text = Valstr;
                            if (Valstr.ToUpper() == "NO")
                            {
                                tbl.Rows[nrRow].Range.Font.Color = WdColor.wdColorDarkRed;
                                tbl.Rows[nrRow].Range.Font.Bold = 1;
                                //tbl.Rows[nrRow].Range.Shading.BackgroundPatternColor = WdColor.wdColorDarkRed;
                                tbl.Rows[nrRow].Range.Shading.ForegroundPatternColor = WdColor.wdColorGray10;
                            }

                            tbl.Rows[nrRow].Alignment = WdRowAlignment.wdAlignRowCenter;
                        }
                    } while (reader.HasRows && reader.Read());
                    //rng.InsertParagraphAfter();
                    rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                    parang = rng;
                    oPara4 = doc.Content.Paragraphs.Add(ref parang);
                    oPara4.Format.SpaceBefore = 5;
                    oPara4.Format.SpaceAfter = 5;

                    oPara4.Range.InsertParagraphAfter();
                    reader.Close();
                }
                else
                {
                    Paragraph oPara5;
                    rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                    object parang2 = rng;
                    oPara5 = doc.Content.Paragraphs.Add(ref parang2);
                    oPara5.Range.InsertParagraphBefore();
                    oPara5.Range.Text = "There is no records found.";
                    oPara5.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                    oPara5.Range.Font.Size = 11;
                    oPara5.Range.Font.Color = WdColor.wdColorAutomatic;
                    oPara5.Range.InsertParagraphAfter();
                }
                reader.Close();
                //rng.InsertParagraphAfter();
            }
            
            con.Close();
            wd.Style styl = CreateTableStyle(ref doc);
            byte tmpint = 1;
            foreach (wd.Table tbl in doc.Tables)
            {
                if (tmpint == 1)
                {
                    tmpint++;
                    continue;
                }
                object objStyle = styl;
                tbl.Range.set_Style(ref objStyle);
                // If the table ends in an "even band" the border will
                // be missing, so in this case add the border.

                if (SqlInt32.Mod(tbl.Rows.Count, 2) != 0)
                {
                    tbl.Borders[bottomBorder].LineStyle = doubleBorder;
                }
            }

            //********To export pdf****************
            try
            {
                log.Debug("Report() - Export to PDF");
                string id = APP_ID.Substring(0, APP_ID.Length - 2);
                string ver = APP_ID.Substring(APP_ID.Length - 2);
                string paramExportFilePath = filename + dwgname + "-'" + ID + "'_" + ver + "_ByeLawReport.PDF";
                string paramExportFilePath2 = filename + id + "_" + ver + "_ByeLawReport.PDF";

                WdExportFormat paramExportFormat = WdExportFormat.wdExportFormatPDF;
                bool paramOpenAfterExport = false;
                WdExportOptimizeFor paramExportOptimizeFor =
                    WdExportOptimizeFor.wdExportOptimizeForPrint;
                WdExportRange paramExportRange = WdExportRange.wdExportAllDocument;
                int paramStartPage = 0;
                int paramEndPage = 0;
                WdExportItem paramExportItem = WdExportItem.wdExportDocumentContent;
                bool paramIncludeDocProps = true;
                bool paramKeepIRM = true;
                WdExportCreateBookmarks paramCreateBookmarks =
                    WdExportCreateBookmarks.wdExportCreateWordBookmarks;
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
                doc.ExportAsFixedFormat(paramExportFilePath2,
                    paramExportFormat, paramOpenAfterExport,
                    paramExportOptimizeFor, paramExportRange, paramStartPage,
                    paramEndPage, paramExportItem, paramIncludeDocProps,
                    paramKeepIRM, paramCreateBookmarks, paramDocStructureTags,
                    paramBitmapMissingFonts, paramUseISO19005_1,
                    ref missing);
                switch (pid)
                {
                    case 1:
                        //System.IO.File.Copy(paramExportFilePath2, "d:\\To-Erp\\" + id + "_" + ver + "_ByeLawReport.PDF", true);
                        System.IO.File.Copy(paramExportFilePath2, "\\\\192.168.1.14\\Shared\\Validation\\fileprocessed\\" + id + "_" + ver + "_ByeLawReport.PDF", true);
                        break;
                    case 2:
                        //System.IO.File.Copy(paramExportFilePath2, "d:\\To-Erp\\" + id + "_" + ver + "_ByeLawReport_CC.PDF", true);
                        System.IO.File.Copy(paramExportFilePath2, "\\\\192.168.1.14\\Shared\\Validation\\fileprocessed\\" + id + "_" + ver + "_ByeLawReport_CC.PDF", true);
                        break;
                    case 3:
                        System.IO.File.Copy(paramExportFilePath2, "\\\\192.168.1.14\\Shared\\Validation\\fileprocessed\\" + id + "_" + ver + "_ByeLawReport_Revised.PDF", true);
                        break;
                    case 4:
                        System.IO.File.Copy(paramExportFilePath2, "\\\\192.168.1.14\\Shared\\Validation\\fileprocessed\\" + id + "_" + ver + "_ByeLawReport_Regularized.PDF", true);
                        break;
                    case 5:
                        System.IO.File.Copy(paramExportFilePath2, "\\\\192.168.1.14\\Shared\\Validation\\fileprocessed\\" + id + "_" + ver + "_ByeLawReport_AA.PDF", true);
                        break;
                                 case 6:
                        System.IO.File.Copy(paramExportFilePath2, "\\\\192.168.1.14\\Shared\\Validation\\fileprocessed\\" + id + "_" + ver + "_ByeLawReport_REVDN.PDF", true);
                        break;
                    case 7:
                        System.IO.File.Copy(paramExportFilePath2, "\\\\192.168.1.14\\Shared\\Validation\\fileprocessed\\" + id + "_" + ver + "_ByeLawReport_SARAL_Revise.PDF", true);
                        break;
                    case 8:
                        System.IO.File.Copy(paramExportFilePath2, "\\\\192.168.1.14\\Shared\\Validation\\fileprocessed\\" + id + "_" + ver + "_ByeLawReport_SANCTION_Up_To_500_Sqmt.PDF", true);
                        break;
                    case 9:
                        System.IO.File.Copy(paramExportFilePath2, "\\\\192.168.1.14\\Shared\\Validation\\fileprocessed\\" + id + "_" + ver + "_ByeLawReport_Revised_SANCTION_Up_To_500_Sqmt.PDF", true);
                        break;
                }
            }
            catch (Exception ex)
            {
                log.Error("report()-Error occured in report generation; Error(" + ex.Message + ")");
                object DocFilename = filename + "-Report.doc";
                doc.SaveAs(ref DocFilename, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);
            }
            //doc.Close(ref savechanges, ref  missing, ref missing);
            WordApp.Quit(ref savechanges, ref  missing, ref missing);

            //con.Close();
            //doc.Content.Paragraphs.Add(ref doc);

            return retval;
        }

        public void RoomReport(Document doc, string APP_ID, MySqlConnection con, int buildTypeId)
        {
            log.Debug("RoomReport() - Generating Room Report");
            MySqlCommand cmd = new MySqlCommand("select count(1) from (SELECT DISTINCT RESFLR_BLDG_NO,FL_NO,RESDU_NO FROM RE_ROOMS WHERE FR_ID_VER = '" + APP_ID + "' )AS subquery;", con);
            cmd.Parameters.AddWithValue("@appId", APP_ID);
            MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows && reader.Read())
            {
            object missing = System.Reflection.Missing.Value;
            object oEndOfDoc = "\\endofdoc";
            //rng.InsertParagraphAfter();
            Range rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            object parang = rng;
            Paragraph oPara4 = doc.Content.Paragraphs.Add(ref parang);
            oPara4.Range.InsertParagraphBefore();
            if (buildTypeId == 117 || buildTypeId == 118)
            {
                oPara4.Range.Text = "Report for Room";
            }
            else
            {
                oPara4.Range.Text = "Report for Dwelling";
            }
            oPara4.Range.Font.Name = "Verdana";
            oPara4.Range.Font.Size = 11;
                oPara4.Range.Font.Color = WdColor.wdColorAutomatic;
            oPara4.Range.Font.Underline = WdUnderline.wdUnderlineSingle;
            oPara4.Range.InsertParagraphAfter();
                int ErrorCnt = reader.GetInt32(0);
                    if (ErrorCnt != 0)
                    {
                    reader.Close();
                    cmd.Dispose();
                    MySqlCommand cmd1 = new MySqlCommand("select count(1) from (SELECT DISTINCT RESFLR_BLDG_NO,FL_NO,RESDU_NO FROM RE_ROOMS WHERE FR_ID_VER ='" + APP_ID + "' AND (COMPLY = 'No'or COMPLY = 'NO') )AS subquery;", con);
                    cmd1.Parameters.AddWithValue("@appId", APP_ID);
                    // int tableCount = (int)cmd1.ExecuteScalar();
                    int tableCount = Convert.ToInt32(cmd1.ExecuteScalar());
                    if (tableCount == 0)
                {
                    rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                    //rng.InsertParagraphAfter();
                    rng.InsertParagraphAfter();
                    rng.Paragraphs.Add(ref missing);
                    //rng.InsertParagraphAfter();
                    rng.Text = "All Rooms dimensions are as per byeLaws.";
                    rng.Font.Name = "Verdana";
                    rng.Font.Size = 11;
                    rng.Font.Color = WdColor.wdColorBlue;
                    rng.Font.Underline = WdUnderline.wdUnderlineNone;
                    rng.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    //rng.ParagraphFormat.LineSpacing = 0;
                    rng.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
                    //rng.InsertParagraphAfter();
                    rng.InsertParagraphAfter();
                }
                else
                {
                        reader.Close();
                        MySqlCommand cmd3 = new MySqlCommand("SELECT DISTINCT RESFLR_BLDG_NO,FL_NO,RESDU_NO,FL_CODE FROM RE_ROOMS WHERE FR_ID_VER ='" + APP_ID + "' AND (COMPLY = 'No'or COMPLY = 'NO')", con);
                        cmd3.Parameters.AddWithValue("@appId", APP_ID);
                        MySqlDataReader reader1 = cmd3.ExecuteReader();

                    while (reader1.HasRows && reader1.Read())
                    {
                        short BldgNo = reader1.GetInt16(0);
                        short FloorNo = reader1.GetInt16(1);
                        int DwellingNO = reader1.GetInt32(2);
                        string Floorcode;
                        if (FloorNo == 0)
                        {
                            Floorcode = reader1.GetString(3);
                            if (Floorcode == "G")
                            {
                                Floorcode = "Ground";
                            }
                            else
                            {
                                Floorcode = FloorNo.ToString();
                            }

                        }
                        else
                        {
                            Floorcode = FloorNo.ToString();
                        }

                        rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                        parang = rng;
                        oPara4 = doc.Content.Paragraphs.Add(ref parang);
                        if (buildTypeId == 117 || buildTypeId ==118)
                        {
                            oPara4.Range.Text = "Report for Room :  from building no : " + BldgNo.ToString() + " and floor no: " + Floorcode.ToString();
                        }
                        else
                        {
                            oPara4.Range.Text = "Report for Dwelling unit : " + DwellingNO.ToString() + "    from building no : " + BldgNo.ToString() + " and floor no: " + Floorcode.ToString();
                        }

                        oPara4.Range.Font.Name = "Verdana";
                        oPara4.Range.Font.Size = 10;
                        oPara4.Range.Font.Underline = WdUnderline.wdUnderlineNone;

                        rng.InsertParagraphAfter();
                            //rng.InsertParagraphAfter();
                            reader1.Close();
                            cmd3.Dispose();
                        MySqlCommand Roomscmd = new MySqlCommand( ";select count(1) from (select * from re_rooms where FR_ID_VER ='" + APP_ID + "' AND RESFLR_BLDG_NO = " + BldgNo + " AND FL_NO = " + FloorNo + " AND RESDU_NO = " + DwellingNO + " AND (COMPLY = 'No'or COMPLY = 'NO'))AS subquery;", con);
                        MySqlDataReader Roomsreader1 = Roomscmd.ExecuteReader();
                        rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                        Roomsreader1.Read();
                        int FC = Roomsreader1.GetInt32(0);
                            Roomsreader1.Close();
                            MySqlCommand Roomscmd1 = new MySqlCommand( ";select * from re_rooms where FR_ID_VER ='" + APP_ID + "' AND RESFLR_BLDG_NO = " + BldgNo + " AND FL_NO = " + FloorNo + " AND RESDU_NO = " + DwellingNO + " AND (COMPLY = 'No'or COMPLY = 'NO')", con);
                            MySqlDataReader Roomsreader = Roomscmd1.ExecuteReader();
                        object objDefaultBehaviorWord8 = WdDefaultTableBehavior.wdWord9TableBehavior;
                        object objAutoFitFixed = WdAutoFitBehavior.wdAutoFitFixed;
                        Table tbl = doc.Tables.Add(rng, FC + 1, 10, ref objDefaultBehaviorWord8, ref objAutoFitFixed);
                        tbl.Range.Font.Size = 7;
                        tbl.ApplyStyleColumnBands = true;
                        tbl.Cell(1, 1).Range.Text = "Room No.";
                        tbl.Cell(1, 2).Range.Text = "Room Type";
                        tbl.Cell(1, 3).Range.Text = "Room Area";
                        tbl.Cell(1, 4).Range.Text = "PERMISSIBLE Min Area";
                        tbl.Cell(1, 5).Range.Text = "Room Width";
                        tbl.Cell(1, 6).Range.Text = "PERMISSIBLE Min Width";
                        tbl.Cell(1, 7).Range.Text = "Room Height";
                        tbl.Cell(1, 8).Range.Text = "PERMISSIBLE Min Height";
                        tbl.Cell(1, 9).Range.Text = "COMPLY";
                        tbl.Cell(1, 10).Range.Text = "Remarks";
                        int rowCnt = 1;
                        while (Roomsreader.HasRows && Roomsreader.Read())
                        {
                            rowCnt++;
                            string roomstr = Roomsreader.GetValue(6).ToString();
                            switch (roomstr)
                            {
                                case "K":
                                    roomstr = "Kitchen";
                                    break;
                                case "KD":
                                    roomstr = "Kitchen and Dining";
                                    break;
                                case "B":
                                    roomstr = "Bathroom";
                                    break;
                                case "WC":
                                    roomstr = "Water Closet";
                                    break;
                                case "BWC":
                                    roomstr = "Bath and Water Closet";
                                    break;
                                case "BED":
                                    roomstr = "Bed Room";
                                    break;
                                case "SR":
                                    roomstr = "Store Room";
                                    break;
                                case "OT":
                                    roomstr = "Other Rooms";
                                    break;
                                default:

                                    break;
                            }

                            tbl.Cell(rowCnt, 1).Range.Text = Roomsreader.GetValue(7).ToString(); ;
                            tbl.Cell(rowCnt, 2).Range.Text = roomstr;
                            tbl.Cell(rowCnt, 3).Range.Text = Roomsreader.GetValue(8).ToString();
                            tbl.Cell(rowCnt, 4).Range.Text = Roomsreader.GetValue(9).ToString();
                            tbl.Cell(rowCnt, 5).Range.Text = Roomsreader.GetValue(10).ToString();
                            tbl.Cell(rowCnt, 6).Range.Text = Roomsreader.GetValue(11).ToString();
                            tbl.Cell(rowCnt, 7).Range.Text = Roomsreader.GetValue(12).ToString();
                            tbl.Cell(rowCnt, 8).Range.Text = Roomsreader.GetValue(13).ToString();
                            tbl.Cell(rowCnt, 9).Range.Text = Roomsreader.GetValue(14).ToString();
                            tbl.Cell(rowCnt, 10).Range.Text = Roomsreader.GetValue(15).ToString();
                            tbl.Rows[rowCnt].Range.Font.Color = WdColor.wdColorDarkRed;
                        }
                            //rng.InsertParagraphAfter();
                            Roomsreader.Close();
                    }

                    rng.InsertParagraphAfter();
                    rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                    rng.Paragraphs.Add(ref missing);
                    //rng.InsertParagraphAfter();
                    rng.Text = "Except above rooms all rooms are as per byelaws";
                    rng.Font.Name = "Verdana";
                    rng.Font.Size = 10;
                    rng.Font.Color = WdColor.wdColorBlue;
                    rng.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    //rng.ParagraphFormat.LineSpacing = 0;
                    rng.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
                    rng.InsertParagraphAfter();

                }
            }
                
            else
            {
                    Paragraph oPara5;
                    rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                    object parang2 = rng;
                    oPara5 = doc.Content.Paragraphs.Add(ref parang2);
                    oPara5.Range.InsertParagraphBefore();
                    oPara5.Range.Text = "There is no Rooms found.";
                    oPara5.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                    oPara5.Range.Font.Size = 11;
                    oPara5.Range.Font.Color = WdColor.wdColorAutomatic;
                    oPara5.Range.InsertParagraphAfter();
                }
            }
            reader.Close();
            log.Debug("RoomReport() - Generated Room Report");
        }

        private void OtherErrorsReport(Document doc, string APP_ID, MySqlConnection con, int buildTypeId)
        {
            // Variable Declaration
            MySqlCommand cmd = null;
            MySqlDataReader reader1 = null;
            object missing = null;
            object oEndOfDoc = null;
            Range rng = null;
            try
            {
                // Executing DB2 Execute Reader Command
                cmd = new MySqlCommand( ";select count(1) from GENERAL_ERRORS WHERE FR_ID_VER = '" + APP_ID + "' AND (COMPLY = 'No'or COMPLY = 'NO')", con);

                reader1 = cmd.ExecuteReader();
                missing = System.Reflection.Missing.Value;
                oEndOfDoc = "\\endofdoc";
                rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                // check for Reader has rows or not
                if (reader1.Read() == true)
                {
                    int ErrorCnt = reader1.GetInt32(0);
                    reader1.Close();
                    MySqlCommand cmd1 = new MySqlCommand( ";select * from GENERAL_ERRORS WHERE FR_ID_VER ='" + APP_ID + "' AND (COMPLY = 'No'or COMPLY = 'NO') ;", con);
                    MySqlDataReader reader = cmd1.ExecuteReader();
                    rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                    object parang = rng;
                    Paragraph oPara4 = doc.Content.Paragraphs.Add(ref parang);
                    oPara4.Range.Text = "Report for General Errors";
                    oPara4.Range.Font.Name = "Verdana";
                    oPara4.Range.Font.Size = 10;
                    oPara4.Range.Font.Color = WdColor.wdColorAutomatic;
                    oPara4.Range.Font.Underline = WdUnderline.wdUnderlineSingle;
                    rng.InsertParagraphAfter();
                    rng.InsertParagraphAfter();
                    rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                    object objDefaultBehaviorWord8 = WdDefaultTableBehavior.wdWord9TableBehavior;
                    object objAutoFitFixed = WdAutoFitBehavior.wdAutoFitFixed;
                    Table tbl = doc.Tables.Add(rng, ErrorCnt + 1, 3, ref objDefaultBehaviorWord8, ref objAutoFitFixed);
                    tbl.Range.Font.Size = 7;
                    tbl.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                    //tbl.ApplyStyleColumnBands = true;
                    tbl.Cell(1, 1).Range.Text = "FR_ID_VER";
                    tbl.Cell(1, 2).Range.Text = "COMPLY";
                    tbl.Cell(1, 3).Range.Text = "REMARKS";
                    int rowCnt = 1;
                    // if reader has data then its printing the report.
                    while (reader.HasRows && reader.Read())
                    {
                        rowCnt++;
                        tbl.Cell(rowCnt, 1).Range.Text = reader.GetValue(0).ToString();
                        tbl.Cell(rowCnt, 2).Range.Text = reader.GetValue(1).ToString();
                        tbl.Cell(rowCnt, 3).Range.Text = reader.GetValue(2).ToString();
                        if (reader.GetValue(1).ToString().ToUpper() == "NO")
                        {
                            tbl.Rows[rowCnt].Range.Font.Color = WdColor.wdColorDarkRed;
                           

                        }
                    }

                    rng.InsertParagraphAfter();
                    rng.InsertParagraphAfter();
                    reader.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }


        public void CommunityHallReport(Document doc, string APP_ID, MySqlConnection con)
        {
            log.Debug("CommunityHallReport() - Generating Setback Report");
            MySqlCommand cmd1 = new MySqlCommand("select count(1) from RE_RES_RGH_COMMUNITYHALLS WHERE FR_ID_VER ='" + APP_ID + "' ;", con);
            MySqlDataReader reader1 = cmd1.ExecuteReader();

            object missing = System.Reflection.Missing.Value;
            object oEndOfDoc = "\\endofdoc";
            Range rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            if (reader1.Read() == true)
            {
                int ErrorCnt = reader1.GetInt32(0);
                reader1.Close();

                MySqlCommand cmd = new MySqlCommand("select * from RE_RES_RGH_COMMUNITYHALLS WHERE FR_ID_VER ='" + APP_ID + "';", con);
                MySqlDataReader reader = cmd.ExecuteReader();
                rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                object parang = rng;
                Paragraph oPara4 = doc.Content.Paragraphs.Add(ref parang);
                oPara4.Range.Text = "Report for COMMUNITY HALLS";
                oPara4.Range.Font.Name = "Verdana";
                oPara4.Range.Font.Size = 10;
                oPara4.Range.Font.Color = WdColor.wdColorAutomatic;
                oPara4.Range.Font.Underline = WdUnderline.wdUnderlineSingle;

                rng.InsertParagraphAfter();
                rng.InsertParagraphAfter();
                rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;

                object objDefaultBehaviorWord8 = WdDefaultTableBehavior.wdWord9TableBehavior;
                object objAutoFitFixed = WdAutoFitBehavior.wdAutoFitFixed;
                Table tbl = doc.Tables.Add(rng, ErrorCnt + 1, 5, ref objDefaultBehaviorWord8, ref objAutoFitFixed);
                tbl.Range.Font.Size = 7;
                tbl.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                //tbl.ApplyStyleColumnBands = true;
                tbl.Cell(1, 1).Range.Text = "HALL TYPE";
                tbl.Cell(1, 2).Range.Text = "HALL NUMBER";
                tbl.Cell(1, 3).Range.Text = "HALL AREA";
                tbl.Cell(1, 4).Range.Text = "COMPLY";
                tbl.Cell(1, 5).Range.Text = "REMARKS";
                int rowCnt = 1;
                while (reader.HasRows && reader.Read())
                {
                    rowCnt++;
                    tbl.Cell(rowCnt, 1).Range.Text = reader.GetValue(1).ToString();
                    tbl.Cell(rowCnt, 2).Range.Text = reader.GetValue(3).ToString();
                    tbl.Cell(rowCnt, 3).Range.Text = reader.GetValue(4).ToString();
                    tbl.Cell(rowCnt, 4).Range.Text = reader.GetValue(5).ToString();
                    tbl.Cell(rowCnt, 5).Range.Text = reader.GetValue(6).ToString();
                    if (reader.GetValue(5).ToString() == "NO")
                    {
                        tbl.Rows[rowCnt].Range.Font.Color = WdColor.wdColorDarkRed;
                    }
                }

                rng.InsertParagraphAfter();
                rng.InsertParagraphAfter();
                reader.Close();
            }
            reader1.Close();
            log.Debug("CommunityHallReport() - Generated Setback Report");
        }


        public void FarmHouseFAR(Document doc, string APP_ID, MySqlConnection con)
        {
            log.Debug("FarmHouseFAR() - Generating FAR Report");
            MySqlCommand cmd1 = new MySqlCommand( ";select count(1) from RE_R110_FAR WHERE FR_ID_VER ='" + APP_ID + "' ;", con);
            MySqlDataReader reader1 = cmd1.ExecuteReader();

            object missing = System.Reflection.Missing.Value;
            object oEndOfDoc = "\\endofdoc";
            Range rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            if (reader1.Read() == true)
            {
                int ErrorCnt = reader1.GetInt32(0);

                MySqlCommand cmd = new MySqlCommand("select * from RE_R110_FAR WHERE FR_ID_VER ='" + APP_ID + "';", con);
                MySqlDataReader reader = cmd.ExecuteReader();
                rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                object parang = rng;
                Paragraph oPara4 = doc.Content.Paragraphs.Add(ref parang);
                oPara4.Range.Text = "Report for FAR";
                oPara4.Range.Font.Name = "Verdana";
                oPara4.Range.Font.Size = 10;
                oPara4.Range.Font.Color = WdColor.wdColorAutomatic;
                oPara4.Range.Font.Underline = WdUnderline.wdUnderlineSingle;

                rng.InsertParagraphAfter();
                rng.InsertParagraphAfter();
                rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;

                object objDefaultBehaviorWord8 = WdDefaultTableBehavior.wdWord9TableBehavior;
                object objAutoFitFixed = WdAutoFitBehavior.wdAutoFitFixed;
                Table tbl = doc.Tables.Add(rng, ErrorCnt + 1, 8, ref objDefaultBehaviorWord8, ref objAutoFitFixed);
                tbl.Range.Font.Size = 7;
                tbl.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                //tbl.ApplyStyleColumnBands = true;
                tbl.Cell(1, 1).Range.Text = "PLOT AREA";
                tbl.Cell(1, 2).Range.Text = "CONSIDERED PLOT AREA";
                tbl.Cell(1, 3).Range.Text = "TOTAL COVERED AREA";
                tbl.Cell(1, 4).Range.Text = "PROPOSED FAR";
                tbl.Cell(1, 5).Range.Text = "PERMISSIBLE FAR";
                tbl.Cell(1, 6).Range.Text = "Excess FAR";
                tbl.Cell(1, 7).Range.Text = "COMPLY";
                tbl.Cell(1, 8).Range.Text = "REMARKS";
                int rowCnt = 1;                      
                while (reader.HasRows && reader.Read())
                {
                    rowCnt++;
                    tbl.Cell(rowCnt, 1).Range.Text = reader.GetValue(1).ToString();
                    tbl.Cell(rowCnt, 2).Range.Text = reader.GetValue(2).ToString();
                    tbl.Cell(rowCnt, 3).Range.Text = reader.GetValue(3).ToString();
                    tbl.Cell(rowCnt, 4).Range.Text = reader.GetValue(4).ToString();
                    tbl.Cell(rowCnt, 5).Range.Text = reader.GetValue(5).ToString();
                    tbl.Cell(rowCnt, 6).Range.Text = reader.GetValue(6).ToString();
                    tbl.Cell(rowCnt, 7).Range.Text = reader.GetValue(7).ToString();
                    tbl.Cell(rowCnt, 8).Range.Text = reader.GetValue(8).ToString();
                    if (reader.GetValue(7).ToString() == "NO")
                    {
                        tbl.Rows[rowCnt].Range.Font.Color = WdColor.wdColorDarkRed;
                    }
                }

                rng.InsertParagraphAfter();
                rng.InsertParagraphAfter();
                reader.Close();
            }
            reader1.Close();
            log.Debug("FarmHouseFAR() - Generated FAR Report");
        }

        public void FarmHouseFEE(Document doc, string APP_ID, MySqlConnection con)
        {
            log.Debug("FarmHouseFEE() - Generating FAR Report");
            MySqlCommand cmd1 = new MySqlCommand( ";select count(1) from RE_R110_FEE WHERE FR_ID_VER ='" + APP_ID + "' ;", con);
            MySqlDataReader reader1 = cmd1.ExecuteReader();

            object missing = System.Reflection.Missing.Value;
            object oEndOfDoc = "\\endofdoc";
            Range rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            if (reader1.Read() == true)
            {
                int ErrorCnt = reader1.GetInt32(0);

                MySqlCommand cmd = new MySqlCommand("select * from RE_R110_FEE WHERE FR_ID_VER ='" + APP_ID + "';", con);
                MySqlDataReader reader = cmd.ExecuteReader();
                rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                object parang = rng;
                Paragraph oPara4 = doc.Content.Paragraphs.Add(ref parang);
                oPara4.Range.Text = "Report for FEE";
                oPara4.Range.Font.Name = "Verdana";
                oPara4.Range.Font.Size = 10;
                oPara4.Range.Font.Color = WdColor.wdColorAutomatic;
                oPara4.Range.Font.Underline = WdUnderline.wdUnderlineSingle;

                rng.InsertParagraphAfter();
                rng.InsertParagraphAfter();
                rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;

                object objDefaultBehaviorWord8 = WdDefaultTableBehavior.wdWord9TableBehavior;
                object objAutoFitFixed = WdAutoFitBehavior.wdAutoFitFixed;
                Table tbl = doc.Tables.Add(rng, ErrorCnt + 1, 5, ref objDefaultBehaviorWord8, ref objAutoFitFixed);
                tbl.Range.Font.Size = 7;
                tbl.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                //tbl.ApplyStyleColumnBands = true;
               
                tbl.Cell(1, 1).Range.Text = "CONVERSION CHARGE";
                tbl.Cell(1, 2).Range.Text = "PENALTY_AMOUNT";
                tbl.Cell(1, 3).Range.Text = "REBATE";                
                tbl.Cell(1, 4).Range.Text = "ADDITIONAL_CHARGES";
                tbl.Cell(1, 5).Range.Text = "TOTAL_FEE";
                int rowCnt = 1;
                while (reader.HasRows && reader.Read())
                {
                    rowCnt++;
                    tbl.Cell(rowCnt, 1).Range.Text = reader.GetValue(1).ToString();
                    tbl.Cell(rowCnt, 2).Range.Text = reader.GetValue(4).ToString();
                    tbl.Cell(rowCnt, 3).Range.Text = reader.GetValue(7).ToString();
                    tbl.Cell(rowCnt, 4).Range.Text = reader.GetValue(10).ToString();
                    tbl.Cell(rowCnt, 5).Range.Text = reader.GetValue(13).ToString();                                        
                }

                rng.InsertParagraphAfter();
                rng.InsertParagraphAfter();
                reader.Close();
            }
            reader1.Close();
            log.Debug("FarmHouseFEE() - Generated FEE Report");
        }


        public void VentilationReport(Document doc, string APP_ID, MySqlConnection con, int buildTypeId)
        {
            log.Debug("VentilationReport() - Generating Ventilation Report");
            MySqlCommand cmd = new MySqlCommand( ";select count(1) from (SELECT DISTINCT BLDG_NO,FL_NO,DU_NO,ROOM_NO FROM RE_VENTILATION WHERE FR_ID_VER ='" + APP_ID + "' AND (COMPLY = 'No'or COMPLY = 'NO'))AS subquery;", con);
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Close();
            MySqlCommand dwgcmd = new MySqlCommand("select count(1) from (SELECT M_VENTILATION FROM file WHERE FR_ID_VER ='" + APP_ID + "')AS subquery", con);
            dwgcmd.Parameters.AddWithValue("@appId", APP_ID);
            MySqlDataReader dwgreader = dwgcmd.ExecuteReader();
            object missing = System.Reflection.Missing.Value;
            object oEndOfDoc = "\\endofdoc";
            Range rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            rng.InsertParagraphAfter();
            //rng.InsertParagraphAfter();
            rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            rng.InsertParagraphAfter();
            object parang = rng;
            Paragraph oPara4 = doc.Content.Paragraphs.Add(ref parang);
            oPara4.Range.Text = "Report for Ventilation";
            oPara4.Range.Font.Name = "Verdana";
            oPara4.Range.Font.Size = 11;
            oPara4.Range.Font.Color = WdColor.wdColorAutomatic;
            oPara4.Range.Font.Underline = WdUnderline.wdUnderlineSingle;
            rng.InsertParagraphAfter();

            if (dwgreader.Read() == true)
            {
                dwgreader.Close();
                dwgcmd.Dispose();
                MySqlCommand drawingcmd = new MySqlCommand("SELECT * FROM file WHERE FR_ID_VER ='" + APP_ID + "';", con);
                MySqlDataReader drawingreader = drawingcmd.ExecuteReader();
                while (drawingreader.HasRows && drawingreader.Read())
                {

                    if (drawingreader.GetDecimal(11) == 'N')
                    {
                        MySqlCommand cmd1 = new MySqlCommand("select count(1) from (SELECT DISTINCT BLDG_NO,FL_NO,DU_NO,ROOM_NO FROM RE_VENTILATION WHERE FR_ID_VER ='" + APP_ID + "' AND (COMPLY = 'No'or COMPLY = 'NO'))AS subquery;", con);
                        MySqlDataReader reader8 = cmd1.ExecuteReader();
                        if (reader8.Read() == true)
                        {
                            int ErrorCnt = reader8.GetInt32(0);
                            if (ErrorCnt == 0)
                            {

                                rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                                rng.InsertParagraphAfter();

                                rng.Paragraphs.Add(ref missing);
                                //rng.InsertParagraphAfter();
                                rng.Text = "All Rooms ventilations are as per byeLaws.";
                                rng.Font.Name = "Verdana";
                                rng.Font.Size = 11;
                                rng.Font.Color = WdColor.wdColorBlue;
                                rng.Font.Underline = WdUnderline.wdUnderlineNone;
                                rng.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                                //rng.ParagraphFormat.LineSpacing = 0;
                                rng.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
                                rng.InsertParagraphAfter();
                                rng.InsertParagraphAfter();
                                reader8.Close();
                            }
                            else
                            {
                                reader8.Close();
                                MySqlCommand cmd4 = new MySqlCommand( ";SELECT DISTINCT BLDG_NO,FL_NO,DU_NO,FLR_CODE FROM RE_VENTILATION WHERE FR_ID_VER ='" + APP_ID + "' AND (COMPLY = 'No'or COMPLY = 'NO')AS subquery;", con);
                                MySqlDataReader reader1 = cmd4.ExecuteReader();


                                //rng.InsertParagraphAfter();
                                //rng.InsertParagraphAfter();

                                while (reader1.HasRows && reader1.Read())
                                {

                                    //rng.InsertParagraphAfter();
                                    short BldgNo = reader1.GetInt16(0);
                                    short FloorNo = reader1.GetInt16(1);
                                    short DwellingNO = reader1.GetInt16(2);
                                    string FloorCode = reader1.GetString(3);
                                    string tmpstr = "floor no: " + FloorNo.ToString();
                                    if (FloorNo == 0)
                                    {
                                        switch (FloorCode.ToUpper())
                                        {
                                            case "G":
                                                {
                                                    tmpstr = "in Ground floor";
                                                    break;
                                                }
                                            case "B":
                                                {
                                                    tmpstr = "in Basement";
                                                    break;
                                                }
                                            case "T":
                                                {
                                                    tmpstr = "in Terrece floor";
                                                    break;
                                                }
                                        }
                                    }
                                    rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                                    parang = rng;
                                    oPara4 = doc.Content.Paragraphs.Add(ref parang);
                                    //oPara4.Range.InsertParagraphBefore();
                                    if (buildTypeId == 117 || buildTypeId == 118)
                                    {
                                        oPara4.Range.Text = "Report for ventilation  from building no : " + BldgNo.ToString() + " and " + tmpstr;
                                    }
                                    else
                                    {
                                        oPara4.Range.Text = "Report for ventilation In dwelling unit : " + DwellingNO.ToString() + "    from building no : " + BldgNo.ToString() + " and " + tmpstr;
                                    }

                                    oPara4.Range.Font.Name = "Verdana";
                                    oPara4.Range.Font.Size = 9;
                                    oPara4.Range.Font.Underline = WdUnderline.wdUnderlineNone;

                                    //oPara4.Format.SpaceAfter = 24;
                                    //oPara4.Range.InsertParagraphAfter();
                                    rng.InsertParagraphAfter();
                                    //rng.InsertParagraphAfter();

                                    reader1.Close();
                                    MySqlCommand Venticmd1 = new MySqlCommand( ";select count(1) from (select * from RE_VENTILATION where FR_ID_VER ='" + APP_ID + "' AND BLDG_NO = " + BldgNo + " AND FL_NO = " + FloorNo + " AND DU_NO = " + DwellingNO + " AND (COMPLY = 'No'or COMPLY = 'NO'))AS subquery;", con);
                                    MySqlDataReader VEntireader1 = Venticmd1.ExecuteReader();
                                    rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                                    VEntireader1.Read();
                                    int FC = VEntireader1.GetInt32(0);
                                    VEntireader1.Close();
                                    MySqlCommand Venticmd = new MySqlCommand( ";select * from RE_VENTILATION where FR_ID_VER ='" + APP_ID + "' AND BLDG_NO = " + BldgNo + " AND FL_NO = " + FloorNo + " AND DU_NO = " + DwellingNO + " AND (COMPLY = 'No'or COMPLY = 'NO')AS subquery;", con);
                                    MySqlDataReader VEntireader = Venticmd.ExecuteReader();
                                    object objDefaultBehaviorWord8 = WdDefaultTableBehavior.wdWord9TableBehavior;
                                    object objAutoFitFixed = WdAutoFitBehavior.wdAutoFitFixed;
                                    Table tbl = doc.Tables.Add(rng, FC + 1, 6, ref objDefaultBehaviorWord8, ref objAutoFitFixed);
                                    tbl.Range.Font.Size = 7;
                                    //tbl.ApplyStyleColumnBands = true;
                                    tbl.Cell(1, 1).Range.Text = "Room No";
                                    tbl.Cell(1, 2).Range.Text = "Room Ventilation Area";
                                    tbl.Cell(1, 3).Range.Text = "Room Area";
                                    tbl.Cell(1, 4).Range.Text = "PERMISSIBLE Min Ventilation";
                                    tbl.Cell(1, 5).Range.Text = "COMPLY";
                                    tbl.Cell(1, 6).Range.Text = "Remarks";
                                    int rowCnt = 1;
                                    while (VEntireader.HasRows && VEntireader.Read())
                                    {
                                        rowCnt++;

                                        tbl.Cell(rowCnt, 1).Range.Text = VEntireader.GetValue(5).ToString();
                                        tbl.Cell(rowCnt, 2).Range.Text = VEntireader.GetValue(7).ToString();
                                        tbl.Cell(rowCnt, 3).Range.Text = VEntireader.GetValue(8).ToString();
                                        tbl.Cell(rowCnt, 4).Range.Text = VEntireader.GetValue(9).ToString();
                                        tbl.Cell(rowCnt, 5).Range.Text = VEntireader.GetValue(10).ToString();
                                        tbl.Cell(rowCnt, 6).Range.Text = VEntireader.GetValue(11).ToString();
                                        tbl.Rows[rowCnt].Range.Font.Color = WdColor.wdColorDarkRed;
                                    }
                                    //rng.InsertParagraphAfter();
                                    VEntireader.Close();
                                }
                                //rng.InsertParagraphAfter();
                                rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                                //rng.InsertParagraphAfter();
                                rng.Paragraphs.Add(ref missing);
                                //rng.InsertParagraphAfter();
                                rng.Text = "Except above rooms all room's ventilation is as per byelaws";
                                rng.Font.Name = "Verdana";
                                rng.Font.Size = 11;
                                rng.Font.Color = WdColor.wdColorBlue;
                                rng.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                                //rng.ParagraphFormat.LineSpacing = 0;
                                //rng.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
                                rng.InsertParagraphAfter();
                                rng.InsertParagraphAfter();
                              
                            }
                        }
                        else
                        {

                        }

                    }
                    else
                    {
                        rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                        rng.InsertParagraphAfter();

                        rng.Paragraphs.Add(ref missing);
                        //rng.InsertParagraphAfter();
                        rng.Text = "As per Architect’s confirmation Mechanical Ventilation is provided to the building.";
                        rng.Font.Name = "Verdana";
                        rng.Font.Size = 11;
                        rng.Font.Color = WdColor.wdColorBlue;
                        rng.Font.Underline = WdUnderline.wdUnderlineNone;
                        rng.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        //rng.ParagraphFormat.LineSpacing = 0;
                        rng.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
                        rng.InsertParagraphAfter();
                        rng.InsertParagraphAfter();
                    }
                   
                }
                drawingreader.Close();
            }

            dwgreader.Close();
            dwgcmd.Dispose();
            log.Debug("VentilationReport() - Generated Ventilation Report");
        }

        public void ParkingReport(Document doc, string APP_ID, MySqlConnection con)
        {
            log.Debug("ParkingReport() - Generating Parking Report");
            MySqlCommand cmd1 = new MySqlCommand( ";select count(1) from RE_PARKING WHERE FR_ID_VER = '" + APP_ID + "' AND (COMPLY = 'No'or COMPLY = 'NO');", con);
            MySqlDataReader reader1 = cmd1.ExecuteReader();

            object missing = System.Reflection.Missing.Value;
            object oEndOfDoc = "\\endofdoc";
            Range rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            if (reader1.Read() == true)
            {
                int ErrorCnt = reader1.GetInt32(0);
                if (ErrorCnt == 0)
                {
                
                    rng.InsertParagraphAfter();
                    rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                    rng.InsertParagraphAfter();

                    rng.Paragraphs.Add(ref missing);
                    //rng.InsertParagraphAfter();
                    rng.Text = "All parkings slot areas are as per byeLaws.";
                    rng.Font.Name = "Verdana";
                    rng.Font.Color = WdColor.wdColorBlue;
                    rng.Font.Size = 11;
                    rng.Font.Underline = WdUnderline.wdUnderlineNone;
                    rng.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    //rng.ParagraphFormat.LineSpacing = 0;
                    rng.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
                    rng.InsertParagraphAfter();
                    rng.InsertParagraphAfter();
                }
                else
                {
                    reader1.Close();
                    MySqlCommand cmd = new MySqlCommand("select * from RE_PARKING WHERE FR_ID_VER = '" + APP_ID + "' AND (COMPLY = 'No'or COMPLY = 'NO');", con);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                    object parang = rng;
                    Paragraph oPara4 = doc.Content.Paragraphs.Add(ref parang);
                    oPara4.Range.Text = "Report for Parking";
                    oPara4.Range.Font.Name = "Verdana";
                    oPara4.Range.Font.Size = 10;
                    oPara4.Range.Font.Underline = WdUnderline.wdUnderlineNone;

                    rng.InsertParagraphAfter();
                    rng.InsertParagraphAfter();
                    rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;

                    object objDefaultBehaviorWord8 = WdDefaultTableBehavior.wdWord9TableBehavior;
                    object objAutoFitFixed = WdAutoFitBehavior.wdAutoFitFixed;
                    Table tbl = doc.Tables.Add(rng, ErrorCnt + 1, 7, ref objDefaultBehaviorWord8, ref objAutoFitFixed);
                    tbl.Range.Font.Size = 7;
                    //tbl.ApplyStyleColumnBands = true;
                    tbl.Cell(1, 1).Range.Text = "Sr.No.";
                    tbl.Cell(1, 2).Range.Text = "Parking Type";
                    tbl.Cell(1, 3).Range.Text = "Parking Slot No";
                    tbl.Cell(1, 4).Range.Text = "Parking Area";
                    tbl.Cell(1, 5).Range.Text = "PERMISSIBLE Min Area";
                    tbl.Cell(1, 6).Range.Text = "COMPLY";
                    tbl.Cell(1, 7).Range.Text = "Remarks";
                    int rowCnt = 1;
                    while (reader.HasRows && reader.Read())
                    {
                        rowCnt++;
                        string roomstr = reader.GetValue(1).ToString();
                        switch (roomstr)
                        {
                            case "OP":
                                roomstr = "Open";
                                break;
                            case "GR":
                                roomstr = "Ground Floor";
                                break;
                            case "BS":
                                roomstr = "Basement";
                                break;
                            case "ML":
                                roomstr = "Multi Level";
                                break;
                            case "AML":
                                roomstr = "Automated Multi Level";
                                break;

                            default:

                                break;
                        }
                        tbl.Cell(rowCnt, 1).Range.Text = (rowCnt - 1).ToString();
                        tbl.Cell(rowCnt, 2).Range.Text = roomstr;
                        tbl.Cell(rowCnt, 3).Range.Text = reader.GetValue(2).ToString();
                        tbl.Cell(rowCnt, 4).Range.Text = reader.GetValue(3).ToString();
                        tbl.Cell(rowCnt, 5).Range.Text = reader.GetValue(4).ToString();
                        tbl.Cell(rowCnt, 6).Range.Text = reader.GetValue(5).ToString();
                        tbl.Cell(rowCnt, 7).Range.Text = reader.GetValue(6).ToString();
                        tbl.Rows[rowCnt].Range.Font.Color = WdColor.wdColorDarkRed;
                    }
                    //rng.InsertParagraphAfter();
                    rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                    rng.InsertParagraphAfter();
                    rng.Paragraphs.Add(ref missing);
                    rng.InsertParagraphAfter();
                    rng.Text = "Except above Parking slot(s) all Parking slots are as per byelaws";
                    rng.Font.Name = "Verdana";
                    rng.Font.Size = 10;
                    rng.Font.Color = WdColor.wdColorBlue;
                    rng.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    //rng.ParagraphFormat.LineSpacing = 0;
                    //rng.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
                    rng.InsertParagraphAfter();
                    rng.InsertParagraphAfter();

                }
            }
            else
            {

            }
            reader1.Close();
            log.Debug("ParkingReport() - Generated Parking Report");
        }

        #region ShopReport

        public void ShopReport(Document doc, string APP_ID, MySqlConnection con)
        {
            log.Debug("ShopReport() - Generating ShopReport Report");

            MySqlCommand cmd1 = new MySqlCommand( ";select count(1) from RE_SHOP WHERE FR_ID_VER ='" + APP_ID + "';", con);
            MySqlDataReader reader1 = cmd1.ExecuteReader();
            if (reader1.HasRows && reader1.Read())
            {

            object missing = System.Reflection.Missing.Value;
            object oEndOfDoc = "\\endofdoc";
            Range rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            rng.InsertParagraphAfter();
                rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            rng.InsertParagraphAfter();


                object parang = rng;
                Paragraph oPara4 = doc.Content.Paragraphs.Add(ref parang);
                oPara4.Range.Text = "Report for Shops";
                oPara4.Range.Font.Name = "Verdana";
            oPara4.Range.Font.Size = 11;
                oPara4.Range.Font.Color = WdColor.wdColorAutomatic;
                oPara4.Range.Font.Underline = WdUnderline.wdUnderlineSingle;
            rng.InsertParagraphAfter();


                int ErrorCnt = reader1.GetInt32(0);
                if (ErrorCnt != 0)
                {
                    reader1.Close();
                    MySqlCommand cmd2 = new MySqlCommand("select Count(1) from RE_SHOP WHERE FR_ID_VER ='" + APP_ID + "' AND (COMPLY = 'No'or COMPLY = 'NO');", con);
                    int tableCount = (int)cmd2.ExecuteScalar();
                    MySqlDataReader reader2 = cmd1.ExecuteReader();
                    if (tableCount != 0)
                {
                        reader2.Close();
                        MySqlCommand cmd = new MySqlCommand("select * from RE_SHOP WHERE FR_ID_VER ='" + APP_ID + "';", con);
                        MySqlDataReader reader = cmd.ExecuteReader();
                    //rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;

                rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;

                object objDefaultBehaviorWord8 = WdDefaultTableBehavior.wdWord9TableBehavior;
                object objAutoFitFixed = WdAutoFitBehavior.wdAutoFitFixed;
                Table tbl = doc.Tables.Add(rng, ErrorCnt + 1, 7, ref objDefaultBehaviorWord8, ref objAutoFitFixed);
                tbl.Range.Font.Size = 7;
                tbl.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                //tbl.ApplyStyleColumnBands = true;
                tbl.Cell(1, 1).Range.Text = "Bldg No";
                tbl.Cell(1, 2).Range.Text = "Shop No";
                tbl.Cell(1, 3).Range.Text = "FloorNo.";
                tbl.Cell(1, 4).Range.Text = "Shop Area";
                tbl.Cell(1, 5).Range.Text = "Permissible Min Area";
                tbl.Cell(1, 6).Range.Text = "COMPLY";
                tbl.Cell(1, 7).Range.Text = "REMARKS";
                int rowCnt = 1;
                while (reader.HasRows && reader.Read())
                {
                    rowCnt++;
                    tbl.Cell(rowCnt, 1).Range.Text = reader.GetValue(1).ToString();
                    tbl.Cell(rowCnt, 2).Range.Text = reader.GetValue(2).ToString();
                    tbl.Cell(rowCnt, 3).Range.Text = reader.GetValue(3).ToString();
                    tbl.Cell(rowCnt, 4).Range.Text = reader.GetValue(5).ToString();
                    tbl.Cell(rowCnt, 5).Range.Text = reader.GetValue(6).ToString();
                    tbl.Cell(rowCnt, 6).Range.Text = reader.GetValue(7).ToString();
                    tbl.Cell(rowCnt, 7).Range.Text = reader.GetValue(8).ToString();
                    if (reader.GetValue(7).ToString() == "No")
                    {
                        tbl.Rows[rowCnt].Range.Font.Color = WdColor.wdColorDarkRed;
                    }
                        }
                        rng.InsertParagraphAfter();

                        rng.InsertParagraphAfter();
                    }

                    else
                    {
                        Paragraph oPara5;
                        rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                        object parang2 = rng;
                        oPara5 = doc.Content.Paragraphs.Add(ref parang2);
                        // oPara5.Range.InsertParagraphBefore();
                        oPara5.Range.Text = "All SHOPS are as per bylaws";
                        oPara5.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                        oPara5.Range.Font.Name = "Verdana";
                        oPara5.Range.Font.Size = 10;
                        oPara5.Range.Font.Color = WdColor.wdColorBlue;
                        oPara5.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        oPara5.Range.InsertParagraphAfter();
                    }
                    reader2.Close();

                }

                else
                {
                    Paragraph oPara5;
                    rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                    object parang2 = rng;
                    oPara5 = doc.Content.Paragraphs.Add(ref parang2);
                    //oPara5.Range.InsertParagraphBefore();
                    oPara5.Range.Text = "There is no SHOPS found.";
                    oPara5.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                    oPara5.Range.Font.Size = 11;
                    oPara5.Range.Font.Color = WdColor.wdColorAutomatic;
                    oPara5.Range.InsertParagraphAfter();
                }
            }
            reader1.Close();
            log.Debug("ShopReport() - Generated ShopReport Report");

        }
        #endregion
        #region OfficeReport
        public void OfficeReport(Document doc, string APP_ID, MySqlConnection con)
        {
            log.Debug("OfficeReport() - Generating OfficeReport Report");
            MySqlCommand cmd1 = new MySqlCommand( ";select count(1) from RE_OFFICE WHERE FR_ID_VER ='" + APP_ID + "' ;", con);
            MySqlDataReader reader1 = cmd1.ExecuteReader();
            if (reader1.HasRows && reader1.Read())
            {

            object missing = System.Reflection.Missing.Value;
            object oEndOfDoc = "\\endofdoc";
            Range rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            rng.InsertParagraphAfter();
                rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            rng.InsertParagraphAfter();


                object parang = rng;
                Paragraph oPara4 = doc.Content.Paragraphs.Add(ref parang);
                oPara4.Range.Text = "Report for Offices";
                oPara4.Range.Font.Name = "Verdana";
            oPara4.Range.Font.Size = 11;
            oPara4.Range.Font.Color = WdColor.wdColorAutomatic;
            oPara4.Range.Font.Underline = WdUnderline.wdUnderlineSingle;
            rng.InsertParagraphAfter();

                int ErrorCnt = reader1.GetInt32(0);
                if (ErrorCnt != 0)
                {
                    reader1.Close();
                    MySqlCommand cmd2 = new MySqlCommand("select count(1) from RE_OFFICE WHERE FR_ID_VER ='" + APP_ID + "' AND (COMPLY = 'No'or COMPLY = 'NO');", con);

                    int tableCount = (int)cmd2.ExecuteScalar();
                    MySqlDataReader reader2 = cmd2.ExecuteReader();
                    if (tableCount != 0)
                {
                        reader2.Close();
                        MySqlCommand cmd = new MySqlCommand("select * from RE_OFFICE WHERE FR_ID_VER ='" + APP_ID + "' AND (COMPLY = 'No'or COMPLY = 'NO');", con);
                        MySqlDataReader reader = cmd.ExecuteReader();
                        //rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;

                rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;

                object objDefaultBehaviorWord8 = WdDefaultTableBehavior.wdWord9TableBehavior;
                object objAutoFitFixed = WdAutoFitBehavior.wdAutoFitFixed;
                Table tbl = doc.Tables.Add(rng, ErrorCnt + 1, 7, ref objDefaultBehaviorWord8, ref objAutoFitFixed);
                tbl.Range.Font.Size = 7;
                tbl.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                //tbl.ApplyStyleColumnBands = true;
                tbl.Cell(1, 1).Range.Text = "Bldg No";
                tbl.Cell(1, 2).Range.Text = "Office No";
                tbl.Cell(1, 3).Range.Text = "FloorNo.";
                tbl.Cell(1, 4).Range.Text = "Office Area";
                tbl.Cell(1, 5).Range.Text = "Permissible Min Area";
                tbl.Cell(1, 6).Range.Text = "COMPLY";
                tbl.Cell(1, 7).Range.Text = "REMARKS";
                int rowCnt = 1;
                while (reader.HasRows && reader.Read())
                {
                    rowCnt++;
                    tbl.Cell(rowCnt, 1).Range.Text = reader.GetValue(1).ToString();
                    tbl.Cell(rowCnt, 2).Range.Text = reader.GetValue(2).ToString();
                    tbl.Cell(rowCnt, 3).Range.Text = reader.GetValue(3).ToString();
                    tbl.Cell(rowCnt, 4).Range.Text = reader.GetValue(5).ToString();
                    tbl.Cell(rowCnt, 5).Range.Text = reader.GetValue(6).ToString();
                    tbl.Cell(rowCnt, 6).Range.Text = reader.GetValue(7).ToString();
                    tbl.Cell(rowCnt, 7).Range.Text = reader.GetValue(8).ToString();
                    if (reader.GetValue(7).ToString() == "No")
                    {
                        tbl.Rows[rowCnt].Range.Font.Color = WdColor.wdColorDarkRed;
                    }
                        }

                        rng.InsertParagraphAfter();

                        rng.InsertParagraphAfter();
                    }
                    else
                    {
                        Paragraph oPara5;
                        rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                        object parang2 = rng;
                        oPara5 = doc.Content.Paragraphs.Add(ref parang2);
                        //oPara5.Range.InsertParagraphBefore();
                        oPara5.Range.Text = "All OFFICES are as per bylaws";
                        oPara5.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                        oPara5.Range.Font.Name = "Verdana";
                        oPara5.Range.Font.Size = 10;
                        oPara5.Range.Font.Color = WdColor.wdColorBlue;
                        oPara5.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        oPara5.Range.InsertParagraphAfter();
                    }




                }
                else
                {
                    Paragraph oPara5;
                    rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                    object parang2 = rng;
                    oPara5 = doc.Content.Paragraphs.Add(ref parang2);
                    //oPara5.Range.InsertParagraphBefore();
                    oPara5.Range.Text = "There is no OFFICES found.";
                    oPara5.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                    oPara5.Range.Font.Size = 11;
                    oPara5.Range.Font.Color = WdColor.wdColorAutomatic;
                    oPara5.Range.InsertParagraphAfter();
                }
            }
            reader1.Close();
            log.Debug("OfficeReport() - Generated OfficeReport Report");

        }
        #endregion

        #region CarLiftReport
        public void CarLiftReport(Document doc, string APP_ID, MySqlConnection con)
        {
            log.Debug("CarLiftReport() - Generating  CarLiftReport Report");
            MySqlCommand cmd1 = new MySqlCommand( ";select count(1) from RE_CARLIFT WHERE FR_ID_VER ='" + APP_ID + "' ;", con);
            MySqlDataReader reader1 = cmd1.ExecuteReader();
            if (reader1.HasRows && reader1.Read())
            {

            object missing = System.Reflection.Missing.Value;
            object oEndOfDoc = "\\endofdoc";
            Range rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            rng.InsertParagraphAfter();
                rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            rng.InsertParagraphAfter();


                object parang = rng;
                Paragraph oPara4 = doc.Content.Paragraphs.Add(ref parang);
                oPara4.Range.Text = "Report for CarLifts";
                oPara4.Range.Font.Name = "Verdana";
            oPara4.Range.Font.Size = 11;
            oPara4.Range.Font.Color = WdColor.wdColorAutomatic;
            oPara4.Range.Font.Underline = WdUnderline.wdUnderlineSingle;
            rng.InsertParagraphAfter();

                int ErrorCnt = reader1.GetInt32(0);
                if (ErrorCnt != 0)
                {
                    reader1.Close();
                    MySqlCommand cmd2 = new MySqlCommand("select count(1) from RE_CARLIFT WHERE FR_ID_VER ='" + APP_ID + "' AND (COMPLY = 'No'or COMPLY = 'NO');", con);
                    int tableCount = (int)cmd2.ExecuteScalar();
                    MySqlDataReader reader2 = cmd2.ExecuteReader();
                    if (tableCount != 0)
                {
                        reader2.Close();

                        MySqlCommand cmd = new MySqlCommand("select * from RE_CARLIFT WHERE FR_ID_VER ='" + APP_ID + "' AND (COMPLY = 'No'or COMPLY = 'NO');", con);
                        MySqlDataReader reader = cmd.ExecuteReader();
                        //rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;

                rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;

                object objDefaultBehaviorWord8 = WdDefaultTableBehavior.wdWord9TableBehavior;
                object objAutoFitFixed = WdAutoFitBehavior.wdAutoFitFixed;
                Table tbl = doc.Tables.Add(rng, ErrorCnt + 1, 8, ref objDefaultBehaviorWord8, ref objAutoFitFixed);
                tbl.Range.Font.Size = 7;
                tbl.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                //tbl.ApplyStyleColumnBands = true;
                tbl.Cell(1, 1).Range.Text = "Bldg No";
                tbl.Cell(1, 2).Range.Text = "Carlift No";
                tbl.Cell(1, 3).Range.Text = "MinPermissible Width";
                tbl.Cell(1, 4).Range.Text = "CarLift Width";
                tbl.Cell(1, 5).Range.Text = "MinPermissible Length";
                tbl.Cell(1, 6).Range.Text = "Carlift Length";
                tbl.Cell(1, 7).Range.Text = "COMPLY";
                tbl.Cell(1, 8).Range.Text = "REMARKS";
                int rowCnt = 1;
                while (reader.HasRows && reader.Read())
                {
                    rowCnt++;
                    tbl.Cell(rowCnt, 1).Range.Text = reader.GetValue(1).ToString();
                    tbl.Cell(rowCnt, 2).Range.Text = reader.GetValue(2).ToString();
                    tbl.Cell(rowCnt, 3).Range.Text = reader.GetValue(3).ToString();
                    tbl.Cell(rowCnt, 4).Range.Text = reader.GetValue(4).ToString();
                    tbl.Cell(rowCnt, 5).Range.Text = reader.GetValue(5).ToString();
                    tbl.Cell(rowCnt, 6).Range.Text = reader.GetValue(6).ToString();
                    tbl.Cell(rowCnt, 7).Range.Text = reader.GetValue(7).ToString();
                    tbl.Cell(rowCnt, 8).Range.Text = reader.GetValue(8).ToString();
                    if (reader.GetValue(7).ToString() == "No")
                    {
                        tbl.Rows[rowCnt].Range.Font.Color = WdColor.wdColorDarkRed;
                    }
                        }
                        rng.InsertParagraphAfter();

                        rng.InsertParagraphAfter();


                    }
                    else
                    {
                        Paragraph oPara5;
                        rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                        object parang2 = rng;
                        oPara5 = doc.Content.Paragraphs.Add(ref parang2);
                        // oPara5.Range.InsertParagraphBefore();
                        oPara5.Range.Text = "All CARLIFTS are as per bylaws";
                        oPara5.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                        oPara5.Range.Font.Name = "Verdana";
                        oPara5.Range.Font.Size = 10;
                        oPara5.Range.Font.Color = WdColor.wdColorBlue;
                        oPara5.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        oPara5.Range.InsertParagraphAfter();
                    }

                    reader2.Close();
                }
                else
                {
                    Paragraph oPara5;
                    rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                    object parang2 = rng;
                    oPara5 = doc.Content.Paragraphs.Add(ref parang2);
                    //oPara5.Range.InsertParagraphBefore();
                    oPara5.Range.Text = "There is no CARLIFTS found.";
                    oPara5.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                    oPara5.Range.Font.Size = 11;
                    oPara5.Range.Font.Color = WdColor.wdColorAutomatic;
                    oPara5.Range.InsertParagraphAfter();
                }
            }
            reader1.Close();
            log.Debug("CarLiftReport() - Generated CarLiftReport Report");

        }
        #endregion

        #region NotifiedRampReport

        public void NotifiedRampReport(Document doc, string APP_ID, MySqlConnection con)
        {
            log.Debug("NotifiedRampReport() - Generating NotifiedRampReport Report");
            MySqlCommand cmd1 = new MySqlCommand( ";select count(1) from RE_NOTIFIED_RAMPS WHERE FR_ID_VER ='" + APP_ID + "' ;", con);
            MySqlDataReader reader1 = cmd1.ExecuteReader();
            if (reader1.HasRows && reader1.Read())
            {

            object missing = System.Reflection.Missing.Value;
            object oEndOfDoc = "\\endofdoc";
            Range rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            rng.InsertParagraphAfter();
            rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            rng.InsertParagraphAfter();


            object parang = rng;
            Paragraph oPara4 = doc.Content.Paragraphs.Add(ref parang);
                oPara4.Range.Text = "Report for Ramps";
            oPara4.Range.Font.Name = "Verdana";
            oPara4.Range.Font.Size = 11;
            oPara4.Range.Font.Color = WdColor.wdColorAutomatic;
            oPara4.Range.Font.Underline = WdUnderline.wdUnderlineSingle;
            rng.InsertParagraphAfter();

                int ErrorCnt = reader1.GetInt32(0);
                if (ErrorCnt != 0)
                {
                    reader1.Close();
                    MySqlCommand cmd2 = new MySqlCommand("select count(1) from RE_NOTIFIED_RAMPS WHERE FR_ID_VER ='" + APP_ID + "' AND (COMPLY = 'No'or COMPLY = 'NO');", con);
                    int tableCount = (int)cmd2.ExecuteScalar();
                    MySqlDataReader reader2 = cmd2.ExecuteReader();
                    if (tableCount != 0)
                {
                        reader2.Close();
                        MySqlCommand cmd = new MySqlCommand("select * from RE_NOTIFIED_RAMPS WHERE FR_ID_VER ='" + APP_ID + "' AND (COMPLY = 'No'or COMPLY = 'NO');", con);
                        MySqlDataReader reader = cmd.ExecuteReader();
                        // rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;

                object objDefaultBehaviorWord8 = WdDefaultTableBehavior.wdWord9TableBehavior;
                object objAutoFitFixed = WdAutoFitBehavior.wdAutoFitFixed;
                Table tbl = doc.Tables.Add(rng, ErrorCnt + 1, 9, ref objDefaultBehaviorWord8, ref objAutoFitFixed);
                tbl.Range.Font.Size = 7;
                tbl.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                //tbl.ApplyStyleColumnBands = true;
                tbl.Cell(1, 1).Range.Text = "Bldg No";
                tbl.Cell(1, 2).Range.Text = "Ramp No";
                tbl.Cell(1, 3).Range.Text = "Ramp Width";
                tbl.Cell(1, 4).Range.Text = "Permissible Ramp Width";
                tbl.Cell(1, 5).Range.Text = "Min Permissible Ramp Ratio";
                tbl.Cell(1, 6).Range.Text = "Ramp Slope Ratio";
                tbl.Cell(1, 7).Range.Text = "Max Permissible Ramp Ratio";
                tbl.Cell(1, 8).Range.Text = "COMPLY";
                tbl.Cell(1, 9).Range.Text = "REMARKS";
                int rowCnt = 1;
                while (reader.HasRows && reader.Read())
                {
                    rowCnt++;
                    tbl.Cell(rowCnt, 1).Range.Text = reader.GetValue(1).ToString();
                    tbl.Cell(rowCnt, 2).Range.Text = reader.GetValue(2).ToString();
                    tbl.Cell(rowCnt, 3).Range.Text = reader.GetValue(3).ToString();
                    tbl.Cell(rowCnt, 4).Range.Text = reader.GetValue(4).ToString();
                    tbl.Cell(rowCnt, 5).Range.Text = reader.GetValue(5).ToString();
                    tbl.Cell(rowCnt, 6).Range.Text = reader.GetValue(6).ToString();
                    tbl.Cell(rowCnt, 7).Range.Text = reader.GetValue(7).ToString();
                    tbl.Cell(rowCnt, 8).Range.Text = reader.GetValue(8).ToString();
                    tbl.Cell(rowCnt, 9).Range.Text = reader.GetValue(9).ToString();
                    if (reader.GetValue(8).ToString() == "No")
                    {
                        tbl.Rows[rowCnt].Range.Font.Color = WdColor.wdColorDarkRed;
                    }
                        }

                        rng.InsertParagraphAfter();
                        rng.InsertParagraphAfter();



                    }
                    else
                    {
                        Paragraph oPara5;
                        rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                        object parang2 = rng;
                        oPara5 = doc.Content.Paragraphs.Add(ref parang2);
                        //oPara5.Range.InsertParagraphBefore();
                        oPara5.Range.Text = "All RAMPS are as per bylaws";
                        oPara5.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                        oPara5.Range.Font.Name = "Verdana";
                        oPara5.Range.Font.Size = 10;
                        oPara5.Range.Font.Color = WdColor.wdColorBlue;
                        oPara5.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        oPara5.Range.InsertParagraphAfter();
                    }
                    reader2.Close();
                }
                else
                {
                    Paragraph oPara5;
                    rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                    object parang2 = rng;
                    oPara5 = doc.Content.Paragraphs.Add(ref parang2);
                    //oPara5.Range.InsertParagraphBefore();
                    oPara5.Range.Text = "There is no RAMPS found.";
                    oPara5.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                    oPara5.Range.Font.Size = 11;
                    oPara5.Range.Font.Color = WdColor.wdColorAutomatic;
                    oPara5.Range.InsertParagraphAfter();
                }
            }
            reader1.Close();
            log.Debug("NotifiedRampReport() - Generated NotifiedRampReport Report");

        }

        #endregion

        #region CommercialFeatureCountReport
        public void CommercialFeatureCountReport(Document doc, string APP_ID, MySqlConnection con)
        {
            log.Debug("CommercialFeatureCountReport() - Generating CommercialFeatureCountReport Report");
            MySqlCommand cmd1 = new MySqlCommand( ";select count(1) from RE_COMMERCIAL_FEATURES_COUNT WHERE FR_ID_VER ='" + APP_ID + "' ;", con);
            MySqlDataReader reader1 = cmd1.ExecuteReader();
            if (reader1.HasRows && reader1.Read())
            {

            object missing = System.Reflection.Missing.Value;
            object oEndOfDoc = "\\endofdoc";
            Range rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            rng.InsertParagraphAfter();
                rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            rng.InsertParagraphAfter();


                object parang = rng;
                Paragraph oPara4 = doc.Content.Paragraphs.Add(ref parang);
                oPara4.Range.Text = "Report for CommercialFeatures";
            oPara4.Range.Font.Name = "Verdana";
            oPara4.Range.Font.Size = 11;
            oPara4.Range.Font.Color = WdColor.wdColorAutomatic;
            oPara4.Range.Font.Underline = WdUnderline.wdUnderlineSingle;
            rng.InsertParagraphAfter();

                int ErrorCnt = reader1.GetInt32(0);

                if (ErrorCnt != 0)
                {
                    reader1.Close();
                    MySqlCommand cmd2 = new MySqlCommand("select count(1) from RE_COMMERCIAL_FEATURES_COUNT WHERE FR_ID_VER ='" + APP_ID + "' AND (COMPLY = 'No'or COMPLY = 'NO');", con);
                    int tableCount = (int)cmd2.ExecuteScalar();
                    MySqlDataReader reader2 = cmd2.ExecuteReader();
                    if (tableCount != 0)
                    {
                        MySqlCommand cmd = new MySqlCommand("select * from RE_COMMERCIAL_FEATURES_COUNT WHERE FR_ID_VER ='" + APP_ID + "' AND (COMPLY = 'No'or COMPLY = 'NO');", con);
                        MySqlDataReader reader = cmd.ExecuteReader();
                        //rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;

                rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;

                object objDefaultBehaviorWord8 = WdDefaultTableBehavior.wdWord9TableBehavior;
                object objAutoFitFixed = WdAutoFitBehavior.wdAutoFitFixed;
                Table tbl = doc.Tables.Add(rng, ErrorCnt + 1, 18, ref objDefaultBehaviorWord8, ref objAutoFitFixed);
                tbl.Range.Font.Size = 7;
                tbl.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                //tbl.ApplyStyleColumnBands = true;
                tbl.Cell(1, 1).Range.Text = "Bldg No";
                tbl.Cell(1, 2).Range.Text = "Floor Code";
                tbl.Cell(1, 3).Range.Text = "Floor No";
                tbl.Cell(1, 4).Range.Text = "Total Commercial Area";
                tbl.Cell(1, 5).Range.Text = "MT Count";
                tbl.Cell(1, 6).Range.Text = "Permissible MT Count";
                tbl.Cell(1, 7).Range.Text = "FT Count";
                tbl.Cell(1, 8).Range.Text = "Permissible FT Count";
                tbl.Cell(1, 9).Range.Text = "Urinal Count";
                tbl.Cell(1, 10).Range.Text = "Permissible Urinal Count";
                tbl.Cell(1, 11).Range.Text = "WB Count";
                tbl.Cell(1, 12).Range.Text = "Permissible WB Count";
                tbl.Cell(1, 13).Range.Text = "DWF Count";
                tbl.Cell(1, 14).Range.Text = "Permissible DWF Count";
                tbl.Cell(1, 15).Range.Text = "CS Count";
                tbl.Cell(1, 16).Range.Text = "Permissible CS Count";
                tbl.Cell(1, 17).Range.Text = "Comply";
                tbl.Cell(1, 18).Range.Text = "Remarks";
                int rowCnt = 1;
                while (reader.HasRows && reader.Read())
                {
                    rowCnt++;
                    tbl.Cell(rowCnt, 1).Range.Text = reader.GetValue(1).ToString();
                    tbl.Cell(rowCnt, 2).Range.Text = reader.GetValue(2).ToString();
                    tbl.Cell(rowCnt, 3).Range.Text = reader.GetValue(3).ToString();
                    tbl.Cell(rowCnt, 4).Range.Text = reader.GetValue(4).ToString();
                    tbl.Cell(rowCnt, 5).Range.Text = reader.GetValue(5).ToString();
                    tbl.Cell(rowCnt, 6).Range.Text = reader.GetValue(6).ToString();
                    tbl.Cell(rowCnt, 7).Range.Text = reader.GetValue(7).ToString();
                    tbl.Cell(rowCnt, 8).Range.Text = reader.GetValue(8).ToString();
                    tbl.Cell(rowCnt, 9).Range.Text = reader.GetValue(9).ToString();
                    tbl.Cell(rowCnt, 10).Range.Text = reader.GetValue(10).ToString();
                    tbl.Cell(rowCnt, 11).Range.Text = reader.GetValue(11).ToString();
                    tbl.Cell(rowCnt, 12).Range.Text = reader.GetValue(12).ToString();
                    tbl.Cell(rowCnt, 13).Range.Text = reader.GetValue(13).ToString();
                    tbl.Cell(rowCnt, 14).Range.Text = reader.GetValue(14).ToString();
                    tbl.Cell(rowCnt, 15).Range.Text = reader.GetValue(15).ToString();
                    tbl.Cell(rowCnt, 16).Range.Text = reader.GetValue(16).ToString();
                    tbl.Cell(rowCnt, 17).Range.Text = reader.GetValue(17).ToString();
                    tbl.Cell(rowCnt, 18).Range.Text = reader.GetValue(18).ToString();
                    if (reader.GetValue(17).ToString() == "No")
                    {
                        tbl.Rows[rowCnt].Range.Font.Color = WdColor.wdColorDarkRed;
                    }
                        }
                        rng.InsertParagraphAfter();
                        rng.InsertParagraphAfter();
                    }

                    else
                    {
                        Paragraph oPara5;
                        rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                        object parang2 = rng;
                        oPara5 = doc.Content.Paragraphs.Add(ref parang2);
                        //oPara5.Range.InsertParagraphBefore();
                        oPara5.Range.Text = "All COMMERCIAL FEATURES as per by laws";
                        oPara5.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                        oPara5.Range.Font.Name = "Verdana";
                        oPara5.Range.Font.Size = 10;
                        oPara5.Range.Font.Color = WdColor.wdColorBlue;
                        oPara5.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        oPara5.Range.InsertParagraphAfter();
                    }
                    reader2.Close();
                }

                else
                {
                    Paragraph oPara5;
                    rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                    object parang2 = rng;
                    oPara5 = doc.Content.Paragraphs.Add(ref parang2);
                    //oPara5.Range.InsertParagraphBefore();
                    oPara5.Range.Text = "There is no COMMERCIAL FEATURES found.";
                    oPara5.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                    oPara5.Range.Font.Size = 11;
                    oPara5.Range.Font.Color = WdColor.wdColorAutomatic;
                    oPara5.Range.InsertParagraphAfter();
                }

            }
            reader1.Close();
            log.Debug("CommercialFeatureCountReport() - Generated CommercialFeatureCountReport Report");

        }
        #endregion
        public void SetbackReport(Document doc, string APP_ID, MySqlConnection con)
        {
            log.Debug("SetbackReport() - Generating Setback Report");
            MySqlCommand cmd1 = new MySqlCommand( ";select count(1) from RE_SETBACK WHERE FR_ID_VER ='" + APP_ID + "' ;", con);
            MySqlDataReader reader1 = cmd1.ExecuteReader();

            object missing = System.Reflection.Missing.Value;
            object oEndOfDoc = "\\endofdoc";
            Range rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            if (reader1.Read() == true)
            {
                int ErrorCnt = reader1.GetInt32(0);
                reader1.Close();
                MySqlCommand cmd = new MySqlCommand("select * from RE_SETBACK WHERE FR_ID_VER ='" + APP_ID + "';", con);
                MySqlDataReader reader = cmd.ExecuteReader();
                rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                object parang = rng;
                Paragraph oPara4 = doc.Content.Paragraphs.Add(ref parang);
                oPara4.Range.Text = "Report for Setbacks";
                oPara4.Range.Font.Name = "Verdana";
                oPara4.Range.Font.Size = 11;
                oPara4.Range.Font.Color = WdColor.wdColorAutomatic;
                oPara4.Range.Font.Underline = WdUnderline.wdUnderlineSingle;

                rng.InsertParagraphAfter();
                rng.InsertParagraphAfter();
                rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;

                object objDefaultBehaviorWord8 = WdDefaultTableBehavior.wdWord9TableBehavior;
                object objAutoFitFixed = WdAutoFitBehavior.wdAutoFitFixed;
                Table tbl = doc.Tables.Add(rng, ErrorCnt + 1, 5, ref objDefaultBehaviorWord8, ref objAutoFitFixed);
                tbl.Range.Font.Size = 7;
                tbl.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                //tbl.ApplyStyleColumnBands = true;
                tbl.Cell(1, 1).Range.Text = "SET CODE";
                tbl.Cell(1, 2).Range.Text = "SETBACK WIDTH";
                tbl.Cell(1, 3).Range.Text = "PERMISSIBLE SETBACK";
                tbl.Cell(1, 4).Range.Text = "COMPLY";
                tbl.Cell(1, 5).Range.Text = "REMARKS";
                int rowCnt = 1;
                while (reader.HasRows && reader.Read())
                {
                    rowCnt++;
                    string setbackcode = reader.GetValue(1).ToString().ToUpper();
                    switch (setbackcode)
                    {
                        case "R":
                            setbackcode = "Rear";
                            break;
                        case "F":
                            setbackcode = "Front";
                            break;
                        case "S1":
                            setbackcode = "Side 1";
                            break;
                        case "S2":
                            setbackcode = "Side 2";
                            break;

                        default:

                            break;
                    }
                    tbl.Cell(rowCnt, 1).Range.Text = setbackcode;
                    tbl.Cell(rowCnt, 2).Range.Text = reader.GetValue(2).ToString();
                    tbl.Cell(rowCnt, 3).Range.Text = reader.GetValue(3).ToString();
                    tbl.Cell(rowCnt, 4).Range.Text = reader.GetValue(4).ToString();
                    tbl.Cell(rowCnt, 5).Range.Text = reader.GetValue(5).ToString();
                    if (reader.GetValue(4).ToString() == "No")
                    {
                        tbl.Rows[rowCnt].Range.Font.Color = WdColor.wdColorDarkRed;
                    }
                }

                rng.InsertParagraphAfter();
                rng.InsertParagraphAfter();
                reader.Close();
            }
            reader1.Close();
            
            log.Debug("SetbackReport() - Generated Setback Report");
        }
       
        public void IndustrialSetbackReport(Document doc, string APP_ID, MySqlConnection con)
        {
            log.Debug("IndustrialSetbackReport() - Generating IndustrialSetback Report");
            MySqlCommand cmd1 = new MySqlCommand(";select count(1) from I117_RE_SETBACK WHERE FR_ID_VER ='" + APP_ID + "' ;", con);
            cmd1.Parameters.AddWithValue("@APP_ID", APP_ID);
            MySqlDataReader reader1 = null;
            MySqlDataReader reader = null;
            try
            {
                reader1 = cmd1.ExecuteReader();
                object oEndOfDoc = "\\endofdoc";
                object missing = System.Reflection.Missing.Value;
                Range rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                if (reader1.Read() == true)
                {
                    int ErrorCnt = reader1.GetInt32(0);
                    reader1.Close();
                    MySqlCommand cmd = new MySqlCommand("select * from I117_RE_SETBACK WHERE FR_ID_VER ='" + APP_ID + "';", con);
                    cmd.Parameters.AddWithValue("@APP_ID", APP_ID);
                    reader = cmd.ExecuteReader();
                    rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                    object parang = rng;
                    Paragraph oPara4 = doc.Content.Paragraphs.Add(ref parang);
                    oPara4.Range.Text = "Report for Setbacks";
                    oPara4.Range.Font.Name = "Verdana";
                    oPara4.Range.Font.Size = 10;
                    oPara4.Range.Font.Color = WdColor.wdColorAutomatic;
                    oPara4.Range.Font.Underline = WdUnderline.wdUnderlineSingle;

                    rng.InsertParagraphAfter();
                    rng.InsertParagraphAfter();
                    rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;

                    object objDefaultBehaviorWord8 = WdDefaultTableBehavior.wdWord9TableBehavior;
                    object objAutoFitFixed = WdAutoFitBehavior.wdAutoFitFixed;
                    Table tbl = doc.Tables.Add(rng, ErrorCnt + 1, 6, ref objDefaultBehaviorWord8, ref objAutoFitFixed);
                    tbl.Range.Font.Size = 7;
                    tbl.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                    //tbl.ApplyStyleColumnBands = true;
                    tbl.Cell(1, 1).Range.Text = "SET CODE";
                    tbl.Cell(1, 2).Range.Text = "SETBACK WIDTH";
                    tbl.Cell(1, 3).Range.Text = "PERMISSIBLE SETBACK";
                    tbl.Cell(1, 4).Range.Text = "PERMISSIBLE AS PER LOP";
                    tbl.Cell(1, 5).Range.Text = "COMPLY";
                    tbl.Cell(1, 6).Range.Text = "REMARKS";
                    int rowCnt = 1;
                    while (reader.HasRows && reader.Read())
                    {
                        rowCnt++;
                        string setbackcode = reader.GetValue(1).ToString().ToUpper();
                        switch (setbackcode)
                        {
                            case "R":
                                setbackcode = "Rear";
                                break;
                            case "F":
                                setbackcode = "Front";
                                break;
                            case "S1":
                                setbackcode = "Side 1";
                                break;
                            case "S2":
                                setbackcode = "Side 2";
                                break;

                            default:

                                break;
                        }
                        tbl.Cell(rowCnt, 1).Range.Text = setbackcode;
                        tbl.Cell(rowCnt, 2).Range.Text = reader.GetValue(2).ToString();
                        tbl.Cell(rowCnt, 3).Range.Text = reader.GetValue(3).ToString();
                        tbl.Cell(rowCnt, 4).Range.Text = reader.GetValue(4).ToString();
                        tbl.Cell(rowCnt, 5).Range.Text = reader.GetValue(5).ToString();
                        tbl.Cell(rowCnt, 6).Range.Text = reader.GetValue(6).ToString();
                        if (reader.GetValue(5).ToString() == "No")
                        {
                            tbl.Rows[rowCnt].Range.Font.Color = WdColor.wdColorDarkRed;
                        }

                    }
                    
                    rng.InsertParagraphAfter();
                    rng.InsertParagraphAfter();

                }
            }
            catch (Exception ex)
            {
                log.Error("Error while generating IndustrialSetbackReport: " + ex.Message);
            }
            finally
            {
                if (reader1 != null && !reader1.IsClosed) reader1.Close();
                if (reader != null && !reader.IsClosed) reader.Close();
                cmd1.Dispose();

                log.Debug("IndustrialSetbackReport() - Generated IndustrialSetback Report");
            }
        }

        public void CoverageReport(Document doc, string APP_ID, MySqlConnection con)
        {
            log.Debug("CoverageReport() - Generating Coverage Report");
            
            MySqlCommand cmd1 = new MySqlCommand("select count(1) from RE_COVERAGE WHERE FR_ID_VER = '" + APP_ID + "' ;", con);
            cmd1.Parameters.AddWithValue("@APP_ID", APP_ID);
            MySqlDataReader reader1 = null;
            MySqlDataReader reader = null;
            try
            { 
             reader1 = cmd1.ExecuteReader();

            object missing = System.Reflection.Missing.Value;
            object oEndOfDoc = "\\endofdoc";
            Range rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                if (reader1.Read() == true)
                {
                    int ErrorCnt = reader1.GetInt32(0);
                    reader1.Close();
                    MySqlCommand cmd = new MySqlCommand("select * from RE_COVERAGE WHERE FR_ID_VER = '" + APP_ID + "';", con);
                     reader = cmd.ExecuteReader();
                    rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                    object parang = rng;
                    Paragraph oPara4 = doc.Content.Paragraphs.Add(ref parang);
                    oPara4.Range.Text = "Report for COVERAGE";
                    oPara4.Range.Font.Name = "Verdana";
                    oPara4.Range.Font.Size = 10;
                    oPara4.Range.Font.Color = WdColor.wdColorAutomatic;
                    oPara4.Range.Font.Underline = WdUnderline.wdUnderlineSingle;

                    rng.InsertParagraphAfter();
                    rng.InsertParagraphAfter();
                    rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;

                    object objDefaultBehaviorWord8 = WdDefaultTableBehavior.wdWord9TableBehavior;
                    object objAutoFitFixed = WdAutoFitBehavior.wdAutoFitFixed;
                    Table tbl = doc.Tables.Add(rng, ErrorCnt + 1, 6, ref objDefaultBehaviorWord8, ref objAutoFitFixed);
                    tbl.Range.Font.Size = 7;
                    tbl.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                    //tbl.ApplyStyleColumnBands = true;
                    tbl.Cell(1, 1).Range.Text = "FLR CODE";
                    tbl.Cell(1, 2).Range.Text = "FLR NO";
                    tbl.Cell(1, 3).Range.Text = "CVRG AREA";
                    tbl.Cell(1, 4).Range.Text = "PERMISSIBLE CVRG AREA";
                    tbl.Cell(1, 5).Range.Text = "COMPLY";
                    tbl.Cell(1, 6).Range.Text = "REMARKS";
                    int rowCnt = 1;
                    while (reader.HasRows && reader.Read())
                    {
                        rowCnt++;
                        string Cvrgcode = reader.GetValue(1).ToString().ToUpper();
                        switch (Cvrgcode)
                        {
                            case "B":
                                Cvrgcode = "Basement";
                                break;
                            case "S":
                                Cvrgcode = "Stilt";
                                break;
                            case "T":
                                Cvrgcode = "Terrace";
                                break;
                            case "O":
                                Cvrgcode = "Others";
                                break;
                            case "G":
                                Cvrgcode = "Ground";
                                break;
                            default:

                                break;
                        }
                        tbl.Cell(rowCnt, 1).Range.Text = Cvrgcode;
                        tbl.Cell(rowCnt, 2).Range.Text = reader.GetValue(2).ToString();
                        tbl.Cell(rowCnt, 3).Range.Text = reader.GetValue(3).ToString();
                        tbl.Cell(rowCnt, 4).Range.Text = reader.GetValue(4).ToString();
                        tbl.Cell(rowCnt, 5).Range.Text = reader.GetValue(5).ToString();
                        tbl.Cell(rowCnt, 6).Range.Text = reader.GetValue(6).ToString();
                        if (reader.GetValue(5).ToString() == "No")
                        {
                            tbl.Rows[rowCnt].Range.Font.Color = WdColor.wdColorDarkRed;
                        }

                    }
                }
                

                rng.InsertParagraphAfter();
                rng.InsertParagraphAfter();
                //reader.Close();
            }

            catch (Exception ex)
            {
                log.Error("Error while generating CoverageReport: " + ex.Message);
            }
            finally
            {
                if (reader1 != null && !reader1.IsClosed) reader1.Close();
                if (reader != null && !reader.IsClosed) reader.Close();
                cmd1.Dispose();

                log.Debug("CoverageReport() - Generated Coverage Report");
            }
        }

        public void BalconyReport(Document doc, string APP_ID, MySqlConnection con)
        {
            try
            {
                log.Debug("BalconyReport() - Generating Balcony Report");
                MySqlCommand cmd1 = new MySqlCommand("select count(1) from (SELECT DISTINCT BLDG_NO,FLR_NO,DU_NO FROM RE_BALCONY WHERE FR_ID_VER ='" + APP_ID + "')AS subquery;", con);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                if (reader1.HasRows && reader1.Read())
                {

                object missing = System.Reflection.Missing.Value;
                object oEndOfDoc = "\\endofdoc";
                Range rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                rng.InsertParagraphAfter();
                rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                rng.InsertParagraphAfter();
                object parang = rng;
                Paragraph oPara4 = doc.Content.Paragraphs.Add(ref parang);
                oPara4.Range.Text = "Report for Balcony";
                oPara4.Range.Font.Name = "Verdana";
                oPara4.Range.Font.Size = 11;
                oPara4.Range.Font.Color = WdColor.wdColorAutomatic;
                oPara4.Range.Font.Underline = WdUnderline.wdUnderlineSingle;
                rng.InsertParagraphAfter();

                    int ErrorCnt = reader1.GetInt32(0);

                    if (ErrorCnt != 0)
                    {
                        reader1.Close();
                        MySqlCommand cmd2 = new MySqlCommand("select count(1) from RE_BALCONY WHERE FR_ID_VER ='" + APP_ID + "' AND (COMPLY = 'No'or COMPLY = 'NO');", con);
                        int tableCount = Convert.ToInt32(cmd1.ExecuteScalar());
                        MySqlDataReader reader2 = cmd1.ExecuteReader();
                        if (tableCount == 0)
                        {

                            // cmd = new MySqlCommand("select * from RE_BALCONY WHERE FR_ID_VER ='" + APP_ID + "' AND (COMPLY = 'No'or COMPLY = 'NO');", con);
                        rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                        //rng.InsertParagraphAfter();
                        rng.InsertParagraphAfter();
                        rng.Paragraphs.Add(ref missing);
                        //rng.InsertParagraphAfter();
                        rng.Text = "All Balcony dimensions are as per byeLaws.";
                        rng.Font.Name = "Verdana";
                            rng.Font.Size = 10;
                        rng.Font.Color = WdColor.wdColorBlue;
                        rng.Font.Underline = WdUnderline.wdUnderlineNone;
                        rng.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        //rng.ParagraphFormat.LineSpacing = 0;
                        rng.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
                        //rng.InsertParagraphAfter();
                        rng.InsertParagraphAfter();
                    }
                    else
                    {
                            reader2.Close();
                            MySqlCommand cmd = new MySqlCommand("SELECT DISTINCT BLDG_NO,FLR_NO,DU_NO FROM RE_BALCONY WHERE FR_ID_VER ='" + APP_ID + "' AND (COMPLY = 'No'or COMPLY = 'NO');", con);
                            MySqlDataReader reader = cmd.ExecuteReader();

                        //rng.InsertParagraphAfter();
                        //rng.InsertParagraphAfter();

                        while (reader.HasRows && reader.Read())
                        {
                            //rng.InsertParagraphBefore();
                            //rng.InsertParagraphAfter();
                            short BldgNo = reader.GetInt16(0);
                            short FloorNo = reader.GetInt16(1);
                            int DwellingNO = reader.GetInt32(2);

                            rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                            parang = rng;
                            oPara4 = doc.Content.Paragraphs.Add(ref parang);
                            oPara4.Range.Text = "Report for Balcony in Dwelling unit : " + DwellingNO.ToString() + "    from building no : " + BldgNo.ToString() + " and floor no: " + FloorNo.ToString();
                            oPara4.Range.Font.Name = "Verdana";
                            oPara4.Range.Font.Size = 10;
                            oPara4.Range.Font.Color = WdColor.wdColorAutomatic;
                            oPara4.Range.Font.Underline = WdUnderline.wdUnderlineNone;

                            rng.InsertParagraphAfter();
                                //rng.InsertParagraphAfter();
                                reader.Close();
                            MySqlCommand Roomscmd = new MySqlCommand("select count(1) from (select * from RE_BALCONY where FR_ID_VER ='" + APP_ID + "' AND BLDG_NO = '" + BldgNo + "' AND FLR_NO = '" + FloorNo + "' AND DU_NO = '" + DwellingNO + "' AND (COMPLY = 'No'or COMPLY = 'NO'))AS subquery;", con);
                            MySqlDataReader Roomsreader1 = Roomscmd.ExecuteReader();
                            rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                            Roomsreader1.Read();
                            int FC = Roomsreader1.GetInt32(0);
                                Roomsreader1.Close();
                                MySqlCommand Roomscmd1 = new MySqlCommand("select * from RE_BALCONY where FR_ID_VER ='" + APP_ID + "' AND BLDG_NO = '" + BldgNo + "' AND FLR_NO = '" + FloorNo + "' AND DU_NO = '" + DwellingNO + "' AND (COMPLY = 'No'or COMPLY = 'NO');", con);
                                MySqlDataReader Roomsreader = Roomscmd1.ExecuteReader();
                            object objDefaultBehaviorWord8 = WdDefaultTableBehavior.wdWord9TableBehavior;
                            object objAutoFitFixed = WdAutoFitBehavior.wdAutoFitFixed;
                            Table tbl = doc.Tables.Add(rng, FC + 1, 5, ref objDefaultBehaviorWord8, ref objAutoFitFixed);
                            tbl.Range.Font.Size = 7;
                            tbl.ApplyStyleColumnBands = true;
                            tbl.Cell(1, 1).Range.Text = "BALCONY NO";
                            tbl.Cell(1, 2).Range.Text = "BALCONY WIDTH";
                            tbl.Cell(1, 3).Range.Text = "PERMISSIBLE WIDTH";
                            tbl.Cell(1, 4).Range.Text = "COMPLY";
                            tbl.Cell(1, 5).Range.Text = "Remarks";
                            int rowCnt = 1;
                            while (Roomsreader.HasRows && Roomsreader.Read())
                            {
                                    rowCnt++;
                                    tbl.Cell(rowCnt, 1).Range.Text = Roomsreader.GetValue(1).ToString();
                                    tbl.Cell(rowCnt, 2).Range.Text = Roomsreader.GetValue(6).ToString();
                                    tbl.Cell(rowCnt, 3).Range.Text = Roomsreader.GetValue(7).ToString();
                                    tbl.Cell(rowCnt, 4).Range.Text = Roomsreader.GetValue(8).ToString();
                                    tbl.Cell(rowCnt, 5).Range.Text = Roomsreader.GetValue(9).ToString();
                                    tbl.Rows[rowCnt].Range.Font.Color = WdColor.wdColorDarkRed;


                                }
                                //rng.InsertParagraphAfter();
                                Roomsreader.Close();
                        }
                        reader.Close();
                        rng.InsertParagraphAfter();
                        rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                        rng.Paragraphs.Add(ref missing);
                        //rng.InsertParagraphAfter();
                        rng.Text = "Except above Balconies all Balconies are as per byelaws";
                        rng.Font.Name = "Verdana";
                        rng.Font.Size = 10;
                        rng.Font.Color = WdColor.wdColorBlue;
                        rng.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        //rng.ParagraphFormat.LineSpacing = 0;
                        rng.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
                        rng.InsertParagraphAfter();

                        }
                    }
                    else
                    {
                        Paragraph oPara5;
                        rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                        object parang2 = rng;
                        oPara5 = doc.Content.Paragraphs.Add(ref parang2);
                        //oPara5.Range.InsertParagraphBefore();
                        oPara5.Range.Text = "There is no Balconies  found.";
                        oPara5.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                        oPara5.Range.Font.Size = 11;
                        oPara5.Range.Font.Color = WdColor.wdColorAutomatic;
                        oPara5.Range.InsertParagraphAfter();
                    }
                    
                }
                reader1.Close();
                log.Debug("BalconyReport() - Generated Balcony Report");
            }
            catch (System.Exception ex)
            {
                log.Error("BalconyReport()-Error occured in Balcony Report; Error(" + ex.Message + ")");
                System.Windows.Forms.MessageBox.Show("Error : " + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
        }

        public void NotesReport(Document doc, string APP_ID, MySqlConnection con)
        {
            try
            {
                log.Debug("NotesReport() - Generating Notes Report");
                int rowcount = 0;

                MySqlCommand cmd1 = new MySqlCommand( ";select count(1) from (SELECT * FROM RE_NOTE WHERE FR_ID_VER ='" + APP_ID + "');", con);
                MySqlDataReader reader1 = cmd1.ExecuteReader();
                object missing = System.Reflection.Missing.Value;
                object oEndOfDoc = "\\endofdoc";
                Range rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                rng.InsertParagraphAfter();
                rng.InsertParagraphAfter();



                if (reader1.Read() == true)
                {
                 
                    MySqlCommand notescmd = new MySqlCommand( ";SELECT * FROM RE_NOTE WHERE FR_ID_VER ='" + APP_ID + "';", con);
                    MySqlDataReader notesreader = notescmd.ExecuteReader();
                    rowcount = reader1.GetInt32(0);
                    object parang = rng;
                    rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                    parang = rng;
                    rng.InsertParagraphAfter();
                    rng.InsertParagraphAfter();
                    rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                    rng.InsertParagraphAfter();
                    int FC = notesreader.FieldCount;
                    Paragraph oPara4 = doc.Content.Paragraphs.Add(ref parang);
                    oPara4.Range.Text = "Report for Notes";
                    oPara4.Range.Font.Name = "Verdana";
                    oPara4.Range.Font.Size = 11;
                    oPara4.Range.Font.Color = WdColor.wdColorAutomatic;

                    object objDefaultBehaviorWord8 = WdDefaultTableBehavior.wdWord9TableBehavior;
                    object objAutoFitFixed = WdAutoFitBehavior.wdAutoFitFixed;
                    Table tbl = doc.Tables.Add(rng, rowcount + 1, FC, ref objDefaultBehaviorWord8, ref objAutoFitFixed);
                    tbl.Range.Font.Size = 7;
                    tbl.Columns.AutoFit();
                    tbl.ApplyStyleColumnBands = true;
                    tbl.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                    tbl.Cell(1, 1).Range.Text = "Sl. NO";
                    tbl.Cell(1, 2).Range.Text = "NOTE";
                    tbl.Cell(1, 3).Range.Text = "COMPLY";
                    while (notesreader.HasRows && notesreader.Read())
                    {
                        int rowCnt = 1;
                        while (notesreader.HasRows && notesreader.Read())
                        {
                            rowCnt++;

                            tbl.Cell(rowCnt, 1).Range.Text = rowCnt.ToString(); ;
                            tbl.Cell(rowCnt, 2).Range.Text = notesreader.GetValue(1).ToString();
                            tbl.Cell(rowCnt, 3).Range.Text = notesreader.GetValue(2).ToString();
                            tbl.Rows[rowCnt].Range.Font.Color = WdColor.wdColorBlue;

                        }
                        //rng.InsertParagraphAfter();
                        notesreader.Close();
                    }
                    notesreader.Close();
                    rng.InsertParagraphAfter();
                    rng = doc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                    rng.Paragraphs.Add(ref missing);
                    //rng.InsertParagraphAfter();
                    //rng.Text = "Notes are as per byelaws";
                    rng.Font.Name = "Verdana";
                    rng.Font.Size = 10;
                    rng.Font.Color = WdColor.wdColorBlue;
                    rng.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    //rng.ParagraphFormat.LineSpacing = 0;
                    rng.ParagraphFormat.LineSpacingRule = WdLineSpacing.wdLineSpaceExactly;
                    rng.InsertParagraphAfter();


                }
                reader1.Close();
                log.Debug("NotesReport() - Generated Notes Report");
            }
            catch (System.Exception ex)
            {
                log.Error("NotesReport()-Error occured in Notes Report; Error(" + ex.Message + ")");
                System.Windows.Forms.MessageBox.Show("Error : " + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
        }

    }


}
