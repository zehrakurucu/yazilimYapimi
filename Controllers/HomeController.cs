using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
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
        public ActionResult Index()//kayıt olmas sayfası
        {
            onayListele();//admin sayfasına onay bildirimlerini ekleme
            return View();
        }
        public ActionResult UrunEkle()// ürün ekleme sayfası
        {
            onayListele();//admin sayfasına onay bildirimlerini ekleme
            ViewBag.Message = "Ürün ekleme sayfası.";
            return View();
        }
        public ActionResult OtoSatinAl()
        {
            onayListele(); //admin sayfasına onay bildirimlerini ekleme
            ViewBag.Message = "Otomatik Satın alma sayfası.";

            List<urunSiparis> urunSiparisleri = new List<urunSiparis>();
            int k_id =Convert.ToInt32( Session["k_id"]);//kullanıcı id çekme
            urunSiparisleri = db.urunSiparis.Where(x => x.k_id == k_id).ToList();//veritabanında sipariş listesini çekme
            ViewBag.urunSiparisleri = urunSiparisleri;

            return View();
        }

        public ActionResult BakiyeEkle()//bakiye ekleem sayfası
        {
            onayListele();// admin sayfasına onay bildirimlerini ekleme
            ViewBag.Message = "Bakiye ekleme.";
            return View();
        }

        public ActionResult Urunler()
        {
            onayListele(); //admin sayfasına onay bildirimlerini ekleme
            ViewBag.Message = "Urun listeleme.";

            List<urunler> urunlers = new List<urunler>();
            urunlers = db.urunler.ToList();//ürün listesii çekme
            List<urun> uruns = new List<urun>();//ürün bilgileri
            foreach (var item in urunlers)//ürün bilgilerini doğru şekilde doldurma
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
            ViewBag.urunList = uruns;//ürün listesi

            return View();
        }

        public void onayListele()//admin sayfaları için bildiirimleri göstertme fonksiyonu
        {
            ourunlers = db.ourunler.ToList();
            foreach (var item in ourunlers)//kullanıcı idye göre kullanıcının adını çekme ürün ekleme isteği
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
            foreach (var item in obakiyes)//kullanıcı idye göre kullanıcının adını çekme bakiye ekleme isteği
            {
                bakiyesahibi bakiyesahibi = new bakiyesahibi();
                bakiyesahibi.kid = item.k_id;
                bakiyesahibi.para = Convert.ToInt32(item.bakiye);
                bakiyesahibi.kullanici = db.kullanici.FirstOrDefault(x => x.k_id == item.k_id).kad;

                if (item.cins!="TL")//para birimine göre kur bilgisini yazdırma
                {
                    bakiyesahibi.kur = Convert.ToDouble(DovizKur(item.cins));
                }
                else
                {
                    bakiyesahibi.kur = 1;
                }

                if (item.cins=="TL")//ekrana yazdırırken düzgün yazdırma
                {
                    bakiyesahibi.cins = "TL";
                    
                }
                else if (item.cins == "USD")//dolar
                {
                    bakiyesahibi.cins = "Dolar";
                    
                }
                else if (item.cins == "EUR")//eurp
                {
                    bakiyesahibi.cins = "Euro";
                }
                else //GBP sterlin
                {
                    bakiyesahibi.cins = "Sterlin";
                }
                //bakiyesahibi.cins = item.cins;
                bakiyesahibis.Add(bakiyesahibi);
            }
            ViewBag.bakiye = bakiyesahibis;//bakiye bildirimleri listesi
        }
        public ActionResult SatisRapor()//rapor sayfası
        {
            return View();
        }


        [HttpPost]//rapor sayfasında isteğe göre rapor olıştuırma
        public ActionResult SatisRaporFiltre(DateTime ilkTarih,DateTime ikinciTarih,string aldiklarim,string sattiklarim)
        {
            int kullaniciId = Convert.ToInt32(Session["k_id"]);
            logSatisInfo logSatisInfo = new logSatisInfo();

            if (aldiklarim!=null)//alınnan ürünler seçili ise
            {
                List<logSatisInfo> logSatisInfos = new List<logSatisInfo>();
                logSatisInfos = logSatisInfo.alisList(kullaniciId);//alış listesi çekiliyor

                for (int i = logSatisInfos.Count - 1; i >= 0; i--)//istenilen tarihlerde olmayanlar siliniyor
                {
                    int t1 = DateTime.Compare(logSatisInfos[i].Tarih, ilkTarih);
                    int t2 = DateTime.Compare(logSatisInfos[i].Tarih, ikinciTarih.AddDays(1));
                    if (t1 < 0 || t2 > 0)
                    {
                        logSatisInfos.RemoveAt(i);
                    }
                }

                ViewBag.Aldiklarim = logSatisInfos;//satış listesi

            }
            if (sattiklarim != null)//satılan ürünler seçili ise
            {
                List<logSatisInfo> logSatisInfos = new List<logSatisInfo>();
                logSatisInfos = logSatisInfo.satisList(kullaniciId);//satış listesi çekiliyor

                for (int i = logSatisInfos.Count-1; i >=0; i--)//istenilen tarihlerde olmayanlar siliniyor
                {
                    int t1 = DateTime.Compare(logSatisInfos[i].Tarih, ilkTarih);
                    int t2 = DateTime.Compare(logSatisInfos[i].Tarih, ikinciTarih.AddDays(1));
                    if (t1 < 0 || t2 > 0)
                    {
                        logSatisInfos.RemoveAt(i);
                    }
                }
                ViewBag.Sattiklarim = logSatisInfos;//alış listesi
            }
            return View("SatisRapor");
        }


        //Kayıt işlemi------------------------------------------------------------------------------------------------------

        [HttpPost]
        public ActionResult Register(string ad, string soyad, string kad, string tc, string sifre, string tel, string mail, string adres, bool yetki = true)
        {
            kullanici kullanici = new kullanici()//yeni kullanıcı oluşturulıyort
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

            db.kullanici.Add(kullanici);//oluşturulan kullanıcı db kaydedliyor
            db.SaveChanges();

            return RedirectToAction("Index");

        }

        [HttpPost]//giriş kontrolleri
        public ActionResult Login(string kad, string sifre)
        {
            if (kad == "admin" && sifre == "123")
            {
                Session["yetki"] = 2;
                return RedirectToAction("Urunler");
            }

            var kullanici = db.kullanici.FirstOrDefault(x => x.kad == kad && x.sifre == sifre);//kullanıcı kontrol ediliyor
            if (kullanici != null)
            {
                Session["yetki"] = kullanici.yetki;
                Session["k_id"] = kullanici.k_id;
                Session["para"] = kullanici.bakiye;

                return RedirectToAction("Urunler");
            }
            else
            {
                ViewBag.hata = "hatalı giriş";//hata nesajı
                return RedirectToAction("Index");
            }
        }
        public ActionResult Cikis()//çıkış işlemi
        {
            Session.Clear();
            return RedirectToAction("Index");
        }


        //bakiye ekle- bakiye admin onay------------------------------------------------------------------------------------------------------

        [HttpPost]//bakiye ekleme isteği
        public ActionResult bakiyeEkle(string bakiye,string cins)
        {
            obakiye obakiye = new obakiye()//veritabanına kaydeilmek üzere OnayBakiye oluşturuluyor
            {
                bakiye = Convert.ToInt32(bakiye),
                k_id = Convert.ToInt32(Session["k_id"]),
                cins = cins
            };
            db.obakiye.Add(obakiye);//veritabanına kaydediliyor
            db.SaveChanges();
            return RedirectToAction("Urunler");
        }
        public ActionResult BakiyeOnay(string id, string onay,string cins, string bakiye="0")//admin onayladığında
        {

            int kid = Convert.ToInt32(id);//bakiye eklenicek kişinin idsi

            if (onay == "1")//admin onayladı ise
            {

                var user = db.kullanici.FirstOrDefault(x => x.k_id == kid);
                user.bakiye +=Convert.ToInt32( Convert.ToInt32(bakiye)*Convert.ToDouble(  DovizKur(cins)));//kullanıcıya birime göre kurla çarpılıp bakiye ekleniyor
                var b = db.obakiye.FirstOrDefault(x => x.k_id == kid);
                db.obakiye.Remove(b);//onaylama bildirimi siliniyıor
                db.SaveChanges();
            }
            else//admin onaylamadı ise
            {
                var b = db.obakiye.FirstOrDefault(x => x.k_id == kid);
                db.obakiye.Remove(b);//onaylama bildirimi siliniyor
                db.SaveChanges();
            }

            return Redirect("Urunler");
        }
        decimal DovizKur(string Cins)
        {
            XDocument xDocument = XDocument.Load("http://www.tcmb.gov.tr/kurlar/today.xml"); //verileri çekmek 
            
            var s = xDocument.Element("Tarih_Date").Elements("Currency").FirstOrDefault(a => a.Attribute("Kod").Value == Cins);
            var bElement = s.Element("ForexBuying");
            decimal fiyat = Convert.ToDecimal(bElement.Value.Replace('.', ','));// noktayı virgül yapmak için 
            
            return fiyat;
        }

        //urun ekle -urun onay------------------------------------------------------------------------------------------------------
        public ActionResult urunEkleme(string urun, string adet, string fiyat)
        {//ürün ekleme isteği oluşturuluyor
            ourunler ourunler = new ourunler()
            {
                k_id = Convert.ToInt32(Session["k_id"]),
                miktar = Convert.ToInt32(adet),
                fiyat = Convert.ToInt32(fiyat),
                urun = urun
            };
            db.ourunler.Add(ourunler);//ürün ekleme isteği db kaydoluyor
            db.SaveChanges();
            return RedirectToAction("UrunEkle");
        }
        public ActionResult UrunOnay(string urun_id, string miktar, string k_id, string urun, string fiyat, string onay)
        {//admin onaylamak itediğinde
            int u_id = Convert.ToInt32(urun_id);//ürün ekleyen kişinin id si
            if (onay == "1")//admin ürün eklemeyi onayladı ise
            {
                urunler urun1 = new urunler()//ürün oluşturuluyor
                {
                    u_id = u_id,
                    k_id = Convert.ToInt32(k_id),
                    miktar = Convert.ToInt32(miktar),
                    fiyat = Convert.ToInt32(fiyat),
                    urun = urun
                };
                db.urunler.Add(urun1);//ürün ekleniyor

                var b = db.ourunler.FirstOrDefault(x => x.urun_id == u_id);
                db.ourunler.Remove(b);//ürün ekleme isteği siliniyor
                db.SaveChanges();

                OtoSatis();// ürün eklendiğinde sipariş veren kişi var ise satış yapılıypr
            }
            else//admin ürün eklemeyi onaylamadı ise
            {
                var b = db.ourunler.FirstOrDefault(x => x.urun_id == u_id);
                db.ourunler.Remove(b);//ürün ekleme isyeği siliniyor
                db.SaveChanges();
            }
            
            return Redirect("Urunler");
        }

        void OtoSatis() // ürün listesi ve sipariş listesini karşılaştırıyor 
        {
            List<urunSiparis> siparisler = new List<urunSiparis>();
            siparisler = db.urunSiparis.ToList();//sipariş listesini çekiyor

            List<urunler> urunler = new List<urunler>();
            urunler = db.urunler.ToList();//ürün listesini çekiyor

            foreach (var siparis in siparisler)
            {
                foreach (var urun in urunler)
                {
                    //ürün adeti yeterli ise, fiyat eşit ya da ucuz ise  ve ürün aynı ürün ise
                    if (urun.urun==siparis.urun&&urun.miktar>=siparis.adet&&urun.fiyat<=siparis.fiyat)
                    {
                        urun.miktar -= siparis.adet;//ürün adetini azaltıyor

                        var satici = db.kullanici.FirstOrDefault(x => x.k_id == urun.k_id);
                        satici.bakiye += urun.fiyat * siparis.adet;//satıcıya ücreti ekliyor

                        var alici=db.kullanici.FirstOrDefault(x => x.k_id == siparis.k_id);//alıcıdan ücret alınıyor ve komisyon kesiliyor
                        alici.bakiye -=Convert.ToInt32( (urun.fiyat * siparis.adet) + ((urun.fiyat * siparis.adet) * 0.01));

                        var muhasebe = db.kullanici.FirstOrDefault(x => x.k_id == 9);//muhasebeye komisyon ekleniyor
                        muhasebe.bakiye+=Convert.ToInt32(urun.fiyat * siparis.adet * 0.01);

                        //satış işlemi kayıt ediliyor
                        LogRaporSave(alici.k_id,satici.k_id,urun.urun,Convert.ToInt32( urun.fiyat), Convert.ToInt32(siparis.adet));

                        if (urun.miktar==0)//ürün bitti ise ürün siliniyor
                        {
                            db.urunler.Remove(urun);
                        }
                        db.urunSiparis.Remove(siparis);//sipariş siliniyor
                    }

                }
            }
            db.SaveChanges();
        }

        //satış işlemi kayıt ediliyor
        void LogRaporSave(int alici,int satici,string urun,int fiyat,int adet)
        {
            logSatis logSatis = new logSatis();

            logSatis.Tarih = DateTime.Now;
            logSatis.Urun = urun;
            logSatis.Fiyat = fiyat;
            logSatis.Miktar = adet;
            logSatis.AliciId = alici;
            logSatis.SaticiId = satici;

            db.logSatis.Add(logSatis);
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
                  
                    musteri.bakiye -= Convert.ToInt32((alinacakUrun.fiyat * miktar) + ((alinacakUrun.fiyat * miktar) * 0.01));//bakiyeden düşülüyor

                    var muhasebe = db.kullanici.FirstOrDefault(x => x.k_id == 9);
                    muhasebe.bakiye += Convert.ToInt32(alinacakUrun.fiyat * miktar * 0.01);

                    Session["para"] = musteri.bakiye;
                    Session["bildirim"]= "satin alim basarili " + miktar + " adet " + alinacakUrun.urun + alinacakUrun.fiyat + " fiyatindan satin alindi,toplam odenen: " + (alinacakUrun.fiyat * miktar);
                   
                    LogRaporSave(musteri.k_id, Convert.ToInt32(alinacakUrun.k_id), alinacakUrun.urun, Convert.ToInt32(fiyat), Convert.ToInt32(adet));

                    if (alinacakUrun.miktar == 0)//ürün bitti ise siliniyor
                    {
                        db.urunler.Remove(alinacakUrun);
                    }
                }
                else
                {
                    Session["bildirim"] = "Yetersiz Bakiye";
                }
            }
            else//istenilen ürün yok ise sipariş listesine ekleniyor
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