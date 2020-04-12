using System;
using System.Collections;
using Dash.Scripts.Network.Cloud;
using Dash.Scripts.UI;
using LeanCloud;
using Michsky.UI.ModernUIPack;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using Random = UnityEngine.Random;

namespace Dash.Scripts.UIManager
{
    public class BootstrapUIManager : MonoBehaviourPunCallbacks
    {
        public Sprite[] sprites;

        public VideoPlayer videoPlayer;

        public ModalWindowTabs tabs;

        public NotificationManager notifySucceed;

        public NotificationManager notifyError;

        public Canvas canvas;

        public Image background;

        public GameObject progressBarRoot;

        public TextMeshProUGUI progressBarText;

        public Image progressBar;

        public Animator waitWindow;

        public ModalWindowManager windowManager;

        public AudioSource audioSource;

        [Header("login")] public TMP_InputField username;

        public TMP_InputField password;

        public Button login;

        [Header("signUp")] public TMP_InputField usernameInSignUp;

        public TMP_InputField passwordInSignUp;

        public TMP_InputField passwordInSignUp2;

        public Button githubLogin;

        public Button signUp;

        public GameObject yiJingDengLuRoot;

        public Button loadNext;

        public Button backToLogin;

        private AsyncOperation loadSceneAsync;

        private void Awake()
        {
            if (Application.isEditor || PlayerPrefs.GetInt("first startup") == 1)
            {
                ShowWindowOnLoad();
                audioSource.Play();
            }
            else
            {
                PlayerPrefs.SetInt("first startup", 1);
                PlayerPrefs.Save();
                videoPlayer.gameObject.SetActive(true);
                videoPlayer.Prepare();
                videoPlayer.prepareCompleted += p => { p.Play(); };
                videoPlayer.loopPointReached += p =>
                {
                    Destroy(videoPlayer.gameObject);
                    videoPlayer = null;
                    StartCoroutine(ShowWindow1());
                    audioSource.Play();
                };
            }

            background.sprite = sprites[Random.Range(0, sprites.Length - 1)];

            signUp.onClick.AddListener(() =>
            {
                var u = usernameInSignUp.text;
                var p = passwordInSignUp.text;
                var p2 = passwordInSignUp2.text;
                OpenWaitWindow();
                CloudManager.SignUp(u, p, p2, e =>
                {
                    CloseWaitWindow();
                    if (e != null)
                    {
                        notifyError.Show("注册失败", e);
                    }
                    else
                    {
                        notifySucceed.Show("注册成功", "您的账号已成功注册");
                        username.text = u;
                        password.text = p;
                        tabs.PanelAnim(0);
                    }
                });
            });
            login.onClick.AddListener(() =>
            {
                var u = username.text;
                var p = password.text;
                OpenWaitWindow();
                CloudManager.LogIn(u, p, e =>
                {
                    CloseWaitWindow();
                    if (e != null)
                    {
                        notifyError.Show("登录失败", e);
                    }
                    else
                    {
                        notifySucceed.Show("登录成功", "您已成功登录");
                        windowManager.CloseWindow();
                        yiJingDengLuRoot.SetActive(true);
                        ClearPrefs();
                        PlayerPrefs.SetString("password", p);
                        PlayerPrefs.SetString("username", u);
                        PlayerPrefs.Save();
        
                    }
                });
            });
            loadNext.onClick.AddListener(() =>
            {
                yiJingDengLuRoot.SetActive(false);
                StartCoroutine(LoadNext());
            });
            backToLogin.onClick.AddListener(() =>
            {
                OpenWaitWindow();
                CloudManager.LogOut(e =>
                {
                    CloseWaitWindow();
                    if (e != null)
                    {
                        notifyError.Show("退出登录失败", e);
                    }
                    else
                    {
                        ClearPrefs();
                        yiJingDengLuRoot.SetActive(false);
                        PhotonNetwork.Disconnect();
                        StartCoroutine(ShowWindow1());
                    }
                });
            });
            githubLogin.onClick.AddListener(() =>
            {
                var go = Resources.Load<GameObject>(
                    Application.platform == RuntimePlatform.WindowsPlayer ||
                    Application.platform == RuntimePlatform.WindowsEditor
                        ? "Prefab/WebView/Win32Webview"
                        : "Prefab/WebView/MobileWebview"
                );
                go = Instantiate(go, canvas.transform);
                go.transform.SetSiblingIndex(waitWindow.transform.GetSiblingIndex());
                var b = go.GetComponent<IWebUIManager>();
                var waiter = CloudManager.GetGithubUrlAndWaitToken(
                    (e, t) =>
                    {
                        if (e == null)
                        {
                            Destroy(go);
                            OpenWaitWindow();
                            CloudManager.LogInWithGithub(t,
                                e2 =>
                                {
                                    CloseWaitWindow();
                                    if (e2 != null)
                                    {
                                        notifyError.Show("登录失败", e2);
                                    }
                                    else
                                    {
                                        notifySucceed.Show("登录成功", "您已成功登录");
                                        windowManager.CloseWindow();
                                        yiJingDengLuRoot.SetActive(true);
                                        ClearPrefs();
                                        PlayerPrefs.SetString("token", t);
                                        PlayerPrefs.Save();
                                    }
                                });
                        }
                    });
                b.Init(CloudManager.GetLogInUrl(), () =>
                {
                    waiter.Abort();
                    Debug.Log("waiter abort");
                    Destroy(go);
                });
            });
        }


