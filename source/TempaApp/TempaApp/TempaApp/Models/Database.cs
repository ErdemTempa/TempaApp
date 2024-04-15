using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TempaApp.Models
{
    public static class Database
    {
        static SQLiteAsyncConnection _database;


        //public Database(string dbPath)
        //{
        //    _database = new SQLiteAsyncConnection(dbPath);
        //    //CreateTableResult createTableResult = await _database.CreateTableAsync<Indicator>();
        //}

        public static async Task Init()
        {
            if(_database!=null)
            {
                return;
            }
            _database = new SQLiteAsyncConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "tempa.db3"));

            await _database.CreateTableAsync<Indicator>();
        }


        public static async Task<List<Indicator>> GetIndicatorsAsync()
        {
            await Init();
            return await _database.Table<Indicator>().ToListAsync();
        }

        public static async Task<Indicator> GetIndicatorAsync(int id)
        {
            await Init();
            return await _database.Table<Indicator>().Where(i=>i.Id == id).FirstOrDefaultAsync();
        }

        public static async Task<int> SaveIndicatorAsync (Indicator indicator)
        {
            await Init();
            if (indicator.Id == 0)
            {
                return await _database.InsertAsync(indicator);
            }
            else
            {
                return await _database.UpdateAsync(indicator);
            }
        }

        public static async Task<int> DeleteIndicatorAsync (Indicator indicator)
        {
            await Init();
            return await _database.DeleteAsync(indicator);
        }

        public static async Task<Indicator> GetCurrentIndicatorAsync()
        {
            await Init();
            return await _database.Table<Indicator>().Where(i=>i.IsCurrentIndicator==true).FirstOrDefaultAsync();
        }



    }
}
