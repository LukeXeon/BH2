using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Dash.Scripts.Cloud;
using Dash.Scripts.UI;
using Michsky.UI.ModernUIPack;
using Photon.Pun;
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
        public AudioSource audioSource;

        public Image background;

        public Button backToLogin;

        public Canvas canvas;

        public Button githubLogin;

        public Button loadNext;

        private AsyncOperation loadSceneAsync;

        public Button login;

        public NotificationManager notifyError;

        public NotificationManager notifySucceed;

        public TMP_InputField password;

        public TMP_InputField passwordInSignUp;

        public TMP_InputField passwordInSignUp2;

        public Image progressBar;

        public GameObject progressBarRoot;

        public TextMeshProUGUI progressBarText;

        public Button signUp;
        public Sprite[] sprites;

        public ModalWindowTabs tabs;

        [Header("login")] public TMP_InputField username;

        [Header("signUp")] public TMP_InputField usernameInSignUp;

        public VideoPlayer videoPlayer;

        public Animator waitWindow;

        public ModalWindowManager windowManager;

        public GameObject yiJingDengLuRoot;

        public GameObject win32WebView;
        public GameObject mobileWebView;


        private void Awake()
        {
            videoPlayer.gameObject.SetActive(true);
            DesktopUIManager.bootBackground = sprites[Random.Range(0, sprites.Length - 1)];
            background.sprite = DesktopUIManager.bootBackground;
            signUp.onClick.AddListener(async () =>
            {
                var u = usernameInSignUp.text;
                var p = passwordInSignUp.text;
                var p2 = passwordInSignUp2.text;
                OpenWaitWindow();
                try
                {
                    if (string.IsNullOrEmpty(u) || string.IsNullOrEmpty(p) || string.IsNullOrEmpty(p2))
                    {
                        throw new ArgumentException("用户名和密码不能为空");
                    }

                    if (p != p2)
                    {
                        throw new ArgumentException("两次输入的密码不一致");
                    }

                    await CloudManager.SignUp(u, p);
                    notifySucceed.Show("注册成功", "您的账号已成功注册");
                    username.text = u;
                    password.text = p;
                    tabs.PanelAnim(0);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    notifyError.Show("注册失败", e.Message);
                }
                finally
                {
                    CloseWaitWindow();
                }
            });
            login.onClick.AddListener(async () =>
            {
                var u = username.text;
                var p = password.text;
                OpenWaitWindow();
                try
                {
                    await CloudManager.LogIn(u, p);
                    notifySucceed.Show("登录成功", "您已成功登录");
                    windowManager.CloseWindow();
                    yiJingDengLuRoot.SetActive(true);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    notifyError.Show("登录失败", e.Message);
                }
                finally
                {
                    CloseWaitWindow();
                }
            });
            loadNext.onClick.AddListener(() =>
            {
                yiJingDengLuRoot.SetActive(false);
                StartCoroutine(LoadNext());
            });
            backToLogin.onClick.AddListener(async () =>
            {
                OpenWaitWindow();
                try
                {
                    await CloudManager.LogOut();
                    yiJingDengLuRoot.SetActive(false);
                    StartCoroutine(ShowWindow1());
                }
                catch (Exception e)
                {
                    notifyError.Show("退出登录失败", e.Message);
                }
                finally
                {
                    CloseWaitWindow();
                }
            });
            githubLogin.onClick.AddListener(async () =>
            {
                var go = Application.platform == RuntimePlatform.WindowsPlayer ||
                         Application.platform == RuntimePlatform.WindowsEditor
                    ? win32WebView
                    : mobileWebView;
                go = Instantiate(go, canvas.transform);
                go.transform.SetSiblingIndex(waitWindow.transform.GetSiblingIndex());
                var b = go.GetComponent<IWebUIManager>();
                var cancelSource = new CancellationTokenSource();
                b.Initialize(GithubClient.LogInUrl, () =>
                {
                    cancelSource.Cancel();
                    Debug.Log("waiter abort");
                    Destroy(go);
                });
                try
                {
                    await GithubClient.LogInAsync(cancelSource.Token);
                    notifySucceed.Show("登录成功", "您已成功登录");
                    windowManager.CloseWindow();
                    yiJingDengLuRoot.SetActive(true);
                }
                catch (TaskCanceledException e)
                {
                    Debug.Log(e);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    notifyError.Show("登录失败", e.Message);
                }
                finally
                {
                    Destroy(go);
                }
            });
        }

        public void OnBooted()
        {
            StartCoroutine(DoStart());
        }

        private IEnumerator DoStart()
        {
            if (Application.isEditor || PlayerPrefs.GetInt("first startup") == 1)
            {
                ShowWindowOnLoad();
                audioSource.Play();
                Destroy(videoPlayer.gameObject);
                videoPlayer = null;
            }
            else
            {
                PlayerPrefs.SetInt("first startup", 1);
                PlayerPrefs.Save();
                videoPlayer.gameObject.SetActive(true);
                var op = Resources.LoadAsync<VideoClip>("Video/BootVideo");
                yield return op;
                videoPlayer.clip = (VideoClip) op.asset;
                videoPlayer.Prepare();
                videoPlayer.prepareCompleted += p => p.Play();
                videoPlayer.loopPointReached += p =>
                {
                    Destroy(videoPlayer.gameObject);
                    videoPlayer = null;
                    StartCoroutine(ShowWindow1());
                    audioSource.Play();
                };
            }
        }

        public async void ShowWindowOnLoad()
        {
            var t = PlayerPrefs.GetString("token");
            windowManager.gameObject.SetActive(false);
            if (string.IsNullOrEmpty(t))
                StartCoroutine(ShowWindow1());
            else
                try
                {
                    await CloudManager.LogInWithToken(t);
                    yiJingDengLuRoot.SetActive(true);
                    notifySucceed.Show("团长，欢迎回来", CloudManager.GetUserInfo().name + " 已经登录");
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    StartCoroutine(ShowWindow1());
                }
        }

        private IEnumerator ShowWindow1()
        {
            windowManager.gameObject.SetActive(true);
            yield return new WaitForEndOfFrame();
            windowManager.OpenWindow();
            tabs.Start();
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
            loadSceneAsync.allowSceneActivation = true;
        }

        public IEnumerator WaitFinish(float s, Action action)
        {
            yield return new WaitForSeconds(s);
            action();
        }
    }
}