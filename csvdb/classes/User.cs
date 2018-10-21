using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace csvdb
{


    public class User : INotifyPropertyChanged // Модель
    {
        
        private string _NameId;
        private string _Birthdate;
        private string _Email;
        private string _Phone;

        [Key]
        public string NameId
        {
            get
            {
                return _NameId;
            }
            set
            {
                _NameId = value;
                OnPropertyChanged("NameId");
            }
        }

        public string Birthdate
        {
            get
            {
                return _Birthdate;
            }
            set
            {
                _Birthdate = value;
                OnPropertyChanged("Birthdate");
            }
        }
        public string Email
        {
            get
            {
                return _Email;
            }
            set
            {
                _Email = value;
                OnPropertyChanged("Email");
            }
        }
        public string Phone
        {
            get
            {
                return _Phone;
            }
            set
            {
                _Phone = value;
                OnPropertyChanged("Phone");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }

    public class DataBaseContext : DbContext
    {
        public DataBaseContext() : base("DefaultConnection") { }
        public DbSet<User> Users { get; set; }
    }

}
