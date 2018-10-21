using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
namespace csvdb
{
    public class WorkWithDBClass
    {
        DataBaseContext Context;

        public WorkWithDBClass(DataBaseContext Context)
        {
            this.Context = Context;
        }

        public async  Task<List<User>> GetAllRows() // Получаем данные из базы
        {
              return await Context.Users.ToListAsync();
        }
    }
}