        public static void ClearPrefs()
        {
            PlayerPrefs.DeleteKey("password");
            PlayerPrefs.DeleteKey("username");
            PlayerPrefs.DeleteKey("token");
            PlayerPrefs.Save();
        }


        public void ShowWindowOnLoad()
        {
            var u = PlayerPrefs.GetString("username");
            var p = PlayerPrefs.GetString("password");
            var t = PlayerPrefs.GetString("token");
            windowManager.gameObject.SetActive(false);
            if ((string.IsNullOrEmpty(u) || string.IsNullOrEmpty(p)) && string.IsNullOrEmpty(t))
            {
                StartCoroutine(ShowWindow1());
            }
            else
            {
                if (!string.IsNullOrEmpty(t))
                {
                    CloudManager.LogInWithGithub(t,
                        e =>
                        {
                            if (e != null)
                            {
                                StartCoroutine(ShowWindow1());
                            }
                            else
                            {
                                yiJingDengLuRoot.SetActive(true);
                            }
                        });
                }
                else
                {
                    CloudManager.LogIn(u, p, e =>
                    {
                        if (e != null)
                        {
                            StartCoroutine(ShowWindow1());
                        }
                        else
                        {
                            yiJingDengLuRoot.SetActive(true);
                        }
                    });
                }
            }
        }


        
        private IEnumerator ShowWindow1()
        {
            windowManager.gameObject.SetActive(true);
            yield return new WaitForEndOfFrame();
            windowManager.OpenWindow();
        }

        public void OpenWaitWindow()
        {
            StartCoroutine(OpenWaitWindow0());
        }

        public IEnumerator OpenWaitWindow0()
        {
            waitWindow.gameObject.SetActive(true);
            yield return null;
            waitWindow.Play("Fade-in");
        }

        public void CloseWaitWindow()
        {
            waitWindow.gameObject.SetActive(false);
        }


        public IEnumerator LoadNext()
        {
            PhotonNetwork.AuthValues = new AuthenticationValues
            {
                UserId = AVUser.CurrentUser.ObjectId
            };
            PhotonNetwork.ConnectUsingSettings();
            progressBarRoot.SetActive(true);
            loadSceneAsync = SceneManager.LoadSceneAsync("Desktop");
            Debug.Log("Load Desktop");
            loadSceneAsync.allowSceneActivation = false;
            progressBar.fillAmount = 0;
            while (loadSceneAsync.progress < 0.9f)
            {
                progressBar.fillAmount = loadSceneAsync.progress;
                progressBarText.text = Math.Round(loadSceneAsync.progress, 2) * 100 + "%";
                yield return null;
            }

            progressBar.fillAmount = 1;
            progressBarText.text = 100 + "%";
            yield return new WaitForEndOfFrame();
            progressBarText.text = "正在连接游戏主服务器";
            yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer);
            loadSceneAsync.allowSceneActivation = true;
        }

        public IEnumerator WaitFinish(float s, Action action)
        {
            yield return new WaitForSeconds(s);
            action();
        }
    }
}