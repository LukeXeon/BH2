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
using Newtonsoft.Json;
using Parse;
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

        private static GameUserEntity localUserEntity;

        public static string GetLogInUrl()
        {
            var info = GameBootManager.info;
            var login = new Url("https://github.com/login/oauth/authorize");
            login.SetQueryParam("client_id", info.githubClientId);
            login.SetQueryParam("redirect_uri", GetCallbackUrl());
            return login.ToString();
        }

        public static event Action<PlayerEntity> playerChanged;

        public static event Action<GameUserEntity> userInfoChanged;

        private static string GetCallbackUrl()
        {
            return @"http://localhost:10086/oauth/redirect/";
        }

        private static WeaponEntity NewWeapon(int typeId)
        {
            var weapon = new WeaponEntity {user = ParseUser.CurrentUser, typeId = typeId, exp = 0};
            return weapon;
        }

        private static SealEntity NewSeal(this PlayerEntity player, int typeId)
        {
            var seal = new SealEntity {player = player, user = ParseUser.CurrentUser, typeId = typeId, exp = 0};
            return seal;
        }

        private static List<ParseObject> NewPlayer(int typeId)
        {
            var list = new List<ParseObject>();
            var player = new PlayerEntity {user = ParseUser.CurrentUser, typeId = typeId, exp = 0};
            var inUseWeapon = new List<InUseWeaponEntity>();
            var inUseSeal = new List<InUseSealEntity>();
            for (var i = 0; i < 3; i++)
            {
                inUseWeapon.Add(new InUseWeaponEntity
                {
                    user = ParseUser.CurrentUser,
                    player = player,
                    index = i,
                    weapon = null
                });
                inUseSeal.Add(new InUseSealEntity
                {
                    user = ParseUser.CurrentUser,
                    player = player,
                    index = i,
                    seal = null
                });
            }

            var w = NewWeapon(0);
            w.player = player;
            inUseWeapon[0].weapon = w;
            list.Add(player);
            list.Add(w);
            list.AddRange(inUseWeapon);
            list.AddRange(inUseSeal);
            return list;
        }

        public static async Task<LuckDrawResult> LuckDraw()
        {
            if (!Application.isEditor)
            {
                if (localUserEntity.shuiJing < 100)
                {
                    throw new UnityException("水晶不足");
                }

                localUserEntity.shuiJing -= 100;
                await localUserEntity.SaveAsync();
            }

            var type = Random.Range(0, 3);
            switch (type)
            {
                case 0:
                {
                    var playerTypeId = Random.Range(0, GameConfigManager.maxWeaponId + 1);
                    var player = (await new ParseQuery<PlayerEntity>()
                        .WhereEqualTo("user", ParseUser.CurrentUser)
                        .WhereEqualTo("typeId", playerTypeId)
                        .FindAsync()).ToArray();
                    if (player.Length == 0)
                    {
                        var p = NewPlayer(playerTypeId);
                        await ParseObject.SaveAllAsync(p);
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
                        await ParseObject.SaveAllAsync(new ParseObject[] {p});
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
                    var sealTypeId = Random.Range(0, GameConfigManager.maxShengHeId + 1);
                    await NewSeal(null, sealTypeId)
                        .SaveAsync();
                    return new LuckDrawResult
                    {
                        typeId = sealTypeId,
                        resultType = LuckDrawResult.Type.Seal
                    };
                }
            }
        }

        public static async Task<Dictionary<string, SealEntity>> GetUserSeals()
        {
            return (await new ParseQuery<SealEntity>()
                .Include("player")
                .WhereEqualTo("user", ParseUser.CurrentUser)
                .FindAsync()).ToDictionary(o => o.ObjectId, o => o);
        }

        public static async Task<Dictionary<string, WeaponEntity>> GetUserWeapons()
        {
            return (await new ParseQuery<WeaponEntity>()
                .Include("player")
                .WhereEqualTo("user", ParseUser.CurrentUser)
                .FindAsync()).ToDictionary(o => o.ObjectId, o => o);
        }

        public static async Task<Equipments> GetEquipments()
        {
            var e = new Equipments();
            var t0 = new ParseQuery<WeaponEntity>()
                .Include("player")
                .WhereEqualTo("user", ParseUser.CurrentUser)
                .FindAsync();
            foreach (var item in await t0) e.weapons.Add(item.ObjectId, item);

            var t1 = new ParseQuery<SealEntity>()
                .Include("player")
                .WhereEqualTo("user", ParseUser.CurrentUser)
                .FindAsync();
            foreach (var item in await t1) e.seals.Add(item.ObjectId, item);

            var t2 = new ParseQuery<InUseWeaponEntity>()
                .Include("player")
                .Include("weapon")
                .WhereEqualTo("user", ParseUser.CurrentUser)
                .FindAsync();
            foreach (var item in await t2)
            {
                Equipments.Player inUse;
                e.players.TryGetValue(item.player.ObjectId, out inUse);
                if (inUse == null)
                {
                    inUse = new Equipments.Player
                    {
                        player = item.player
                    };
                    e.players.Add(item.player.ObjectId, inUse);
                }

                inUse.weapons.Add(item);
            }

            var t3 = new ParseQuery<InUseSealEntity>()
                .Include("player")
                .Include("seal")
                .WhereEqualTo("user", ParseUser.CurrentUser)
                .FindAsync();
            foreach (var item in await t3)
            {
                Equipments.Player inUse;
                e.players.TryGetValue(item.player.ObjectId, out inUse);
                if (inUse == null)
                {
                    inUse = new Equipments.Player
                    {
                        player = item.player
                    };
                    e.players.Add(item.player.ObjectId, inUse);
                }

                inUse.seals.Add(item);
            }

            foreach (var inUse in e.players.Values)
            {
                inUse.seals.Sort((o1, o2) => o1.index.CompareTo(o2.index));
                inUse.weapons.Sort((o1, o2) => o1.index.CompareTo(o2.index));
            }

            return e;
        }

        public static async Task SignUp(string username, string password, string password2)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(password2))
                throw new ArgumentException("用户名和密码不能为空");

            if (password != password2)
            {
                throw new ArgumentException("两次输入的密码不一致");
            }
            var myUser = new ParseUser
            {
                Password = password, Username = username
            };
            await myUser.SignUpAsync();
        }

        private static async Task HandleLogin()
        {
            var t1 = new ParseQuery<GameUserEntity>()
                .Include("player")
                .WhereEqualTo("user", ParseUser.CurrentUser)
                .FindAsync();
            var t2 = new ParseQuery<PlayerEntity>()
                .WhereEqualTo("user", ParseUser.CurrentUser)
                .CountAsync();
            var userMate = (await t1).FirstOrDefault();
            if (userMate == null)
            {
                var count = await new ParseQuery<ParseUser>()
                    .CountAsync();
                userMate = new GameUserEntity
                {
                    user = ParseUser.CurrentUser,
                    nameInGame = "玩家" + count,
                    shuiJing = 1000,
                };
            }

            localUserEntity = userMate;
            var playerCount = await t2;
            if (playerCount == 0)
            {
                var p = NewPlayer(0);
                userMate.player = (PlayerEntity) p[0];
                p.Add(userMate);
                await ParseObject.SaveAllAsync(p);
            }
        }

        public static async Task LogInWithToken(string token)
        {
            await ParseUser.BecomeAsync(token);
            await HandleLogin();
        }

        public static async Task LogIn(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new ArgumentException("用户名和密码不能为空");

            await ParseUser.LogInAsync(username, password);
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
                throw new UnityException();
                //                await ParseUser.LogInWithAuthDataAsync(new Dictionary<string, object>
//                {
//                    {"access_token", token},
//                    {"expires_in", 7200},
//                    {"uid", userInfo["id"]}
//                }, "github");
                await HandleLogin();
            }
            else
            {
                throw new IOException("http错误码" + request.responseCode);
            }
        }

        //callback（降下的，换上去的，错误信息）
        public static async Task<SealEntity[]> ReplaceShengHen(
            InUseSealEntity index,
            SealEntity shengHen
        )
        {
            var toUpdate = new List<ParseObject> {index};
            var player = index.player;
            var old = index.seal;
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

            index.seal = shengHen;
            await ParseObject.SaveAllAsync(toUpdate);
            return new[] {old, shengHen};
        }

        //callback（降下的，换上去的，错误信息）
        public static async Task<WeaponEntity[]> ReplaceWeapon(
            InUseWeaponEntity index,
            WeaponEntity weapon
        )
        {
            var toUpdate = new List<ParseObject> {index};
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

            await ParseObject.SaveAllAsync(toUpdate);
            return new[] {old, weapon};
        }

        public static async Task<CompletePlayer> GetCompletePlayer()
        {
            var player = localUserEntity.player;
            var task1 = new ParseQuery<InUseWeaponEntity>().WhereEqualTo("player", player)
                .Include("weapon")
                .FindAsync();
            var task2 = new ParseQuery<InUseSealEntity>().WhereEqualTo("player", player)
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
                shengHens = s.Where(i => i.seal != null).Select(i => i.seal).ToList()
            };
        }

        public static async Task UpdateCurrentPlayer(PlayerEntity player)
        {
            localUserEntity.player = player;
            await localUserEntity.SaveAsync();
            playerChanged?.Invoke(player);
        }


        public static async Task LogOut()
        {
            await ParseUser.LogOutAsync();
        }

        public static PlayerEntity GetCurrentPlayer()
        {
            return localUserEntity.player;
        }

        public static string GetNameInGame()
        {
            return localUserEntity.nameInGame;
        }

        public static GameUserEntity GetUserInfo()
        {
            return localUserEntity;
        }

        public static async Task SetNameInGame(string name)
        {
            localUserEntity.nameInGame = name;
            await localUserEntity.SaveAsync();
            userInfoChanged?.Invoke(localUserEntity);
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