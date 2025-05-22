using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using PrototipoTrue.Models;
namespace PrototipoTrue.Helpers
{
    public class SQLiteDatabaseHelpers
    {
        readonly SQLiteAsyncConnection _conn;

        public SQLiteDatabaseHelpers(string dbPath)
        {
            _conn = new SQLiteAsyncConnection(dbPath);
            _conn.CreateTableAsync<Editora>().Wait();
            _conn.CreateTableAsync<Autor>().Wait();
            _conn.CreateTableAsync<Livro>().Wait();
        }

        public Task<int> Insert<T>(T item) where T : class, new()
        {
            return _conn.InsertAsync(item);
        }
        public Task<int> Update<T>(T item) where T : class, new()
        {
            return _conn.UpdateAsync(item);
        }
        public async Task<int> Delete<T>(int id) where T : class, new()
        {
            var obj = await _conn.FindAsync<T>(id);
            if (obj is null)
            {
                return 0;
            }

            return await _conn.DeleteAsync(obj);
        }

        public Task<List<T>> GetAll<T>() where T : class, new()
        {
            return _conn.Table<T>().ToListAsync();
        }

        public Task<List<T>> Search<T>(string columnName, string search) where T : class, new()
        {
            string tablename = typeof(T).Name;
            string sql = $"SELECT * FROM {tablename} WHERE {columnName} LIKE ?";
            return _conn.QueryAsync<T>(sql, $"%{search}%");
        }

        public async Task Purge<T>() where T : class, new()
        {
            string tableName = typeof(T).Name;
            await _conn.ExecuteAsync($"DELETE FROM {tableName};");
            await _conn.ExecuteAsync($"DELETE FROM SQLITE_SEQUENCE WHERE name='{tableName}';");
            await _conn.ExecuteAsync("VACUUM;");
        }
    }
}
