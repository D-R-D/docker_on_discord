using Newtonsoft.Json;
using System.Collections;
using System.Text.Json;
using System.Text.Unicode;

namespace discord_template
{
    internal class CommandBuilder
    {
        public readonly string[] m_JsonList;

        //Jsonを触るときに使うかもしれない設定
        static JsonSerializerOptions jopt = new JsonSerializerOptions()
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };

        //m_JsonListを初期化
        public CommandBuilder(string targetfile)
        {
            if (!Directory.Exists(Directory.GetCurrentDirectory() + @"/command/")) 
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/command");
                throw new Exception("Dir name \"Command\" is not Exists.");
            }

            string[] json_list = Directory.GetFiles(Directory.GetCurrentDirectory() + @"/command/", targetfile);
            if(json_list == null || json_list.Length <= 0) { throw new Exception("No jsonfile Found."); }

            m_JsonList = json_list;
        }

        //m_JsonList内のJson構造中のtarget_nameと一致する要素を現在のコンテナ名と置き換える
        public string CommandPush(string target_name) 
        {
            if(m_JsonList == null || m_JsonList.Length <= 0) { throw new Exception($"{nameof(m_JsonList)} is null or Empty."); }
            if(Tools.IsNullOrEmpty(target_name)) { throw new ArgumentNullException(nameof(target_name)); }

            string[] containers = Get_Containers();
            Dictionary<string, string> PathJsonPairs = ReadJsonFiles();

            foreach(KeyValuePair<string,string> PathandJson in PathJsonPairs)
            {

            }

            return "";
        }

        public Dictionary<string, string> ReadJsonFiles()
        {
            if(m_JsonList == null || m_JsonList.Length <= 0) { throw new Exception($"{nameof(m_JsonList)} is null or Empty."); }
            Dictionary<string, string> commandvalue = new Dictionary<string, string>();

            foreach (string jsonpath in m_JsonList)
            {
                commandvalue.Add(jsonpath, File.ReadAllText(jsonpath));
            }

            return commandvalue;
        }

        public string[] Get_Containers()
        {
            //docker ps -a --format "{{.Names}}"
            return new string[0];
        }
    }
}
