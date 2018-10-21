using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CsvHelper;
using Microsoft.Win32;
using System.Windows;
using CsvHelper.Configuration;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace csvdb
{
    public class WorkWithFileClass
    {

        public class QuestionnaireClassMapper : ClassMap<User>
        {
            public QuestionnaireClassMapper()
            {
                Map(x => x.NameId).Name("NameId").Index(0);
                Map(x => x.Birthdate).Name("Birthdate").Index(1);
                Map(x => x.Email).Name("Email").Index(2);
                Map(x => x.Phone).Name("Phone").Index(3);
            }
        }

        List<User> GetAllRecords(CsvReader csv)
        {
            try
            {
                return csv.GetRecords<User>().ToList();
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<User>> GetAllStringsAsync(string absolutePath, string delimiter)
        {
            List<User> records = null;
            using (StreamReader sr = new StreamReader(absolutePath))
            {
                using (CsvReader csv = new CsvReader(sr)) // Освобождает StreamReader
                {
                    csv.Configuration.Delimiter = delimiter;
                    csv.Configuration.RegisterClassMap<QuestionnaireClassMapper>();
                    records = await Task.Run(() => GetAllRecords(csv));
                    sr.Close();
                    return records;
                }
            }
            
        }

        public void SetAllStrings(string absolutePath, string delimiter, IEnumerable<User> list)
        {
            using (StreamWriter sr = new StreamWriter(absolutePath))
            {
                using (CsvWriter csv = new CsvWriter(sr))
                {
                    csv.Configuration.Delimiter = delimiter;
                    csv.Configuration.RegisterClassMap<QuestionnaireClassMapper>();
                    csv.WriteRecords<User>(list);
                    sr.Flush();
                    sr.Close();
                }
            }
        }

        public List<User> ExtractDataFromTable(DataGrid DataGrid)
        {
            List<User> list = new List<User>();
            for (int i = 0; i < DataGrid.Items.Count; i++)
            {
                try
                {
                    User tmp;
                    if (DataGrid.Items[i] != null)
                    {
                        tmp = (User)DataGrid.Items[i];
                        if(!string.IsNullOrEmpty(tmp.NameId))
                        {
                            list.Add(tmp);
                        }
                    }
                }
                catch {  }
            }
            return list;
        }

        public void SaveAndSetAllData(DataGrid DataGrid)
        {
            SaveFileDialog sf = new SaveFileDialog() { Filter = "CSV|*.csv|TSV|*.tsv"};
            if (sf.ShowDialog() == true) // Получаем путь
            {
                List<User> list = ExtractDataFromTable(DataGrid);
                if (Path.GetExtension(sf.FileName) == ".csv")  SetAllStrings(sf.FileName, ",", list);       // Определяем разделитель и получаем данные из файла
                else if (Path.GetExtension(sf.FileName) == ".tsv")  SetAllStrings(sf.FileName, "\t", list);
                else MessageBox.Show("Неизвестный формат");
            }
        }

        public async Task<List<User>> OpenAndGetAllDataAsync(string PathToFile)
        {
            List<User> List = new List<User>();
            if (Path.GetExtension(PathToFile) == ".csv") return await GetAllStringsAsync(PathToFile, ",");       // Определяем разделитель и получаем данные из файла
            else if(Path.GetExtension(PathToFile) == ".tsv") return await GetAllStringsAsync(PathToFile, "\t");
            else MessageBox.Show("Неизвестный формат");
            return List;
        }


        public void CreateNewRows(DataGrid DataGrid)
        {
            DataGrid.CanUserAddRows = true;
            List<User> list = new List<User>();
            DataGrid.ItemsSource = list;
            DataGrid.Columns[0].Header = "Имя";
            DataGrid.Columns[1].Header = "Дата рождения";
            DataGrid.Columns[2].Header = "E-mail";
            DataGrid.Columns[3].Header = "Телефон";
        }
    }
}
