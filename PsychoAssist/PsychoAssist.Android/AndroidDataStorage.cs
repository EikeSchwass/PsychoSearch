using Android.App;
using Android.Content;

namespace PsychoAssist.Droid
{
    public class AndroidDataStorage : IApplicationDataStorage
    {
        private ISharedPreferences SharedPreferences { get; }

        public AndroidDataStorage()
        {
            SharedPreferences = Application.Context.GetSharedPreferences("PsychoAssist", FileCreationMode.Private);
        }

        public void DeleteValue(string key)
        {
            var edit = SharedPreferences.Edit();
            edit.Remove(key);
            edit.Apply();
        }

        public string GetData(string key)
        {
            if (!HasKey(key))
                return null;
            return SharedPreferences.All[key].ToString();
        }

        public bool HasKey(string key)
        {
            return SharedPreferences.Contains(key);
        }

        public void SaveValue(string key, string value)
        {
            var edit = SharedPreferences.Edit();
            edit.PutString(key, value);
            edit.Apply();
        }
    }
}