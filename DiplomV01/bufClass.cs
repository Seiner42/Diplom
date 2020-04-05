using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomV01
{
    class bufClass
    {
        public int bufSize;//размер буфера
        public int dataInBuf = 0;//занятые ячейки
        public int input;//номер машины которая записывает в буфер
        public int output;//номер машины которая читает из буфера
        public int blockedDPMnum = -1;//номер заблокированной машины
        public int lastDataChange = 0;//время последнего появления данных
    }
}
