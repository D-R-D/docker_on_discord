using System.Configuration;

namespace docker_on_discord
{
    internal class Settings
    {
        private static Cache<Settings> CachedSettings = new Cache<Settings>(() => new Settings());
        public static Settings Shared => CachedSettings.Value;

        public readonly string m_Token;
        public readonly string m_DiscordAPIVersion;
        public readonly string m_ApplicationId;
        public readonly string[] m_GuildIds;
        public readonly string[] m_AdminIds;

        public List<string> m_ContainerList;

#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
        public Settings()
        {
            #region AppSettingsReaderからの設定読み込み
            var reader = new AppSettingsReader();

            //config内のtokenを取得する
            m_Token = (string)reader.GetValue("token", typeof(string));
            if (m_Token.IsNullOrEmpty()) { throw new Exception($"{nameof(m_Token)}\ntokenがnullもしくは空白です。"); }

            //config内のdiscordapi_versionを取得する
            m_DiscordAPIVersion = (string)reader.GetValue("discordapi_version", typeof(string));
            if (m_DiscordAPIVersion.IsNullOrEmpty()) { throw new Exception($"{m_DiscordAPIVersion}.\ndiscordapi_versionがnullもしくは空白です。"); }

            //config内のapplication_idを取得する
            m_ApplicationId = (string)reader.GetValue("application_id", typeof(string));
            if (m_ApplicationId.IsNullOrEmpty()) { throw new Exception($"{nameof(m_ApplicationId)}.\napplication_idがnullもしくは空白です。"); }

            //config内の","で区切られたguild_idを取得する
            var guildId = (string)reader.GetValue("guild_id", typeof(string));
            if (guildId.IsNullOrEmpty()) { throw new Exception($"{nameof(guildId)}.\nguild_idがnullもしくは空白です。"); }
            m_GuildIds = guildId!.Split(',');

            //config内の","で区切られたadmin_idを取得する
            string adminId = (string)reader.GetValue("admin_id", typeof(string));
            if (adminId.IsNullOrEmpty()) { throw new Exception($"{nameof(adminId)}.\nadmin_idがnullもしくは空白です。"); }
            m_AdminIds = adminId!.Split(',');
            #endregion

            string ContainerNames = ConsoleCommandRunner.GetCommandResult("\"docker ps -a --format {{.Names}}\"");
            string[] ContainerArray = ContainerNames.Split('\n');
            m_ContainerList = new List<string>(ContainerArray);
            Console.WriteLine("-------------------------------");
            foreach(string item in m_ContainerList)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("-------------------------------");
        }
#pragma warning restore CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
    }
}
