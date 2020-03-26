using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Dash.Scripts.Assets;
using LeanCloud;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;
using Url = Flurl.Url;

namespace Dash.Scripts.Network.Cloud
{
    public static class CloudManager
    {
        private const string errorMessage = "网络错误";

        private const string errorMessageInLogIn = "网络错误或玩家不存在";

        private const string errorMessageInternal = "游戏内部错误";

        private static string GetCallbackUrl()
        {
            return $"http://localhost:{FreePort.GetFirstAvailablePort()}/oauth/redirect/";
        }

        private static EUserMate localUserMate;

        private static EWeapon NewWeapon(int typeId)
        {
            var weapon = new EWeapon();
            weapon.user = AVUser.CurrentUser;
            weapon.typeId = typeId;
            weapon.exp = 0;
            return weapon;
        }

        private static EShengHen NewShengHen(this EPlayer player, int typeId)
        {
            var shengHen = new EShengHen();
            shengHen.player = player;
            shengHen.user = AVUser.CurrentUser;
            shengHen.typeId = typeId;
            shengHen.exp = 0;
            return shengHen;
        }

        private static List<AVObject> NewPlayer(int typeId)
        {
            List<AVObject> list = new List<AVObject>();
            var player = new EPlayer();
            player.user = AVUser.CurrentUser;
            player.typeId = typeId;
            player.exp = 0;
            var inUseWeapon = new List<EInUseWeapon>();
            var inUseShengHen = new List<EInUseShengHen>();
            for (int i = 0; i < 3; i++)
            {
                inUseWeapon.Add(new EInUseWeapon
                {
                    user = AVUser.CurrentUser,
                    player = player,
                    index = i,
                    weapon = null
                });
                inUseShengHen.Add(new EInUseShengHen
                {
                    user = AVUser.CurrentUser,
                    player = player,
                    index = i,
                    shengHen = null
                });
            }

            var w = NewWeapon(0);
            w.player = player;
            inUseWeapon[0].weapon = w;
            list.Add(player);
            list.Add(w);
            list.AddRange(inUseWeapon);
            list.AddRange(inUseShengHen);
            return list;
        }


        public static void LuckDraw(Action<LuckDrawResult, string> callback)
        {
            var type = Random.Range(0, 3);
            switch (type)
            {
                case 0:
                {
                    var playerTypeId = Random.Range(0, GameInfoManager.maxWeaponId + 1);
                    new AVQuery<EPlayer>()
                        .WhereEqualTo("user", AVUser.CurrentUser)
                        .WhereEqualTo("typeId", playerTypeId)
                        .FindAsync()
                        .Post(t =>
                            {
                                if (t.IsCanceled || t.IsFaulted)
                                {
                                    Debug.Log(t.Exception);
                                    callback(null, errorMessage);
                                    return;
                                }

                                var player = t.Result.ToList();
                                if (player.Count == 0)
                                {
                                    var p = NewPlayer(playerTypeId);
                                    AVObject.SaveAllAsync(p).Post(t2 =>
                                    {
                                        if (t.IsCanceled || t.IsFaulted)
                                        {
                                            Debug.Log(t.Exception);
                                            callback(null, errorMessage);
                                            return;
                                        }

                                        callback(new LuckDrawResult
                                        {
                                            typeId = playerTypeId,
                                            resultType = LuckDrawResult.Type.UnLockPlayer
                                        }, null);
                                    });
                                }
                                else
                                {
                                    var p = player.First();
                                    p.exp += GameInfoManager.gameInfoAsset.playerLuckDrawExpAddOnce;
                                    p.SaveAsync()
                                        .Post(t2 =>
                                        {
                                            if (t.IsCanceled || t.IsFaulted)
                                            {
                                                Debug.Log(t.Exception);
                                                callback(null, errorMessage);
                                                return;
                                            }

                                            callback(new LuckDrawResult
                                            {
                                                typeId = playerTypeId,
                                                resultType = LuckDrawResult.Type.AddPlayerExp
                                            }, null);
                                        });
                                }
                            }
                        );
                    break;
                }

                case 1:
                {
                    var weaponTypeId = Random.Range(0, GameInfoManager.maxWeaponId + 1);
                    NewWeapon(weaponTypeId)
                        .SaveAsync()
                        .Post(t =>
                        {
                            if (t.IsCanceled || t.IsFaulted)
                            {
                                Debug.Log(t.Exception);
                                callback(null, errorMessage);
                                return;
                            }

                            callback(new LuckDrawResult
                            {
                                typeId = weaponTypeId,
                                resultType = LuckDrawResult.Type.Weapon
                            }, null);
                        });
                }
                    break;
                default:
                {
                    var shengHenTypeId = Random.Range(0, GameInfoManager.maxShengHeId + 1);
                    NewShengHen(null, shengHenTypeId)
                        .SaveAsync()
                        .Post(t =>
                        {
                            if (t.IsCanceled || t.IsFaulted)
                            {
                                Debug.Log(t.Exception);
                                callback(null, errorMessage);
                                return;
                            }

                            callback(new LuckDrawResult
                            {
                                typeId = shengHenTypeId,
                                resultType = LuckDrawResult.Type.ShengHen
                            }, null);
                        });
                }
                    break;
            }
        }

