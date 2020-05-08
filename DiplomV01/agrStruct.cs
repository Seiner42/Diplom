using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomV01
{
    class agrStruct
    {
        public class comClass//класс описывающий коммуникатор
        {
            public List<int> comBufList = new List<int>();//буферы реализуемые коммуникатором
            public List<int> connectedDPMs = new List<int>();//машины подключенные к коммуникатору
        }
        public List<comClass> comList = new List<comClass>();//список коммуникаторов каждой итерации
        public int exchangeCount;//число обменов
    }
}
