using Microsoft.Data.Sqlite;
using System.Data;
using System.Drawing;
using System.Text;

internal class Program
{
    private static string ConnectionString = "Data Source=students.sqlite;";

    private static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        using var connection = new SqliteConnection(ConnectionString);

        connection.Open();

        CreateTable(connection);

        CreateItem(connection);

        string[] comands = {
            "SELECT * FROM student_marks",
            "SELECT firstname, lastname FROM student_marks",
            "SELECT midlemark FROM student_marks GROUP BY midlemark",
            "SELECT firstname, lastname, midlemark FROM student_marks WHERE midlemark > 9",
            "SELECT subjectmin FROM student_marks GROUP BY subjectmin",
            "SELECT MIN(midlemark) FROM student_marks",
            "SELECT MAX(midlemark) FROM student_marks",
            "SELECT COUNT(subjectmin) FROM student_marks WHERE subjectmin=\"Математика\"",
            "SELECT COUNT(subjectmin) FROM student_marks WHERE subjectmax=\"Математика\"",
            "SELECT number,  COUNT(number) FROM student_marks GROUP BY number",
            "SELECT number, AVG(midlemark)  FROM student_marks GROUP BY number"
        };


        string[] texts = {
            "Вся інформація про оцінки студентів",
            "ПІБ студентів",
            "Середні оцінки студентів",
            "Студенти, в яких середній бал більше 9",
            "Назви усіх предметів із мінімальними середніми оцінками",
            "Мінімальна середня оцінка",
            "Максимальна середня оцінка",
            "Кількість студентів з мінімальною середньою оцінкою з математики",
            "Кількість студентів з максимальною середньою оцінкою з математики",
            "Кількість студентів у кожній групі",
            "Cередня оцінка групи"
        };

        ReadAndDisplayAll(connection, comands, texts);
    }

    private static void CreateTable(SqliteConnection connection)
    {
        string sql = "create table student_marks (firstname varchar(255), lastname varchar(255), number varchar(255), midlemark int,subjectmin varchar(255),subjectmax varchar(255))";
        var command = connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    private static void CreateItem(SqliteConnection connection)
    {
        using var transaction = connection.BeginTransaction();

        string[,] items = {
    {"Лавренчук", "Артур", "242 Б","9", "Фізика", "Історія"},
    {"Іванов", "Олександр", "210 А","10", "Програмування", "Математика"},
    {"Петрова", "Марія", "305 В","8", "Екологія", "Література"},
    {"Сидоренко", "Олег", "410 Г","9", "Фізика", "Хімія"},
    {"Коваленко", "Оксана", "210 А","7", "Живопис", "Географія"},
    {"Мельник", "Віктор", "305 В","6", "Економіка", "Право"},
    {"Захарченко", "Ірина", "305 В","11", "Музика", "Історія"},
    {"Козлов", "Андрій", "242 Б","9", "Філософія", "Соціологія"},
    {"Павленко", "Поліна", "440 Г","12", "Біологія", "Математика"},
    {"Григоренко", "Валерій", "210 А","7", "Математика", "Самозахист"},
    {"Ковальчук", "Максим", "410 Г","8", "Історія", "Фізика"},
    {"Семененко", "Наталія", "305 В","11", "Математика", "Програмування"},
    {"Зінченко", "Кирило", "440 Г","9", "Інформатика", "Соціологія"},
    {"Остапенко", "Євгенія", "242 Б","10", "Хімія", "Біологія"},
    {"Бондаренко", "Маркіян", "305 В","8", "Фізика", "Історія"},
    {"Литвиненко", "Тетяна", "410 Г","11", "Математика", "Математика"},
    {"Савченко", "Василь", "210 А","9", "Географія", "Музика"},
    {"Мірошниченко", "Віталій", "410 Г","12", "Історія", "Хімія"},
    {"Мельниченко", "Анастасія", "410 Г","10", "Хімія", "Програмування"},
    {"Грищенко", "Вікторія", "440 Г","9", "Інформатика", "Математика"}

};

        for (int i = 0; i < items.GetLength(0); i++)
        {
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = "INSERT INTO student_marks (firstname, lastname, number, midlemark,subjectmin ,subjectmax) VALUES ($firstname, $lastname, $number, $midlemark,$subjectmin ,$subjectmax)";

            string firstname = items[i, 0];
            string lastname = items[i, 1];
            string number = items[i, 2];
            int midlemark = int.Parse(items[i, 3]);
            string subjectmin = items[i, 4];
            string subjectmax = items[i, 5];

            insertCommand.Parameters.AddWithValue("$firstname", firstname);
            insertCommand.Parameters.AddWithValue("$lastname", lastname);
            insertCommand.Parameters.AddWithValue("$number", number);
            insertCommand.Parameters.AddWithValue("$midlemark", midlemark);
            insertCommand.Parameters.AddWithValue("subjectmin", subjectmin);
            insertCommand.Parameters.AddWithValue("subjectmax", subjectmax);

            insertCommand.ExecuteNonQuery();
        }

        transaction.Commit();

    }
    private static void ReadAndDisplayAll(SqliteConnection connection, string[] commands, string[] displayMessages)
    {
        if (commands.Length != displayMessages.Length)
        {
            Console.WriteLine("Кількість команд та повідомлень для відображення не збігається.");
            return;
        }

        using var command = connection.CreateCommand();

        for (int batchIndex = 0; batchIndex < commands.Length; batchIndex++)
        {
            command.CommandText = commands[batchIndex];

            using var reader = command.ExecuteReader();
            Console.WriteLine();
            Console.WriteLine(displayMessages[batchIndex]);
            Console.WriteLine();

            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Console.Write($"{reader.GetString(i),-15}\t");
                }
                Console.WriteLine();
            }
        }
    }


}