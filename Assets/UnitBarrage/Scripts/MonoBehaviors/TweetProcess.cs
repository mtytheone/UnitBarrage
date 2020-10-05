using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace hatuxes.UnitBarrage
{
    /// <summary>
    /// ツイートボタンを押してツイートするためのクラス
    /// </summary>
    public class TweetProcess : MonoBehaviour
    {
        [SerializeField]
        private Button _tweetButton;

        void Start()
        {
            _tweetButton.onClick.RemoveAllListeners();
            _tweetButton.onClick.AddListener(Tweet);
        }

        private void Tweet()
        {
            // 合計スコアを取得
            var totalScore = GameManager.Instance.GetTotalScore();

            // ツイート文とハッシュタグを決める
            string text = UnityWebRequest.EscapeURL("私のスコアは \"" + totalScore.ToString() + "\" ptでした！\nhttps://github.com/mtytheone/UnitBarrage/releases/\n");
            string hashtag = UnityWebRequest.EscapeURL("UnitBarrage");

            // URLにして発行
            string url = "https://twitter.com/intent/tweet?text=" + text + "&hashtags=" + hashtag;
            Application.OpenURL(url);
        }
    }
}