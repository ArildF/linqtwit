using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using LinqTwit.Twitter;

namespace LinqTwit.QueryModule.ViewModels
{
    public class TweetViewModel : INotifyPropertyChanged
    {
        private readonly Status status;

        public TweetViewModel(Status status)
        {
            this.status = status;
        }

        public event PropertyChangedEventHandler PropertyChanged;


        public DateTime Created
        {
            get { return this.status.CreatedAt; }
        }

        public string Text
        {
            get { return this.status.Text; }
        }

        public string FullName
        {
            get { return this.status.User.Name; }
        }

        public string ScreenName
        {
            get { return this.status.User.ScreenName; }
        }

        public string ProfileImageUrl { get{ return this.status.User.ProfileImageUrl;}}
    }
}