        public static void GetUserShengHen(Action<Dictionary<string, EShengHen>, string> callback)
        {
            new AVQuery<EShengHen>()
                .Include("player")
                .WhereEqualTo("user", AVUser.CurrentUser)
                .FindAsync()
                .Post(t =>
                {
                    if (t.IsCanceled || t.IsFaulted)
                    {
                        Debug.Log(t.Exception);
                        callback(null, errorMessage);
                        return;
                    }

                    callback(t.Result.ToDictionary(o => o.ObjectId, o => o), null);
                });
        }

        public static void GetUserWeapons(Action<Dictionary<string, EWeapon>, string> callback)
        {
            new AVQuery<EWeapon>()
                .Include("player")
                .WhereEqualTo("user", AVUser.CurrentUser)
                .FindAsync()
                .Post(t =>
                {
                    if (t.IsCanceled || t.IsFaulted)
                    {
                        Debug.Log(t.Exception);
                        callback(null, errorMessage);
                        return;
                    }

                    callback(t.Result.ToDictionary(o => o.ObjectId, o => o), null);
                });
        }

        public static void GetEquipments(Action<Equipments, string> callback)
        {
            var t0 = new AVQuery<EWeapon>()
                .Include("player")
                .WhereEqualTo("user", AVUser.CurrentUser)
                .FindAsync();
            var t1 = new AVQuery<EShengHen>()
                .Include("player")
                .WhereEqualTo("user", AVUser.CurrentUser)
                .FindAsync();
            var t2 = new AVQuery<EInUseWeapon>()
                .Include("player")
                .Include("weapon")
                .WhereEqualTo("user", AVUser.CurrentUser)
                .FindAsync();
            var t3 = new AVQuery<EInUseShengHen>()
                .Include("player")
                .Include("shengHen")
                .WhereEqualTo("user", AVUser.CurrentUser)
                .FindAsync();

            Task.WhenAll(t0, t1, t2, t3)
                .Post(t =>
                {
                    if (t.IsCanceled || t.IsFaulted)
                    {
                        Debug.Log(t.Exception);
                        callback(null, errorMessage);
                        return;
                    }

                    var e = new Equipments();
                    foreach (var item in t2.Result)
                    {
                        PlayerWithUsing inUse;
                        e.players.TryGetValue(item.player.ObjectId, out inUse);
                        if (inUse == null)
                        {
                            inUse = new PlayerWithUsing
                            {
                                player = item.player
                            };
                            e.players.Add(item.player.ObjectId, inUse);
                        }

                        inUse.weapons.Add(item);
                    }


                    foreach (var item in t3.Result)
                    {
                        PlayerWithUsing inUse;
                        e.players.TryGetValue(item.player.ObjectId, out inUse);
                        if (inUse == null)
                        {
                            inUse = new PlayerWithUsing
                            {
                                player = item.player
                            };
                            e.players.Add(item.player.ObjectId, inUse);
                        }

                        inUse.shengHens.Add(item);
                    }

                    foreach (var item in t0.Result)
                    {
                        e.weapons.Add(item.ObjectId, item);
                    }

                    foreach (var item in t1.Result)
                    {
                        e.shengHens.Add(item.ObjectId, item);
                    }

                    foreach (var inUse in e.players.Values)
                    {
                        inUse.shengHens.Sort((o1, o2) => o1.index.CompareTo(o2.index));
                        inUse.weapons.Sort((o1, o2) => o1.index.CompareTo(o2.index));
                    }

                    callback(e, null);
                });
        }

