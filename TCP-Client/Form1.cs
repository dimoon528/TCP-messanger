using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace TCP_Client
{
    public partial class Form1 : Form
    {
        Dictionary<char, string> cyrillicToIso = new Dictionary<char, string>()
        {
        {'А', "0xC0"}, {'Б', "0xC1"}, {'В', "0xC2"}, {'Г', "0xC3"},
        {'Д', "0xC4"}, {'Е', "0xC5"}, {'Ё', "0xA1"}, {'Ж', "0xC6"},
        {'З', "0xC7"}, {'И', "0xC8"}, {'Й', "0xC9"}, {'К', "0xCA"},
        {'Л', "0xCB"}, {'М', "0xCC"}, {'Н', "0xCD"}, {'О', "0xCE"},
        {'П', "0xCF"}, {'Р', "0xD0"}, {'С', "0xD1"}, {'Т', "0xD2"},
        {'У', "0xD3"}, {'Ф', "0xD4"}, {'Х', "0xD5"}, {'Ц', "0xD6"},
        {'Ч', "0xD7"}, {'Ш', "0xD8"}, {'Щ', "0xD9"}, {'Ъ', "0xDA"},
        {'Ы', "0xDB"}, {'Ь', "0xDC"}, {'Э', "0xDD"}, {'Ю', "0xDE"},
        {'Я', "0xDF"}, {'а', "0xE0"}, {'б', "0xE1"}, {'в', "0xE2"},
        {'г', "0xE3"}, {'д', "0xE4"}, {'е', "0xE5"}, {'ё', "0xB5"},
        {'ж', "0xE6"}, {'з', "0xE7"}, {'и', "0xE8"}, {'й', "0xE9"},
        {'к', "0xEA"}, {'л', "0xEB"}, {'м', "0xEC"}, {'н', "0xED"},
        {'о', "0xEE"}, {'п', "0xEF"}, {'р', "0xF0"}, {'с', "0xF1"},
        {'т', "0xF2"}, {'у', "0xF3"}, {'ф', "0xF4"}, {'х', "0xF5"},
        {'ц', "0xF6"}, {'ч', "0xF7"}, {'ш', "0xF8"}, {'щ', "0xF9"},
        {'ъ', "0xFA"}, {'ы', "0xFB"}, {'ь', "0xFC"}, {'э', "0xFD"},
        {'ю', "0xFE"}, {'я', "0xFF"},
        {'0', "0x30"}, {'1', "0x31"}, {'2', "0x32"}, {'3', "0x33"},
        {'4', "0x34"}, {'5', "0x35"}, {'6', "0x36"}, {'7', "0x37"},
        {'8', "0x38"}, {'9', "0x39"},
        {'.', "0x2E"}, {',', "0x2C"}, {'!', "0x21"}, {'?', "0x3F"},
        {';', "0x3B"}, {':', "0x3A"}, {'-', "0x2D"}, {'(', "0x28"},
        {')', "0x29"}, {'[', "0x5B"}, {']', "0x5D"}, {'^', "0x5E"},
        {'*', "0x2A"}, {'+', "0x2B"}, {'=', "0x3D"}, {'<', "0x3C"},
        {'>', "0x3E"}, {' ', "0x20"}

        };

        Dictionary<string, char> isoToCyrillic = new Dictionary<string, char>()
          {
            {"0xC0", 'А'}, {"0xC1", 'Б'}, {"0xC2", 'В'}, {"0xC3", 'Г'},
            {"0xC4", 'Д'}, {"0xC5", 'Е'}, {"0xA1", 'Ё'}, {"0xC6", 'Ж'},
            {"0xC7", 'З'}, {"0xC8", 'И'}, {"0xC9", 'Й'}, {"0xCA", 'К'},
            {"0xCB", 'Л'}, {"0xCC", 'М'}, {"0xCD", 'Н'}, {"0xCE", 'О'},
            {"0xCF", 'П'}, {"0xD0", 'Р'}, {"0xD1", 'С'}, {"0xD2", 'Т'},
            {"0xD3", 'У'}, {"0xD4", 'Ф'}, {"0xD5", 'Х'}, {"0xD6", 'Ц'},
            {"0xD7", 'Ч'}, {"0xD8", 'Ш'}, {"0xD9", 'Щ'}, {"0xDA", 'Ъ'},
            {"0xDB", 'Ы'}, {"0xDC", 'Ь'}, {"0xDD", 'Э'}, {"0xDE", 'Ю'},
            {"0xDF", 'Я'}, {"0xE0", 'а'}, {"0xE1", 'б'}, {"0xE2", 'в'},
            {"0xE3", 'г'}, {"0xE4", 'д'}, {"0xE5", 'е'}, {"0xB5", 'ё'},
            {"0xE6", 'ж'}, {"0xE7", 'з'}, {"0xE8", 'и'}, {"0xE9", 'й'},
            {"0xEA", 'к'}, {"0xEB", 'л'}, {"0xEC", 'м'}, {"0xED", 'н'},
            {"0xEE", 'о'}, {"0xEF", 'п'}, {"0xF0", 'р'}, {"0xF1", 'с'},
            {"0xF2", 'т'}, {"0xF3", 'у'}, {"0xF4", 'ф'}, {"0xF5", 'х'},
            {"0xF6", 'ц'}, {"0xF7", 'ч'}, {"0xF8", 'ш'}, {"0xF9", 'щ'},
            {"0xFA", 'ъ'}, {"0xFB", 'ы'}, {"0xFC", 'ь'}, {"0xFD", 'э'},
            {"0xFE", 'ю'}, {"0xFF", 'я'},
            { "0x30", '0' }, { "0x31", '1' }, { "0x32", '2' }, { "0x33", '3' },
            { "0x34", '4' }, { "0x35", '5' }, { "0x36", '6' }, { "0x37", '7' },
            { "0x38", '8' }, { "0x39", '9' }, { "0x2E", '.' }, { "0x2C", ',' },
            { "0x21", '!' }, { "0x3F", '?' }, { "0x3B", ';' }, { "0x3A", ':' },
            { "0x2D", '-' }, { "0x28", '(' }, { "0x29", ')' }, { "0x5B", '[' },
            { "0x5D", ']' }, { "0x5E", '^' }, { "0x2A", '*' }, { "0x2B", '+' },
            { "0x3D", '=' }, { "0x3C", '<' }, { "0x3E", '>' }, { "0x20", ' ' }
        };

        // Метод для кодирования текста в ISO 8859-5 и двоичный вид
        public string[] encode(string text)
        {
            // Инициализация массива для хранения результатов: закодированный текст и его двоичное представление
            string[] res = { "", "" };

            // Получение длины строки
            int strLength = text.Length;

            // Перебор каждого символа в строке
            for (int i = 0; i < strLength; i++)
            {
                // Проверка, есть ли символ в словаре cyrillicToIso
                if (cyrillicToIso.ContainsKey(text[i]))
                {
                    // Добавление закодированного символа в первый элемент массива
                    res[0] += cyrillicToIso[text[i]];
                    // Добавление двоичного представления символа во второй элемент массива
                    res[1] += Convert.ToString(text[i], 2);
                    // Добавление пробела после каждого двоичного представления символа
                    res[1] += " ";
                }
            }

            // Возвращение массива с закодированным текстом и его двоичным представлением
            return res;
        }

        // Метод для декодирования текста из ISO 8859-5
        public string decode(string text)
        {
            // Инициализация строки для хранения разделенного текста
            string spltd = "";
            // Получение длины строки
            int strLength = text.Length;

            // Перебор каждого символа в строке
            for (int i = 0; i < strLength; i++)
            {
                // Добавление пробела после каждого четвертого символа
                if (i % 4 == 0)
                {
                    spltd += " ";
                }
                // Добавление символа в строку
                spltd += text[i];
            }

            // Инициализация строки для хранения декодированного текста
            string res = "";
            // Разделение строки на массив подстрок по пробелам
            string[] test = spltd.Split(' ');

            // Перебор каждой подстроки в массиве
            foreach (string i in test)
            {
                // Проверка, есть ли подстрока в словаре isoToCyrillic
                if (isoToCyrillic.ContainsKey(i))
                {
                    // Добавление декодированного символа в строку
                    res += isoToCyrillic[i];
                }
            }

            // Возвращение декодированного текста
            return res;
        }

        // Объявление переменных для хранения IP-адреса сервера, порта сервера и сокета клиента
        string serverIp;
        int serverPort;
        Socket client;

        // Объявление переменной для хранения локального порта
        int localPort = Convert.ToInt32("11006");

        // Объявление переменной для хранения имени пользователя
        string name;

        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            // Объявление переменной для хранения локального IP-адреса
            string localIP;

            // Использование сокета для получения локального IP-адреса
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                // Подключение к удаленному серверу для получения локального IP-адреса
                socket.Connect("8.8.8.8", 65530);
                // Получение локального IP-адреса
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            // Отображение локального IP-адреса в текстовом поле
            textBox5.Text = localIP;
            // Отображение локального порта в текстовом поле
            textBox6.Text = "11006";

            // Создание конечной точки для сервера
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(localIP), localPort);
            // Создание сокета сервера
            Socket server = new Socket(
                ipEndPoint.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            // Привязка сокета сервера к конечной точке
            server.Bind(ipEndPoint);
            // Прослушивание входящих соединений
            server.Listen(10);

            // Принятие входящего соединения
            Socket handler = await server.AcceptAsync();
            // Бесконечный цикл для обработки входящих сообщений
            while (true)
            {
                // Создание буфера для хранения входящего сообщения
                Byte[] buffer = new byte[1024];
                // Получение входящего сообщения
                int message = await handler.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                // Декодирование входящего сообщения
                string encoded = Encoding.UTF8.GetString(buffer, 0, message);

                // Разделение сообщения на части
                string[] test = encoded.Split(new string[] { "0x3A" }, StringSplitOptions.None);

                // Отображение декодированного сообщения в текстовом поле
                textBox3.Text += Environment.NewLine + decode(encoded);

                // Отображение декодированного имени отправителя и сообщения в текстовом поле
                textBox4.Text += Environment.NewLine + decode(test[0]) + ": " + test[1];
                // Отображение декодированного имени отправителя и закодированного сообщения в текстовом поле
                textBox4.Text += Environment.NewLine + decode(test[0]) + ": " + encode(test[1])[1];
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            // Проверка, что имя пользователя не пустое
            if (textBox1.Text.Length > 0)
            {
                // Получение IP-адреса сервера из текстового поля
                serverIp = textBox5.Text;
                // Получение порта сервера из текстового поля
                serverPort = Convert.ToInt32(textBox6.Text);

                // Получение имени пользователя из текстового поля
                name = textBox1.Text;

                // Активация кнопки отправки сообщения
                button1.Enabled = true;

                // Создание конечной точки для клиента
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);

                // Создание сокета клиента
                client = new Socket(
                    ipEndPoint.AddressFamily,
                    SocketType.Stream,
                    ProtocolType.Tcp);

                // Подключение к серверу
                await client.ConnectAsync(ipEndPoint);
            }
        }

        // Обработчик события нажатия кнопки отправки сообщения
        private async void button1_Click(object sender, EventArgs e)
        {
            // Получение сообщения из текстового поля
            string message = textBox2.Text;
            // Кодирование сообщения
            byte[] messageBytes = encode(name + ": " + message)[0].Select(s => (byte)s).ToArray();
            // Отправка сообщения на сервер
            await client.SendAsync(new ArraySegment<byte>(messageBytes), SocketFlags.None);

            // Отображение отправленного сообщения в текстовом поле
            textBox3.Text += Environment.NewLine + name + ": " + message;

            // Отображение закодированного сообщения в текстовом поле
            textBox4.Text += Environment.NewLine + name + ": " + encode(message)[0];
            // Отображение двоичного представления сообщения в текстовом поле
            textBox4.Text += Environment.NewLine + name + ": " + encode(message)[1];
        }
    }
}
