using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using yazilimYapimi.Models;

namespace yazilimYapimi.Controllers
{
    
    public class HomeController : Controller
    {
        yazilimyapimiEntities db = new yazilimyapimiEntities();
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(string ad, string soyad, string kad, string tc, string sifre, string tel, string mail, string adres, bool yetki = true)
        {
            kullanici kullanici = new kullanici()
            {
                ad = ad,
                soyad = soyad,
                kad = kad,
                sifre = sifre,
                tel = tel,
                mail = mail,
                adres = adres,
                yetki = yetki,
                tc = tc,
                bakiye = 0
            };

            db.kullanici.Add(kullanici);
            db.SaveChanges();


            return RedirectToAction("Index");

        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult UrunEkle()
        {
            ViewBag.Message = "Ürün ekleme sayfası.";

            return View();
        }

        public ActionResult BakiyeEkle()
        {
            ViewBag.Message = "Bakiye ekleme.";

            return View();
        }

        public ActionResult Urunler()
        {
            ViewBag.Message = "Urun listeleme.";

            return View();
        }
    }
}