using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RGR
{
    // Оголошення класу форми
    public partial class FormGame : Form
    {
        // Оголошення змінних
        private string filePath;
        private List<string> words;
        private string previousWord;
        private FormRules instructionForm;
        private string gameTopic;
        private HashSet<char> exhaustedLetters;
        private Timer countdownTimer; // Таймер для відліку часу
        private int timeLeft; // Залишок часу

        // Конструктор класу Form3
        public FormGame(string filePath, string gameTopic)
        {
            InitializeComponent();
            this.filePath = filePath;
            this.gameTopic = gameTopic;
            LoadWords(filePath);
            exhaustedLetters = new HashSet<char>();
            this.Load += Form3_Load;

            // Ініціалізація таймера
            countdownTimer = new Timer();
            countdownTimer.Interval = 1000;
            countdownTimer.Tick += CountdownTimer_Tick;
        }

        // Налаштування форми та її елементів
        private void Form3_Load(object sender, EventArgs e)
        {
            label1.Text = "Тема гри: ";
            label2.Text = gameTopic;
            label3.Text = "Попереднє слово: ";
            label4.Text = "";
            label5.Text = "Введіть слово ";
            label6.Text = "";
            label7.Text = "";
            label8.Text = "Список використаних слів:";
            button1.Text = "Правила гри";
            button2.Text = "ОК";
            button3.Text = "На головну";

            // Ініціалізація попереднього слова як порожнього рядка
            previousWord = "";

            // Ініціалізація таймера на початку гри
            StartTimer();
        }

        // Налаштування кнопки для відкриття форми правил гри
        private void button1_Click1(object sender, EventArgs e)
        {
            // Перевірка чи не відкрита форма правил, щоб відкривалася лише одна
            if (instructionForm == null || instructionForm.IsDisposed)
            {
                instructionForm = new FormRules();
                instructionForm.TopMost = true;
            }
            instructionForm.Show();
        }
        
        // Налаштування кнопки для підтвердження введеного слова
        private void button2_Click2(object sender, EventArgs e)
        {
            // Зупинка таймера перед перевіркою слова
            countdownTimer.Stop();

            // Отримання введеного слова
            string userWord = textBox1.Text.Trim();

            if (string.IsNullOrWhiteSpace(userWord))
            {
                StartTimer(); // Перезапуск таймера
                return;
            }

            // Форматування слова користувача для listBox1
            userWord = userWord.First().ToString().ToUpper() + userWord.Substring(1).ToLower();
            
            if (!IsValidWord(userWord))
            {
                StartTimer(); // Перезапуск таймера
                return;
            }

            // Додавання слова користувача до списку використаних слів
            listBox1.Items.Add(userWord);
            label4.Text = userWord;
            previousWord = userWord;

            // Отримання слова від комп'ютера
            string computerWord = GetComputerWord(previousWord);
            if (computerWord == null)
            {
                MessageBox.Show("Комп'ютер не може знайти слово. Ви перемогли!");
                this.Close();
                return;
            }

            // Форматування слова комп'ютера для listBox1
            computerWord = computerWord.First().ToString().ToUpper() + computerWord.Substring(1).ToLower();
            
            // Додавання слова комп'ютера до списку використаних слів
            listBox1.Items.Add(computerWord);
            label4.Text = computerWord;
            previousWord = computerWord;

            // Оновлення наступної літери та очистка текстового поля
            UpdateNextLetterLabel(computerWord);

            textBox1.Clear();
            
            StartTimer();
        }
        
        // Налаштування кнопки для закриття форми
        private void button3_Click(object sender, EventArgs e)
        {
            countdownTimer.Stop();
            this.Close();
        }

        // Обробник подій натискання клавіш в текстовому полі
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && e.KeyChar != '-' && e.KeyChar != ' ' && e.KeyChar != (char)Keys.Back && e.KeyChar != (char)Keys.Delete)
            {
                e.Handled = true;
            }
            
            // 'Натискання' кнопки підтвердження введеного слова при натисканні Enter
            if (e.KeyChar == (char)Keys.Enter)
            {
                button2_Click2(sender, e);
            }
        }
        
        // Метод для завантаження слів з файлу та їх фільтрація
        private void LoadWords(string filePath)
        {
            try
            {
                words = File.ReadAllLines(filePath)
                    .Select(line =>
                        line.Split(new char[] { '(', '.', ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim())
                    .ToList();

                foreach (var word in words)
                {
                    Console.WriteLine(word);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка при завантаженні слів: " + ex.Message);
                words = new List<string>();
            }
        }

        // Метод для збереження нових слів у файл
        private void SaveWordToFile(string word)
        {
            try
            {
                using (StreamWriter sw = File.AppendText(filePath))
                {
                    sw.WriteLine(word);
                }
                words.Add(word);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка при збереженні слова: " + ex.Message);
            }
        }
        
        // Метод для перевірки правильності введеного слова від користувача
        private bool IsValidWord(string word)
        {
            Console.WriteLine($"Перевірка слова:{word}");
            
            // Перевірка чи слово вже було використано
            if (listBox1.Items.Cast<string>()
                .Any(item => string.Equals(item, word, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Це слово вже було використано.");
                return false;
            }

            // Перевірка чи слово є в словнику
            if (!words.Contains(word, StringComparer.OrdinalIgnoreCase))
            {
                var result = MessageBox.Show($"Слово \"{word}\" не знайдено у словнику. Чи це правильне слово?", 
                    "Підтвердження слова", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    SaveWordToFile(word);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            // Перевірка чи слово починається з правильної букви
            if (!string.IsNullOrEmpty(previousWord))
            {
                char lastChar = char.ToLower(previousWord.Last());
                char secondLastChar = char.ToLower(previousWord[previousWord.Length - 2]);
                char thirdLastChar = previousWord.Length > 2 ? char.ToLower(previousWord[previousWord.Length - 3]) : char.MinValue;
                char expectedStartChar = '\0';

                // Якщо остання буква попереднього слова 'ь' або 'и', використовувати передостанню букву
                if (lastChar == 'ь' || lastChar == 'и')
                {
                    lastChar = secondLastChar;
                    secondLastChar = thirdLastChar;
                    thirdLastChar = previousWord.Length > 3 ? char.ToLower(previousWord[previousWord.Length - 4]) : char.MinValue;
                }

                // Перевірка чи буква вже використана
                if (exhaustedLetters.Contains(lastChar))
                {
                    if (!exhaustedLetters.Contains(secondLastChar))
                    {
                        expectedStartChar = secondLastChar;
                    }
                    else if (thirdLastChar != char.MinValue && !exhaustedLetters.Contains(thirdLastChar))
                    {
                        expectedStartChar = thirdLastChar;
                    }
                }
                else
                {
                    expectedStartChar = lastChar;
                }

                // Перевірка чи слово починається з правильної букви
                if (expectedStartChar != '\0' && char.ToLower(word[0]) != expectedStartChar)
                {
                    MessageBox.Show($"Слово повинно починатися з літери '{char.ToUpper(expectedStartChar)}'.");
                    return false;
                }
            }
            return true;
        }

        // Метод для отримання слова від комп'ютера
        private string GetComputerWord(string lastWord)
        {
            char lastChar = char.ToLower(lastWord[lastWord.Length - 1]);
            char secondLastChar = char.ToLower(lastWord[lastWord.Length - 2]);
            char thirdLastChar = lastWord.Length > 2 ? char.ToLower(lastWord[lastWord.Length - 3]) : char.MinValue;
            Random random = new Random(); // Ініціалізація об'єкту для генерації випадкових чисел

            // Пошук слів, що починаються з останньої букви попереднього слова
            var validWords = words.Where(word => char.ToLower(word[0]) == lastChar && !listBox1.Items.Cast<string>()
                .Any(item => string.Equals(item, word, StringComparison.OrdinalIgnoreCase))).ToList();

            if (validWords.Any())
            {
                if (validWords.Count == 1)
                {
                    exhaustedLetters.Add(lastChar);
                    UpdateStatusLabel($"Слова на літеру '{char.ToUpper(lastChar)}' закінчилися.");
                }
                return validWords[random.Next(validWords.Count)]; // Випадковий вибір слова
            }
            else
            {
                exhaustedLetters.Add(lastChar);
                if (!label7.Text.Contains($"Слова на літеру '{char.ToUpper(lastChar)}' закінчилися."))
                {
                    UpdateStatusLabel($"Слова на літеру '{char.ToUpper(lastChar)}' закінчилися.");
                }
            }

            // Пошук слів, що починаються з передостанньої букви
            var alternativeWords = words.Where(word => char.ToLower(word[0]) == secondLastChar && !listBox1.Items
                .Cast<string>()
                .Any(item => string.Equals(item, word, StringComparison.OrdinalIgnoreCase))).ToList();

            if (alternativeWords.Any())
            {
                if (alternativeWords.Count == 1)
                {
                    exhaustedLetters.Add(secondLastChar);
                    UpdateStatusLabel($"Слова на літеру '{char.ToUpper(secondLastChar)}' закінчилися.");
                }
                return alternativeWords[random.Next(alternativeWords.Count)]; // Випадковий вибір слова
            }
            else
            {
                exhaustedLetters.Add(secondLastChar);
                if (!label7.Text.Contains($"Слова на літеру '{char.ToUpper(secondLastChar)}' закінчилися."))
                {
                    UpdateStatusLabel($"Слова на літеру '{char.ToUpper(secondLastChar)}' закінчилися.");
                }
            }

            // Пошук слів, що починаються з третьої з кінця букви
            if (thirdLastChar != char.MinValue)
            {
                var thirdAlternativeWords = words.Where(word => char.ToLower(word[0]) == thirdLastChar && !listBox1
                    .Items.Cast<string>()
                    .Any(item => string.Equals(item, word, StringComparison.OrdinalIgnoreCase))).ToList();

                if (thirdAlternativeWords.Any())
                {
                    if (thirdAlternativeWords.Count == 1)
                    {
                        exhaustedLetters.Add(thirdLastChar);
                        UpdateStatusLabel($"Слова на літеру '{char.ToUpper(thirdLastChar)}' закінчилися.");
                    }
                    return thirdAlternativeWords[random.Next(thirdAlternativeWords.Count)]; // Випадковий вибір слова
                }
                else
                {
                    exhaustedLetters.Add(thirdLastChar);
                    if (!label7.Text.Contains($"Слова на літеру '{char.ToUpper(thirdLastChar)}' закінчилися."))
                    {
                        UpdateStatusLabel($"Слова на літеру '{char.ToUpper(thirdLastChar)}' закінчилися.");
                    }
                }
            }
            return null;
        }

        
        // Метод для оновлення тексту в label7
        private void UpdateStatusLabel(string message)
        {
            // Перевірка чи повідомлення не містить певних фраз і його виведення
            if (!message.Contains("Слова на літеру 'Ь' закінчилися.") && !message.Contains("Слова на літеру 'И' закінчилися."))
            {
                label7.Text += message + "\n";
            }
        }

        // Метод для оновлення наступної літери для слова користувача
        private void UpdateNextLetterLabel(string computerWord)
        {
            char lastChar = char.ToLower(computerWord.Last());
            char secondLastChar = char.ToLower(computerWord[computerWord.Length - 2]);
            char thirdLastChar = computerWord.Length > 2 ? char.ToLower(computerWord[computerWord.Length - 3]) : char.MinValue;
            char nextWordStartChar = '\0';

            // Пошук наступної букви для слова користувача
            if (lastChar != 'ь' && words.Any(word => char.ToLower(word[0]) == lastChar && !exhaustedLetters.Contains(lastChar)))
            {
                nextWordStartChar = lastChar;
            }
            else if (secondLastChar != 'ь' && words.Any(word => char.ToLower(word[0]) == secondLastChar && !exhaustedLetters.Contains(secondLastChar)))
            {
                nextWordStartChar = secondLastChar;
            }
            else if (thirdLastChar != char.MinValue && thirdLastChar != 'ь' && words.Any(word => char.ToLower(word[0]) == thirdLastChar && !exhaustedLetters.Contains(thirdLastChar)))
            {
                nextWordStartChar = thirdLastChar;
            }
            else
            {
                MessageBox.Show("Комп'ютер не може знайти слово. Ви перемогли!");
                this.Close();
                return;
            }
            label6.Text = $"на літеру '{char.ToUpper(nextWordStartChar)}':";
        }

        // Метод для запуску таймера
        private void StartTimer()
        {
            timeLeft = 30;
            label9.Text = $"Залишок часу: {timeLeft} секунд"; 
            countdownTimer.Start();
        }

        // Обробник події таймера
        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            if (timeLeft > 0)
            {
                timeLeft--;
                label9.Text = $"Залишок часу: {timeLeft} секунд";
            }
            else
            {
                countdownTimer.Stop();
                MessageBox.Show("Час на відповідь вийшов! Ви програли.");
                this.Close();
            }
        }
    }
}