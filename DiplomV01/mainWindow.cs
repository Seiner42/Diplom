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

namespace DiplomV01
{
    public partial class mainWindow : Form
    {
        OpenFileDialog openFile = new OpenFileDialog();
        String line = "";
        List<dpmClass> dpmList = new List<dpmClass>();
        public mainWindow()
        {
            InitializeComponent();
        }

        private void mainWindow_Load(object sender, EventArgs e)
        {
            openFile.Filter = "Text files (.txt)| *.txt";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                StreamReader sr = new StreamReader(openFile.FileName);
                int dpmNumb = 0;
                while (line != null)
                {
                    line = sr.ReadLine();
                    if (line != null)
                    {
                        string[] words = line.Split(' ');
                        switch (words[0][0])
                        {
                            case 'P':
                                dpmList.Add(new dpmClass());
                                for(int i = 0; i < 2; i++)
                                {
                                    string[] digits = Regex.Split(words[i+1], @"\D+");
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
                                break;
                            case 'B':
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
    }
}
