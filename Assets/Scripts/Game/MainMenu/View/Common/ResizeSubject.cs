using R3;
using UnityEngine;

namespace Game.MainMenu.View.Common
{
    /**
     * Для передачи в родительский объект события изменения размеров
     */
    public class ResizeSubject : MonoBehaviour
    {
        public Subject<Unit> OnResize = new();
        
    }
}