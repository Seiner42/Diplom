using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomV01
{
    class nedogruzMod
    {
        public List<int> comBufList = new List<int>();//буферы реализуемые коммуникатором
        public List<int> connectedDPMs = new List<int>();//машины подключенные к данному коммуникатору
        public int performance = 0;//кол-во обменов совершеннных коммуникатором
    }
}
