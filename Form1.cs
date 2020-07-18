using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace TYAP_OPZ
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                textBox2.Clear();
                Perevod_texta_v_kod PTVK = new Perevod_texta_v_kod();


                for (int i = 0; i < textBox1.Text.Length; i++)
                    if (textBox1.Text[i] == ':' || textBox1.Text[i] == '=')
                        if (i == textBox1.Text.Length - 1 || i == 0)
                            throw new Exception("Последний или первый символ - это двоеточие, либо равно, что недопустимо");
                        else
                        {
                            if (textBox1.Text[i] == '=' && textBox1.Text[i - 1] == ':' || textBox1.Text[i] == ':' && textBox1.Text[i + 1] == '=')
                                ;
                            else
                                throw new Exception("Неверный формат ввода присвоения");
                        }


                string str = PTVK.Zamena_prisvoenia(textBox1.Text);

                char[] sep = { ';' };
                string[] arr = str.Split(sep);

                for (int i = 0; i < arr.Length - 1; i++)
                {
                    if (arr[i] == "")
                        throw new Exception("Пропущена точка с запятой. Либо введена пустая строка");

                    PTVK.Raschet_OPZ(PTVK.Poluchenie_OPZ(arr[i]));
                }

                for (int i = 0; PTVK.yacheiki_Pamyatis[i].Imya != "НЕ ЛЕЗЬ"; i++)
                {
                    textBox2.Text += PTVK.yacheiki_Pamyatis[i].Imya + " = " + Convert.ToInt32(PTVK.yacheiki_Pamyatis[i].Znachenie) + Environment.NewLine;
                }
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message, "Ошибка");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = ".txt файл (*.txt)|*.txt";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if(saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamWriter streamWriter = new StreamWriter(saveFileDialog1.FileName);
                streamWriter.WriteLine(textBox1.Text);
                streamWriter.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = ".txt файл (*.txt)|*.txt";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamReader streamReader = new StreamReader(openFileDialog1.FileName);
                textBox1.Text = streamReader.ReadToEnd();
                streamReader.Close();
            }
        }
    }

    public class Yacheiki_pamyati
    {
        public bool Znachenie;
        public string Imya;
    }

    public class Perevod_texta_v_kod
    {

        public Yacheiki_pamyati[] yacheiki_Pamyatis = new Yacheiki_pamyati[15];

        public Perevod_texta_v_kod()
        {
            for (int i = 0; i < yacheiki_Pamyatis.Length; i++)
            {
                yacheiki_Pamyatis[i] = new Yacheiki_pamyati();
                yacheiki_Pamyatis[i].Imya = "НЕ ЛЕЗЬ"; // null или "" как показатель отсутствия имени
            }
        }
        
        public void sozdanie_peremennoi(string name, bool s)
        {
            bool k = true;
            int i = 0;
            bool t = true; // триггер попытки изменить переменную

            for(int j = 0; j < yacheiki_Pamyatis.Length; j++)
            {
                if (yacheiki_Pamyatis[j].Imya == name)
                {
                    popitka_izm_per(name, s);
                    t = false;
                    break;
                }
            }

            while (k && t)
            {
                if (yacheiki_Pamyatis[i].Imya == "НЕ ЛЕЗЬ")
                {
                    yacheiki_Pamyatis[i].Imya = name;
                    yacheiki_Pamyatis[i].Znachenie = s;
                    k = false;
                }

                i++;
                if (i == yacheiki_Pamyatis.Length)
                    throw new Exception("Допустимое количество переменных превышено");
            }
        }

        public void popitka_izm_per(string name, bool znach)
        {
            int k = 0;

            for (int i = 0; i < yacheiki_Pamyatis.Length; i++)
            {
                if (yacheiki_Pamyatis[i].Imya == name)
                {
                    yacheiki_Pamyatis[i].Znachenie = znach;
                }
                else
                {
                    k++;
                }
            }

            if (k == yacheiki_Pamyatis.Length)
                throw new Exception("Переменная, которую вы пытаетесь использовать - не объявлена или является константой");
        }

        public string Zamena_prisvoenia(string ish_kod)
        {
            // возможно неоптимально
            return Regex.Replace(ish_kod, "\\s|:", "")
                .ToLower()
                .Replace("xor", "^")
                .Replace("or", "+")
                .Replace("and", "&")
                // только префиксная и распр только на 1 следующий аргумент
                .Replace("not", "~");
        }

        public string Poluchenie_OPZ(string kod)
        {
            int i = 0;
            string itog = "";
            Stack<char> st = new Stack<char>(); // сюда кладем операции и скобки
            string s = ""; // здесь будет вычленено имя переменной, в кт идет присвоение

            if (kod[i] == '=')
                throw new Exception("Не указано имя переменной");

            try
            {
                while (kod[i] != '=')
                {
                    s += kod[i];
                    i++;
                }
            }
            catch
            {
                throw new Exception("Пропущено присвоение");
            }

            // этот стек нужен для "=" и имени переменной (его содержимое выталкивается в самом конце)
            Stack<string> dop_st = new Stack<string>(); 
            dop_st.Push(s);
            dop_st.Push(kod[i].ToString());
            i++;

            // заменил for (; i < kod.Length - 1; i++), т.к. в эту ф-ию 
            // kod приходит уже без ;
            // костыль
            kod += ";";
            for (; i < kod.Length - 1; i++) // for (; i < kod.Length; i++) не нужен, ибо строчка 55
            {
                switch(kod[i])
                {
                    case '~':
                        st.Push('~');
                        break;
                    case '&':       // зачем эта проверка на (? как минимум потому, что ни ( ни ) не должно быть в выходной строке
                        // это (и все, что ниже и выше) полностью соотв условиям, описанным в википедии
                        while (st.Count != 0 &&
                            (st.Peek() == '~' || st.Peek() == '&'))
                            itog += st.Pop() + " ";
                        st.Push('&');
                        break;
                    case '+':
                        while (st.Count != 0 &&
                            (st.Peek() == '~' || st.Peek() == '&' 
                            || st.Peek() == '+' || st.Peek() == '^'))
                            itog += st.Pop() + " ";
                        st.Push('+');
                        break;
                    case '^':
                        while (st.Count != 0 &&
                            (st.Peek() == '~' || st.Peek() == '&'
                            || st.Peek() == '+' || st.Peek() == '^'))
                            itog += st.Pop() + " ";
                        st.Push('^');
                        break;
                    case '(':
                        st.Push('(');
                        break;
                    case ')':
                        while(st.Count != 0 && st.Peek() != '(')
                            itog += st.Pop() + " ";
                        st.Pop();
                        break;
                    default:
                        // это проверка на случай, если число состоит не из одной цифры
                        if (kod[i + 1] != '(' && kod[i + 1] != ')' 
                            && kod[i + 1] != '~' && kod[i + 1] != '&' 
                            && kod[i + 1] != '+' && kod[i + 1] != '^')
                            itog += kod[i];
                        else
                            itog += kod[i] + " ";
                        break;
                }
            }

            try
            {
                if (itog[itog.Length - 1] != ' ')
                    itog += " ";
            }
            catch (IndexOutOfRangeException)
            {
                throw new Exception("В переменную ничего не присваивается");
            }

            while (st.Count != 0)
                itog += st.Pop() + " ";

            itog += dop_st.Pop() + " ";
            while (dop_st.Count != 0)
                itog += dop_st.Pop();

            

            return itog.Trim();
        }

        bool Per_Or_Cifr(string s) // возвращает true, если это константа 0 или 1
        {
            if (s.Length == 1 && (s == "0" || s == "1"))
                return true;

            return false;
        }

        public void Raschet_OPZ(string kod_opz)
        {
            char[] sep = { ' ' };
            List<string> kod_opz_mas = new List<string>(kod_opz.Split(sep));
            bool b = false;

            for (int i = 0; i < kod_opz_mas.Count - 2; i++)
            {
                b = false;

                // проверки на скобки не нужна, ибо код в опз всегда без скобок
                // т.е. если это операция или число, то..
                if (kod_opz_mas[i] == "~" || kod_opz_mas[i] == "&" || kod_opz_mas[i] == "+"
                    || kod_opz_mas[i] == "^" || Per_Or_Cifr(kod_opz_mas[i]))
                    b = true;
                else
                {
                    // теперь помимо проверки здесь заменяем переменные на их значения
                    for (int a = 0; a < yacheiki_Pamyatis.Length; a++)
                        if (yacheiki_Pamyatis[a].Imya == kod_opz_mas[i])
                        {
                            kod_opz_mas[i] = Convert.ToInt32(yacheiki_Pamyatis[a].Znachenie).ToString();
                            b = true;
                            break;
                        }
                }

                if (!b)
                    throw new Exception("Попытка использовать несуществующую переменную. Либо пропущена точка с запятой");

            }

            int j = 0;
            while (kod_opz_mas.Count != 3)
            {
                bool[] a = new bool[2];

                switch (kod_opz_mas[j])
                {
                    case "~":
                        a[0] = Convert.ToBoolean(Convert.ToInt32(kod_opz_mas[j - 1]));    
                        kod_opz_mas[j] = Convert.ToInt32((!a[0])).ToString();
                        kod_opz_mas.RemoveAt(j - 1);
                        break;

                    case "&":
                        a[0] = Convert.ToBoolean(Convert.ToInt32(kod_opz_mas[j - 1])); 
                        a[1] = Convert.ToBoolean(Convert.ToInt32(kod_opz_mas[j - 2])); 
                        kod_opz_mas[j] = Convert.ToInt32((a[0] && a[1])).ToString();
                        kod_opz_mas.RemoveRange(j - 2, 2);
                        j--;
                        break;

                    case "+":
                        a[0] = Convert.ToBoolean(Convert.ToInt32(kod_opz_mas[j - 1])); 
                        a[1] = Convert.ToBoolean(Convert.ToInt32(kod_opz_mas[j - 2])); 
                        kod_opz_mas[j] = Convert.ToInt32((a[0] || a[1])).ToString();
                        kod_opz_mas.RemoveRange(j - 2, 2);
                        j--;
                        break;

                    case "^":
                        a[0] = Convert.ToBoolean(Convert.ToInt32(kod_opz_mas[j - 1])); 
                        a[1] = Convert.ToBoolean(Convert.ToInt32(kod_opz_mas[j - 2])); 
                        kod_opz_mas[j] = Convert.ToInt32((a[0] ^ a[1])).ToString();
                        kod_opz_mas.RemoveRange(j - 2, 2);
                        j--;
                        break;

                    default:
                        j++;
                        break;
                    
                }
            }
            sozdanie_peremennoi(kod_opz_mas[2], Convert.ToBoolean(Convert.ToInt32(kod_opz_mas[0])));
        }
    }
}