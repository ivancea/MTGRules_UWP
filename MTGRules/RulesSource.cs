using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTGRules {
    class RulesSource {
        public string FileName { get; set; }
        public Uri Uri { get; set; }
        public DateTime Date { get; set; }
        public Encoding Encoding { get; set; }

        public RulesSource(string fileName, Uri uri, DateTime date, Encoding encoding) {
            FileName = fileName;
            Uri = uri;
            Date = date;
            Encoding = encoding;
        }
    }
}
