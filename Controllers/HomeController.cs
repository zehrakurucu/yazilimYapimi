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

        List<ourunler> ourunlers = new List<ourunler>();

        List<bakiyesahibi> bakiyesahibis = new List<bakiyesahibi>();

        List<urunsahibi> urunsahibis = new List<urunsahibi>();

        //sayfa action resultları------------------------------------------------------------------------------------------------------
        public ActionResult Index()
        {
            onayListele();
            return View();
        }
        public ActionResult UrunEkle()
        {
            onayListele();
            ViewBag.Message = "Ürün ekleme sayfası.";
            return View();
        }
        public ActionResult OtoSatinAl()
        {
            onayListele();
            ViewBag.Message = "Otomatik Satın alma sayfası.";



            List<urunSiparis> urunSiparisleri = new List<urunSiparis>();
            int k_id =Convert.ToInt32( Session["k_id"]);
            urunSiparisleri = db.urunSiparis.Where(x => x.k_id == k_id).ToList();
            ViewBag.urunSiparisleri = urunSiparisleri;


            return View();
        }

        public ActionResult BakiyeEkle()
        {
            onayListele();
            ViewBag.Message = "Bakiye ekleme.";
            return View();
        }

        public ActionResult Urunler()
        {
            onayListele();
            ViewBag.Message = "Urun listeleme.";

            List<urunler> urunlers = new List<urunler>();
            urunlers = db.urunler.ToList();
            List<urun> uruns = new List<urun>();
            foreach (var item in urunlers)
            {
                int kullanici_id = Convert.ToInt32(item.k_id);
                urun urun = new urun()
                {
                    urunadi = item.urun,
                    fiyat = Convert.ToInt32(item.fiyat),
                    k_id = kullanici_id,
                    miktar = Convert.ToInt32(item.miktar),
                    u_id = Convert.ToInt32(item.u_id),
                    satici = db.kullanici.FirstOrDefault(x => x.k_id == kullanici_id).kad
                };
                uruns.Add(urun);
            }
            ViewBag.urunList = uruns;

            return View();
        }

        public void onayListele()
        {
            ourunlers = db.ourunler.ToList();
            foreach (var item in ourunlers)
            {
                urunsahibi urunSahibi = new urunsahibi();
                urunSahibi.fiyat = Convert.ToInt32(item.fiyat);
                urunSahibi.k_id = Convert.ToInt32(item.k_id);
                urunSahibi.miktar = Convert.ToInt32(item.miktar);
                urunSahibi.urun = item.urun;
                urunSahibi.urun_id = Convert.ToInt32(item.urun_id);
                urunSahibi.kullanici = db.kullanici.FirstOrDefault(x => x.k_id == item.k_id).kad;
                urunsahibis.Add(urunSahibi);
            }
            ViewBag.urun = urunsahibis;

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
        }


        //Kayıt işlemi------------------------------------------------------------------------------------------------------

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
        public ActionResult Login(string kad, string sifre)
        {
            if (kad == "admin" && sifre == "123")
            {
                Session["yetki"] = 2;
                return RedirectToAction("Urunler");
            }

            var kullanici = db.kullanici.FirstOrDefault(x => x.kad == kad && x.sifre == sifre);
            if (kullanici != null)
            {
                Session["yetki"] = kullanici.yetki;
                Session["k_id"] = kullanici.k_id;
                Session["para"] = kullanici.bakiye;



                return RedirectToAction("Urunler");
            }
            else
            {
                ViewBag.hata = "hatalı giriş";
                return RedirectToAction("Index");
            }
        }
        public ActionResult Cikis()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }


        //bakiye ekle- bakiye admin onay------------------------------------------------------------------------------------------------------

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
                k_id = Convert.ToInt32(Session["k_id"])
            };
            db.obakiye.Add(obakiye);
            db.SaveChanges();
            return RedirectToAction("Urunler");
        }
        public ActionResult BakiyeOnay(string id, string onay, string bakiye = "0")
        {
            int kid = Convert.ToInt32(id);

            if (onay == "1")
            {
                var user = db.kullanici.FirstOrDefault(x => x.k_id == kid);
                user.bakiye += Convert.ToInt32(bakiye);
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

            return Redirect("Urunler");
        }

        //urun ekle -urun onay------------------------------------------------------------------------------------------------------
        public ActionResult urunEkleme(string urun, string adet, string fiyat)
        {
            ourunler ourunler = new ourunler()
            {
                k_id = Convert.ToInt32(Session["k_id"]),
                miktar = Convert.ToInt32(adet),
                fiyat = Convert.ToInt32(fiyat),
                urun = urun
            };
            db.ourunler.Add(ourunler);
            db.SaveChanges();
            return RedirectToAction("UrunEkle");
        }
        public ActionResult UrunOnay(string urun_id, string miktar, string k_id, string urun, string fiyat, string onay)
        {
            int u_id = Convert.ToInt32(urun_id);
            if (onay == "1")
            {
                urunler urun1 = new urunler()
                {
                    u_id = u_id,
                    k_id = Convert.ToInt32(k_id),
                    miktar = Convert.ToInt32(miktar),
                    fiyat = Convert.ToInt32(fiyat),
                    urun = urun
                };
                db.urunler.Add(urun1);

                var b = db.ourunler.FirstOrDefault(x => x.urun_id == u_id);
                db.ourunler.Remove(b);
                db.SaveChanges();

                OtoSatis();
            }
            else
            {
                var b = db.ourunler.FirstOrDefault(x => x.urun_id == u_id);
                db.ourunler.Remove(b);
                db.SaveChanges();
            }
            
            return Redirect("Urunler");
        }

        void OtoSatis() 
        {
            List<urunSiparis> siparisler = new List<urunSiparis>();
            siparisler = db.urunSiparis.ToList();

            List<urunler> urunler = new List<urunler>();
            urunler = db.urunler.ToList();

            foreach (var siparis in siparisler)
            {
                foreach (var urun in urunler)
                {

                    if (urun.urun==siparis.urun&&urun.miktar>=siparis.adet&&urun.fiyat<=siparis.fiyat)
                    {
                        urun.miktar -= siparis.adet;

                        var satici = db.kullanici.FirstOrDefault(x => x.k_id == urun.k_id);
                        satici.bakiye += urun.fiyat * siparis.adet;

                        var alici=db.kullanici.FirstOrDefault(x => x.k_id == siparis.k_id);
                        alici.bakiye-= urun.fiyat * siparis.adet;


                        if (urun.miktar==0)
                        {
                            db.urunler.Remove(urun);
                        }
                        db.urunSiparis.Remove(siparis);
                    }

                }
            }
            db.SaveChanges();
        }

        //otomatik satın alma------------------------------------------------------------------------------------------------------

        public ActionResult ActionOtoSatinAL(string urun, string adet, string fiyat)
        {
            int miktar = Convert.ToInt32(adet);
            int ucret = Convert.ToInt32(fiyat);
            int kullaniciId = Convert.ToInt32(Session["k_id"]);


            var alinacakUrun = db.urunler.FirstOrDefault(x => x.miktar >= miktar && x.fiyat <= ucret && x.urun == urun);

            var musteri = db.kullanici.FirstOrDefault(x => x.k_id == kullaniciId);
            if (alinacakUrun != null)// istenilen miktarda istenilen ücretin altında ürün var ise
            {
                if (musteri.bakiye >= (miktar * alinacakUrun.fiyat))//müşteride yeterli para var ise
                {
                    alinacakUrun.miktar -= miktar;//ürün adeti düşürülüyor
                    if (alinacakUrun.miktar == 0)//ürün bitti ise siliniyor
                    {
                        db.urunler.Remove(alinacakUrun);
                    }

                    musteri.bakiye -= miktar * alinacakUrun.fiyat;//bakiyeden düşülüyor
                    Session["para"] = musteri.bakiye;
                    Session["bildirim"]= "satin alim basarili " + miktar + " adet " + alinacakUrun.urun + alinacakUrun.fiyat + " fiyatindan satin alindi,toplam odenen: " + (alinacakUrun.fiyat * miktar);
                   
                }
                else
                {
                    Session["bildirim"] = "Yetersiz Bakiye";
                }
            }
            else
            {
                urunSiparis urunSiparis = new urunSiparis
                {
                    k_id = kullaniciId,
                    urun = urun,
                    fiyat = ucret,
                    adet = miktar
                };
                db.urunSiparis.Add(urunSiparis);

                Session["bildirim"] = "istenilen urun bulunamadı siparis listesine eklendi";
            }
            db.SaveChanges();
            return Redirect("OtoSatinAl");
        }


    }
}
