using Microsoft.EntityFrameworkCore;

namespace SSASLogBase.Data
{
    public class DBInitializer
    {
        public static void Initialize(DataContext context)
        {
            //context.Database.EnsureCreated();
            context.Database.Migrate();
        }
    }
}