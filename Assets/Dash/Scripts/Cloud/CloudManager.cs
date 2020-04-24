using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Dash.Scripts.Config;
using Dash.Scripts.Core;
using Flurl;
using LeanCloud;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace Dash.Scripts.Cloud
{
    public static class CloudManager
    {
        private const string errorMessage = "网络错误";

        private const string errorMessageInLogIn = "网络错误或玩家不存在";

        private const string errorMessageInternal = "游戏内部错误";

        private static EUserMate localUserMate;

        public static string GetLogInUrl()
        {
            var info = GameBootManager.info;
            var login = new Url("https://github.com/login/oauth/authorize");
            login.SetQueryParam("client_id", info.githubClientId);
            login.SetQueryParam("redirect_uri", GetCallbackUrl());
            return login.ToString();
        }

        public static event Action<EPlayer> playerChanged;

        public static event Action<EUserMate> userInfoChanged;

        private static string GetCallbackUrl()
        {
            return @"http://localhost:10086/oauth/redirect/";
        }

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
            var list = new List<AVObject>();
            var player = new EPlayer();
            player.user = AVUser.CurrentUser;
            player.typeId = typeId;
            player.exp = 0;
            var inUseWeapon = new List<EInUseWeapon>();
            var inUseShengHen = new List<EInUseShengHen>();
            for (var i = 0; i < 3; i++)
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

        public static async Task<LuckDrawResult> LuckDraw()
        {
            var type = Random.Range(0, 3);
            switch (type)
            {
                case 0:
                {
                    var playerTypeId = Random.Range(0, GameConfigManager.maxWeaponId + 1);
                    var player = (await new AVQuery<EPlayer>()
                        .WhereEqualTo("user", AVUser.CurrentUser)
                        .WhereEqualTo("typeId", playerTypeId)
                        .FindAsync()).ToArray();
                    if (player.Length == 0)
                    {
                        var p = NewPlayer(playerTypeId);
                        await AVObject.SaveAllAsync(p);
                        return new LuckDrawResult
                        {
                            typeId = playerTypeId,
                            resultType = LuckDrawResult.Type.UnLockPlayer
                        };
                    }
                    else
                    {
                        var p = player.First();
                        p.exp += GameConfigManager.levelInfo.playerLuckDrawExpAddOnce;
                        await AVObject.SaveAllAsync(new AVObject[] {p});
                        return new LuckDrawResult
                        {
                            typeId = playerTypeId,
                            resultType = LuckDrawResult.Type.AddPlayerExp
                        };
                    }
                }

                case 1:
                {
                    var weaponTypeId = Random.Range(0, GameConfigManager.maxWeaponId + 1);
                    await NewWeapon(weaponTypeId)
                        .SaveAsync();
                    return new LuckDrawResult
                    {
                        typeId = weaponTypeId,
                        resultType = LuckDrawResult.Type.Weapon
                    };
                }

                default:
                {
                    var shengHenTypeId = Random.Range(0, GameConfigManager.maxShengHeId + 1);
                    await NewShengHen(null, shengHenTypeId)
                        .SaveAsync();
                    return new LuckDrawResult
                    {
                        typeId = shengHenTypeId,
                        resultType = LuckDrawResult.Type.ShengHen
                    };
                }
            }
        }

        public static async Task<Dictionary<string, EShengHen>> GetUserShengHen()
        {
            return (await new AVQuery<EShengHen>()
                .Include("player")
                .WhereEqualTo("user", AVUser.CurrentUser)
                .FindAsync()).ToDictionary(o => o.ObjectId, o => o);
        }

        public static async Task<Dictionary<string, EWeapon>> GetUserWeapons()
        {
            return (await new AVQuery<EWeapon>()
                .Include("player")
                .WhereEqualTo("user", AVUser.CurrentUser)
                .FindAsync()).ToDictionary(o => o.ObjectId, o => o);
        }

        public static async Task<Equipments> GetEquipments()
        {
            var e = new Equipments();
            var t0 = new AVQuery<EWeapon>()
                .Include("player")
                .WhereEqualTo("user", AVUser.CurrentUser)
                .FindAsync();
            foreach (var item in await t0) e.weapons.Add(item.ObjectId, item);

            var t1 = new AVQuery<EShengHen>()
                .Include("player")
                .WhereEqualTo("user", AVUser.CurrentUser)
                .FindAsync();
            foreach (var item in await t1) e.shengHens.Add(item.ObjectId, item);

            var t2 = new AVQuery<EInUseWeapon>()
                .Include("player")
                .Include("weapon")
                .WhereEqualTo("user", AVUser.CurrentUser)
                .FindAsync();
            foreach (var item in await t2)
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

            var t3 = new AVQuery<EInUseShengHen>()
                .Include("player")
                .Include("shengHen")
                .WhereEqualTo("user", AVUser.CurrentUser)
                .FindAsync();
            foreach (var item in await t3)
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

            foreach (var inUse in e.players.Values)
            {
                inUse.shengHens.Sort((o1, o2) => o1.index.CompareTo(o2.index));
                inUse.weapons.Sort((o1, o2) => o1.index.CompareTo(o2.index));
            }

            return e;
        }

        public static async Task SignUp(string username, string password, string password2)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(password2))
                throw new ArgumentException("用户名和密码不能为空");

            if (password != password2) throw new ArgumentException("两次输入的密码不一致");

            var myUser = new AVUser
            {
                Password = password, Username = username
            };
            await myUser.SignUpAsync();
        }

        private static async Task HandleLogin()
        {
            var t1 = new AVQuery<EUserMate>()
                .Include("player")
                .WhereEqualTo("user", AVUser.CurrentUser)
                .FindAsync();
            var t2 = new AVQuery<EPlayer>()
                .WhereEqualTo("user", AVUser.CurrentUser)
                .CountAsync();
            var userMate = (await t1).FirstOrDefault();
            if (userMate == null)
            {
                var count = await new AVQuery<AVUser>()
                    .CountAsync();
                userMate = new EUserMate
                {
                    user = AVUser.CurrentUser,
                    nameInGame = "玩家" + count
                };
            }
            localUserMate = userMate;
            var playerCount = await t2;
            if (playerCount == 0)
            {
                var p = NewPlayer(0);
                userMate.player = (EPlayer) p[0];
                p.Add(userMate);
                await AVObject.SaveAllAsync(p);
            }
        }

        public static async Task LogInWithToken(string token)
        {
            await AVUser.BecomeAsync(token);
            await HandleLogin();
        }

        public static async Task LogIn(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new ArgumentException("用户名和密码不能为空");

            await AVUser.LogInAsync(username, password);
            await HandleLogin();
        }

        private static UnityWebRequestAwaitable GetAwaiter(this UnityWebRequestAsyncOperation operation)
        {
            return new UnityWebRequestAwaitable(operation);
        }

        public static async Task LogInWithGithub(CancellationToken cancellationToken)
        {
            var info = GameBootManager.info;
            var http = new HttpListener {AuthenticationSchemes = AuthenticationSchemes.Anonymous};
            http.Prefixes.Add(GetCallbackUrl());
            http.Start();
            var ctxTask = http.GetContextAsync();
            var cancelTask = Task.Run(async () =>
                {
                    while (!cancellationToken.IsCancellationRequested && !ctxTask.IsCompleted) await Task.Yield();
                }
                , cancellationToken);
            await Task.WhenAny(cancelTask, ctxTask);
            if (cancellationToken.IsCancellationRequested)
            {
                http.Abort();
                throw new TimeoutException("");
            }

            var code = (await ctxTask).Request.QueryString["code"];
            http.Abort();
            Debug.Log(code);
            var u = new Url("https://github.com/login/oauth/access_token");
            u.SetQueryParam("client_id", info.githubClientId);
            u.SetQueryParam("client_secret", info.githubClientSecret);
            u.SetQueryParam("code", code);
            var wr = UnityWebRequest.Post(u, new Dictionary<string, string>());
            wr.SetRequestHeader("Accept", "application/json");
            await wr.SendWebRequest();
            var token =
                JsonConvert.DeserializeObject<Dictionary<string, string>>(wr.downloadHandler.text)["access_token"];
            var request = UnityWebRequest.Get("https://api.github.com/user");
            request.SetRequestHeader("Authorization", "token " + token);
            request.SetRequestHeader("Accept", "application/json");
            await request.SendWebRequest();
            if (request.responseCode == 200)
            {
                var userInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(request.downloadHandler.text);
                await AVUser.LogInWithAuthDataAsync(new Dictionary<string, object>
                {
                    {"access_token", token},
                    {"expires_in", 7200},
                    {"uid", userInfo["id"]}
                }, "github");
                await HandleLogin();
            }
            else
            {
                throw new IOException("http错误码" + request.responseCode);
            }
        }

        //callback（降下的，换上去的，错误信息）
        public static async Task<EShengHen[]> ReplaceShengHen(
            EInUseShengHen index,
            EShengHen shengHen
        )
        {
            var toUpdate = new List<AVObject> {index};
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
            await AVObject.SaveAllAsync(toUpdate);
            return new[] {old, shengHen};
        }

        //callback（降下的，换上去的，错误信息）
        public static async Task<EWeapon[]> ReplaceWeapon(
            EInUseWeapon index,
            EWeapon weapon
        )
        {
            var toUpdate = new List<AVObject> {index};
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

            await AVObject.SaveAllAsync(toUpdate);
            return new[] {old, weapon};
        }

        public static async Task<CompletePlayer> GetCompletePlayer()
        {
            var player = localUserMate.player;
            var task1 = new AVQuery<EInUseWeapon>().WhereEqualTo("player", player)
                .Include("weapon")
                .FindAsync();
            var task2 = new AVQuery<EInUseShengHen>().WhereEqualTo("player", player)
                .Include("shengHen")
                .FindAsync();

            var w = (await task1).ToList();
            var s = (await task2).ToList();
            w.Sort((a, b) => a.index.CompareTo(b.index));
            s.Sort((a, b) => a.index.CompareTo(b.index));
            return new CompletePlayer
            {
                player = player,
                weapons = w.Where(i => i.weapon != null).Select(i => i.weapon).ToList(),
                shengHens = s.Where(i => i.shengHen != null).Select(i => i.shengHen).ToList()
            };
        }

        public static async Task UpdateCurrentPlayer(EPlayer player)
        {
            localUserMate.player = player;
            await localUserMate.SaveAsync();
            playerChanged?.Invoke(player);
        }


        public static async Task LogOut()
        {
            await AVUser.LogOutAsync();
        }

        public static EPlayer GetCurrentPlayer()
        {
            return localUserMate.player;
        }

        public static string GetNameInGame()
        {
            return localUserMate.nameInGame;
        }

        public static EUserMate GetUserInfo()
        {
            return localUserMate;
        }

        public static async Task SetNameInGame(string name)
        {
            localUserMate.nameInGame = name;
            await localUserMate.SaveAsync();
            userInfoChanged?.Invoke(localUserMate);
        }

        private struct UnityWebRequestAwaitable : INotifyCompletion
        {
            private readonly UnityWebRequestAsyncOperation operation;

            public UnityWebRequestAwaitable(UnityWebRequestAsyncOperation operation)
            {
                this.operation = operation;
            }

            public void OnCompleted(Action continuation)
            {
                operation.completed += o => continuation();
            }

            public bool IsCompleted => operation.isDone;

            public UnityWebRequest GetResult()
            {
                return operation.webRequest;
            }
        }
    }
}