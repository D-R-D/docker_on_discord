using Discord;

namespace docker_on_discord
{
    internal class SelectMenuEditor
    {
        public static async Task<SelectMenuBuilder> CreateContainerMenu(string funcName, int page)
        {
            SelectMenuBuilder builder = new SelectMenuBuilder().WithPlaceholder($"コンテナ一覧 p.{page}").WithCustomId(funcName).WithMinValues(1).WithMaxValues(1);

            if (page > 0)
            {
                builder.AddOption("Previous page.", $"page@{page - 1}", $"Go to page {(page - 1)}.");
            }
            var containers = await GetContainers(page, Settings.Shared.m_ContainerList);

            foreach (var container in containers)
            {
                try
                {
                    builder.AddOption(container, $"container@{container}", $"No params found.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            if (await PageExist(page + 1, Settings.Shared.m_ContainerList))
            {
                builder.AddOption("Next page.", $"page@{page + 1}", $"Go to page {page + 1}.");
            }

            return builder;
        }

        public static async Task<bool> PageExist(int page, List<string> Containers)
        {
            if (page < 0)
            {
                return false;
            }
            while (Containers == null)
            {
                await Task.Yield();
            }
            return Containers.ToArray().Length > page * 16;
        }

        public static async Task<List<string>> GetContainers(int page, List<string> Containers)
        {
            while (Containers == null)
            {
                await Task.Yield();
            }
            return new List<string>(Containers.Skip(16 * page).Take(16));
        }
    }
}
