using System.Threading.Tasks;
using Tweetinvi;

namespace AikidoWebsite.Web.Service
{
    public class TwitterService
    {
        private readonly TwitterClient Client;
        
        public TwitterService(string consumerKey, string consumerSecret, string userAccessToken, string userAccessSecret)
        {
            Client = new TwitterClient(consumerKey, consumerSecret, userAccessToken, userAccessSecret);
        }

        public async Task<string> Publish(string text)
        {
            var tweet = await Client.Tweets.PublishTweetAsync(text);
            return tweet.IdStr;
        }
    }
}
