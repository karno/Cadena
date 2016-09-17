using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cadena.Data;
using Cadena.Meteor;

namespace Cadena.Test
{
    internal static class TweetSamples
    {
        // simple status
        // https://twitter.com/twitterapi/status/655139342613004288
        private static readonly string _statusJson0 = @"{
  ""created_at"": ""Fri Oct 16 21:52:45 +0000 2015"",
  ""id"": 655139342613004300,
  ""id_str"": ""655139342613004288"",
  ""text"": ""The streaming endpoints are returning to normal."",
  ""source"": ""<a href=\""https://about.twitter.com/products/tweetdeck\"" rel=\""nofollow\"">TweetDeck</a>"",
  ""truncated"": false,
  ""in_reply_to_status_id"": null,
  ""in_reply_to_status_id_str"": null,
  ""in_reply_to_user_id"": null,
  ""in_reply_to_user_id_str"": null,
  ""in_reply_to_screen_name"": null,
  ""user"":  {
    ""id"": 6253282,
    ""id_str"": ""6253282"",
    ""name"": ""Twitter API"",
    ""screen_name"": ""twitterapi"",
    ""location"": ""San Francisco, CA"",
    ""description"": ""The Real Twitter API. I tweet about API changes, service issues and happily answer questions about Twitter and our API. Don't get an answer? It's on my website."",
    ""url"": ""http://t.co/78pYTvWfJd"",
    ""entities"":  {
      ""url"":  {
        ""urls"":  [
           {
            ""url"": ""http://t.co/78pYTvWfJd"",
            ""expanded_url"": ""http://dev.twitter.com"",
            ""display_url"": ""dev.twitter.com"",
            ""indices"":  [
              0,
              22
            ]
          }
        ]
      },
      ""description"":  {
        ""urls"":  []
      }
    },
    ""protected"": false,
    ""followers_count"": 5305850,
    ""friends_count"": 48,
    ""listed_count"": 13011,
    ""created_at"": ""Wed May 23 06:01:13 +0000 2007"",
    ""favourites_count"": 27,
    ""utc_offset"": -28800,
    ""time_zone"": ""Pacific Time (US & Canada)"",
    ""geo_enabled"": true,
    ""verified"": true,
    ""statuses_count"": 3554,
    ""lang"": ""en"",
    ""contributors_enabled"": false,
    ""is_translator"": false,
    ""is_translation_enabled"": false,
    ""profile_background_color"": ""C0DEED"",
    ""profile_background_image_url"": ""http://pbs.twimg.com/profile_background_images/656927849/miyt9dpjz77sc0w3d4vj.png"",
    ""profile_background_image_url_https"": ""https://pbs.twimg.com/profile_background_images/656927849/miyt9dpjz77sc0w3d4vj.png"",
    ""profile_background_tile"": true,
    ""profile_image_url"": ""http://pbs.twimg.com/profile_images/2284174872/7df3h38zabcvjylnyfe3_normal.png"",
    ""profile_image_url_https"": ""https://pbs.twimg.com/profile_images/2284174872/7df3h38zabcvjylnyfe3_normal.png"",
    ""profile_banner_url"": ""https://pbs.twimg.com/profile_banners/6253282/1431474710"",
    ""profile_link_color"": ""0084B4"",
    ""profile_sidebar_border_color"": ""C0DEED"",
    ""profile_sidebar_fill_color"": ""DDEEF6"",
    ""profile_text_color"": ""333333"",
    ""profile_use_background_image"": true,
    ""has_extended_profile"": false,
    ""default_profile"": false,
    ""default_profile_image"": false,
    ""following"": true,
    ""follow_request_sent"": false,
    ""notifications"": false
  },
  ""geo"": null,
  ""coordinates"": null,
  ""place"": null,
  ""contributors"": null,
  ""is_quote_status"": false,
  ""retweet_count"": 73,
  ""favorite_count"": 91,
  ""entities"":  {
    ""hashtags"":  [],
    ""symbols"":  [],
    ""user_mentions"":  [],
    ""urls"":  []
  },
  ""favorited"": false,
  ""retweeted"": false,
  ""lang"": ""en""
}";
        // contains hashtag, mention, and movie.
        // https://twitter.com/TwitterSportsJP/status/683195297640517634
        private static readonly string _statusJson1 = @"{
  ""created_at"": ""Sat Jan 02 07:57:06 +0000 2016"",
  ""id"": 683195297640517634,
  ""id_str"": ""683195297640517634"",
  ""text"": ""#箱根駅伝 往路に関して、Twitterでの盛り上がりをまとめました。最も盛り上がったのは青山学院大学（@aogaku_rikujyou）が往路優勝、そして2連覇を決めたモーメントでした。 https://t.co/XVtVcBvDxI"",
  ""source"": ""<a href=\""http://twitter.com\"" rel=\""nofollow\"">Twitter Web Client</a>"",
  ""truncated"": false,
  ""in_reply_to_status_id"": null,
  ""in_reply_to_status_id_str"": null,
  ""in_reply_to_user_id"": null,
  ""in_reply_to_user_id_str"": null,
  ""in_reply_to_screen_name"": null,
  ""user"":  {
    ""id"": 1159274324,
    ""id_str"": ""1159274324"",
    ""name"": ""Twitter Sports Japan"",
    ""screen_name"": ""TwitterSportsJP"",
    ""location"": ""Tokyo, JAPAN"",
    ""description"": ""Twitter Sports Japan 公式アカウントです。選手やチームのリストはこちらから。https://twitter.com/TwitterSportsJP/lists\nTokyo, JAPAN"",
    ""url"": ""https://t.co/QjOgMnzmxL"",
    ""entities"":  {
      ""url"":  {
        ""urls"":  [
           {
            ""url"": ""https://t.co/QjOgMnzmxL"",
            ""expanded_url"": ""https://blog.twitter.com/ja/media"",
            ""display_url"": ""blog.twitter.com/ja/media"",
            ""indices"":  [
              0,
              23
            ]
          }
        ]
      },
      ""description"":  {
        ""urls"":  []
      }
    },
    ""protected"": false,
    ""followers_count"": 237746,
    ""friends_count"": 399,
    ""listed_count"": 391,
    ""created_at"": ""Fri Feb 08 05:54:29 +0000 2013"",
    ""favourites_count"": 552,
    ""utc_offset"": 32400,
    ""time_zone"": ""Tokyo"",
    ""geo_enabled"": false,
    ""verified"": true,
    ""statuses_count"": 2469,
    ""lang"": ""ja"",
    ""contributors_enabled"": false,
    ""is_translator"": false,
    ""is_translation_enabled"": false,
    ""profile_background_color"": ""131516"",
    ""profile_background_image_url"": ""http://abs.twimg.com/images/themes/theme14/bg.gif"",
    ""profile_background_image_url_https"": ""https://abs.twimg.com/images/themes/theme14/bg.gif"",
    ""profile_background_tile"": true,
    ""profile_image_url"": ""http://pbs.twimg.com/profile_images/422903820658552833/-QfUQWGG_normal.png"",
    ""profile_image_url_https"": ""https://pbs.twimg.com/profile_images/422903820658552833/-QfUQWGG_normal.png"",
    ""profile_banner_url"": ""https://pbs.twimg.com/profile_banners/1159274324/1397006603"",
    ""profile_link_color"": ""009999"",
    ""profile_sidebar_border_color"": ""FFFFFF"",
    ""profile_sidebar_fill_color"": ""EFEFEF"",
    ""profile_text_color"": ""333333"",
    ""profile_use_background_image"": true,
    ""has_extended_profile"": false,
    ""default_profile"": false,
    ""default_profile_image"": false,
    ""following"": false,
    ""follow_request_sent"": false,
    ""notifications"": false
  },
  ""geo"": null,
  ""coordinates"": null,
  ""place"": null,
  ""contributors"": null,
  ""is_quote_status"": false,
  ""retweet_count"": 73,
  ""favorite_count"": 106,
  ""entities"":  {
    ""hashtags"":  [
       {
        ""text"": ""箱根駅伝"",
        ""indices"":  [
          0,
          5
        ]
      }
    ],
    ""symbols"":  [],
    ""user_mentions"":  [
       {
        ""screen_name"": ""aogaku_rikujyou"",
        ""name"": ""青学大陸上競技部(長距離ブロック)"",
        ""id"": 420374800,
        ""id_str"": ""420374800"",
        ""indices"":  [
          52,
          68
        ]
      }
    ],
    ""urls"":  [],
    ""media"":  [
       {
        ""id"": 683194502811521000,
        ""id_str"": ""683194502811521025"",
        ""indices"":  [
          95,
          118
        ],
        ""media_url"": ""http://pbs.twimg.com/ext_tw_video_thumb/683194502811521025/pu/img/Q5hwh5BWMi00lw9A.jpg"",
        ""media_url_https"": ""https://pbs.twimg.com/ext_tw_video_thumb/683194502811521025/pu/img/Q5hwh5BWMi00lw9A.jpg"",
        ""url"": ""https://t.co/XVtVcBvDxI"",
        ""display_url"": ""pic.twitter.com/XVtVcBvDxI"",
        ""expanded_url"": ""http://twitter.com/TwitterSportsJP/status/683195297640517634/video/1"",
        ""type"": ""photo"",
        ""sizes"":  {
          ""thumb"":  {
            ""w"": 150,
            ""h"": 150,
            ""resize"": ""crop""
          },
          ""medium"":  {
            ""w"": 600,
            ""h"": 338,
            ""resize"": ""fit""
          },
          ""small"":  {
            ""w"": 340,
            ""h"": 191,
            ""resize"": ""fit""
          },
          ""large"":  {
            ""w"": 1024,
            ""h"": 576,
            ""resize"": ""fit""
          }
        }
      }
    ]
  },
  ""extended_entities"":  {
    ""media"":  [
       {
        ""id"": 683194502811521000,
        ""id_str"": ""683194502811521025"",
        ""indices"":  [
          95,
          118
        ],
        ""media_url"": ""http://pbs.twimg.com/ext_tw_video_thumb/683194502811521025/pu/img/Q5hwh5BWMi00lw9A.jpg"",
        ""media_url_https"": ""https://pbs.twimg.com/ext_tw_video_thumb/683194502811521025/pu/img/Q5hwh5BWMi00lw9A.jpg"",
        ""url"": ""https://t.co/XVtVcBvDxI"",
        ""display_url"": ""pic.twitter.com/XVtVcBvDxI"",
        ""expanded_url"": ""http://twitter.com/TwitterSportsJP/status/683195297640517634/video/1"",
        ""type"": ""video"",
        ""sizes"":  {
          ""thumb"":  {
            ""w"": 150,
            ""h"": 150,
            ""resize"": ""crop""
          },
          ""medium"":  {
            ""w"": 600,
            ""h"": 338,
            ""resize"": ""fit""
          },
          ""small"":  {
            ""w"": 340,
            ""h"": 191,
            ""resize"": ""fit""
          },
          ""large"":  {
            ""w"": 1024,
            ""h"": 576,
            ""resize"": ""fit""
          }
        },
        ""video_info"":  {
          ""aspect_ratio"":  [
            16,
            9
          ],
          ""duration_millis"": 7233,
          ""variants"":  [
             {
              ""bitrate"": 320000,
              ""content_type"": ""video/mp4"",
              ""url"": ""https://video.twimg.com/ext_tw_video/683194502811521025/pu/vid/320x180/CaYaA_cFxPcT0qyT.mp4""
            },
             {
              ""content_type"": ""application/dash+xml"",
              ""url"": ""https://video.twimg.com/ext_tw_video/683194502811521025/pu/pl/kydleQFggFS25asl.mpd""
            },
             {
              ""bitrate"": 832000,
              ""content_type"": ""video/mp4"",
              ""url"": ""https://video.twimg.com/ext_tw_video/683194502811521025/pu/vid/640x360/vstQvlX7I-uateSj.mp4""
            },
             {
              ""bitrate"": 2176000,
              ""content_type"": ""video/mp4"",
              ""url"": ""https://video.twimg.com/ext_tw_video/683194502811521025/pu/vid/1280x720/C5mauzOC0xbgoVYy.mp4""
            },
             {
              ""bitrate"": 832000,
              ""content_type"": ""video/webm"",
              ""url"": ""https://video.twimg.com/ext_tw_video/683194502811521025/pu/vid/640x360/vstQvlX7I-uateSj.webm""
            },
             {
              ""content_type"": ""application/x-mpegURL"",
              ""url"": ""https://video.twimg.com/ext_tw_video/683194502811521025/pu/pl/kydleQFggFS25asl.m3u8""
            }
          ]
        }
      }
    ]
  },
  ""favorited"": false,
  ""retweeted"": false,
  ""possibly_sensitive"": false,
  ""possibly_sensitive_appealable"": false,
  ""lang"": ""ja""
}";

        // contains url
        // https://twitter.com/TwitterHelpJP/status/651185122062942208
        private static readonly string _statusJson2 = @"{
  ""created_at"": ""Tue Oct 06 00:00:06 +0000 2015"",
  ""id"": 651185122062942200,
  ""id_str"": ""651185122062942208"",
  ""text"": ""パスワードを忘れた場合は、以下ヘルプにある手順に従ってパスワードリセットをお試しください。https://t.co/zYQJ9cuHGk"",
  ""source"": ""<a href=\""https://about.twitter.com/products/tweetdeck\"" rel=\""nofollow\"">TweetDeck</a>"",
  ""truncated"": false,
  ""in_reply_to_status_id"": null,
  ""in_reply_to_status_id_str"": null,
  ""in_reply_to_user_id"": null,
  ""in_reply_to_user_id_str"": null,
  ""in_reply_to_screen_name"": null,
  ""user"":  {
    ""id"": 167164816,
    ""id_str"": ""167164816"",
    ""name"": ""Twitterサポート"",
    ""screen_name"": ""TwitterHelpJP"",
    ""location"": """",
    ""description"": ""Twitterに関する不具合や新機能をツイートでご報告！お困りの際、まずこちらのツイートを確認しhttp://t.co/WjE2P04PxT をご覧下さい。そちらからお問い合せも可能です。@ツイートやDMをいただいても回答することができませんのでご了承ください。"",
    ""url"": ""http://t.co/WjE2P04PxT"",
    ""entities"":  {
      ""url"":  {
        ""urls"":  [
           {
            ""url"": ""http://t.co/WjE2P04PxT"",
            ""expanded_url"": ""http://support.twitter.com"",
            ""display_url"": ""support.twitter.com"",
            ""indices"":  [
              0,
              22
            ]
          }
        ]
      },
      ""description"":  {
        ""urls"":  [
           {
            ""url"": ""http://t.co/WjE2P04PxT"",
            ""expanded_url"": ""http://support.twitter.com"",
            ""display_url"": ""support.twitter.com"",
            ""indices"":  [
              48,
              70
            ]
          }
        ]
      }
    },
    ""protected"": false,
    ""followers_count"": 757001,
    ""friends_count"": 18,
    ""listed_count"": 7670,
    ""created_at"": ""Thu Jul 15 22:37:46 +0000 2010"",
    ""favourites_count"": 25,
    ""utc_offset"": -28800,
    ""time_zone"": ""Pacific Time (US & Canada)"",
    ""geo_enabled"": false,
    ""verified"": true,
    ""statuses_count"": 4312,
    ""lang"": ""ja"",
    ""contributors_enabled"": false,
    ""is_translator"": false,
    ""is_translation_enabled"": false,
    ""profile_background_color"": ""C0DEED"",
    ""profile_background_image_url"": ""http://pbs.twimg.com/profile_background_images/819903197/ffaad5bca02cc4536400f81345e5683d.png"",
    ""profile_background_image_url_https"": ""https://pbs.twimg.com/profile_background_images/819903197/ffaad5bca02cc4536400f81345e5683d.png"",
    ""profile_background_tile"": true,
    ""profile_image_url"": ""http://pbs.twimg.com/profile_images/2284174748/o26wjnpmzstufiwiq6a7_normal.png"",
    ""profile_image_url_https"": ""https://pbs.twimg.com/profile_images/2284174748/o26wjnpmzstufiwiq6a7_normal.png"",
    ""profile_banner_url"": ""https://pbs.twimg.com/profile_banners/167164816/1347989456"",
    ""profile_link_color"": ""0084B4"",
    ""profile_sidebar_border_color"": ""FFFFFF"",
    ""profile_sidebar_fill_color"": ""DDEEF6"",
    ""profile_text_color"": ""333333"",
    ""profile_use_background_image"": true,
    ""has_extended_profile"": false,
    ""default_profile"": false,
    ""default_profile_image"": false,
    ""following"": false,
    ""follow_request_sent"": false,
    ""notifications"": false
  },
  ""geo"": null,
  ""coordinates"": null,
  ""place"": null,
  ""contributors"": null,
  ""is_quote_status"": false,
  ""retweet_count"": 132,
  ""favorite_count"": 292,
  ""entities"":  {
    ""hashtags"":  [],
    ""symbols"":  [],
    ""user_mentions"":  [],
    ""urls"":  [
       {
        ""url"": ""https://t.co/zYQJ9cuHGk"",
        ""expanded_url"": ""https://support.twitter.com/articles/247958-"",
        ""display_url"": ""support.twitter.com/articles/24795…"",
        ""indices"":  [
          45,
          68
        ]
      }
    ]
  },
  ""favorited"": false,
  ""retweeted"": false,
  ""possibly_sensitive"": false,
  ""possibly_sensitive_appealable"": false,
  ""lang"": ""ja""
}";

        // contains quote of status
        // https://twitter.com/TwitterJP/status/676617542249525248
        private static readonly string _statusJson3 = @"{
  ""created_at"": ""Tue Dec 15 04:19:27 +0000 2015"",
  ""id"": 676617542249525200,
  ""id_str"": ""676617542249525248"",
  ""text"": ""ごめんなさい！「贈れる」でした！\nhttps://t.co/8UCLngo3Dg"",
  ""source"": ""<a href=\""http://twitter.com\"" rel=\""nofollow\"">Twitter Web Client</a>"",
  ""truncated"": false,
  ""in_reply_to_status_id"": null,
  ""in_reply_to_status_id_str"": null,
  ""in_reply_to_user_id"": null,
  ""in_reply_to_user_id_str"": null,
  ""in_reply_to_screen_name"": null,
  ""user"":  {
    ""id"": 7080152,
    ""id_str"": ""7080152"",
    ""name"": ""TwitterJP"",
    ""screen_name"": ""TwitterJP"",
    ""location"": ""東京都中央区"",
    ""description"": ""日本語版Twitter公式アカウントです。サービスに関しては https://t.co/mfQkUQLUhe をご参照ください。"",
    ""url"": ""https://t.co/A9dNuL0CCa"",
    ""entities"":  {
      ""url"":  {
        ""urls"":  [
           {
            ""url"": ""https://t.co/A9dNuL0CCa"",
            ""expanded_url"": ""http://blog.jp.twitter.com"",
            ""display_url"": ""blog.jp.twitter.com"",
            ""indices"":  [
              0,
              23
            ]
          }
        ]
      },
      ""description"":  {
        ""urls"":  [
           {
            ""url"": ""https://t.co/mfQkUQLUhe"",
            ""expanded_url"": ""https://support.twitter.com/"",
            ""display_url"": ""support.twitter.com"",
            ""indices"":  [
              31,
              54
            ]
          }
        ]
      }
    },
    ""protected"": false,
    ""followers_count"": 2106847,
    ""friends_count"": 79,
    ""listed_count"": 16507,
    ""created_at"": ""Tue Jun 26 01:54:35 +0000 2007"",
    ""favourites_count"": 115,
    ""utc_offset"": 32400,
    ""time_zone"": ""Tokyo"",
    ""geo_enabled"": false,
    ""verified"": true,
    ""statuses_count"": 3606,
    ""lang"": ""ja"",
    ""contributors_enabled"": false,
    ""is_translator"": false,
    ""is_translation_enabled"": false,
    ""profile_background_color"": ""C0DEED"",
    ""profile_background_image_url"": ""http://pbs.twimg.com/profile_background_images/567482719571959809/WwgdnJCP.jpeg"",
    ""profile_background_image_url_https"": ""https://pbs.twimg.com/profile_background_images/567482719571959809/WwgdnJCP.jpeg"",
    ""profile_background_tile"": true,
    ""profile_image_url"": ""http://pbs.twimg.com/profile_images/3407356865/62f0d53222361fbd2c1fe9889f4cc559_normal.png"",
    ""profile_image_url_https"": ""https://pbs.twimg.com/profile_images/3407356865/62f0d53222361fbd2c1fe9889f4cc559_normal.png"",
    ""profile_banner_url"": ""https://pbs.twimg.com/profile_banners/7080152/1451864421"",
    ""profile_link_color"": ""0084B4"",
    ""profile_sidebar_border_color"": ""FFFFFF"",
    ""profile_sidebar_fill_color"": ""DDEEF6"",
    ""profile_text_color"": ""333333"",
    ""profile_use_background_image"": true,
    ""has_extended_profile"": true,
    ""default_profile"": false,
    ""default_profile_image"": false,
    ""following"": false,
    ""follow_request_sent"": false,
    ""notifications"": false
  },
  ""geo"": null,
  ""coordinates"": null,
  ""place"": null,
  ""contributors"": null,
  ""quoted_status_id"": 676611661818667000,
  ""quoted_status_id_str"": ""676611661818667008"",
  ""quoted_status"":  {
    ""created_at"": ""Tue Dec 15 03:56:05 +0000 2015"",
    ""id"": 676611661818667000,
    ""id_str"": ""676611661818667008"",
    ""text"": ""今年の🎄 #クリスマスボックス 🎁では、もらうだけではなく、ツイートするだけでフォロワーさんにお得なギフトが遅れちゃうのはご存知ですか？ https://t.co/uUooVVhGWh https://t.co/DXwvhNC7f5"",
    ""source"": ""<a href=\""http://vine.co\"" rel=\""nofollow\"">Vine - Make a Scene</a>"",
    ""truncated"": false,
    ""in_reply_to_status_id"": null,
    ""in_reply_to_status_id_str"": null,
    ""in_reply_to_user_id"": null,
    ""in_reply_to_user_id_str"": null,
    ""in_reply_to_screen_name"": null,
    ""user"":  {
      ""id"": 7080152,
      ""id_str"": ""7080152"",
      ""name"": ""TwitterJP"",
      ""screen_name"": ""TwitterJP"",
      ""location"": ""東京都中央区"",
      ""description"": ""日本語版Twitter公式アカウントです。サービスに関しては https://t.co/mfQkUQLUhe をご参照ください。"",
      ""url"": ""https://t.co/A9dNuL0CCa"",
      ""entities"":  {
        ""url"":  {
          ""urls"":  [
             {
              ""url"": ""https://t.co/A9dNuL0CCa"",
              ""expanded_url"": ""http://blog.jp.twitter.com"",
              ""display_url"": ""blog.jp.twitter.com"",
              ""indices"":  [
                0,
                23
              ]
            }
          ]
        },
        ""description"":  {
          ""urls"":  [
             {
              ""url"": ""https://t.co/mfQkUQLUhe"",
              ""expanded_url"": ""https://support.twitter.com/"",
              ""display_url"": ""support.twitter.com"",
              ""indices"":  [
                31,
                54
              ]
            }
          ]
        }
      },
      ""protected"": false,
      ""followers_count"": 2106847,
      ""friends_count"": 79,
      ""listed_count"": 16507,
      ""created_at"": ""Tue Jun 26 01:54:35 +0000 2007"",
      ""favourites_count"": 115,
      ""utc_offset"": 32400,
      ""time_zone"": ""Tokyo"",
      ""geo_enabled"": false,
      ""verified"": true,
      ""statuses_count"": 3606,
      ""lang"": ""ja"",
      ""contributors_enabled"": false,
      ""is_translator"": false,
      ""is_translation_enabled"": false,
      ""profile_background_color"": ""C0DEED"",
      ""profile_background_image_url"": ""http://pbs.twimg.com/profile_background_images/567482719571959809/WwgdnJCP.jpeg"",
      ""profile_background_image_url_https"": ""https://pbs.twimg.com/profile_background_images/567482719571959809/WwgdnJCP.jpeg"",
      ""profile_background_tile"": true,
      ""profile_image_url"": ""http://pbs.twimg.com/profile_images/3407356865/62f0d53222361fbd2c1fe9889f4cc559_normal.png"",
      ""profile_image_url_https"": ""https://pbs.twimg.com/profile_images/3407356865/62f0d53222361fbd2c1fe9889f4cc559_normal.png"",
      ""profile_banner_url"": ""https://pbs.twimg.com/profile_banners/7080152/1451864421"",
      ""profile_link_color"": ""0084B4"",
      ""profile_sidebar_border_color"": ""FFFFFF"",
      ""profile_sidebar_fill_color"": ""DDEEF6"",
      ""profile_text_color"": ""333333"",
      ""profile_use_background_image"": true,
      ""has_extended_profile"": true,
      ""default_profile"": false,
      ""default_profile_image"": false,
      ""following"": false,
      ""follow_request_sent"": false,
      ""notifications"": false
    },
    ""geo"": null,
    ""coordinates"": null,
    ""place"": null,
    ""contributors"": null,
    ""is_quote_status"": false,
    ""retweet_count"": 140,
    ""favorite_count"": 200,
    ""entities"":  {
      ""hashtags"":  [
         {
          ""text"": ""クリスマスボックス"",
          ""indices"":  [
            5,
            15
          ]
        }
      ],
      ""symbols"":  [],
      ""user_mentions"":  [],
      ""urls"":  [
         {
          ""url"": ""https://t.co/uUooVVhGWh"",
          ""expanded_url"": ""https://xmasbox.net/#/gift"",
          ""display_url"": ""xmasbox.net/#/gift"",
          ""indices"":  [
            69,
            92
          ]
        },
         {
          ""url"": ""https://t.co/DXwvhNC7f5"",
          ""expanded_url"": ""https://vine.co/v/imAdrlJPYvv"",
          ""display_url"": ""vine.co/v/imAdrlJPYvv"",
          ""indices"":  [
            93,
            116
          ]
        }
      ]
    },
    ""favorited"": false,
    ""retweeted"": false,
    ""possibly_sensitive"": false,
    ""possibly_sensitive_appealable"": false,
    ""lang"": ""ja""
  },
  ""is_quote_status"": true,
  ""retweet_count"": 23,
  ""favorite_count"": 46,
  ""entities"":  {
    ""hashtags"":  [],
    ""symbols"":  [],
    ""user_mentions"":  [],
    ""urls"":  [
       {
        ""url"": ""https://t.co/8UCLngo3Dg"",
        ""expanded_url"": ""https://twitter.com/TwitterJP/status/676611661818667008"",
        ""display_url"": ""twitter.com/TwitterJP/stat…"",
        ""indices"":  [
          17,
          40
        ]
      }
    ]
  },
  ""favorited"": false,
  ""retweeted"": false,
  ""possibly_sensitive"": false,
  ""possibly_sensitive_appealable"": false,
  ""lang"": ""ja""
}";

        // contains multiple images
        // https://twitter.com/TwitterJP/status/675261031887712256
        private static readonly string _statusJson4 = @"{
  ""created_at"": ""Fri Dec 11 10:29:10 +0000 2015"",
  ""id"": 675261031887712300,
  ""id_str"": ""675261031887712256"",
  ""text"": ""今日はオフィスで、家族や友達と一緒の🎄クリスマスパーティー。 https://t.co/EoaIP1yeVP"",
  ""source"": ""<a href=\""http://twitter.com/download/iphone\"" rel=\""nofollow\"">Twitter for iPhone</a>"",
  ""truncated"": false,
  ""in_reply_to_status_id"": null,
  ""in_reply_to_status_id_str"": null,
  ""in_reply_to_user_id"": null,
  ""in_reply_to_user_id_str"": null,
  ""in_reply_to_screen_name"": null,
  ""user"":  {
    ""id"": 7080152,
    ""id_str"": ""7080152"",
    ""name"": ""TwitterJP"",
    ""screen_name"": ""TwitterJP"",
    ""location"": ""東京都中央区"",
    ""description"": ""日本語版Twitter公式アカウントです。サービスに関しては https://t.co/mfQkUQLUhe をご参照ください。"",
    ""url"": ""https://t.co/A9dNuL0CCa"",
    ""entities"":  {
      ""url"":  {
        ""urls"":  [
           {
            ""url"": ""https://t.co/A9dNuL0CCa"",
            ""expanded_url"": ""http://blog.jp.twitter.com"",
            ""display_url"": ""blog.jp.twitter.com"",
            ""indices"":  [
              0,
              23
            ]
          }
        ]
      },
      ""description"":  {
        ""urls"":  [
           {
            ""url"": ""https://t.co/mfQkUQLUhe"",
            ""expanded_url"": ""https://support.twitter.com/"",
            ""display_url"": ""support.twitter.com"",
            ""indices"":  [
              31,
              54
            ]
          }
        ]
      }
    },
    ""protected"": false,
    ""followers_count"": 2106848,
    ""friends_count"": 79,
    ""listed_count"": 16507,
    ""created_at"": ""Tue Jun 26 01:54:35 +0000 2007"",
    ""favourites_count"": 115,
    ""utc_offset"": 32400,
    ""time_zone"": ""Tokyo"",
    ""geo_enabled"": false,
    ""verified"": true,
    ""statuses_count"": 3606,
    ""lang"": ""ja"",
    ""contributors_enabled"": false,
    ""is_translator"": false,
    ""is_translation_enabled"": false,
    ""profile_background_color"": ""C0DEED"",
    ""profile_background_image_url"": ""http://pbs.twimg.com/profile_background_images/567482719571959809/WwgdnJCP.jpeg"",
    ""profile_background_image_url_https"": ""https://pbs.twimg.com/profile_background_images/567482719571959809/WwgdnJCP.jpeg"",
    ""profile_background_tile"": true,
    ""profile_image_url"": ""http://pbs.twimg.com/profile_images/3407356865/62f0d53222361fbd2c1fe9889f4cc559_normal.png"",
    ""profile_image_url_https"": ""https://pbs.twimg.com/profile_images/3407356865/62f0d53222361fbd2c1fe9889f4cc559_normal.png"",
    ""profile_banner_url"": ""https://pbs.twimg.com/profile_banners/7080152/1451864421"",
    ""profile_link_color"": ""0084B4"",
    ""profile_sidebar_border_color"": ""FFFFFF"",
    ""profile_sidebar_fill_color"": ""DDEEF6"",
    ""profile_text_color"": ""333333"",
    ""profile_use_background_image"": true,
    ""has_extended_profile"": true,
    ""default_profile"": false,
    ""default_profile_image"": false,
    ""following"": false,
    ""follow_request_sent"": false,
    ""notifications"": false
  },
  ""geo"": null,
  ""coordinates"": null,
  ""place"": null,
  ""contributors"": null,
  ""is_quote_status"": false,
  ""retweet_count"": 61,
  ""favorite_count"": 138,
  ""entities"":  {
    ""hashtags"":  [],
    ""symbols"":  [],
    ""user_mentions"":  [],
    ""urls"":  [],
    ""media"":  [
       {
        ""id"": 675261020978344000,
        ""id_str"": ""675261020978343936"",
        ""indices"":  [
          31,
          54
        ],
        ""media_url"": ""http://pbs.twimg.com/media/CV8CUiYU8AAL3ZV.jpg"",
        ""media_url_https"": ""https://pbs.twimg.com/media/CV8CUiYU8AAL3ZV.jpg"",
        ""url"": ""https://t.co/EoaIP1yeVP"",
        ""display_url"": ""pic.twitter.com/EoaIP1yeVP"",
        ""expanded_url"": ""http://twitter.com/TwitterJP/status/675261031887712256/photo/1"",
        ""type"": ""photo"",
        ""sizes"":  {
          ""large"":  {
            ""w"": 1024,
            ""h"": 768,
            ""resize"": ""fit""
          },
          ""thumb"":  {
            ""w"": 150,
            ""h"": 150,
            ""resize"": ""crop""
          },
          ""medium"":  {
            ""w"": 600,
            ""h"": 450,
            ""resize"": ""fit""
          },
          ""small"":  {
            ""w"": 340,
            ""h"": 255,
            ""resize"": ""fit""
          }
        }
      }
    ]
  },
  ""extended_entities"":  {
    ""media"":  [
       {
        ""id"": 675261020978344000,
        ""id_str"": ""675261020978343936"",
        ""indices"":  [
          31,
          54
        ],
        ""media_url"": ""http://pbs.twimg.com/media/CV8CUiYU8AAL3ZV.jpg"",
        ""media_url_https"": ""https://pbs.twimg.com/media/CV8CUiYU8AAL3ZV.jpg"",
        ""url"": ""https://t.co/EoaIP1yeVP"",
        ""display_url"": ""pic.twitter.com/EoaIP1yeVP"",
        ""expanded_url"": ""http://twitter.com/TwitterJP/status/675261031887712256/photo/1"",
        ""type"": ""photo"",
        ""sizes"":  {
          ""large"":  {
            ""w"": 1024,
            ""h"": 768,
            ""resize"": ""fit""
          },
          ""thumb"":  {
            ""w"": 150,
            ""h"": 150,
            ""resize"": ""crop""
          },
          ""medium"":  {
            ""w"": 600,
            ""h"": 450,
            ""resize"": ""fit""
          },
          ""small"":  {
            ""w"": 340,
            ""h"": 255,
            ""resize"": ""fit""
          }
        }
      },
       {
        ""id"": 675261020978286600,
        ""id_str"": ""675261020978286592"",
        ""indices"":  [
          31,
          54
        ],
        ""media_url"": ""http://pbs.twimg.com/media/CV8CUiYUEAAOTeZ.jpg"",
        ""media_url_https"": ""https://pbs.twimg.com/media/CV8CUiYUEAAOTeZ.jpg"",
        ""url"": ""https://t.co/EoaIP1yeVP"",
        ""display_url"": ""pic.twitter.com/EoaIP1yeVP"",
        ""expanded_url"": ""http://twitter.com/TwitterJP/status/675261031887712256/photo/1"",
        ""type"": ""photo"",
        ""sizes"":  {
          ""large"":  {
            ""w"": 768,
            ""h"": 1024,
            ""resize"": ""fit""
          },
          ""thumb"":  {
            ""w"": 150,
            ""h"": 150,
            ""resize"": ""crop""
          },
          ""small"":  {
            ""w"": 340,
            ""h"": 453,
            ""resize"": ""fit""
          },
          ""medium"":  {
            ""w"": 600,
            ""h"": 800,
            ""resize"": ""fit""
          }
        }
      },
       {
        ""id"": 675261021003448300,
        ""id_str"": ""675261021003448320"",
        ""indices"":  [
          31,
          54
        ],
        ""media_url"": ""http://pbs.twimg.com/media/CV8CUieUAAA8M5m.jpg"",
        ""media_url_https"": ""https://pbs.twimg.com/media/CV8CUieUAAA8M5m.jpg"",
        ""url"": ""https://t.co/EoaIP1yeVP"",
        ""display_url"": ""pic.twitter.com/EoaIP1yeVP"",
        ""expanded_url"": ""http://twitter.com/TwitterJP/status/675261031887712256/photo/1"",
        ""type"": ""photo"",
        ""sizes"":  {
          ""thumb"":  {
            ""w"": 150,
            ""h"": 150,
            ""resize"": ""crop""
          },
          ""medium"":  {
            ""w"": 600,
            ""h"": 800,
            ""resize"": ""fit""
          },
          ""large"":  {
            ""w"": 768,
            ""h"": 1024,
            ""resize"": ""fit""
          },
          ""small"":  {
            ""w"": 340,
            ""h"": 453,
            ""resize"": ""fit""
          }
        }
      }
    ]
  },
  ""favorited"": false,
  ""retweeted"": false,
  ""possibly_sensitive"": false,
  ""possibly_sensitive_appealable"": false,
  ""lang"": ""ja""
}";

        // contains a movie.
        // https://twitter.com/TwitterJP/status/686888903882608642
        private static readonly string _statusJson5 = @"{
  ""created_at"": ""Tue Jan 12 12:34:11 +0000 2016"",
  ""id"": 686888903882608600,
  ""id_str"": ""686888903882608642"",
  ""text"": ""Periscopeの中継動画が、これからはTwitter上でも簡単にご覧いただけるようになります。まずはiOSから提供が始まります。\nhttps://t.co/XW5xLBLRXW https://t.co/9S77eikMaD"",
  ""source"": ""<a href=\""http://twitter.com\"" rel=\""nofollow\"">Twitter Web Client</a>"",
  ""truncated"": false,
  ""in_reply_to_status_id"": null,
  ""in_reply_to_status_id_str"": null,
  ""in_reply_to_user_id"": null,
  ""in_reply_to_user_id_str"": null,
  ""in_reply_to_screen_name"": null,
  ""user"":  {
    ""id"": 7080152,
    ""id_str"": ""7080152"",
    ""name"": ""TwitterJP"",
    ""screen_name"": ""TwitterJP"",
    ""location"": ""東京都中央区"",
    ""description"": ""日本語版Twitter公式アカウントです。サービスに関しては https://t.co/mfQkUQLUhe をご参照ください。"",
    ""url"": ""https://t.co/A9dNuL0CCa"",
    ""entities"":  {
      ""url"":  {
        ""urls"":  [
           {
            ""url"": ""https://t.co/A9dNuL0CCa"",
            ""expanded_url"": ""http://blog.jp.twitter.com"",
            ""display_url"": ""blog.jp.twitter.com"",
            ""indices"":  [
              0,
              23
            ]
          }
        ]
      },
      ""description"":  {
        ""urls"":  [
           {
            ""url"": ""https://t.co/mfQkUQLUhe"",
            ""expanded_url"": ""https://support.twitter.com/"",
            ""display_url"": ""support.twitter.com"",
            ""indices"":  [
              31,
              54
            ]
          }
        ]
      }
    },
    ""protected"": false,
    ""followers_count"": 2106853,
    ""friends_count"": 79,
    ""listed_count"": 16507,
    ""created_at"": ""Tue Jun 26 01:54:35 +0000 2007"",
    ""favourites_count"": 115,
    ""utc_offset"": 32400,
    ""time_zone"": ""Tokyo"",
    ""geo_enabled"": false,
    ""verified"": true,
    ""statuses_count"": 3606,
    ""lang"": ""ja"",
    ""contributors_enabled"": false,
    ""is_translator"": false,
    ""is_translation_enabled"": false,
    ""profile_background_color"": ""C0DEED"",
    ""profile_background_image_url"": ""http://pbs.twimg.com/profile_background_images/567482719571959809/WwgdnJCP.jpeg"",
    ""profile_background_image_url_https"": ""https://pbs.twimg.com/profile_background_images/567482719571959809/WwgdnJCP.jpeg"",
    ""profile_background_tile"": true,
    ""profile_image_url"": ""http://pbs.twimg.com/profile_images/3407356865/62f0d53222361fbd2c1fe9889f4cc559_normal.png"",
    ""profile_image_url_https"": ""https://pbs.twimg.com/profile_images/3407356865/62f0d53222361fbd2c1fe9889f4cc559_normal.png"",
    ""profile_banner_url"": ""https://pbs.twimg.com/profile_banners/7080152/1451864421"",
    ""profile_link_color"": ""0084B4"",
    ""profile_sidebar_border_color"": ""FFFFFF"",
    ""profile_sidebar_fill_color"": ""DDEEF6"",
    ""profile_text_color"": ""333333"",
    ""profile_use_background_image"": true,
    ""has_extended_profile"": true,
    ""default_profile"": false,
    ""default_profile_image"": false,
    ""following"": false,
    ""follow_request_sent"": false,
    ""notifications"": false
  },
  ""geo"": null,
  ""coordinates"": null,
  ""place"": null,
  ""contributors"": null,
  ""is_quote_status"": false,
  ""retweet_count"": 177,
  ""favorite_count"": 176,
  ""entities"":  {
    ""hashtags"":  [],
    ""symbols"":  [],
    ""user_mentions"":  [],
    ""urls"":  [
       {
        ""url"": ""https://t.co/XW5xLBLRXW"",
        ""expanded_url"": ""https://blog.twitter.com/ja/2016/0112periscope"",
        ""display_url"": ""blog.twitter.com/ja/2016/0112pe…"",
        ""indices"":  [
          67,
          90
        ]
      }
    ],
    ""media"":  [
       {
        ""id"": 686888059955089400,
        ""id_str"": ""686888059955089408"",
        ""indices"":  [
          91,
          114
        ],
        ""media_url"": ""http://pbs.twimg.com/ext_tw_video_thumb/686888059955089408/pu/img/4IRtJdu-Lid8GZxS.jpg"",
        ""media_url_https"": ""https://pbs.twimg.com/ext_tw_video_thumb/686888059955089408/pu/img/4IRtJdu-Lid8GZxS.jpg"",
        ""url"": ""https://t.co/9S77eikMaD"",
        ""display_url"": ""pic.twitter.com/9S77eikMaD"",
        ""expanded_url"": ""http://twitter.com/TwitterJP/status/686888903882608642/video/1"",
        ""type"": ""photo"",
        ""sizes"":  {
          ""thumb"":  {
            ""w"": 150,
            ""h"": 150,
            ""resize"": ""crop""
          },
          ""medium"":  {
            ""w"": 600,
            ""h"": 338,
            ""resize"": ""fit""
          },
          ""small"":  {
            ""w"": 340,
            ""h"": 191,
            ""resize"": ""fit""
          },
          ""large"":  {
            ""w"": 1024,
            ""h"": 576,
            ""resize"": ""fit""
          }
        }
      }
    ]
  },
  ""extended_entities"":  {
    ""media"":  [
       {
        ""id"": 686888059955089400,
        ""id_str"": ""686888059955089408"",
        ""indices"":  [
          91,
          114
        ],
        ""media_url"": ""http://pbs.twimg.com/ext_tw_video_thumb/686888059955089408/pu/img/4IRtJdu-Lid8GZxS.jpg"",
        ""media_url_https"": ""https://pbs.twimg.com/ext_tw_video_thumb/686888059955089408/pu/img/4IRtJdu-Lid8GZxS.jpg"",
        ""url"": ""https://t.co/9S77eikMaD"",
        ""display_url"": ""pic.twitter.com/9S77eikMaD"",
        ""expanded_url"": ""http://twitter.com/TwitterJP/status/686888903882608642/video/1"",
        ""type"": ""video"",
        ""sizes"":  {
          ""thumb"":  {
            ""w"": 150,
            ""h"": 150,
            ""resize"": ""crop""
          },
          ""medium"":  {
            ""w"": 600,
            ""h"": 338,
            ""resize"": ""fit""
          },
          ""small"":  {
            ""w"": 340,
            ""h"": 191,
            ""resize"": ""fit""
          },
          ""large"":  {
            ""w"": 1024,
            ""h"": 576,
            ""resize"": ""fit""
          }
        },
        ""video_info"":  {
          ""aspect_ratio"":  [
            16,
            9
          ],
          ""duration_millis"": 20000,
          ""variants"":  [
             {
              ""bitrate"": 832000,
              ""content_type"": ""video/mp4"",
              ""url"": ""https://video.twimg.com/ext_tw_video/686888059955089408/pu/vid/640x360/JPPa0-aIOehG3tyU.mp4""
            },
             {
              ""bitrate"": 320000,
              ""content_type"": ""video/mp4"",
              ""url"": ""https://video.twimg.com/ext_tw_video/686888059955089408/pu/vid/320x180/IGxB8aGhE311Wk7C.mp4""
            },
             {
              ""content_type"": ""application/dash+xml"",
              ""url"": ""https://video.twimg.com/ext_tw_video/686888059955089408/pu/pl/JIgNQEW6tHLe7V3d.mpd""
            },
             {
              ""bitrate"": 832000,
              ""content_type"": ""video/webm"",
              ""url"": ""https://video.twimg.com/ext_tw_video/686888059955089408/pu/vid/640x360/JPPa0-aIOehG3tyU.webm""
            },
             {
              ""content_type"": ""application/x-mpegURL"",
              ""url"": ""https://video.twimg.com/ext_tw_video/686888059955089408/pu/pl/JIgNQEW6tHLe7V3d.m3u8""
            },
             {
              ""bitrate"": 2176000,
              ""content_type"": ""video/mp4"",
              ""url"": ""https://video.twimg.com/ext_tw_video/686888059955089408/pu/vid/1280x720/i45Ro4Inh-Jsm7Ao.mp4""
            }
          ]
        }
      }
    ]
  },
  ""favorited"": false,
  ""retweeted"": false,
  ""possibly_sensitive"": false,
  ""possibly_sensitive_appealable"": false,
  ""lang"": ""ja""
}";

        private static readonly string _streamDeleteJson = @"{
  ""delete"":{
    ""status"":{
      ""id"":1234,
      ""id_str"":""1234"",
      ""user_id"":3,
      ""user_id_str"":""3""
    }
  }
}";

        private static readonly string _streamScrubGeoJson = @"{
  ""scrub_geo"":{
    ""user_id"":14090452,
    ""user_id_str"":""14090452"",
    ""up_to_status_id"":23260136625,
    ""up_to_status_id_str"":""23260136625""
  }
}";

        private static readonly string _streamLimitNoticeJson = @"{
  ""limit"":{
    ""track"":1234
  }
}";
        private static readonly string _streamStatusWithheldJson = @"{
  ""status_withheld"":{
      ""id"":1234567890,
      ""user_id"":123456,
      ""withheld_in_countries"":[""DE"", ""AR""]
  }
}";
        private static readonly string _streamUserWithheldJson = @"{ 
  ""user_withheld"":{
    ""id"":123456,
    ""withheld_in_countries"":[""DE"",""AR""]
  }
}";


        private static readonly string _streamDisconnectJson = @"{
  ""disconnect"":{
    ""code"": 4,
    ""stream_name"":"""",
    ""reason"":""""
  }
}";

        private static readonly string _streamStallJson = @"{
  ""warning"":{
    ""code"":""FALLING_BEHIND"",
    ""message"":""Your connection is falling behind and messages are being queued for delivery to you. Your queue is now over 60% full. You will be disconnected when the queue is full."",
    ""percent_full"": 60
  }
}";

        private static readonly string _userObjectJson = @"{
  ""id"": 167164816,
  ""id_str"": ""167164816"",
  ""name"": ""Twitterサポート"",
  ""screen_name"": ""TwitterHelpJP"",
  ""location"": """",
  ""profile_location"": null,
  ""description"": ""Twitterに関する不具合や新機能をツイートでご報告！お困りの際、まずこちらのツイートを確認しhttp://t.co/WjE2P04PxT をご覧下さい。そちらからお問い合せも可能です。@ツイートやDMをいただいても回答することができませんのでご了承ください。"",
  ""url"": ""http://t.co/WjE2P04PxT"",
  ""entities"":  {
    ""url"":  {
      ""urls"":  [
         {
          ""url"": ""http://t.co/WjE2P04PxT"",
          ""expanded_url"": ""http://support.twitter.com"",
          ""display_url"": ""support.twitter.com"",
          ""indices"":  [
            0,
            22
          ]
        }
      ]
    },
    ""description"":  {
      ""urls"":  [
         {
          ""url"": ""http://t.co/WjE2P04PxT"",
          ""expanded_url"": ""http://support.twitter.com"",
          ""display_url"": ""support.twitter.com"",
          ""indices"":  [
            48,
            70
          ]
        }
      ]
    }
  },
  ""protected"": false,
  ""followers_count"": 762637,
  ""friends_count"": 18,
  ""listed_count"": 7679,
  ""created_at"": ""Thu Jul 15 22:37:46 +0000 2010"",
  ""favourites_count"": 25,
  ""utc_offset"": -28800,
  ""time_zone"": ""Pacific Time (US & Canada)"",
  ""geo_enabled"": false,
  ""verified"": true,
  ""statuses_count"": 4313,
  ""lang"": ""ja"",
  ""status"":  {
    ""created_at"": ""Tue Jan 19 00:00:15 +0000 2016"",
    ""id"": 689235887599693800,
    ""id_str"": ""689235887599693828"",
    ""text"": ""好きなユーザーがツイートした画像や動画を見たい場合は、そのユーザーのプロフィールページにある [画像/動画] タイムラインからご覧になれます。🌅🚢🌉👀"",
    ""source"": ""<a href=\""https://about.twitter.com/products/tweetdeck\"" rel=\""nofollow\"">TweetDeck</a>"",
    ""truncated"": false,
    ""in_reply_to_status_id"": null,
    ""in_reply_to_status_id_str"": null,
    ""in_reply_to_user_id"": null,
    ""in_reply_to_user_id_str"": null,
    ""in_reply_to_screen_name"": null,
    ""geo"": null,
    ""coordinates"": null,
    ""place"": null,
    ""contributors"": null,
    ""is_quote_status"": false,
    ""retweet_count"": 38,
    ""favorite_count"": 91,
    ""entities"":  {
      ""hashtags"":  [],
      ""symbols"":  [],
      ""user_mentions"":  [],
      ""urls"":  []
    },
    ""favorited"": false,
    ""retweeted"": false,
    ""lang"": ""ja""
  },
  ""contributors_enabled"": false,
  ""is_translator"": false,
  ""is_translation_enabled"": false,
  ""profile_background_color"": ""C0DEED"",
  ""profile_background_image_url"": ""http://pbs.twimg.com/profile_background_images/819903197/ffaad5bca02cc4536400f81345e5683d.png"",
  ""profile_background_image_url_https"": ""https://pbs.twimg.com/profile_background_images/819903197/ffaad5bca02cc4536400f81345e5683d.png"",
  ""profile_background_tile"": true,
  ""profile_image_url"": ""http://pbs.twimg.com/profile_images/2284174748/o26wjnpmzstufiwiq6a7_normal.png"",
  ""profile_image_url_https"": ""https://pbs.twimg.com/profile_images/2284174748/o26wjnpmzstufiwiq6a7_normal.png"",
  ""profile_banner_url"": ""https://pbs.twimg.com/profile_banners/167164816/1347989456"",
  ""profile_link_color"": ""0084B4"",
  ""profile_sidebar_border_color"": ""FFFFFF"",
  ""profile_sidebar_fill_color"": ""DDEEF6"",
  ""profile_text_color"": ""333333"",
  ""profile_use_background_image"": true,
  ""has_extended_profile"": false,
  ""default_profile"": false,
  ""default_profile_image"": false,
  ""following"": false,
  ""follow_request_sent"": false,
  ""notifications"": false
}";

        private static readonly string _clientApplicationJson = "\"client_application\"";

        private static readonly string _listObjectJson1 = @"{
    ""id"": 208584152,
    ""id_str"": ""208584152"",
    ""name"": ""二郎を食べ続ける人"",
    ""uri"": ""/karno/lists/list"",
    ""subscriber_count"": 0,
    ""member_count"": 1,
    ""mode"": ""public"",
    ""description"": ""興味深い"",
    ""slug"": ""list"",
    ""full_name"": ""@karno/list"",
    ""created_at"": ""Tue May 26 13:06:36 +0000 2015"",
    ""following"": true,
    ""user"":  {
      ""id"": 14157941,
      ""id_str"": ""14157941"",
      ""name"": ""ソフィー・ノイエンミュラー"",
      ""screen_name"": ""karno"",
      ""location"": ""キルヘン・ベルの町外れ"",
      ""description"": ""キルヘン・ベルの街はずれでアトリエを営んでいる少女。明るくほわっとした性格で周りを和ませるが、時間にルーズであるなど、ずぼらなところも多い。[アトリエシリーズ/ごちうさ/きんモザ/ゆゆ式/ろこどる など](icon: @yaplus)"",
      ""url"": ""https://t.co/lgtRJUv91j"",
      ""entities"":  {
        ""url"":  {
          ""urls"":  [
             {
              ""url"": ""https://t.co/lgtRJUv91j"",
              ""expanded_url"": ""http://social.gust.co.jp/sophie/"",
              ""display_url"": ""social.gust.co.jp/sophie/"",
              ""indices"":  [
                0,
                23
              ]
            }
          ]
        },
        ""description"":  {
          ""urls"":  []
        }
      },
      ""protected"": true,
      ""followers_count"": 4132,
      ""friends_count"": 905,
      ""listed_count"": 424,
      ""created_at"": ""Sun Mar 16 15:46:37 +0000 2008"",
      ""favourites_count"": 185758,
      ""utc_offset"": 32400,
      ""time_zone"": ""Tokyo"",
      ""geo_enabled"": false,
      ""verified"": false,
      ""statuses_count"": 385644,
      ""lang"": ""en"",
      ""contributors_enabled"": false,
      ""is_translator"": false,
      ""is_translation_enabled"": false,
      ""profile_background_color"": ""DBE9ED"",
      ""profile_background_image_url"": ""http://pbs.twimg.com/profile_background_images/613006118197964800/BuDzS_fS.jpg"",
      ""profile_background_image_url_https"": ""https://pbs.twimg.com/profile_background_images/613006118197964800/BuDzS_fS.jpg"",
      ""profile_background_tile"": false,
      ""profile_image_url"": ""http://pbs.twimg.com/profile_images/688563897498914816/acAUYKFU_normal.png"",
      ""profile_image_url_https"": ""https://pbs.twimg.com/profile_images/688563897498914816/acAUYKFU_normal.png"",
      ""profile_banner_url"": ""https://pbs.twimg.com/profile_banners/14157941/1441710382"",
      ""profile_link_color"": ""CC3366"",
      ""profile_sidebar_border_color"": ""FFFFFF"",
      ""profile_sidebar_fill_color"": ""E6F6F9"",
      ""profile_text_color"": ""333333"",
      ""profile_use_background_image"": true,
      ""has_extended_profile"": true,
      ""default_profile"": false,
      ""default_profile_image"": false,
      ""following"": false,
      ""follow_request_sent"": false,
      ""notifications"": false
    }
}";

        private static readonly string _listObjectJson2 = @"{
    ""id"": 175568310,
    ""id_str"": ""175568310"",
    ""name"": ""alchemists"",
    ""uri"": ""/karno/lists/alchemists"",
    ""subscriber_count"": 3,
    ""member_count"": 15,
    ""mode"": ""public"",
    ""description"": ""錬金術士各位"",
    ""slug"": ""alchemists"",
    ""full_name"": ""@karno/alchemists"",
    ""created_at"": ""Fri Oct 24 19:13:42 +0000 2014"",
    ""following"": true,
    ""user"":  {
      ""id"": 14157941,
      ""id_str"": ""14157941"",
      ""name"": ""ソフィー・ノイエンミュラー"",
      ""screen_name"": ""karno"",
      ""location"": ""キルヘン・ベルの町外れ"",
      ""description"": ""キルヘン・ベルの街はずれでアトリエを営んでいる少女。明るくほわっとした性格で周りを和ませるが、時間にルーズであるなど、ずぼらなところも多い。[アトリエシリーズ/ごちうさ/きんモザ/ゆゆ式/ろこどる など](icon: @yaplus)"",
      ""url"": ""https://t.co/lgtRJUv91j"",
      ""entities"":  {
        ""url"":  {
          ""urls"":  [
             {
              ""url"": ""https://t.co/lgtRJUv91j"",
              ""expanded_url"": ""http://social.gust.co.jp/sophie/"",
              ""display_url"": ""social.gust.co.jp/sophie/"",
              ""indices"":  [
                0,
                23
              ]
            }
          ]
        },
        ""description"":  {
          ""urls"":  []
        }
      },
      ""protected"": true,
      ""followers_count"": 4132,
      ""friends_count"": 905,
      ""listed_count"": 424,
      ""created_at"": ""Sun Mar 16 15:46:37 +0000 2008"",
      ""favourites_count"": 185758,
      ""utc_offset"": 32400,
      ""time_zone"": ""Tokyo"",
      ""geo_enabled"": false,
      ""verified"": false,
      ""statuses_count"": 385644,
      ""lang"": ""en"",
      ""contributors_enabled"": false,
      ""is_translator"": false,
      ""is_translation_enabled"": false,
      ""profile_background_color"": ""DBE9ED"",
      ""profile_background_image_url"": ""http://pbs.twimg.com/profile_background_images/613006118197964800/BuDzS_fS.jpg"",
      ""profile_background_image_url_https"": ""https://pbs.twimg.com/profile_background_images/613006118197964800/BuDzS_fS.jpg"",
      ""profile_background_tile"": false,
      ""profile_image_url"": ""http://pbs.twimg.com/profile_images/688563897498914816/acAUYKFU_normal.png"",
      ""profile_image_url_https"": ""https://pbs.twimg.com/profile_images/688563897498914816/acAUYKFU_normal.png"",
      ""profile_banner_url"": ""https://pbs.twimg.com/profile_banners/14157941/1441710382"",
      ""profile_link_color"": ""CC3366"",
      ""profile_sidebar_border_color"": ""FFFFFF"",
      ""profile_sidebar_fill_color"": ""E6F6F9"",
      ""profile_text_color"": ""333333"",
      ""profile_use_background_image"": true,
      ""has_extended_profile"": true,
      ""default_profile"": false,
      ""default_profile_image"": false,
      ""following"": false,
      ""follow_request_sent"": false,
      ""notifications"": false
    }
  }";

        private static readonly string _listObjectJson3 = @"{
    ""id"": 69108535,
    ""id_str"": ""69108535"",
    ""name"": ""toshi_a"",
    ""uri"": ""/karno/lists/toshi-a"",
    ""subscriber_count"": 0,
    ""member_count"": 1,
    ""mode"": ""public"",
    ""description"": ""ておくれとしぁさん"",
    ""slug"": ""toshi-a"",
    ""full_name"": ""@karno/toshi-a"",
    ""created_at"": ""Wed Apr 18 09:17:45 +0000 2012"",
    ""following"": true,
    ""user"":  {
      ""id"": 14157941,
      ""id_str"": ""14157941"",
      ""name"": ""ソフィー・ノイエンミュラー"",
      ""screen_name"": ""karno"",
      ""location"": ""キルヘン・ベルの町外れ"",
      ""description"": ""キルヘン・ベルの街はずれでアトリエを営んでいる少女。明るくほわっとした性格で周りを和ませるが、時間にルーズであるなど、ずぼらなところも多い。[アトリエシリーズ/ごちうさ/きんモザ/ゆゆ式/ろこどる など](icon: @yaplus)"",
      ""url"": ""https://t.co/lgtRJUv91j"",
      ""entities"":  {
        ""url"":  {
          ""urls"":  [
             {
              ""url"": ""https://t.co/lgtRJUv91j"",
              ""expanded_url"": ""http://social.gust.co.jp/sophie/"",
              ""display_url"": ""social.gust.co.jp/sophie/"",
              ""indices"":  [
                0,
                23
              ]
            }
          ]
        },
        ""description"":  {
          ""urls"":  []
        }
      },
      ""protected"": true,
      ""followers_count"": 4132,
      ""friends_count"": 905,
      ""listed_count"": 424,
      ""created_at"": ""Sun Mar 16 15:46:37 +0000 2008"",
      ""favourites_count"": 185758,
      ""utc_offset"": 32400,
      ""time_zone"": ""Tokyo"",
      ""geo_enabled"": false,
      ""verified"": false,
      ""statuses_count"": 385644,
      ""lang"": ""en"",
      ""contributors_enabled"": false,
      ""is_translator"": false,
      ""is_translation_enabled"": false,
      ""profile_background_color"": ""DBE9ED"",
      ""profile_background_image_url"": ""http://pbs.twimg.com/profile_background_images/613006118197964800/BuDzS_fS.jpg"",
      ""profile_background_image_url_https"": ""https://pbs.twimg.com/profile_background_images/613006118197964800/BuDzS_fS.jpg"",
      ""profile_background_tile"": false,
      ""profile_image_url"": ""http://pbs.twimg.com/profile_images/688563897498914816/acAUYKFU_normal.png"",
      ""profile_image_url_https"": ""https://pbs.twimg.com/profile_images/688563897498914816/acAUYKFU_normal.png"",
      ""profile_banner_url"": ""https://pbs.twimg.com/profile_banners/14157941/1441710382"",
      ""profile_link_color"": ""CC3366"",
      ""profile_sidebar_border_color"": ""FFFFFF"",
      ""profile_sidebar_fill_color"": ""E6F6F9"",
      ""profile_text_color"": ""333333"",
      ""profile_use_background_image"": true,
      ""has_extended_profile"": true,
      ""default_profile"": false,
      ""default_profile_image"": false,
      ""following"": false,
      ""follow_request_sent"": false,
      ""notifications"": false
    }
  }";

        private static readonly string _friendListJson = @"{
  ""friends"":[
    1497,
    169686021,
    790205,
    15211564,
    1145141919,
    1145141920,
    1145141921,
    1145141922,
    1145141923,
    1145141924,
    1145141925,
    1145141926,
    1145141927,
    1145141928,
    1145141929,
    1145141930,
    1145141931,
    1145141932,
    1145141933,
    1145141934,
    1145141935,
    1145141936,
    1145141937,
    1145141938,
    1145141939,
    9223372036854775806,
    9223372036854775807
  ]
}";

        private static readonly string _friendListStringJson = @"{
