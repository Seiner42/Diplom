using MoreLinq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace DiplomV01
{
    public partial class mainWindow : Form
    {
        OpenFileDialog openFile = new OpenFileDialog();//считывание файла
        List<dpmClass> dpmList = new List<dpmClass>();//массив машин
        List<bufClass> bufList = new List<bufClass>();//массив буферов
        GraphPane pane;

        public mainWindow()
        {
            InitializeComponent();
        }

        private void mainWindow_Load(object sender, EventArgs e)
        {
            openFile.Filter = "Текстовые файлы| *.txt";
            comboBox1.Items.Add("Перебор");//добавление вариантов агрегации
            comboBox1.Items.Add("Удалять недогруженные");
            comboBox1.Items.Add("sizeOfData");
            comboBox1.Items.Add("connections");
            comboBox1.Items.Add("parallel");
            comboBox1.Items.Add("dpmcenter");

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
            ppl.Add(10, 1, 20);//добавить точку в массив 1 - точка начала, 1 - ось, 2 = длина
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

        private void button1_Click(object sender, EventArgs e)//нажатие на ввод данных
        {
            try
            {
                if (openFile.ShowDialog() == DialogResult.OK)//если файл открывается
                {
                    String line = "";
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
                                                dpmList[dpmNumb].dpmCommandList[dpmList[dpmNumb].dpmCommandList.Count - 1].destination = Convert.ToInt32(Regex.Split(comand[1], @"\D+")[1]) - 1;
                                                dpmList[dpmNumb].dpmCommandList[dpmList[dpmNumb].dpmCommandList.Count - 1].dataSize = Convert.ToInt32(comand[2]);
                                                break;
                                        }
                                    }
                                    break;
                                case 'B':
                                    for (int i = 1; i <= words.Length - 1; i++)
                                    {
                                        bufList.Add(new bufClass());
                                        bufList[i - 1].bufSize = Convert.ToInt32(words[i]);
                                        for (int j = 0; j < dpmList.Count; j++)
                                        {
                                            for (int k = 0; k < dpmList[j].inBuf.Count; k++)
                                            {
                                                if (dpmList[j].inBuf[k] == i)
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
                    sr.Close();//закрыть файл
                    button1.BackColor = Color.LightGreen;//данные ввдены, цвет кнопки сигнализирует об этом
                }
            }
            catch
            {
                MessageBox.Show("Ошибка исходных данных", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dpmconnections()
        {
            clearData();
            //считаем
            //число связей между машинами
            List<DPMConnectionClass> connetctionList = new List<DPMConnectionClass>();//список связанных с каждой машиной машин
            for(int i = 0; i < dpmList.Count; i++)//для каждой машины
            {
                for(int j = 0; j < dpmList[i].inBuf.Count;j++)//для каждого входящего буфера
                {
                    //провреять В и список буферов
                    int idxB = bufList[(dpmList[i].inBuf[j] - 1)].input;//индекс второй машины покдлюченной к буферу
                    int idxcn = -1;
                    bool exists = false;//есть ли такая же связь
                    for(int m = 0; m < connetctionList.Count; m++)//проверка всех связей
                    {
                        //ищем, есть ли уже такая связь
                        //если А = i и B = idxB OR A = idxB и В = i
                        //A = i && b = idxB
                        bool us1 = connetctionList[m].DPM_A == i;
                        bool us2 = connetctionList[m].DPM_B == idxB;
                        //||
                        //A = idxB && B = i
                        bool us3 = connetctionList[m].DPM_A == idxB;
                        bool us4 = connetctionList[m].DPM_B == i;
                        if ((us1&&us2)||(us3&&us4))
                        {
                            exists = true;//если связь найдена
                            idxcn = m;//номер связи
                        }

                    }
                    //если есть такая связь то, проверить наличине буфера в связи
                    if (exists)
                    {
                        if (!(connetctionList[idxcn].buffers.Contains((dpmList[i].inBuf[j] - 1))))
                        {
                            connetctionList[idxcn].buffers.Add((dpmList[i].inBuf[j] - 1));//добавялем буфер, если связь есть, но буфер в ней нет.
                        }
                    }
                    //если связи нет, добавление и редактирование
                    if (exists == false)
                    {
                        //добавление связи
                        connetctionList.Add(new DPMConnectionClass());
                        connetctionList[connetctionList.IndexOf(connetctionList.Last())].DPM_A = i;//номер текущей машины
                        connetctionList[connetctionList.IndexOf(connetctionList.Last())].DPM_B = idxB;//номер второй машины
                        connetctionList[connetctionList.IndexOf(connetctionList.Last())].buffers.Add((dpmList[i].inBuf[j] - 1));//буфер
                    }
                }
                for (int k = 0; k < dpmList[i].outBuf.Count; k++)//для каждого выходящего буфера
                {
                    //провреять В и список буферов
                    int idxB = bufList[(dpmList[i].outBuf[k] - 1)].output;//индекс второй машины покдлюченной к буферу
                    int idxcn = -1;
                    bool exists = false;//есть ли такая же связь
                    for (int m = 0; m < connetctionList.Count; m++)//проверка всех связей
                    {
                        //ищем, есть ли уже такая связь
                        //если А = i и B = idxB OR A = idxB и В = i
                        //A = i && b = idxB
                        bool us1 = connetctionList[m].DPM_A == i;
                        bool us2 = connetctionList[m].DPM_B == idxB;
                        //||
                        //A = idxB && B = i
                        bool us3 = connetctionList[m].DPM_A == idxB;
                        bool us4 = connetctionList[m].DPM_B == i;
                        if ((us1 && us2) || (us3 && us4))
                        {
                            exists = true;//если связь найдена
                            idxcn = m;//номер связи
                        }

                    }
                    //если есть такая связь то, проверить наличине буфера в связи
                    if (exists)
                    {
                        if (!(connetctionList[idxcn].buffers.Contains((dpmList[i].outBuf[k] - 1))))
                        {
                            connetctionList[idxcn].buffers.Add((dpmList[i].outBuf[k] - 1));//добавялем буфер, если связь есть, но буфер в ней нет.
                        }
                    }
                    //если связи нет, добавление и редактирование
                    if (exists == false)
                    {
                        //добавление связи
                        connetctionList.Add(new DPMConnectionClass());
                        connetctionList[connetctionList.IndexOf(connetctionList.Last())].DPM_A = idxB;//номер текущей машины
                        connetctionList[connetctionList.IndexOf(connetctionList.Last())].DPM_B = i;//номер второй машины
                        connetctionList[connetctionList.IndexOf(connetctionList.Last())].buffers.Add((dpmList[i].outBuf[k] - 1));//буфер
                    }
                }
            }
            //получив список связей
            //3 варианта, либо буферов больше, чем связей, либо меньше, либо равно
            int comCount = Convert.ToInt32(textBox1.Text);//кол-во коммуникаторов
            int menu = -1;
            if (comCount == connetctionList.Count)
            {
                menu = 1;
            }
            if (comCount > connetctionList.Count)
            {
                menu = 2;
            }
            if (comCount < connetctionList.Count)
            {
                menu = 3;
            }
            List<nedogruzMod> comList = new List<nedogruzMod>();//список коммуникаторов
            switch (menu)
            {
                case 1://если предпологаемое число коммуникаторов = число связей между машинами
                       //каждый коммуникатор реализует одну связь
                    for (int i = 0; i < comCount; i++)//каждый коммуникатор
                    {
                        comList.Add(new nedogruzMod());//создаем коммуникатор
                        //добавляем все буферы текущей соединялки
                        //добавляем в коммуникатор буферы
                        for (int j = 0; j < connetctionList[i].buffers.Count; j++)
                        {
                            comList[i].comBufList.Add(connetctionList[i].buffers[j]);//распределили буферы по коммуникаторам
                        }
                    }
                    //добавляем машины
                    for (int j = 0; j < comList.Count; j++)//для каждого коммуникатора
                    {
                        for (int k = 0; k < comList[j].comBufList.Count; k++)//каждый коммуникатор реализаует минимум один буфер, обходим все
                        {
                            if (!comList[j].connectedDPMs.Exists((x) => x == bufList[comList[j].comBufList[k]].input))
                            {
                                comList[j].connectedDPMs.Add(bufList[comList[j].comBufList[k]].input);
                            }
                            if (!comList[j].connectedDPMs.Exists((x) => x == bufList[comList[j].comBufList[k]].output))
                            {
                                comList[j].connectedDPMs.Add(bufList[comList[j].comBufList[k]].output);
                            }
                        }
                    }
                    break;
                case 2://если коммуникаторов больше, чем связей
                    //если коммуникаторов больше, чем связей, нужно разбивать самые нагруженнные связи
                    //считаем процент нагрузки каждой связи
                    //подсчет рабочих циклов
                    List<bool> isEnded = new List<bool>();//прошла ли машина все свои команды
                    List<int> isEndedCount = new List<int>();//сколько рабочих циклов каждой машины
                    for (int i = 0; i < dpmList.Count; i++)
                    {
                        isEnded.Add(false);
                        isEndedCount.Add(0);
                    }
                    bool cycleOver = false;//
                                           //вычисляем рабочий цикл
                    while (cycleOver != true)//пока все машины не отработают свой рабочй цикл
                    {
                        //моделирование
                        for (int i = 0; i < dpmList.Count; i++)//Обход каждой машины
                        {
                            //проверка на текущую команду
                            if (dpmList[i].currentCommand >= dpmList[i].dpmCommandList.Count)
                            {//если машина выполнила все свои инструкции начинаем с *
                                dpmList[i].currentCommand = dpmList[i].repeatComNum;
                                //фиксируем конец машины
                                isEnded[i] = true;
                                isEndedCount[i]++;
                            }
                            //выполнение команды
                            //определить тип команды
                            switch (dpmList[i].dpmCommandList[dpmList[i].currentCommand].commandType)//
                            {
                                case "write"://если запись
                                             //удачно
                                             //проверяем свободное место
                                    if ((bufList[dpmList[i].dpmCommandList[dpmList[i].currentCommand].destination].bufSize - bufList[dpmList[i].dpmCommandList[dpmList[i].currentCommand].destination].dataInBuf) >= dpmList[i].dpmCommandList[dpmList[i].currentCommand].dataSize)
                                    {//добавляем данные
                                        bufList[dpmList[i].dpmCommandList[dpmList[i].currentCommand].destination].dataInBuf += dpmList[i].dpmCommandList[dpmList[i].currentCommand].dataSize;
                                        dpmList[i].currentCommand++;
                                    }
                                    //неудачно
                                    break;
                                case "read"://если чтение
                                            //удачно
                                            //проверяем наличие данных
                                    if (bufList[dpmList[i].dpmCommandList[dpmList[i].currentCommand].destination].dataInBuf >= dpmList[i].dpmCommandList[dpmList[i].currentCommand].dataSize)
                                    {//удаляем их
                                        bufList[dpmList[i].dpmCommandList[dpmList[i].currentCommand].destination].dataInBuf -= dpmList[i].dpmCommandList[dpmList[i].currentCommand].dataSize;
                                        dpmList[i].currentCommand++;
                                    }
                                    //неудачно
                                    break;
                                case "wait"://если ждать
                                            //переход к следующей команде
                                    dpmList[i].currentCommand++;
                                    break;
                            }
                        }
                        //проверка на завершение
                        int cnt = 0;
                        for (int i = 0; i < dpmList.Count; i++)
                        {
                            if (isEnded[i] == true)
                            {
                                cnt++;
                            }
                        }//если все машины выполнили свою программу хотябы 1 раз
                        if (cnt == dpmList.Count)
                        {
                            cycleOver = true;
                        }
                    }
                    //считаем объем 
                    List<int> bufComCount = new List<int>();//кол-во обращений к каждому буферу
                    for (int i = 0; i < bufList.Count; i++)
                    {
                        bufComCount.Add(0);
                    }
                    int comandsNumber = 0;//общее количество команд
                                          //нужно посчитать сколько обращений к каждому буферу, учитывая рабочие циклы
                    for (int i = 0; i < dpmList.Count; i++)//для каждой машины
                    {
                        for (int j = 0; j < isEndedCount[i]; j++)//для каждого рабочего цикла
                        {
                            for (int k = 0; k < dpmList[i].dpmCommandList.Count; k++)//обходим список команд
                            {
                                if ((dpmList[i].dpmCommandList[k].commandType == "write") || (dpmList[i].dpmCommandList[k].commandType == "read"))
                                {
                                    comandsNumber++;//общее число команд
                                    bufComCount[dpmList[i].dpmCommandList[k].destination]++;//число обращений к конкретному буферу
                                }
                            }
                        }
                    }
                    //теперь посчитать, сколько в процентном соотношении обращений к каждому буферу
                    List<int> percentList = new List<int>();
                    for (int i = 0; i < bufComCount.Count; i++)
                    {
                        percentList.Add(GetPercent(bufComCount[i], comandsNumber));
                    }

                    for (int l = 0; l < connetctionList.Count; l++)
                    {//каждая связь
                        //обходим буферы каждой связи
                        for(int n = 0; n < connetctionList[l].buffers.Count; n++)
                        {
                            connetctionList[l].impPercent += percentList[connetctionList[l].buffers[n]];

                        }
                    }
                    //вычисление чила "лишних" коммуникаторов = n
                    int cm = comCount - connetctionList.Count;
                    //лишние используем чтобы пополам понизить нагрузку на самые нагруженные связи
                    List<int> percents = new List<int>();
                    for(int s = 0; s < connetctionList.Count; s++)
                    {
                        percents.Add(connetctionList[s].impPercent);//списк нагрузки связей
                    }
                    //распрежедение лишних коммуникаторов 
                    //находим самую нагруженную
                    for(int s = 0; s < cm; s++)//обход лишних коммуникаторов
                    {
                        int idxofMaxConnection = percents.IndexOf(percents.Max());//индекс самой нагруженной связи
                        int bcount = connetctionList[idxofMaxConnection].buffers.Count;//кол-во буферов в выбранной связи
                        int del = bcount / 2;//сколько буферов забрать
                        comList.Add(new nedogruzMod());//создали com
                        for (int z = 0; z < del; z++)//заполняем коммуникатор буферами
                        {
                            //поиск самого "тяжелого" буфера
                            int idxMax = 0;
                            int mx = 0;
                            for (int o = 0; o < bcount; o++)//обходим каждый буфер
                            {
                                if (percentList[connetctionList[idxofMaxConnection].buffers[o]] > mx)
                                {//нашли самый нагруженный буфер
                                    mx = percentList[connetctionList[idxofMaxConnection].buffers[o]];
                                    idxMax = connetctionList[idxofMaxConnection].buffers[o];
                                }
                            }
                            //самый нагруженный буфер добавляем в коммуникатор
                            comList[s].comBufList.Add(idxMax);
                            //и обнуляем его нагрузку
                            percents[idxofMaxConnection] -= percentList[idxMax];//уменьшили нагружку на связь
                            percentList[idxMax] = -1;
                            //удаляем из связи
                            connetctionList[idxofMaxConnection].buffers.Remove(idxMax);
                            //теперь удалить из percents проценты
                        }
                    }
                    //распределение остальных коммуникаторов
                    for (int i = cm; i < comCount; i++)//каждый коммуникатор
                    {
                        comList.Add(new nedogruzMod());//создаем коммуникатор
                        //добавляем все буферы текущей соединялки
                        //добавляем в коммуникатор буферы
                        for (int j = 0; j < connetctionList[i-cm].buffers.Count; j++)//-1
                        {
                            comList[i].comBufList.Add(connetctionList[i-cm].buffers[j]);//распределили буферы по коммуникаторам
                        }
                    }
                    //добавляем машины
                    for (int j = 0; j < comList.Count; j++)//для каждого коммуникатора
                    {
                        for (int k = 0; k < comList[j].comBufList.Count; k++)//каждый коммуникатор реализаует минимум один буфер, обходим все
                        {
                            if (!comList[j].connectedDPMs.Exists((x) => x == bufList[comList[j].comBufList[k]].input))
                            {
                                comList[j].connectedDPMs.Add(bufList[comList[j].comBufList[k]].input);
                            }
                            if (!comList[j].connectedDPMs.Exists((x) => x == bufList[comList[j].comBufList[k]].output))
                            {
                                comList[j].connectedDPMs.Add(bufList[comList[j].comBufList[k]].output);
                            }
                        }
                    }
                    for (int s = 0; s < comList.Count; s++)
                    {
                        comList[s].connectedDPMs.Sort();
                        comList[s].comBufList.Sort();
                    }
                    break;
                case 3://если коммуникаторов меньше, чем связей
                       //если коммуникаторов меньше, значит нужно определить % нагрузки каждой связи от общего и распределить равномерно
                    List<bool> isEnded1 = new List<bool>();//прошла ли машина все свои команды
                    List<int> isEndedCount1 = new List<int>();//сколько рабочих циклов каждой машины
                    for (int i = 0; i < dpmList.Count; i++)
                    {
                        isEnded1.Add(false);
                        isEndedCount1.Add(0);
                    }
                    bool cycleOver1 = false;//
                                           //вычисляем рабочий цикл
                    while (cycleOver1 != true)//пока все машины не отработают свой рабочй цикл
                    {
                        //моделирование
                        for (int i = 0; i < dpmList.Count; i++)//Обход каждой машины
                        {
                            //проверка на текущую команду
                            if (dpmList[i].currentCommand >= dpmList[i].dpmCommandList.Count)
                            {//если машина выполнила все свои инструкции начинаем с *
                                dpmList[i].currentCommand = dpmList[i].repeatComNum;
                                //фиксируем конец машины
                                isEnded1[i] = true;
                                isEndedCount1[i]++;
                            }
                            //выполнение команды
                            //определить тип команды
                            switch (dpmList[i].dpmCommandList[dpmList[i].currentCommand].commandType)//
                            {
                                case "write"://если запись
                                             //удачно
                                             //проверяем свободное место
                                    if ((bufList[dpmList[i].dpmCommandList[dpmList[i].currentCommand].destination].bufSize - bufList[dpmList[i].dpmCommandList[dpmList[i].currentCommand].destination].dataInBuf) >= dpmList[i].dpmCommandList[dpmList[i].currentCommand].dataSize)
                                    {//добавляем данные
                                        bufList[dpmList[i].dpmCommandList[dpmList[i].currentCommand].destination].dataInBuf += dpmList[i].dpmCommandList[dpmList[i].currentCommand].dataSize;
                                        dpmList[i].currentCommand++;
                                    }
                                    //неудачно
                                    break;
                                case "read"://если чтение
                                            //удачно
                                            //проверяем наличие данных
                                    if (bufList[dpmList[i].dpmCommandList[dpmList[i].currentCommand].destination].dataInBuf >= dpmList[i].dpmCommandList[dpmList[i].currentCommand].dataSize)
                                    {//удаляем их
                                        bufList[dpmList[i].dpmCommandList[dpmList[i].currentCommand].destination].dataInBuf -= dpmList[i].dpmCommandList[dpmList[i].currentCommand].dataSize;
                                        dpmList[i].currentCommand++;
                                    }
                                    //неудачно
                                    break;
                                case "wait"://если ждать
                                            //переход к следующей команде
                                    dpmList[i].currentCommand++;
                                    break;
                            }
                        }
                        //проверка на завершение
                        int cnt = 0;
                        for (int i = 0; i < dpmList.Count; i++)
                        {
                            if (isEnded1[i] == true)
                            {
                                cnt++;
                            }
                        }//если все машины выполнили свою программу хотябы 1 раз
                        if (cnt == dpmList.Count)
                        {
                            cycleOver1 = true;
                        }
                    }
                    //считаем объем 
                    List<int> bufComCount1 = new List<int>();//кол-во обращений к каждому буферу
                    for (int i = 0; i < bufList.Count; i++)
                    {
                        bufComCount1.Add(0);
                    }
                    int comandsNumber1 = 0;//общее количество команд
                                          //нужно посчитать сколько обращений к каждому буферу, учитывая рабочие циклы
                    for (int i = 0; i < dpmList.Count; i++)//для каждой машины
                    {
                        for (int j = 0; j < isEndedCount1[i]; j++)//для каждого рабочего цикла
                        {
                            for (int k = 0; k < dpmList[i].dpmCommandList.Count; k++)//обходим список команд
                            {
                                if ((dpmList[i].dpmCommandList[k].commandType == "write") || (dpmList[i].dpmCommandList[k].commandType == "read"))
                                {
                                    comandsNumber1++;//общее число команд
                                    bufComCount1[dpmList[i].dpmCommandList[k].destination]++;//число обращений к конкретному буферу
                                }
                            }
                        }
                    }
                    //теперь посчитать, сколько в процентном соотношении обращений к каждому буферу
                    List<int> percentList1 = new List<int>();
                    for (int i = 0; i < bufComCount1.Count; i++)
                    {
                        percentList1.Add(GetPercent(bufComCount1[i], comandsNumber1));
                    }

                    for (int l = 0; l < connetctionList.Count; l++)
                    {//каждая связь
                        //обходим буферы каждой связи
                        for (int n = 0; n < connetctionList[l].buffers.Count; n++)
                        {
                            connetctionList[l].impPercent += percentList1[connetctionList[l].buffers[n]];

                        }
                    }
                    //нашли % нагназки каждой связи
                    //распределяем равномерно по коммуникаторам
                    List<int> percents1 = new List<int>();
                    for (int s = 0; s < connetctionList.Count; s++)
                    {
                        percents1.Add(connetctionList[s].impPercent);//списк нагрузки связей
                    }

                    int[] numbers = percents1.ToArray();
                    int pilesNumber = comCount;
                    int sum = numbers.Sum();/*
                    int combNumber = numbers.Aggregate(1, (m, number) => m * pilesNumber);
                    if (combNumber < 0)
                    {
                        combNumber = combNumber * -1;//pgrll
                    }
                    var perfectCombination =
                        Enumerable.Range(0, combNumber)
                                  .Select(x =>
                                  {
                                      var piles =
                                          Enumerable.Range(0, pilesNumber)
                                                    .Select(y => new List<int>(numbers.Length))
                                                    .ToArray();
                                      foreach (var n in numbers)
                                      {
                                          piles[x % pilesNumber].Add(n);
                                          x /= pilesNumber;
                                      }
                                      return piles;
                                  })
                                  .MinBy(piles => piles.Sum(pile => Math.Abs(pile.Sum() * pilesNumber - sum)));*/
                    List<List<int>> agreg = new List<List<int>>();//вариант агрегации итоговый
                    List<int> comPercents = new List<int>();//проценты нагруузки каждого коммуникатора
                    for (int cmcnt = 0; cmcnt < comCount; cmcnt++)
                    {
                        comPercents.Add(0);//стартовая нагрузка коммуникаторов
                        agreg.Add(new List<int>());//создание коммуникаторов
                    }
                    //расрпделение буферов
                    for (int pr = 0; pr < percents1.Count; pr++)//обход каждого буфера
                    {
                        int cmin = comPercents.IndexOf(comPercents.Min());//находим самый свободный коммуникатор
                                                                          //увеличиваем нагрузку на ком
                        comPercents[cmin] += percents1[pr];
                        //добавляем буфер в ком
                        agreg[cmin].Add(percents1[pr]);
                    }
                    //agreg = perfectCombination.First().ToList();

                    for (int i = 0; i < comCount; i++)//каждый коммуникатор
                    {
                        comList.Add(new nedogruzMod());//создаем коммуникатор
                                                       //добавляем в коммуникатор буферы
                        for (int j = 0; j < agreg[i].Count; j++)
                        {
                            for (int buf = 0; buf < connetctionList[percents1.IndexOf(agreg[i][j])].buffers.Count; buf++)
                            {
                                comList[i].comBufList.Add(connetctionList[percents1.IndexOf(agreg[i][j])].buffers[buf]);
                            }
                            percents1[percents1.IndexOf(agreg[i][j])] = -1;
                        }
                    }
                    //добавляем машины
                    for (int j = 0; j < comList.Count; j++)//для каждого коммуникатора
                    {
                        for (int k = 0; k < comList[j].comBufList.Count; k++)//каждый коммуникатор реализаует минимум один буфер, обходим все
                        {
                            if (!comList[j].connectedDPMs.Exists((x) => x == bufList[comList[j].comBufList[k]].input))
                            {
                                comList[j].connectedDPMs.Add(bufList[comList[j].comBufList[k]].input);
                            }
                            if (!comList[j].connectedDPMs.Exists((x) => x == bufList[comList[j].comBufList[k]].output))
                            {
                                comList[j].connectedDPMs.Add(bufList[comList[j].comBufList[k]].output);
                            }
                        }
                    }
                    for (int s = 0; s < comList.Count; s++)
                    {
                        comList[s].connectedDPMs.Sort();
                        comList[s].comBufList.Sort();
                    }
                    break;
            }
            //здесь финальное моделирование
            int pfmc = 0;
            PointPairList dpmPointList = new PointPairList();//
            PointPairList bufReadPointList = new PointPairList();//Массивы точек для каждой итерации
            PointPairList bufWritePointList = new PointPairList();//
            PointPairList bePointList = new PointPairList();//

            List<TextObj> lablesList = new List<TextObj>();
            List<TextObj> lablesList1 = new List<TextObj>();
            List<TextObj> lablesList2 = new List<TextObj>();
            //сбрасываем общие настройки
            dataReset();
            //второй этап промоделировать до определенного момента
            for (int circle = 0; circle < 1000; circle++)//цикл в котором происходит моделирование
            {
                //первый этап, машины выставляют заявки
                for (int j = 0; j < dpmList.Count; j++)//обход каждой машины
                {
                    //проверка на повторное исполнение команд
                    if (dpmList[j].currentCommand >= dpmList[j].dpmCommandList.Count)
                    {
                        dpmList[j].currentCommand = dpmList[j].repeatComNum;//если машина выполнила все инструкции, начинает работать с повторяемой
                    }
                    if (dpmList[j].exchangeReady == false)//если машина не выставила заявку на обмен
                    {
                        switch (dpmList[j].dpmCommandList[dpmList[j].currentCommand].commandType)//проверяем тип текущей команды машины
                        {
                            case "wait"://если ожидание
                                dpmPointList.Add(dpmList[j].currentTime, j + 1, dpmList[j].currentTime + dpmList[j].dpmCommandList[dpmList[j].currentCommand].waitTime);
                                dpmList[j].currentTime += dpmList[j].dpmCommandList[dpmList[j].currentCommand].waitTime;//если инструкция ждать, машина просто ждет
                                dpmList[j].currentCommand++;//Выполнили команду, переходим к следующей
                                break;
                            case "write"://если запись
                            case "read"://если чтение
                                dpmList[j].exchangeReady = true;//заявка на обмен выставлена
                                break;
                        }
                    }
                }
                //Этап второй, коммуникаторы обрабатывают заявки
                for (int k = 0; k < comList.Count; k++)//обход каждого коммуникатора
                {
                    int stp = 0;
                    int min = 0;
                    int minIdx = 0;
                    //опрос коммуникатором машины
                    for (int l = 0; l < comList[k].connectedDPMs.Count; l++)//каждый коммуникатор проверяет связанные с ним машины
                    {
                        //если машина готова к обмену и хочет использовать именно этот коммуникатор и не заблокирована
                        if ((dpmList[comList[k].connectedDPMs[l]].exchangeReady == true) && (comList[k].comBufList.Contains(dpmList[comList[k].connectedDPMs[l]].dpmCommandList[dpmList[comList[k].connectedDPMs[l]].currentCommand].destination)) && (dpmList[comList[k].connectedDPMs[l]].isBlocked == false))
                        {
                            //ищем минимальную выставленную заявку
                            if (stp == 0)//если это первая готовая машина
                            {
                                minIdx = comList[k].connectedDPMs[l];//запоминаем индекс
                                min = dpmList[comList[k].connectedDPMs[l]].currentTime;//запоминаем время
                                stp++;
                            }
                            else
                            {
                                if (dpmList[comList[k].connectedDPMs[l]].currentTime < min)
                                {
                                    minIdx = comList[k].connectedDPMs[l];//запоминаем индекс
                                    min = dpmList[comList[k].connectedDPMs[l]].currentTime;//запоминаем время
                                }
                            }
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
                                    //проверить параллельность
                                    if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange > dpmList[minIdx].currentTime)
                                    {//если данные в буфере появились позже чем текущее время машины. "догоняем"
                                     //эмулируем неудачный обмен
                                     //bePointList[i].Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + 1);
                                        dpmList[minIdx].currentTime = bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange;
                                    }

                                    //в буфер записались данные
                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].dataInBuf += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                    //добавляем на график (curTime/bufNum/dataSize)
                                    bufWritePointList.Add(dpmList[minIdx].currentTime, -dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination - 1, dpmList[minIdx].currentTime + dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize);
                                    bufWritePointList.Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize);
                                    //время машины сдвигаем
                                    //время машины сдвигаем
                                    dpmList[minIdx].currentTime += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                    //сохраняем время когда данные есть
                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange = dpmList[minIdx].currentTime;
                                    //двигаем все остальные буферы подключенные к коммуникатору на объем данных
                                    for (int s = 0; s < comList[k].comBufList.Count; s++)
                                    {
                                        if (s != dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination)
                                        {  //кроме текущего
                                           //bufList[pereborList[i].comList[k].comBufList[s]].lastDataChange += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                            bufList[comList[k].comBufList[s]].lastDataChange = dpmList[minIdx].currentTime;
                                        }
                                    }
                                    //проверям можно ли разблокировать какую-то машину
                                    if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum != -1)
                                    {
                                        //разблокируем машина
                                        dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].isBlocked = false;
                                        //нужно продвинуть время машины
                                        //Новое текущее время заблоченной машины = curTime разлокрирубщей - curTime заблокированной
                                        //   dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime += (dpmList[minIdx].currentTime - dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime);
                                        //очищаем номер
                                        bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum = -1;
                                    }
                                    //переключаем команду дпм
                                    dpmList[minIdx].currentCommand++;
                                    //снимаем заявку на обмен
                                    dpmList[minIdx].exchangeReady = false;
                                    //фиксируем удачный обмен
                                    pfmc++;
                                }
                                else//неудачная попытка обмена
                                {
                                    bePointList.Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + 1);
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
                                    //проверить параллельность
                                    if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange > dpmList[minIdx].currentTime)
                                    { //если данные в буфере появились позже чем текущее время машины. "догоняем"
                                      //эмулируем неудачный обмен
                                      //bePointList[i].Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + 1);
                                        dpmList[minIdx].currentTime = bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange;
                                    }

                                    //в буфере удаляются данные
                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].dataInBuf -= dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                    //добавляем на график (curTime/bufNum/dataSize)
                                    bufReadPointList.Add(dpmList[minIdx].currentTime, -dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination - 1, dpmList[minIdx].currentTime + dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize);
                                    bufReadPointList.Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize);
                                    //время машины сдвигаем
                                    //время машины сдвигаем
                                    dpmList[minIdx].currentTime += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                    //сохраняем время когда данные есть
                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange = dpmList[minIdx].currentTime;
                                    //двигаем все остальные буферы подключенные к коммуникатору на объем данных
                                    for (int s = 0; s < comList[k].comBufList.Count; s++)
                                    {
                                        if (s != dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination)
                                        {   //кроме текущего
                                            //bufList[pereborList[i].comList[k].comBufList[s]].lastDataChange += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                            bufList[comList[k].comBufList[s]].lastDataChange = dpmList[minIdx].currentTime;
                                        }
                                    }
                                    //проверям можно ли разблокировать какую-то машину
                                    if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum != -1)
                                    {
                                        //разблокируем машина
                                        dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].isBlocked = false;
                                        //нужно продвинуть время машины
                                        //Новое текущее время заблоченной машины = curTime разлокрирубщей - curTime заблокированной
                                        //   dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime += (dpmList[minIdx].currentTime - dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime);
                                        //очищаем номер
                                        bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum = -1;
                                    }
                                    //переключаем команду дпм
                                    dpmList[minIdx].currentCommand++;
                                    //снимаем заявку на обмен
                                    dpmList[minIdx].exchangeReady = false;
                                    //фиксируем удачный обмен
                                    pfmc++;
                                }
                                else//неудачная попытка обмена
                                {
                                    bePointList.Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + 1);
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
            HiLowBarItem bar = pane.AddHiLowBar("Ожидание", dpmPointList, Color.Black);//задаем цвет и название(опционально) каждого массива точек
            bar.Bar.Fill = new Fill(Color.Gray);//работа машины (wait)

            HiLowBarItem bar1 = pane.AddHiLowBar("Запись", bufWritePointList, Color.Red);
            bar1.Bar.Fill = new Fill(Color.Yellow);//запись в буфер

            HiLowBarItem ba2 = pane.AddHiLowBar("Чтение", bufReadPointList, Color.LightGreen);
            ba2.Bar.Fill = new Fill(Color.FromArgb(146, 208, 80));//чтение из буфера

            HiLowBarItem bar3 = pane.AddHiLowBar("Ошибка", bePointList, Color.Red);
            bar3.Bar.Fill = new Fill(Color.Red);//запись в буфер

            for (int lbl = 0; lbl < dpmPointList.Count; lbl++)
            {
                lablesList.Add(new TextObj(Convert.ToString(bar.Points[lbl].Z - bar.Points[lbl].X), bar.Points[lbl].X + (bar.Points[lbl].Z - bar.Points[lbl].X) / 2, bar.Points[lbl].Y));
                lablesList[lbl].FontSpec.Fill = new Fill(Color.Gray);//цвет фона текста
                lablesList[lbl].FontSpec.Size = 11;//размер
                lablesList[lbl].FontSpec.Border.IsVisible = false;//выделение раниц
                pane.GraphObjList.Add(lablesList[lbl]);//добавить текст
            }
            for (int lbl = 0; lbl < bufWritePointList.Count; lbl++)
            {
                lablesList1.Add(new TextObj(Convert.ToString(bar1.Points[lbl].Z - bar1.Points[lbl].X), bar1.Points[lbl].X + (bar1.Points[lbl].Z - bar1.Points[lbl].X) / 2, bar1.Points[lbl].Y));
                lablesList1[lbl].FontSpec.Fill = new Fill(Color.Yellow);//цвет фона текста
                lablesList1[lbl].FontSpec.Size = 11;//размер
                lablesList1[lbl].FontSpec.Border.IsVisible = false;//выделение раниц
                pane.GraphObjList.Add(lablesList1[lbl]);//добавить текст
            }
            for (int lbl = 0; lbl < bufReadPointList.Count; lbl++)
            {
                lablesList2.Add(new TextObj(Convert.ToString(ba2.Points[lbl].Z - ba2.Points[lbl].X), ba2.Points[lbl].X + (ba2.Points[lbl].Z - ba2.Points[lbl].X) / 2, ba2.Points[lbl].Y));
                lablesList2[lbl].FontSpec.Fill = new Fill(Color.FromArgb(146, 208, 80));//цвет фона текста
                lablesList2[lbl].FontSpec.Size = 11;//размер 
                lablesList2[lbl].FontSpec.Border.IsVisible = false;//выделение раниц
                pane.GraphObjList.Add(lablesList2[lbl]);//добавить текст
            }
            textBox2.Text += "Агрегация проведена \n";
            for (int agr = 0; agr < comList.Count; agr++)//обходим все коммуникаторы
            {
                textBox2.Text += "\r\nКомуникатор " + (agr + 1) + " реализует буферы: ";
                for (int bf = 0; bf < comList[agr].comBufList.Count; bf++)
                {
                    textBox2.Text += (comList[agr].comBufList[bf] + 1) + " ";
                }
                textBox2.Text += "\n";
            }
            textBox2.Text += "\r\nКол-во обменов: " + pfmc;
            Graphics g = this.CreateGraphics();//вывод графика
            pane.AxisChange(g);
            g.Dispose();
            zedGraphControl1.Refresh();
        }
        
        private void dpmcenter()
        {
            clearData();
            List<dpmEnterclass> dpmEntersList = new List<dpmEnterclass>();//список входов машин
            for (int i = 0; i < dpmList.Count; i++)//для каждой машины
            {
                if (dpmList[i].inBuf.Count > 0)//если в машину входят буферы
                {
                    dpmEntersList.Add(new dpmEnterclass());//добавляем запись
                    for (int j = 0; j < dpmList[i].inBuf.Count; j++)//для каждого входящего буфера
                    {
                        dpmEntersList[dpmEntersList.IndexOf(dpmEntersList.Last())].DPMNumber = i;//запоминаем номер машины
                        dpmEntersList[dpmEntersList.IndexOf(dpmEntersList.Last())].dpmEnterBuffers.Add(dpmList[i].inBuf[j]-1);
                    }
                }
            }
            //получив список связей
            //3 варианта, либо буферов больше, чем связей, либо меньше, либо равно
            int comCount = Convert.ToInt32(textBox1.Text);//кол-во коммуникаторов
            int menu = -1;
            if (comCount == dpmEntersList.Count)
            {
                menu = 1;
            }
            if (comCount > dpmEntersList.Count)
            {
                menu = 2;
            }
            if (comCount < dpmEntersList.Count)
            {
                menu = 3;
            }
            List<nedogruzMod> comList = new List<nedogruzMod>();//список коммуникаторов

            switch (menu)
            {
                case 1://если предпологаемое число коммуникаторов = число связей между машинами
                       //каждый коммуникатор реализует одну связь
                    for (int i = 0; i < comCount; i++)//каждый коммуникатор
                    {
                        comList.Add(new nedogruzMod());//создаем коммуникатор
                        //добавляем все буферы текущей соединялки
                        //добавляем в коммуникатор буферы
                        for (int j = 0; j < dpmEntersList[i].dpmEnterBuffers.Count; j++)
                        {
                            comList[i].comBufList.Add(dpmEntersList[i].dpmEnterBuffers[j]);//распределили буферы по коммуникаторам
                        }
                    }
                    //добавляем машины
                    for (int j = 0; j < comList.Count; j++)//для каждого коммуникатора
                    {
                        for (int k = 0; k < comList[j].comBufList.Count; k++)//каждый коммуникатор реализаует минимум один буфер, обходим все
                        {
                            if (!comList[j].connectedDPMs.Exists((x) => x == bufList[comList[j].comBufList[k]].input))
                            {
                                comList[j].connectedDPMs.Add(bufList[comList[j].comBufList[k]].input);
                            }
                            if (!comList[j].connectedDPMs.Exists((x) => x == bufList[comList[j].comBufList[k]].output))
                            {
                                comList[j].connectedDPMs.Add(bufList[comList[j].comBufList[k]].output);
                            }
                        }
                    }
                    break;
                case 2://если коммуникаторов больше, чем входов
                    int raznica = comCount - dpmEntersList.Count;//на сколько больше коммуникаторов
                    for(int r = 0; r < raznica; r++)
                    {//распределяем буферы по лишним коммуникаторам
                        //ищем самый нагруженный вход
                        int max = dpmEntersList[0].dpmEnterBuffers.Count;
                        int maxIdx = 0;
                        for (int cnt = 1; cnt < dpmEntersList.Count; cnt++)
                        {//обходим все входы кроме 0
                            if (dpmEntersList[cnt].dpmEnterBuffers.Count > max)
                            {
                                max = dpmEntersList[cnt].dpmEnterBuffers.Count;
                                maxIdx = cnt;
                            }
                        }
                        //добавялем буферы из самого нагруженного входа в отдельный коммуникатор
                        int bufToMove = dpmEntersList[maxIdx].dpmEnterBuffers.Count / 2;//сколько буферов перенесте
                        comList.Add(new nedogruzMod());//создаем коммуникатор
                        for (int btm = 0; btm < bufToMove; btm++)
                        {

                            //переносим буферы из самного нагруженного входа 
                            comList[comList.Count-1].comBufList.Add(dpmEntersList[maxIdx].dpmEnterBuffers[btm]);
                            //удаляем буферы из входа
                            dpmEntersList[maxIdx].dpmEnterBuffers.RemoveAt(btm);
                        }

                    }
                    //распределяем остальные коммуникаторы
                    for(int cm = 0; cm < dpmEntersList.Count; cm++)
                    {
                        comList.Add(new nedogruzMod());
                        for(int bcnt = 0; bcnt < dpmEntersList[cm].dpmEnterBuffers.Count; bcnt++)
                        {
                            //добавляем в коммуникатор буферы
                            comList[comList.Count - 1].comBufList.Add(dpmEntersList[cm].dpmEnterBuffers[bcnt]);
                        }
                    }
                    //добавляем машины
                    for (int j = 0; j < comList.Count; j++)//для каждого коммуникатора
                    {
                        for (int k = 0; k < comList[j].comBufList.Count; k++)//каждый коммуникатор реализаует минимум один буфер, обходим все
                        {
                            if (!comList[j].connectedDPMs.Exists((x) => x == bufList[comList[j].comBufList[k]].input))
                            {
                                comList[j].connectedDPMs.Add(bufList[comList[j].comBufList[k]].input);
                            }
                            if (!comList[j].connectedDPMs.Exists((x) => x == bufList[comList[j].comBufList[k]].output))
                            {
                                comList[j].connectedDPMs.Add(bufList[comList[j].comBufList[k]].output);
                            }
                        }
                    }
                    break;
                case 3://если коммуникаторов меньше, чем входов
                    int razn = dpmEntersList.Count - comCount;//на сколько меньше коммуникаторов
                    for(int rz = 0; rz < razn; rz++)
                    {//удаляем лишние входы
                     //ищем самый ненагруженный вход
                        int min = dpmEntersList[0].dpmEnterBuffers.Count;
                        int minIdx = 0;
                        for (int cnt = 1; cnt < dpmEntersList.Count; cnt++)
                        {//обходим все входы кроме 0
                            if (dpmEntersList[cnt].dpmEnterBuffers.Count < min)
                            {
                                min = dpmEntersList[cnt].dpmEnterBuffers.Count;
                                minIdx = cnt;
                            }
                        }
                        //сохраняем буферы
                        List<int> tmpBf = new List<int>();
                        tmpBf.Clear();
                        tmpBf = dpmEntersList[minIdx].dpmEnterBuffers;
                        //удаляем его
                        dpmEntersList.RemoveAt(minIdx);
                        //ищем новый минимальный коммуникатор
                        min = dpmEntersList[0].dpmEnterBuffers.Count;
                        minIdx = 0;
                        for (int cnt = 1; cnt < dpmEntersList.Count; cnt++)
                        {//обходим все входы кроме 0
                            if (dpmEntersList[cnt].dpmEnterBuffers.Count < min)
                            {
                                min = dpmEntersList[cnt].dpmEnterBuffers.Count;
                                minIdx = cnt;
                            }
                        }
                        //добавляем старые буферы в новый вход
                        for(int gs = 0; gs < tmpBf.Count; gs++)
                        {
                            dpmEntersList[minIdx].dpmEnterBuffers.Add(tmpBf[gs]);
                        }
                    }
                    //распределяем остальные коммуникаторы
                    for (int cm = 0; cm < dpmEntersList.Count; cm++)
                    {
                        comList.Add(new nedogruzMod());
                        for (int bcnt = 0; bcnt < dpmEntersList[cm].dpmEnterBuffers.Count; bcnt++)
                        {
                            //добавляем в коммуникатор буферы
                            comList[comList.Count - 1].comBufList.Add(dpmEntersList[cm].dpmEnterBuffers[bcnt]);
                        }
                    }
                    //добавляем машины
                    for (int j = 0; j < comList.Count; j++)//для каждого коммуникатора
                    {
                        for (int k = 0; k < comList[j].comBufList.Count; k++)//каждый коммуникатор реализаует минимум один буфер, обходим все
                        {
                            if (!comList[j].connectedDPMs.Exists((x) => x == bufList[comList[j].comBufList[k]].input))
                            {
                                comList[j].connectedDPMs.Add(bufList[comList[j].comBufList[k]].input);
                            }
                            if (!comList[j].connectedDPMs.Exists((x) => x == bufList[comList[j].comBufList[k]].output))
                            {
                                comList[j].connectedDPMs.Add(bufList[comList[j].comBufList[k]].output);
                            }
                        }
                    }
                    break;
            }
            //здесь финальное моделирование
            int pfmc = 0;
            PointPairList dpmPointList = new PointPairList();//
            PointPairList bufReadPointList = new PointPairList();//Массивы точек для каждой итерации
            PointPairList bufWritePointList = new PointPairList();//
            PointPairList bePointList = new PointPairList();//

            List<TextObj> lablesList = new List<TextObj>();
            List<TextObj> lablesList1 = new List<TextObj>();
            List<TextObj> lablesList2 = new List<TextObj>();
            //сбрасываем общие настройки
            for (int sbs = 0; sbs < dpmList.Count; sbs++)
            {
                dpmList[sbs].exchangeReady = false;//При старте новой агрегации, данные в машинах сбрасываются
                dpmList[sbs].currentTime = 0;
                dpmList[sbs].currentCommand = 0;
                dpmList[sbs].isBlocked = false;
            }
            for (int sbs = 0; sbs < bufList.Count; sbs++)//сброс данных для буферов
            {
                bufList[sbs].blockedDPMnum = -1;
                bufList[sbs].dataInBuf = 0;
                bufList[sbs].lastDataChange = 0;
            }
            //второй этап промоделировать до определенного момента
            for (int circle = 0; circle < 1000; circle++)//цикл в котором происходит моделирование
            {
                //первый этап, машины выставляют заявки
                for (int j = 0; j < dpmList.Count; j++)//обход каждой машины
                {
                    //проверка на повторное исполнение команд
                    if (dpmList[j].currentCommand >= dpmList[j].dpmCommandList.Count)
                    {
                        dpmList[j].currentCommand = dpmList[j].repeatComNum;//если машина выполнила все инструкции, начинает работать с повторяемой
                    }
                    if (dpmList[j].exchangeReady == false)//если машина не выставила заявку на обмен
                    {
                        switch (dpmList[j].dpmCommandList[dpmList[j].currentCommand].commandType)//проверяем тип текущей команды машины
                        {
                            case "wait"://если ожидание
                                dpmPointList.Add(dpmList[j].currentTime, j + 1, dpmList[j].currentTime + dpmList[j].dpmCommandList[dpmList[j].currentCommand].waitTime);
                                dpmList[j].currentTime += dpmList[j].dpmCommandList[dpmList[j].currentCommand].waitTime;//если инструкция ждать, машина просто ждет
                                dpmList[j].currentCommand++;//Выполнили команду, переходим к следующей
                                break;
                            case "write"://если запись
                            case "read"://если чтение
                                dpmList[j].exchangeReady = true;//заявка на обмен выставлена
                                break;
                        }
                    }
                }
                //Этап второй, коммуникаторы обрабатывают заявки
                for (int k = 0; k < comList.Count; k++)//обход каждого коммуникатора
                {
                    int stp = 0;
                    int min = 0;
                    int minIdx = 0;
                    //опрос коммуникатором машины
                    for (int l = 0; l < comList[k].connectedDPMs.Count; l++)//каждый коммуникатор проверяет связанные с ним машины
                    {
                        //если машина готова к обмену и хочет использовать именно этот коммуникатор и не заблокирована
                        if ((dpmList[comList[k].connectedDPMs[l]].exchangeReady == true) && (comList[k].comBufList.Contains(dpmList[comList[k].connectedDPMs[l]].dpmCommandList[dpmList[comList[k].connectedDPMs[l]].currentCommand].destination)) && (dpmList[comList[k].connectedDPMs[l]].isBlocked == false))
                        {
                            //ищем минимальную выставленную заявку
                            if (stp == 0)//если это первая готовая машина
                            {
                                minIdx = comList[k].connectedDPMs[l];//запоминаем индекс
                                min = dpmList[comList[k].connectedDPMs[l]].currentTime;//запоминаем время
                                stp++;
                            }
                            else
                            {
                                if (dpmList[comList[k].connectedDPMs[l]].currentTime < min)
                                {
                                    minIdx = comList[k].connectedDPMs[l];//запоминаем индекс
                                    min = dpmList[comList[k].connectedDPMs[l]].currentTime;//запоминаем время
                                }
                            }
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
                                    //проверить параллельность
                                    if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange > dpmList[minIdx].currentTime)
                                    {//если данные в буфере появились позже чем текущее время машины. "догоняем"
                                     //эмулируем неудачный обмен
                                     //bePointList[i].Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + 1);
                                        dpmList[minIdx].currentTime = bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange;
                                    }

                                    //в буфер записались данные
                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].dataInBuf += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                    //добавляем на график (curTime/bufNum/dataSize)
                                    bufWritePointList.Add(dpmList[minIdx].currentTime, -dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination - 1, dpmList[minIdx].currentTime + dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize);
                                    bufWritePointList.Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize);
                                    //время машины сдвигаем
                                    //время машины сдвигаем
                                    dpmList[minIdx].currentTime += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                    //сохраняем время когда данные есть
                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange = dpmList[minIdx].currentTime;
                                    //двигаем все остальные буферы подключенные к коммуникатору на объем данных
                                    for (int s = 0; s < comList[k].comBufList.Count; s++)
                                    {
                                        if (s != dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination)
                                        {  //кроме текущего
                                           //bufList[pereborList[i].comList[k].comBufList[s]].lastDataChange += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                            bufList[comList[k].comBufList[s]].lastDataChange = dpmList[minIdx].currentTime;
                                        }
                                    }
                                    //проверям можно ли разблокировать какую-то машину
                                    if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum != -1)
                                    {
                                        //разблокируем машина
                                        dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].isBlocked = false;
                                        //нужно продвинуть время машины
                                        //Новое текущее время заблоченной машины = curTime разлокрирубщей - curTime заблокированной
                                        //   dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime += (dpmList[minIdx].currentTime - dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime);
                                        //очищаем номер
                                        bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum = -1;
                                    }
                                    //переключаем команду дпм
                                    dpmList[minIdx].currentCommand++;
                                    //снимаем заявку на обмен
                                    dpmList[minIdx].exchangeReady = false;
                                    //фиксируем удачный обмен
                                    pfmc++;
                                }
                                else//неудачная попытка обмена
                                {
                                    bePointList.Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + 1);
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
                                    //проверить параллельность
                                    if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange > dpmList[minIdx].currentTime)
                                    { //если данные в буфере появились позже чем текущее время машины. "догоняем"
                                      //эмулируем неудачный обмен
                                      //bePointList[i].Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + 1);
                                        dpmList[minIdx].currentTime = bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange;
                                    }

                                    //в буфере удаляются данные
                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].dataInBuf -= dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                    //добавляем на график (curTime/bufNum/dataSize)
                                    bufReadPointList.Add(dpmList[minIdx].currentTime, -dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination - 1, dpmList[minIdx].currentTime + dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize);
                                    bufReadPointList.Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize);
                                    //время машины сдвигаем
                                    //время машины сдвигаем
                                    dpmList[minIdx].currentTime += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                    //сохраняем время когда данные есть
                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange = dpmList[minIdx].currentTime;
                                    //двигаем все остальные буферы подключенные к коммуникатору на объем данных
                                    for (int s = 0; s < comList[k].comBufList.Count; s++)
                                    {
                                        if (s != dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination)
                                        {   //кроме текущего
                                            //bufList[pereborList[i].comList[k].comBufList[s]].lastDataChange += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                            bufList[comList[k].comBufList[s]].lastDataChange = dpmList[minIdx].currentTime;
                                        }
                                    }
                                    //проверям можно ли разблокировать какую-то машину
                                    if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum != -1)
                                    {
                                        //разблокируем машина
                                        dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].isBlocked = false;
                                        //нужно продвинуть время машины
                                        //Новое текущее время заблоченной машины = curTime разлокрирубщей - curTime заблокированной
                                        //   dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime += (dpmList[minIdx].currentTime - dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime);
                                        //очищаем номер
                                        bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum = -1;
                                    }
                                    //переключаем команду дпм
                                    dpmList[minIdx].currentCommand++;
                                    //снимаем заявку на обмен
                                    dpmList[minIdx].exchangeReady = false;
                                    //фиксируем удачный обмен
                                    pfmc++;
                                }
                                else//неудачная попытка обмена
                                {
                                    bePointList.Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + 1);
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
            HiLowBarItem bar = pane.AddHiLowBar("Ожидание", dpmPointList, Color.Black);//задаем цвет и название(опционально) каждого массива точек
            bar.Bar.Fill = new Fill(Color.Gray);//работа машины (wait)

            HiLowBarItem bar1 = pane.AddHiLowBar("Запись", bufWritePointList, Color.Red);
            bar1.Bar.Fill = new Fill(Color.Yellow);//запись в буфер

            HiLowBarItem ba2 = pane.AddHiLowBar("Чтение", bufReadPointList, Color.LightGreen);
            ba2.Bar.Fill = new Fill(Color.FromArgb(146, 208, 80));//чтение из буфера

            HiLowBarItem bar3 = pane.AddHiLowBar("Ошибка", bePointList, Color.Red);
            bar3.Bar.Fill = new Fill(Color.Red);//запись в буфер

            for (int lbl = 0; lbl < dpmPointList.Count; lbl++)
            {
                lablesList.Add(new TextObj(Convert.ToString(bar.Points[lbl].Z - bar.Points[lbl].X), bar.Points[lbl].X + (bar.Points[lbl].Z - bar.Points[lbl].X) / 2, bar.Points[lbl].Y));
                lablesList[lbl].FontSpec.Fill = new Fill(Color.Gray);//цвет фона текста
                lablesList[lbl].FontSpec.Size = 11;//размер
                lablesList[lbl].FontSpec.Border.IsVisible = false;//выделение раниц
                pane.GraphObjList.Add(lablesList[lbl]);//добавить текст
            }
            for (int lbl = 0; lbl < bufWritePointList.Count; lbl++)
            {
                lablesList1.Add(new TextObj(Convert.ToString(bar1.Points[lbl].Z - bar1.Points[lbl].X), bar1.Points[lbl].X + (bar1.Points[lbl].Z - bar1.Points[lbl].X) / 2, bar1.Points[lbl].Y));
                lablesList1[lbl].FontSpec.Fill = new Fill(Color.Yellow);//цвет фона текста
                lablesList1[lbl].FontSpec.Size = 11;//размер
                lablesList1[lbl].FontSpec.Border.IsVisible = false;//выделение раниц
                pane.GraphObjList.Add(lablesList1[lbl]);//добавить текст
            }
            for (int lbl = 0; lbl < bufReadPointList.Count; lbl++)
            {
                lablesList2.Add(new TextObj(Convert.ToString(ba2.Points[lbl].Z - ba2.Points[lbl].X), ba2.Points[lbl].X + (ba2.Points[lbl].Z - ba2.Points[lbl].X) / 2, ba2.Points[lbl].Y));
                lablesList2[lbl].FontSpec.Fill = new Fill(Color.FromArgb(146, 208, 80));//цвет фона текста
                lablesList2[lbl].FontSpec.Size = 11;//размер 
                lablesList2[lbl].FontSpec.Border.IsVisible = false;//выделение раниц
                pane.GraphObjList.Add(lablesList2[lbl]);//добавить текст
            }
            textBox2.Text += "Агрегация проведена \n";
            for (int agr = 0; agr < comList.Count; agr++)//обходим все коммуникаторы
            {
                textBox2.Text += "\r\nКомуникатор " + (agr + 1) + " реализует буферы: ";
                for (int bf = 0; bf < comList[agr].comBufList.Count; bf++)
                {
                    textBox2.Text += (comList[agr].comBufList[bf] + 1) + " ";
                }
                textBox2.Text += "\n";
            }
            textBox2.Text += "\r\nКол-во обменов: " + pfmc;
            Graphics g = this.CreateGraphics();//вывод графика
            pane.AxisChange(g);
            g.Dispose();
            zedGraphControl1.Refresh();
        }
    
        private void modelPerebor()//моделирование перебором
        {
            clearData();
            List<pereborMod> pereborList = new List<pereborMod>();//Список содержащий все варианты агрегации схемы
            int comNumb = Convert.ToInt32(textBox1.Text);//число коммуникаторов
            int agregNumb = GetStarling(bufList.Count, comNumb);//число всех возможных агрегаций при заданном числе коммуникаторов
            for (int i = 0; i < agregNumb; i++)
            {
                pereborList.Add(new pereborMod());
                for (int j = 0; j < comNumb; j++)
                {
                    pereborList[i].comList.Add(new pereborMod.comClass());
                }
            }
            int[] nums = new int[bufList.Count];
            for (int i = 0; i < bufList.Count; i++)
            {
                nums[i] = i;
            }
            IEnumerable<int[][]> resultSets = Partitioning.GetAllPartitions(nums);//получаем ВСЕ разбиения
            int[][][] preConvertInts = resultSets.Select(x => x.ToArray()).ToArray();//конвертирует ienumerable в обычный массив
            int indexPerebora = 0;//число переборов
            for (int i = 0; i < preConvertInts.Length; i++)
            {
                if (preConvertInts[i].Length == comNumb)//если разбиваем на нужное коичество групп
                {
                    for (int j = 0; j < comNumb; j++)
                    {
                        for (int k = 0; k < preConvertInts[i][j].Length; k++)
                        {
                            pereborList[indexPerebora].comList[j].comBufList.Add(preConvertInts[i][j][k]);
                        }
                    }
                    indexPerebora++;
                }
            }
            //очистка памяти
            Array.Clear(nums, 0, nums.Length);
            Array.Clear(preConvertInts, 0, preConvertInts.Length);
            //заполнение списка обслуживаемых машин у каждого коммуникатора
            for (int i = 0; i < agregNumb; i++)//для каждого варианта агрегации
            {
                for (int j = 0; j < comNumb; j++)//для каждого коммуникатора
                {
                    for (int k = 0; k < pereborList[i].comList[j].comBufList.Count; k++)//обход буферов каждого коммуникатора
                    {
                        if (!pereborList[i].comList[j].connectedDPMs.Exists((x) => x == bufList[pereborList[i].comList[j].comBufList[k]].input))
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
            for (int i = 0; i < agregNumb; i++)//моделирование проводится для каждой возможной агрегации
            {
                //сброс общих настроек
                dataReset();
                for (int circle = 0; circle < 1000; circle++)//цикл в котором происходит моделирование
                {
                    //первый этап, машины выставляют заявки
                    for (int j = 0; j < dpmList.Count; j++)//обход каждой машины
                    {
                        //проверка на повторное исполнение команд
                        if (dpmList[j].currentCommand >= dpmList[j].dpmCommandList.Count)
                        {
                            dpmList[j].currentCommand = dpmList[j].repeatComNum;//если машина выполнила все инструкции, начинает работать с повторяемой
                        }
                        if (dpmList[j].exchangeReady == false)//если машина не выставила заявку на обмен
                        {
                            switch (dpmList[j].dpmCommandList[dpmList[j].currentCommand].commandType)//проверяем тип текущей команды машины
                            {
                                case "wait"://если ожидание
                                    dpmList[j].currentTime += dpmList[j].dpmCommandList[dpmList[j].currentCommand].waitTime;//если инструкция ждать, машина просто ждет
                                    dpmList[j].currentCommand++;//Выполнили команду, переходим к следующей
                                    break;
                                case "write"://если запись
                                case "read"://если чтение
                                    dpmList[j].exchangeReady = true;//заявка на обмен выставлена
                                    break;
                            }
                        }
                    }
                    //Этап второй, коммуникаторы обрабатывают заявки
                    for (int k = 0; k < comNumb; k++)//обход каждого коммуникатора
                    {
                        int stp = 0;
                        int min = 0;
                        int minIdx = 0;
                        //опрос коммуникатором машины
                        for (int l = 0; l < pereborList[i].comList[k].connectedDPMs.Count; l++)//каждый коммуникатор проверяет связанные с ним машины
                        {
                            //если машина готова к обмену и хочет использовать именно этот коммуникатор и не заблокирована
                            if ((dpmList[pereborList[i].comList[k].connectedDPMs[l]].exchangeReady == true) && (pereborList[i].comList[k].comBufList.Contains(dpmList[pereborList[i].comList[k].connectedDPMs[l]].dpmCommandList[dpmList[pereborList[i].comList[k].connectedDPMs[l]].currentCommand].destination)) && (dpmList[pereborList[i].comList[k].connectedDPMs[l]].isBlocked == false))
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
                                    if (dpmList[pereborList[i].comList[k].connectedDPMs[l]].currentTime < min)
                                    {
                                        minIdx = pereborList[i].comList[k].connectedDPMs[l];//запоминаем индекс
                                        min = dpmList[pereborList[i].comList[k].connectedDPMs[l]].currentTime;//запоминаем время
                                    }
                                }
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
                                        //проверить параллельность
                                        if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange > dpmList[minIdx].currentTime)
                                        {//если данные в буфере появились позже чем текущее время машины. "догоняем"
                                            dpmList[minIdx].currentTime = bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange;
                                        }

                                        //в буфер записались данные
                                        bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].dataInBuf += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                        //время машины сдвигаем
                                        dpmList[minIdx].currentTime += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                        //сохраняем время когда данные есть
                                        bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange = dpmList[minIdx].currentTime;
                                        //двигаем все остальные буферы подключенные к коммуникатору на объем данных
                                        for (int s = 0; s < pereborList[i].comList[k].comBufList.Count; s++)
                                        {
                                            if (s != dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination)
                                            {  //кроме текущего
                                                //bufList[pereborList[i].comList[k].comBufList[s]].lastDataChange += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                                bufList[pereborList[i].comList[k].comBufList[s]].lastDataChange = dpmList[minIdx].currentTime;
                                            }
                                        }
                                        //проверям можно ли разблокировать какую-то машину
                                        if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum != -1)
                                        {
                                            //разблокируем машина
                                            dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].isBlocked = false;
                                            //нужно продвинуть время машины
                                            //Новое текущее время заблоченной машины = curTime разлокрирубщей - curTime заблокированной
                                            //   dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime += (dpmList[minIdx].currentTime - dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime);
                                            //очищаем номер
                                            bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum = -1;
                                        }
                                        //переключаем команду дпм
                                        dpmList[minIdx].currentCommand++;
                                        //снимаем заявку на обмен
                                        dpmList[minIdx].exchangeReady = false;
                                        //фиксируем удачный обмен
                                        pereborList[i].exchangeCount++;
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
                                        //проверить параллельность
                                        if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange > dpmList[minIdx].currentTime)
                                        { //если данные в буфере появились позже чем текущее время машины. "догоняем"
                                            dpmList[minIdx].currentTime = bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange;
                                        }

                                        //в буфере удаляются данные
                                        bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].dataInBuf -= dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                        //время машины сдвигаем
                                        dpmList[minIdx].currentTime += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                        //сохраняем время когда данные есть
                                        bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange = dpmList[minIdx].currentTime;
                                        //двигаем все остальные буферы подключенные к коммуникатору на объем данных
                                        for (int s = 0; s < pereborList[i].comList[k].comBufList.Count; s++)
                                        {
                                            if (s != dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination)
                                            {   //кроме текущего
                                                bufList[pereborList[i].comList[k].comBufList[s]].lastDataChange = dpmList[minIdx].currentTime;
                                            }
                                        }
                                        //проверям можно ли разблокировать какую-то машину
                                        if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum != -1)
                                        {
                                            //разблокируем машина
                                            dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].isBlocked = false;
                                            //очищаем номер
                                            bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum = -1;
                                        }
                                        //переключаем команду дпм
                                        dpmList[minIdx].currentCommand++;
                                        //снимаем заявку на обмен
                                        dpmList[minIdx].exchangeReady = false;
                                        //фиксируем удачный обмен
                                        pereborList[i].exchangeCount++;
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
            //РИСОВАНИЕ
            //находим номер перебора, в котором было выполнено больше всего обменов
            int drowIdx = pereborList.FindIndex(r => r.exchangeCount == pereborList.Max(x => x.exchangeCount));
            //Инициализция графиков
            //массивы точек
            List<PointPairList> dpmPointList = new List<PointPairList>();
            List<PointPairList> bufReadPointList = new List<PointPairList>();
            List<PointPairList> bufWritePointList = new List<PointPairList>();
            List<PointPairList> bePointList = new List<PointPairList>();
            List<TextObj> lablesList = new List<TextObj>();
            List<TextObj> lablesList1 = new List<TextObj>();
            List<TextObj> lablesList2 = new List<TextObj>();
            //сюда добавить повторное моделирование, чтобы нарисовать
            dpmPointList.Add(new PointPairList());//
            bufReadPointList.Add(new PointPairList());//Добавляем для каждой итерации свой экземпляр массива
            bufWritePointList.Add(new PointPairList());//
            bePointList.Add(new PointPairList());
            //сбрасываем общие настройки
            dataReset();
            for (int circle = 0; circle < 1000; circle++)//цикл в котором происходит моделирование
            {
                //первый этап, машины выставляют заявки
                for (int j = 0; j < dpmList.Count; j++)//обход каждой машины
                {
                    //проверка на повторное исполнение команд
                    if (dpmList[j].currentCommand >= dpmList[j].dpmCommandList.Count)
                    {
                        dpmList[j].currentCommand = dpmList[j].repeatComNum;//если машина выполнила все инструкции, начинает работать с повторяемой
                    }
                    if (dpmList[j].exchangeReady == false)//если машина не выставила заявку на обмен
                    {
                        switch (dpmList[j].dpmCommandList[dpmList[j].currentCommand].commandType)//проверяем тип текущей команды машины
                        {
                            case "wait"://если ожидание
                                dpmPointList[0].Add(dpmList[j].currentTime, j + 1, dpmList[j].currentTime + dpmList[j].dpmCommandList[dpmList[j].currentCommand].waitTime);//добавляем на график
                                dpmList[j].currentTime += dpmList[j].dpmCommandList[dpmList[j].currentCommand].waitTime;//если инструкция ждать, машина просто ждет
                                dpmList[j].currentCommand++;//Выполнили команду, переходим к следующей
                                break;
                            case "write"://если запись
                            case "read"://если чтение
                                dpmList[j].exchangeReady = true;//заявка на обмен выставлена
                                break;
                        }
                    }
                }
                //Этап второй, коммуникаторы обрабатывают заявки
                for (int k = 0; k < comNumb; k++)//обход каждого коммуникатора
                {
                    int stp = 0;
                    int min = 0;
                    int minIdx = 0;
                    //опрос коммуникатором машины
                    for (int l = 0; l < pereborList[drowIdx].comList[k].connectedDPMs.Count; l++)//каждый коммуникатор проверяет связанные с ним машины
                    {
                        //если машина готова к обмену и хочет использовать именно этот коммуникатор и не заблокирована
                        if ((dpmList[pereborList[drowIdx].comList[k].connectedDPMs[l]].exchangeReady == true) && (pereborList[drowIdx].comList[k].comBufList.Contains(dpmList[pereborList[drowIdx].comList[k].connectedDPMs[l]].dpmCommandList[dpmList[pereborList[drowIdx].comList[k].connectedDPMs[l]].currentCommand].destination)) && (dpmList[pereborList[drowIdx].comList[k].connectedDPMs[l]].isBlocked == false))
                        {
                            //ищем минимальную выставленную заявку
                            if (stp == 0)//если это первая готовая машина
                            {
                                minIdx = pereborList[drowIdx].comList[k].connectedDPMs[l];//запоминаем индекс
                                min = dpmList[pereborList[drowIdx].comList[k].connectedDPMs[l]].currentTime;//запоминаем время
                                stp++;
                            }
                            else
                            {
                                if (dpmList[pereborList[drowIdx].comList[k].connectedDPMs[l]].currentTime < min)
                                {
                                    minIdx = pereborList[drowIdx].comList[k].connectedDPMs[l];//запоминаем индекс
                                    min = dpmList[pereborList[drowIdx].comList[k].connectedDPMs[l]].currentTime;//запоминаем время
                                }
                            }
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
                                    //проверить параллельность
                                    if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange > dpmList[minIdx].currentTime)
                                    {//если данные в буфере появились позже чем текущее время машины. "догоняем"
                                        dpmList[minIdx].currentTime = bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange;
                                    }

                                    //в буфер записались данные
                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].dataInBuf += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                    //добавляем на график (curTime/bufNum/dataSize)
                                    bufWritePointList[0].Add(dpmList[minIdx].currentTime, -dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination - 1, dpmList[minIdx].currentTime + dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize);
                                    bufWritePointList[0].Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize);
                                    //время машины сдвигаем
                                    dpmList[minIdx].currentTime += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                    //сохраняем время когда данные есть
                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange = dpmList[minIdx].currentTime;
                                    //двигаем все остальные буферы подключенные к коммуникатору на объем данных
                                    for (int s = 0; s < pereborList[drowIdx].comList[k].comBufList.Count; s++)
                                    {
                                        if (s != dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination)
                                        {  //кроме текущего
                                           //bufList[pereborList[i].comList[k].comBufList[s]].lastDataChange += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                            bufList[pereborList[drowIdx].comList[k].comBufList[s]].lastDataChange = dpmList[minIdx].currentTime;
                                        }
                                    }
                                    //проверям можно ли разблокировать какую-то машину
                                    if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum != -1)
                                    {
                                        //разблокируем машина
                                        dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].isBlocked = false;
                                        //очищаем номер
                                        bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum = -1;
                                    }
                                    //переключаем команду дпм
                                    dpmList[minIdx].currentCommand++;
                                    //снимаем заявку на обмен
                                    dpmList[minIdx].exchangeReady = false;
                                }
                                else//неудачная попытка обмена
                                {
                                    bePointList[0].Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + 1);
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
                                    //проверить параллельность
                                    if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange > dpmList[minIdx].currentTime)
                                    { //если данные в буфере появились позже чем текущее время машины. "догоняем"
                                        dpmList[minIdx].currentTime = bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange;
                                    }

                                    //в буфере удаляются данные
                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].dataInBuf -= dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                    //добавляем на график (curTime/bufNum/dataSize)
                                    bufReadPointList[0].Add(dpmList[minIdx].currentTime, -dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination - 1, dpmList[minIdx].currentTime + dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize);
                                    bufReadPointList[0].Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize);
                                    //время машины сдвигаем
                                    dpmList[minIdx].currentTime += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                    //сохраняем время когда данные есть
                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange = dpmList[minIdx].currentTime;
                                    //двигаем все остальные буферы подключенные к коммуникатору на объем данных
                                    for (int s = 0; s < pereborList[drowIdx].comList[k].comBufList.Count; s++)
                                    {
                                        if (s != dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination)
                                        {   //кроме текущего
                                            bufList[pereborList[drowIdx].comList[k].comBufList[s]].lastDataChange = dpmList[minIdx].currentTime;
                                        }
                                    }
                                    //проверям можно ли разблокировать какую-то машину
                                    if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum != -1)
                                    {
                                        //разблокируем машину
                                        dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].isBlocked = false;
                                        //очищаем номер
                                        bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum = -1;
                                    }
                                    //переключаем команду дпм
                                    dpmList[minIdx].currentCommand++;
                                    //снимаем заявку на обмен
                                    dpmList[minIdx].exchangeReady = false;
                                }
                                else//неудачная попытка обмена
                                {
                                    bePointList[0].Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + 1);
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

            HiLowBarItem bar = pane.AddHiLowBar("Ожидание", dpmPointList[0], Color.Black);//задаем цвет и название(опционально) каждого массива точек
            bar.Bar.Fill = new Fill(Color.Gray);//работа машины (wait)

            HiLowBarItem bar1 = pane.AddHiLowBar("Запись", bufWritePointList[0], Color.Red);
            bar1.Bar.Fill = new Fill(Color.Yellow);//запись в буфер

            HiLowBarItem ba2 = pane.AddHiLowBar("Чтение", bufReadPointList[0], Color.LightGreen);
            ba2.Bar.Fill = new Fill(Color.FromArgb(146, 208, 80));//чтение из буфера

            HiLowBarItem bar3 = pane.AddHiLowBar("Ошибка", bePointList[0], Color.Red);
            bar3.Bar.Fill = new Fill(Color.Red);//запись в буфер

            for (int lbl = 0; lbl < dpmPointList[0].Count; lbl++)
            {
                lablesList.Add(new TextObj(Convert.ToString(bar.Points[lbl].Z - bar.Points[lbl].X), bar.Points[lbl].X + (bar.Points[lbl].Z - bar.Points[lbl].X) / 2, bar.Points[lbl].Y));
                lablesList[lbl].FontSpec.Fill = new Fill(Color.Gray);//цвет фона текста
                lablesList[lbl].FontSpec.Size = 11;//размер
                lablesList[lbl].FontSpec.Border.IsVisible = false;//выделение раниц
                pane.GraphObjList.Add(lablesList[lbl]);//добавить текст
            }
            for (int lbl = 0; lbl < bufWritePointList[0].Count; lbl++)
            {
                lablesList1.Add(new TextObj(Convert.ToString(bar1.Points[lbl].Z - bar1.Points[lbl].X), bar1.Points[lbl].X + (bar1.Points[lbl].Z - bar1.Points[lbl].X) / 2, bar1.Points[lbl].Y));
                lablesList1[lbl].FontSpec.Fill = new Fill(Color.Yellow);//цвет фона текста
                lablesList1[lbl].FontSpec.Size = 11;//размер
                lablesList1[lbl].FontSpec.Border.IsVisible = false;//выделение раниц
                pane.GraphObjList.Add(lablesList1[lbl]);//добавить текст
            }
            for (int lbl = 0; lbl < bufReadPointList[0].Count; lbl++)
            {
                lablesList2.Add(new TextObj(Convert.ToString(ba2.Points[lbl].Z - ba2.Points[lbl].X), ba2.Points[lbl].X + (ba2.Points[lbl].Z - ba2.Points[lbl].X) / 2, ba2.Points[lbl].Y));
                lablesList2[lbl].FontSpec.Fill = new Fill(Color.FromArgb(146, 208, 80));//цвет фона текста
                lablesList2[lbl].FontSpec.Size = 11;//размер 
                lablesList2[lbl].FontSpec.Border.IsVisible = false;//выделение раниц
                pane.GraphObjList.Add(lablesList2[lbl]);//добавить текст
            }
            textBox2.Text += "Выбран вариант агрегации №" + (drowIdx + 1) + " при котором:" + "\n";
            for (int agr = 0; agr < pereborList[drowIdx].comList.Count; agr++)//обходим все коммуникаторы
            {
                textBox2.Text += "\r\nКомуникатор " + (agr + 1) + " реализует буферы: ";
                for (int bf = 0; bf < pereborList[drowIdx].comList[agr].comBufList.Count; bf++)
                {
                    textBox2.Text += (pereborList[drowIdx].comList[agr].comBufList[bf] + 1) + " ";
                }
                textBox2.Text += "\n";
            }
            textBox2.Text += "\r\nКол-во обменов: " + pereborList[drowIdx].exchangeCount;
            Graphics g = this.CreateGraphics();//вывод графика
            pane.AxisChange(g);
            g.Dispose();
            zedGraphControl1.Refresh();

            //вывод лога
            StringBuilder sb = new StringBuilder();
            string filePath = @"I:\Dplom\";
            File.AppendAllText(filePath + "log.txt", sb.ToString());
            sb.Append("--------------------------------------------------------\r\n");
            sb.Append(DateTime.Now.ToString("dd MMMM yyyy | HH:mm:ss"));
            sb.Append("\r\nВсего " + agregNumb + " вариантов агрегации");
            for (int s = 0; s < agregNumb; s++)
            {
                sb.Append("\r\nВариант агрегации №" + (s + 1));
                for (int cm = 0; cm < pereborList[s].comList.Count; cm++)
                {
                    sb.Append("\r\nКомуникатор " + (cm + 1) + " реализует буферы: ");
                    for (int bf = 0; bf < pereborList[s].comList[cm].comBufList.Count; bf++)
                    {
                        sb.Append((pereborList[s].comList[cm].comBufList[bf] + 1) + " ");
                    }
                }
                sb.Append("\r\nКол-во совершенных обменов: " + pereborList[s].exchangeCount);
                sb.Append("\n");
            }

            File.AppendAllText(filePath + "log.txt", sb.ToString());
            sb.Clear();//очистка памяти
        }

        private void parallel()
        {
            clearData();
            //моделирование когда число коммуникаторов = число буферов
            //первый этап, каждый внешний буфер реализуется отдельным коммуникатором
            List<nedogruzMod> comList = new List<nedogruzMod>();//список коммуникаторов
            for (int i = 0; i < bufList.Count; i++)
            {//присвоение коммуникаторам буферов
                comList.Add(new nedogruzMod());//создаем коммуникатор
                comList[i].comBufList.Add(i);//добавляем в него буфер
            }
            //подключенные к коммуникатору машины
            for (int j = 0; j < comList.Count; j++)//для каждого коммуникатора
            {
                for (int k = 0; k < comList[j].comBufList.Count; k++)//каждый коммуникатор реализаует минимум один буфер, обходим все
                {
                    if (!comList[j].connectedDPMs.Exists((x) => x == bufList[comList[j].comBufList[k]].input))
                    {
                        comList[j].connectedDPMs.Add(bufList[comList[j].comBufList[k]].input);
                    }
                    if (!comList[j].connectedDPMs.Exists((x) => x == bufList[comList[j].comBufList[k]].output))
                    {
                        comList[j].connectedDPMs.Add(bufList[comList[j].comBufList[k]].output);
                    }
                }
            }
            for (int circle = 0; circle < 1000; circle++)//цикл в котором происходит моделирование
            {
                //первый этап, машины выставляют заявки
                for (int j = 0; j < dpmList.Count; j++)//обход каждой машины
                {
                    //проверка на повторное исполнение команд
                    if (dpmList[j].currentCommand >= dpmList[j].dpmCommandList.Count)
                    {
                        dpmList[j].currentCommand = dpmList[j].repeatComNum;//если машина выполнила все инструкции, начинает работать с повторяемой
                    }
                    if (dpmList[j].exchangeReady == false)//если машина не выставила заявку на обмен
                    {
                        switch (dpmList[j].dpmCommandList[dpmList[j].currentCommand].commandType)//проверяем тип текущей команды машины
                        {
                            case "wait"://если ожидание
                                dpmList[j].currentTime += dpmList[j].dpmCommandList[dpmList[j].currentCommand].waitTime;//если инструкция ждать, машина просто ждет
                                dpmList[j].currentCommand++;//Выполнили команду, переходим к следующей
                                break;
                            case "write"://если запись
                            case "read"://если чтение
                                dpmList[j].exchangeReady = true;//заявка на обмен выставлена
                                break;
                        }
                    }
                }
                //Этап второй, коммуникаторы обрабатывают заявки
                for (int k = 0; k < comList.Count; k++)//обход каждого коммуникатора
                {
                    int stp = 0;
                    int min = 0;
                    int minIdx = 0;
                    //опрос коммуникатором машины
                    for (int l = 0; l < comList[k].connectedDPMs.Count; l++)//каждый коммуникатор проверяет связанные с ним машины
                    {
                        //если машина готова к обмену и хочет использовать именно этот коммуникатор и не заблокирована
                        if ((dpmList[comList[k].connectedDPMs[l]].exchangeReady == true) && (comList[k].comBufList.Contains(dpmList[comList[k].connectedDPMs[l]].dpmCommandList[dpmList[comList[k].connectedDPMs[l]].currentCommand].destination)) && (dpmList[comList[k].connectedDPMs[l]].isBlocked == false))
                        {
                            //ищем минимальную выставленную заявку
                            if (stp == 0)//если это первая готовая машина
                            {
                                minIdx = comList[k].connectedDPMs[l];//запоминаем индекс
                                min = dpmList[comList[k].connectedDPMs[l]].currentTime;//запоминаем время
                                stp++;
                            }
                            else
                            {
                                if (dpmList[comList[k].connectedDPMs[l]].currentTime < min)
                                {
                                    minIdx = comList[k].connectedDPMs[l];//запоминаем индекс
                                    min = dpmList[comList[k].connectedDPMs[l]].currentTime;//запоминаем время
                                }
                            }
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
                                    //проверить параллельность
                                    if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange > dpmList[minIdx].currentTime)
                                    {//если данные в буфере появились позже чем текущее время машины. "догоняем"
                                     //эмулируем неудачный обмен
                                     //bePointList[i].Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + 1);
                                        dpmList[minIdx].currentTime = bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange;
                                    }

                                    //в буфер записались данные
                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].dataInBuf += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                    //добавляем на график (curTime/bufNum/dataSize)
                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].dataChangedList.Add(dpmList[minIdx].currentTime);
                                    //время машины сдвигаем
                                    dpmList[minIdx].currentTime += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                    //сохраняем время когда данные есть
                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange = dpmList[minIdx].currentTime;
                                    //двигаем все остальные буферы подключенные к коммуникатору на объем данных
                                    for (int s = 0; s < comList[k].comBufList.Count; s++)
                                    {
                                        if (s != dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination)
                                        {  //кроме текущего
                                           //bufList[pereborList[i].comList[k].comBufList[s]].lastDataChange += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                            bufList[comList[k].comBufList[s]].lastDataChange = dpmList[minIdx].currentTime;
                                        }
                                    }
                                    //проверям можно ли разблокировать какую-то машину
                                    if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum != -1)
                                    {
                                        //разблокируем машина
                                        dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].isBlocked = false;
                                        //нужно продвинуть время машины
                                        //Новое текущее время заблоченной машины = curTime разлокрирубщей - curTime заблокированной
                                        //   dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime += (dpmList[minIdx].currentTime - dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime);
                                        //очищаем номер
                                        bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum = -1;
                                    }
                                    //переключаем команду дпм
                                    dpmList[minIdx].currentCommand++;
                                    //снимаем заявку на обмен
                                    dpmList[minIdx].exchangeReady = false;
                                    //фиксируем удачный обмен
                                    comList[k].performance++;
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
                                    //проверить параллельность
                                    if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange > dpmList[minIdx].currentTime)
                                    { //если данные в буфере появились позже чем текущее время машины. "догоняем"
                                      //эмулируем неудачный обмен
                                      //bePointList[i].Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + 1);
                                        dpmList[minIdx].currentTime = bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange;
                                    }

                                    //в буфере удаляются данные
                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].dataInBuf -= dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                    //добавляем на график (curTime/bufNum/dataSize)
                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].dataChangedList.Add(dpmList[minIdx].currentTime);
                                    //время машины сдвигаем
                                    dpmList[minIdx].currentTime += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                    //сохраняем время когда данные есть
                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange = dpmList[minIdx].currentTime;
                                    //двигаем все остальные буферы подключенные к коммуникатору на объем данных
                                    for (int s = 0; s < comList[k].comBufList.Count; s++)
                                    {
                                        if (s != dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination)
                                        {   //кроме текущего
                                            //bufList[pereborList[i].comList[k].comBufList[s]].lastDataChange += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                            bufList[comList[k].comBufList[s]].lastDataChange = dpmList[minIdx].currentTime;
                                        }
                                    }
                                    //проверям можно ли разблокировать какую-то машину
                                    if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum != -1)
                                    {
                                        //разблокируем машина
                                        dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].isBlocked = false;
                                        //нужно продвинуть время машины
                                        //Новое текущее время заблоченной машины = curTime разлокрирубщей - curTime заблокированной
                                        //   dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime += (dpmList[minIdx].currentTime - dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime);
                                        //очищаем номер
                                        bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum = -1;
                                    }
                                    //переключаем команду дпм
                                    dpmList[minIdx].currentCommand++;
                                    //снимаем заявку на обмен
                                    dpmList[minIdx].exchangeReady = false;
                                    //фиксируем удачный обмен
                                    comList[k].performance++;
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
            //подсчет числа параллельных команд каждого с каждым
            for(int i = 0; i < bufList.Count; i++)//для каждого буфера
            {
                for(int j = 0; j < bufList.Count; j++)//сравниваем с другими буферами
                {
                    if (i != j)//кроме самого себя
                    {
                        bufList[i].allParallelsList.Add(new bufClass.parall());
                        bufList[i].allParallelsList[bufList[i].allParallelsList.IndexOf(bufList[i].allParallelsList.Last())].oposeBuFNum = j;
                        //число совпадающих элементов
                        bufList[i].allParallelsList[bufList[i].allParallelsList.IndexOf(bufList[i].allParallelsList.Last())].totalParallel = bufList[i].dataChangedList.Intersect(bufList[j].dataChangedList).Count();
                        //получаем сколько параллельно сполняемых команд у буферов относительно друг друга
                        //if (j > i)//если без повторов
                        {
                        }
                      
                    }
                }
            }

            //заполнение списка коммуникаторов
            int reqComNum = Convert.ToInt32(textBox1.Text);//требуесое кол-во коммуникаторов
            int comToDelete = comList.Count - reqComNum;
            for(int i = 0; i < comToDelete; i++)//распределяем буферы из лишних коммнуникаторов по коммуникаторам
            {
                List<int> tmpBufLst = new List<int>();
                tmpBufLst.Clear();
                for (int sz = 0; sz < comList[comList.IndexOf(comList.Last())].comBufList.Count; sz++)
                {//сохраняем реализуемые этим коммуникатором буферы
                    tmpBufLst.Add(comList[comList.IndexOf(comList.Last())].comBufList[sz]);
                }
                comList.RemoveAt(comList.IndexOf(comList.Last()));//удаляем коммуникатор
                for (int j = 0; j < tmpBufLst.Count; j++)
                {//проверяем список буферов в последнем коммуникаторе
                    List<int> minPar = new List<int>();
                    minPar.Clear();
                    //проверяем параллельность с другими коммуникаторами
                    for (int m = 0; m < comList.Count; m++)
                    {
                        int tmp = 0;
                        for(int v = 0; v < comList[m].comBufList.Count; v++)
                        {
                            tmp += bufList[tmpBufLst[j]].allParallelsList[bufList[tmpBufLst[j]].allParallelsList.IndexOf(bufList[tmpBufLst[j]].allParallelsList.Find(x => x.oposeBuFNum == comList[m].comBufList[v]))].totalParallel;
                        }
                        minPar.Add(tmp);
                    }
                    //находим коммуникатор в котором найденный буфер
                    int comToStoreIdx = minPar.IndexOf(minPar.Min());

                    //добавляем буфер в новый коммуникатор
                    comList[comToStoreIdx].comBufList.Add(tmpBufLst[j]);
                    //добавляем машины в новый коммуникатор
                    if (!comList[comToStoreIdx].connectedDPMs.Contains(bufList[tmpBufLst[j]].input))
                    {
                        comList[comToStoreIdx].connectedDPMs.Add(bufList[tmpBufLst[j]].input);//добавляем машины в новый коммуникатор
                    }
                    if (!comList[comToStoreIdx].connectedDPMs.Contains(bufList[tmpBufLst[j]].output))
                    {
                        comList[comToStoreIdx].connectedDPMs.Add(bufList[tmpBufLst[j]].output);//добавляем машины в новый коммуникатор
                    }
                    //удаляем буфер и


                    //comList[comList.IndexOf(comList.Last())].comBufList.Remove(comList[comList.IndexOf(comList.Last())].comBufList[j]);
                    for ( int s= 0; s < comList.Count; s++)
                    {
                        comList[s].connectedDPMs.Sort();
                        comList[s].comBufList.Sort();
                    }
                }
            }
            //моделирование
            for (int i = 0; i < comList.Count; i++)
            {
                comList[i].connectedDPMs.Sort();
                comList[i].comBufList.Sort();
            }
            int pfmc = 0;
            PointPairList dpmPointList = new PointPairList();//
            PointPairList bufReadPointList = new PointPairList();//Массивы точек для каждой итерации
            PointPairList bufWritePointList = new PointPairList();//
            PointPairList bePointList = new PointPairList();//

            List<TextObj> lablesList = new List<TextObj>();
            List<TextObj> lablesList1 = new List<TextObj>();
            List<TextObj> lablesList2 = new List<TextObj>();
            //сбрасываем общие настройки
            for (int sbs = 0; sbs < dpmList.Count; sbs++)
            {
                dpmList[sbs].exchangeReady = false;//При старте новой агрегации, данные в машинах сбрасываются
                dpmList[sbs].currentTime = 0;
                dpmList[sbs].currentCommand = 0;
                dpmList[sbs].isBlocked = false;
            }
            for (int sbs = 0; sbs < bufList.Count; sbs++)//сброс данных для буферов
            {
                bufList[sbs].blockedDPMnum = -1;
                bufList[sbs].dataInBuf = 0;
                bufList[sbs].lastDataChange = 0;
            }
            //второй этап промоделировать до определенного момента
            for (int circle = 0; circle < 1000; circle++)//цикл в котором происходит моделирование
            {
                //первый этап, машины выставляют заявки
                for (int j = 0; j < dpmList.Count; j++)//обход каждой машины
                {
                    //проверка на повторное исполнение команд
                    if (dpmList[j].currentCommand >= dpmList[j].dpmCommandList.Count)
                    {
                        dpmList[j].currentCommand = dpmList[j].repeatComNum;//если машина выполнила все инструкции, начинает работать с повторяемой
                    }
                    if (dpmList[j].exchangeReady == false)//если машина не выставила заявку на обмен
                    {
                        switch (dpmList[j].dpmCommandList[dpmList[j].currentCommand].commandType)//проверяем тип текущей команды машины
                        {
                            case "wait"://если ожидание
                                dpmPointList.Add(dpmList[j].currentTime, j + 1, dpmList[j].currentTime + dpmList[j].dpmCommandList[dpmList[j].currentCommand].waitTime);
                                dpmList[j].currentTime += dpmList[j].dpmCommandList[dpmList[j].currentCommand].waitTime;//если инструкция ждать, машина просто ждет
                                dpmList[j].currentCommand++;//Выполнили команду, переходим к следующей
                                break;
                            case "write"://если запись
                            case "read"://если чтение
                                dpmList[j].exchangeReady = true;//заявка на обмен выставлена
                                break;
                        }
                    }
                }
                //Этап второй, коммуникаторы обрабатывают заявки
                for (int k = 0; k < comList.Count; k++)//обход каждого коммуникатора
                {
                    int stp = 0;
                    int min = 0;
                    int minIdx = 0;
                    //опрос коммуникатором машины
                    for (int l = 0; l < comList[k].connectedDPMs.Count; l++)//каждый коммуникатор проверяет связанные с ним машины
                    {
                        //если машина готова к обмену и хочет использовать именно этот коммуникатор и не заблокирована
                        if ((dpmList[comList[k].connectedDPMs[l]].exchangeReady == true) && (comList[k].comBufList.Contains(dpmList[comList[k].connectedDPMs[l]].dpmCommandList[dpmList[comList[k].connectedDPMs[l]].currentCommand].destination)) && (dpmList[comList[k].connectedDPMs[l]].isBlocked == false))
                        {
                            //ищем минимальную выставленную заявку
                            if (stp == 0)//если это первая готовая машина
                            {
                                minIdx = comList[k].connectedDPMs[l];//запоминаем индекс
                                min = dpmList[comList[k].connectedDPMs[l]].currentTime;//запоминаем время
                                stp++;
                            }
                            else
                            {
                                if (dpmList[comList[k].connectedDPMs[l]].currentTime < min)
                                {
                                    minIdx = comList[k].connectedDPMs[l];//запоминаем индекс
                                    min = dpmList[comList[k].connectedDPMs[l]].currentTime;//запоминаем время
                                }
                            }
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
                                    //проверить параллельность
                                    if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange > dpmList[minIdx].currentTime)
                                    {//если данные в буфере появились позже чем текущее время машины. "догоняем"
                                     //эмулируем неудачный обмен
                                     //bePointList[i].Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + 1);
                                        dpmList[minIdx].currentTime = bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange;
                                    }

                                    //в буфер записались данные
                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].dataInBuf += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                    //добавляем на график (curTime/bufNum/dataSize)
                                    bufWritePointList.Add(dpmList[minIdx].currentTime, -dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination - 1, dpmList[minIdx].currentTime + dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize);
                                    bufWritePointList.Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize);
                                    //время машины сдвигаем
                                    //время машины сдвигаем
                                    dpmList[minIdx].currentTime += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                    //сохраняем время когда данные есть
                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange = dpmList[minIdx].currentTime;
                                    //двигаем все остальные буферы подключенные к коммуникатору на объем данных
                                    for (int s = 0; s < comList[k].comBufList.Count; s++)
                                    {
                                        if (s != dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination)
                                        {  //кроме текущего
                                           //bufList[pereborList[i].comList[k].comBufList[s]].lastDataChange += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                            bufList[comList[k].comBufList[s]].lastDataChange = dpmList[minIdx].currentTime;
                                        }
                                    }
                                    //проверям можно ли разблокировать какую-то машину
                                    if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum != -1)
                                    {
                                        //разблокируем машина
                                        dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].isBlocked = false;
                                        //нужно продвинуть время машины
                                        //Новое текущее время заблоченной машины = curTime разлокрирубщей - curTime заблокированной
                                        //   dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime += (dpmList[minIdx].currentTime - dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime);
                                        //очищаем номер
                                        bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum = -1;
                                    }
                                    //переключаем команду дпм
                                    dpmList[minIdx].currentCommand++;
                                    //снимаем заявку на обмен
                                    dpmList[minIdx].exchangeReady = false;
                                    //фиксируем удачный обмен
                                    pfmc++;
                                }
                                else//неудачная попытка обмена
                                {
                                    bePointList.Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + 1);
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
                                    //проверить параллельность
                                    if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange > dpmList[minIdx].currentTime)
                                    { //если данные в буфере появились позже чем текущее время машины. "догоняем"
                                      //эмулируем неудачный обмен
                                      //bePointList[i].Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + 1);
                                        dpmList[minIdx].currentTime = bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange;
                                    }

                                    //в буфере удаляются данные
                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].dataInBuf -= dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                    //добавляем на график (curTime/bufNum/dataSize)
                                    bufReadPointList.Add(dpmList[minIdx].currentTime, -dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination - 1, dpmList[minIdx].currentTime + dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize);
                                    bufReadPointList.Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize);
                                    //время машины сдвигаем
                                    //время машины сдвигаем
                                    dpmList[minIdx].currentTime += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                    //сохраняем время когда данные есть
                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange = dpmList[minIdx].currentTime;
                                    //двигаем все остальные буферы подключенные к коммуникатору на объем данных
                                    for (int s = 0; s < comList[k].comBufList.Count; s++)
                                    {
                                        if (s != dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination)
                                        {   //кроме текущего
                                            //bufList[pereborList[i].comList[k].comBufList[s]].lastDataChange += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                            bufList[comList[k].comBufList[s]].lastDataChange = dpmList[minIdx].currentTime;
                                        }
                                    }
                                    //проверям можно ли разблокировать какую-то машину
                                    if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum != -1)
                                    {
                                        //разблокируем машина
                                        dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].isBlocked = false;
                                        //нужно продвинуть время машины
                                        //Новое текущее время заблоченной машины = curTime разлокрирубщей - curTime заблокированной
                                        //   dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime += (dpmList[minIdx].currentTime - dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime);
                                        //очищаем номер
                                        bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum = -1;
                                    }
                                    //переключаем команду дпм
                                    dpmList[minIdx].currentCommand++;
                                    //снимаем заявку на обмен
                                    dpmList[minIdx].exchangeReady = false;
                                    //фиксируем удачный обмен
                                    pfmc++;
                                }
                                else//неудачная попытка обмена
                                {
                                    bePointList.Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + 1);
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
            HiLowBarItem bar = pane.AddHiLowBar("Ожидание", dpmPointList, Color.Black);//задаем цвет и название(опционально) каждого массива точек
            bar.Bar.Fill = new Fill(Color.Gray);//работа машины (wait)

            HiLowBarItem bar1 = pane.AddHiLowBar("Запись", bufWritePointList, Color.Red);
            bar1.Bar.Fill = new Fill(Color.Yellow);//запись в буфер

            HiLowBarItem ba2 = pane.AddHiLowBar("Чтение", bufReadPointList, Color.LightGreen);
            ba2.Bar.Fill = new Fill(Color.FromArgb(146, 208, 80));//чтение из буфера

            HiLowBarItem bar3 = pane.AddHiLowBar("Ошибка", bePointList, Color.Red);
            bar3.Bar.Fill = new Fill(Color.Red);//запись в буфер

            for (int lbl = 0; lbl < dpmPointList.Count; lbl++)
            {
                lablesList.Add(new TextObj(Convert.ToString(bar.Points[lbl].Z - bar.Points[lbl].X), bar.Points[lbl].X + (bar.Points[lbl].Z - bar.Points[lbl].X) / 2, bar.Points[lbl].Y));
                lablesList[lbl].FontSpec.Fill = new Fill(Color.Gray);//цвет фона текста
                lablesList[lbl].FontSpec.Size = 11;//размер
                lablesList[lbl].FontSpec.Border.IsVisible = false;//выделение раниц
                pane.GraphObjList.Add(lablesList[lbl]);//добавить текст
            }
            for (int lbl = 0; lbl < bufWritePointList.Count; lbl++)
            {
                lablesList1.Add(new TextObj(Convert.ToString(bar1.Points[lbl].Z - bar1.Points[lbl].X), bar1.Points[lbl].X + (bar1.Points[lbl].Z - bar1.Points[lbl].X) / 2, bar1.Points[lbl].Y));
                lablesList1[lbl].FontSpec.Fill = new Fill(Color.Yellow);//цвет фона текста
                lablesList1[lbl].FontSpec.Size = 11;//размер
                lablesList1[lbl].FontSpec.Border.IsVisible = false;//выделение раниц
                pane.GraphObjList.Add(lablesList1[lbl]);//добавить текст
            }
            for (int lbl = 0; lbl < bufReadPointList.Count; lbl++)
            {
                lablesList2.Add(new TextObj(Convert.ToString(ba2.Points[lbl].Z - ba2.Points[lbl].X), ba2.Points[lbl].X + (ba2.Points[lbl].Z - ba2.Points[lbl].X) / 2, ba2.Points[lbl].Y));
                lablesList2[lbl].FontSpec.Fill = new Fill(Color.FromArgb(146, 208, 80));//цвет фона текста
                lablesList2[lbl].FontSpec.Size = 11;//размер 
                lablesList2[lbl].FontSpec.Border.IsVisible = false;//выделение раниц
                pane.GraphObjList.Add(lablesList2[lbl]);//добавить текст
            }
            textBox2.Text += "Агрегация проведена \n";
            for (int agr = 0; agr < comList.Count; agr++)//обходим все коммуникаторы
            {
                textBox2.Text += "\r\nКомуникатор " + (agr + 1) + " реализует буферы: ";
                for (int bf = 0; bf < comList[agr].comBufList.Count; bf++)
                {
                    textBox2.Text += (comList[agr].comBufList[bf] + 1) + " ";
                }
                textBox2.Text += "\n";
            }
            textBox2.Text += "\r\nКол-во обменов: " + pfmc;
            Graphics g = this.CreateGraphics();//вывод графика
            pane.AxisChange(g);
            g.Dispose();
            zedGraphControl1.Refresh();
        }

        private void nedogruz()
        {
            clearData();
            //первый этап, каждый внешний буфер реализуется отдельным коммуникатором
            List<nedogruzMod> comList = new List<nedogruzMod>();//список коммуникаторов
            for(int i = 0; i < bufList.Count; i++)
            {//присвоение коммуникаторам буферов
                comList.Add(new nedogruzMod());//создаем коммуникатор
                comList[i].comBufList.Add(i);//добавляем в него буфер
            }
            //подключенные к коммуникатору машины
            for (int j = 0; j < comList.Count; j++)//для каждого коммуникатора
            {
                for (int k = 0; k < comList[j].comBufList.Count; k++)//каждый коммуникатор реализаует минимум один буфер, обходим все
                {
                    if (!comList[j].connectedDPMs.Exists((x) => x == bufList[comList[j].comBufList[k]].input))
                    {
                        comList[j].connectedDPMs.Add(bufList[comList[j].comBufList[k]].input);
                    }
                    if (!comList[j].connectedDPMs.Exists((x) => x == bufList[comList[j].comBufList[k]].output))
                    {
                        comList[j].connectedDPMs.Add(bufList[comList[j].comBufList[k]].output);
                    }
                }
            }
            int reqComNum = Convert.ToInt32(textBox1.Text);//требуесое кол-во коммуникаторов
            while (comList.Count != reqComNum)//общий цикл
            {//крутится, пока число коммуникаторов не будет равно заданному
                //сбрасываем общие настройки
                dataReset();
                for (int i = 0; i < comList.Count;i++)
                {
                    comList[i].performance = 0;
                }
                //второй этап промоделировать до определенного момента
                for (int circle = 0; circle < 1000; circle++)//цикл в котором происходит моделирование
                {
                    //первый этап, машины выставляют заявки
                    for (int j = 0; j < dpmList.Count; j++)//обход каждой машины
                    {
                        //проверка на повторное исполнение команд
                        if (dpmList[j].currentCommand >= dpmList[j].dpmCommandList.Count)
                        {
                            dpmList[j].currentCommand = dpmList[j].repeatComNum;//если машина выполнила все инструкции, начинает работать с повторяемой
                        }
                        if (dpmList[j].exchangeReady == false)//если машина не выставила заявку на обмен
                        {
                            switch (dpmList[j].dpmCommandList[dpmList[j].currentCommand].commandType)//проверяем тип текущей команды машины
                            {
                                case "wait"://если ожидание
                                    dpmList[j].currentTime += dpmList[j].dpmCommandList[dpmList[j].currentCommand].waitTime;//если инструкция ждать, машина просто ждет
                                    dpmList[j].currentCommand++;//Выполнили команду, переходим к следующей
                                    break;
                                case "write"://если запись
                                case "read"://если чтение
                                    dpmList[j].exchangeReady = true;//заявка на обмен выставлена
                                    break;
                            }
                        }
                    }
                    //Этап второй, коммуникаторы обрабатывают заявки
                    for (int k = 0; k < comList.Count; k++)//обход каждого коммуникатора
                    {
                        int stp = 0;
                        int min = 0;
                        int minIdx = 0;
                        //опрос коммуникатором машины
                        for (int l = 0; l < comList[k].connectedDPMs.Count; l++)//каждый коммуникатор проверяет связанные с ним машины
                        {
                            //если машина готова к обмену и хочет использовать именно этот коммуникатор и не заблокирована
                            if ((dpmList[comList[k].connectedDPMs[l]].exchangeReady == true) && (comList[k].comBufList.Contains(dpmList[comList[k].connectedDPMs[l]].dpmCommandList[dpmList[comList[k].connectedDPMs[l]].currentCommand].destination)) && (dpmList[comList[k].connectedDPMs[l]].isBlocked == false))
                            {
                                //ищем минимальную выставленную заявку
                                if (stp == 0)//если это первая готовая машина
                                {
                                    minIdx = comList[k].connectedDPMs[l];//запоминаем индекс
                                    min = dpmList[comList[k].connectedDPMs[l]].currentTime;//запоминаем время
                                    stp++;
                                }
                                else
                                {
                                    if (dpmList[comList[k].connectedDPMs[l]].currentTime < min)
                                    {
                                        minIdx = comList[k].connectedDPMs[l];//запоминаем индекс
                                        min = dpmList[comList[k].connectedDPMs[l]].currentTime;//запоминаем время
                                    }
                                }
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
                                        //проверить параллельность
                                        if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange > dpmList[minIdx].currentTime)
                                        {//если данные в буфере появились позже чем текущее время машины. "догоняем"
                                         //эмулируем неудачный обмен
                                         //bePointList[i].Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + 1);
                                            dpmList[minIdx].currentTime = bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange;
                                        }

                                        //в буфер записались данные
                                        bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].dataInBuf += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                        //добавляем на график (curTime/bufNum/dataSize)
                                        //время машины сдвигаем
                                        dpmList[minIdx].currentTime += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                        //сохраняем время когда данные есть
                                        bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange = dpmList[minIdx].currentTime;
                                        //двигаем все остальные буферы подключенные к коммуникатору на объем данных
                                        for (int s = 0; s < comList[k].comBufList.Count; s++)
                                        {
                                            if (s != dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination)
                                            {  //кроме текущего
                                               //bufList[pereborList[i].comList[k].comBufList[s]].lastDataChange += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                                bufList[comList[k].comBufList[s]].lastDataChange = dpmList[minIdx].currentTime;
                                            }
                                        }
                                        //проверям можно ли разблокировать какую-то машину
                                        if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum != -1)
                                        {
                                            //разблокируем машина
                                            dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].isBlocked = false;
                                            //нужно продвинуть время машины
                                            //Новое текущее время заблоченной машины = curTime разлокрирубщей - curTime заблокированной
                                            //   dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime += (dpmList[minIdx].currentTime - dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime);
                                            //очищаем номер
                                            bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum = -1;
                                        }
                                        //переключаем команду дпм
                                        dpmList[minIdx].currentCommand++;
                                        //снимаем заявку на обмен
                                        dpmList[minIdx].exchangeReady = false;
                                        //фиксируем удачный обмен
                                        comList[k].performance++;
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
                                        //проверить параллельность
                                        if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange > dpmList[minIdx].currentTime)
                                        { //если данные в буфере появились позже чем текущее время машины. "догоняем"
                                          //эмулируем неудачный обмен
                                          //bePointList[i].Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + 1);
                                            dpmList[minIdx].currentTime = bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange;
                                        }

                                        //в буфере удаляются данные
                                        bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].dataInBuf -= dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                        //добавляем на график (curTime/bufNum/dataSize)
                                        //время машины сдвигаем
                                        dpmList[minIdx].currentTime += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                        //сохраняем время когда данные есть
                                        bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange = dpmList[minIdx].currentTime;
                                        //двигаем все остальные буферы подключенные к коммуникатору на объем данных
                                        for (int s = 0; s < comList[k].comBufList.Count; s++)
                                        {
                                            if (s != dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination)
                                            {   //кроме текущего
                                                //bufList[pereborList[i].comList[k].comBufList[s]].lastDataChange += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                                bufList[comList[k].comBufList[s]].lastDataChange = dpmList[minIdx].currentTime;
                                            }
                                        }
                                        //проверям можно ли разблокировать какую-то машину
                                        if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum != -1)
                                        {
                                            //разблокируем машина
                                            dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].isBlocked = false;
                                            //нужно продвинуть время машины
                                            //Новое текущее время заблоченной машины = curTime разлокрирубщей - curTime заблокированной
                                            //   dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime += (dpmList[minIdx].currentTime - dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime);
                                            //очищаем номер
                                            bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum = -1;
                                        }
                                        //переключаем команду дпм
                                        dpmList[minIdx].currentCommand++;
                                        //снимаем заявку на обмен
                                        dpmList[minIdx].exchangeReady = false;
                                        //фиксируем удачный обмен
                                        comList[k].performance++;
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
                //поиск самого свободного ком
                int minIdxNum = comList.FindIndex(r => r.performance == comList.Min(x => x.performance));
                List<int> tmpBufLst = new List<int>();
                List<int> tmpDPMLst = new List<int>();
                tmpBufLst.Clear();
                tmpDPMLst.Clear();
                for(int sz = 0;sz < comList[minIdxNum].comBufList.Count; sz++)
                {//сохраняем реализуемые этим коммуникатором буферы
                    tmpBufLst.Add(comList[minIdxNum].comBufList[sz]);
                }
                for(int ds = 0; ds < comList[minIdxNum].connectedDPMs.Count; ds++)
                {
                    tmpDPMLst.Add(comList[minIdxNum].connectedDPMs[ds]);
                }
                comList.RemoveAt(minIdxNum);//удаляем коммуникатор
                //находим следующий наименее загруженный коммуникатор
                int bufToStore = comList.FindIndex(r => r.performance == comList.Min(x => x.performance));
                for (int bl = 0; bl < tmpBufLst.Count; bl++)
                {
                    comList[bufToStore].comBufList.Add(tmpBufLst[bl]);//добавляем буферы в новый коммуникатор
                }
                for (int bl = 0; bl < tmpDPMLst.Count; bl++)
                {
                    if (!comList[bufToStore].connectedDPMs.Contains(tmpDPMLst[bl]))
                    {
                        comList[bufToStore].connectedDPMs.Add(tmpDPMLst[bl]);//добавляем машины в новый коммуникатор
                    }
                }
                for (int i = 0; i < comList.Count; i++)
                {
                    comList[i].connectedDPMs.Sort();
                    comList[i].comBufList.Sort();
                }
            }

            for(int i = 0;i < comList.Count; i++)
            {
                comList[i].connectedDPMs.Sort();
                comList[i].comBufList.Sort();
            }
            int pfmc = 0;
            PointPairList dpmPointList = new PointPairList();//
            PointPairList bufReadPointList = new PointPairList();//Массивы точек для каждой итерации
            PointPairList bufWritePointList = new PointPairList();//
            PointPairList bePointList = new PointPairList();//

            List<TextObj> lablesList = new List<TextObj>();
            List<TextObj> lablesList1 = new List<TextObj>();
            List<TextObj> lablesList2 = new List<TextObj>();
            //сбрасываем общие настройки
            dataReset();
            //второй этап промоделировать до определенного момента
            for (int circle = 0; circle < 1000; circle++)//цикл в котором происходит моделирование
            {
                //первый этап, машины выставляют заявки
                for (int j = 0; j < dpmList.Count; j++)//обход каждой машины
                {
                    //проверка на повторное исполнение команд
                    if (dpmList[j].currentCommand >= dpmList[j].dpmCommandList.Count)
                    {
                        dpmList[j].currentCommand = dpmList[j].repeatComNum;//если машина выполнила все инструкции, начинает работать с повторяемой
                    }
                    if (dpmList[j].exchangeReady == false)//если машина не выставила заявку на обмен
                    {
                        switch (dpmList[j].dpmCommandList[dpmList[j].currentCommand].commandType)//проверяем тип текущей команды машины
                        {
                            case "wait"://если ожидание
                                dpmPointList.Add(dpmList[j].currentTime, j + 1, dpmList[j].currentTime + dpmList[j].dpmCommandList[dpmList[j].currentCommand].waitTime);
                                dpmList[j].currentTime += dpmList[j].dpmCommandList[dpmList[j].currentCommand].waitTime;//если инструкция ждать, машина просто ждет
                                dpmList[j].currentCommand++;//Выполнили команду, переходим к следующей
                                break;
                            case "write"://если запись
                            case "read"://если чтение
                                dpmList[j].exchangeReady = true;//заявка на обмен выставлена
                                break;
                        }
                    }
                }
                //Этап второй, коммуникаторы обрабатывают заявки
                for (int k = 0; k < comList.Count; k++)//обход каждого коммуникатора
                {
                    int stp = 0;
                    int min = 0;
                    int minIdx = 0;
                    //опрос коммуникатором машины
                    for (int l = 0; l < comList[k].connectedDPMs.Count; l++)//каждый коммуникатор проверяет связанные с ним машины
                    {
                        //если машина готова к обмену и хочет использовать именно этот коммуникатор и не заблокирована
                        if ((dpmList[comList[k].connectedDPMs[l]].exchangeReady == true) && (comList[k].comBufList.Contains(dpmList[comList[k].connectedDPMs[l]].dpmCommandList[dpmList[comList[k].connectedDPMs[l]].currentCommand].destination)) && (dpmList[comList[k].connectedDPMs[l]].isBlocked == false))
                        {
                            //ищем минимальную выставленную заявку
                            if (stp == 0)//если это первая готовая машина
                            {
                                minIdx = comList[k].connectedDPMs[l];//запоминаем индекс
                                min = dpmList[comList[k].connectedDPMs[l]].currentTime;//запоминаем время
                                stp++;
                            }
                            else
                            {
                                if (dpmList[comList[k].connectedDPMs[l]].currentTime < min)
                                {
                                    minIdx = comList[k].connectedDPMs[l];//запоминаем индекс
                                    min = dpmList[comList[k].connectedDPMs[l]].currentTime;//запоминаем время
                                }
                            }
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
                                    //проверить параллельность
                                    if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange > dpmList[minIdx].currentTime)
                                    {//если данные в буфере появились позже чем текущее время машины. "догоняем"
                                     //эмулируем неудачный обмен
                                     //bePointList[i].Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + 1);
                                        dpmList[minIdx].currentTime = bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange;
                                    }

                                    //в буфер записались данные
                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].dataInBuf += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                    //добавляем на график (curTime/bufNum/dataSize)
                                    bufWritePointList.Add(dpmList[minIdx].currentTime, -dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination - 1, dpmList[minIdx].currentTime + dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize);
                                    bufWritePointList.Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize);
                                    //время машины сдвигаем
                                    //время машины сдвигаем
                                    dpmList[minIdx].currentTime += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                    //сохраняем время когда данные есть
                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange = dpmList[minIdx].currentTime;
                                    //двигаем все остальные буферы подключенные к коммуникатору на объем данных
                                    for (int s = 0; s < comList[k].comBufList.Count; s++)
                                    {
                                        if (s != dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination)
                                        {  //кроме текущего
                                           //bufList[pereborList[i].comList[k].comBufList[s]].lastDataChange += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                            bufList[comList[k].comBufList[s]].lastDataChange = dpmList[minIdx].currentTime;
                                        }
                                    }
                                    //проверям можно ли разблокировать какую-то машину
                                    if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum != -1)
                                    {
                                        //разблокируем машина
                                        dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].isBlocked = false;
                                        //нужно продвинуть время машины
                                        //Новое текущее время заблоченной машины = curTime разлокрирубщей - curTime заблокированной
                                        //   dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime += (dpmList[minIdx].currentTime - dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime);
                                        //очищаем номер
                                        bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum = -1;
                                    }
                                    //переключаем команду дпм
                                    dpmList[minIdx].currentCommand++;
                                    //снимаем заявку на обмен
                                    dpmList[minIdx].exchangeReady = false;
                                    //фиксируем удачный обмен
                                    pfmc++;
                                }
                                else//неудачная попытка обмена
                                {
                                    bePointList.Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + 1);
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
                                    //проверить параллельность
                                    if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange > dpmList[minIdx].currentTime)
                                    { //если данные в буфере появились позже чем текущее время машины. "догоняем"
                                      //эмулируем неудачный обмен
                                      //bePointList[i].Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + 1);
                                        dpmList[minIdx].currentTime = bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange;
                                    }

                                    //в буфере удаляются данные
                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].dataInBuf -= dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                    //добавляем на график (curTime/bufNum/dataSize)
                                    bufReadPointList.Add(dpmList[minIdx].currentTime, -dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination - 1, dpmList[minIdx].currentTime + dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize);
                                    bufReadPointList.Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize);
                                    //время машины сдвигаем
                                    //время машины сдвигаем
                                    dpmList[minIdx].currentTime += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                    //сохраняем время когда данные есть
                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange = dpmList[minIdx].currentTime;
                                    //двигаем все остальные буферы подключенные к коммуникатору на объем данных
                                    for (int s = 0; s < comList[k].comBufList.Count; s++)
                                    {
                                        if (s != dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination)
                                        {   //кроме текущего
                                            //bufList[pereborList[i].comList[k].comBufList[s]].lastDataChange += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                            bufList[comList[k].comBufList[s]].lastDataChange = dpmList[minIdx].currentTime;
                                        }
                                    }
                                    //проверям можно ли разблокировать какую-то машину
                                    if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum != -1)
                                    {
                                        //разблокируем машина
                                        dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].isBlocked = false;
                                        //нужно продвинуть время машины
                                        //Новое текущее время заблоченной машины = curTime разлокрирубщей - curTime заблокированной
                                        //   dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime += (dpmList[minIdx].currentTime - dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime);
                                        //очищаем номер
                                        bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum = -1;
                                    }
                                    //переключаем команду дпм
                                    dpmList[minIdx].currentCommand++;
                                    //снимаем заявку на обмен
                                    dpmList[minIdx].exchangeReady = false;
                                    //фиксируем удачный обмен
                                    pfmc++;
                                }
                                else//неудачная попытка обмена
                                {
                                    bePointList.Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + 1);
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
            HiLowBarItem bar = pane.AddHiLowBar("Ожидание", dpmPointList, Color.Black);//задаем цвет и название(опционально) каждого массива точек
            bar.Bar.Fill = new Fill(Color.Gray);//работа машины (wait)

            HiLowBarItem bar1 = pane.AddHiLowBar("Запись", bufWritePointList, Color.Red);
            bar1.Bar.Fill = new Fill(Color.Yellow);//запись в буфер

            HiLowBarItem ba2 = pane.AddHiLowBar("Чтение", bufReadPointList, Color.LightGreen);
            ba2.Bar.Fill = new Fill(Color.FromArgb(146, 208, 80));//чтение из буфера

            HiLowBarItem bar3 = pane.AddHiLowBar("Ошибка", bePointList, Color.Red);
            bar3.Bar.Fill = new Fill(Color.Red);//запись в буфер

            for (int lbl = 0; lbl < dpmPointList.Count; lbl++)
            {
                lablesList.Add(new TextObj(Convert.ToString(bar.Points[lbl].Z - bar.Points[lbl].X), bar.Points[lbl].X + (bar.Points[lbl].Z - bar.Points[lbl].X) / 2, bar.Points[lbl].Y));
                lablesList[lbl].FontSpec.Fill = new Fill(Color.Gray);//цвет фона текста
                lablesList[lbl].FontSpec.Size = 11;//размер
                lablesList[lbl].FontSpec.Border.IsVisible = false;//выделение раниц
                pane.GraphObjList.Add(lablesList[lbl]);//добавить текст
            }
            for (int lbl = 0; lbl < bufWritePointList.Count; lbl++)
            {
                lablesList1.Add(new TextObj(Convert.ToString(bar1.Points[lbl].Z - bar1.Points[lbl].X), bar1.Points[lbl].X + (bar1.Points[lbl].Z - bar1.Points[lbl].X) / 2, bar1.Points[lbl].Y));
                lablesList1[lbl].FontSpec.Fill = new Fill(Color.Yellow);//цвет фона текста
                lablesList1[lbl].FontSpec.Size = 11;//размер
                lablesList1[lbl].FontSpec.Border.IsVisible = false;//выделение раниц
                pane.GraphObjList.Add(lablesList1[lbl]);//добавить текст
            }
            for (int lbl = 0; lbl < bufReadPointList.Count; lbl++)
            {
                lablesList2.Add(new TextObj(Convert.ToString(ba2.Points[lbl].Z - ba2.Points[lbl].X), ba2.Points[lbl].X + (ba2.Points[lbl].Z - ba2.Points[lbl].X) / 2, ba2.Points[lbl].Y));
                lablesList2[lbl].FontSpec.Fill = new Fill(Color.FromArgb(146, 208, 80));//цвет фона текста
                lablesList2[lbl].FontSpec.Size = 11;//размер 
                lablesList2[lbl].FontSpec.Border.IsVisible = false;//выделение раниц
                pane.GraphObjList.Add(lablesList2[lbl]);//добавить текст
            }
            textBox2.Text += "Агрегация проведена \n";
            for (int agr = 0; agr < comList.Count; agr++)//обходим все коммуникаторы
            {
                textBox2.Text += "\r\nКомуникатор " + (agr + 1) + " реализует буферы: ";
                for (int bf = 0; bf < comList[agr].comBufList.Count; bf++)
                {
                    textBox2.Text += (comList[agr].comBufList[bf] + 1) + " ";
                }
                textBox2.Text += "\n";
            }
            textBox2.Text += "\r\nКол-во обменов: " + pfmc;
            Graphics g = this.CreateGraphics();//вывод графика
            pane.AxisChange(g);
            g.Dispose();
            zedGraphControl1.Refresh();
        }

        private void sizeOfData()
        {
            clearData();
            List<bool> isEnded = new List<bool>();//прошла ли машина все свои команды
            List<int> isEndedCount = new List<int>();//сколько рабочих циклов каждой машины
            for (int i = 0; i < dpmList.Count; i++)
            {
                isEnded.Add(false);
                isEndedCount.Add(0);
            }
            bool cycleOver = false;//
            //вычисляем рабочий цикл
            while(cycleOver != true)//пока все машины не отработают свой рабочй цикл
            {
                //моделирование
                for(int i = 0; i < dpmList.Count; i++)//Обход каждой машины
                {
                    //проверка на текущую команду
                    if(dpmList[i].currentCommand >= dpmList[i].dpmCommandList.Count)
                    {//если машина выполнила все свои инструкции начинаем с *
                        dpmList[i].currentCommand = dpmList[i].repeatComNum;
                        //фиксируем конец машины
                        isEnded[i] = true;
                        isEndedCount[i]++;
                    }
                    //выполнение команды
                    //определить тип команды
                    switch (dpmList[i].dpmCommandList[dpmList[i].currentCommand].commandType)//
                    {
                        case "write"://если запись
                            //удачно
                            //проверяем свободное место
                            if((bufList[dpmList[i].dpmCommandList[dpmList[i].currentCommand].destination].bufSize - bufList[dpmList[i].dpmCommandList[dpmList[i].currentCommand].destination].dataInBuf) >= dpmList[i].dpmCommandList[dpmList[i].currentCommand].dataSize)
                            {//добавляем данные
                                bufList[dpmList[i].dpmCommandList[dpmList[i].currentCommand].destination].dataInBuf += dpmList[i].dpmCommandList[dpmList[i].currentCommand].dataSize;
                                dpmList[i].currentCommand++;
                            }
                            //неудачно
                            break;
                        case "read"://если чтение
                            //удачно
                            //проверяем наличие данных
                            if (bufList[dpmList[i].dpmCommandList[dpmList[i].currentCommand].destination].dataInBuf >= dpmList[i].dpmCommandList[dpmList[i].currentCommand].dataSize)
                            {//удаляем их
                                bufList[dpmList[i].dpmCommandList[dpmList[i].currentCommand].destination].dataInBuf -= dpmList[i].dpmCommandList[dpmList[i].currentCommand].dataSize;
                                dpmList[i].currentCommand++;
                            }
                            //неудачно
                            break;
                        case "wait"://если ждать
                            //переход к следующей команде
                            dpmList[i].currentCommand++;
                            break;
                    }
                }
                //проверка на завершение
                int cnt = 0;
                for (int i = 0; i < dpmList.Count; i++)
                {
                    if (isEnded[i] == true)
                    {
                        cnt++;
                    }
                }//если все машины выполнили свою программу хотябы 1 раз
                if(cnt == dpmList.Count)
                {
                    cycleOver = true;
                }
            }
            //считаем объем 
            List<int> bufComCount = new List<int>();//кол-во обращений к каждому буферу
            for(int i = 0; i < bufList.Count; i++)
            {
                bufComCount.Add(0);
            }
            int comandsNumber = 0;//общее количество команд
            //нужно посчитать сколько обращений к каждому буферу, учитывая рабочие циклы
            for(int i = 0; i < dpmList.Count; i++)//для каждой машины
            {
                for(int j = 0; j < isEndedCount[i]; j++)//для каждого рабочего цикла
                {
                    for(int k = 0; k < dpmList[i].dpmCommandList.Count; k++)//обходим список команд
                    {
                        if((dpmList[i].dpmCommandList[k].commandType=="write") || (dpmList[i].dpmCommandList[k].commandType == "read"))
                        {
                            comandsNumber++;//общее число команд
                            bufComCount[dpmList[i].dpmCommandList[k].destination]++;//число обращений к конкретному буферу
                        }
                    }
                }
            }
            //теперь посчитать, сколько в процентном соотношении обращений к каждому буферу
            List<int> percentList = new List<int>();
            List<int> tmp = new List<int>();
            for(int i = 0; i < bufComCount.Count; i++)
            {
                percentList.Add(GetPercent(bufComCount[i],comandsNumber));
                tmp.Add(GetPercent(bufComCount[i], comandsNumber));
            }
            //распределяем в процентном соотношении буферы
            int comCount = Convert.ToInt32(textBox1.Text);//кол-во коммуникаторов
            //распределять буферы по коммуникаторам в зависимости от их % важности
            int[] numbers = percentList.ToArray();
            UInt64[] longs = numbers.Select(item => (UInt64)item).ToArray();
            int pilesNumber = comCount;
            int sum = numbers.Sum();/*
            int combNumber =numbers.Aggregate(1, (m, number) => m * pilesNumber);
            UInt64 combNumberLng = (UInt64)longs.Aggregate(1, (m, number) => m * pilesNumber);//число вохможных варантов
            if (combNumber < 0)
            {
                combNumber = combNumber * -1;//pgrll
            }
            var perfectCombination =
                Enumerable.Range(0, combNumber)
                          .Select(x =>
                          {
                              var piles =
                                  Enumerable.Range(0, pilesNumber)
                                            .Select(y => new List<int>(numbers.Length))
                                            .ToArray();
                              foreach (var n in numbers)
                              {
                                  piles[x % pilesNumber].Add(n);
                                  x /= pilesNumber;
                              }
                              return piles;
                          })
                          .MinBy(piles => piles.Sum(pile => Math.Abs(pile.Sum() * pilesNumber - sum)));*/
            List<List<int>> agreg = new List<List<int>>();//вариант агрегации итоговый
            List<int> comPercents = new List<int>();//проценты нагруузки каждого коммуникатора
            for (int cmcnt = 0; cmcnt < comCount; cmcnt++)
            {
                comPercents.Add(0);//стартовая нагрузка коммуникаторов
                agreg.Add(new List<int>());//создание коммуникаторов
            }
            //расрпделение буферов
            for (int pr = 0; pr < percentList.Count; pr++)//обход каждого буфера
            {
                int cmin = comPercents.IndexOf(comPercents.Min());//находим самый свободный коммуникатор
                //увеличиваем нагрузку на ком
                comPercents[cmin] += percentList[pr];
                //добавляем буфер в ком
                agreg[cmin].Add(percentList[pr]);
            }
            //agreg = perfectCombination.First().ToList();//закоментить
            //подключенные к коммуникатору машины
            List<nedogruzMod> comList = new List<nedogruzMod>();//список коммуникаторов
            for(int i = 0; i < comCount; i++)//каждый коммуникатор
            {
                comList.Add(new nedogruzMod());//создаем коммуникатор
                //добавляем в коммуникатор буферы
                for(int j = 0; j < agreg[i].Count; j++)
                {
                    comList[i].comBufList.Add(percentList.IndexOf(agreg[i][j]));//распределили буферы по коммуникаторам
                    percentList[percentList.IndexOf(agreg[i][j])] = -1;
                }
            }
            //добавляем машины
            for (int j = 0; j < comList.Count; j++)//для каждого коммуникатора
            {
                for (int k = 0; k < comList[j].comBufList.Count; k++)//каждый коммуникатор реализаует минимум один буфер, обходим все
                {
                    if (!comList[j].connectedDPMs.Exists((x) => x == bufList[comList[j].comBufList[k]].input))
                    {
                        comList[j].connectedDPMs.Add(bufList[comList[j].comBufList[k]].input);
                    }
                    if (!comList[j].connectedDPMs.Exists((x) => x == bufList[comList[j].comBufList[k]].output))
                    {
                        comList[j].connectedDPMs.Add(bufList[comList[j].comBufList[k]].output);
                    }
                }
            }
            //финальное моделирование
            int pfmc = 0;
            PointPairList dpmPointList = new PointPairList();//
            PointPairList bufReadPointList = new PointPairList();//Массивы точек для каждой итерации
            PointPairList bufWritePointList = new PointPairList();//
            PointPairList bePointList = new PointPairList();//

            List<TextObj> lablesList = new List<TextObj>();
            List<TextObj> lablesList1 = new List<TextObj>();
            List<TextObj> lablesList2 = new List<TextObj>();
            //сбрасываем общие настройки
            for (int sbs = 0; sbs < dpmList.Count; sbs++)
            {
                dpmList[sbs].exchangeReady = false;//При старте новой агрегации, данные в машинах сбрасываются
                dpmList[sbs].currentTime = 0;
                dpmList[sbs].currentCommand = 0;
                dpmList[sbs].isBlocked = false;
            }
            for (int sbs = 0; sbs < bufList.Count; sbs++)//сброс данных для буферов
            {
                bufList[sbs].blockedDPMnum = -1;
                bufList[sbs].dataInBuf = 0;
                bufList[sbs].lastDataChange = 0;
            }
            //второй этап промоделировать до определенного момента
            for (int circle = 0; circle < 1000; circle++)//цикл в котором происходит моделирование
            {
                //первый этап, машины выставляют заявки
                for (int j = 0; j < dpmList.Count; j++)//обход каждой машины
                {
                    //проверка на повторное исполнение команд
                    if (dpmList[j].currentCommand >= dpmList[j].dpmCommandList.Count)
                    {
                        dpmList[j].currentCommand = dpmList[j].repeatComNum;//если машина выполнила все инструкции, начинает работать с повторяемой
                    }
                    if (dpmList[j].exchangeReady == false)//если машина не выставила заявку на обмен
                    {
                        switch (dpmList[j].dpmCommandList[dpmList[j].currentCommand].commandType)//проверяем тип текущей команды машины
                        {
                            case "wait"://если ожидание
                                dpmPointList.Add(dpmList[j].currentTime, j + 1, dpmList[j].currentTime + dpmList[j].dpmCommandList[dpmList[j].currentCommand].waitTime);
                                dpmList[j].currentTime += dpmList[j].dpmCommandList[dpmList[j].currentCommand].waitTime;//если инструкция ждать, машина просто ждет
                                dpmList[j].currentCommand++;//Выполнили команду, переходим к следующей
                                break;
                            case "write"://если запись
                            case "read"://если чтение
                                dpmList[j].exchangeReady = true;//заявка на обмен выставлена
                                break;
                        }
                    }
                }
                //Этап второй, коммуникаторы обрабатывают заявки
                for (int k = 0; k < comList.Count; k++)//обход каждого коммуникатора
                {
                    int stp = 0;
                    int min = 0;
                    int minIdx = 0;
                    //опрос коммуникатором машины
                    for (int l = 0; l < comList[k].connectedDPMs.Count; l++)//каждый коммуникатор проверяет связанные с ним машины
                    {
                        //если машина готова к обмену и хочет использовать именно этот коммуникатор и не заблокирована
                        if ((dpmList[comList[k].connectedDPMs[l]].exchangeReady == true) && (comList[k].comBufList.Contains(dpmList[comList[k].connectedDPMs[l]].dpmCommandList[dpmList[comList[k].connectedDPMs[l]].currentCommand].destination)) && (dpmList[comList[k].connectedDPMs[l]].isBlocked == false))
                        {
                            //ищем минимальную выставленную заявку
                            if (stp == 0)//если это первая готовая машина
                            {
                                minIdx = comList[k].connectedDPMs[l];//запоминаем индекс
                                min = dpmList[comList[k].connectedDPMs[l]].currentTime;//запоминаем время
                                stp++;
                            }
                            else
                            {
                                if (dpmList[comList[k].connectedDPMs[l]].currentTime < min)
                                {
                                    minIdx = comList[k].connectedDPMs[l];//запоминаем индекс
                                    min = dpmList[comList[k].connectedDPMs[l]].currentTime;//запоминаем время
                                }
                            }
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
                                    //проверить параллельность
                                    if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange > dpmList[minIdx].currentTime)
                                    {//если данные в буфере появились позже чем текущее время машины. "догоняем"
                                     //эмулируем неудачный обмен
                                     //bePointList[i].Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + 1);
                                        dpmList[minIdx].currentTime = bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange;
                                    }

                                    //в буфер записались данные
                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].dataInBuf += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                    //добавляем на график (curTime/bufNum/dataSize)
                                    bufWritePointList.Add(dpmList[minIdx].currentTime, -dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination - 1, dpmList[minIdx].currentTime + dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize);
                                    bufWritePointList.Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize);
                                    //время машины сдвигаем
                                    //время машины сдвигаем
                                    dpmList[minIdx].currentTime += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                    //сохраняем время когда данные есть
                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange = dpmList[minIdx].currentTime;
                                    //двигаем все остальные буферы подключенные к коммуникатору на объем данных
                                    for (int s = 0; s < comList[k].comBufList.Count; s++)
                                    {
                                        if (s != dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination)
                                        {  //кроме текущего
                                           //bufList[pereborList[i].comList[k].comBufList[s]].lastDataChange += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                            bufList[comList[k].comBufList[s]].lastDataChange = dpmList[minIdx].currentTime;
                                        }
                                    }
                                    //проверям можно ли разблокировать какую-то машину
                                    if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum != -1)
                                    {
                                        //разблокируем машина
                                        dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].isBlocked = false;
                                        //нужно продвинуть время машины
                                        //Новое текущее время заблоченной машины = curTime разлокрирубщей - curTime заблокированной
                                        //   dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime += (dpmList[minIdx].currentTime - dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime);
                                        //очищаем номер
                                        bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum = -1;
                                    }
                                    //переключаем команду дпм
                                    dpmList[minIdx].currentCommand++;
                                    //снимаем заявку на обмен
                                    dpmList[minIdx].exchangeReady = false;
                                    //фиксируем удачный обмен
                                    pfmc++;
                                }
                                else//неудачная попытка обмена
                                {
                                    bePointList.Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + 1);
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
                                    //проверить параллельность
                                    if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange > dpmList[minIdx].currentTime)
                                    { //если данные в буфере появились позже чем текущее время машины. "догоняем"
                                      //эмулируем неудачный обмен
                                      //bePointList[i].Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + 1);
                                        dpmList[minIdx].currentTime = bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange;
                                    }

                                    //в буфере удаляются данные
                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].dataInBuf -= dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                    //добавляем на график (curTime/bufNum/dataSize)
                                    bufReadPointList.Add(dpmList[minIdx].currentTime, -dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination - 1, dpmList[minIdx].currentTime + dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize);
                                    bufReadPointList.Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize);
                                    //время машины сдвигаем
                                    //время машины сдвигаем
                                    dpmList[minIdx].currentTime += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                    //сохраняем время когда данные есть
                                    bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].lastDataChange = dpmList[minIdx].currentTime;
                                    //двигаем все остальные буферы подключенные к коммуникатору на объем данных
                                    for (int s = 0; s < comList[k].comBufList.Count; s++)
                                    {
                                        if (s != dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination)
                                        {   //кроме текущего
                                            //bufList[pereborList[i].comList[k].comBufList[s]].lastDataChange += dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].dataSize;
                                            bufList[comList[k].comBufList[s]].lastDataChange = dpmList[minIdx].currentTime;
                                        }
                                    }
                                    //проверям можно ли разблокировать какую-то машину
                                    if (bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum != -1)
                                    {
                                        //разблокируем машина
                                        dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].isBlocked = false;
                                        //нужно продвинуть время машины
                                        //Новое текущее время заблоченной машины = curTime разлокрирубщей - curTime заблокированной
                                        //   dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime += (dpmList[minIdx].currentTime - dpmList[bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum].currentTime);
                                        //очищаем номер
                                        bufList[dpmList[minIdx].dpmCommandList[dpmList[minIdx].currentCommand].destination].blockedDPMnum = -1;
                                    }
                                    //переключаем команду дпм
                                    dpmList[minIdx].currentCommand++;
                                    //снимаем заявку на обмен
                                    dpmList[minIdx].exchangeReady = false;
                                    //фиксируем удачный обмен
                                    pfmc++;
                                }
                                else//неудачная попытка обмена
                                {
                                    bePointList.Add(dpmList[minIdx].currentTime, minIdx + 1, dpmList[minIdx].currentTime + 1);
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
            HiLowBarItem bar = pane.AddHiLowBar("Ожидание", dpmPointList, Color.Black);//задаем цвет и название(опционально) каждого массива точек
            bar.Bar.Fill = new Fill(Color.Gray);//работа машины (wait)

            HiLowBarItem bar1 = pane.AddHiLowBar("Запись", bufWritePointList, Color.Red);
            bar1.Bar.Fill = new Fill(Color.Yellow);//запись в буфер

            HiLowBarItem ba2 = pane.AddHiLowBar("Чтение", bufReadPointList, Color.LightGreen);
            ba2.Bar.Fill = new Fill(Color.FromArgb(146, 208, 80));//чтение из буфера

            HiLowBarItem bar3 = pane.AddHiLowBar("Ошибка", bePointList, Color.Red);
            bar3.Bar.Fill = new Fill(Color.Red);//запись в буфер

            for (int lbl = 0; lbl < dpmPointList.Count; lbl++)
            {
                lablesList.Add(new TextObj(Convert.ToString(bar.Points[lbl].Z - bar.Points[lbl].X), bar.Points[lbl].X + (bar.Points[lbl].Z - bar.Points[lbl].X) / 2, bar.Points[lbl].Y));
                lablesList[lbl].FontSpec.Fill = new Fill(Color.Gray);//цвет фона текста
                lablesList[lbl].FontSpec.Size = 11;//размер
                lablesList[lbl].FontSpec.Border.IsVisible = false;//выделение раниц
                pane.GraphObjList.Add(lablesList[lbl]);//добавить текст
            }
            for (int lbl = 0; lbl < bufWritePointList.Count; lbl++)
            {
                lablesList1.Add(new TextObj(Convert.ToString(bar1.Points[lbl].Z - bar1.Points[lbl].X), bar1.Points[lbl].X + (bar1.Points[lbl].Z - bar1.Points[lbl].X) / 2, bar1.Points[lbl].Y));
                lablesList1[lbl].FontSpec.Fill = new Fill(Color.Yellow);//цвет фона текста
                lablesList1[lbl].FontSpec.Size = 11;//размер
                lablesList1[lbl].FontSpec.Border.IsVisible = false;//выделение раниц
                pane.GraphObjList.Add(lablesList1[lbl]);//добавить текст
            }
            for (int lbl = 0; lbl < bufReadPointList.Count; lbl++)
            {
                lablesList2.Add(new TextObj(Convert.ToString(ba2.Points[lbl].Z - ba2.Points[lbl].X), ba2.Points[lbl].X + (ba2.Points[lbl].Z - ba2.Points[lbl].X) / 2, ba2.Points[lbl].Y));
                lablesList2[lbl].FontSpec.Fill = new Fill(Color.FromArgb(146, 208, 80));//цвет фона текста
                lablesList2[lbl].FontSpec.Size = 11;//размер 
                lablesList2[lbl].FontSpec.Border.IsVisible = false;//выделение раниц
                pane.GraphObjList.Add(lablesList2[lbl]);//добавить текст
            }
            textBox2.Text += "Агрегация проведена \n";
            for (int agr = 0; agr < comList.Count; agr++)//обходим все коммуникаторы
            {
                textBox2.Text += "\r\nКомуникатор " + (agr + 1) + " реализует буферы: ";
                for (int bf = 0; bf < comList[agr].comBufList.Count; bf++)
                {
                    textBox2.Text += (comList[agr].comBufList[bf] + 1) + " ";
                }
                textBox2.Text += "\n";
            }
            textBox2.Text += "\r\nКол-во обменов: " + pfmc;
            Graphics g = this.CreateGraphics();//вывод графика
            pane.AxisChange(g);
            g.Dispose();
            zedGraphControl1.Refresh();
        }

        private void dataReset()//сброс данных для машин и буферов
        {
            for (int sbs = 0; sbs < dpmList.Count; sbs++)//сброс данных для машин
            {
                dpmList[sbs].exchangeReady = false;
                dpmList[sbs].currentTime = 0;
                dpmList[sbs].currentCommand = 0;
                dpmList[sbs].isBlocked = false;
            }
            for (int sbs = 0; sbs < bufList.Count; sbs++)//сброс данных для буферов
            {
                bufList[sbs].blockedDPMnum = -1;
                bufList[sbs].dataInBuf = 0;
                bufList[sbs].lastDataChange = 0;
            }
        }

        public static Int32 GetPercent(Int32 a, Int32 b)//получить процент от числа
        {
            if (b == 0) return 0;

            return (Int32)(a / (b / 100M));
        }

        public static class Partitioning//Класс разбиений
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

        public static int GetStarling(int n, int k)//получить число стирлинга второго рода
        {
            if (n == k)
                return 1;  // S(n,n) = 1
            if (n <= 0 || k <= 0 || n < k)
                return 0;  // S(n, 0) = 0; S(0, k) = 0
            return GetStarling(n - 1, k - 1) + k * GetStarling(n - 1, k);
        }

        private void button3_Click(object sender, EventArgs e)//по нажатию моделировать
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
                    if(Convert.ToInt32(textBox1.Text) > bufList.Count)
                    {
                        MessageBox.Show("Количество коммуникаторов не может превосходить количество буферов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                            case "Удалять недогруженные":
                                if (textBox1.TextLength == 0)
                                {
                                    MessageBox.Show("Введите кол-во коммуникаторов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    break;
                                }
                                else
                                {
                                    nedogruz();
                                    break;
                                }
                            case "sizeOfData":
                                if (textBox1.TextLength == 0)
                                {
                                    MessageBox.Show("Введите кол-во коммуникаторов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    break;
                                }
                                else
                                {
                                    sizeOfData();
                                    break;
                                }
                            case "connections":
                                if (textBox1.TextLength == 0)
                                {
                                    MessageBox.Show("Введите кол-во коммуникаторов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    break;
                                }
                                else
                                {
                                    dpmconnections();
                                    break;
                                }
                            case "parallel":
                                if (textBox1.TextLength == 0)
                                {
                                    MessageBox.Show("Введите кол-во коммуникаторов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    break;
                                }
                                else
                                {
                                    parallel();
                                    break;
                                }
                            case "dpmcenter":
                                if (textBox1.TextLength == 0)
                                {
                                    MessageBox.Show("Введите кол-во коммуникаторов", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    break;
                                }
                                else
                                {
                                    dpmcenter();
                                    break;
                                }
                        }
                    }
                }
            }
        }

        private void clearData()
        {
            zedGraphControl1.GraphPane.CurveList.Clear();
            zedGraphControl1.GraphPane.GraphObjList.Clear();
            textBox2.Clear();
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)//ввод числа коммуникаторов
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
