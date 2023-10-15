using PokerApp.Domain;
using System;
using System.IO;

namespace FileWatcherService
{
    public class FileWatcherService : IFileWatcherService
    {
        private FileSystemWatcher _fileWatcher;
        private IHandLocationProvider _handLocationProvider;

        public FileWatcherService(IHandLocationProvider handLocationProvider)
        {
            _handLocationProvider = handLocationProvider;
        }

        public event IFileWatcherService.NotifyNewHand NewHand;

        public void StartWatch()
        {
            _fileWatcher = new FileSystemWatcher(_handLocationProvider.GetHandLocation());
            _fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
            _fileWatcher.Filter = "*.txt";
            _fileWatcher.Changed += FileWatcher_Changed;
            _fileWatcher.Created += FileWatcher_Changed;
            _fileWatcher.EnableRaisingEvents = true;
        }

        private void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            NewHand(e.FullPath);
        }

        public void StopWatch()
        {
            _fileWatcher.EnableRaisingEvents = false;
        }


    }
}
