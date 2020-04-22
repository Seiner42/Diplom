using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomV01
{
    class dpmClass
    {
        public class commandStruct
        {
            public string commandType;//тип команды
            public int waitTime;//время ожидания
            public int dataSize;//размер данных
            public int destination;//в какой буфер положить/считать
        }
        public List<int> inBuf = new List<int>();//список буферов входящих в машину
        public List<int> outBuf = new List<int>();//список буферов выходящих из машины
        public List<commandStruct> dpmCommandList = new List<commandStruct>();//список команд каждой машны
        public int repeatComNum;//номер команды, с которой начинается повтор
        public int currentCommand = 0;//текущая выполняемая команда
        public bool exchangeReady = false;//флаг готовности совершить обмен
        public int currentTime = 0;//текущее модельное время
        public bool isBlocked = false;//флаг заблокирована ли машина или нет
    }
}
