using System;
namespace atm.Models
{

    public class AtmItem
    {
        public int id { get; set; }

        //ATM module for flowDemo
         public string type { get; set; }
         public double x { get; set; }
         public double y { get; set; }
         public DateTime timeStamp { get; set; }
         public string scopeId { get; set; }


        //ATM module for ATM game
        /*
        public int Wrongtimes { get; set; }
        public string Tasktype { get; set; }
        public string Finishtime { get; set; }
        public DateTime timeStamp { get; set; }*/
    }
}
