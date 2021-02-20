using System.IO;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace DiscordBot
{
    public class LocalInfoHandler
    {
        #region Instance Methods

        public async Task<LocalInfo> LoadLocalInfoAsync()
        {
            using (var reader = new StreamReader(File.Open("Secret.json",
                                                           FileMode.Open)))
            {
                var json = await reader.ReadToEndAsync();

                return JsonConvert.DeserializeObject<LocalInfo>(json);
            }
        }

        public async Task SaveGatewayUrlAsync(string gatewayUrl)
        {
            var existingInfo = await LoadLocalInfoAsync();

            existingInfo.GatewayUrl = gatewayUrl;

            await SaveLocalInfoAsync(existingInfo);
        }

        private async Task SaveLocalInfoAsync(LocalInfo localInfo)
        {
            using (var writer = new StreamWriter(File.Open("Secret.json",
                                                           FileMode.Open)))
            {
                var jsonString = JsonConvert.SerializeObject(localInfo);

                // Set writer back to beginning of file
                writer.BaseStream.Position = 0;

                await writer.WriteAsync(jsonString);
            }
        }

        #endregion
    }
}