using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using yazilimYapimi.Models;

namespace yazilimYapimi.classes
{
    public class logSatisInfo
    {
        public DateTime Tarih { get; set; }
        public string Urun { get; set; }
        public int Fiyat { get; set; }
        public int Miktar { get; set; }
        public string Alici { get; set; }
        public string Satici { get; set; }

        yazilimyapimiEntities db = new yazilimyapimiEntities();
        public List<logSatisInfo> satisList(int kullaniciId)
        {
            List<logSatisInfo> satisInfos = new List<logSatisInfo>();
            List<logSatis> satislar = new List<logSatis>();
            satislar = db.logSatis.Where(x => x.SaticiId == kullaniciId).ToList();
            foreach (var item in satislar)
            {
                logSatisInfo logSatisInfo = new logSatisInfo()
                {
                    Tarih = Convert.ToDateTime(item.Tarih),
                    Urun = item.Urun,
                    Fiyat = Convert.ToInt32(item.Fiyat),
                    Miktar = Convert.ToInt32(item.Miktar),
                    Alici = db.kullanici.FirstOrDefault(x => x.k_id == item.AliciId).kad,
                    Satici = db.kullanici.FirstOrDefault(x => x.k_id == item.SaticiId).kad,
                };
                satisInfos.Add(logSatisInfo);
            }
            return satisInfos;
        }

        public List<logSatisInfo> alisList(int kullaniciId)
        {
            List<logSatisInfo> alisInfos = new List<logSatisInfo>();
            List<logSatis> alislar = new List<logSatis>();
            alislar = db.logSatis.Where(x => x.AliciId == kullaniciId).ToList();
            foreach (var item in alislar)
            {
                logSatisInfo logSatisInfo = new logSatisInfo()
                {
                    Tarih = Convert.ToDateTime(item.Tarih),
                    Urun = item.Urun,
                    Fiyat = Convert.ToInt32(item.Fiyat),
                    Miktar = Convert.ToInt32(item.Miktar),
                    Alici = db.kullanici.FirstOrDefault(x => x.k_id == item.AliciId).kad,
                    Satici = db.kullanici.FirstOrDefault(x => x.k_id == item.SaticiId).kad,
                };
                alisInfos.Add(logSatisInfo);
            }
            return alisInfos;
        }

    }
}
