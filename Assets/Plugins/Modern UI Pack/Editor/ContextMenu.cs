using UnityEngine;
using UnityEditor;

namespace Michsky.UI.ModernUIPack
{
    public class ContextMenu : MonoBehaviour
    {
        static void CreateObject(string resourcePath)
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>(resourcePath), Vector3.zero, Quaternion.identity) as GameObject;

            try
            {
                if (Selection.activeGameObject == null)
                {
                    var canvas = (Canvas)GameObject.FindObjectsOfType(typeof(Canvas))[0];
                    clone.transform.SetParent(canvas.transform, false);
                }

                else
                {
                    clone.transform.SetParent(Selection.activeGameObject.transform, false);
                }
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }

            catch
            {
                CreateCanvas();
                var canvas = (Canvas)GameObject.FindObjectsOfType(typeof(Canvas))[0];
                clone.transform.SetParent(canvas.transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }
        }

        [MenuItem("Tools/Modern UI Pack/Show UI Manager %#M")]
        static void ShowManager()
        {
            Selection.activeObject = Resources.Load("MUIP Manager");

            if (Selection.activeObject == null)
                Debug.Log("Can't find a file named 'MUIP Manager'. Make sure you have 'MUIP Manager' file in Resources folder.");
        }

        [MenuItem("GameObject/Modern UI Pack/Canvas", false, -1)]
        static void CreateCanvas()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Other/Canvas"), Vector3.zero, Quaternion.identity) as GameObject;
            clone.name = clone.name.Replace("(Clone)", "").Trim();
        }

        [MenuItem("GameObject/Modern UI Pack/Animated Icon/Hamburger to Exit", false, 0)]
        static void AIHTE()
        {
            CreateObject("Animated Icon/Hamburger to Exit");
        }

        [MenuItem("GameObject/Modern UI Pack/Animated Icon/Heart Pop", false, 0)]
        static void AIHP()
        {
            CreateObject("Animated Icon/Heart Pop");
        }

        [MenuItem("GameObject/Modern UI Pack/Animated Icon/Load", false, 0)]
        static void AILOA()
        {
            CreateObject("Animated Icon/Load");
        }

        [MenuItem("GameObject/Modern UI Pack/Animated Icon/Lock", false, 0)]
        static void AIL()
        {
            CreateObject("Animated Icon/Lock");
        }

        [MenuItem("GameObject/Modern UI Pack/Animated Icon/Message Bubbles", false, 0)]
        static void AILMB()
        {
            CreateObject("Animated Icon/Message Bubbles");
        }

        [MenuItem("GameObject/Modern UI Pack/Animated Icon/No to Yes", false, 0)]
        static void AINTY()
        {
            CreateObject("Animated Icon/No to Yes");
        }

        [MenuItem("GameObject/Modern UI Pack/Animated Icon/Notification Bell", false, 0)]
        static void AINTFB()
        {
            CreateObject("Animated Icon/Notification Bell");
        }

        [MenuItem("GameObject/Modern UI Pack/Animated Icon/Sand Clock", false, 0)]
        static void AISC()
        {
            CreateObject("Animated Icon/Sand Clock");
        }

        [MenuItem("GameObject/Modern UI Pack/Animated Icon/Slider", false, 0)]
        static void AISL()
        {
            CreateObject("Animated Icon/Slider");
        }

        [MenuItem("GameObject/Modern UI Pack/Animated Icon/Window", false, 0)]
        static void AIWI()
        {
            CreateObject("Animated Icon/Window");
        }

        [MenuItem("GameObject/Modern UI Pack/Animated Icon/Yes to No", false, 0)]
        static void AIYTN()
        {
            CreateObject("Animated Icon/Yes to No");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic/Standard", false, 0)]
        static void BBST()
        {
            CreateObject("Button/Basic/Standard");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic/White", false, 0)]
        static void BBWHI()
        {
            CreateObject("Button/Basic/White");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic/Gray", false, 0)]
        static void BBGR()
        {
            CreateObject("Button/Basic/Gray");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic/Blue", false, 0)]
        static void BBBL()
        {
            CreateObject("Button/Basic/Blue");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic/Brown", false, 0)]
        static void BBBRW()
        {
            CreateObject("Button/Basic/Brown");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic/Green", false, 0)]
        static void BBGRE()
        {
            CreateObject("Button/Basic/Green");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic/Night", false, 0)]
        static void BBNI()
        {
            CreateObject("Button/Basic/Night");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic/Orange", false, 0)]
        static void BBOR()
        {
            CreateObject("Button/Basic/Orange");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic/Pink", false, 0)]
        static void BBPIN()
        {
            CreateObject("Button/Basic/Pink");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic/Purple", false, 0)]
        static void BBPURP()
        {
            CreateObject("Button/Basic/Purple");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic/Red", false, 0)]
        static void BBRED()
        {
            CreateObject("Button/Basic/Red");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Gradient/White", false, 0)]
        static void BGWHI()
        {
            CreateObject("Button/Basic - Gradient/White");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Gradient/Gray", false, 0)]
        static void BGGR()
        {
            CreateObject("Button/Basic - Gradient/Gray");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Gradient/Blue", false, 0)]
        static void BGBL()
        {
            CreateObject("Button/Basic - Gradient/Blue");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Gradient/Brown", false, 0)]
        static void BGBRW()
        {
            CreateObject("Button/Basic - Gradient/Brown");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Gradient/Green", false, 0)]
        static void BGGRE()
        {
            CreateObject("Button/Basic - Gradient/Green");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Gradient/Night", false, 0)]
        static void BGNI()
        {
            CreateObject("Button/Basic - Gradient/Night");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Gradient/Orange", false, 0)]
        static void BGOR()
        {
            CreateObject("Button/Basic - Gradient/Orange");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Gradient/Pink", false, 0)]
        static void BGPIN()
        {
            CreateObject("Button/Basic - Gradient/Pink");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Gradient/Purple", false, 0)]
        static void BGPURP()
        {
            CreateObject("Button/Basic - Gradient/Purple");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Gradient/Red", false, 0)]
        static void BGRED()
        {
            CreateObject("Button/Basic - Gradient/Red");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Only Icon/Standard", false, 0)]
        static void BBOICS()
        {
            CreateObject("Button/Basic - Only Icon/Standard");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Only Icon/White", false, 0)]
        static void BBOICW()
        {
            CreateObject("Button/Basic - Only Icon/White");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Only Icon/Gray", false, 0)]
        static void BBOICG()
        {
            CreateObject("Button/Basic - Only Icon/Gray");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Only Icon/Blue", false, 0)]
        static void BBOICB()
        {
            CreateObject("Button/Basic - Only Icon/Blue");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Only Icon/Brown", false, 0)]
        static void BBOICBR()
        {
            CreateObject("Button/Basic - Only Icon/Brown");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Only Icon/Green", false, 0)]
        static void BBOICGR()
        {
            CreateObject("Button/Basic - Only Icon/Green");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Only Icon/Night", false, 0)]
        static void BBOICN()
        {
            CreateObject("Button/Basic - Only Icon/Night");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Only Icon/Orange", false, 0)]
        static void BBOICO()
        {
            CreateObject("Button/Basic - Only Icon/Orange");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Only Icon/Pink", false, 0)]
        static void BBOICP()
        {
            CreateObject("Button/Basic - Only Icon/Pink");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Only Icon/Purple", false, 0)]
        static void BBOICPU()
        {
            CreateObject("Button/Basic - Only Icon/Purple");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Only Icon/Red", false, 0)]
        static void BBOICR()
        {
            CreateObject("Button/Basic - Only Icon/Red");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - With Icon/Standard", false, 0)]
        static void BBWICS()
        {
            CreateObject("Button/Basic - With Icon/Standard");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - With Icon/White", false, 0)]
        static void BBWICW()
        {
            CreateObject("Button/Basic - With Icon/White");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - With Icon/Gray", false, 0)]
        static void BBWICG()
        {
            CreateObject("Button/Basic - With Icon/Gray");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - With Icon/Blue", false, 0)]
        static void BBWICB()
        {
            CreateObject("Button/Basic - With Icon/Blue");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - With Icon/Brown", false, 0)]
        static void BBWICBR()
        {
            CreateObject("Button/Basic - With Icon/Brown");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - With Icon/Green", false, 0)]
        static void BBWICGR()
        {
            CreateObject("Button/Basic - With Icon/Green");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - With Icon/Night", false, 0)]
        static void BBWICN()
        {
            CreateObject("Button/Basic - With Icon/Night");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - With Icon/Orange", false, 0)]
        static void BBWICO()
        {
            CreateObject("Button/Basic - With Icon/Orange");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - With Icon/Pink", false, 0)]
        static void BBWICP()
        {
            CreateObject("Button/Basic - With Icon/Pink");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - With Icon/Purple", false, 0)]
        static void BBWICPU()
        {
            CreateObject("Button/Basic - With Icon/Purple");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - With Icon/Red", false, 0)]
        static void BBWICR()
        {
            CreateObject("Button/Basic - With Icon/Red");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline/Standard", false, 0)]
        static void BOWHS()
        {
            CreateObject("Button/Basic - Outline/Standard");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline/White", false, 0)]
        static void BOWHI()
        {
            CreateObject("Button/Basic - Outline/White");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline/Gray", false, 0)]
        static void BOGR()
        {
            CreateObject("Button/Basic - Outline/Gray");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline/Blue", false, 0)]
        static void BOBL()
        {
            CreateObject("Button/Basic - Outline/Blue");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline/Brown", false, 0)]
        static void BOBRW()
        {
            CreateObject("Button/Basic - Outline/Brown");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline/Green", false, 0)]
        static void BOGRE()
        {
            CreateObject("Button/Basic - Outline/Green");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline/Night", false, 0)]
        static void BONI()
        {
            CreateObject("Button/Basic - Outline/Night");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline/Orange", false, 0)]
        static void BOOR()
        {
            CreateObject("Button/Basic - Outline/Orange");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline/Pink", false, 0)]
        static void BOPIN()
        {
            CreateObject("Button/Basic Outline/Pink");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline/Purple", false, 0)]
        static void BOPURP()
        {
            CreateObject("Button/Basic - Outline/Purple");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline/Red", false, 0)]
        static void BORED()
        {
            CreateObject("Button/Basic - Outline/Red");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline Gradient/White", false, 0)]
        static void BOGWHI()
        {
            CreateObject("Button/Basic - Outline Gradient/White");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline Gradient/Gray", false, 0)]
        static void BOGBGR()
        {
            CreateObject("Button/Basic - Outline Gradient/Gray");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline Gradient/Blue", false, 0)]
        static void BOGBL()
        {
            CreateObject("Button/Basic - Outline Gradient/Blue");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline Gradient/Brown", false, 0)]
        static void BOGBRW()
        {
            CreateObject("Button/Basic - Outline Gradient/Brown");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline Gradient/Green", false, 0)]
        static void BOGGRE()
        {
            CreateObject("Button/Basic - Outline Gradient/Green");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline Gradient/Night", false, 0)]
        static void BOGNI()
        {
            CreateObject("Button/Basic - Outline Gradient/Night");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline Gradient/Orange", false, 0)]
        static void BOGOR()
        {
            CreateObject("Button/Basic - Outline Gradient/Orange");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline Gradient/Pink", false, 0)]
        static void BOGPIN()
        {
            CreateObject("Button/Basic - Outline Gradient/Pink");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline Gradient/Purple", false, 0)]
        static void BOGPURP()
        {
            CreateObject("Button/Basic - Outline Gradient/Purple");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline Gradient/Red", false, 0)]
        static void BOGRED()
        {
            CreateObject("Button/Basic - Outline Gradient/Red");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline Only Icon/Standard", false, 0)]
        static void BOOIS()
        {
            CreateObject("Button/Basic - Outline Only Icon/Standard");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline Only Icon/White", false, 0)]
        static void BOOIR()
        {
            CreateObject("Button/Basic - Outline Only Icon/White");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline Only Icon/Gray", false, 0)]
        static void BOOIG()
        {
            CreateObject("Button/Basic - Outline Only Icon/Gray");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline Only Icon/Blue", false, 0)]
        static void BOOIB()
        {
            CreateObject("Button/Basic - Outline Only Icon/Blue");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline Only Icon/Brown", false, 0)]
        static void BOOIBR()
        {
            CreateObject("Button/Basic - Outline Only Icon/Brown");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline Only Icon/Green", false, 0)]
        static void BOOIBG()
        {
            CreateObject("Button/Basic - Outline Only Icon/Green");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline Only Icon/Night", false, 0)]
        static void BOOIBN()
        {
            CreateObject("Button/Basic - Outline Only Icon/Night");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline Only Icon/Orange", false, 0)]
        static void BOOIBO()
        {
            CreateObject("Button/Basic - Outline Only Icon/Orange");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline Only Icon/Pink", false, 0)]
        static void BOOIBP()
        {
            CreateObject("Button/Basic - Outline Only Icon/Pink");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline Only Icon/Purple", false, 0)]
        static void BOOIBPU()
        {
            CreateObject("Button/Basic - Outline Only Icon/Purple");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline Only Icon/Red", false, 0)]
        static void BOOIBRE()
        {
            CreateObject("Button/Basic - Outline Only Icon/Red");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline With Icon/Standard", false, 0)]
        static void BOWIBS()
        {
            CreateObject("Button/Basic - Outline With Icon/Standard");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline With Icon/White", false, 0)]
        static void BOWIBW()
        {
            CreateObject("Button/Basic - Outline With Icon/White");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline With Icon/Gray", false, 0)]
        static void BOWIBG()
        {
            CreateObject("Button/Basic - Outline With Icon/Gray");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline With Icon/Blue", false, 0)]
        static void BOWIBB()
        {
            CreateObject("Button/Basic - Outline With Icon/Blue");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline With Icon/Brown", false, 0)]
        static void BOWIBBR()
        {
            CreateObject("Button/Basic - Outline With Icon/Brown");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline With Icon/Green", false, 0)]
        static void BOWIBGR()
        {
            CreateObject("Button/Basic - Outline With Icon/Green");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline With Icon/Night", false, 0)]
        static void BOWIBN()
        {
            CreateObject("Button/Basic - Outline With Icon/Night");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline With Icon/Orange", false, 0)]
        static void BOWIBO()
        {
            CreateObject("Button/Basic - Outline With Icon/Orange");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline With Icon/Pink", false, 0)]
        static void BOWIBP()
        {
            CreateObject("Button/Basic - Outline With Icon/Pink");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline With Icon/Purple", false, 0)]
        static void BOWIBPU()
        {
            CreateObject("Button/Basic - Outline With Icon/Purple");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Basic - Outline With Icon/Red", false, 0)]
        static void BOWIBRE()
        {
            CreateObject("Button/Basic - Outline With Icon/Red");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Radial - Only Icon/Standard", false, 0)]
        static void BROIS()
        {
            CreateObject("Button/Radial - Only Icon/Standard");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Radial - Only Icon/White", false, 0)]
        static void BROIW()
        {
            CreateObject("Button/Radial - Only Icon/White");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Radial - Only Icon/Gray", false, 0)]
        static void BROIG()
        {
            CreateObject("Button/Radial - Only Icon/Gray");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Radial - Only Icon/Blue", false, 0)]
        static void BROIB()
        {
            CreateObject("Button/Radial - Only Icon/Blue");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Radial - Only Icon/Brown", false, 0)]
        static void BROIBR()
        {
            CreateObject("Button/Radial - Only Icon/Brown");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Radial - Only Icon/Green", false, 0)]
        static void BROIGR()
        {
            CreateObject("Button/Radial - Only Icon/Green");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Radial - Only Icon/Night", false, 0)]
        static void BROIN()
        {
            CreateObject("Button/Radial - Only Icon/Night");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Radial - Only Icon/Orange", false, 0)]
        static void BROIO()
        {
            CreateObject("Button/Radial - Only Icon/Orange");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Radial - Only Icon/Pink", false, 0)]
        static void BROIP()
        {
            CreateObject("Button/Radial - Only Icon/Pink");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Radial - Only Icon/Purple", false, 0)]
        static void BROIPU()
        {
            CreateObject("Button/Radial - Only Icon/Purple");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Radial - Only Icon/Red", false, 0)]
        static void BROIR()
        {
            CreateObject("Button/Radial - Only Icon/Red");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Radial - Outline Only Icon/Standard", false, 0)]
        static void BROOIS()
        {
            CreateObject("Button/Radial - Outline Only Icon/Standard");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Radial - Outline Only Icon/White", false, 0)]
        static void BROOIW()
        {
            CreateObject("Button/Radial - Outline Only Icon/White");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Radial - Outline Only Icon/Gray", false, 0)]
        static void BROOIG()
        {
            CreateObject("Button/Radial - Outline Only Icon/Gray");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Radial - Outline Only Icon/Blue", false, 0)]
        static void BROOIB()
        {
            CreateObject("Button/Radial - Outline Only Icon/Blue");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Radial - Outline Only Icon/Brown", false, 0)]
        static void BROOIBR()
        {
            CreateObject("Button/Radial - Outline Only Icon/Brown");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Radial - Outline Only Icon/Green", false, 0)]
        static void BROOIGR()
        {
            CreateObject("Button/Radial - Outline Only Icon/Green");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Radial - Outline Only Icon/Night", false, 0)]
        static void BROOIN()
        {
            CreateObject("Button/Radial - Outline Only Icon/Night");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Radial - Outline Only Icon/Orange", false, 0)]
        static void BROOIO()
        {
            CreateObject("Button/Radial - Outline Only Icon/Orange");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Radial - Outline Only Icon/Pink", false, 0)]
        static void BROOIP()
        {
            CreateObject("Button/Radial - Outline Only Icon/Pink");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Radial - Outline Only Icon/Purple", false, 0)]
        static void BROOIPU()
        {
            CreateObject("Button/Radial - Outline Only Icon/Purple");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Radial - Outline Only Icon/Red", false, 0)]
        static void BROOIR()
        {
            CreateObject("Button/Radial - Outline Only Icon/Red");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded/Standard", false, 0)]
        static void BRS()
        {
            CreateObject("Button/Rounded/Standard");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded/White", false, 0)]
        static void BRW()
        {
            CreateObject("Button/Rounded/White");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded/Gray", false, 0)]
        static void BRG()
        {
            CreateObject("Button/Rounded/Gray");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded/Blue", false, 0)]
        static void BRB()
        {
            CreateObject("Button/Rounded/Blue");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded/Brown", false, 0)]
        static void BRBR()
        {
            CreateObject("Button/Rounded/Brown");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded/Green", false, 0)]
        static void BRGR()
        {
            CreateObject("Button/Rounded/Green");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded/Night", false, 0)]
        static void BRN()
        {
            CreateObject("Button/Rounded/Night");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded/Orange", false, 0)]
        static void BRO()
        {
            CreateObject("Button/Rounded/Orange");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded/Pink", false, 0)]
        static void BRP()
        {
            CreateObject("Button/Rounded/Pink");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded/Purple", false, 0)]
        static void BRPU()
        {
            CreateObject("Button/Rounded/Purple");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded/Red", false, 0)]
        static void BRR()
        {
            CreateObject("Button/Rounded/Red");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded - Gradient/White", false, 0)]
        static void BRGW()
        {
            CreateObject("Button/Rounded - Gradient/White");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded - Gradient/Gray", false, 0)]
        static void BRGG()
        {
            CreateObject("Button/Rounded - Gradient/Gray");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded - Gradient/Blue", false, 0)]
        static void BRGB()
        {
            CreateObject("Button/Rounded - Gradient/Blue");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded - Gradient/Brown", false, 0)]
        static void BRGBR()
        {
            CreateObject("Button/Rounded - Gradient/Brown");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded - Gradient/Green", false, 0)]
        static void BRGGR()
        {
            CreateObject("Button/Rounded - Gradient/Green");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded - Gradient/Night", false, 0)]
        static void BRGN()
        {
            CreateObject("Button/Rounded - Gradient/Night");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded - Gradient/Orange", false, 0)]
        static void BRGO()
        {
            CreateObject("Button/Rounded - Gradient/Orange");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded - Gradient/Pink", false, 0)]
        static void BRGP()
        {
            CreateObject("Button/Rounded - Gradient/Pink");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded - Gradient/Purple", false, 0)]
        static void BRGPU()
        {
            CreateObject("Button/Rounded - Gradient/Purple");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded - Gradient/Red", false, 0)]
        static void BRGRE()
        {
            CreateObject("Button/Rounded - Gradient/Red");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded - Outline/Standard", false, 0)]
        static void BROS()
        {
            CreateObject("Button/Rounded - Outline/Standard");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded - Outline/White", false, 0)]
        static void BROW()
        {
            CreateObject("Button/Rounded - Outline/White");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded - Outline/Gray", false, 0)]
        static void BROG()
        {
            CreateObject("Button/Rounded - Outline/Gray");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded - Outline/Blue", false, 0)]
        static void BROB()
        {
            CreateObject("Button/Rounded - Outline/Blue");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded - Outline/Brown", false, 0)]
        static void BROBR()
        {
            CreateObject("Button/Rounded - Outline/Brown");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded - Outline/Green", false, 0)]
        static void BROGR()
        {
            CreateObject("Button/Rounded - Outline/Green");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded - Outline/Night", false, 0)]
        static void BRON()
        {
            CreateObject("Button/Rounded - Outline/Night");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded - Outline/Orange", false, 0)]
        static void BROO()
        {
            CreateObject("Button/Rounded - Outline/Orange");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded - Outline/Pink", false, 0)]
        static void BROP()
        {
            CreateObject("Button/Rounded - Outline/Pink");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded - Outline/Purple", false, 0)]
        static void BROPU()
        {
            CreateObject("Button/Rounded - Outline/Purple");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded - Outline/Red", false, 0)]
        static void BRORE()
        {
            CreateObject("Button/Rounded - Outline/Red");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded - Outline Gradient/White", false, 0)]
        static void BROGW()
        {
            CreateObject("Button/Rounded - Outline Gradient/White");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded - Outline Gradient/Gray", false, 0)]
        static void BROGG()
        {
            CreateObject("Button/Rounded - Outline Gradient/Gray");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded - Outline Gradient/Blue", false, 0)]
        static void BROGB()
        {
            CreateObject("Button/Rounded - Outline Gradient/Blue");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded - Outline Gradient/Brown", false, 0)]
        static void BROGBR()
        {
            CreateObject("Button/Rounded - Outline Gradient/Brown");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded - Outline Gradient/Green", false, 0)]
        static void BROGGR()
        {
            CreateObject("Button/Rounded - Outline Gradient/Green");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded - Outline Gradient/Night", false, 0)]
        static void BROGN()
        {
            CreateObject("Button/Rounded - Outline Gradient/Night");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded - Outline Gradient/Orange", false, 0)]
        static void BROGO()
        {
            CreateObject("Button/Rounded - Outline Gradient/Orange");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded - Outline Gradient/Pink", false, 0)]
        static void BROGP()
        {
            CreateObject("Button/Rounded - Outline Gradient/Pink");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded - Outline Gradient/Purple", false, 0)]
        static void BROGPU()
        {
            CreateObject("Button/Rounded - Outline Gradient/Purple");
        }

        [MenuItem("GameObject/Modern UI Pack/Button/Rounded - Outline Gradient/Red", false, 0)]
        static void BROGRE()
        {
            CreateObject("Button/Rounded - Outline Gradient/Red");
        }

        [MenuItem("GameObject/Modern UI Pack/Dropdown/Standard", false, 0)]
        static void DSD()
        {
            CreateObject("Dropdown/Standard");
        }

        [MenuItem("GameObject/Modern UI Pack/Dropdown/Multi Select", false, 0)]
        static void DMSD()
        {
            CreateObject("Dropdown/Multi Select");
        }

        [MenuItem("GameObject/Modern UI Pack/Horizontal Selector/Standard", false, 0)]
        static void HSS()
        {
            CreateObject("Horizontal Selector/Standard");
        }

        [MenuItem("GameObject/Modern UI Pack/Input Field/Multi-Line", false, 0)]
        static void IFFML()
        {
            CreateObject("Input Field/Multi-Line");
        }

        [MenuItem("GameObject/Modern UI Pack/Input Field/Fading (Left Aligned)", false, 0)]
        static void IFFLA()
        {
            CreateObject("Input Field/Fading (Left Aligned)");
        }

        [MenuItem("GameObject/Modern UI Pack/Input Field/Fading (Middle Aligned)", false, 0)]
        static void IFFMA()
        {
            CreateObject("Input Field/Fading (Middle Aligned)");
        }

        [MenuItem("GameObject/Modern UI Pack/Input Field/Fading (Right Aligned)", false, 0)]
        static void IFFRA()
        {
            CreateObject("Input Field/Fading (Right Aligned)");
        }

        [MenuItem("GameObject/Modern UI Pack/Input Field/Standard (Left Aligned)", false, 0)]
        static void IFSLA()
        {
            CreateObject("Input Field/Standard (Left Aligned)");
        }

        [MenuItem("GameObject/Modern UI Pack/Input Field/Standard (Middle Aligned)", false, 0)]
        static void IFSMA()
        {
            CreateObject("Input Field/Standard (Middle Aligned)");
        }

        [MenuItem("GameObject/Modern UI Pack/Input Field/Standard (Right Aligned)", false, 0)]
        static void IFSRA()
        {
            CreateObject("Input Field/Standard (Right Aligned)");
        }

        [MenuItem("GameObject/Modern UI Pack/List View/Standard", false, 0)]
        static void LVS()
        {
            CreateObject("List View/Standard");
        }

        [MenuItem("GameObject/Modern UI Pack/Modal Window/Style 1/Standard", false, 0)]
        static void MWSS()
        {
            CreateObject("Modal Window/Style 1/Standard");
        }

        [MenuItem("GameObject/Modern UI Pack/Modal Window/Style 1/With Tabs", false, 0)]
        static void MWSWT()
        {
            CreateObject("Modal Window/Style 1/With Tabs");
        }

        [MenuItem("GameObject/Modern UI Pack/Modal Window/Style 2/Standard", false, 0)]
        static void MWSSS()
        {
            CreateObject("Modal Window/Style 2/Standard");
        }

        [MenuItem("GameObject/Modern UI Pack/Modal Window/Style 2/With Tabs", false, 0)]
        static void MWSSWT()
        {
            CreateObject("Modal Window/Style 2/With Tabs");
        }

        [MenuItem("GameObject/Modern UI Pack/Notification/Fading Notification", false, 0)]
        static void NSN()
        {
            CreateObject("Notification/Fading Notification");
        }

        [MenuItem("GameObject/Modern UI Pack/Notification/Popup Notification", false, 0)]
        static void NSP()
        {
            CreateObject("Notification/Popup Notification");
        }

        [MenuItem("GameObject/Modern UI Pack/Notification/Sliding Notification", false, 0)]
        static void NSS()
        {
            CreateObject("Notification/Sliding Notification");
        }

        [MenuItem("GameObject/Modern UI Pack/Progress Bar/Standard", false, 0)]
        static void PBS()
        {
            CreateObject("Progress Bar/Standard");
        }

        [MenuItem("GameObject/Modern UI Pack/Progress Bar/Radial Standard", false, 0)]
        static void PBRST()
        {
            CreateObject("Progress Bar/Radial Standard");
        }

        [MenuItem("GameObject/Modern UI Pack/Progress Bar/Radial Thin", false, 0)]
        static void PBRT()
        {
            CreateObject("Progress Bar/Radial Thin");
        }

        [MenuItem("GameObject/Modern UI Pack/Progress Bar/Radial Light", false, 0)]
        static void PBRL()
        {
            CreateObject("Progress Bar/Radial Light");
        }

        [MenuItem("GameObject/Modern UI Pack/Progress Bar/Radial Regular", false, 0)]
        static void PBRR()
        {
            CreateObject("Progress Bar/Radial Regular");
        }

        [MenuItem("GameObject/Modern UI Pack/Progress Bar/Radial Bold", false, 0)]
        static void PBRB()
        {
            CreateObject("Progress Bar/Radial Bold");
        }

        [MenuItem("GameObject/Modern UI Pack/Progress Bar/Radial Filled Horizontal", false, 0)]
        static void PBRFH()
        {
            CreateObject("Progress Bar/Radial Filled Horizontal");
        }

        [MenuItem("GameObject/Modern UI Pack/Progress Bar/Radial Filled Vertical", false, 0)]
        static void PBRFV()
        {
            CreateObject("Progress Bar/Radial Filled Vertical");
        }

        [MenuItem("GameObject/Modern UI Pack/Progress Bar (Loop)/Standard Fastly", false, 0)]
        static void PBLSF()
        {
            CreateObject("Progress Bar (Loop)/Standard Fastly");
        }

        [MenuItem("GameObject/Modern UI Pack/Progress Bar (Loop)/Standard Run", false, 0)]
        static void PBLSR()
        {
            CreateObject("Progress Bar (Loop)/Standard Run");
        }

        [MenuItem("GameObject/Modern UI Pack/Progress Bar (Loop)/Radial Material", false, 0)]
        static void PBLRM()
        {
            CreateObject("Progress Bar (Loop)/Radial Material");
        }

        [MenuItem("GameObject/Modern UI Pack/Progress Bar (Loop)/Radial Pie", false, 0)]
        static void PBLRP()
        {
            CreateObject("Progress Bar (Loop)/Radial Pie");
        }

        [MenuItem("GameObject/Modern UI Pack/Progress Bar (Loop)/Radial Run", false, 0)]
        static void PBLRR()
        {
            CreateObject("Progress Bar (Loop)/Radial Run");
        }

        [MenuItem("GameObject/Modern UI Pack/Progress Bar (Loop)/Radial Trapez", false, 0)]
        static void PBLRT()
        {
            CreateObject("Progress Bar (Loop)/Radial Trapez");
        }

        [MenuItem("GameObject/Modern UI Pack/Scrollbar/Standard", false, 0)]
        static void SCS()
        {
            CreateObject("Scrollbar/Standard");
        }

        [MenuItem("GameObject/Modern UI Pack/Slider/Standard/Standard", false, 0)]
        static void SLS()
        {
            CreateObject("Slider/Standard/Standard");
        }

        [MenuItem("GameObject/Modern UI Pack/Slider/Standard/Standard (Popup)", false, 0)]
        static void SLSP()
        {
            CreateObject("Slider/Standard/Standard (Popup)");
        }

        [MenuItem("GameObject/Modern UI Pack/Slider/Standard/Standard (Value)", false, 0)]
        static void SLSV()
        {
            CreateObject("Slider/Standard/Standard (Value)");
        }

        [MenuItem("GameObject/Modern UI Pack/Slider/Gradient/Gradient", false, 0)]
        static void SLG()
        {
            CreateObject("Slider/Gradient/Gradient");
        }

        [MenuItem("GameObject/Modern UI Pack/Slider/Gradient/Gradient (Popup)", false, 0)]
        static void SLGP()
        {
            CreateObject("Slider/Gradient/Gradient (Popup)");
        }

        [MenuItem("GameObject/Modern UI Pack/Slider/Gradient/Gradient (Value)", false, 0)]
        static void SLGV()
        {
            CreateObject("Slider/Gradient/Gradient (Value)");
        }

        [MenuItem("GameObject/Modern UI Pack/Slider/Outline/Outline", false, 0)]
        static void SLO()
        {
            CreateObject("Slider/Outline/Outline");
        }

        [MenuItem("GameObject/Modern UI Pack/Slider/Outline/Outline (Popup)", false, 0)]
        static void SLOP()
        {
            CreateObject("Slider/Outline/Outline (Popup)");
        }

        [MenuItem("GameObject/Modern UI Pack/Slider/Outline/Outline (Value)", false, 0)]
        static void SLOV()
        {
            CreateObject("Slider/Outline/Outline (Value)");
        }

        [MenuItem("GameObject/Modern UI Pack/Slider/Radial/Radial", false, 0)]
        static void SLR()
        {
            CreateObject("Slider/Radial/Radial");
        }

        [MenuItem("GameObject/Modern UI Pack/Slider/Radial/Radial (Gradient)", false, 0)]
        static void SLRG()
        {
            CreateObject("Slider/Radial/Radial (Gradient)");
        }

        [MenuItem("GameObject/Modern UI Pack/Slider/Range/Range", false, 0)]
        static void SLRA()
        {
            CreateObject("Slider/Range/Range");
        }

        [MenuItem("GameObject/Modern UI Pack/Slider/Range/Range (Label)", false, 0)]
        static void SLRAL()
        {
            CreateObject("Slider/Range/Range (Label)");
        }

        [MenuItem("GameObject/Modern UI Pack/Switch/Standard", false, 0)]
        static void SWS()
        {
            CreateObject("Switch/Standard");
        }

        [MenuItem("GameObject/Modern UI Pack/Switch/Material", false, 0)]
        static void SWM()
        {
            CreateObject("Switch/Material");
        }

        [MenuItem("GameObject/Modern UI Pack/Toggle/Standard", false, 0)]
        static void TSTST()
        {
            CreateObject("Toggle/Standard");
        }

        [MenuItem("GameObject/Modern UI Pack/Toggle/Standard (Light)", false, 0)]
        static void TSTL()
        {
            CreateObject("Toggle/Standard (Light)");
        }

        [MenuItem("GameObject/Modern UI Pack/Toggle/Standard (Regular)", false, 0)]
        static void TSTR()
        {
            CreateObject("Toggle/Standard (Regular)");
        }

        [MenuItem("GameObject/Modern UI Pack/Toggle/Standard (Bold)", false, 0)]
        static void TSTB()
        {
            CreateObject("Toggle/Standard (Bold)");
        }

        [MenuItem("GameObject/Modern UI Pack/Toggle/Toggle Group Panel", false, 0)]
        static void TTGP()
        {
            CreateObject("Toggle/Toggle Group Panel");
        }

        [MenuItem("GameObject/Modern UI Pack/Tooltip/Tooltip System", false, 0)]
        static void TTS()
        {
            CreateObject("Tooltip/Tooltip");
        }
    }
}