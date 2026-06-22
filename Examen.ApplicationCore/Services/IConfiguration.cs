internal interface IConfiguration
{
    object GetSection(string v);
    T GetValue<T>(string v);
}