""friends"":[
    ""1497"",
    ""169686021"",
    ""790205"",
    ""15211564"",
    ""1145141919"",
    ""1145141920"",
    ""1145141921"",
    ""1145141922"",
    ""1145141923"",
    ""1145141924"",
    ""1145141925"",
    ""1145141926"",
    ""1145141927"",
    ""1145141928"",
    ""1145141929"",
    ""1145141930"",
    ""1145141931"",
    ""1145141932"",
    ""1145141933"",
    ""1145141934"",
    ""1145141935"",
    ""1145141936"",
    ""1145141937"",
    ""1145141938"",
    ""1145141939"",
    ""9223372036854775806"",
    ""9223372036854775807""
  ]
}";

        private static readonly string _eventAccessRevokedJson = BuildEventString("access_revoked",
            _userObjectJson, _userObjectJson, _clientApplicationJson);

        private static readonly string _eventBlockJson = BuildEventString("block",
            _userObjectJson, _userObjectJson, null);

        private static readonly string _eventUnblockJson = BuildEventString("unblock",
            _userObjectJson, _userObjectJson, null);

        private static readonly string _eventFavoriteJson = BuildEventString("favorite",
            _userObjectJson, _userObjectJson, _statusJson1);

        private static readonly string _eventUnfavoriteJson = BuildEventString("unfavorite",
            _userObjectJson, _userObjectJson, _statusJson1);

        private static readonly string _eventFollowJson = BuildEventString("follow",
            _userObjectJson, _userObjectJson, null);

        private static readonly string _eventUnfollowJson = BuildEventString("unfollow",
            _userObjectJson, _userObjectJson, null);

        private static readonly string _eventListMemberAddedJson = BuildEventString("list_member_added",
            _userObjectJson, _userObjectJson, _listObjectJson1);

        private static readonly string _eventListMemberRemovedJson = BuildEventString("list_member_removed",
            _userObjectJson, _userObjectJson, _listObjectJson1);

        private static readonly string _eventListMemberSubscribedJson = BuildEventString("list_member_subscribed",
            _userObjectJson, _userObjectJson, _listObjectJson1);

        private static readonly string _eventListMemberUnsubscribedJson = BuildEventString("list_member_unsubscribed",
            _userObjectJson, _userObjectJson, _listObjectJson1);

        private static readonly string _eventQuotedJson = BuildEventString("quoted_tweet",
            _userObjectJson, _userObjectJson, _statusJson1);

        private static readonly string _eventUserUpdateJson = BuildEventString("user_update",
            _userObjectJson, _userObjectJson, null);


        private static string BuildEventString(string eventName, string source, string target, string targetObject)
        {
            var template = @"{{
  ""event"":""{0}"",
  ""created_at"": ""Sat Sep 4 16:10:54 +0000 2010"",
  ""target"": {2},
  ""source"": {1},
  ""target_object"": {3}
}}";
            if (targetObject == null)
            {
                targetObject = "null";
            }
            return String.Format(template, eventName, source, target, targetObject);
        }

        /// <summary>
        /// Get sample status and JSON representation.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Tuple<TwitterStatus, string>> GetTweetSamples()
        {
            var statuses = new[] { _statusJson0, _statusJson1, _statusJson2, _statusJson3, _statusJson4, _statusJson5 };
            while (true)
            {
                foreach (var status in statuses)
                {
                    var stobj = new TwitterStatus(MeteorJson.Parse(status));
                    yield return Tuple.Create(stobj, status);
                }
            }
        }

        /// <summary>
        /// Get stream json payload samples.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetStreamSampleElements()
        {
            yield return _friendListStringJson.Unline() + Environment.NewLine;
            var statuses = new[] { _statusJson0, _statusJson1, _statusJson2, _statusJson3, _statusJson4, _statusJson5 };
            var notifications = new[]
            {
                // we shouldn't use revoked, this is not have stable specification.
                // _eventAccessRevokedJson,
                _eventBlockJson,
                _eventFavoriteJson,
                _eventFollowJson,
                _eventListMemberAddedJson,
                _eventListMemberRemovedJson,
                _eventListMemberSubscribedJson,
                _eventListMemberUnsubscribedJson,
                _eventQuotedJson,
                _eventUnblockJson,
                _eventUnfavoriteJson,
                _eventUnfollowJson,
                _eventUserUpdateJson
            };
            var joined = statuses.Concat(notifications)
                .Select(s => s.Unline())
                .ToArray();
            foreach (var s in joined)
            {
                yield return s;
            }
        }


        /// <summary>
        /// Get stream json payload samples.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetStreamSamples()
        {
            yield return _friendListStringJson.Unline() + Environment.NewLine;
            var statuses = new[] { _statusJson0, _statusJson1, _statusJson2, _statusJson3, _statusJson4, _statusJson5 };
            var notifications = new[]
            {
                // we shouldn't use revoked, this is not have stable specification.
                // _eventAccessRevokedJson,
                _eventBlockJson,
                _eventFavoriteJson,
                _eventFollowJson,
                _eventListMemberAddedJson,
                _eventListMemberRemovedJson,
                _eventListMemberSubscribedJson,
                _eventListMemberUnsubscribedJson,
                _eventQuotedJson,
                _eventUnblockJson,
                _eventUnfavoriteJson,
                _eventUnfollowJson,
                _eventUserUpdateJson
            };
            var joined = statuses.Concat(statuses).Concat(statuses).Concat(statuses).Concat(notifications)
                .Select(s => s.Unline())
                .ToArray();
            var rand = new Random();
            while (true)
            {
                var index = rand.Next(joined.Length);
                yield return joined[index] + Environment.NewLine;
            }
        }

        /// <summary>
        /// Get stream json payload samples.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<byte[]> GetBinalyStreamSamples()
        {
            yield return Encoding.UTF8.GetBytes(_friendListStringJson.Unline() + Environment.NewLine);
            var statuses = new[] { _statusJson0, _statusJson1, _statusJson2, _statusJson3, _statusJson4, _statusJson5 };
            var notifications = new[]
            {
                // we shouldn't use revoked, this is not have stable specification.
                // _eventAccessRevokedJson,
                _eventBlockJson,
                _eventFavoriteJson,
                _eventFollowJson,
                _eventListMemberAddedJson,
                _eventListMemberRemovedJson,
                _eventListMemberSubscribedJson,
                _eventListMemberUnsubscribedJson,
                _eventQuotedJson,
                _eventUnblockJson,
                _eventUnfavoriteJson,
                _eventUnfollowJson,
                _eventUserUpdateJson
            };
            var joined = statuses.Concat(notifications)
                                 .Select(s => s.Unline())
                                 .Select(s => s + Environment.NewLine)
                                 .Select(Encoding.UTF8.GetBytes)
                                 .ToArray();
            var rand = new Random();
            while (true)
            {
                var index = rand.Next(joined.Length);
                yield return joined[index];
            }
        }

        private static string Unline(this string readableJson)
        {
            return readableJson
                .Replace("/", "\\/")
                .Replace("\r", "")
                .Replace("\n", "");
        }
    }
}
