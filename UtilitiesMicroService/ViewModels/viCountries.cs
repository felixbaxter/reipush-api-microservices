using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UtilitiesMicroService.ViewModels
{
    public class viCountries
    {
        public int CountryId { get; set; }
        public string Name { get; set; }
        public string TwoLetterISOCode { get; set; }
        public string ThreeLetterISOCode { get; set; }
        public int NumericISOCode { get; set; }

    }
}
