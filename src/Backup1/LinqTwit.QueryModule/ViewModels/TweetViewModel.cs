using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using LinqTwit.Infrastructure;
using LinqTwit.Twitter;

namespace LinqTwit.QueryModule.ViewModels
{
    public class TweetViewModel : ViewModelBase
    {
        private readonly Status _status;

        public TweetViewModel(Status status)
        {
            this._status = status;
        }

        public DateTime Created
        {
            get { return this._status.CreatedAt; }
        }

        public string Text
        {
            get { return this._status.Text; }
        }

        public string FullName
        {
            get { return this._status.User.Name; }
        }

        public string ScreenName
        {
            get { return this._status.User.ScreenName; }
        }

        public string ProfileImageUrl { get{ return this._status.User.ProfileImageUrl;}}

        public Status Status
        {
            get {
                return this._status;
            }
        }
    }
}
