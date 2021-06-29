using SMGJ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.Build.Tasks;
using Microsoft.Reporting.WebForms;

namespace SMGJ.Controllers
{
    public class RAPORTIMIController : BaseController
    {

        SMGJDB db = new SMGJDB();
        // GET: RAPORTIMI
        public async Task<ActionResult> Index()
        {
            var user = await GetUser();

            //e mbushni nji dropdown me raporte enums ose database, e thirrni ne index
            return View();
        }


        //Mrena e krijoni nji model me i dergu si parametra 
        public void HapRaportin()
        {
            var user = (GetUser)Session["user"];
            //string data_prej = Convert.ToString(model.Prej);
            //string data_deri = Convert.ToString(model.Deri);

            //if (data_prej != "")
            //{
            //    data_prej = data_prej.Split('/')[2].Trim() + "-" + data_prej.Split('/')[1] + "-" + data_prej.Split('/')[0];
            //}
            //if (data_deri != "")
            //{
            //    data_deri = data_deri.Split('/')[2].Trim() + "-" + data_deri.Split('/')[1] + "-" + data_deri.Split('/')[0];
            //}

            //Session["strQueryReport"] = "EXEC test";
            //Session["strEmriRaportit"] = "Raportet\\test.rdl";
        }

        // duhet me e dergu nji parameter ne RAPORT_DESIGN per me caktu formatin
        public ActionResult RAPORTI_DESIGN()
        {
            DataTable dt = RunQuery(Session["strQueryReport"].ToString(), "Tabela").Tables[0];
            ReportViewer rvRaporti = new ReportViewer();
            rvRaporti.ProcessingMode = ProcessingMode.Local;
            rvRaporti.LocalReport.DataSources.Clear();

            rvRaporti.LocalReport.ReportPath = Session["strEmriRaportit"].ToString();
            rvRaporti.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dt));


            //pdf format
            
                string reportType = "PDF";
                string mimeType;
                string encoding;
                string fileNameExtension;
                int page_width = 13;
                string deviceInfo = "<DeviceInfo>" + "  <OutputFormat>PDF</OutputFormat>" + "  <PageWidth>" + page_width + "in</PageWidth>" + "  <PageHeight>8.5in</PageHeight>" + "  <MarginTop>0.5in</MarginTop>" + "  <MarginLeft>0in</MarginLeft>" + "  <MarginRight>0in</MarginRight>" + "  <MarginBottom>0.5in</MarginBottom>" + "</DeviceInfo>";

                Microsoft.Reporting.WebForms.Warning[] warnings;
                string[] streams;
                byte[] renderedBytes;

                renderedBytes = rvRaporti.LocalReport.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);

                string lContentType = "application/pdf";
                Response.Buffer = true;
                Response.Clear();
                Response.ClearContent();
                Response.ClearHeaders();
                Response.ContentType = lContentType;
                Response.AddHeader("Content-Length", renderedBytes.Length.ToString());
                Response.AddHeader("content-disposition", "inline;filename=Raporti.pdf");
                Response.AddHeader("Content-Type", lContentType);
                Response.BinaryWrite(renderedBytes);
                Response.End();


            // detyra e juve me bo ne XLS format


            return View();
        }


        private static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public DataSet RunQuery(string Query, string tableName)
        {

            SqlConnection Connection = new SqlConnection(connectionString);
            DataSet dataSet = new DataSet();
            Connection.Open();
            SqlDataAdapter sqlDA = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand(Query, Connection);
            cmd.CommandType = CommandType.Text;
            sqlDA.SelectCommand = cmd;
            sqlDA.Fill(dataSet, tableName);
            cmd.CommandTimeout = 20000;
            Connection.Close();
            return dataSet;
        }

        // GET: RAPORTIMI/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: RAPORTIMI/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RAPORTIMI/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: RAPORTIMI/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: RAPORTIMI/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: RAPORTIMI/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: RAPORTIMI/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
