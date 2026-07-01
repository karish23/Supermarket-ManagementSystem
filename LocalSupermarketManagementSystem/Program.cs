using LocalSupermarketManagementSystem.Data;
using LocalSupermarketManagementSystem.UI;
using Microsoft.EntityFrameworkCore;

namespace LocalSupermarketManagementSystem;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        using (var db = new SupermarketDbContext())
        {
            db.Database.EnsureCreated();
            DatabaseSeeder.Seed(db);
        }

        Application.Run(new MainForm());
    }
}
