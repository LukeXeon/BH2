using System;

namespace Dash.Scripts.UIManager
{
    public interface IWebUIManager
    {
        void Initialize(string url, Action back);
    }
}