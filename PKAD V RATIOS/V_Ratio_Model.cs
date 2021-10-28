using System;
using System.Collections.Generic;
using System.Text;

namespace PKAD_V_RATIOS
{
    public class V_Ratio_Model
    {
        public int pallet { get; set; }
        public string box { get; set; }
        public string batch { get; set; }
        public int total_ballots { get; set; }
        public int trump { get; set; }
        public int biden { get; set; }
        public int jorgenson { get; set; }
        public int writeInOverUnder { get; set; }
        public string ballottype { get; set; }
        public double percent { get; set; }

    }
}
