using AmpHelper.Library.Interfaces;
using System;

namespace AmpHelper.Library.Tweaks
{
    public class UnlockAllTweak : ITweak
    {
        private string Path { get; set; }
        public ITweak SetPath(string path)
        {
            Path = path;
            return this;
        }

        public bool IsEnabled()
        {
            throw new NotImplementedException();
        }

        public void EnableTweak()
        {
            throw new NotImplementedException();
        }

        public void DisableTweak()
        {
            throw new NotImplementedException();
        }
    }
}
