using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using File = Google.Apis.Drive.v3.Data.File;

namespace Plachtovac.Client.Services
{
    public class GoogleDriveWrapper
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly IAccessTokenProvider _accessTokenProvider;

        public GoogleDriveWrapper(AuthenticationStateProvider authenticationStateProvider,
            IAccessTokenProvider accessTokenProvider)
        {
            _authenticationStateProvider = authenticationStateProvider;
            _accessTokenProvider = accessTokenProvider;
        }

        public async IAsyncEnumerable<File> LoadFiles(string query = null, string fields = "nextPageToken, files(id, name,version,mimeType,size,parents)")
        {
            var service = await GetService();
            String pageToken;
            do
            {
                var listRequest = service.Files.List();
                listRequest.Q = $"trashed = false and ({query ?? "trashed = false"})";
                listRequest.Spaces = "drive";
                listRequest.Fields = fields;

                var result = await listRequest.ExecuteAsync();
                foreach (var resultFile in result.Files)
                {
                    yield return resultFile;
                }

                pageToken = result.NextPageToken;
            } while (pageToken != null);
        }

        public IAsyncEnumerable<File> LoadFilesInDirectory(string directory, string query = null)
        {
            return LoadFiles(query + $" and '{directory}' in parents");
        }

        public async Task<File> GetOrCreateDirectory(string name, string parent = null)
        {
            return await GetOrCreateEntry(name, "application/vnd.google-apps.folder", parent);
        }

        public async Task<File> GetOrCreateEntry(string name, string mimeType, string parent = null)
        {
            var service = await GetService();
            var files = await LoadFiles($"name='{name}' and mimeType = '{mimeType}'" +
                                        (parent == null ? "" : $" and '{parent}' in parents")).ToListAsync();
            if (files.Any())
            {
                return files.First();
            }

            var file = new File
            {
                Name = name,
                MimeType = mimeType,
                Parents = new List<string>()
            };
            if (parent != null)
            {
                file.Parents.Add(parent);
            }

            return await service.Files.Create(file).ExecuteAsync();
        }

        public async Task SaveFile(File file, Stream content)
        {
            var service = await GetService();
            await service.Files.Update(new File(), file.Id, content, file.MimeType).UploadAsync();
        }

        public async Task<byte[]> ReadFile(File file)
        {
            var service = await GetService();
            using (var ms = new MemoryStream())
            {
                await service.Files.Get(file.Id).DownloadAsync(ms);
                return ms.ToArray();
            }
        }

        private async Task<DriveService> GetService()
        {
            var user = await _authenticationStateProvider.GetAuthenticationStateAsync();
            if (!user.User.Identity.IsAuthenticated) throw new UnauthorizedAccessException();
            var token = await _accessTokenProvider.RequestAccessToken();
            if (token.TryGetToken(out var accessToken))
            {
                Console.WriteLine(accessToken.Value);


                return new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = GoogleCredential.FromAccessToken(accessToken.Value),
                    GZipEnabled = false //Gzip sa použiva na pozadi, C# dostane uz dekomprimovaný content z web api
                });
            }
            throw new UnauthorizedAccessException();
        }

        public async Task<string> ShareFile(File file)
        {
            var service = await GetService();
            var permission = new Permission
            {
                Type = "anyone",
                Role = "reader"
            };
            var createdPermission = await service.Permissions.Create(permission, file.Id).ExecuteAsync();
            return $"https://drive.google.com/uc?export=view&id={file.Id}";
        }
    }
}
