using System;

namespace Dash.Scripts.UIManager
{
    public interface IWebUIManager
    {
        void Init(string url, Action back);
    }
}