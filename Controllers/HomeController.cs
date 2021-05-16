using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using yazilimYapimi.classes;
using yazilimYapimi.Models;


namespace yazilimYapimi.Controllers
{
    
    public class HomeController : Controller
    {
        yazilimyapimiEntities db = new yazilimyapimiEntities();

        List<obakiye> obakiyes = new List<obakiye>();

        List<bakiyesahibi> bakiyesahibis = new List<bakiyesahibi>();

        public ActionResult Index()
        {
            obakiyes = db.obakiye.ToList();
            foreach (var item in obakiyes)
            {
                bakiyesahibi bakiyesahibi = new bakiyesahibi();
                bakiyesahibi.kid = item.k_id;
                bakiyesahibi.para =Convert.ToInt32(item.bakiye);
                bakiyesahibi.kullanici = db.kullanici.FirstOrDefault(x => x.k_id == item.k_id).kad;
                bakiyesahibis.Add(bakiyesahibi);
            }
             ViewBag.bakiye=bakiyesahibis;
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

        [HttpPost]
        public ActionResult Login(string kad,string sifre)
        {
            if (kad=="admin"&&sifre=="123")
            {
                Session["yetki"] = 2;
                return RedirectToAction("Urunler");
            }

            var kullanici = db.kullanici.FirstOrDefault(x => x.kad == kad && x.sifre == sifre);
            if (kullanici!=null)
            {
                Session["yetki"] = kullanici.yetki;
                Session["k_id"] = kullanici.k_id;
                return RedirectToAction("Urunler");
            }
            else
            {
                ViewBag.hata = "hatalı giriş";
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public ActionResult bakiyeEkle(string bakiye)
        {
            foreach (var item in obakiyes)
            {
                bakiyesahibi bakiyesahibi = new bakiyesahibi();
                bakiyesahibi.kid = item.k_id;
                bakiyesahibi.para = Convert.ToInt32(item.bakiye);
                bakiyesahibi.kullanici = db.kullanici.FirstOrDefault(x => x.k_id == item.k_id).kad;
                bakiyesahibis.Add(bakiyesahibi);
            }
            ViewBag.bakiye = bakiyesahibis;

            obakiye obakiye = new obakiye()
            {
                bakiye = Convert.ToInt32(bakiye),
                k_id=Convert.ToInt32(Session["k_id"])
            };
            db.obakiye.Add(obakiye);
            db.SaveChanges();
            return RedirectToAction("Urunler");
        }

        



        public ActionResult Cikis()
        {
            Session.Clear();
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
            obakiyes = db.obakiye.ToList();
            foreach (var item in obakiyes)
            {
                bakiyesahibi bakiyesahibi = new bakiyesahibi();
                bakiyesahibi.kid = item.k_id;
                bakiyesahibi.para = Convert.ToInt32(item.bakiye);
                bakiyesahibi.kullanici = db.kullanici.FirstOrDefault(x => x.k_id == item.k_id).kad;
                bakiyesahibis.Add(bakiyesahibi);
            }
            ViewBag.bakiye = bakiyesahibis;
            ViewBag.Message = "Ürün ekleme sayfası.";

            return View();
        }

        public ActionResult BakiyeEkle()
        {
            obakiyes = db.obakiye.ToList();
            foreach (var item in obakiyes)
            {
                bakiyesahibi bakiyesahibi = new bakiyesahibi();
                bakiyesahibi.kid = item.k_id;
                bakiyesahibi.para = Convert.ToInt32(item.bakiye);
                bakiyesahibi.kullanici = db.kullanici.FirstOrDefault(x => x.k_id == item.k_id).kad;
                bakiyesahibis.Add(bakiyesahibi);
            }
            ViewBag.bakiye = bakiyesahibis;
            ViewBag.Message = "Bakiye ekleme.";

            return View();
        }

        public ActionResult Urunler()
        {
            obakiyes = db.obakiye.ToList();
            foreach (var item in obakiyes)
            {
                bakiyesahibi bakiyesahibi = new bakiyesahibi();
                bakiyesahibi.kid = item.k_id;
                bakiyesahibi.para = Convert.ToInt32(item.bakiye);
                bakiyesahibi.kullanici = db.kullanici.FirstOrDefault(x => x.k_id == item.k_id).kad;
                bakiyesahibis.Add(bakiyesahibi);
            }
            ViewBag.bakiye = bakiyesahibis;
            ViewBag.Message = "Urun listeleme.";

            return View();
        }

        public ActionResult BakiyeOnay(string id,string onay,string bakiye="0")
        {
            int kid = Convert.ToInt32(id);
            int para = Convert.ToInt32(bakiye);

            if (onay=="1")
            {
                var user = db.kullanici.FirstOrDefault(x => x.k_id == kid);
                user.bakiye += para;
                var b = db.obakiye.FirstOrDefault(x => x.k_id == kid);
                db.obakiye.Remove(b);
                db.SaveChanges();
            }
            else
            {
                var b = db.obakiye.FirstOrDefault(x => x.k_id == kid);
                db.obakiye.Remove(b);
                db.SaveChanges();
            }

            return Redirect("/");
        }
    }
}