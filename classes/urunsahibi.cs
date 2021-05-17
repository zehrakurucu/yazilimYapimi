using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace yazilimYapimi.classes
{
    public class urunsahibi
    {
        public int urun_id { get; set; }
        public int k_id { get; set; }
        public string kullanici { get; set; }
        public int miktar { get; set; }
        public int fiyat { get; set; }
        public string urun { get; set; }
    }
}