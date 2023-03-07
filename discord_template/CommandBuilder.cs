using docker_on_discord;
using Newtonsoft.Json;
using System.Collections;
using System.Text.Json;
using System.Text.Unicode;

namespace discord_template
{
    internal class CommandBuilder
    {
        public readonly string[] m_JsonList;

        /// <summary>
        /// Jsonを触るときに使うかもしれない設定
        /// </summary>
        static JsonSerializerOptions jopt = new JsonSerializerOptions()
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };

        /// <summary>
        /// m_JsonListを初期化
        /// </summary>
        /// <param name="targetfile"></param>
        /// <exception cref="Exception"></exception>
        public CommandBuilder(string targetfile)
        {
            if (!Directory.Exists(Directory.GetCurrentDirectory() + @"/commands/")) 
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/commands");
                throw new Exception("Dir name \"Command\" is not Exists.");
            }

            string[] json_list = Directory.GetFiles(Directory.GetCurrentDirectory() + @"/commands/", targetfile);
            if(json_list == null || json_list.Length <= 0) { throw new Exception("No jsonfile Found."); }

            m_JsonList = json_list; //[jsonpath, jsonpath, ...]
        }

        /// <summary>
        /// m_JsonList内のJson構造中の、target_nameと一致する要素を現在のコンテナ名と置き換える
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void CommandPush() 
        {
            if(m_JsonList == null || m_JsonList.Length <= 0) { throw new Exception($"{nameof(m_JsonList)} is null or Empty."); }

            List<Dictionary<string,string>> containers = Get_Containers();
            Dictionary<string, string> PathJsonPairs = ReadJsonFiles(); //{ JsonPath, JsonContent }

            foreach(KeyValuePair<string,string> PathandJson in PathJsonPairs)
            {
                string edited = PushContainerList(containers, PathandJson.Value);
                using (var sw = new StreamWriter(PathandJson.Key.ToString(), false))
                {
                    sw.WriteLine(edited);
                }
            }
        }

        /// <summary>
        /// https://www.fenet.jp/dotnet/column/language/5722/
        /// </summary>
        /// <returns></returns>
        private List<Dictionary<string, string>> Get_Containers()
        {
            string ContainerNames = ConsoleCommandRunner.GetCommandResult("\"docker ps -a --format {{.Names}}\"");
            string[] ContainerList = ContainerNames.Split('\n');
            Console.WriteLine(ContainerNames);

            List<Dictionary<string, string>> additem = new List<Dictionary<string, string>>();
            foreach (string name in ContainerList)
            {
                if (name != "")
                {
                    Dictionary<string, string> tempitem = new Dictionary<string, string>()
                    {
                        { "name", name.ToString() },
                        { "value", name.ToString() }
                    };
                    additem.Add(tempitem);
                }
            }

            return additem;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private Dictionary<string, string> ReadJsonFiles()
        {
            if(m_JsonList == null || m_JsonList.Length <= 0) { throw new Exception($"{nameof(m_JsonList)} is null or Empty."); }
            Dictionary<string, string> commandvalue = new Dictionary<string, string>();

            foreach (string jsonpath in m_JsonList)
            {
                commandvalue.Add(jsonpath, File.ReadAllText(jsonpath));
            }

            return commandvalue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="containers"></param>
        /// <param name="JsonContent"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        private string PushContainerList(List<Dictionary<string,string>> containers, string JsonContent)
        {
            if(containers == null) { throw new ArgumentNullException(nameof(containers)); }
            if(Tools.IsNullOrEmpty(JsonContent)) { throw new Exception($"{nameof(JsonContent)} is null or Empty."); }


            //読みだしたjson全体を(Dictionary)commandにデシリアライズする
            Dictionary<string, object> command = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonContent)!;

            //(Dictionary)commandの配列要素optionをjsonにシリアライズしてから、再度(ArrayList)option_arrayとしてデシリアライズする
            ArrayList option_array = JsonConvert.DeserializeObject<ArrayList>(JsonConvert.SerializeObject(command!["options"]))!;

            //(ArrayList)option_arrayの1番目の要素をjsonにシリアライズしてから、再度(Dictionary)container_commandとしてデシリアライズする
            Dictionary<string, object> container_command = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(option_array[1]))!;

            //(Dictionaty)container_commandのchoices要素として(List)additemを代入する
            container_command!["choices"] = containers;

            //(ArrayList)option_arrayの1番目の要素として(Dictionary)container_commandを代入する
            option_array[1] = container_command;

            //(Dictionary)commandのoptions要素として(ArrayList)option_arrayを代入する
            command["options"] = option_array;


            return JsonConvert.SerializeObject(command,Formatting.Indented);
        }
    }
}
