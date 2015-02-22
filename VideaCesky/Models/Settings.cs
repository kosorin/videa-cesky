using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using VideaCesky.Common;
using VideaCesky.Helpers;
using Windows.Storage;

namespace VideaCesky.Models
{
    [DataContract]
    public class Settings : BindableBase
    {
        public Settings()
        {
        }

        public static Settings Current { get; private set; }

        public const string DataFileName = "settings.txt";

        #region WatchLaterList
        private List<Video> _watchLaterList = null;
        [DataMember]
        public List<Video> WatchLaterList
        {
            get
            {
                if (_watchLaterList == null)
                {
                    _watchLaterList = new List<Video>();
                }
                return _watchLaterList;
            }
            set { SetProperty(ref _watchLaterList, value); }
        }

        public async Task AddVideo(Video video)
        {
            WatchLaterList.Insert(0, video);
            await SaveAsync();
        }

        public async Task RemoveVideo(Video video)
        {
            WatchLaterList.Remove(video);
            await SaveAsync();
        }

        public async Task ClearWatchLaterList()
        {
            WatchLaterList.Clear();
            await SaveAsync();
        }
        #endregion // end of WatchLaterList

        #region SavedTags
        private ObservableCollection<Tag> _savedTags = null;
        [DataMember]
        public ObservableCollection<Tag> SavedTags
        {
            get
            {
                if (_savedTags == null)
                {
                    _savedTags = new ObservableCollection<Tag>();
                }
                return _savedTags;
            }
            set { SetProperty(ref _savedTags, value); }
        }

        public async Task AddTagAsync(Tag tag)
        {
            SavedTags.Add(tag);
            await SaveAsync();
        }

        public async Task RemoveTag(Tag tag)
        {
            SavedTags.Remove(tag);
            await SaveAsync();
        }

        public async Task ClearSavedTags()
        {
            SavedTags.Clear();
            await SaveAsync();
        }
        #endregion // end of SavedTags

        public async Task SaveAsync()
        {
            await FileHelper.WriteToJson(DataFileName, this);
        }

        public static async Task LoadAsync()
        {
            Current = await FileHelper.ReadFromJson<Settings>(DataFileName);
        }
    }
}
