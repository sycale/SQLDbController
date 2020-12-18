Результаты:

Перевел всевозможные функции считывания информации с базы данных и работу с ними в async/await формат по методолгии TAP
вот пример:
```SQL
public static Task<int> AddLog (string Message) {
    return Task.Run (() => {
        using (SqlConnection connection = new SqlConnection (GetConnectionString ())) {
            connection.Open ();
            string query = $"USE LAB; INSERT INTO logs (id, message) VALUES ({Message.GetHashCode() + rnd.Next()}, '{Message.Replace("'", "''")}');";
            SqlCommand command = new SqlCommand(query, connection);
            return command.ExecuteNonQuery();
        }
    });
}
```
В последствии он вызывается вот так:
```C#
static async void HandleError (string ErrorMessage) {
    Console.ForegroundColor = ConsoleColor.DarkRed;
    Console.WriteLine (ErrorMessage);
    Console.ForegroundColor = ConsoleColor.Gray;
    await DataManager.AddLog (ErrorMessage);
}
```
Выполнил потоковое разделение программы, где новый поток запускается для того, чтобы счесть таблицы из базы данных, вот пример:
```C#
Thread UpdateTablesThread = new Thread (new ThreadStart (UpdateTables));
```
В последствии вызывается, соответственно, так:
```C#
UpdateTablesThread.Start();
UpdateTablesThread.Join();
```

