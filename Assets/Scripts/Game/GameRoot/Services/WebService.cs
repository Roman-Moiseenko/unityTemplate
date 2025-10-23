using System.Net;
using Game.Common;
using UnityEngine.Networking;

namespace Game.GameRoot.Services
{
    public class WebService
    {
        public bool FirstAuthorization(string userId)
        {
            var request = new UnityWebRequest(WebConstants.WEB_AUTH);
            //WebRequest(request, null, null);
            return true;
        }
        
        //public void Load
    }
}