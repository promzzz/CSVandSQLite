using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;

namespace csvdb
{
    /// Использовал CsvHelper для работы с CSV и TSV файлами и Entity Framework (https://joshclose.github.io/CsvHelper/)
    public partial class MainWindow : Window
    {
        WorkWithFileClass workWithFile;                     // Класс содержит методы для работы с файлами
        WorkWithDBClass workWithDB;                         // Класс содержит методы для работы c базой
        DataBaseContext questionnaireDBContext;             // Контекст базы данных
        List<User> list;                                    // Буферный список для хранения объектов
        bool dbState = false;
        public MainWindow()
        {
            InitializeComponent();
            workWithFile = new WorkWithFileClass();
            questionnaireDBContext = new DataBaseContext();
            workWithDB = new WorkWithDBClass(questionnaireDBContext);
            ContextMenu menu = new ContextMenu();
            MenuItem delete_item = new MenuItem();
            delete_item.Header = "Удалить";
            delete_item.Click += Delete_item_Click;
            menu.Items.Add(delete_item);
            DataGrid.ContextMenu = menu;
        }

        private void Delete_item_Click(object sender, RoutedEventArgs e)
        {
            list.Remove((User)DataGrid.SelectedItem);
            if (dbState == true)
            {
                questionnaireDBContext.Users.Remove((User)DataGrid.SelectedItem);
                questionnaireDBContext.SaveChanges();
            }
            DataGrid.Items.Refresh();
        }

        private void Grid_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private  async void OpenFile(object sender, RoutedEventArgs e)  // Считываем из файла
        {
            OpenFileDialog of = new OpenFileDialog() { Filter = "CSV|*.csv|TSV|*.tsv" };
            if (of.ShowDialog() == true) // Получаем путь
            {
                if (File.Exists(of.FileName)) // Существует ли файл
                {
                    list = await workWithFile.OpenAndGetAllDataAsync(of.FileName);
                    if (list == null) return;
                    DataGrid.ItemsSource = list;
                    DataGrid.Columns[0].Header = "Имя";
                    DataGrid.Columns[1].Header = "Дата рождения";
                    DataGrid.Columns[2].Header = "E-mail";
                    DataGrid.Columns[3].Header = "Телефон";
                    dbState = false;
                }   
            }
        }
        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SaveFile(object sender, RoutedEventArgs e) // Сохраняем в файл
        {
            workWithFile.SaveAndSetAllData(DataGrid);
        }

       private  async void GetAllRowsFromDB(object sender, RoutedEventArgs e) // Получаем данные из базы
       {
            list = await workWithDB.GetAllRows();
            DataGrid.ItemsSource = list;
            DataGrid.Columns[0].Header = "Имя";
            DataGrid.Columns[1].Header = "Дата рождения";
            DataGrid.Columns[2].Header = "E-mail";
            DataGrid.Columns[3].Header = "Телефон";
            DataGrid.Columns[0].IsReadOnly = true;
            DataGrid.CanUserAddRows = false;
            dbState = true;
       }

        private async void SaveAllRowOnDB(object sender, RoutedEventArgs e) // Сохраняем изменения объектов полученных из базы
        {
           await questionnaireDBContext.SaveChangesAsync();
        }

        private async void AddNewRowsOnDB(object sender, RoutedEventArgs e) // Добавляем объекты открытые в гриде, в базу. Если объект с полем name уже есть в базе - не добавляем
        {
            List<User> list_add = workWithFile.ExtractDataFromTable(DataGrid);
            List<User> list_compare = questionnaireDBContext.Users.ToList();
            for (int i = 0; i < list_add.Count; i++) if (list_compare.FindAll(x => x.NameId == list_add[i].NameId).Count == 0) questionnaireDBContext.Users.Add(list_add[i]);
            await questionnaireDBContext.SaveChangesAsync();
        }

        private void CreateEmptyList(object sender, RoutedEventArgs e) // Создаём новую пустую запись
        {
            workWithFile.CreateNewRows(DataGrid);
        }

    }
}
