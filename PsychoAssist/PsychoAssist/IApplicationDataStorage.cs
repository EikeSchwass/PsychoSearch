namespace PsychoAssist
{
    public interface IApplicationDataStorage
    {
        void SaveValue(string key, string value);

        string GetData(string key);

        void DeleteValue(string key);

        bool HasKey(string key);
    }
}