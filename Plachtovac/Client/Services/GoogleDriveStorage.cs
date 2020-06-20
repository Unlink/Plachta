using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Components.Authorization;

namespace Plachtovac.Client.Services
{
    public class GoogleDriveStorage
    {
        private readonly GoogleDriveWrapper _driveWrapper;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        private string _plachtovacDir;
        private string _currentProject;

        public GoogleDriveStorage(GoogleDriveWrapper driveWrapper, AuthenticationStateProvider authenticationStateProvider)
        {
            _driveWrapper = driveWrapper;
            _authenticationStateProvider = authenticationStateProvider;
            _authenticationStateProvider.AuthenticationStateChanged += task =>
            {
                _plachtovacDir = null; //Clear cache
                _currentProject = null;
            };
        }

        public bool IsTracked()
        {
            return _currentProject != null;
        }

        public async Task Save(string rozvrh, string projectName = null)
        {
            if (projectName == null && !IsTracked()) throw new InvalidOperationException("Project is not tracked");
            if (projectName != null)
            {
                _currentProject = (await _driveWrapper.GetOrCreateDirectory(projectName, await GetPlachtaDir())).Id;
            }

            var projectFile = await _driveWrapper.GetOrCreateEntry("plachta.json", "text/json", _currentProject);
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(rozvrh)))
            {
                await _driveWrapper.SaveFile(projectFile, ms);
            }
        }

        public async Task<string> Open(string project)
        {
            _currentProject = (await _driveWrapper.GetOrCreateDirectory(project, await GetPlachtaDir())).Id;
            var projectFile = await _driveWrapper.GetOrCreateEntry("plachta.json", "text/json", _currentProject);
            var data = await _driveWrapper.ReadFile(projectFile);
            return Encoding.UTF8.GetString(data);
        }

        private async Task<string> GetPlachtaDir()
        {
            return _plachtovacDir ??= (await _driveWrapper.GetOrCreateDirectory("Plachtovac")).Id;
        }

        public async Task<IEnumerable<string>> GetProjects()
        {
            return await _driveWrapper
                .LoadFilesInDirectory(await GetPlachtaDir(), "mimeType = 'application/vnd.google-apps.folder'")
                .Select(f => f.Name)
                .ToListAsync();
        }

    }
}
