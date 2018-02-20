using System;


namespace VideoLeecher.core.Models
{
    public class TwitchAuthInfo
    {

        #region კონსტრუქტორები

        public TwitchAuthInfo(string accessToken , string username)
        {

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            AccessToken = accessToken;
            UserName = username;

        }

        #endregion კონსტრუქტორები

        #region თვისებები

        public string AccessToken { get; private set; }

        public string UserName   { get; private set; }

        #endregion


    }
}
