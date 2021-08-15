using MyCoffeeApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace MyCoffeeApp.Services
{
    public static class CoffeeService
    {
        public static SQLiteAsyncConnection db;
        static async Task Init()
        {
            if (db != null)
                return;

            string username = Preferences.Get("username", "default_name");
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, $"data_{username}.db");
            db = new SQLiteAsyncConnection(databasePath);
            await db.CreateTableAsync<Coffee>();
        }

        public static async Task AddCoffee(string name, string roaster, string type)
        {
            await Init();
            string image = "";
            switch (type)
            {
                case "Work":
                    image = "https://gimg2.baidu.com/image_search/src=http%3A%2F%2Fdpic.tiankong.com%2Fis%2Fd8%2FQJ8105531402.jpg%3Fx-oss-process%3Dstyle%2Fshows&refer=http%3A%2F%2Fdpic.tiankong.com&app=2002&size=f9999,10000&q=a80&n=0&g=0n&fmt=jpeg?sec=1624455292&t=1bd3ecad12b9efb5daf90fa93b0fe090";
                    break;
                case "Personal":
                    image = "https://ss0.bdstatic.com/70cFvHSh_Q1YnxGkpoWK1HF6hhy/it/u=2343442373,774490915&fm=224&gp=0.jpg";
                    break;
                case "Other":
                    image = "https://ss1.bdstatic.com/70cFuXSh_Q1YnxGkpoWK1HF6hhy/it/u=3247305665,2479551166&fm=26&gp=0.jpg";
                    break;
                case "Life":
                    image = "https://gimg2.baidu.com/image_search/src=http%3A%2F%2Fb-ssl.duitang.com%2Fuploads%2Fitem%2F201503%2F22%2F20150322185746_afn38.jpeg&refer=http%3A%2F%2Fb-ssl.duitang.com&app=2002&size=f9999,10000&q=a80&n=0&g=0n&fmt=jpeg?sec=1624456205&t=622bb320a2293894c7bde61358ea5736";
                    break;
            }
            var coffee = new Coffee
            {
                Name = name,
                Roaster = roaster,
                Image = image,
                Type = type
            };
            var id = await db.InsertAsync(coffee);
        }

        public static async Task RemoveCoffee(int id)
        {
            await Init();
            await db.DeleteAsync<Coffee>(id);
        }

        public static async Task<Coffee> GetCoffee(int id)
        {
            await Init();
            var coffee = await db.Table<Coffee>().FirstOrDefaultAsync(c => c.Id == id);
            return coffee;
        }

        public static async Task<IEnumerable<Coffee>> GetAllCoffee()
        {
            await Init();
            var coffee = await db.Table<Coffee>().ToListAsync();
            var sortedcoffee = coffee.OrderBy(x => x.Name).ToList();
            return sortedcoffee;
        }

        public static async Task UpdateCoffee(int id, string name, string roaster)
        {
            await Init();
            var coffee = await db.Table<Coffee>().FirstOrDefaultAsync(c => c.Id == id);
            coffee.Name = name;
            coffee.Roaster = roaster;
            await db.UpdateAsync(coffee);
        }
    }
}
