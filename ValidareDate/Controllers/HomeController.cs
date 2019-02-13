﻿using DotNetDBF;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Migrations;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ValidareDate.Data;
using ValidareDate.Helpers;
using ValidareDate.Models;

namespace ValidareDate.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var dbContext = new AppDbContext())
            {
                if (dbContext.Facturi.ToList().Count > 0)
                {
                    ViewBag.showEraseBtn = true;
                }
                else
                {
                    ViewBag.showEraseBtn = false;
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase postedFile)
        {
            string errorMessage = null;
            if (postedFile != null)
            {
                string[] validFileTypes = { ".xls", ".xlsx" };

                var extension = Path.GetExtension(postedFile.FileName);

                if (validFileTypes.Contains(extension))
                {
                    string path = Server.MapPath("~/Uploads/Facturi/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    var correctFileName = postedFile.FileName.Replace('-', '_');
                    var sheetName = "rap 2";

                    var filePath = path + Path.GetFileName(correctFileName);
                    postedFile.SaveAs(filePath);

                    string connString = getConnStringByExtension(extension, filePath);

                    var dt = FileHelper.ConvertExcelToDataTable(connString, sheetName);

                    var dbContext = new AppDbContext();

                    var facturi = new List<Factura>();

                    foreach (DataRow row in dt.Rows)
                    {
                        var factura = new Factura();
                        factura.ConvertFromDataRow(row);
                        facturi.Add(factura);
                    }

                    dbContext.Facturi.AddOrUpdate(facturi.ToArray());
                    dbContext.SaveChanges();

                    ViewBag.Message = "Fisierul cu facturi fost procesat";
                    ViewBag.FilePath = filePath;
                }
                else
                {
                    errorMessage = "Fisierul nu este crespunzator. Acesta trebuie sa aiba extensia .xls sau .xlsl";
                }
            }
            else
            {
                errorMessage = "Fisier inexistent!";
            }

            if (errorMessage != null)
            {
                ViewBag.Error = errorMessage;
            }
            ViewBag.showEraseBtn = false;
            return View();
        }

        public ActionResult EmptyFacturi()
        {
            using (var dbContext = new AppDbContext())
            {
                if (dbContext.Facturi.ToList().Count > 0)
                    dbContext.Database.ExecuteSqlCommand("TRUNCATE TABLE [Facturas]");
            }

            return RedirectToAction("Index");
        }

        public ActionResult Clienti()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Clienti(HttpPostedFileBase postedFile)
        {
            string errorMessage = null;
            if (postedFile != null)
            {
                string[] validFileTypes = { ".xls", ".xlsx" };

                var extension = Path.GetExtension(postedFile.FileName);

                if (validFileTypes.Contains(extension))
                {
                    string path = Server.MapPath("~/Uploads/Clienti/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    var correctFileName = postedFile.FileName.Replace('-', '_');
                    var sheetName = postedFile.FileName.Substring(0, postedFile.FileName.LastIndexOf('.'));

                    var filePath = path + Path.GetFileName(correctFileName);
                    postedFile.SaveAs(filePath);

                    string connString = getConnStringByExtension(extension, filePath);

                    var dt = FileHelper.ConvertExcelToDataTable(connString, sheetName);

                    var dbContext = new AppDbContext();

                    var clientsList = new List<Client>();

                    foreach (DataRow row in dt.Rows)
                    {
                        var client = new Client();
                        client.ConvertFromDataRow(row);
                        clientsList.Add(client);
                    }

                    dbContext.Clienti.AddOrUpdate(clientsList.ToArray());
                    dbContext.SaveChanges();

                    ViewBag.Message = "Fisierul cu clienti fost procesat";
                    ViewBag.FilePath = filePath;
                }
                else
                {
                    errorMessage = "Fisierul nu este crespunzator. Acesta trebuie sa aiba extensia .xls sau .xlsl";
                }
            }
            else
            {
                errorMessage = "Fisier inexistent!";
            }

            if (errorMessage != null)
            {
                ViewBag.Error = errorMessage;
            }
            return View();
        }
        

        public async Task<ActionResult> ProcessFiles()
        {
            var iesiri = await IesiriHelper.verificaFacturi();
            if(iesiri.Count == 0)
            {
                ViewBag.ErrorProcess = "nu exista nicio iesire";
            }
            else
            {
                var fileIesiri = DBFHelper.genereazaFisierIesiri(iesiri, Server);
                var fileClienti = DBFHelper.genereazaFisierClienti(Server);
                ViewBag.fileIesiri = Path.GetFileName(fileIesiri);
                ViewBag.fileClienti = Path.GetFileName(fileClienti);
            }

            return View();
        }

        private string getConnStringByExtension(string extension, string filePath)
        {
            var connString = extension == ".xls"
                ? $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={filePath};Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\""
                : $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={filePath};Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
            return connString;
        }

        public FileResult Download(string filePath)
        {
            string path = Server.MapPath($"~/Download/{filePath}");

            string fileName = Path.GetFileName(path);

            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
    }
}