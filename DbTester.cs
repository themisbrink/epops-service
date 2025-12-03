namespace EpopsService.Diagnostics;
public static class DbTester
{
    public static async Task TestConnection()
    {
    
        using var conn = new Npgsql.NpgsqlConnection("Host=aws-1-eu-west-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.ofrczsgpaqenvumycgra;Password=xYvDn09KyJbaFMWQ;SslMode=Require;TrustServerCertificate=True");

        await conn.OpenAsync();
        Console.WriteLine("Connected OK");
    }
}