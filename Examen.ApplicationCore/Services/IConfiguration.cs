internal interface IConfiguration
{
    T GetValue<T>(string v);
}