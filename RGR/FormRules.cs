using System;
using System.Windows.Forms;

namespace RGR
{
    // Оголошення класу форми
    public partial class FormRules : Form
    {
        // Конструктор класу Form2
        public FormRules()
        {
            InitializeComponent();
        }

        // Налаштування форми та її елементів
        private void Form2_Load(object sender, EventArgs e)
        {
            label1.Text = "Гравець називає слово, а комп'ютер повинен запропонувати інше, що починається з тієї " +
                          "букви, на яку закінчується назване. У випадку якщо слово закінчуеться на 'Ь' або 'И', то потрібно назвати " +
                          "слово на передостанню букву названого. Також для відповіді надається 30с.";
            button1.Text = "ОК";
        }
        
        // Налаштування кнопки для закриття форми
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}