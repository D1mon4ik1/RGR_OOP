using System;
using System.IO;
using System.Windows.Forms;

namespace RGR
{
    // Оголошення класу форми
    public partial class FormMenu : Form
    {
        // Оголошення змінних
        private FormRules instructionForm;
        private string selectedTopic;

        // Конструктор класу Form1
        public FormMenu()
        {
            InitializeComponent();
        }

        // Налаштування форми та її елементів
        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = "Гра в слова";
            label2.Text = "Оберіть тему гри:";
            comboBox1.Items.Add("1. Тварини");
            comboBox1.Items.Add("2. Рослини");
            comboBox1.Items.Add("3. Країни");
            comboBox1.Items.Add("4. Міста");
            comboBox1.Items.Add("5. Фрукти й овочі");
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            button1.Text = "Правила гри";
            button2.Text = "Почати гру";
            button3.Text = "Вихід";
            
            // Відображення правил гри при завантаженні форми
            ShowInstructionForm();
        }

        // Обробник події зміни вибраного елемента в комбінованому списку
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedTopic = comboBox1.SelectedItem.ToString();
        }

        // Налаштування кнопки для відкриття форми правил гри
        private void button1_Click(object sender, EventArgs e)
        {
            // Перевірка чи не відкрита форма правил, щоб відкривалася лише одна
            if (instructionForm == null || instructionForm.IsDisposed)
            {
                instructionForm = new FormRules();
                instructionForm.TopMost = true;
            }
            instructionForm.Show();
        }

        // Налаштування кнопки для початку гри
        private void button2_Click(object sender, EventArgs e)
        {
            // Перевірка на обрану тему
            if (string.IsNullOrEmpty(selectedTopic))
            {
                MessageBox.Show("Будь ласка, оберіть тему перед початком гри.");
                return;
            }

            // Отримання файлу словника теми та перевірка його наявності
            string fileName = GetFileNameForSelectedTopic(selectedTopic);
            if (!File.Exists(Path.Combine("topics", fileName)))
            {
                MessageBox.Show("Файл словника для обраної теми не знайдено.");
                return;
            }

            // Видалення номера теми
            string topicWithoutNumber = RemoveNumberFromTopic(selectedTopic);

            // Створення та відображення форми гри і приховання основної форми
            FormGame gameForm = new FormGame(Path.Combine("topics", fileName), topicWithoutNumber);
            gameForm.FormClosed += GameForm_FormClosed;
            gameForm.Show();
            this.Hide();
        }
        
        // Налаштування кнопки для закриття форми
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        // Обробник події відображення основної форми при закритті форми гри
        private void GameForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Show();
        }
        
        // Метод для видалення номера з теми
        private string RemoveNumberFromTopic(string topic)
        {
            return topic.Substring(topic.IndexOf(' ') + 1);
        }
        
        // Метод для відображення форми правил гри
        private void ShowInstructionForm()
        {
            // Перевірка чи не відкрита форма правил, щоб відкривалася лише одна
            if (instructionForm == null || instructionForm.IsDisposed)
            {
                instructionForm = new FormRules();
                instructionForm.TopMost = true;
            }
            instructionForm.Show();
        }

        // Метод для отримання файлу словника для обраної теми
        private string GetFileNameForSelectedTopic(string topic)
        {
            // Повернення відповідного файлу словника залежно від обраної теми
            switch (topic)
            {
                case "1. Тварини":
                    return "animals.txt";
                case "2. Рослини":
                    return "plants.txt";
                case "3. Країни":
                    return "countries.txt";
                case "4. Міста":
                    return "cities.txt";
                case "5. Фрукти й овочі":
                    return "fruits_and_vegetables.txt";
                default:
                    return null;
            }
        }
    }
}
