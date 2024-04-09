using StackExchange.Redis;

namespace Valuator;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();

        builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379"));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();

        //testRedis();

        app.Run();       
    }

    private static void testRedis()
    {
        // Создание подключения
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379");
        IDatabase db = redis.GetDatabase();

        // Сохранение и извлечение простой строки
        db.StringSet("foo", "bar");
        Console.WriteLine(db.StringGet("foo")); // выводит "bar"

        // Сохранение и извлечение HashMap
        var hash = new HashEntry[] {
        new HashEntry("name", "John"),
        new HashEntry("surname", "Smith"),
        new HashEntry("company", "Redis"),
        new HashEntry("age", "29"),
};
        db.HashSet("user-session:123", hash);
        var hashFields = db.HashGetAll("user-session:123");
        Console.WriteLine(String.Join("; ", hashFields)); // выводит "name: John; surname: Smith; company: Redis; age: 29"
    }
}
