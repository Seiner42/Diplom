using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace DiplomV01
{
    public partial class mainWindow : Form
    {
        OpenFileDialog openFile = new OpenFileDialog();
        String line = "";
        List<dpmClass> dpmList = new List<dpmClass>();//массив машин
        List<bufClass> bufList = new List<bufClass>();//массив буферов
        List<pereborMod> pereborList = new List<pereborMod>();//массив переборов вариантов агрегации
        GraphPane pane;

        public mainWindow()
        {
            InitializeComponent();
        }

        private void mainWindow_Load(object sender, EventArgs e)
        {
            openFile.Filter = "Текстовые файлы| *.txt";
            comboBox1.Items.Add("Перебор");//добавление вариантов агрегации
            //инициализация графика
            pane = zedGraphControl1.GraphPane;//панель рисования(холст)
            pane.XAxis.Type = AxisType.Linear;//тип осей
            pane.YAxis.Type = AxisType.Linear;
            pane.BarSettings.Base = BarBase.Y;//откуда строим
            pane.Title.Text = "Моделирование";//названия графика 
            pane.YAxis.Title.Text = "Машины обработки данных";//названия осей
            pane.XAxis.Title.Text = "Модельное время";
            //насторйка графика
            zedGraphControl1.IsShowContextMenu = false;//отключение контекстного меню (ПКМ)
            zedGraphControl1.ZoomButtons = MouseButtons.None;//отключить приближение на клавиши мыши
            zedGraphControl1.PanButtons = MouseButtons.Left;//назначить перемещение графика на ЛКМ
            zedGraphControl1.PanModifierKeys = Keys.None;//отключить зажатие ctrl для перемещения
            //настройка стандартного масштаба
            pane.XAxis.Scale.Min = 0;//По оси Х от 0,
            pane.XAxis.Scale.Max = 20;// до 25
            pane.YAxis.Scale.Min = 0; //по оси У от 0.
            //настройк шага рисок по осям
            pane.XAxis.Scale.MajorStep = 1.0;
            pane.XAxis.Scale.MinorStep = 1.0;
            pane.YAxis.Scale.MajorStep = 1.0;
            pane.YAxis.Scale.MinorStep = 1.0;
            pane.XAxis.MajorGrid.IsVisible = true;//включить фоновую по оси Х 
            pane.YAxis.MajorGrid.IsVisible = true;//включить фоновуюп по оси У
            pane.XAxis.MajorGrid.IsZeroLine = true;//нулевая линия
            zedGraphControl1.GraphPane.BarSettings.Type = BarType.Overlay;//строить графики на одной оси


            /*

            //тестовый сегмент, потом удалю
            PointPairList ppl = new PointPairList();//массивы точек
            ppl.Add(1, 1, 2);//добавить точку в массив 1 - точка начала, 1 - ось, 2 = длина
            ppl.Add(1, 2, 2);
            ppl.Add(2, 2, 100);

            PointPairList ppl2 = new PointPairList();
            PointPairList ppl3 = new PointPairList();
            ppl2.Add(2, -1, 5);
            ppl3.Add(5, 1, 10);

            //HiLowBarItem myBar = pane.AddHiLowBar("Max", ppl, Color.Black);
            HiLowBarItem bar = pane.AddHiLowBar("1", ppl, Color.Black);//задаем цвет и название(опционально) каждого массива точек
            bar.Bar.Fill = new Fill(Color.Gray);//работа машины (wait)

            HiLowBarItem bar1 = pane.AddHiLowBar("2", ppl2, Color.Red);
            bar1.Bar.Fill = new Fill(Color.Yellow);//запись в буфер

            HiLowBarItem ba2 = pane.AddHiLowBar("3", ppl3, Color.LightGreen);
            ba2.Bar.Fill = new Fill(Color.FromArgb(146, 208, 80));//чтение из буфера

            TextObj barLabel = new TextObj("1", bar.Points[0].X+(bar.Points[0].Z - bar.Points[0].X)/2, bar.Points[0].Y);//добавить текст в опеределнное место(в середину палки)
            barLabel.FontSpec.Fill = new Fill(Color.Gray);//цвет фона текста
            barLabel.FontSpec.Size = 11;//размер
            barLabel.FontSpec.Border.IsVisible = false;//выделение раниц
            pane.GraphObjList.Add(barLabel);//добавить текст
            bar.Label.IsVisible = true;//ЛЭЙБЛЫ?

            //BarItem.CreateBarLabels(pane, false, "");

            PointPairList tst = new PointPairList();//массив курсива
            tst.Add(2, 1);
            tst.Add(2, -1);
            LineItem myCurve = pane.AddCurve("", tst, Color.Black, SymbolType.None);//добавить курсивную линию
            myCurve.Line.Style = System.Drawing.Drawing2D.DashStyle.Custom;
            myCurve.Line.Width = 2;
            myCurve.Line.DashOn = 5;
            myCurve.Line.DashOff = 5;

            PointPairList tst1 = new PointPairList();
            tst1.Add(5, -1);
            tst1.Add(5, 1);
            LineItem myCurve1 = pane.AddCurve("", tst1, Color.Black, SymbolType.None);
            myCurve1.Line.Style = System.Drawing.Drawing2D.DashStyle.Custom;
            myCurve1.Line.Width = 2;
            myCurve1.Line.DashOn = 5;
            myCurve1.Line.DashOff = 5;

            // Calculate the Axis Scale Ranges
            Graphics g = this.CreateGraphics();//вывод графика
            pane.AxisChange(g);
            g.Dispose();
            //конец текстового сегмента

    */

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    StreamReader sr = new StreamReader(openFile.FileName);
                    int dpmNumb = 0;
                    while (line != null)
                    {
                        line = sr.ReadLine();//считывает строку
                        if (line != null)
                        {
                            string[] words = line.Split(' ');
                            switch (words[0][0])
                            {
                                case 'P':
                                    dpmList.Add(new dpmClass());
                                    for (int i = 0; i < 2; i++)
                                    {
                                        string[] digits = Regex.Split(words[i + 1], @"\D+");
                                        foreach (string value in digits)
                                        {
                                            int number;
                                            if (int.TryParse(value, out number))
                                            {
                                                if (i == 0)
                                                {
                                                    dpmList[dpmNumb].inBuf.Add(Convert.ToInt32(value));
                                                }
                                                else
                                                {
                                                    dpmList[dpmNumb].outBuf.Add(Convert.ToInt32(value));
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case 'S':
                                    if ((words[0][words[0].Length - 2]) == '*')
                                    {
                                        dpmList[dpmNumb].repeatComNum = dpmList[dpmNumb].dpmCommandList.Count;
                                    }
                                    for (int i = 1; i < words.Length; i++)
                                    {
                                        switch (words[i][0])
                                        {
                                            case '(':
                                                string[] digits = Regex.Split(words[i], @"\D+");
                                                dpmList[dpmNumb].dpmCommandList.Add(new dpmClass.commandStruct());
                                                dpmList[dpmNumb].dpmCommandList[dpmList[dpmNumb].dpmCommandList.Count - 1].commandType = "wait";
                                                dpmList[dpmNumb].dpmCommandList[dpmList[dpmNumb].dpmCommandList.Count - 1].waitTime = Convert.ToInt32(digits[1]);
                                                break;
                                            case '[':
                                                dpmList[dpmNumb].dpmCommandList.Add(new dpmClass.commandStruct());
                                                string[] comand = Regex.Replace(Regex.Replace(words[i], @"[,]+", " "), @"[][]+", "").Split(new char[] { ' ' });
                                                switch (comand[0])
                                                {
                                                    case "w":
                                                        dpmList[dpmNumb].dpmCommandList[dpmList[dpmNumb].dpmCommandList.Count - 1].commandType = "write";
                                                        break;
                                                    case "r":
                                                        dpmList[dpmNumb].dpmCommandList[dpmList[dpmNumb].dpmCommandList.Count - 1].commandType = "read";
                                                        break;
                                                }
                                                string[] test = Regex.Split(comand[1], @"\D+");
                                                dpmList[dpmNumb].dpmCommandList[dpmList[dpmNumb].dpmCommandList.Count - 1].destination = Convert.ToInt32(Regex.Split(comand[1], @"\D+")[1])-1;
                                                dpmList[dpmNumb].dpmCommandList[dpmList[dpmNumb].dpmCommandList.Count - 1].dataSize = Convert.ToInt32(comand[2]);
                                                break;
                                        }
                                    }
                                    break;
                                case 'B':
                                    for(int i = 1; i <= words.Length-1; i++)
                                    {
                                        bufList.Add(new bufClass());
                                        bufList[i - 1].bufSize = Convert.ToInt32(words[i]);
                                        for(int j = 0; j < dpmList.Count; j++)
                                        {
                                            for(int k = 0; k < dpmList[j].inBuf.Count; k++)
                                            {
                                                if(dpmList[j].inBuf[k] == i)
                                                {

                                                    bufList[i - 1].output = j;
                                                }
                                            }
                                            for (int k = 0; k < dpmList[j].outBuf.Count; k++)
                                            {
                                                if (dpmList[j].outBuf[k] == i)
                                                {
                                                    bufList[i - 1].input = j;
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case '*':
                                    dpmNumb++;
                                    break;
                            }
                        }
                    }
                    sr.Close();
                }
            }
            catch
            {
                MessageBox.Show("Ошибка исходных данных", "Ошибка",MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void modelPerebor()
        {
            int comNumb = Convert.ToInt32(textBox1.Text);//число коммуникаторов
            int agregNumb = GetStarling(bufList.Count, comNumb);//получаем число разбиений
            int bellNumb = bellNumber(bufList.Count); //получаем число белла
            for (int i = 0; i < agregNumb; i++)
            {
                pereborList.Add(new pereborMod());
                for(int j = 0; j < comNumb; j++)
                {
                    pereborList[i].comList.Add(new pereborMod.comClass());
                }
            }
            int[] nums = new int[bufList.Count];
            for (int i = 0; i < bufList.Count; i++)
            {
                nums[i] = i;
            }
            //IEnumerable<string[][]> resultSets1 = Partitioning.GetAllPartitions(new[] { "a", "b", "c" });
            IEnumerable<int[][]> resultSets = Partitioning.GetAllPartitions(nums);//получаем ВСЕ разбиения
            int[][][] preConvertInts = resultSets.Select(x => x.ToArray()).ToArray();//конвертирует ienumerable в обычный массив
            int indexPerebora = 0;//число переборов
            for(int i = 0;i< preConvertInts.Length; i++)
            {
                if (preConvertInts[i].Length == comNumb)//если разбиваем на нужное коичество групп
                {
                    for(int j = 0; j < comNumb; j++)
                    {
                        for(int k = 0; k < preConvertInts[i][j].Length; k++)
                        {
                            pereborList[indexPerebora].comList[j].comBufList.Add(preConvertInts[i][j][k]);
                        }
                    }
                    indexPerebora++;
                }
            }
            //Выявление подключенных к каждому коммуникатору машин
            for(int i = 0; i < agregNumb; i++)//для каждого варианта агрегации
            {
                for(int j = 0; j < comNumb; j++)//для каждого коммуникатора
                {
                    for(int k = 0; k < pereborList[i].comList[j].comBufList.Count;k++)//каждый коммуникатор реализаует минимум один буфер, обходим все
                    {
                        if(!pereborList[i].comList[j].connectedDPMs.Exists((x) => x == bufList[pereborList[i].comList[j].comBufList[k]].input))
                        {
                            pereborList[i].comList[j].connectedDPMs.Add(bufList[pereborList[i].comList[j].comBufList[k]].input);
                        }
                        if (!pereborList[i].comList[j].connectedDPMs.Exists((x) => x == bufList[pereborList[i].comList[j].comBufList[k]].output))
                        {
                            pereborList[i].comList[j].connectedDPMs.Add(bufList[pereborList[i].comList[j].comBufList[k]].output);
                        }
                    }
                }
            }
            //Инициализция графиков
            List<PointPairList> dpmPointList = new List<PointPairList>();//
            List<PointPairList> bufReadPointList = new List<PointPairList>();// Массивы точек для каждой итерации
            List<PointPairList> bufWritePointList = new List<PointPairList>();//

            for (int i=0;i < agregNumb; i++)//моделирование проводится для каждой возможной агрегации
            {
                //Инициализация массивов точек для каждой возможной агрегации
                dpmPointList.Add(new PointPairList());//
                bufReadPointList.Add(new PointPairList());//Добавляем для каждой итерации свой экземпляр массива
                bufWritePointList.Add(new PointPairList());//
                //сбрасываем общие настройки
                for(int sbs = 0; sbs < dpmList.Count; sbs++)
                {
                    dpmList[sbs].exchangeReady = false;//При старте новой агрегации, данные в машинах сбрасываются
                    dpmList[sbs].currentTime = 0;
                    dpmList[sbs].currentCommand = 0;
                }
                for (int circle = 0; circle < 100; circle++)//цикл в котором происходит моделирование
                {
                    //первый этап, машины выставляют заявки
                    for(int j = 0; j < dpmList.Count; j++)//обход каждой машины
                    {
                        if (dpmList[j].exchangeReady == false)//если машина не выставила заявку на обмен
                        {
                            switch (dpmList[j].dpmCommandList[dpmList[j].currentCommand].commandType)//проверяем тип текущей команды машины
                            {
                                case "wait"://если ожидание
                                    dpmPointList[i].Add(dpmList[j].currentTime, j+1, dpmList[j].dpmCommandList[dpmList[j].currentCommand].waitTime);//добавляем на график
                                    dpmList[j].currentTime += dpmList[j].dpmCommandList[dpmList[j].currentCommand].waitTime;//если инструкция ждать, машина просто ждет
                                    dpmList[j].currentCommand++;//Выполнили команду, переходим к следующей
                                    break;
                                case "write"://если запись
                                case "read"://если чтение
                                    dpmList[j].exchangeReady = true;//заявка на обмен выставлена
                                    break;
                            }
                            //(перенести, чтобы коммуникатор это делал, по заверешению операции)
                            //dpmList[j].currentCommand++;//после  обработки команды ставим указатель на следующую 
                        }
                    }
                    //Этап второй, коммуникаторы обрабатывают заявки
                    for(int k = 0;k< comNumb; k++)//обход каждого коммуникатора
                    {
                        int stp = 0;
                        int min = 0;
                        int minIdx = 0;
                        //опрос коммуникатором машины
                        for (int l = 0;l<pereborList[i].comList[k].connectedDPMs.Count;l++)//каждый коммуникатор проверяет связанные с ним машины
                        {
                            //если машина готова к обмену и хочет использовать именно этот коммуникатор и не заблокирована
                            if ((dpmList[pereborList[i].comList[k].connectedDPMs[l]].exchangeReady == true)&&(pereborList[i].comList[k].comBufList.Contains(dpmList[pereborList[i].comList[k].connectedDPMs[l]].dpmCommandList[dpmList[pereborList[i].comList[k].connectedDPMs[l]].currentCommand].destination))&&(dpmList[pereborList[i].comList[k].connectedDPMs[l]].isBlocked == false))
                            {
                                //ищем минимальную выставленную заявку
                                if (stp == 0)//если это первая готовая машина
                                {
                                    minIdx = pereborList[i].comList[k].connectedDPMs[l];//запоминаем индекс
                                    min = dpmList[pereborList[i].comList[k].connectedDPMs[l]].currentTime;//запоминаем время
                                    stp++;
                                }
                                else
                                {
                                    if (dpmList[pereborList[i].comList[k].connectedDPMs[l]].currentTime<min)
                                    {
                                        minIdx = pereborList[i].comList[k].connectedDPMs[l];//запоминаем индекс
                                        min = dpmList[pereborList[i].comList[k].connectedDPMs[l]].currentTime;//запоминаем время
                                    }
                                }
                                if (stp > 0)//если нашли заявку нужно её отработать
                                {
                                    //отработать удачный обмен и неудачный 
                                    switch (dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].commandType)
                                    {
                                        case "write"://если машина хочет записать информацию, проверяем что в буфере есть свободные ячейки
                                            //вычисляем свободные ячейки в нужно буфере
                                            int freeSpace = bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].bufSize - bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].dataInBuf;
                                            //если число свободных ячеек больше или равен чем объекм передаваемых данных
                                            if (freeSpace >= dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize)
                                            {
                                                //в буфер записались данные
                                                bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].dataInBuf += bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].dataInBuf;
                                                //добавляем на график (curTime/bufNum/dataSize)
                                                bufWritePointList[i].Add(dpmList[minIdx].currentTime, -dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination - 1, dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize);
                                                //время машины сдвигаем
                                                dpmList[minIdx].currentTime += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                                //переключаем команду дпм
                                                dpmList[minIdx].currentCommand++;

                                                //проверям можно ли разблокировать какую-то машину
                                                if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum != -1)
                                                {
                                                    //разблокируем машина
                                                    dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].isBlocked = false;
                                                    //нужно продвинуть время машины
                                                    //Новое текущее время заблоченной машины = curTime разлокрирубщей - curTime заблокированной
                                                    dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime += (dpmList[minIdx].currentTime - dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime);
                                                    //очищаем номер
                                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum = -1;
                                                }
                                            }
                                            else//неудачная попытка обмена
                                            {
                                                //блокируем машину
                                                dpmList[minIdx].isBlocked = true;
                                                //записываем номер заблокированной машины
                                                bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum = minIdx;
                                            }
                                            break;
                                        case "read"://если машина хочет считать информацию, проверяем что информация есть в нужном буфере
                                            //если число данных в буфере больше или равно объема принимаемых данных
                                            if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].dataInBuf >= dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize)
                                            {
                                                //в буфере удаляются данные
                                                bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].dataInBuf -= bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].dataInBuf;
                                                //добавляем на график (curTime/bufNum/dataSize)
                                                bufReadPointList[i].Add(dpmList[minIdx].currentTime, -dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination - 1, dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize);
                                                //время машины сдвигаем
                                                dpmList[minIdx].currentTime += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                                //переключаем команду дпм
                                                dpmList[minIdx].currentCommand++;

                                                //проверям можно ли разблокировать какую-то машину
                                                if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum != -1)
                                                {
                                                    //разблокируем машина
                                                    dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].isBlocked = false;
                                                    //нужно продвинуть время машины
                                                    //Новое текущее время заблоченной машины = curTime разлокрирубщей - curTime заблокированной
                                                    dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime += (dpmList[minIdx].currentTime - dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime);
                                                    //очищаем номер
                                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum = -1;
                                                }
                                            }
                                            else//неудачная попытка обмена
                                            {
                                                //блокируем машину
                                                dpmList[minIdx].isBlocked = true;
                                                //записываем номер заблокированной машины
                                                bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum = minIdx;
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    //requests();//создание заявок, заполнение дескрипторов
                    //reqproc();//поиск заявки с которой будет работаеть коммуникатор
                    //exchange();//обмен
                }
                break;//для одной агрегации
            }
            //РИСОВАНИЕ
            HiLowBarItem bar = pane.AddHiLowBar("1", dpmPointList[0], Color.Black);//задаем цвет и название(опционально) каждого массива точек
            bar.Bar.Fill = new Fill(Color.Gray);//работа машины (wait)

            HiLowBarItem bar1 = pane.AddHiLowBar("2", bufWritePointList[0], Color.Red);
            bar1.Bar.Fill = new Fill(Color.Yellow);//запись в буфер

            HiLowBarItem ba2 = pane.AddHiLowBar("3", bufReadPointList[0], Color.LightGreen);
            ba2.Bar.Fill = new Fill(Color.FromArgb(146, 208, 80));//чтение из буфера
            Graphics g = this.CreateGraphics();//вывод графика
            pane.AxisChange(g);
            g.Dispose();
        }



        public static int bellNumber(int n)//считает число белла
        {
            int[,] bell = new int[n + 1,
                                  n + 1];
            bell[0, 0] = 1;

            for (int i = 1; i <= n; i++)
            {

                // Explicitly fill for j = 0 
                bell[i, 0] = bell[i - 1, i - 1];

                // Fill for remaining values of j 
                for (int j = 1; j <= i; j++)
                    bell[i, j] = bell[i - 1, j - 1] +
                                 bell[i, j - 1];
            }

            return bell[n, 0];
        }


        public static class Partitioning//возвращает все возможные переборы
        {
            public static IEnumerable<T[][]> GetAllPartitions<T>(T[] elements)
            {
                return GetAllPartitions(new T[][] { }, elements);
            }

            private static IEnumerable<T[][]> GetAllPartitions<T>(
                T[][] fixedParts, T[] suffixElements)
            {
                // A trivial partition consists of the fixed parts
                // followed by all suffix elements as one block
                yield return fixedParts.Concat(new[] { suffixElements }).ToArray();

                // Get all two-group-partitions of the suffix elements
                // and sub-divide them recursively
                var suffixPartitions = GetTuplePartitions(suffixElements);
                foreach (Tuple<T[], T[]> suffixPartition in suffixPartitions)
                {
                    var subPartitions = GetAllPartitions(
                        fixedParts.Concat(new[] { suffixPartition.Item1 }).ToArray(),
                        suffixPartition.Item2);
                    foreach (var subPartition in subPartitions)
                    {
                        yield return subPartition;
                    }
                }
            }

            private static IEnumerable<Tuple<T[], T[]>> GetTuplePartitions<T>(
                T[] elements)
            {
                // No result if less than 2 elements
                if (elements.Length < 2) yield break;

                // Generate all 2-part partitions
                for (int pattern = 1; pattern < 1 << (elements.Length - 1); pattern++)
                {
                    // Create the two result sets and
                    // assign the first element to the first set
                    List<T>[] resultSets = {
                    new List<T> { elements[0] }, new List<T>() };
                    // Distribute the remaining elements
                    for (int index = 1; index < elements.Length; index++)
                    {
                        resultSets[(pattern >> (index - 1)) & 1].Add(elements[index]);
                    }

                    yield return Tuple.Create(
                        resultSets[0].ToArray(), resultSets[1].ToArray());
                }
            }
        }



        public static int GetStarling(int n, int k)//возвращает число стирлинга
        {
            if (n == k)
                return 1;  // S(n,n) = 1
            if (n <= 0 || k <= 0 || n < k)
                return 0;  // S(n, 0) = 0; S(0, k) = 0
            return GetStarling(n - 1, k - 1) + k * GetStarling(n - 1, k);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dpmList.Count < 1)
            {
                MessageBox.Show("Данные не введены", "Ошибка",MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (comboBox1.SelectedIndex == -1)
                {
                    MessageBox.Show("Выберите вариант агрегации", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    switch (comboBox1.SelectedItem)
                    {
                        case "Перебор":
                            if (textBox1.TextLength == 0)
                            {
                                MessageBox.Show("Введите кол-во коммуникаторов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                break;
                            }
                            else
                            {
                                modelPerebor();
                                break;
                            }
                    }
                }
            }
        }
        private void zedGraphControl1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= '0') && (e.KeyChar <= '9'))
            {
                // цифра
                return;
            }

            if (e.KeyChar == '.')
            {
                // точку заменим запятой
                e.KeyChar = ',';
            }

            if (e.KeyChar == ',')
            {
                if (textBox1.Text.IndexOf(',') != -1)
                {
                    // запятая уже есть в поле редактирования
                    e.Handled = true;
                }
                return;
            }

            if (Char.IsControl(e.KeyChar))
            {
                // <Enter>, <Backspace>, <Esc>
                if (e.KeyChar == (char)Keys.Enter)
                    // нажата клавиша <Enter>
                    // установить курсор на кнопку OK
                    button3.Focus();
                return;
            }

            // остальные символы запрещены
            e.Handled = true;
        }
    }
}
