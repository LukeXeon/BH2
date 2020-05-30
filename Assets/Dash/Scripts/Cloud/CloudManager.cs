using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dash.Scripts.Setting;
using Parse;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dash.Scripts.Cloud
{
    public static class CloudManager
    {
        private const string errorMessage = "网络错误";

        private const string errorMessageInLogIn = "网络错误或玩家不存在";

        private const string errorMessageInternal = "游戏内部错误";

        private static GameUserEntity localUserEntity;

        public static event Action<PlayerEntity> playerChanged;

        public static event Action<GameUserEntity> userInfoChanged;

        private static WeaponEntity NewWeapon(int typeId)
        {
            var weapon = new WeaponEntity {user = ParseUser.CurrentUser, typeId = typeId, exp = 0};
            return weapon;
        }

        private static SealEntity NewSeal(int typeId)
        {
            var seal = new SealEntity {user = ParseUser.CurrentUser, typeId = typeId, exp = 0};
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
                if (localUserEntity.crystal < 100)
                {
                    throw new UnityException("水晶不足");
                }

                localUserEntity.crystal -= 100;
                await localUserEntity.SaveAsync();
                userInfoChanged?.Invoke(localUserEntity);
            }

            var type = Random.Range(0, 3);
            switch (type)
            {
                case 0:
                {
                    var playerTypeId = Random.Range(0, GameSettingManager.maxWeaponId + 1);
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
                        p.exp += GameSettingManager.setting.playerLuckDrawExpAddOnce;
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
                    var weaponTypeId = Random.Range(0, GameSettingManager.maxWeaponId + 1);
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
                    var sealTypeId = Random.Range(0, GameSettingManager.maxSealId + 1);
                    await NewSeal(sealTypeId)
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
                e.players.TryGetValue(item.player.ObjectId, out var inUse);
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
                e.players.TryGetValue(item.player.ObjectId, out var inUse);
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

        public static async Task SignUp(string username, string password)
        {
            var myUser = new ParseUser
            {
                Password = password, Username = username
            };
            await myUser.SignUpAsync();
        }

        internal static async Task HandleLogin()
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
                    name = "玩家" + count,
                    crystal = 1000,
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

            PhotonNetwork.AuthValues = new AuthenticationValues
            {
                UserId = ParseUser.CurrentUser.ObjectId
            };
            PhotonNetwork.ConnectUsingSettings();
            while (PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer)
            {
                await Task.Yield();
            }

            PlayerPrefs.SetString("token", ParseUser.CurrentUser.SessionToken);
            PlayerPrefs.Save();
        }

        public static async Task LogInWithToken(string token)
        {
            await ParseUser.BecomeAsync(token);
            await HandleLogin();
        }

        public static async Task LogIn(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("用户名和密码不能为空");
            }

            await ParseUser.LogInAsync(username, password);
            await HandleLogin();
        }


        //callback（降下的，换上去的，错误信息）
        public static async Task<SealEntity[]> ReplaceSeal(
            InUseSealEntity index,
            SealEntity seal
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

            if (seal != null)
            {
                seal.player = player;
                toUpdate.Add(seal);
            }

            index.seal = seal;
            await ParseObject.SaveAllAsync(toUpdate);
            return new[] {old, seal};
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

        public static async Task<CompletePlayer> GetCurrentCompletePlayer()
        {
            var player = localUserEntity.player;
            var task1 = new ParseQuery<InUseWeaponEntity>().WhereEqualTo("player", player)
                .Include("weapon")
                .FindAsync();
            var task2 = new ParseQuery<InUseSealEntity>().WhereEqualTo("player", player)
                .Include("seal")
                .FindAsync();

            var w = (await task1).ToList();
            var s = (await task2).ToList();
            w.Sort((a, b) => a.index.CompareTo(b.index));
            s.Sort((a, b) => a.index.CompareTo(b.index));
            return new CompletePlayer
            {
                player = player,
                weapons = w.Where(i => i.weapon != null).Select(i => i.weapon).ToList(),
                seals = s.Where(i => i.seal != null).Select(i => i.seal).ToList()
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
            PhotonNetwork.Disconnect();
            await ParseUser.LogOutAsync();
            PlayerPrefs.DeleteKey("token");
            PlayerPrefs.Save();
        }

        public static PlayerEntity GetCurrentPlayer()
        {
            return localUserEntity.player;
        }

        public static async Task<int[]> GetOwnerPlayerTypeIds()
        {
            return (await new ParseQuery<PlayerEntity>()
                    .WhereEqualTo("user", ParseUser.CurrentUser)
                    .FindAsync())
                .Select(u => u.typeId)
                .ToArray();
        }

        public static string GetUserName()
        {
            return localUserEntity.name;
        }

        public static async Task BuyPlayer(int typeId, int cost)
        {
            if (!Application.isEditor)
            {
                var c = localUserEntity.crystal;
                if (c - cost > 0)
                {
                    localUserEntity.crystal -= cost;
                    await localUserEntity.SaveAsync();
                    userInfoChanged?.Invoke(localUserEntity);
                }
                else
                {
                    throw new Exception("购买失败，水晶不足");
                }
            }

            var p = await new ParseQuery<PlayerEntity>()
                .WhereEqualTo("user", ParseUser.CurrentUser)
                .WhereEqualTo("typeId", typeId)
                .FirstOrDefaultAsync();
            if (p == null)
            {
                await ParseObject.SaveAllAsync(NewPlayer(typeId));
            }
            else
            {
                throw new Exception("角色已存在，不可重复购买");
            }
        }

        public static async Task BuySeal(int typeId, int cost)
        {
            if (!Application.isEditor)
            {
                var c = localUserEntity.crystal;
                if (c - cost > 0)
                {
                    localUserEntity.crystal -= cost;
                    await localUserEntity.SaveAsync();
                    userInfoChanged?.Invoke(localUserEntity);
                }
                else
                {
                    throw new Exception("购买失败，水晶不足");
                }
            }

            await NewSeal(typeId).SaveAsync();
        }

        public static async Task BuyWeapon(int typeId, int cost)
        {
            if (!Application.isEditor)
            {
                var c = localUserEntity.crystal;
                if (c - cost > 0)
                {
                    localUserEntity.crystal -= cost;
                    await localUserEntity.SaveAsync();
                    userInfoChanged?.Invoke(localUserEntity);
                }
                else
                {
                    throw new Exception("购买失败，水晶不足");
                }
            }

            await NewWeapon(typeId).SaveAsync();
        }

        public static GameUserEntity GetUserInfo()
        {
            return localUserEntity;
        }

        public static async Task SetNameInGame(string name)
        {
            localUserEntity.name = name;
            await localUserEntity.SaveAsync();
            userInfoChanged?.Invoke(localUserEntity);
        }
    }
}