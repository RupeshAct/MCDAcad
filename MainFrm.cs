using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Collections;
using System.Reflection;
using System.IO;
using Microsoft.Win32;
//using IBM.Data.DB2;
using MySql.Data.MySqlClient;
using log4net;
using System.Configuration;
using Autodesk.AutoCAD.Interop.Common;
using Autodesk.AutoCAD.Interop;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using MCD;

using System.Configuration;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]
namespace MCD
{
    public partial class MainFrm : Form
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(TestPage1).Name);  
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        [System.Runtime.InteropServices.DllImport("user32")]
        static extern int GetWindowThreadProcessId(IntPtr hWnd, out int processId);
        private const int WM_ACTIVATEAPP = 0x001C;
        //private bool appActive = true;
        private List<Thread> Threads = new List<Thread>();
        private DateTime now = DateTime.Now;
        private byte count = 0;
        private MySqlDataReader MainReader;
        public static int appstage;
        public MainFrm()
        {

            InitializeComponent();

        }

       

        public static AcadApplication StartAutoCADSession()
        {
            // Each time create a new instance of AutoCAD


            const string progID = "AutoCAD.Application.24.2";

            AcadApplication acApp = null;
            try
            {
                log.Debug("StartAutoCADSession() - Started");
                Type acType = Type.GetTypeFromProgID(progID);
                acApp = (AcadApplication)Activator.CreateInstance(acType, true);
                log.Debug("StartAutoCADSession() - Ended");
            }
            catch (System.Exception ex)
            {
                //Environment.Exit(0); 
                log.Error("StartAutoCADSession()-Unable to start Autocad session-Error(" + ex.Message + ")");
                //MessageBox.Show("Error Occured : " + ex.Message + "\n" + ex.StackTrace);
            }

            return acApp;
        }

        protected class opencadclass
        {
            string Path;
            public string FR_ID_ver;
            public opencadclass(string pathname, string fr_id_Ver)
            {
                log.Debug("opencadclass():Starts with (" + fr_id_Ver + ") and (" + pathname + ")");
                Path = pathname;
                FR_ID_ver = fr_id_Ver;
                log.Debug("opencadclass():Ends(" + fr_id_Ver + ") and (" + pathname + ")");
            }


            public void Openacad()
            {
                try
                {
                    log.Debug("Openacad() - Started");
                    AcadApplication acapp = StartAutoCADSession();
                    System.Data.DataTable MainDtApp = FunctionsNvar.Executequery( ";select status from  file_watch where FR_ID_VER = '" + FR_ID_ver + "';commit;");
                    appstage = int.Parse(MainDtApp.Rows[0][0].ToString());
                    //-->>> Included getting Appstage of Application ID into log while selecting it on 10th July 2013 By Kiran Bishaj.
                    // AppStageslog.DebugLog("Openacad()- Selected Application ID " + ID_ver + " with status (" + appstage + ")");
                    //<<<-- Included getting Appstage of Application ID into log while selecting it on 10th July 2013 By Kiran Bishaj.
                    // IntPtr hnwdintptr = (IntPtr)acapp.HWND;
                    if (acapp == null)
                    {
                        if (appstage == 23 || appstage == 24)
                        {

                            FunctionsNvar.ExecuteNquery( ";update  file_watch set status = " + appstage + " where FR_ID_VER = '" + FR_ID_ver + "';commit;");
                            //-->>> Included Appstage log on 29-05-2013 By Kiran  
                            AppStageslog.DebugLog("Openacad()- Updated status  is (" + appstage + ") for  file_watch FR_ID " + FR_ID_ver);
                            //<<<-- Included Appstage log on 29-05-2013 By Kiran 
                        }
                        else
                        {
                            //Updating status_temp to null to avoid strucking of request id's at status_temp 4 on 23rd Sept 2013.
                            FunctionsNvar.ExecuteNquery(";update  file_watch set status = 1,status_temp = NULL where FR_ID_VER = '" + FR_ID_ver + "';commit;");
                            //-->>> Included Appstage log on 29-05-2013 By Kiran  
                            AppStageslog.DebugLog("Openacad()- Updated status is 1 its status_temp is NULL for  file_watch FR_ID " + FR_ID_ver);
                            //<<<-- Included Appstage log on 29-05-2013 By Kiran
                        }



                        return;

                    }
                    else
                    {
                        IntPtr hnwdintptr = (IntPtr)acapp.HWND;
                        try
                        {
                            log.Debug("Openacad()-Inserting into EXCEPTIONREMARKS table");
                            FunctionsNvar.ExecuteNquery("INSERT INTO EXCEPTIONREMARKS(EXCPTREMRKS_FR_ID_VER, EXCPTREMRKS_REMARKS)" +
                                "VALUES ('" + FR_ID_ver + "'," + acapp.HWND.ToString() + ");");
                            FunctionsNvar.ExecuteNquery(";update file set UPDATED_TIME = '" + DateTime.Now.ToShortTimeString() + "' where FR_ID_VER = '" + FR_ID_ver + "';commit;");
                            log.Debug("Openacad()-Updated UPDATED_TIME in file table");

                            AcadDocument acd;

                            try
                            {
                                acd = acapp.Documents.Open(Path, false, "");

                            }
                            catch (System.Exception ex)
                            {
                                log.Error("Openacad()-Unable to Open Autocad -Error(" + ex.Message + ")");
                                string fr_id = FR_ID_ver.Substring(0, FR_ID_ver.Length - 2);
                                string ver = FR_ID_ver.Substring(FR_ID_ver.Length - 2);
                                string TxtFilePath = "\\\\192.168.1.14\\Shared\\Validation\\fileprocessed\\" + fr_id + "_" + ver + "_ValidationReport.Txt";
                                using (StreamWriter TxtFile = new StreamWriter(TxtFilePath, true))
                                {
                                    TxtFile.WriteLine("Drawing can not open, Please upload valid dwg");
                                }
                                ValidateReport vr = new ValidateReport();
                                vr.validReport(FR_ID_ver, TxtFilePath);
                                System.IO.File.Delete(TxtFilePath);
                                if ((appstage == 23) || (appstage == 24))
                                {
                                }
                                else
                                {
                                    FunctionsNvar.ExecuteNquery( ";update  file_watch set status = 51,status_temp = NULL  where FR_ID_VER = '" + FR_ID_ver + "';commit;");
                                    //-->>> Included Appstage log on 29-05-2013 By Kiran  
                                    AppStageslog.DebugLog("Openacad()-Updated status is 51 and its status_temp is NULL for  file_watch FR_ID " + FR_ID_ver);
                                    //<<<-- Included Appstage log on 29-05-2013 By Kiran 
                                }

                                MySqlConnection con = new MySqlConnection(FunctionsNvar.Constr);
                                con.Open();
                                MySqlCommand Cmd1 = new MySqlCommand(";select pid from  file_watch where fr_id_ver ='" + FR_ID_ver + "';commit;", con);
                                int pid = Convert.ToInt16(Cmd1.ExecuteScalar());
                                StringBuilder Validation_FileName = new StringBuilder();
                                switch (pid)
                                {
                                    case 1:
                                        Validation_FileName.Append(fr_id + "_" + ver + "_ValidationReport.PDF");
                                        break;
                                    case 2:
                                        Validation_FileName.Append(fr_id + "_" + ver + "_ValidationReport_CC.PDF");
                                        break;
                                    case 3:
                                        Validation_FileName.Append(fr_id + "_" + ver + "_ValidationReport_Revised.PDF");
                                        break;
                                    case 4:
                                        Validation_FileName.Append(fr_id + "_" + ver + "_ValidationReport_Regularized.PDF");
                                        break;
                                    case 5:
                                        Validation_FileName.Append(fr_id + "_" + ver + "_ValidationReport_AA.PDF");
                                        break;
                                    case 6:
                                        Validation_FileName.Append(fr_id + "_" + ver + "_ValidationReport_REVDN.PDF");
                                        break;
                                    case 7:
                                        Validation_FileName.Append(fr_id + "_" + ver + "_ValidationReport_SARAL_Revise.PDF");
                                        break;
                                    case 8:
                                        Validation_FileName.Append(fr_id + "_" + ver + "_ValidationReport_SANCTION_Up_To_500_Sqmt.PDF");
                                        break;
                                    case 9:
                                        Validation_FileName.Append(fr_id + "_" + ver + "_ValidationReport_Revised_SANCTION_Up_To_500_Sqmt.PDF");
                                        break;
                                }
                                con.Close();

                                FunctionsNvar.ExecuteNquery(";update file set processed_filenames = " + Validation_FileName + " where FR_ID_VER = " + fr_id + ver + ";commit;");
                                return;

                            }

                            if (appstage == 24 || appstage == 23)
                            {

                            }
                            else
                            {

                                FunctionsNvar.ExecuteNquery( ";update  file_watch set status = 45 where FR_ID_VER = '" + FR_ID_ver + "';commit;");
                                //-->>> Included Appstage log on 29-05-2013 By Kiran   
                                AppStageslog.DebugLog("Openacad()-Updated status is 45  for  file_watch FR_ID " + FR_ID_ver);
                                //<<<-- Included Appstage log on 29-05-2013 By Kiran 
                            }
                            try
                            {
                                acapp.Visible = true;
                            }
                            catch (System.Exception)
                            {

                            }
                            try
                            {
                                acd.SetVariable("autosnap", 63);
                            }
                            catch (System.Exception)
                            {

                            }



                            Thread.Sleep(10000);
                            try
                            {
                                acd.Close(false, Path);
                            }
                            catch (System.Exception)
                            {

                            }

                            finally
                            {

                            }


                        }
                        catch (System.Exception ex)
                        {
                            log.Error("Openacad()-Unable to Open Autocad -Error(" + ex.Message + ")");
                            FunctionsNvar.ExecuteNquery(";update  file_watch set status = 44,status_temp = NULL where FR_ID_VER = '" + FR_ID_ver + "';commit;");
                            //MessageBox.Show("Error Occured : " + ex.Message + "\n" + ex.StackTrace);
                            //-->>> Included Appstage log on 29-05-2013 By Kiran  
                            AppStageslog.DebugLog("Openacad()-Updated status is 44 and its status_temp is NULL for  file_watch FR_ID " + FR_ID_ver);
                            //<<<-- Included Appstage log on 29-05-2013 By Kiran 
                        }
                        finally
                        {
                            //PlotDwg.PlotCurrentLayout(
                            try
                            {
                                int processid;
                                //IntPtr hnwdintptr = (IntPtr)acapp.HWND;
                                int threadid = GetWindowThreadProcessId(hnwdintptr, out processid);
                                System.Diagnostics.Process Pracad = System.Diagnostics.Process.GetProcessById(processid);

                                Pracad.Kill();
                                //acapp.Quit();
                            }
                            catch (System.Exception)
                            {

                            }

                            if (System.IO.File.Exists(Path) == true)
                            {
                                try
                                {
                                    System.IO.File.Delete(Path);
                                }
                                catch (System.Exception)
                                {

                                }
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {

                    MessageBox.Show("Error Occured : " + ex.Message + "\n" + ex.StackTrace);
                }
            }

        }

        //private void ChkDbTimer_Tick(object sender, EventArgs e)
        //{ string dwgName = String.Empty;
        //}
        private void ChkDbTimer_Tick(object sender, EventArgs e)
        {
            string dwgName = String.Empty;
            string id_no = String.Empty;
            string fr_id = String.Empty;
            string ver = String.Empty;
            string TxtFilePath = String.Empty;
            int Appstage, pid;
            TimeSpan ts = DateTime.Now.Subtract(now);
            StringBuilder Validation_FileName = new StringBuilder();
            System.Data.DataTable FileDtApp = FunctionsNvar.Executequery( ";select * from  file_watch where (status = 2 or status = 3 or status = 1) and status_temp IS NULL;commit;");
            //System.Data.DataTable FileDtApp = FunctionsNvar.Executequery( ";select * from  file_watch a,SYSTEM_NUMBER s where a.ID_VER=s.ID_VER and (a.status = 2 or a.status = 3 or a.status = 1) and status_temp IS NULL and s.SYS_N0=7;commit;");   
            for (int i = 0; i < FileDtApp.Rows.Count; i++)
            {
                string FR_ID_VER = FileDtApp.Rows[i]["fr_id_ver"].ToString();
                Int32 appstage = (Int32)FileDtApp.Rows[i]["status"];
                //-->>> Included getting Appstage of  file_watch ID into log while selecting it on 10th July 2013 By Kiran Bishaj.
                // AppStageslog.DebugLog("ChkDbTimer_Tick()- Selected  file_watch ID '" + FR_ID_VER + "' with status (" + appstage + ")");
                //<<<-- Included getting Appstage of  file_watch ID into log while selecting it on 10th July 2013 By Kiran Bishaj.

                System.Data.DataTable DtDwg = FunctionsNvar.Executequery("select * from file where FR_ID_VER = '" + FR_ID_VER + "' order by  DWG_VER DESC;");
                if (DtDwg.Rows.Count == 0)
                {
                    //FunctionsNvar.ExecuteNquery( ";update  file_watch set status_temp = 7 where ID_VER = '" + ID + "';commit;");
                    continue;
                }
                dwgName = DtDwg.Rows[0]["fr_name"].ToString();
                id_no = DtDwg.Rows[0]["FR_ID"].ToString();
                string[] dwgId = dwgName.Split('_');
                if ((String.IsNullOrEmpty(dwgName)) || (dwgId[0].ToString() != id_no.ToString()))
                {
                    fr_id = FR_ID_VER.Substring(0, FR_ID_VER.Length - 2);
                    ver = FR_ID_VER.Substring(FR_ID_VER.Length - 2);
                    TxtFilePath = "\\\\192.168.1.14\\Shared\\Validation\\fileprocessed\\" + fr_id + "_" + ver + "_ValidationReport.Txt";
                    using (StreamWriter TxtFile = new StreamWriter(TxtFilePath, true))
                    {
                        TxtFile.WriteLine(MCD.ConstantStrings.STR_DWGNAME_ISSUE_TXT);
                    }
                    ValidateReport vr = new ValidateReport();
                    vr.validReport(FR_ID_VER, TxtFilePath);
                    System.IO.File.Delete(TxtFilePath);
                    Appstage = AppnDbquery(FR_ID_VER);
                    if ((MCD.ConstantStrings.INT_FILE_PROCESS_START1 != Appstage) || (MCD.ConstantStrings.INT_FILE_PROCESS_START2 != Appstage))
                    {
                        FunctionsNvar.ExecuteNquery(";update  file_watch set status = 51,status_temp = NULL  where FR_ID_VER = '" + FR_ID_VER + "';commit;");
                        AppStageslog.DebugLog("ChkDbTimer_Tick()-Updated status is 51 and its status_temp is NULL for  file_watch FR_ID " + FR_ID_VER);
                    }
                    pid = AppnDbquery(FR_ID_VER);
                    switch (pid)
                    {
                        case 1:
                            Validation_FileName.Append(fr_id + "_" + ver + "_ValidationReport.PDF");
                            break;
                        case 2:
                            Validation_FileName.Append(fr_id + "_" + ver + "_ValidationReport_CC.PDF");
                            break;
                        case 3:
                            Validation_FileName.Append(fr_id + "_" + ver + "_ValidationReport_Revised.PDF");
                            break;
                        case 4:
                            Validation_FileName.Append(fr_id + "_" + ver + "_ValidationReport_Regularized.PDF");
                            break;
                        case 5:
                            Validation_FileName.Append(fr_id + "_" + ver + "_ValidationReport_AA.PDF");
                            break;
                        case 6:
                            Validation_FileName.Append(fr_id + "_" + ver + "_ValidationReport_REVDN.PDF");
                            break;
                        case 7:
                            Validation_FileName.Append(fr_id + "_" + ver + "_ValidationReport_SARAL_Revise.PDF");
                            break;
                        case 8:
                            Validation_FileName.Append(fr_id + "_" + ver + "_ValidationReport_SANCTION_Up_To_500_Sqmt.PDF");
                            break;
                        case 9:
                            Validation_FileName.Append(fr_id + "_" + ver + "_ValidationReport_Revised_SANCTION_Up_To_500_Sqmt.PDF");
                            break;
                    }
                    FunctionsNvar.ExecuteNquery(";update file set processed_filenames = '" + Validation_FileName + "' where FR_ID_VER = " + fr_id + ver + ";commit;");
                    break;
                }

                //System.IO.FileInfo chkfile = new System.IO.FileInfo(@"D:\File\" + dwgName);
                System.IO.FileInfo chkfile = new System.IO.FileInfo("\\\\192.168.1.14\\Shared\\Validation\\file\\" + dwgName);
                if (chkfile.Extension.ToUpper() != ".DWG")
                {
                    //chkfile = new System.IO.FileInfo(@"D:\From-ERP\" + dwgName + ".dwg");
                    chkfile = new System.IO.FileInfo("\\\\192.168.1.14\\Shared\\Validation\\file\\" + dwgName + ".dwg");
                }
                if (chkfile.Exists == true)
                {
                    switch (appstage)
                    {
                        case 1:
                            {
                                FunctionsNvar.ExecuteNquery(";update  file_watch set status = 21 where FR_ID_VER = '" + FR_ID_VER + "';commit;");
                                AppStageslog.DebugLog("ChkDbTimer_Tick()-Updated status from 1 to 21 for  file_watch FR_ID " + FR_ID_VER);
                                break;
                            }
                        case 2:
                            {
                                FunctionsNvar.ExecuteNquery(";update  file_watch set status = 23 where FR_ID_VER = '" + FR_ID_VER + "';commit;");
                                AppStageslog.DebugLog("ChkDbTimer_Tick()-Updated status from 2 to 23 for  file_watch FR_ID " + FR_ID_VER);
                                break;
                            }
                        case 3:
                            {
                                FunctionsNvar.ExecuteNquery(";update  file_watch set status = 24 where FR_ID_VER = '" + FR_ID_VER + "';commit;");
                                AppStageslog.DebugLog("ChkDbTimer_Tick()-Updated status from 3 to 24 for  file_watch FR_ID " + FR_ID_VER);
                                break;
                            }

                    }

                }
            }
            System.Data.DataTable ProcessChkDT = FunctionsNvar.Executequery( ";select * from  file_watch where status = 47 and status_temp = 4;commit;");
            //System.Data.DataTable ProcessChkDT = FunctionsNvar.Executequery( ";select * from  file_watch a,System_Number s where a.ID_VER=s.ID_VER and a.status = 47 and a.status_temp = 4 and s.SYS_N0=7;commit;");          --------nov20
            if (ProcessChkDT.Rows.Count != 0)
            {
                string FR_ID_VER = ProcessChkDT.Rows[0]["fr_id_ver"].ToString();
                System.Data.DataTable DtDwg = FunctionsNvar.Executequery(";select * from file where FR_ID_VER = '" + FR_ID_VER + "';");
                string timestr = DtDwg.Rows[0]["UPDATED_TIME"].ToString();
                System.Data.DataTable ExceptionRemark = FunctionsNvar.Executequery( ";select * from EXCEPTIONREMARKS where EXCPTREMRKS FR_ID_VER = '" + FR_ID_VER + "';");
                if (ExceptionRemark.Rows.Count != 0)
                {
                    string hwnd = ExceptionRemark.Rows[0]["EXCPTREMRKS_REMARKS"].ToString();
                    DateTime dt;
                    DateTime.TryParse(timestr, out dt);
                    TimeSpan t1 = DateTime.Now.Subtract(dt);
                    if (t1.Minutes >= 5)
                    {
                        for (int ThreadNo = 0; ThreadNo < Threads.Count; ThreadNo++)
                        {
                            Thread TmpTh = Threads[ThreadNo];
                            if (TmpTh.Name == FR_ID_VER)
                            {
                                TmpTh.Abort();
                                int processid;
                                IntPtr hnwdintptr = (IntPtr)Convert.ToInt32(hwnd);
                                int threadid = GetWindowThreadProcessId(hnwdintptr, out processid);
                                System.Diagnostics.Process Pracad = System.Diagnostics.Process.GetProcessById(processid);
                                Pracad.Kill();
                                TmpTh.Suspend();
                                log.Debug("Thread Suspended");
                                //TmpTh.Join();
                                Threads.Remove(TmpTh);
                                count--;
                                fr_id = FR_ID_VER.Substring(0, FR_ID_VER.Length - 2);
                                ver = FR_ID_VER.Substring(FR_ID_VER.Length - 2);
                                TxtFilePath = "\\\\192.168.1.14\\Shared\\Validation\\fileprocessed\\" + fr_id + "_" + ver + "_ValidationReport.Txt";
                                using (StreamWriter TxtFile = new StreamWriter(TxtFilePath, true))
                                {
                                    TxtFile.WriteLine("Drawing can not open, Please upload valid dwg");
                                }
                                ValidateReport vr = new ValidateReport();
                                vr.validReport(FR_ID_VER, TxtFilePath);
                                System.IO.File.Delete(TxtFilePath);
                                MySqlConnection con = new MySqlConnection(FunctionsNvar.Constr);
                                con.Open();
                                MySqlCommand AppstageCommand = new MySqlCommand( ";select status from  file_watch where fr_id_ver ='" + FR_ID_VER + "';commit;", con);
                                Appstage = Convert.ToInt16(AppstageCommand.ExecuteScalar());
                                if ((Appstage == 23) || (Appstage == 24))
                                {
                                }
                                else
                                {
                                    FunctionsNvar.ExecuteNquery(";update  file_watch set status = 51,status_temp = NULL  where FR_ID_VER = '" + FR_ID_VER + "';commit;");
                                    //-->>> Included Appstage log on 29-05-2013 By Kiran  
                                    AppStageslog.DebugLog("ChkDbTimer_Tick()-Updated status is 51 and its status_temp is NULL for  file_watch FR_ID " + FR_ID_VER);
                                    //<<<-- Included Appstage log on 29-05-2013 By Kiran 
                                }

                                MySqlCommand Cmd1 = new MySqlCommand( ";select pid from  file_watch where fr_id_ver ='" + FR_ID_VER + "';commit;", con);
                                pid = Convert.ToInt16(Cmd1.ExecuteScalar());
                                // StringBuilder Validation_FileName = new StringBuilder();
                                switch (pid)
                                {
                                    case 1:
                                        Validation_FileName.Append(fr_id + "_" + ver + "_ValidationReport.PDF");
                                        break;
                                    case 2:
                                        Validation_FileName.Append(fr_id + "_" + ver + "_ValidationReport_CC.PDF");
                                        break;
                                    case 3:
                                        Validation_FileName.Append(fr_id + "_" + ver + "_ValidationReport_Revised.PDF");
                                        break;
                                    case 4:
                                        Validation_FileName.Append(fr_id + "_" + ver + "_ValidationReport_Regularized.PDF");
                                        break;
                                    case 5:
                                        Validation_FileName.Append(fr_id + "_" + ver + "_ValidationReport_AA.PDF");
                                        break;
                                    case 6:
                                        Validation_FileName.Append(fr_id + "_" + ver + "_ValidationReport_REVDN.PDF");
                                        break;
                                    case 7:
                                        Validation_FileName.Append(fr_id + "_" + ver + "_ValidationReport_SARAL_Revise.PDF");
                                        break;
                                    case 8:
                                        Validation_FileName.Append(fr_id + "_" + ver + "_ValidationReport_SANCTION_Up_To_500_Sqmt.PDF");
                                        break;
                                    case 9:
                                        Validation_FileName.Append(fr_id + "_" + ver + "_ValidationReport_Revised_SANCTION_Up_To_500_Sqmt.PDF");
                                        break;
                                }
                                con.Close();

                                FunctionsNvar.ExecuteNquery(";update file set processed_filenames = '" + Validation_FileName + "' where FR_ID_VER = " + fr_id + ver + ";commit;");
                                break;
                            }
                        }

                    }
                }
            }
            if (count < 2)
            {
                try
                {
                    log.Debug("ChkDbTimer_Tick()- Started");
                   
                    System.Data.DataTable MainDtApp = FunctionsNvar.Executequery( ";select * from  file_watch where (status = 24 or status = 23 or status = 21) and status_temp IS NULL;commit;");
                    //System.Data.DataTable MainDtApp = FunctionsNvar.Executequery( ";select * from  file_watch a, System_Number s where a.ID_VER=s.ID_VER and (a.status = 24 or a.status = 23 or a.status = 21) and (a.status_temp IS NULL) and s.SYS_N0=7;commit;");    

                    if (MainDtApp.Rows.Count != 0)
                    {
                        count++;
                        string FR_ID_VER = MainDtApp.Rows[0]["fr_id_ver"].ToString();
                        //     FunctionsNvar.ExecuteNquery( ";update  file_watch set status_temp = 4 where ID_VER = " + ID_VER + ";commit;");
                        System.Data.DataTable DtDwg = FunctionsNvar.Executequery(";select * from file where FR_ID_VER = '" + FR_ID_VER + "' and dwg_ver = (select  max(dwg_ver) from file where FR_ID_VER = '" + FR_ID_VER + "');");
                        if (DtDwg.Rows.Count != 0)
                        {
                            if (string.IsNullOrEmpty(DtDwg.Rows[0][6].ToString()))
                            {
                                FunctionsNvar.ExecuteNquery(";update file set C_PLOT = '0' where FR_ID_VER = '" + FR_ID_VER + "';commit;");
                            }
                            int app_stage = int.Parse(MainDtApp.Rows[0]["status"].ToString());
                            if (app_stage == 23 || app_stage == 24)
                            {
                                //nothing now   
                            }
                            else
                            {
                                FunctionsNvar.ExecuteNquery(";update  file_watch set status = 47 where FR_ID_VER = '" + FR_ID_VER + "';commit;");
                                //-->>> Included Appstage log on 29-05-2013 By Kiran  
                                AppStageslog.DebugLog("ChkDbTimer_Tick()-Updated status is 47  for  file_watch FR_ID " + FR_ID_VER);
                                //<<<-- Included Appstage log on 29-05-2013 By Kiran 
                            }

                            string dwgname = DtDwg.Rows[0][10].ToString();
                            if (dwgname != string.Empty)
                            {
                                string dwgPath = "\\\\192.168.1.14\\Shared\\Validation\\file\\";
                                //int Ver = int.Parse(DtDwg.Rows[0][2].ToString());
                                int Ver = 1;
                                System.IO.FileInfo newfi = new System.IO.FileInfo(dwgPath + dwgname);
                                if (newfi.Extension.ToUpper() != ".DWG")
                                {
                                    newfi = new System.IO.FileInfo(dwgPath + dwgname + ".dwg");
                                }
                                StringBuilder NewDwgPathStrBlder = new StringBuilder(newfi.FullName);
                                NewDwgPathStrBlder.Remove(NewDwgPathStrBlder.Length - 4, 4);
                                FilePathLabel.Text = "Processing drawing test --- > " + newfi.Name;
                                this.Width = FilePathLabel.Width + 34;
                                GrpBxLabl.Width = this.Width - 20;
                                this.Refresh();
                                NewDwgPathStrBlder.Append("_" + FR_ID_VER.ToString() + ".dwg");
                                string OldDwg = newfi.FullName;
                                string NewDwg = NewDwgPathStrBlder.ToString();
                                if (System.IO.File.Exists(OldDwg) == false)
                                {
                                    switch (app_stage)
                                    {
                                        case 21:
                                            {
                                                FunctionsNvar.ExecuteNquery(";update  file_watch set status = 42,status_temp = NULL  where FR_ID_VER = '" + FR_ID_VER + "';commit;");
                                                //-->>> Included Appstage log on 29-05-2013 By Kiran  
                                                AppStageslog.DebugLog("ChkDbTimer_Tick()-Updated status is 42 and its status_temp is NULL for  file_watch FR_ID " + FR_ID_VER);
                                                //<<<-- Included Appstage log on 29-05-2013 By Kiran 
                                                log.Error("ChkDbTimer_Tick()- Not obtaining the Drawing from File folder for status 1");
                                                break;
                                            }
                                        case 23:
                                            {
                                                FunctionsNvar.ExecuteNquery(";update  file_watch set status = 40,status_temp = NULL  where FR_ID_VER = '" + FR_ID_VER + "';commit;");
                                                //-->>> Included Appstage log on 29-05-2013 By Kiran  
                                                AppStageslog.DebugLog("ChkDbTimer_Tick()-Updated status is 40 and its status_temp is NULL for  file_watch FR_ID " + FR_ID_VER);
                                                //<<<-- Included Appstage log on 29-05-2013 By Kiran 
                                                log.Error("ChkDbTimer_Tick()- Not obtaining the Drawing from File folder for status 2");
                                                break;
                                            }
                                        case 24:
                                            {
                                                FunctionsNvar.ExecuteNquery(";update  file_watch set status = 41,status_temp = NULL  where FR_ID_VER = '" + FR_ID_VER + "';commit;");
                                                //-->>> Included Appstage log on 29-05-2013 By Kiran  
                                                AppStageslog.DebugLog("ChkDbTimer_Tick()-Updated status is 41 and its status_temp is NULL for  file_watch FR_ID " + FR_ID_VER);
                                                //<<<-- Included Appstage log on 29-05-2013 By Kiran 
                                                log.Error("ChkDbTimer_Tick()- Not obtaining the Drawing from FROM-ERP folder for status 3");
                                                break;
                                            }

                                    }
                                    count--;
                                    return;

                                    
                                }
                                try
                                {
                                    log.Debug("ChkDbTimer_Tick()- Coping old file to new file in FROM-ERP folder");
                                    System.IO.File.Copy(OldDwg, NewDwg, true);
                                }
                                catch (System.Exception ex)
                                {
                                    log.Error("ChkDbTimer_Tick()-Coping old file to new file in File folder -Error(" + ex.Message + ")");
                                    FunctionsNvar.ExecuteNquery(";update  file_watch set status = 1,status_temp = NULL where FR_ID_VER = '" + FR_ID_VER + "';commit;");
                                    //-->>> Included Appstage log on 29-05-2013 By Kiran  
                                    AppStageslog.DebugLog("ChkDbTimer_Tick()-Updated status is 1 and its status_temp is NULL for  file_watch FR_ID " + FR_ID_VER);
                                    //<<<-- Included Appstage log on 29-05-2013 By Kiran 
                                    count--;
                                    return;
                                }

                                FunctionsNvar.FilePath = NewDwg;
                                opencadclass opcad = new opencadclass(NewDwg, FR_ID_VER);
                                Thread th = new Thread(new ThreadStart(opcad.Openacad));
                                th.Name = FR_ID_VER;
                                th.Start();
                                Threads.Add(th);
                                FunctionsNvar.ExecuteNquery(";update  file_watch set status_temp = 4 where FR_ID_VER = '" + FR_ID_VER + "';commit;");
                                //-->>> Included Appstage log on 29-05-2013 By Kiran  
                                AppStageslog.DebugLog("ChkDbTimer_Tick()-Updated  status_temp is 4 for  file_watch FR_ID " + FR_ID_VER);
                                //<<<-- Included Appstage log on 29-05-2013 By Kiran 
                            }
                            else
                            {
                                count--;
                            }
                        }
                        else
                        {
                            count--;
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    log.Error("ChkDbTimer_Tick()-Unable to Open Autocad -Error(" + ex.Message + ")");
                    MessageBox.Show("Error : " + ex.Message + "\n" + ex.Source);
                }
            }
            for (int ThreadNo = 0; ThreadNo < Threads.Count; ThreadNo++)
            {
                Thread TmpTh = Threads[ThreadNo];
                if (TmpTh.ThreadState == ThreadState.Stopped || TmpTh.ThreadState == ThreadState.Aborted)
                {
                    Threads.RemoveAt(ThreadNo);
                    ThreadNo--;
                    count--;
                }
            }
        }

        private void ChkDbDocTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                log.Debug("ChkDbDocTimer_Tick()- Started");
                
                System.Data.DataTable MainDtApp = FunctionsNvar.Executequery( ";select * from  file_watch where status = 1;commit;"); //Selecting records with status 68 changed by Kiran  on 21st Aug 2013.
                if (MainDtApp.Rows.Count != 0)
                {

                    string ID = MainDtApp.Rows[0]["fr_id_ver"].ToString();
                    string appid = ID.Substring(0, ID.Length - 2);
                    string Ver = ID.Substring(ID.Length - 2);
                    System.Data.DataTable DtDwg = FunctionsNvar.Executequery(";select * from file where FR_ID_VER = '" + ID + "'; commit;");
                    if (DtDwg.Rows.Count != 0)
                    {
                        FunctionsNvar.ExecuteNquery(";update  file_watch set status = 48,status_temp = NULL where FR_ID_VER = '" + ID + "';commit;");
                        //-->>> Included Appstage log on 29-05-2013 By Kiran  
                        AppStageslog.DebugLog("ChkDbDocTimer_Tick()-Updated status is 48 and its status_temp is NULL for  file_watch FR_ID " + ID);
                        //<<<-- Included Appstage log on 29-05-2013 By Kiran 
                        string dwgname = DtDwg.Rows[0][10].ToString();
                        //string dwgPath = @"D:\File";
                        string dwgPath = "\\\\192.168.1.14\\Shared\\Validation\\file\\";

                        System.Data.DataTable DtBuildType = FunctionsNvar.Executequery(";select  b_category from  file_watch where FR_ID_VER = '" + ID + "'; commit;");
                        int buildTypeId = int.Parse(DtBuildType.Rows[0][0].ToString());
                        FilePathLabel.Text = "Processing Report for --- > " + dwgname;
                        this.Width = FilePathLabel.Width + 34;
                        GrpBxLabl.Width = this.Width - 20;
                        this.Refresh();
                       
                        ReportDoc rdoc = new ReportDoc();
                        bool approved = rdoc.report(dwgPath, ID.ToString(), buildTypeId);

                        //cmd2 = new MySqlCommand("set schema " + FunctionsNvar.schema  + ";update  file_watch set status = 5 where ID = '" + ID + "';commit;", con);
                        //cmd2.ExecuteNonQuery();
                        if (approved == true)
                        {
                            FunctionsNvar.ExecuteNquery(";update  file_watch set status = 52,status_temp = NULL where FR_ID_VER = '" + ID + "';commit;");
                            //-->>> Included Appstage log on 29-05-2013 By Kiran  
                            AppStageslog.DebugLog("ChkDbDocTimer_Tick()-Updated status is 52 and its status_temp is NULL for  file_watch FR_ID " + ID);

                            //<<<-- Included Appstage log on 29-05-2013 By Kiran 
                            MySqlConnection con = new MySqlConnection(FunctionsNvar.Constr);
                            con.Open();
                            MySqlCommand Cmd1 = new MySqlCommand( ";select pid from  file_watch where fr_id_ver ='" + ID + "';commit;", con);
                            int pid = Convert.ToInt16(Cmd1.ExecuteScalar());
                            StringBuilder ByeLaw_FileName = new StringBuilder();
                            switch (pid)
                            {
                                case 1:
                                    ByeLaw_FileName.Append(appid + "_" + Ver + "_ByeLawReport.PDF");
                                    break;
                                case 2:
                                    ByeLaw_FileName.Append(appid + "_" + Ver + "_ByeLawReport_CC.PDF");
                                    break;
                                case 3:
                                    ByeLaw_FileName.Append(appid + "_" + Ver + "_ByeLawReport_Revised.PDF");
                                    break;
                                case 4:
                                    ByeLaw_FileName.Append(appid + "_" + Ver + "_ByeLawReport_Regularized.PDF");
                                    break;
                                case 5:
                                    ByeLaw_FileName.Append(appid + "_" + Ver + "_ByeLawReport_AA.PDF");
                                    break;
                                case 6:
                                    ByeLaw_FileName.Append(appid + "_" + Ver + "_ByeLawReport_REVDN.PDF");
                                    break;
                                case 7:
                                    ByeLaw_FileName.Append(appid + "_" + Ver + "_ByeLawReport_SARAL_Revise.PDF");
                                    break;
                                case 8:
                                    ByeLaw_FileName.Append(appid + "_" + Ver + "_ByeLawReport_SANCTION_Up_To_500_Sqmt.PDF");
                                    break;
                                case 9:
                                    ByeLaw_FileName.Append(appid + "_" + Ver + "_ByeLawReport_Revised_SANCTION_Up_To_500_Sqmt.PDF");
                                    break;
                            }
                            con.Close();

                            FunctionsNvar.ExecuteNquery(";update file set bylaw_filename = '" + ByeLaw_FileName + "' where FR_ID_VER = '" + ID + "';commit;");

                            log.Debug("In-order Bye-Law report generated successfully for drwaing:- (" + dwgname + ") with fr_id : '" + ID + "' ");
                        }
                        else
                        {
                            FunctionsNvar.ExecuteNquery( ";update  file_watch set status = 53,status_temp = NULL where FR_ID_VER = '" + ID + "';commit;");
                            //-->>> Included Appstage log on 29-05-2013 By Kiran  
                            AppStageslog.DebugLog("ChkDbDocTimer_Tick()-Updated status is 53 and its status_temp is NULL for  file_watch FR_ID " + ID);
                            //<<<-- Included Appstage log on 29-05-2013 By Kiran 

                            MySqlConnection con = new MySqlConnection(FunctionsNvar.Constr);
                            con.Open();
                            MySqlCommand Cmd1 = new MySqlCommand( ";select pid from  file_watch where fr_id_ver ='" + ID + "';commit;", con);
                            int pid = Convert.ToInt16(Cmd1.ExecuteScalar());
                            StringBuilder ByeLaw_FileName = new StringBuilder();
                            switch (pid)
                            {
                                case 1:
                                    ByeLaw_FileName.Append(appid + "_" + Ver + "_ByeLawReport.PDF," + appid + "_" + Ver + "_Error.dwg");
                                    break;
                                case 2:
                                    ByeLaw_FileName.Append(appid + "_" + Ver + "_ByeLawReport_CC.PDF," + appid + "_" + Ver + "_Error_CC.dwg");
                                    break;
                                case 3:
                                    ByeLaw_FileName.Append(appid + "_" + Ver + "_ByeLawReport_Revised.PDF," + appid + "_" + Ver + "_Error_Revised.dwg");
                                    break;
                                case 4:
                                    ByeLaw_FileName.Append(appid + "_" + Ver + "_ByeLawReport_Regularized.PDF," + appid + "_" + Ver + "_Error_Regularized.dwg");
                                    break;
                                case 5:
                                    ByeLaw_FileName.Append(appid + "_" + Ver + "_ByeLawReport_AA.PDF," + appid + "_" + Ver + "_Error_AA.dwg");
                                    break;
                                case 6:
                                    ByeLaw_FileName.Append(appid + "_" + Ver + "_ByeLawReport_REVDN.PDF," + appid + "_" + Ver + "_Error_REVDN.dwg");
                                    break;
                                case 7:
                                    ByeLaw_FileName.Append(appid + "_" + Ver + "_ByeLawReport_SARAL_Revise.PDF," + appid + "_" + Ver + "_Error_SARAL_Revise.dwg");
                                    break;
                                case 8:
                                    ByeLaw_FileName.Append(appid + "_" + Ver + "_ByeLawReport_SANCTION_Up_To_500_Sqmt.PDF," + appid + "_" + Ver + "_Error_SANCTION_Up_To_500_Sqmt.dwg");
                                    break;
                                case 9:
                                    ByeLaw_FileName.Append(appid + "_" + Ver + "_ByeLawReport_Revised_SANCTION_Up_To_500_Sqmt.PDF," + appid + "_" + Ver + "_Error_Revised_SANCTION_Up_To_500_Sqmt.dwg");
                                    break;
                            }
                            con.Close();

                            FunctionsNvar.ExecuteNquery(";update file set bylaw_filename = '" + ByeLaw_FileName + "' where FR_ID_VER = '" + ID + "';commit;");



                            log.Debug("Not In-order Bye-Law report generated successfully for drwaing:- (" + dwgname + ") with fr_id : '" + ID + "' ");
                        }
                        FilePathLabel.Text = "Process complete for Report --- > " + dwgname;
                        log.Debug("Bye-Law report generated successfully for drwaing:- (" + dwgname + ") with fr_id : '" + ID + "' ");
                        this.Width = FilePathLabel.Width + 34;
                        GrpBxLabl.Width = this.Width - 20;
                    }

                }
                //con.Close();
            }
            catch (System.Exception ex)
            {
                log.Error("ChkDbDocTimer_Tick()-Unable to Open Autocad -Error(" + ex.Message + ")");

                MessageBox.Show("Error : " + ex.Message + "\n" + ex.StackTrace + "\n" + ex.Source);
            }
        }

        private void StopBttn_Click(object sender, EventArgs e)
        {
            log.Debug("Stop Button clicked");
            DialogResult dr = MessageBox.Show("Do you want to stop execution?", "Mcd Building Plan", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (dr.Equals(DialogResult.Yes) == true)
            {
                count = 100;
                for (int ThNo = 0; ThNo < Threads.Count; ThNo++)
                {
                    Thread Th = Threads[ThNo];
                    Th.Suspend();
                    log.Debug("Thread Suspended");
                }
            }

        }

        private void GrpBxLabl_Enter(object sender, EventArgs e)
        {

        }
        public int AppnDbquery(string FR_ID_VER)
        {
            int intIdAppStage;
            MySqlConnection con = new MySqlConnection(FunctionsNvar.Constr);
            con.Open();
            MySqlCommand AppstageCommand = new MySqlCommand( ";select status from  file_watch where fr_id_ver ='" + FR_ID_VER + "';commit;", con);
            intIdAppStage = Convert.ToInt16(AppstageCommand.ExecuteScalar());
            con.Close();
            return intIdAppStage;
        }
       
        private void startupRecovery()

        {
            
            log.Debug("startupRecovery()- Started");

            FilePathLabel.Text = "Recovery started test.";
            this.Refresh();
            FunctionsNvar.ExecuteNquery( ";update  file_watch set status = 1,status_temp=null " +
                                                "where status = 42 or status = 43 or status = 44 or status = 21;");
            //-->>> Included Appstage log on 29-05-2013 By Kiran  
            AppStageslog.DebugLog("startupRecovery()- Updated status to 1 and status_temp to NULL ,If status is 42 or 43 or 44 or 21");
            //<<<-- Included Appstage log on 29-05-2013 By Kiran 
            System.Data.DataTable FileDtApp = FunctionsNvar.Executequery( ";select * from  file_watch " +
                                                "where status = 22 or status = 23 or status = 24  or  status = 40 or  status = 41 or  status = 45 or " +
                                                "status = 47 or status = 48 or status = 66 or  status = 67;");
            log.Debug("startupRecovery()- Selected the records having status=22 or 45 or 47 or 66");
            //FunctionsNvar.ExecuteNquery( ";delete from  EXCEPTIONREMARKS;commit;");
            log.Debug("startupRecovery()- Deleted data from Exceptionremarks Table");
            for (int i = 0; i < FileDtApp.Rows.Count; i++)
            {
                string FR_ID_ver = FileDtApp.Rows[i]["fr_id_ver"].ToString();

    //            string[,] tablecolumn = new string[,]
    //{

    //        {"DA_BASEMENT","BASE_ID_VER"},
    //        {"DA_BATH_WATERCLOSET_ROOM","BATH_ID_VER"},
    //        {"DA_CORRIDORS","ID_VER"},
    //        {"DA_FIREESCAPE_STAIRCASE","ID_VER"},
    //        {"DA_GSQ_GARAGE","GARAGE_ID_VER"},
    //        {"DA_LIFTLOBBY","ID_VER"},
    //        {"DA_LIFTPIT","LP_ID_VER"},
    //        {"DA_LOFT","ID_VER"},
    //        {"DA_MEZZANINE","MEZ_ID_VER"},
    //      {"DA_NOTIFIED_COMMERCIAL_AREA", "NOTIFIED_COMMAREA_ID_VER"},
    //      {"DA_NOTIFIED_RAMPS", "RAMP_ID_VER"},
    //      {"DA_NOTIFIED_STAIRCASE", "NS_ID_VER"},
    //        {"DA_PARKING","ID_VER"},
    //        {"DA_PASSAGEWAYS_WT","ID_VER"},
    //        {"DA_PERGOLA","ID_VER"},	  
		  ////{"DA_PERMITFEE_AREA", "FEES_FLR_ID_VER"},--Down
    //        {"DA_RES_BALCONY","BALCONY_ID_VER"},
    //        {"DA_RES_BNDRY_WALL","RBW_ID_VER"},
    //        {"DA_RES_BUILDING","RESPLTH_ID_VER"},
    //        {"DA_RES_CANOPY","CANOPY_ID_VER"},
    //      {"DA_RES_COMMERCIAL_FEATURES", "CF_ID_VER"},
    //      {"DA_RES_COMMERCIAL_SUBFEATURES", "CSF_ID_VER"},
    //      //{"DA_RES_COV_FEE", "ID_VER"},	 --Down
    //        {"DA_RES_CUPBOARD_SHELVES","ID_VER"},
    //        {"DA_RES_DOOR_WINDOW","RESDRW_ID_VER"},
    //        {"DA_PERMITFEE_AREA","FEES_FLR_ID_VER"},
		  ////{"DA_RES_DWELLING", "RESDU_ID_VER"}, --Down
    //      //{"DA_RES_FLOOR", "RESFLR_ID_VER"},	  --Down
    //      //{"DA_RES_FLOOR_HT", "RESFLRHT_ID_VER"}, --Down
    //        {"DA_RES_GARAGE","RG_ID_VER"},
    //        {"DA_RES_HABITABLE_ROOM","RESHABR_ID_VER"},
    //        {"DA_RES_HAND_RAILS","RHR_ID_VER"},
    //        {"DA_RES_HEADROOM_STAIRCASE","RHS_ID_VER"},
    //        {"DA_RES_INTERIOR_COURTYARD","RIC_ID_VER"},
		  ////{"DA_RES_INTERMEDIATE_FEE", "ID_VER"}, --Down
    //        {"DA_RES_LEDGE_TAND","ID_VER"},

		  ////{"DA_RES_OPENAREA", "OPENAREA_ID_VER"},--Down
    //        {"DA_RES_PANTRIES","RP_ID_VER"},	
		  ////{"DA_RES_PLOT", "RESPLT_ID_VER"},--Down
    //        {"DA_RES_PPT_WALL","RPW_ID_VER"},
    //        {"DA_RES_PRV_LIFT","RPL_ID_VER"},
    //      {"DA_RES_RGH_COMMUNITYHALLS", "ID_VER"},
    //      {"DA_RES_RGH_EWS", "ID_VER"},
    //      {"DA_RES_RGH_EWSDWELLING", "ID_VER"},
    //        {"DA_RES_ROOM_D_W","RESRDW_ID_VER"},
    //        {"DA_RES_SETBACK","RESSBID_VAR"},
    //        {"DA_RES_SPIRAL_STAIRS","RSS_ID_VER"},
    //        {"DA_RES_SQ_BLOCK","RESSQ_ID_VER"},
    //        {"DA_RES_STAIRWAYS","RS_ID_VER"},
    //        {"DA_RES_WEATHER_SHADE","RWS_ID_VER"},
    //        {"DA_SERVANT_QUARTERS","ID_VER"},
    //        {"DA_STILT","ST_ID_VER"},
    //        {"DA_STORE_ROOM","SR_ID_VER"},
    //        {"DA_VENT_SHAFT","VSHAFT_ID_VER"}, 
		  ////{"DA_VERANDA", "ID_VER"},	--Down
    //      //{"ERROR_SUMMARY", "ID_VER"},--Down
    //      {"EXCEPTIONREMARKS", "EXCPTREMRKS_ID_VER"},
    //        {"GENERAL_ERRORS","ID_VER"},	
		  ////{"I117_ERROR_SUMMARY", "ID_VER"},-Down
    //        {"I117_RE_FEE","ID_VER"},
    //        {"I117_RE_SETBACK","ID_VER"},
    //      {"RE102_PRORATA", "ID_VER"},
    //      //{"RES_INTERMEDIATE_FLOOR_HT", "INTRMDT_FLRHT_ID_VER"},--Down
    //        {"RE_BALCONY","ID_VER"},
    //        {"RE_CANOPY","ID_VER"},
    //        {"RE_CANOPY_TOTAL","ID_VER"},
    //      {"RE_CARLIFT", "ID_VER"},
    //      {"RE_COMMERCIAL_FEATURES_COUNT", "ID_VER"},
    //        {"RE_CORRIDORS","ID_VER"},
    //        {"RE_COURTYARD","ID_VER"},
    //        {"RE_COVERAGE","ID_VER"},
    //         {"RE_COVERAGE_DIFF", "ID_VER"},
    //        {"RE_DWELLING_UNIT_COUNT","ID_VER"},
    //        {"RE_FAR","ID_VER"},
    //      {"RE_FEES_DIFFERENCE", "ID_VER"},
    //        {"RE_FIREESCAPE_STAIRCASE","ID_VER"}, 
		  ////{"RE_FLOOR_WISE_PERMIT_FEE", "ID_VER"},--undefined in Production
    //        {"RE_HEIGHT","ID_VER"},
    //        {"RE_INDIVIDUAL_DWELLING_COUNT","ID_VER"},
    //        {"RE_LOFT","ID_VER"},
    //        {"RE_LOFT_HT","ID_VER"},
    //        {"RE_NOTE","ID_VER"},
    //      {"RE_NOTIFIED_DWELLING_UNIT_COUNT", "ID_VER"},
		  ////{"RE_NOTIFIED_ERROR_SUMMARY", "ID_VER"},--Down
		  //{"RE_NOTIFIED_RAMPS", "ID_VER"},
    //      {"RE_NOTIFIED_STAIRCASE", "ID_VER"},
    //      {"RE_OFFICE", "ID_VER"},
    //        {"RE_PARKING","ID_VER"},
    //        {"RE_PARKING_TOTAL_NO","ID_VER"},
    //        {"RE_PASSAGEWAYS_WT","ID_VER"},
    //        {"RE_PERGOLA","ID_VER"},
    //        {"RE_PERGOLA_TOTAL","ID_VER"}, 
		  ////{"RE_RES_BASEMENT","ID_VER"},--Down
    //        {"RE_RES_BNDRY_WALL","ID_VER"},
    //        {"RE_RES_CUPBOARD_SHELVES","ID_VER"},
    //        {"RE_RES_FEE","ID_VER"},
		  // // {"RE_RES_GARAGE","ID_VER"},--Down
    //        {"RE_RES_HAND_RAILS","ID_VER"},
    //        {"RE_RES_HEADROOM_STAIRCASE","ID_VER"},
    //        {"RE_RES_LEDGE_TAND","ID_VER"},
    //        {"RE_RES_LEDGE_TAND_HT","ID_VER"},
    //      {"RE_RES_NOTIFIED_FEES","ID_VER"},
    //        {"RE_RES_PANTRIES","ID_VER"},
    //        {"RE_RES_PARAPET_WALL","ID_VER"},
    //        {"RE_RES_PROVSION_LIFT","ID_VER"},
    //      {"RE_RES_RGH_COMMUNITYHALLS","ID_VER"},
    //      {"RE_RES_RGH_EWSDWELLING","ID_VER"},
    //        {"RE_RES_SPIRAL_STAIRS","ID_VER"},
    //        {"RE_RES_STAIRWAYS","ID_VER"}, 
		  ////{"RE_RES_STILT","ID_VER"},	 --Down
    //        {"RE_RES_STORE_ROOM","ID_VER"},	
		  ////{"RE_RES_TOTAL_CUPBOARD_SHELVES","ID_VER"}, --Down
    //        {"RE_RES_WEATHER_SHD","ID_VER"},
    //        {"RE_ROOMS","ID_VER"},
    //        {"RE_SERVANT_QUARTERS","ID_VER"},
    //        {"RE_SETBACK","ID_VER"},
    //        {"RE_SHAFT","ID_VER"},	
		  ////{"RE_SHOP","ID_VER"},
    //        {"RE_VENTILATION","ID_VER"},	
		  ////{"RE_VERANDA","ID_VER"}, --Down
    //        {"ERROR_SUMMARY","ID_VER"},
    //        {"I117_ERROR_SUMMARY","ID_VER"},
    //        {"RE_NOTIFIED_ERROR_SUMMARY","ID_VER"},
    //        {"EXCEPTIONREMARKS","EXCPTREMRKS_ID_VER"},
    //        {"DA_RES_DWELLING","RESDU_ID_VER"},
    //        {"DA_RES_FLOOR","RESFLR_ID_VER"},
    //        {"DA_RES_FLOOR_HT","RESFLRHT_ID_VER"},
    //        {"RE_RES_STILT","ID_VER"},
    //        {"RE_RES_GARAGE","ID_VER"},
    //        {"RE_RES_TOTAL_CUPBOARD_SHELVES","ID_VER"},
    //        {"DA_RES_INTERMEDIATE_FEE","ID_VER"},
    //        {"DA_RES_COV_FEE","ID_VER"},
    //        {"DA_RES_OPENAREA","OPENAREA_ID_VER"},
    //        {"DA_VERANDA","ID_VER"},
    //        {"RE_VERANDA","ID_VER"},
    //        {"RES_INTERMEDIATE_FLOOR_HT","INTRMDT_FLRHT_ID_VER"},
    //        {"RE_RES_BASEMENT","ID_VER"},
    //        {"DA_RES_PLOT","RESPLT_ID_VER"},

    //    };




    //            Int32 appstage = (Int32)FileDtApp.Rows[i]["status"];
    //            switch (appstage)
    //            {
    //                case 23:

    //                case 40:
    //                    FunctionsNvar.ExecuteNquery( ";update  file_watch set status = 2,status_temp=null " +
    //                                             "where fr_id_ver = '" + FR_ID_VER + "' ;commit;");
    //                    break;
    //                case 24:
    //                case 41:
    //                    FunctionsNvar.ExecuteNquery( ";update  file_watch set status = 3,status_temp=null " +
    //                                            "where fr_id_ver = '" + FR_ID_VER + "' ;commit;");
    //                    break;
    //                default:

    //                    for (int k = 0; k <= tablecolumn.GetUpperBound(0); k++)
    //                    {
    //                        string s1 = tablecolumn[k, 0]; // Table names
    //                        string s2 = tablecolumn[k, 1]; //id Column names

    //                        deleteData(s1, s2, FR_ID_ver);
    //                    }
    //                    FunctionsNvar.ExecuteNquery( ";update  file_watch set status = 1,status_temp=null " +
    //                                             "where fr_id_ver = '" + FR_ID_VER + "' ;commit;");
    //                    break;

    //            }

                System.Data.DataTable MainDtApp = FunctionsNvar.Executequery("select status from  file_watch where FR_ID_VER = '" + FR_ID_ver + "';commit;");
                int updatedAppstage = int.Parse(MainDtApp.Rows[0][0].ToString());
                //-->>> Included Appstage log on 29-05-2013 By Kiran  
                AppStageslog.DebugLog("startupRecovery()- Updated status to " + updatedAppstage + "  and status_temp to NULL for FR_ID_VER -('" + FR_ID_ver + "')");
                //<<<-- Included Appstage log on 29-05-2013 By Kiran 
            }
            FilePathLabel.Text = "Recovery Completed";
            log.Debug("startupRecovery()- Completed");
        }

        private void deleteData(string table, string idColumn, string idver)
        {
            FunctionsNvar.ExecuteNquery( ";delete from " + table + " where " + idColumn + " =  " + idver + " ;commit;");
        }



        private void MainFrm_Shown(object sender, EventArgs e)
        {
            log.Debug("MainFrm_Shown()- Started");
            startupRecovery();
            ChkDbAcadTimer.Enabled = true;
            ChkDbDocTimer.Enabled = true;
            log.Debug("MainFrm_Shown()- Completed");
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {

        }

    }

}
