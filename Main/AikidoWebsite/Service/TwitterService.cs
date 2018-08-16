using Tweetinvi;

namespace AikidoWebsite.Web.Service
{
    public class TwitterService
    {
        public TwitterService(string consumerKey, string consumerSecret, string userAccessToken, string userAccessSecret)
        {
            Auth.SetUserCredentials(consumerKey, consumerSecret, userAccessToken, userAccessSecret);
        }

        public string Publish(string text)
        {
            var result = Tweet.PublishTweet(text);
            return result.IdStr;
        }
    }
}