        public static void SignUp(string username, string password, string password2, Action<string> callback)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(password2))
            {
                callback("用户名和密码不能为空");
                return;
            }

            if (password != password2)
            {
                callback("两次输入的密码不一致");
                return;
            }

            var myUser = new AVUser
            {
                Password = password, Username = username
            };
            myUser.SignUpAsync()
                .Post(t =>
                    {
                        if (t.IsFaulted || t.IsCanceled)
                        {
                            Debug.Log(t.Exception);
                            callback("账号重复或网络不通");
                        }
                        else
                        {
                            callback(null);
                        }
                    }
                );
        }


        private static void LoginToPost(Task<AVUser> t, Action<string> callback)
        {
            if (t.IsCanceled || t.IsFaulted)
            {
                Debug.Log(t.Exception);
                callback(errorMessageInLogIn);
                return;
            }

            var t1 = new AVQuery<EUserMate>()
                .Include("player")
                .WhereEqualTo("user", AVUser.CurrentUser)
                .FindAsync();
            var t2 = new AVQuery<EPlayer>()
                .WhereEqualTo("user", AVUser.CurrentUser)
                .CountAsync();
            Task.WhenAll(t1, t2).Post(all =>
                {
                    if (all.IsCanceled || all.IsFaulted)
                    {
                        Debug.Log(all.Exception);
                        callback(errorMessageInLogIn);
                        return;
                    }

                    var userMate = t1.Result.FirstOrDefault() ?? new EUserMate
                    {
                        user = AVUser.CurrentUser
                    };
                    localUserMate = userMate;
                    var count = t2.Result;
                    if (count == 0)
                    {
                        var p = NewPlayer(0);
                        userMate.player = (EPlayer) p[0];
                        p.Add(userMate);
                        AVObject.SaveAllAsync(p).Post(t3 =>
                            {
                                if (t3.IsCanceled || t3.IsFaulted)
                                {
                                    Debug.Log(t3.Exception);
                                    callback(errorMessageInLogIn);
                                    return;
                                }

                                callback(null);
                            }
                        );
                    }
                    else
                    {
                        callback(null);
                    }
                }
            );
        }

        public static void LogIn(string username, string password, Action<string> callback)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                callback("用户名和密码不能为空");
                return;
            }

            AVUser.LogInAsync(username, password).Post(t => LoginToPost(t, callback));
        }

        public static string GetGithubUrlAndWaitToken(Action<string, string> callback, out Thread waitThread)
        {
            var info = GameSDKManager.instance.info;
            var callbackUrl = GetCallbackUrl();
            var login = new Url("https://github.com/login/oauth/authorize");
            login.SetQueryParam("client_id", info.githubClientId);
            login.SetQueryParam("redirect_uri", callbackUrl);
            var url = login.ToString();
            waitThread = new Thread(async () =>
            {
                try
                {
                    var http = new HttpListener {AuthenticationSchemes = AuthenticationSchemes.Anonymous};
                    http.Prefixes.Add(callbackUrl);
                    http.Start();
                    var ctx = await http.GetContextAsync();
                    var code = ctx.Request.QueryString["code"];
                    http.Close();
                    Debug.Log(code);
                    var u = new Url("https://github.com/login/oauth/access_token");
                    u.SetQueryParam("client_id", info.githubClientId);
                    u.SetQueryParam("client_secret", info.githubClientSecret);
                    u.SetQueryParam("code", code);
                    var url2 = u.ToString();
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        var wr = UnityWebRequest.Post(url2, new Dictionary<string, string>());
                        wr.SetRequestHeader("Accept", "application/json");
                        wr.SendWebRequest().completed += w =>
                        {
                            var token =
                                JsonConvert.DeserializeObject<Dictionary<string, string>>(wr.downloadHandler.text)[
                                    "access_token"];
                            callback(null, token);
                        };
                    });
                }
                catch (ThreadAbortException)
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => { callback(errorMessage, null); });
                }
                finally
                {
                    Debug.Log("Task Finish");
                }
            }) {IsBackground = true};
            waitThread.Start();
            return url;
        }

        public static void LogInWithGithub(string token, Action<string> callback)
        {
            UnityWebRequest request = UnityWebRequest.Get("https://api.github.com/user");
            request.SetRequestHeader("Authorization", "token " + token);
            request.SetRequestHeader("Accept", "application/json");
            request.SendWebRequest().completed += w =>
            {
                if (request.responseCode == 200)
                {
                    var userInfo =
                        JsonConvert.DeserializeObject<Dictionary<string, string>>(request.downloadHandler.text);
                    AVUser.LogInWithAuthDataAsync(new Dictionary<string, object>
                        {
                            {"access_token", token},
                            {"expires_in", 7200},
                            {"uid", userInfo["id"]}
                        }, "github")
                        .Post(t => LoginToPost(t, callback));
                }
                else
                {
                    callback(errorMessage);
                }
            };
        }

        //callback（降下的，换上去的，错误信息）
        public static void ReplaceShengHen(
            EInUseShengHen index,
            EShengHen shengHen,
            Action<EShengHen, EShengHen, string> callback
        )
        {
            List<AVObject> toUpdate = new List<AVObject> {index};
            var player = index.player;
            var old = index.shengHen;
            if (old != null)
            {
                old.player = null;
                toUpdate.Add(old);
            }

            if (shengHen != null)
            {
                shengHen.player = player;
                toUpdate.Add(shengHen);
            }

            index.shengHen = shengHen;
            AVObject.SaveAllAsync(toUpdate).Post(t =>
            {
                if (t.IsCanceled || t.IsFaulted)
                {
                    Debug.Log(t.Exception);
                    callback(null, null, errorMessage);
                    return;
                }

                callback(old, shengHen, null);
            });
        }

        //callback（降下的，换上去的，错误信息）
        public static void ReplaceWeapon(
            EInUseWeapon index,
            EWeapon weapon,
            Action<EWeapon, EWeapon, string> callback
        )
        {
            List<AVObject> toUpdate = new List<AVObject> {index};
            var player = index.player;
            var old = index.weapon;
            if (old != null)
            {
                old.player = null;
                toUpdate.Add(old);
            }

            if (weapon != null)
            {
                weapon.player = player;
                toUpdate.Add(weapon);
            }

            index.weapon = weapon;

            AVObject.SaveAllAsync(toUpdate).Post(t =>
            {
                if (t.IsCanceled || t.IsFaulted)
                {
                    Debug.Log(t.Exception);
                    callback(null, null, errorMessage);
                    return;
                }

                callback(old, weapon, null);
            });
        }

        public static void UpdateCurrentPlayer(EPlayer player, Action<string> callback)
        {
            localUserMate.player = player;
            localUserMate.SaveAsync().Post(t =>
            {
                if (t.IsCanceled || t.IsFaulted)
                {
                    Debug.Log(t.Exception);
                    callback(errorMessageInLogIn);
                    return;
                }

                callback(null);
            });
        }

        public static void LogOut(Action<string> callback)
        {
            AVUser.LogOutAsync().Post(t =>
            {
                if (t.IsCanceled || t.IsFaulted)
                {
                    Debug.Log(t.Exception);
                    callback(errorMessageInLogIn);
                    return;
                }

                localUserMate = null;
                callback(null);
            });
        }

        public static EPlayer GetCurrentPlayer()
        {
            return localUserMate.player;
        }

        private delegate void OnMainThreadCallback<T>(Task<T> t);

        private delegate void OnMainThreadCallback(Task t);

        private static void Post(this Task task, OnMainThreadCallback callback)
        {
            if (task.IsCompleted)
            {
                callback(task);
            }
            else
            {
                var w = task.GetAwaiter();
                w.OnCompleted(() => callback(task));
            }
        }

        private static void Post<T>(this Task<T> task, OnMainThreadCallback<T> callback)
        {
            if (task.IsCompleted)
            {
                callback(task);
            }
            else
            {
                var w = task.GetAwaiter();
                w.OnCompleted(() => callback(task));
            }
        }
    }
}